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
        private TMP_Text messageText;
        private TMP_Text scoreText;
        private TMP_Text bestText;
        private Image retryButtonImage;
        private Image homeButtonImage;
        private OverlayTransitionController transitionController;

        public bool IsVisible => transitionController != null && transitionController.IsVisible;

        public void Initialize(Transform parent, ThemeConfig theme, System.Action retryAction, System.Action homeAction)
        {
            root = UIFactory.CreatePanel("ResultOverlay", parent, new Color(0f, 0f, 0f, 0.46f));
            UIFactory.Stretch(root, 0f);

            RectTransform panel = UIFactory.CreatePanel("ResultPanel", root, UIFactory.PanelColor(0.95f));
            UIFactory.SetAnchors(panel, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(760f, 640f));
            panelImage = panel.GetComponent<Image>();

            TMP_Text titleText = UIFactory.CreateText("Title", panel, "Run Over", 42, FontStyles.Bold, TextAlignmentOptions.Center, Color.white);
            UIFactory.SetAnchors((RectTransform)titleText.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -74f), new Vector2(460f, 58f));

            scoreText = UIFactory.CreateText("Score", panel, "0", 110, FontStyles.Bold, TextAlignmentOptions.Center, Color.white);
            UIFactory.SetAnchors((RectTransform)scoreText.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 88f), new Vector2(420f, 120f));

            bestText = UIFactory.CreateText("Best", panel, "Best 0", 30, FontStyles.Normal, TextAlignmentOptions.Center, theme.PlayerAccentColor);
            UIFactory.SetAnchors((RectTransform)bestText.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 10f), new Vector2(360f, 48f));

            messageText = UIFactory.CreateText("Message", panel, "One more run", 28, FontStyles.Normal, TextAlignmentOptions.Center, new Color(0.9f, 0.94f, 1f, 1f));
            UIFactory.SetAnchors((RectTransform)messageText.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -58f), new Vector2(560f, 46f));

            Button retryButton = UIFactory.CreateButton("RetryButton", panel, "Retry", UIFactory.AccentColor(theme), new Color(0.05f, 0.08f, 0.12f, 1f), retryAction);
            UIFactory.SetAnchors((RectTransform)retryButton.transform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 174f), new Vector2(500f, 112f));
            retryButtonImage = retryButton.GetComponent<Image>();

            Button homeButton = UIFactory.CreateButton("HomeButton", panel, "Home", UIFactory.PanelColor(1f), Color.white, homeAction);
            UIFactory.SetAnchors((RectTransform)homeButton.transform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 40f), new Vector2(500f, 78f));
            homeButtonImage = homeButton.GetComponent<Image>();

            transitionController = gameObject.AddComponent<OverlayTransitionController>();
            transitionController.Initialize(root, panel, -26f);
        }

        public void ApplyTheme(ThemeConfig theme)
        {
            if (panelImage != null)
            {
                panelImage.color = UIFactory.PanelColor(0.95f);
            }

            if (bestText != null)
            {
                bestText.color = theme.PlayerAccentColor;
            }

            if (retryButtonImage != null)
            {
                retryButtonImage.color = UIFactory.AccentColor(theme);
            }

            if (homeButtonImage != null)
            {
                homeButtonImage.color = UIFactory.PanelColor(1f);
            }
        }

        public void Show(int score, int bestScore, bool isNewBest, string message)
        {
            if (root == null)
            {
                return;
            }

            scoreText.text = score.ToString();
            bestText.text = isNewBest ? $"New Best {bestScore}" : $"Best {bestScore}";
            messageText.text = message;
            transitionController?.Show();
        }

        public void Hide()
        {
            transitionController?.Hide();
        }
    }
}
