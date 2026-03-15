using UnityEngine;
using Voltline.Data;

namespace Voltline.Save
{
    public sealed class SaveService : MonoBehaviour
    {
        private static SaveService instance;

        [SerializeField] private PlayerProfileSaveData currentProfile;

        public static SaveService Instance => EnsureExists();

        public PlayerProfileSaveData CurrentProfile => currentProfile;
        public int BestScore => currentProfile != null ? currentProfile.bestScore : 0;
        public float MusicVolume => currentProfile != null ? currentProfile.musicVolume : 1f;
        public float SfxVolume => currentProfile != null ? currentProfile.sfxVolume : 1f;
        public bool VibrationEnabled => currentProfile == null || currentProfile.vibrationEnabled;
        public string SelectedThemeId => currentProfile != null ? currentProfile.selectedThemeId : SaveSchema.DefaultThemeId;

        public event System.Action ProfileChanged;

        public static SaveService EnsureExists()
        {
            if (instance != null)
            {
                return instance;
            }

            instance = FindFirstObjectByType<SaveService>();
            if (instance != null)
            {
                instance.InitializeIfNeeded();
                return instance;
            }

            GameObject root = new("SaveService");
            instance = root.AddComponent<SaveService>();
            instance.InitializeIfNeeded();
            return instance;
        }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            InitializeIfNeeded();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                PersistProfile();
            }
        }

        private void OnApplicationQuit()
        {
            PersistProfile();
        }

        public void RecordRunScore(int score)
        {
            int clampedScore = Mathf.Max(0, score);
            if (currentProfile.bestScore >= clampedScore)
            {
                return;
            }

            currentProfile.bestScore = clampedScore;
            NotifyProfileChanged(true);
        }

        public bool SynchronizeThemeUnlocks(ThemeCatalog catalog)
        {
            if (catalog == null || currentProfile == null)
            {
                return false;
            }

            currentProfile.unlockedThemeIds ??= new System.Collections.Generic.List<string>();
            bool changed = false;
            for (int i = 0; i < catalog.Themes.Count; i++)
            {
                ThemeConfig theme = catalog.Themes[i];
                if (theme == null)
                {
                    continue;
                }

                bool shouldUnlock = theme.UnlockedByDefault || currentProfile.bestScore >= theme.UnlockBestScoreThreshold;
                if (shouldUnlock && !currentProfile.unlockedThemeIds.Contains(theme.ThemeId))
                {
                    currentProfile.unlockedThemeIds.Add(theme.ThemeId);
                    changed = true;
                }
            }

            if (!catalog.TryGetTheme(currentProfile.selectedThemeId, out ThemeConfig selectedTheme) || !IsThemeUnlocked(currentProfile.selectedThemeId))
            {
                ThemeConfig fallback = catalog.DefaultTheme;
                for (int i = 0; i < catalog.Themes.Count; i++)
                {
                    ThemeConfig candidate = catalog.Themes[i];
                    if (candidate != null && IsThemeUnlocked(candidate.ThemeId))
                    {
                        fallback = candidate;
                        break;
                    }
                }

                string fallbackThemeId = fallback != null ? fallback.ThemeId : SaveSchema.DefaultThemeId;
                if (currentProfile.selectedThemeId != fallbackThemeId)
                {
                    currentProfile.selectedThemeId = fallbackThemeId;
                    changed = true;
                }
            }

            if (changed)
            {
                NotifyProfileChanged(true);
            }

            return changed;
        }

        public void SetMusicVolume(float value)
        {
            float clamped = Mathf.Clamp01(value);
            if (Mathf.Approximately(currentProfile.musicVolume, clamped))
            {
                return;
            }

            currentProfile.musicVolume = clamped;
            NotifyProfileChanged(true);
        }

        public void SetSfxVolume(float value)
        {
            float clamped = Mathf.Clamp01(value);
            if (Mathf.Approximately(currentProfile.sfxVolume, clamped))
            {
                return;
            }

            currentProfile.sfxVolume = clamped;
            NotifyProfileChanged(true);
        }

        public void SetVibrationEnabled(bool enabled)
        {
            if (currentProfile.vibrationEnabled == enabled)
            {
                return;
            }

            currentProfile.vibrationEnabled = enabled;
            NotifyProfileChanged(true);
        }

        public void SetSelectedThemeId(string themeId)
        {
            if (string.IsNullOrWhiteSpace(themeId) || currentProfile.selectedThemeId == themeId || !IsThemeUnlocked(themeId))
            {
                return;
            }

            currentProfile.selectedThemeId = themeId;
            NotifyProfileChanged(true);
        }

        public ThemeConfig ResolveSelectedTheme(ThemeCatalog catalog)
        {
            return catalog != null ? catalog.ResolveThemeOrDefault(SelectedThemeId) : null;
        }

        public bool IsThemeUnlocked(string themeId)
        {
            return !string.IsNullOrWhiteSpace(themeId)
                && currentProfile != null
                && currentProfile.unlockedThemeIds != null
                && currentProfile.unlockedThemeIds.Contains(themeId);
        }

        public void ForceReloadFromDiskForTests()
        {
            currentProfile = LoadOrCreateProfile();
            NotifyProfileChanged(false);
        }

        public static void ResetInstanceForTests()
        {
            if (instance == null)
            {
                return;
            }

            if (Application.isPlaying)
            {
                Object.Destroy(instance.gameObject);
            }
            else
            {
#if UNITY_EDITOR
                Object.DestroyImmediate(instance.gameObject);
#else
                Object.Destroy(instance.gameObject);
#endif
            }

            instance = null;
        }

        private void InitializeIfNeeded()
        {
            if (Application.isPlaying)
            {
                DontDestroyOnLoad(gameObject);
            }

            if (currentProfile == null)
            {
                currentProfile = LoadOrCreateProfile();
            }
        }

        private PlayerProfileSaveData LoadOrCreateProfile()
        {
            PlayerProfileSaveData loadedProfile = null;
            if (SaveStorage.TryReadProfile(out string json))
            {
                try
                {
                    SaveFileEnvelope envelope = JsonUtility.FromJson<SaveFileEnvelope>(json);
                    loadedProfile = envelope != null ? envelope.profile : null;
                }
                catch (System.Exception exception)
                {
                    Debug.LogWarning($"Voltline failed to parse save profile. A fresh profile will be created. {exception.Message}");
                }
            }

            PlayerProfileSaveData profile = SaveSchema.UpgradeToCurrent(loadedProfile, SaveSchema.DefaultThemeId);
            PersistProfile(profile);
            return profile;
        }

        private void NotifyProfileChanged(bool persist)
        {
            if (persist)
            {
                PersistProfile();
            }

            ProfileChanged?.Invoke();
        }

        private void PersistProfile()
        {
            PersistProfile(currentProfile);
        }

        private static void PersistProfile(PlayerProfileSaveData profile)
        {
            if (profile == null)
            {
                return;
            }

            SaveFileEnvelope envelope = new() { profile = profile };
            string json = JsonUtility.ToJson(envelope, true);
            SaveStorage.WriteProfile(json);
        }
    }
}


