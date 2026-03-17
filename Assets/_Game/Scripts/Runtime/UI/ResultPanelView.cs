using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Voltline.Data;

namespace Voltline.UI
{
    public sealed class ResultPanelView : MonoBehaviour
    {
        private RectTransform root;
        private Image panelImage;
        private TMP_Text titleText;
        private TMP_Text messageText;
        private TMP_Text scoreText;
        private TMP_Text bestText;
        private Image retryButtonImage;
        private Image homeButtonImage;
        private Image shareButtonImage;
        private UIThemeConfig uiThemeConfig;
        private ProductionCopyConfig productionCopyConfig;
        private OverlayTransitionController transitionController;

        public bool IsVisible => transitionController != null && transitionController.IsVisible;
        public bool HasShareButton => shareButtonImage != null;

        public void Initialize(Transform parent, ThemeConfig theme, ThemeCatalog themeCatalog, System.Action retryAction, System.Action homeAction, System.Action shareAction)
        {
            uiThemeConfig = themeCatalog != null ? themeCatalog.UiThemeConfig : null;
            productionCopyConfig = themeCatalog != null ? themeCatalog.ProductionCopyConfig : null;

            root = UIFactory.CreateSurface("ResultOverlay", parent, uiThemeConfig, UiSurfaceRole.Overlay, theme);
            UIFactory.Stretch(root, 0f);

            RectTransform panel = UIFactory.CreateSurface("ResultPanel", root, uiThemeConfig, UiSurfaceRole.Panel, theme);
            UIFactory.SetAnchors(panel, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(760f, 640f));
            panelImage = panel.GetComponent<Image>();

            titleText = UIFactory.CreateStyledText("Title", panel, string.Empty, 42, FontStyles.Bold, TextAlignmentOptions.Center, uiThemeConfig, UiTextRole.Heading, theme);
            UIFactory.SetAnchors((RectTransform)titleText.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -74f), new Vector2(460f, 58f));

            scoreText = UIFactory.CreateStyledText("Score", panel, "0", 110, FontStyles.Bold, TextAlignmentOptions.Center, uiThemeConfig, UiTextRole.Score, theme);
            UIFactory.SetAnchors((RectTransform)scoreText.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 88f), new Vector2(420f, 120f));

            bestText = UIFactory.CreateStyledText("Best", panel, FormatBestScore(0, false), 30, FontStyles.Normal, TextAlignmentOptions.Center, uiThemeConfig, UiTextRole.Secondary, theme);
            UIFactory.SetAnchors((RectTransform)bestText.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 10f), new Vector2(360f, 48f));

            messageText = UIFactory.CreateStyledText("Message", panel, string.Empty, 28, FontStyles.Normal, TextAlignmentOptions.Center, uiThemeConfig, UiTextRole.Body, theme);
            messageText.textWrappingMode = TextWrappingModes.Normal;
            UIFactory.SetAnchors((RectTransform)messageText.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -58f), new Vector2(560f, 70f));

            Button retryButton = UIFactory.CreateStyledButton("RetryButton", panel, productionCopyConfig != null ? productionCopyConfig.RetryButtonLabel : "Retry", uiThemeConfig, UiSurfaceRole.PrimaryButton, theme, retryAction);
            UIFactory.SetAnchors((RectTransform)retryButton.transform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 174f), new Vector2(500f, 112f));
            retryButtonImage = retryButton.GetComponent<Image>();

            if (shareAction != null)
            {
                Button homeButton = UIFactory.CreateStyledButton("HomeButton", panel, productionCopyConfig != null ? productionCopyConfig.HomeButtonLabel : "Home", uiThemeConfig, UiSurfaceRole.SecondaryButton, theme, homeAction);
                UIFactory.SetAnchors((RectTransform)homeButton.transform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(-136f, 44f), new Vector2(240f, 78f));
                homeButtonImage = homeButton.GetComponent<Image>();

                Button shareButton = UIFactory.CreateStyledButton("ShareButton", panel, productionCopyConfig != null ? productionCopyConfig.ShareButtonLabel : "Share", uiThemeConfig, UiSurfaceRole.AccentButton, theme, shareAction);
                UIFactory.SetAnchors((RectTransform)shareButton.transform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(136f, 44f), new Vector2(240f, 78f));
                shareButtonImage = shareButton.GetComponent<Image>();
            }
            else
            {
                Button homeButton = UIFactory.CreateStyledButton("HomeButton", panel, productionCopyConfig != null ? productionCopyConfig.HomeButtonLabel : "Home", uiThemeConfig, UiSurfaceRole.SecondaryButton, theme, homeAction);
                UIFactory.SetAnchors((RectTransform)homeButton.transform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 40f), new Vector2(500f, 78f));
                homeButtonImage = homeButton.GetComponent<Image>();
            }

            transitionController = gameObject.AddComponent<OverlayTransitionController>();
            transitionController.Initialize(root, panel, -26f);
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

            if (scoreText != null)
            {
                UIFactory.ApplyTextStyle(scoreText, uiThemeConfig, UiTextRole.Score, theme);
            }

            if (bestText != null)
            {
                UIFactory.ApplyTextStyle(bestText, uiThemeConfig, UiTextRole.Secondary, theme);
            }

            if (messageText != null)
            {
                UIFactory.ApplyTextStyle(messageText, uiThemeConfig, UiTextRole.Body, theme);
            }

            if (retryButtonImage != null)
            {
                UIFactory.ApplySurfaceStyle(retryButtonImage, uiThemeConfig, UiSurfaceRole.PrimaryButton, theme);
            }

            if (homeButtonImage != null)
            {
                UIFactory.ApplySurfaceStyle(homeButtonImage, uiThemeConfig, UiSurfaceRole.SecondaryButton, theme);
            }

            if (shareButtonImage != null)
            {
                UIFactory.ApplySurfaceStyle(shareButtonImage, uiThemeConfig, UiSurfaceRole.AccentButton, theme);
            }
        }

        public void Show(int score, int bestScore, bool isNewBest, ResultPresentationCopy copy)
        {
            if (root == null)
            {
                return;
            }

            scoreText.text = score.ToString();
            bestText.text = FormatBestScore(bestScore, isNewBest);
            titleText.text = copy.Title;
            messageText.text = copy.Message;
            transitionController?.Show();
        }

        public void Hide()
        {
            transitionController?.Hide();
        }

        private string FormatBestScore(int score, bool isNewBest)
        {
            if (productionCopyConfig == null)
            {
                return isNewBest ? $"New Best {score}" : $"Best {score}";
            }

            return isNewBest ? productionCopyConfig.FormatNewBestScore(score) : productionCopyConfig.FormatBestScore(score);
        }
    }
}
