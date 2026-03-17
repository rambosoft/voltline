using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
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
        private GridStatusOverlayView gridStatusOverlayView;
        private ExpandedTutorialOverlayView expandedTutorialOverlayView;
        private SplashLogoMomentView splashLogoMomentView;
        private ProductionCopyConfig productionCopyConfig;
        private BrandingPresentationConfig brandingPresentationConfig;
        private UIThemeConfig uiThemeConfig;
        private ThemeConfig activeTheme;
        private TMP_Text bestScoreText;
        private RectTransform bestSummaryPanel;
        private CanvasGroup bestSummaryCanvasGroup;
        private readonly HashSet<BrandingMenuEntryId> createdMenuEntries = new();

        public bool IsSettingsVisible => settingsOverlayView != null && settingsOverlayView.IsVisible;
        public bool SettingsHasThemeSelectionSection => settingsOverlayView != null && settingsOverlayView.HasThemeSelectionSection;
        public bool IsGridStatusVisible => gridStatusOverlayView != null && gridStatusOverlayView.IsVisible;
        public bool IsTutorialVisible => expandedTutorialOverlayView != null && expandedTutorialOverlayView.IsVisible;
        public bool IsSplashVisible => splashLogoMomentView != null && splashLogoMomentView.IsVisible;
        public string PublicTitle => brandingPresentationConfig != null ? brandingPresentationConfig.PublicTitle : string.Empty;
        public string Subtitle => brandingPresentationConfig != null ? brandingPresentationConfig.Subtitle : string.Empty;
        public string PrimaryMenuHint => productionCopyConfig != null ? productionCopyConfig.MenuHintPrimary : string.Empty;
        public bool IsBestSummaryVisible => bestSummaryCanvasGroup != null && bestSummaryCanvasGroup.alpha > 0.5f;

        private void Awake()
        {
            if (themeCatalog == null || themeCatalog.DefaultTheme == null || playerVisualConfig == null || obstacleVisualCatalog == null || audioCueCatalog == null)
            {
                Debug.LogError("MainMenuView is missing a valid ThemeCatalog, PlayerVisualConfig, ObstacleVisualCatalog, or AudioCueCatalog reference.");
                enabled = false;
                return;
            }

            brandingPresentationConfig = themeCatalog.BrandingPresentationConfig;
            productionCopyConfig = themeCatalog.ProductionCopyConfig;
            uiThemeConfig = themeCatalog.UiThemeConfig;
            if (brandingPresentationConfig == null || productionCopyConfig == null || uiThemeConfig == null)
            {
                Debug.LogError("MainMenuView requires ThemeCatalog branding, production copy, and UI theme config references.");
                enabled = false;
                return;
            }

            saveService = SaveService.EnsureExists();
            saveService.SynchronizeThemeUnlocks(themeCatalog);
            activeTheme = saveService.ResolveSelectedTheme(themeCatalog) ?? themeCatalog.DefaultTheme;
            AudioService audioService = AudioService.EnsureExists();
            audioService.Configure(audioCueCatalog, saveService, audioMixer);
            audioService.ApplyTheme(activeTheme);
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
            if (brandingPresentationConfig != null && brandingPresentationConfig.ShowExpandedTutorialHint && saveService != null && !saveService.HasSeenFirstLaunchHint)
            {
                settingsOverlayView?.Hide();
                gridStatusOverlayView?.Hide();
                expandedTutorialOverlayView?.Show();
                return;
            }

            LoadGameplay();
        }

        public void HandleConfirmTutorialPressed()
        {
            saveService?.SetHasSeenFirstLaunchHint(true);
            expandedTutorialOverlayView?.Hide();
            LoadGameplay();
        }

        public void HandleCloseTutorialPressed()
        {
            expandedTutorialOverlayView?.Hide();
        }

        public void HandleOpenSettingsPressed()
        {
            gridStatusOverlayView?.Hide();
            settingsOverlayView?.Show();
        }

        public void HandleCloseSettingsPressed()
        {
            settingsOverlayView?.Hide();
        }

        public void HandleOpenGridStatusPressed()
        {
            settingsOverlayView?.Hide();
            gridStatusOverlayView?.Show();
        }

        public void HandleCloseGridStatusPressed()
        {
            gridStatusOverlayView?.Hide();
        }

        public BrandingMenuEntryState GetMenuEntryState(BrandingMenuEntryId entryId)
        {
            return brandingPresentationConfig != null ? brandingPresentationConfig.GetMenuEntryState(entryId) : BrandingMenuEntryState.Hidden;
        }

        public bool HasMenuEntryButton(BrandingMenuEntryId entryId)
        {
            return createdMenuEntries.Contains(entryId);
        }

        private void HandleToggleBestPressed()
        {
            if (bestSummaryPanel == null || bestSummaryCanvasGroup == null)
            {
                return;
            }

            bool show = bestSummaryCanvasGroup.alpha < 0.5f;
            bestSummaryCanvasGroup.alpha = show ? 1f : 0f;
            bestSummaryCanvasGroup.blocksRaycasts = show;
            bestSummaryCanvasGroup.interactable = show;
        }

        private void BuildInterface(ThemeConfig resolvedTheme)
        {
            createdMenuEntries.Clear();
            Camera mainCamera = Camera.main ?? FindAnyObjectByType<Camera>();
            if (mainCamera != null)
            {
                mainCamera.backgroundColor = Color.Lerp(resolvedTheme.BackgroundTopColor, resolvedTheme.BackgroundBottomColor, 0.5f);
            }

            PlayerVisualConfig resolvedPlayerVisual = resolvedTheme.ResolvePlayerVisual(playerVisualConfig);
            ObstacleVisualCatalog resolvedObstacleVisualCatalog = resolvedTheme.ResolveObstacleVisualCatalog(obstacleVisualCatalog);

            UIFactory.EnsureEventSystem();
            Canvas canvas = UIFactory.CreateCanvas("MainMenuCanvas", transform);
            RectTransform safeAreaRoot = UIFactory.CreateSafeAreaRoot(canvas);

            RectTransform topBackground = UIFactory.CreatePanel("TopBackground", safeAreaRoot, new Color(resolvedTheme.BackgroundTopColor.r, resolvedTheme.BackgroundTopColor.g, resolvedTheme.BackgroundTopColor.b, 1f));
            topBackground.anchorMin = new Vector2(0f, 0.5f);
            topBackground.anchorMax = new Vector2(1f, 1f);
            topBackground.offsetMin = Vector2.zero;
            topBackground.offsetMax = Vector2.zero;

            RectTransform bottomBackground = UIFactory.CreatePanel("BottomBackground", safeAreaRoot, new Color(resolvedTheme.BackgroundBottomColor.r, resolvedTheme.BackgroundBottomColor.g, resolvedTheme.BackgroundBottomColor.b, 1f));
            bottomBackground.anchorMin = new Vector2(0f, 0f);
            bottomBackground.anchorMax = new Vector2(1f, 0.5f);
            bottomBackground.offsetMin = Vector2.zero;
            bottomBackground.offsetMax = Vector2.zero;

            RectTransform titleRoot = new GameObject("TitleRoot", typeof(RectTransform)).GetComponent<RectTransform>();
            titleRoot.SetParent(safeAreaRoot, false);
            UIFactory.SetAnchors(titleRoot, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -92f), new Vector2(920f, 150f));

            if (brandingPresentationConfig.LogoSprite != null)
            {
                Image logoImage = UIFactory.CreateSpriteImage("Logo", titleRoot, brandingPresentationConfig.LogoSprite, Color.white);
                UIFactory.Stretch((RectTransform)logoImage.transform, 0f);
            }
            else
            {
                TMP_Text titleText = UIFactory.CreateStyledText("Title", titleRoot, brandingPresentationConfig.PublicTitle, 72, FontStyles.Bold, TextAlignmentOptions.Center, uiThemeConfig, UiTextRole.Display, resolvedTheme);
                UIFactory.Stretch((RectTransform)titleText.transform, 0f);
            }

            TMP_Text subtitleText = UIFactory.CreateStyledText("Subtitle", safeAreaRoot, brandingPresentationConfig.Subtitle, 22, FontStyles.Normal, TextAlignmentOptions.Center, uiThemeConfig, UiTextRole.Micro, resolvedTheme);
            UIFactory.SetAnchors((RectTransform)subtitleText.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -162f), new Vector2(420f, 40f));

            RectTransform previewRoot = new GameObject("PreviewView", typeof(RectTransform)).GetComponent<RectTransform>();
            previewRoot.SetParent(safeAreaRoot, false);
            MainMenuPreviewView previewView = previewRoot.gameObject.AddComponent<MainMenuPreviewView>();
            previewView.Initialize(previewRoot, resolvedTheme, themeCatalog, resolvedPlayerVisual, resolvedObstacleVisualCatalog);

            TMP_Text hintText = UIFactory.CreateStyledText("Hint", safeAreaRoot, productionCopyConfig.MenuHintPrimary, 28, FontStyles.Normal, TextAlignmentOptions.Center, uiThemeConfig, UiTextRole.Body, resolvedTheme);
            UIFactory.SetAnchors((RectTransform)hintText.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -404f), new Vector2(540f, 44f));

            TMP_Text secondaryHintText = UIFactory.CreateStyledText("HintSecondary", safeAreaRoot, productionCopyConfig.MenuHintSecondary, 22, FontStyles.Normal, TextAlignmentOptions.Center, uiThemeConfig, UiTextRole.Micro, resolvedTheme);
            UIFactory.SetAnchors((RectTransform)secondaryHintText.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -442f), new Vector2(520f, 38f));

            Button playButton = UIFactory.CreateStyledButton("PlayButton", safeAreaRoot, productionCopyConfig.PlayButtonLabel, uiThemeConfig, UiSurfaceRole.PrimaryButton, resolvedTheme, HandlePlayPressed);
            UIFactory.SetAnchors((RectTransform)playButton.transform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 262f), new Vector2(540f, 118f));

            BuildSecondaryRow(safeAreaRoot, resolvedTheme);
            BuildBestSummaryPanel(safeAreaRoot, resolvedTheme);

            TMP_Text versionText = UIFactory.CreateStyledText("VersionText", safeAreaRoot, $"v{Application.version}", 18, FontStyles.Normal, TextAlignmentOptions.Center, uiThemeConfig, UiTextRole.Micro, resolvedTheme);
            UIFactory.SetAnchors((RectTransform)versionText.transform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 18f), new Vector2(220f, 28f));

            settingsOverlayView = gameObject.AddComponent<SettingsOverlayView>();
            settingsOverlayView.Initialize(safeAreaRoot, resolvedTheme, themeCatalog, saveService, HandleCloseSettingsPressed);

            if (brandingPresentationConfig.ShowGridStatusEntry)
            {
                gridStatusOverlayView = gameObject.AddComponent<GridStatusOverlayView>();
                gridStatusOverlayView.Initialize(safeAreaRoot, resolvedTheme, themeCatalog, saveService, HandleCloseGridStatusPressed);
            }

            if (brandingPresentationConfig.ShowExpandedTutorialHint)
            {
                expandedTutorialOverlayView = gameObject.AddComponent<ExpandedTutorialOverlayView>();
                expandedTutorialOverlayView.Initialize(safeAreaRoot, resolvedTheme, themeCatalog, HandleConfirmTutorialPressed, HandleCloseTutorialPressed);
            }

            if (brandingPresentationConfig.ShowSplashLogoMoment)
            {
                splashLogoMomentView = gameObject.AddComponent<SplashLogoMomentView>();
                splashLogoMomentView.Initialize(safeAreaRoot, resolvedTheme, themeCatalog);
                splashLogoMomentView.ShowIfNeeded();
            }
        }

        private void BuildSecondaryRow(RectTransform safeAreaRoot, ThemeConfig resolvedTheme)
        {
            RectTransform rowRoot = new GameObject("SecondaryRow", typeof(RectTransform)).GetComponent<RectTransform>();
            rowRoot.SetParent(safeAreaRoot, false);
            UIFactory.SetAnchors(rowRoot, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 136f), new Vector2(860f, 88f));

            CreateMenuEntryButton(rowRoot, resolvedTheme, BrandingMenuEntryId.Daily, -318f, null);
            CreateMenuEntryButton(rowRoot, resolvedTheme, BrandingMenuEntryId.Themes, -106f, brandingPresentationConfig.ShowGridStatusEntry ? HandleOpenGridStatusPressed : null);
            CreateMenuEntryButton(rowRoot, resolvedTheme, BrandingMenuEntryId.Best, 106f, HandleToggleBestPressed);
            CreateMenuEntryButton(rowRoot, resolvedTheme, BrandingMenuEntryId.Settings, 318f, HandleOpenSettingsPressed);
        }

        private void CreateMenuEntryButton(RectTransform parent, ThemeConfig resolvedTheme, BrandingMenuEntryId entryId, float xPosition, System.Action onClick)
        {
            BrandingMenuEntryState entryState = brandingPresentationConfig.GetMenuEntryState(entryId);
            if (entryState == BrandingMenuEntryState.Hidden)
            {
                return;
            }

            createdMenuEntries.Add(entryId);
            string label = ResolveMenuEntryLabel(entryId);
            Button button = UIFactory.CreateStyledButton($"MenuEntry_{entryId}", parent, label, uiThemeConfig, UiSurfaceRole.SecondaryButton, resolvedTheme, onClick);
            RectTransform buttonRect = (RectTransform)button.transform;
            UIFactory.SetAnchors(buttonRect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(xPosition, 0f), new Vector2(180f, 76f));
            button.interactable = entryState == BrandingMenuEntryState.Enabled;

            if (entryState == BrandingMenuEntryState.Disabled)
            {
                TMP_Text statusText = UIFactory.CreateStyledText($"Status_{entryId}", button.transform, productionCopyConfig.ComingSoonStatusLabel, 16, FontStyles.Bold, TextAlignmentOptions.Bottom, uiThemeConfig, UiTextRole.Micro, resolvedTheme);
                RectTransform statusRect = (RectTransform)statusText.transform;
                UIFactory.SetAnchors(statusRect, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 8f), new Vector2(120f, 18f));
            }
        }

        private string ResolveMenuEntryLabel(BrandingMenuEntryId entryId)
        {
            if (productionCopyConfig == null)
            {
                return string.Empty;
            }

            if (entryId == BrandingMenuEntryId.Themes && brandingPresentationConfig != null && brandingPresentationConfig.ShowGridStatusEntry)
            {
                return productionCopyConfig.GridStatusButtonLabel;
            }

            return productionCopyConfig.GetMenuEntryLabel(entryId);
        }

        private void BuildBestSummaryPanel(RectTransform safeAreaRoot, ThemeConfig resolvedTheme)
        {
            bestSummaryPanel = UIFactory.CreateSurface("BestSummaryPanel", safeAreaRoot, uiThemeConfig, UiSurfaceRole.Panel, resolvedTheme);
            UIFactory.SetAnchors(bestSummaryPanel, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 382f), new Vector2(420f, 132f));
            bestSummaryCanvasGroup = bestSummaryPanel.gameObject.AddComponent<CanvasGroup>();
            bestSummaryCanvasGroup.alpha = 0f;
            bestSummaryCanvasGroup.blocksRaycasts = false;
            bestSummaryCanvasGroup.interactable = false;

            TMP_Text panelTitle = UIFactory.CreateStyledText("BestPanelTitle", bestSummaryPanel, productionCopyConfig.BestPanelTitle, 22, FontStyles.Bold, TextAlignmentOptions.Center, uiThemeConfig, UiTextRole.Heading, resolvedTheme);
            UIFactory.SetAnchors((RectTransform)panelTitle.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -24f), new Vector2(260f, 34f));

            bestScoreText = UIFactory.CreateStyledText("BestScore", bestSummaryPanel, FormatBestScore(0), 34, FontStyles.Bold, TextAlignmentOptions.Center, uiThemeConfig, UiTextRole.Score, resolvedTheme);
            UIFactory.SetAnchors((RectTransform)bestScoreText.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -4f), new Vector2(260f, 42f));
        }

        private void RefreshBestScore()
        {
            if (bestScoreText != null && saveService != null)
            {
                bestScoreText.text = FormatBestScore(saveService.BestScore);
            }

            gridStatusOverlayView?.Refresh();
        }

        private string FormatBestScore(int score)
        {
            return productionCopyConfig != null ? productionCopyConfig.FormatBestScore(score) : $"Best {score}";
        }

        private void LoadGameplay()
        {
            settingsOverlayView?.Hide();
            gridStatusOverlayView?.Hide();
            expandedTutorialOverlayView?.Hide();
            splashLogoMomentView?.Hide();
            SceneManager.LoadScene(SceneCatalog.Gameplay, LoadSceneMode.Single);
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
