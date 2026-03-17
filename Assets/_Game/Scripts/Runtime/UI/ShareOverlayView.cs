using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Voltline.Data;

namespace Voltline.UI
{
    public readonly struct SharePresentationCopy
    {
        public SharePresentationCopy(string summary, string flavor, string clipboardText)
        {
            Summary = summary;
            Flavor = flavor;
            ClipboardText = clipboardText;
        }

        public string Summary { get; }
        public string Flavor { get; }
        public string ClipboardText { get; }
    }

    public sealed class ShareOverlayView : MonoBehaviour
    {
        private UIThemeConfig uiThemeConfig;
        private ProductionCopyConfig productionCopyConfig;
        private OverlayTransitionController transitionController;
        private Image panelImage;
        private Image copyButtonImage;
        private Image closeButtonImage;
        private TMP_Text summaryText;
        private TMP_Text flavorText;
        private TMP_Text statusText;
        private SharePresentationCopy currentCopy;

        public bool IsVisible => transitionController != null && transitionController.IsVisible;

        public void Initialize(Transform parent, ThemeConfig theme, ThemeCatalog themeCatalog, System.Action closeAction)
        {
            uiThemeConfig = themeCatalog != null ? themeCatalog.UiThemeConfig : null;
            productionCopyConfig = themeCatalog != null ? themeCatalog.ProductionCopyConfig : null;

            RectTransform root = UIFactory.CreateSurface("ShareOverlay", parent, uiThemeConfig, UiSurfaceRole.Overlay, theme);
            UIFactory.Stretch(root, 0f);

            RectTransform panel = UIFactory.CreateSurface("SharePanel", root, uiThemeConfig, UiSurfaceRole.Panel, theme);
            UIFactory.SetAnchors(panel, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(720f, 540f));
            panelImage = panel.GetComponent<Image>();

            TMP_Text titleText = UIFactory.CreateStyledText("ShareTitle", panel, productionCopyConfig != null ? productionCopyConfig.ShareTitle : "SHARE STATUS", 40, FontStyles.Bold, TextAlignmentOptions.Center, uiThemeConfig, UiTextRole.Heading, theme);
            UIFactory.SetAnchors((RectTransform)titleText.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -64f), new Vector2(460f, 54f));

            summaryText = UIFactory.CreateStyledText("ShareSummary", panel, string.Empty, 28, FontStyles.Bold, TextAlignmentOptions.Center, uiThemeConfig, UiTextRole.Body, theme);
            summaryText.textWrappingMode = TextWrappingModes.Normal;
            UIFactory.SetAnchors((RectTransform)summaryText.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 62f), new Vector2(520f, 72f));

            flavorText = UIFactory.CreateStyledText("ShareFlavor", panel, string.Empty, 24, FontStyles.Normal, TextAlignmentOptions.Center, uiThemeConfig, UiTextRole.Micro, theme);
            flavorText.textWrappingMode = TextWrappingModes.Normal;
            UIFactory.SetAnchors((RectTransform)flavorText.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -22f), new Vector2(560f, 80f));

            statusText = UIFactory.CreateStyledText("ShareStatus", panel, string.Empty, 20, FontStyles.Bold, TextAlignmentOptions.Center, uiThemeConfig, UiTextRole.Status, theme);
            UIFactory.SetAnchors((RectTransform)statusText.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -92f), new Vector2(320f, 34f));

            Button copyButton = UIFactory.CreateStyledButton("ShareCopyButton", panel, productionCopyConfig != null ? productionCopyConfig.ShareCopyButtonLabel : "Copy", uiThemeConfig, UiSurfaceRole.AccentButton, theme, CopyToClipboard);
            UIFactory.SetAnchors((RectTransform)copyButton.transform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(-128f, 40f), new Vector2(220f, 78f));
            copyButtonImage = copyButton.GetComponent<Image>();

            Button closeButton = UIFactory.CreateStyledButton("ShareCloseButton", panel, productionCopyConfig != null ? productionCopyConfig.CloseButtonLabel : "Close", uiThemeConfig, UiSurfaceRole.SecondaryButton, theme, closeAction);
            UIFactory.SetAnchors((RectTransform)closeButton.transform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(128f, 40f), new Vector2(220f, 78f));
            closeButtonImage = closeButton.GetComponent<Image>();

            transitionController = gameObject.AddComponent<OverlayTransitionController>();
            transitionController.Initialize(root, panel, -18f);
        }

        public void ApplyTheme(ThemeConfig theme)
        {
            if (panelImage != null)
            {
                UIFactory.ApplySurfaceStyle(panelImage, uiThemeConfig, UiSurfaceRole.Panel, theme);
            }

            if (copyButtonImage != null)
            {
                UIFactory.ApplySurfaceStyle(copyButtonImage, uiThemeConfig, UiSurfaceRole.AccentButton, theme);
            }

            if (closeButtonImage != null)
            {
                UIFactory.ApplySurfaceStyle(closeButtonImage, uiThemeConfig, UiSurfaceRole.SecondaryButton, theme);
            }
        }

        public void Show(SharePresentationCopy copy)
        {
            currentCopy = copy;
            if (summaryText != null)
            {
                summaryText.text = copy.Summary;
            }

            if (flavorText != null)
            {
                flavorText.text = copy.Flavor;
            }

            if (statusText != null)
            {
                statusText.text = string.Empty;
            }

            transitionController?.Show();
        }

        public void Hide()
        {
            if (statusText != null)
            {
                statusText.text = string.Empty;
            }

            transitionController?.Hide();
        }

        private void CopyToClipboard()
        {
            if (string.IsNullOrWhiteSpace(currentCopy.ClipboardText))
            {
                return;
            }

            GUIUtility.systemCopyBuffer = currentCopy.ClipboardText;
            if (statusText != null)
            {
                statusText.text = productionCopyConfig != null ? productionCopyConfig.ShareCopiedStatusLabel : "Copied";
            }
        }
    }
}
