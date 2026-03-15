using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using Voltline.Audio;
using Voltline.Core;
using Voltline.Data;
using Voltline.Save;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Voltline.UI
{
    public sealed class MainMenuView : MonoBehaviour
    {
        [SerializeField] private ThemeCatalog themeCatalog;
        [SerializeField] private PlayerVisualConfig playerVisualConfig;
        [SerializeField] private ObstacleVisualCatalog obstacleVisualCatalog;
        [SerializeField] private AudioCueCatalog audioCueCatalog;
        [SerializeField] private AudioMixer audioMixer;

        private SaveService saveService;
        private SettingsOverlayView settingsOverlayView;
        private TMP_Text bestScoreText;

        public bool IsSettingsVisible => settingsOverlayView != null && settingsOverlayView.IsVisible;

        private void Awake()
        {
            if (themeCatalog == null || themeCatalog.DefaultTheme == null || playerVisualConfig == null || obstacleVisualCatalog == null || audioCueCatalog == null)
            {
                Debug.LogError("MainMenuView is missing a valid ThemeCatalog, PlayerVisualConfig, ObstacleVisualCatalog, or AudioCueCatalog reference.");
                enabled = false;
                return;
            }

            saveService = SaveService.EnsureExists();
            saveService.SynchronizeThemeUnlocks(themeCatalog);
            ThemeConfig activeTheme = saveService.ResolveSelectedTheme(themeCatalog) ?? themeCatalog.DefaultTheme;
            AudioService audioService = AudioService.EnsureExists();
            audioService.Configure(audioCueCatalog, saveService, audioMixer);
            audioService.PlayMusicLoop(AudioCueIds.MainLoop);

            BuildInterface(activeTheme);
            RefreshBestScore();
            saveService.ProfileChanged += RefreshBestScore;
        }

        private void OnDestroy()
        {
            if (saveService != null)
            {
                saveService.ProfileChanged -= RefreshBestScore;
            }
        }

        public void HandlePlayPressed()
        {
            SceneManager.LoadScene(SceneCatalog.Gameplay, LoadSceneMode.Single);
        }

        public void HandleOpenSettingsPressed()
        {
            settingsOverlayView.Show();
        }

        public void HandleCloseSettingsPressed()
        {
            settingsOverlayView.Hide();
        }

        private void BuildInterface(ThemeConfig activeTheme)
        {
            Camera mainCamera = Camera.main ?? FindAnyObjectByType<Camera>();
            if (mainCamera != null)
            {
                mainCamera.backgroundColor = Color.Lerp(activeTheme.BackgroundTopColor, activeTheme.BackgroundBottomColor, 0.5f);
            }

            UIFactory.EnsureEventSystem();
            Canvas canvas = UIFactory.CreateCanvas("MainMenuCanvas", transform);
            RectTransform safeAreaRoot = UIFactory.CreateSafeAreaRoot(canvas);

            RectTransform topBackground = UIFactory.CreatePanel("TopBackground", safeAreaRoot, activeTheme.BackgroundTopColor);
            topBackground.anchorMin = new Vector2(0f, 0.48f);
            topBackground.anchorMax = new Vector2(1f, 1f);
            topBackground.offsetMin = Vector2.zero;
            topBackground.offsetMax = Vector2.zero;

            RectTransform bottomBackground = UIFactory.CreatePanel("BottomBackground", safeAreaRoot, activeTheme.BackgroundBottomColor);
            bottomBackground.anchorMin = new Vector2(0f, 0f);
            bottomBackground.anchorMax = new Vector2(1f, 0.52f);
            bottomBackground.offsetMin = Vector2.zero;
            bottomBackground.offsetMax = Vector2.zero;

            TMP_Text titleText = UIFactory.CreateText("Title", safeAreaRoot, "STAY ON THE LINE", 68, FontStyles.Bold, TextAlignmentOptions.Center, Color.white);
            UIFactory.SetAnchors((RectTransform)titleText.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -92f), new Vector2(920f, 104f));

            bestScoreText = UIFactory.CreateText("BestScore", safeAreaRoot, "Best 0", 30, FontStyles.Normal, TextAlignmentOptions.Center, activeTheme.PlayerAccentColor);
            UIFactory.SetAnchors((RectTransform)bestScoreText.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -162f), new Vector2(340f, 46f));

            RectTransform previewRoot = new GameObject("PreviewView", typeof(RectTransform)).GetComponent<RectTransform>();
            previewRoot.SetParent(safeAreaRoot, false);
            MainMenuPreviewView previewView = previewRoot.gameObject.AddComponent<MainMenuPreviewView>();
            previewView.Initialize(previewRoot, activeTheme, playerVisualConfig, obstacleVisualCatalog);

            TMP_Text hintText = UIFactory.CreateText("Hint", safeAreaRoot, "Tap to flip sides", 28, FontStyles.Normal, TextAlignmentOptions.Center, new Color(0.9f, 0.94f, 1f, 1f));
            UIFactory.SetAnchors((RectTransform)hintText.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -390f), new Vector2(460f, 44f));

            UnityEngine.UI.Button playButton = UIFactory.CreateButton("PlayButton", safeAreaRoot, "Play", UIFactory.AccentColor(activeTheme), new Color(0.05f, 0.08f, 0.12f, 1f), HandlePlayPressed);
            UIFactory.SetAnchors((RectTransform)playButton.transform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 220f), new Vector2(540f, 118f));

            UnityEngine.UI.Button settingsButton = UIFactory.CreateButton("SettingsButton", safeAreaRoot, "Settings", UIFactory.PanelColor(1f), Color.white, HandleOpenSettingsPressed);
            UIFactory.SetAnchors((RectTransform)settingsButton.transform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 78f), new Vector2(540f, 84f));

            settingsOverlayView = gameObject.AddComponent<SettingsOverlayView>();
            settingsOverlayView.Initialize(safeAreaRoot, activeTheme, themeCatalog, saveService, HandleCloseSettingsPressed);
        }

        private void RefreshBestScore()
        {
            if (bestScoreText != null && saveService != null)
            {
                bestScoreText.text = $"Best {saveService.BestScore}";
            }
        }

#if UNITY_EDITOR
        private void Reset()
        {
            AssignDefaults();
        }

        private void OnValidate()
        {
            AssignDefaults();
        }

        private void AssignDefaults()
        {
            themeCatalog ??= AssetDatabase.LoadAssetAtPath<ThemeCatalog>(ProjectConfigAssetPaths.ThemeCatalog);
            playerVisualConfig ??= AssetDatabase.LoadAssetAtPath<PlayerVisualConfig>(ProjectConfigAssetPaths.PlayerVisualConfig);
            obstacleVisualCatalog ??= AssetDatabase.LoadAssetAtPath<ObstacleVisualCatalog>(ProjectConfigAssetPaths.ObstacleVisualCatalog);
            audioCueCatalog ??= AssetDatabase.LoadAssetAtPath<AudioCueCatalog>(ProjectConfigAssetPaths.AudioCueCatalog);
            audioMixer ??= AssetDatabase.LoadAssetAtPath<AudioMixer>(ProjectConfigAssetPaths.AudioMixer);
        }
#endif
    }
}
