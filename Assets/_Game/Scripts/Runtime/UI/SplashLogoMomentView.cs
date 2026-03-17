using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Voltline.Data;

namespace Voltline.UI
{
    public sealed class SplashLogoMomentView : MonoBehaviour
    {
        private static bool hasShownThisSession;

        private BrandingPresentationConfig brandingPresentationConfig;
        private OverlayTransitionController transitionController;
        private Coroutine autoHideRoutine;

        public bool IsVisible => transitionController != null && transitionController.IsVisible;

        public void Initialize(Transform parent, ThemeConfig theme, ThemeCatalog themeCatalog)
        {
            brandingPresentationConfig = themeCatalog != null ? themeCatalog.BrandingPresentationConfig : null;
            UIThemeConfig uiThemeConfig = themeCatalog != null ? themeCatalog.UiThemeConfig : null;

            RectTransform root = UIFactory.CreateSurface("SplashOverlay", parent, uiThemeConfig, UiSurfaceRole.Overlay, theme);
            root.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.2f);
            UIFactory.Stretch(root, 0f);

            RectTransform panel = UIFactory.CreateSurface("SplashPanel", root, uiThemeConfig, UiSurfaceRole.HighlightPanel, theme);
            UIFactory.SetAnchors(panel, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -24f), new Vector2(700f, 320f));

            RectTransform accentLine = UIFactory.CreatePanel("AccentLine", panel, WithAlpha(theme != null ? theme.LineGlowColor : Color.white, 0.82f));
            UIFactory.SetAnchors(accentLine, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -26f), new Vector2(280f, 10f));

            if (brandingPresentationConfig != null && brandingPresentationConfig.LogoSprite != null)
            {
                Image logoImage = UIFactory.CreateSpriteImage("SplashLogo", panel, brandingPresentationConfig.LogoSprite, Color.white);
                RectTransform logoRect = (RectTransform)logoImage.transform;
                UIFactory.SetAnchors(logoRect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 26f), new Vector2(420f, 132f));
            }
            else
            {
                TMP_Text titleText = UIFactory.CreateStyledText("SplashTitle", panel, brandingPresentationConfig != null ? brandingPresentationConfig.PublicTitle : "Voltline", 84, FontStyles.Bold, TextAlignmentOptions.Center, uiThemeConfig, UiTextRole.Display, theme);
                UIFactory.SetAnchors((RectTransform)titleText.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 26f), new Vector2(500f, 96f));
            }

            TMP_Text subtitleText = UIFactory.CreateStyledText("SplashSubtitle", panel, brandingPresentationConfig != null ? brandingPresentationConfig.Subtitle : string.Empty, 26, FontStyles.Normal, TextAlignmentOptions.Center, uiThemeConfig, UiTextRole.Micro, theme);
            UIFactory.SetAnchors((RectTransform)subtitleText.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -48f), new Vector2(480f, 42f));

            transitionController = gameObject.AddComponent<OverlayTransitionController>();
            transitionController.Initialize(root, panel, 22f);
        }

        public void ShowIfNeeded()
        {
            if (brandingPresentationConfig == null || !brandingPresentationConfig.ShowSplashLogoMoment || hasShownThisSession)
            {
                return;
            }

            hasShownThisSession = true;
            transitionController?.Show();
            if (autoHideRoutine != null)
            {
                StopCoroutine(autoHideRoutine);
            }

            autoHideRoutine = StartCoroutine(AutoHideAfterDelay());
        }

        public void Hide()
        {
            if (autoHideRoutine != null)
            {
                StopCoroutine(autoHideRoutine);
                autoHideRoutine = null;
            }

            transitionController?.Hide();
        }

        private IEnumerator AutoHideAfterDelay()
        {
            float delay = brandingPresentationConfig != null ? brandingPresentationConfig.SplashDurationSeconds : 1.35f;
            yield return new WaitForSecondsRealtime(Mathf.Max(0.2f, delay));
            transitionController?.Hide();
            autoHideRoutine = null;
        }
        private static Color WithAlpha(Color color, float alpha)
        {
            color.a = alpha;
            return color;
        }

    }
}
