using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Voltline.Data;

namespace Voltline.UI
{
    public sealed class PauseOverlayView : MonoBehaviour
    {
        private RectTransform root;
        private Image panelImage;
        private Image resumeButtonImage;
        private Image restartButtonImage;
        private Image settingsButtonImage;
        private Image homeButtonImage;
        private UIThemeConfig uiThemeConfig;
        private ProductionCopyConfig productionCopyConfig;
        private TMP_Text titleText;
        private OverlayTransitionController transitionController;

        public bool IsVisible => transitionController != null && transitionController.IsVisible;

        public void Initialize(
            Transform parent,
            ThemeConfig theme,
            ThemeCatalog themeCatalog,
            System.Action resumeAction,
            System.Action restartAction,
            System.Action homeAction,
            System.Action settingsAction)
        {
            uiThemeConfig = themeCatalog != null ? themeCatalog.UiThemeConfig : null;
            productionCopyConfig = themeCatalog != null ? themeCatalog.ProductionCopyConfig : null;

            root = UIFactory.CreateSurface("PauseOverlay", parent, uiThemeConfig, UiSurfaceRole.Overlay, theme);
            UIFactory.Stretch(root, 0f);

            RectTransform panel = UIFactory.CreateSurface("PausePanel", root, uiThemeConfig, UiSurfaceRole.Panel, theme);
            UIFactory.SetAnchors(panel, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(760f, 720f));
            panelImage = panel.GetComponent<Image>();

            titleText = UIFactory.CreateStyledText("Title", panel, productionCopyConfig != null ? productionCopyConfig.PauseTitle : "PAUSED", 52, FontStyles.Bold, TextAlignmentOptions.Center, uiThemeConfig, UiTextRole.Heading, theme);
            UIFactory.SetAnchors((RectTransform)titleText.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -92f), new Vector2(400f, 72f));

            Button resumeButton = UIFactory.CreateStyledButton("ResumeButton", panel, productionCopyConfig != null ? productionCopyConfig.ResumeButtonLabel : "Resume", uiThemeConfig, UiSurfaceRole.PrimaryButton, theme, resumeAction);
            UIFactory.SetAnchors((RectTransform)resumeButton.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -240f), new Vector2(460f, 100f));
            resumeButtonImage = resumeButton.GetComponent<Image>();

            Button restartButton = UIFactory.CreateStyledButton("RestartButton", panel, productionCopyConfig != null ? productionCopyConfig.RestartButtonLabel : "Restart", uiThemeConfig, UiSurfaceRole.SecondaryButton, theme, restartAction);
            UIFactory.SetAnchors((RectTransform)restartButton.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -366f), new Vector2(460f, 88f));
            restartButtonImage = restartButton.GetComponent<Image>();

            Button settingsButton = UIFactory.CreateStyledButton("SettingsButton", panel, productionCopyConfig != null ? productionCopyConfig.SettingsButtonLabel : "Settings", uiThemeConfig, UiSurfaceRole.AccentButton, theme, settingsAction);
            UIFactory.SetAnchors((RectTransform)settingsButton.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -478f), new Vector2(460f, 84f));
            settingsButtonImage = settingsButton.GetComponent<Image>();

            Button homeButton = UIFactory.CreateStyledButton("HomeButton", panel, productionCopyConfig != null ? productionCopyConfig.HomeButtonLabel : "Home", uiThemeConfig, UiSurfaceRole.DestructiveButton, theme, homeAction);
            UIFactory.SetAnchors((RectTransform)homeButton.transform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 74f), new Vector2(240f, 76f));
            homeButtonImage = homeButton.GetComponent<Image>();

            transitionController = gameObject.AddComponent<OverlayTransitionController>();
            transitionController.Initialize(root, panel, -22f);
        }

        public void ApplyTheme(ThemeConfig theme)
        {
            if (panelImage != null)
            {
                UIFactory.ApplySurfaceStyle(panelImage, uiThemeConfig, UiSurfaceRole.Panel, theme);
            }

            if (titleText != null)
            {
                UIFactory.ApplyTextStyle(titleText, uiThemeConfig, UiTextRole.Heading, theme);
            }

            if (resumeButtonImage != null)
            {
                UIFactory.ApplySurfaceStyle(resumeButtonImage, uiThemeConfig, UiSurfaceRole.PrimaryButton, theme);
            }

            if (restartButtonImage != null)
            {
                UIFactory.ApplySurfaceStyle(restartButtonImage, uiThemeConfig, UiSurfaceRole.SecondaryButton, theme);
            }

            if (settingsButtonImage != null)
            {
                UIFactory.ApplySurfaceStyle(settingsButtonImage, uiThemeConfig, UiSurfaceRole.AccentButton, theme);
            }

            if (homeButtonImage != null)
            {
                UIFactory.ApplySurfaceStyle(homeButtonImage, uiThemeConfig, UiSurfaceRole.DestructiveButton, theme);
            }
        }

        public void Show()
        {
            transitionController?.Show();
        }

        public void Hide()
        {
            transitionController?.Hide();
        }
    }
}
