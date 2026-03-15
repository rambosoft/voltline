using TMPro;
using UnityEngine;
using Voltline.Data;

namespace Voltline.UI
{
    public sealed class HudView : MonoBehaviour
    {
        private RectTransform root;
        private TMP_Text scoreText;
        private TMP_Text bestText;
        private Vector3 scoreBaseScale;
        private Color scoreBaseColor;
        private Color milestoneColor;
        private float scorePulseTime;
        private float milestonePulseTime;

        public void Initialize(Transform parent, ThemeConfig theme, System.Action pauseAction)
        {
            root = new GameObject("HudView", typeof(RectTransform)).GetComponent<RectTransform>();
            root.SetParent(parent, false);
            UIFactory.Stretch(root, 0f);

            scoreText = UIFactory.CreateText("ScoreText", root, "0", 58, FontStyles.Bold, TextAlignmentOptions.Center, Color.white);
            UIFactory.SetAnchors((RectTransform)scoreText.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -44f), new Vector2(280f, 72f));
            scoreBaseScale = Vector3.one;
            scoreBaseColor = scoreText.color;
            milestoneColor = theme.MilestoneColor;

            bestText = UIFactory.CreateText("BestText", root, "Best 0", 26, FontStyles.Normal, TextAlignmentOptions.TopRight, theme.PlayerAccentColor);
            UIFactory.SetAnchors((RectTransform)bestText.transform, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-28f, -42f), new Vector2(220f, 50f));

            UnityEngine.UI.Button pauseButton = UIFactory.CreateButton("PauseButton", root, "||", UIFactory.PanelColor(0.94f), Color.white, pauseAction);
            UIFactory.SetAnchors((RectTransform)pauseButton.transform, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(58f, -44f), new Vector2(84f, 84f));
        }

        private void Update()
        {
            if (scoreText == null)
            {
                return;
            }

            scorePulseTime = Mathf.Max(0f, scorePulseTime - Time.deltaTime);
            milestonePulseTime = Mathf.Max(0f, milestonePulseTime - Time.deltaTime);

            float scorePulse = scorePulseTime > 0f ? Mathf.Sin((1f - (scorePulseTime / 0.16f)) * Mathf.PI) * 0.12f : 0f;
            float milestonePulse = milestonePulseTime > 0f ? Mathf.Sin((1f - (milestonePulseTime / 0.24f)) * Mathf.PI) * 0.22f : 0f;
            float scale = 1f + scorePulse + milestonePulse;
            scoreText.rectTransform.localScale = scoreBaseScale * scale;
            scoreText.color = Color.Lerp(scoreBaseColor, milestoneColor, Mathf.Clamp01(milestonePulse * 2f));
        }

        public void SetScore(int score)
        {
            scoreText.text = score.ToString();
        }

        public void SetBestScore(int score)
        {
            bestText.text = $"Best {score}";
        }

        public void PlayScorePop()
        {
            scorePulseTime = 0.16f;
        }

        public void PlayMilestonePulse()
        {
            milestonePulseTime = 0.24f;
        }
    }
}
