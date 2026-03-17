using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Voltline.Data;

namespace Voltline.UI
{
    public sealed class ExpandedTutorialOverlayView : MonoBehaviour
    {
        private OverlayTransitionController transitionController;
        private Image panelImage;
        private Image primaryButtonImage;
        private Image closeButtonImage;
        private UIThemeConfig uiThemeConfig;

        public bool IsVisible => transitionController != null && transitionController.IsVisible;

        public void Initialize(Transform parent, ThemeConfig theme, ThemeCatalog themeCatalog, System.Action confirmAction, System.Action closeAction)
        {
            uiThemeConfig = themeCatalog != null ? themeCatalog.UiThemeConfig : null;
            ProductionCopyConfig productionCopyConfig = themeCatalog != null ? themeCatalog.ProductionCopyConfig : null;

            RectTransform root = UIFactory.CreateSurface("TutorialOverlay", parent, uiThemeConfig, UiSurfaceRole.Overlay, theme);
            UIFactory.Stretch(root, 0f);

            RectTransform panel = UIFactory.CreateSurface("TutorialPanel", root, uiThemeConfig, UiSurfaceRole.Panel, theme);
            UIFactory.SetAnchors(panel, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(760f, 700f));
            panelImage = panel.GetComponent<Image>();

            TMP_Text titleText = UIFactory.CreateStyledText("TutorialTitle", panel, productionCopyConfig != null ? productionCopyConfig.TutorialTitle : "KEEP THE GRID ALIVE", 44, FontStyles.Bold, TextAlignmentOptions.Center, uiThemeConfig, UiTextRole.Heading, theme);
            UIFactory.SetAnchors((RectTransform)titleText.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -72f), new Vector2(520f, 58f));

            TMP_Text primaryLine = UIFactory.CreateStyledText("TutorialPrimary", panel, productionCopyConfig != null ? productionCopyConfig.TutorialPrimaryLine : string.Empty, 30, FontStyles.Bold, TextAlignmentOptions.Center, uiThemeConfig, UiTextRole.Body, theme);
            primaryLine.textWrappingMode = TextWrappingModes.Normal;
            UIFactory.SetAnchors((RectTransform)primaryLine.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 92f), new Vector2(590f, 86f));

            TMP_Text secondaryLine = UIFactory.CreateStyledText("TutorialSecondary", panel, productionCopyConfig != null ? productionCopyConfig.TutorialSecondaryLine : string.Empty, 24, FontStyles.Normal, TextAlignmentOptions.Center, uiThemeConfig, UiTextRole.Micro, theme);
            secondaryLine.textWrappingMode = TextWrappingModes.Normal;
            UIFactory.SetAnchors((RectTransform)secondaryLine.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -12f), new Vector2(590f, 90f));

            RectTransform laneHint = UIFactory.CreateSurface("TutorialLaneHint", panel, uiThemeConfig, UiSurfaceRole.HighlightPanel, theme);
            UIFactory.SetAnchors(laneHint, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -126f), new Vector2(520f, 84f));
            RectTransform coreLine = UIFactory.CreatePanel("CoreLine", laneHint, theme != null ? theme.LineCoreColor : Color.white);
            UIFactory.SetAnchors(coreLine, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(360f, 8f));
            RectTransform leftNode = UIFactory.CreatePanel("LeftNode", laneHint, theme != null ? theme.PlayerAccentColor : Color.white);
            UIFactory.SetAnchors(leftNode, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-118f, 0f), new Vector2(40f, 40f));
            RectTransform rightNode = UIFactory.CreatePanel("RightNode", laneHint, WithAlpha(theme != null ? theme.MilestoneColor : Color.white, 0.78f));
            UIFactory.SetAnchors(rightNode, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(118f, 0f), new Vector2(30f, 30f));

            Button startButton = UIFactory.CreateStyledButton("TutorialStartButton", panel, productionCopyConfig != null ? productionCopyConfig.TutorialActionLabel : "Start Run", uiThemeConfig, UiSurfaceRole.PrimaryButton, theme, confirmAction);
            UIFactory.SetAnchors((RectTransform)startButton.transform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 128f), new Vector2(460f, 96f));
            primaryButtonImage = startButton.GetComponent<Image>();

            Button closeButton = UIFactory.CreateStyledButton("TutorialCloseButton", panel, productionCopyConfig != null ? productionCopyConfig.CloseButtonLabel : "Close", uiThemeConfig, UiSurfaceRole.SecondaryButton, theme, closeAction);
            UIFactory.SetAnchors((RectTransform)closeButton.transform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 28f), new Vector2(320f, 72f));
            closeButtonImage = closeButton.GetComponent<Image>();

            transitionController = gameObject.AddComponent<OverlayTransitionController>();
            transitionController.Initialize(root, panel, -20f);
        }

        public void Show()
        {
            transitionController?.Show();
        }

        public void Hide()
        {
            transitionController?.Hide();
        }

        public void ApplyTheme(ThemeConfig theme)
        {
            if (panelImage != null)
            {
                UIFactory.ApplySurfaceStyle(panelImage, uiThemeConfig, UiSurfaceRole.Panel, theme);
            }

            if (primaryButtonImage != null)
            {
                UIFactory.ApplySurfaceStyle(primaryButtonImage, uiThemeConfig, UiSurfaceRole.PrimaryButton, theme);
            }

            if (closeButtonImage != null)
            {
                UIFactory.ApplySurfaceStyle(closeButtonImage, uiThemeConfig, UiSurfaceRole.SecondaryButton, theme);
            }
        }
        private static Color WithAlpha(Color color, float alpha)
        {
            color.a = alpha;
            return color;
        }

    }
}
