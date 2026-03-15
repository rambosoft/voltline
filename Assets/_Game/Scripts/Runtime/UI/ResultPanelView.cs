using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Voltline.Data;

namespace Voltline.UI
{
    public sealed class ResultPanelView : MonoBehaviour
    {
        private RectTransform root;
        private TMP_Text messageText;
        private TMP_Text scoreText;
        private TMP_Text bestText;

        public bool IsVisible => root != null && root.gameObject.activeSelf;

        public void Initialize(Transform parent, ThemeConfig theme, System.Action retryAction, System.Action homeAction)
        {
            root = UIFactory.CreatePanel("ResultOverlay", parent, new Color(0f, 0f, 0f, 0.52f));
            UIFactory.Stretch(root, 0f);
            root.gameObject.SetActive(false);

            RectTransform panel = UIFactory.CreatePanel("ResultPanel", root, UIFactory.PanelColor(0.95f));
            UIFactory.SetAnchors(panel, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 24f), new Vector2(760f, 620f));

            TMP_Text titleText = UIFactory.CreateText("Title", panel, "Run over", 40, FontStyles.Bold, TextAlignmentOptions.Center, Color.white);
            UIFactory.SetAnchors((RectTransform)titleText.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -72f), new Vector2(440f, 56f));

            scoreText = UIFactory.CreateText("Score", panel, "0", 96, FontStyles.Bold, TextAlignmentOptions.Center, Color.white);
            UIFactory.SetAnchors((RectTransform)scoreText.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 78f), new Vector2(380f, 110f));

            bestText = UIFactory.CreateText("Best", panel, "Best 0", 28, FontStyles.Normal, TextAlignmentOptions.Center, theme.PlayerAccentColor);
            UIFactory.SetAnchors((RectTransform)bestText.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 8f), new Vector2(320f, 50f));

            messageText = UIFactory.CreateText("Message", panel, "One more run", 28, FontStyles.Normal, TextAlignmentOptions.Center, new Color(0.9f, 0.94f, 1f, 1f));
            UIFactory.SetAnchors((RectTransform)messageText.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -54f), new Vector2(520f, 46f));

            Button retryButton = UIFactory.CreateButton("RetryButton", panel, "Retry", UIFactory.AccentColor(theme), new Color(0.05f, 0.08f, 0.12f, 1f), retryAction);
            UIFactory.SetAnchors((RectTransform)retryButton.transform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 136f), new Vector2(460f, 110f));

            Button homeButton = UIFactory.CreateButton("HomeButton", panel, "Home", UIFactory.PanelColor(1f), Color.white, homeAction);
            UIFactory.SetAnchors((RectTransform)homeButton.transform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 52f), new Vector2(240f, 76f));
        }

        public void Show(int score, int bestScore, bool isNewBest, string message)
        {
            if (root == null)
            {
                return;
            }

            root.gameObject.SetActive(true);
            scoreText.text = score.ToString();
            bestText.text = isNewBest ? $"New Best {bestScore}" : $"Best {bestScore}";
            messageText.text = message;
        }

        public void Hide()
        {
            if (root != null)
            {
                root.gameObject.SetActive(false);
            }
        }
    }
}
