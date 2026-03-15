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
        private ThemeConfig theme;
        private float elapsed;

        public void Initialize(RectTransform parent, ThemeConfig activeTheme)
        {
            theme = activeTheme;

            RectTransform root = UIFactory.CreatePanel("PreviewRoot", parent, new Color(0.03f, 0.05f, 0.11f, 0.72f));
            UIFactory.SetAnchors(root, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -70f), new Vector2(520f, 720f));

            lineRect = UIFactory.CreatePanel("PreviewLine", root, theme.LineGlowColor);
            UIFactory.SetAnchors(lineRect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(16f, 560f));

            RectTransform hazardParent = new GameObject("HazardRoot", typeof(RectTransform)).GetComponent<RectTransform>();
            hazardParent.SetParent(root, false);
            UIFactory.Stretch(hazardParent, 0f);

            hazardRect = UIFactory.CreatePanel("PreviewHazard", hazardParent, theme.DangerColor);
            UIFactory.SetAnchors(hazardRect, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(86f, -168f), new Vector2(98f, 132f));

            RectTransform telegraphRect = UIFactory.CreatePanel("PreviewTelegraph", hazardParent, new Color(1f, 0.95f, 0.35f, 0.18f));
            UIFactory.SetAnchors(telegraphRect, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(86f, -168f), new Vector2(156f, 188f));

            playerRect = UIFactory.CreatePanel("PreviewPlayer", root, theme.PlayerAccentColor);
            UIFactory.SetAnchors(playerRect, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(76f, 132f), new Vector2(92f, 92f));

            Image playerImage = playerRect.GetComponent<Image>();
            playerImage.sprite = RuntimeSpriteFactory.WhiteSprite;
        }

        private void Update()
        {
            if (playerRect == null)
            {
                return;
            }

            elapsed += Time.deltaTime;
            float cycle = Mathf.PingPong(elapsed * 0.75f, 1f);
            float side = Mathf.Lerp(-76f, 76f, cycle);
            float pulse = 1f + (Mathf.Sin(elapsed * 4f) * 0.04f);

            playerRect.anchoredPosition = new Vector2(side, 132f);
            playerRect.localScale = new Vector3(pulse, pulse, 1f);
            hazardRect.localScale = new Vector3(1f, 1f + (Mathf.Sin(elapsed * 3f) * 0.06f), 1f);
            lineRect.localScale = new Vector3(1f + (Mathf.Sin(elapsed * 2f) * 0.05f), 1f, 1f);
        }
    }
}