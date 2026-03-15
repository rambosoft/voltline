using TMPro;
using UnityEngine;
using Voltline.Data;
using Voltline.Save;

namespace Voltline.UI
{
    public sealed class HudView : MonoBehaviour
    {
        private RectTransform root;
        private TMP_Text scoreText;
        private TMP_Text bestText;

        public void Initialize(Transform parent, ThemeConfig theme, System.Action pauseAction)
        {
            root = new GameObject("HudView", typeof(RectTransform)).GetComponent<RectTransform>();
            root.SetParent(parent, false);
            UIFactory.Stretch(root, 0f);

            scoreText = UIFactory.CreateText("ScoreText", root, "0", 58, FontStyles.Bold, TextAlignmentOptions.Center, Color.white);
            UIFactory.SetAnchors((RectTransform)scoreText.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -44f), new Vector2(280f, 72f));

            bestText = UIFactory.CreateText("BestText", root, "Best 0", 26, FontStyles.Normal, TextAlignmentOptions.TopRight, theme.PlayerAccentColor);
            UIFactory.SetAnchors((RectTransform)bestText.transform, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-28f, -42f), new Vector2(220f, 50f));

            UnityEngine.UI.Button pauseButton = UIFactory.CreateButton("PauseButton", root, "||", UIFactory.PanelColor(0.94f), Color.white, pauseAction);
            UIFactory.SetAnchors((RectTransform)pauseButton.transform, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(58f, -44f), new Vector2(84f, 84f));
        }

        public void SetScore(int score)
        {
            scoreText.text = score.ToString();
        }

        public void SetBestScore(int score)
        {
            bestText.text = $"Best {score}";
        }
    }
}