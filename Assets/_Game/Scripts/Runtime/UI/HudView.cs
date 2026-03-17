using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Voltline.Data;

namespace Voltline.UI
{
    public sealed class HudView : MonoBehaviour
    {
        private RectTransform root;
        private TMP_Text scoreText;
        private TMP_Text bestText;
        private TMP_Text districtText;
        private TMP_Text milestoneText;
        private Image pauseButtonImage;
        private UIThemeConfig uiThemeConfig;
        private ProductionCopyConfig productionCopyConfig;
        private Vector3 scoreBaseScale;
        private Color scoreBaseColor;
        private Color milestoneColor;
        private float scorePulseTime;
        private float milestonePulseTime;
        private float milestoneMessageTime;

        public void Initialize(Transform parent, ThemeConfig theme, ThemeCatalog themeCatalog, System.Action pauseAction)
        {
            uiThemeConfig = themeCatalog != null ? themeCatalog.UiThemeConfig : null;
            productionCopyConfig = themeCatalog != null ? themeCatalog.ProductionCopyConfig : null;

            root = new GameObject("HudView", typeof(RectTransform)).GetComponent<RectTransform>();
            root.SetParent(parent, false);
            UIFactory.Stretch(root, 0f);

            scoreText = UIFactory.CreateStyledText("ScoreText", root, "0", 64, FontStyles.Bold, TextAlignmentOptions.Center, uiThemeConfig, UiTextRole.Score, theme);
            UIFactory.SetAnchors((RectTransform)scoreText.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -44f), new Vector2(320f, 72f));
            scoreBaseScale = Vector3.one;
            scoreBaseColor = scoreText.color;
            milestoneColor = theme.MilestoneColor;

            districtText = UIFactory.CreateStyledText("DistrictText", root, string.Empty, 20, FontStyles.Normal, TextAlignmentOptions.Center, uiThemeConfig, UiTextRole.Micro, theme);
            UIFactory.SetAnchors((RectTransform)districtText.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -94f), new Vector2(360f, 32f));

            milestoneText = UIFactory.CreateStyledText("MilestoneText", root, string.Empty, 24, FontStyles.Bold, TextAlignmentOptions.Center, uiThemeConfig, UiTextRole.Status, theme);
            UIFactory.SetAnchors((RectTransform)milestoneText.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -122f), new Vector2(420f, 34f));
            milestoneText.alpha = 0f;

            bestText = UIFactory.CreateStyledText("BestText", root, FormatBestScore(0), 26, FontStyles.Normal, TextAlignmentOptions.TopRight, uiThemeConfig, UiTextRole.Secondary, theme);
            UIFactory.SetAnchors((RectTransform)bestText.transform, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-28f, -42f), new Vector2(240f, 50f));

            Button pauseButton = UIFactory.CreateStyledButton("PauseButton", root, "||", uiThemeConfig, UiSurfaceRole.Chip, theme, pauseAction);
            UIFactory.SetAnchors((RectTransform)pauseButton.transform, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(58f, -44f), new Vector2(84f, 84f));
            pauseButtonImage = pauseButton.GetComponent<Image>();
        }

        public void ApplyTheme(ThemeConfig theme)
        {
            milestoneColor = theme.MilestoneColor;
            if (scoreText != null)
            {
                UIFactory.ApplyTextStyle(scoreText, uiThemeConfig, UiTextRole.Score, theme);
                scoreBaseColor = scoreText.color;
            }

            if (districtText != null)
            {
                UIFactory.ApplyTextStyle(districtText, uiThemeConfig, UiTextRole.Micro, theme);
            }

            if (milestoneText != null)
            {
                UIFactory.ApplyTextStyle(milestoneText, uiThemeConfig, UiTextRole.Status, theme);
            }

            if (bestText != null)
            {
                UIFactory.ApplyTextStyle(bestText, uiThemeConfig, UiTextRole.Secondary, theme);
            }

            if (pauseButtonImage != null)
            {
                UIFactory.ApplySurfaceStyle(pauseButtonImage, uiThemeConfig, UiSurfaceRole.Chip, theme);
            }
        }

        private void Update()
        {
            if (scoreText == null)
            {
                return;
            }

            scorePulseTime = Mathf.Max(0f, scorePulseTime - Time.deltaTime);
            milestonePulseTime = Mathf.Max(0f, milestonePulseTime - Time.deltaTime);
            milestoneMessageTime = Mathf.Max(0f, milestoneMessageTime - Time.deltaTime);

            float scorePulse = scorePulseTime > 0f ? Mathf.Sin((1f - (scorePulseTime / 0.16f)) * Mathf.PI) * 0.12f : 0f;
            float milestonePulse = milestonePulseTime > 0f ? Mathf.Sin((1f - (milestonePulseTime / 0.24f)) * Mathf.PI) * 0.22f : 0f;
            float scale = 1f + scorePulse + milestonePulse;
            scoreText.rectTransform.localScale = scoreBaseScale * scale;
            scoreText.color = Color.Lerp(scoreBaseColor, milestoneColor, Mathf.Clamp01(milestonePulse * 2f));

            if (milestoneText != null)
            {
                float alpha = milestoneMessageTime > 0f ? Mathf.Clamp01(milestoneMessageTime / 0.9f) : 0f;
                milestoneText.alpha = alpha;
            }
        }

        public void SetScore(int score)
        {
            scoreText.text = score.ToString();
        }

        public void SetBestScore(int score)
        {
            bestText.text = FormatBestScore(score);
        }

        public void SetDistrictStatus(string statusLabel)
        {
            if (districtText != null)
            {
                districtText.text = statusLabel;
            }
        }

        public void ShowMilestoneMessage(string message)
        {
            if (milestoneText == null || string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            milestoneText.text = message;
            milestoneMessageTime = 0.9f;
        }

        public void PlayScorePop()
        {
            scorePulseTime = 0.16f;
        }

        public void PlayMilestonePulse()
        {
            milestonePulseTime = 0.24f;
        }

        private string FormatBestScore(int score)
        {
            return productionCopyConfig != null ? productionCopyConfig.FormatBestScore(score) : $"Best {score}";
        }
    }
}
