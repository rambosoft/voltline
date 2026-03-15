using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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

        private SaveService saveService;
        private SettingsOverlayView settingsOverlayView;
        private TMP_Text bestScoreText;

        public bool IsSettingsVisible => settingsOverlayView != null && settingsOverlayView.IsVisible;

        private void Awake()
        {
            if (themeCatalog == null || themeCatalog.DefaultTheme == null)
            {
                Debug.LogError("MainMenuView is missing a valid ThemeCatalog reference.");
                enabled = false;
                return;
            }

            saveService = SaveService.EnsureExists();
            BuildInterface(themeCatalog.DefaultTheme);
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
            topBackground.anchorMin = new Vector2(0f, 0.5f);
            topBackground.anchorMax = new Vector2(1f, 1f);
            topBackground.offsetMin = Vector2.zero;
            topBackground.offsetMax = Vector2.zero;

            RectTransform bottomBackground = UIFactory.CreatePanel("BottomBackground", safeAreaRoot, activeTheme.BackgroundBottomColor);
            bottomBackground.anchorMin = new Vector2(0f, 0f);
            bottomBackground.anchorMax = new Vector2(1f, 0.5f);
            bottomBackground.offsetMin = Vector2.zero;
            bottomBackground.offsetMax = Vector2.zero;

            TMP_Text titleText = UIFactory.CreateText("Title", safeAreaRoot, "STAY ON THE LINE", 64, FontStyles.Bold, TextAlignmentOptions.Center, Color.white);
            UIFactory.SetAnchors((RectTransform)titleText.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -92f), new Vector2(900f, 100f));

            bestScoreText = UIFactory.CreateText("BestScore", safeAreaRoot, "Best 0", 28, FontStyles.Normal, TextAlignmentOptions.Center, activeTheme.PlayerAccentColor);
            UIFactory.SetAnchors((RectTransform)bestScoreText.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -162f), new Vector2(320f, 44f));

            RectTransform previewRoot = new GameObject("PreviewView", typeof(RectTransform)).GetComponent<RectTransform>();
            previewRoot.SetParent(safeAreaRoot, false);
            MainMenuPreviewView previewView = previewRoot.gameObject.AddComponent<MainMenuPreviewView>();
            previewView.Initialize(previewRoot, activeTheme);

            TMP_Text hintText = UIFactory.CreateText("Hint", safeAreaRoot, "Tap to flip sides", 28, FontStyles.Normal, TextAlignmentOptions.Center, new Color(0.9f, 0.94f, 1f, 1f));
            UIFactory.SetAnchors((RectTransform)hintText.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -480f), new Vector2(480f, 44f));

            UnityEngine.UI.Button playButton = UIFactory.CreateButton("PlayButton", safeAreaRoot, "Play", UIFactory.AccentColor(activeTheme), new Color(0.05f, 0.08f, 0.12f, 1f), HandlePlayPressed);
            UIFactory.SetAnchors((RectTransform)playButton.transform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 170f), new Vector2(500f, 110f));

            UnityEngine.UI.Button settingsButton = UIFactory.CreateButton("SettingsButton", safeAreaRoot, "Settings", UIFactory.PanelColor(1f), Color.white, HandleOpenSettingsPressed);
            UIFactory.SetAnchors((RectTransform)settingsButton.transform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 78f), new Vector2(260f, 76f));

            settingsOverlayView = gameObject.AddComponent<SettingsOverlayView>();
            settingsOverlayView.Initialize(safeAreaRoot, activeTheme, saveService, HandleCloseSettingsPressed);
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
        }
#endif
    }
}
