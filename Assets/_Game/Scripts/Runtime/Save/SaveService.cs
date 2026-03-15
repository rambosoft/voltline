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

        public void RecordRunScore(int score)
        {
            if (score > currentProfile.bestScore)
            {
                currentProfile.bestScore = score;
            }

            ProfileChanged?.Invoke();
        }

        public void SetMusicVolume(float value)
        {
            currentProfile.musicVolume = Mathf.Clamp01(value);
            ProfileChanged?.Invoke();
        }

        public void SetSfxVolume(float value)
        {
            currentProfile.sfxVolume = Mathf.Clamp01(value);
            ProfileChanged?.Invoke();
        }

        public void SetVibrationEnabled(bool enabled)
        {
            currentProfile.vibrationEnabled = enabled;
            ProfileChanged?.Invoke();
        }

        private void InitializeIfNeeded()
        {
            DontDestroyOnLoad(gameObject);

            if (currentProfile != null)
            {
                return;
            }

            ThemeCatalog themeCatalog = ResourcesSafeLoad<ThemeCatalog>(ProjectConfigAssetPaths.ThemeCatalog);
            string defaultThemeId = themeCatalog != null ? themeCatalog.DefaultThemeId : "theme.neon-night";
            currentProfile = SaveSchema.CreateDefaultProfile(defaultThemeId);
        }

        private static T ResourcesSafeLoad<T>(string path) where T : UnityEngine.Object
        {
#if UNITY_EDITOR
            return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
#else
            return null;
#endif
        }
    }
}