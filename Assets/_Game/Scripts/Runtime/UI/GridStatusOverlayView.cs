using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Voltline.Data;
using Voltline.Save;

namespace Voltline.UI
{
    public sealed class GridStatusOverlayView : MonoBehaviour
    {
        private sealed class DistrictCardView
        {
            public WorldDistrictStateDefinition District;
            public Image PanelImage;
            public TMP_Text RangeText;
            public TMP_Text StateText;
        }

        private readonly List<DistrictCardView> districtCards = new();

        private ThemeConfig theme;
        private SaveService saveService;
        private WorldProgressionConfig worldProgressionConfig;
        private UIThemeConfig uiThemeConfig;
        private ProductionCopyConfig productionCopyConfig;
        private OverlayTransitionController transitionController;
        private Image panelImage;
        private TMP_Text summaryText;
        private TMP_Text currentDistrictText;
        private Image closeButtonImage;

        public bool IsVisible => transitionController != null && transitionController.IsVisible;

        public void Initialize(Transform parent, ThemeConfig activeTheme, ThemeCatalog themeCatalog, SaveService service, System.Action closeAction)
        {
            theme = activeTheme;
            saveService = service;
            worldProgressionConfig = activeTheme != null ? activeTheme.ResolveWorldProgressionConfig() : null;
            uiThemeConfig = themeCatalog != null ? themeCatalog.UiThemeConfig : null;
            productionCopyConfig = themeCatalog != null ? themeCatalog.ProductionCopyConfig : null;

            RectTransform root = UIFactory.CreateSurface("GridStatusOverlay", parent, uiThemeConfig, UiSurfaceRole.Overlay, theme);
            UIFactory.Stretch(root, 0f);

            RectTransform panel = UIFactory.CreateSurface("GridStatusPanel", root, uiThemeConfig, UiSurfaceRole.Panel, theme);
            UIFactory.SetAnchors(panel, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(820f, 1180f));
            panelImage = panel.GetComponent<Image>();

            TMP_Text titleText = UIFactory.CreateStyledText("GridStatusTitle", panel, productionCopyConfig != null ? productionCopyConfig.GridStatusTitle : "GRID STATUS", 46, FontStyles.Bold, TextAlignmentOptions.Center, uiThemeConfig, UiTextRole.Heading, theme);
            UIFactory.SetAnchors((RectTransform)titleText.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -82f), new Vector2(520f, 58f));

            summaryText = UIFactory.CreateStyledText("GridStatusSummary", panel, string.Empty, 26, FontStyles.Normal, TextAlignmentOptions.Center, uiThemeConfig, UiTextRole.Body, theme);
            UIFactory.SetAnchors((RectTransform)summaryText.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -148f), new Vector2(620f, 44f));

            currentDistrictText = UIFactory.CreateStyledText("GridStatusCurrentDistrict", panel, string.Empty, 24, FontStyles.Bold, TextAlignmentOptions.Center, uiThemeConfig, UiTextRole.Status, theme);
            UIFactory.SetAnchors((RectTransform)currentDistrictText.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -194f), new Vector2(620f, 38f));

            RectTransform cardsRoot = new GameObject("GridStatusCards", typeof(RectTransform)).GetComponent<RectTransform>();
            cardsRoot.SetParent(panel, false);
            UIFactory.SetAnchors(cardsRoot, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -18f), new Vector2(660f, 720f));

            if (worldProgressionConfig != null)
            {
                for (int i = 0; i < worldProgressionConfig.DistrictStates.Count; i++)
                {
                    WorldDistrictStateDefinition district = worldProgressionConfig.DistrictStates[i];
                    if (district == null)
                    {
                        continue;
                    }

                    RectTransform card = UIFactory.CreateSurface($"DistrictCard_{i}", cardsRoot, uiThemeConfig, UiSurfaceRole.HighlightPanel, theme);
                    UIFactory.SetAnchors(card, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -(i * 140f)), new Vector2(620f, 112f));

                    TMP_Text nameText = UIFactory.CreateStyledText("Name", card, district.DisplayName, 30, FontStyles.Bold, TextAlignmentOptions.TopLeft, uiThemeConfig, UiTextRole.Body, theme);
                    RectTransform nameRect = (RectTransform)nameText.transform;
                    nameRect.anchorMin = new Vector2(0f, 1f);
                    nameRect.anchorMax = new Vector2(1f, 1f);
                    nameRect.pivot = new Vector2(0f, 1f);
                    nameRect.anchoredPosition = new Vector2(26f, -18f);
                    nameRect.sizeDelta = new Vector2(-180f, 36f);

                    TMP_Text rangeText = UIFactory.CreateStyledText("Range", card, string.Empty, 20, FontStyles.Normal, TextAlignmentOptions.BottomLeft, uiThemeConfig, UiTextRole.Micro, theme);
                    RectTransform rangeRect = (RectTransform)rangeText.transform;
                    rangeRect.anchorMin = new Vector2(0f, 0f);
                    rangeRect.anchorMax = new Vector2(1f, 0f);
                    rangeRect.pivot = new Vector2(0f, 0f);
                    rangeRect.anchoredPosition = new Vector2(26f, 18f);
                    rangeRect.sizeDelta = new Vector2(-180f, 28f);

                    TMP_Text stateText = UIFactory.CreateStyledText("State", card, string.Empty, 22, FontStyles.Bold, TextAlignmentOptions.MidlineRight, uiThemeConfig, UiTextRole.Status, theme);
                    RectTransform stateRect = (RectTransform)stateText.transform;
                    stateRect.anchorMin = new Vector2(1f, 0.5f);
                    stateRect.anchorMax = new Vector2(1f, 0.5f);
                    stateRect.pivot = new Vector2(1f, 0.5f);
                    stateRect.anchoredPosition = new Vector2(-26f, 0f);
                    stateRect.sizeDelta = new Vector2(180f, 32f);

                    districtCards.Add(new DistrictCardView
                    {
                        District = district,
                        PanelImage = card.GetComponent<Image>(),
                        RangeText = rangeText,
                        StateText = stateText,
                    });
                }
            }

            Button closeButton = UIFactory.CreateStyledButton("GridStatusCloseButton", panel, productionCopyConfig != null ? productionCopyConfig.CloseButtonLabel : "Close", uiThemeConfig, UiSurfaceRole.SecondaryButton, theme, closeAction);
            UIFactory.SetAnchors((RectTransform)closeButton.transform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 48f), new Vector2(360f, 86f));
            closeButtonImage = closeButton.GetComponent<Image>();

            transitionController = gameObject.AddComponent<OverlayTransitionController>();
            transitionController.Initialize(root, panel, -20f);
        }

        public void ApplyTheme(ThemeConfig activeTheme)
        {
            theme = activeTheme;
            if (panelImage != null)
            {
                UIFactory.ApplySurfaceStyle(panelImage, uiThemeConfig, UiSurfaceRole.Panel, theme);
            }

            if (closeButtonImage != null)
            {
                UIFactory.ApplySurfaceStyle(closeButtonImage, uiThemeConfig, UiSurfaceRole.SecondaryButton, theme);
            }
        }

        public void Show()
        {
            Refresh();
            transitionController?.Show();
        }

        public void Hide()
        {
            transitionController?.Hide();
        }

        public void Refresh()
        {
            if (worldProgressionConfig == null)
            {
                return;
            }

            int bestScore = saveService != null ? saveService.BestScore : 0;
            int districtsOnline = worldProgressionConfig.GetRestoredDistrictCount(bestScore);
            WorldDistrictStateDefinition currentDistrict = worldProgressionConfig.GetRequiredDistrictForScore(bestScore);

            if (summaryText != null)
            {
                summaryText.text = productionCopyConfig != null
                    ? productionCopyConfig.FormatGridStatusSummary(bestScore, districtsOnline)
                    : $"Best {bestScore}";
            }

            if (currentDistrictText != null)
            {
                currentDistrictText.text = currentDistrict != null ? currentDistrict.StatusLabel : string.Empty;
            }

            for (int i = 0; i < districtCards.Count; i++)
            {
                DistrictCardView card = districtCards[i];
                if (card == null || card.District == null)
                {
                    continue;
                }

                bool isUnlocked = bestScore >= card.District.MinScore;
                bool isCurrent = currentDistrict == card.District;

                if (card.RangeText != null)
                {
                    card.RangeText.text = productionCopyConfig != null
                        ? productionCopyConfig.FormatGridStatusScoreBand(card.District.MinScore, card.District.MaxScoreInclusive)
                        : card.District.MinScore.ToString();
                }

                if (card.StateText != null)
                {
                    if (!isUnlocked)
                    {
                        card.StateText.text = productionCopyConfig != null
                            ? productionCopyConfig.FormatGridStatusLockedRequirement(card.District.MinScore)
                            : $"Reach {card.District.MinScore}";
                        card.StateText.color = new Color(1f, 1f, 1f, 0.68f);
                    }
                    else if (isCurrent)
                    {
                        card.StateText.text = productionCopyConfig != null ? productionCopyConfig.GridStatusCurrentLabel : "CURRENT";
                        card.StateText.color = theme != null ? theme.PlayerAccentColor : Color.white;
                    }
                    else
                    {
                        card.StateText.text = productionCopyConfig != null ? productionCopyConfig.GridStatusOnlineLabel : "ONLINE";
                        card.StateText.color = theme != null ? theme.MilestoneColor : Color.white;
                    }
                }

                if (card.PanelImage != null)
                {
                    if (!isUnlocked)
                    {
                        card.PanelImage.color = new Color(0.11f, 0.13f, 0.18f, 0.8f);
                    }
                    else if (isCurrent)
                    {
                        card.PanelImage.color = WithAlpha(theme != null ? theme.LineGlowColor : Color.white, 0.22f);
                    }
                    else
                    {
                        card.PanelImage.color = WithAlpha(theme != null ? theme.MilestoneColor : Color.white, 0.14f);
                    }
                }
            }
        }
        private static Color WithAlpha(Color color, float alpha)
        {
            color.a = alpha;
            return color;
        }

    }
}
