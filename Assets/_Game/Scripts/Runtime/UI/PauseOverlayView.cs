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
        private Image settingsButtonImage;
        private Image homeButtonImage;
        private OverlayTransitionController transitionController;

        public bool IsVisible => transitionController != null && transitionController.IsVisible;

        public void Initialize(
            Transform parent,
            ThemeConfig theme,
            System.Action resumeAction,
            System.Action restartAction,
            System.Action homeAction,
            System.Action settingsAction)
        {
            root = UIFactory.CreatePanel("PauseOverlay", parent, new Color(0f, 0f, 0f, 0.52f));
            UIFactory.Stretch(root, 0f);

            RectTransform panel = UIFactory.CreatePanel("PausePanel", root, UIFactory.PanelColor(0.95f));
            UIFactory.SetAnchors(panel, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(760f, 720f));
            panelImage = panel.GetComponent<Image>();

            TMP_Text title = UIFactory.CreateText("Title", panel, "Paused", 52, FontStyles.Bold, TextAlignmentOptions.Center, Color.white);
            UIFactory.SetAnchors((RectTransform)title.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -92f), new Vector2(400f, 72f));

            Button resumeButton = UIFactory.CreateButton("ResumeButton", panel, "Resume", UIFactory.AccentColor(theme), new Color(0.05f, 0.08f, 0.12f, 1f), resumeAction);
            UIFactory.SetAnchors((RectTransform)resumeButton.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -240f), new Vector2(460f, 100f));
            resumeButtonImage = resumeButton.GetComponent<Image>();

            Button restartButton = UIFactory.CreateButton("RestartButton", panel, "Restart", UIFactory.PanelColor(1f), Color.white, restartAction);
            UIFactory.SetAnchors((RectTransform)restartButton.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -366f), new Vector2(460f, 88f));

            Button settingsButton = UIFactory.CreateButton("SettingsButton", panel, "Settings", theme.PlayerAccentColor, new Color(0.08f, 0.08f, 0.12f, 1f), settingsAction);
            UIFactory.SetAnchors((RectTransform)settingsButton.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -478f), new Vector2(460f, 84f));
            settingsButtonImage = settingsButton.GetComponent<Image>();

            Button homeButton = UIFactory.CreateButton("HomeButton", panel, "Home", UIFactory.DangerColor(theme), Color.white, homeAction);
            UIFactory.SetAnchors((RectTransform)homeButton.transform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 74f), new Vector2(240f, 76f));
            homeButtonImage = homeButton.GetComponent<Image>();

            transitionController = gameObject.AddComponent<OverlayTransitionController>();
            transitionController.Initialize(root, panel, -22f);
        }

        public void ApplyTheme(ThemeConfig theme)
        {
            if (panelImage != null)
            {
                panelImage.color = UIFactory.PanelColor(0.95f);
            }

            if (resumeButtonImage != null)
            {
                resumeButtonImage.color = UIFactory.AccentColor(theme);
            }

            if (settingsButtonImage != null)
            {
                settingsButtonImage.color = theme.PlayerAccentColor;
            }

            if (homeButtonImage != null)
            {
                homeButtonImage.color = UIFactory.DangerColor(theme);
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
