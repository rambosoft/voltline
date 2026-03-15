using UnityEngine;
using UnityEngine.UI;
using Voltline.Data;
using Voltline.Utilities;

namespace Voltline.UI
{
    public sealed class MainMenuPreviewView : MonoBehaviour
    {
        private RectTransform playerRect;
        private RectTransform hazardRect;
        private RectTransform lineRect;
        private RectTransform glowRect;
        private float elapsed;

        public void Initialize(RectTransform parent, ThemeConfig activeTheme)
        {
            RectTransform root = UIFactory.CreatePanel("PreviewRoot", parent, new Color(0.035f, 0.05f, 0.11f, 0.78f));
            UIFactory.SetAnchors(root, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -10f), new Vector2(560f, 620f));

            RectTransform innerFrame = UIFactory.CreatePanel("InnerFrame", root, new Color(1f, 1f, 1f, 0.04f));
            UIFactory.Stretch(innerFrame, 18f);

            glowRect = UIFactory.CreatePanel("PreviewGlow", innerFrame, new Color(activeTheme.LineGlowColor.r, activeTheme.LineGlowColor.g, activeTheme.LineGlowColor.b, 0.08f));
            UIFactory.SetAnchors(glowRect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(180f, 480f));

            lineRect = UIFactory.CreatePanel("PreviewLine", innerFrame, activeTheme.LineGlowColor);
            UIFactory.SetAnchors(lineRect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(18f, 500f));

            RectTransform hazardTelegraph = UIFactory.CreatePanel("PreviewTelegraph", innerFrame, new Color(activeTheme.DangerColor.r, activeTheme.DangerColor.g, activeTheme.DangerColor.b, 0.14f));
            UIFactory.SetAnchors(hazardTelegraph, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(112f, -150f), new Vector2(168f, 174f));

            hazardRect = UIFactory.CreatePanel("PreviewHazard", innerFrame, activeTheme.DangerColor);
            UIFactory.SetAnchors(hazardRect, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(112f, -150f), new Vector2(92f, 138f));

            RectTransform hazardAccent = UIFactory.CreatePanel("PreviewHazardAccent", hazardRect, new Color(1f, 1f, 1f, 0.24f));
            UIFactory.SetAnchors(hazardAccent, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -24f), new Vector2(52f, 16f));

            playerRect = UIFactory.CreatePanel("PreviewPlayer", innerFrame, activeTheme.PlayerAccentColor);
            UIFactory.SetAnchors(playerRect, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(84f, 116f), new Vector2(88f, 88f));

            Image playerImage = playerRect.GetComponent<Image>();
            playerImage.sprite = RuntimeSpriteFactory.WhiteSprite;

            RectTransform caption = UIFactory.CreatePanel("CaptionStrip", root, new Color(1f, 1f, 1f, 0.03f));
            UIFactory.SetAnchors(caption, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 38f), new Vector2(360f, 42f));
        }

        private void Update()
        {
            if (playerRect == null)
            {
                return;
            }

            elapsed += Time.deltaTime;
            float cycle = Mathf.PingPong(elapsed * 0.82f, 1f);
            float side = Mathf.Lerp(-84f, 84f, cycle);
            float pulse = 1f + (Mathf.Sin(elapsed * 4f) * 0.045f);
            float glowPulse = 1f + (Mathf.Sin(elapsed * 2.1f) * 0.08f);

            playerRect.anchoredPosition = new Vector2(side, 116f);
            playerRect.localScale = new Vector3(pulse, pulse, 1f);
            hazardRect.localScale = new Vector3(1f, 1f + (Mathf.Sin(elapsed * 3f) * 0.05f), 1f);
            lineRect.localScale = new Vector3(1f + (Mathf.Sin(elapsed * 2f) * 0.04f), 1f, 1f);
            glowRect.localScale = new Vector3(glowPulse, 1f, 1f);
        }
    }
}
