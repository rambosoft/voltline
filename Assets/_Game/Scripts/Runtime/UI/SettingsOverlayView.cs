using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Voltline.Data;
using Voltline.Save;

namespace Voltline.UI
{
    public sealed class SettingsOverlayView : MonoBehaviour
    {
        private readonly struct VolumeRow
        {
            public readonly TMP_Text ValueText;

            public VolumeRow(TMP_Text valueText)
            {
                ValueText = valueText;
            }
        }

        private sealed class ThemeRow
        {
            public string ThemeId;
            public ThemeConfig Theme;
            public Button Button;
            public TMP_Text LabelText;
            public TMP_Text StatusText;
            public Image ButtonImage;
        }

        private SaveService saveService;
        private ThemeCatalog themeCatalog;
        private BrandingPresentationConfig brandingPresentationConfig;
        private ProductionCopyConfig productionCopyConfig;
        private UIThemeConfig uiThemeConfig;
        private RectTransform root;
        private Image panelImage;
        private TMP_Text themeHintText;
        private Image vibrationButtonImage;
        private TMP_Text vibrationButtonLabel;
        private Image closeButtonImage;
        private VolumeRow musicRow;
        private VolumeRow sfxRow;
        private Button vibrationButton;
        private OverlayTransitionController transitionController;
        private readonly List<ThemeRow> themeRows = new();

        public bool IsVisible => transitionController != null && transitionController.IsVisible;
        public bool HasThemeSelectionSection => themeRows.Count > 0;

        public void Initialize(Transform parent, ThemeConfig theme, ThemeCatalog catalog, SaveService service, System.Action closeAction)
        {
            saveService = service;
            themeCatalog = catalog;
            brandingPresentationConfig = catalog != null ? catalog.BrandingPresentationConfig : null;
            productionCopyConfig = catalog != null ? catalog.ProductionCopyConfig : null;
            uiThemeConfig = catalog != null ? catalog.UiThemeConfig : null;

            root = UIFactory.CreateSurface("SettingsOverlay", parent, uiThemeConfig, UiSurfaceRole.Overlay, theme);
            UIFactory.Stretch(root, 0f);

            RectTransform panel = UIFactory.CreateSurface("SettingsPanel", root, uiThemeConfig, UiSurfaceRole.Panel, theme);
            UIFactory.SetAnchors(panel, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(760f, 1080f));
            panelImage = panel.GetComponent<Image>();

            TMP_Text title = UIFactory.CreateStyledText("Title", panel, productionCopyConfig != null ? productionCopyConfig.SettingsTitle : "Settings", 52, FontStyles.Bold, TextAlignmentOptions.Center, uiThemeConfig, UiTextRole.Heading, theme);
            UIFactory.SetAnchors((RectTransform)title.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -82f), new Vector2(560f, 80f));

            musicRow = CreateVolumeRow(panel, productionCopyConfig != null ? productionCopyConfig.MusicLabel : "Music", new Vector2(0f, -190f), saveService.MusicVolume, saveService.SetMusicVolume, theme);
            sfxRow = CreateVolumeRow(panel, productionCopyConfig != null ? productionCopyConfig.SfxLabel : "SFX", new Vector2(0f, -320f), saveService.SfxVolume, saveService.SetSfxVolume, theme);

            TMP_Text vibrationLabel = UIFactory.CreateStyledText("VibrationLabel", panel, productionCopyConfig != null ? productionCopyConfig.VibrationLabel : "Haptics", 34, FontStyles.Bold, TextAlignmentOptions.Left, uiThemeConfig, UiTextRole.Body, theme);
            UIFactory.SetAnchors((RectTransform)vibrationLabel.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(-168f, -448f), new Vector2(220f, 50f));

            vibrationButton = UIFactory.CreateStyledButton("VibrationButton", panel, productionCopyConfig != null ? productionCopyConfig.OnLabel : "On", uiThemeConfig, UiSurfaceRole.AccentButton, theme, ToggleVibration);
            UIFactory.SetAnchors((RectTransform)vibrationButton.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(164f, -448f), new Vector2(188f, 62f));
            vibrationButtonImage = vibrationButton.GetComponent<Image>();
            vibrationButtonLabel = vibrationButton.GetComponentInChildren<TMP_Text>();
            if (vibrationButtonLabel != null)
            {
                vibrationButtonLabel.fontSize = 28;
            }

            if (themeCatalog != null && themeCatalog.ShouldShowThemeSelectionInSettings)
            {
                TMP_Text themeLabel = UIFactory.CreateStyledText("ThemeLabel", panel, productionCopyConfig != null ? productionCopyConfig.ThemeLabel : "Themes", 34, FontStyles.Bold, TextAlignmentOptions.Left, uiThemeConfig, UiTextRole.Body, theme);
                UIFactory.SetAnchors((RectTransform)themeLabel.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(-168f, -600f), new Vector2(220f, 50f));

                themeHintText = UIFactory.CreateStyledText("ThemeHint", panel, productionCopyConfig != null ? productionCopyConfig.ThemeSelectionHint : "Unlocked themes only. Applies on next run.", 22, FontStyles.Normal, TextAlignmentOptions.Left, uiThemeConfig, UiTextRole.Micro, theme);
                UIFactory.SetAnchors((RectTransform)themeHintText.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(76f, -602f), new Vector2(420f, 42f));

                RectTransform themeSection = UIFactory.CreateSurface("ThemeSection", panel, uiThemeConfig, UiSurfaceRole.HighlightPanel, theme);
                UIFactory.SetAnchors(themeSection, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -790f), new Vector2(560f, 250f));

                GameObject viewportObject = new("ThemeViewport", typeof(RectTransform), typeof(Image), typeof(Mask), typeof(ScrollRect));
                viewportObject.transform.SetParent(themeSection, false);
                RectTransform viewport = (RectTransform)viewportObject.transform;
                UIFactory.Stretch(viewport, 14f);
                Image viewportImage = viewportObject.GetComponent<Image>();
                viewportImage.sprite = Voltline.Utilities.RuntimeSpriteFactory.WhiteSprite;
                viewportImage.type = Image.Type.Sliced;
                viewportImage.color = new Color(0f, 0f, 0f, 0.02f);
                Mask viewportMask = viewportObject.GetComponent<Mask>();
                viewportMask.showMaskGraphic = false;

                RectTransform content = new GameObject("ThemeContent", typeof(RectTransform)).GetComponent<RectTransform>();
                content.SetParent(viewport, false);
                content.anchorMin = new Vector2(0f, 1f);
                content.anchorMax = new Vector2(1f, 1f);
                content.pivot = new Vector2(0.5f, 1f);
                content.anchoredPosition = Vector2.zero;
                content.offsetMin = Vector2.zero;
                content.offsetMax = Vector2.zero;

                ScrollRect scrollRect = viewportObject.GetComponent<ScrollRect>();
                scrollRect.viewport = viewport;
                scrollRect.content = content;
                scrollRect.horizontal = false;
                scrollRect.vertical = true;
                scrollRect.movementType = ScrollRect.MovementType.Clamped;
                scrollRect.scrollSensitivity = 18f;

                float rowHeight = 82f;
                float rowSpacing = 18f;
                float topInset = 8f;
                int rowIndex = 0;
                for (int i = 0; i < themeCatalog.Themes.Count; i++)
                {
                    ThemeConfig themeEntry = themeCatalog.Themes[i];
                    if (themeEntry == null)
                    {
                        continue;
                    }

                    string themeId = themeEntry.ThemeId;
                    Button themeButton = UIFactory.CreateStyledButton($"ThemeButton_{i}", content, themeEntry.DisplayName, uiThemeConfig, UiSurfaceRole.SecondaryButton, theme, () => SelectTheme(themeId));
                    RectTransform themeButtonRect = (RectTransform)themeButton.transform;
                    themeButtonRect.anchorMin = new Vector2(0f, 1f);
                    themeButtonRect.anchorMax = new Vector2(1f, 1f);
                    themeButtonRect.pivot = new Vector2(0.5f, 1f);
                    themeButtonRect.sizeDelta = new Vector2(-16f, rowHeight);
                    themeButtonRect.anchoredPosition = new Vector2(0f, -(topInset + rowIndex * (rowHeight + rowSpacing)));

                    TMP_Text labelText = themeButton.GetComponentInChildren<TMP_Text>();
                    RectTransform labelRect = (RectTransform)labelText.transform;
                    labelRect.anchorMin = new Vector2(0f, 0f);
                    labelRect.anchorMax = new Vector2(1f, 1f);
                    labelRect.offsetMin = new Vector2(28f, 0f);
                    labelRect.offsetMax = new Vector2(-164f, 0f);
                    labelText.alignment = TextAlignmentOptions.MidlineLeft;
                    labelText.fontSize = 32;
                    UIFactory.ApplyTextStyle(labelText, uiThemeConfig, UiTextRole.Body, theme);

                    TMP_Text statusText = UIFactory.CreateStyledText($"ThemeStatus_{i}", themeButton.transform, string.Empty, 22, FontStyles.Normal, TextAlignmentOptions.MidlineRight, uiThemeConfig, UiTextRole.Status, theme);
                    RectTransform statusRect = (RectTransform)statusText.transform;
                    statusRect.anchorMin = new Vector2(1f, 0.5f);
                    statusRect.anchorMax = new Vector2(1f, 0.5f);
                    statusRect.pivot = new Vector2(1f, 0.5f);
                    statusRect.anchoredPosition = new Vector2(-24f, 0f);
                    statusRect.sizeDelta = new Vector2(180f, 34f);

                    themeRows.Add(new ThemeRow
                    {
                        ThemeId = themeId,
                        Theme = themeEntry,
                        Button = themeButton,
                        ButtonImage = themeButton.GetComponent<Image>(),
                        LabelText = labelText,
                        StatusText = statusText,
                    });

                    rowIndex++;
                }

                float contentHeight = Mathf.Max(220f, topInset * 2f + rowIndex * rowHeight + Mathf.Max(0, rowIndex - 1) * rowSpacing);
                content.sizeDelta = new Vector2(0f, contentHeight);
            }

            Button closeButton = UIFactory.CreateStyledButton("CloseButton", panel, productionCopyConfig != null ? productionCopyConfig.CloseButtonLabel : "Close", uiThemeConfig, UiSurfaceRole.DestructiveButton, theme, closeAction);
            UIFactory.SetAnchors((RectTransform)closeButton.transform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 72f), new Vector2(420f, 92f));
            closeButtonImage = closeButton.GetComponent<Image>();

            transitionController = gameObject.AddComponent<OverlayTransitionController>();
            transitionController.Initialize(root, panel, -24f);

            ApplyTheme(theme);
            Refresh();
            saveService.ProfileChanged += Refresh;
        }

        private void OnDestroy()
        {
            if (saveService != null)
            {
                saveService.ProfileChanged -= Refresh;
            }
        }

        public void ApplyTheme(ThemeConfig theme)
        {
            if (theme == null)
            {
                return;
            }

            if (panelImage != null)
            {
                UIFactory.ApplySurfaceStyle(panelImage, uiThemeConfig, UiSurfaceRole.Panel, theme);
            }

            if (themeHintText != null)
            {
                UIFactory.ApplyTextStyle(themeHintText, uiThemeConfig, UiTextRole.Micro, theme);
            }

            if (musicRow.ValueText != null)
            {
                UIFactory.ApplyTextStyle(musicRow.ValueText, uiThemeConfig, UiTextRole.Secondary, theme);
            }

            if (sfxRow.ValueText != null)
            {
                UIFactory.ApplyTextStyle(sfxRow.ValueText, uiThemeConfig, UiTextRole.Secondary, theme);
            }

            if (vibrationButtonImage != null)
            {
                UIFactory.ApplySurfaceStyle(vibrationButtonImage, uiThemeConfig, UiSurfaceRole.AccentButton, theme);
            }

            if (closeButtonImage != null)
            {
                UIFactory.ApplySurfaceStyle(closeButtonImage, uiThemeConfig, UiSurfaceRole.DestructiveButton, theme);
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

        private VolumeRow CreateVolumeRow(Transform parent, string label, Vector2 anchoredPosition, float initialValue, System.Action<float> setter, ThemeConfig theme)
        {
            TMP_Text labelText = UIFactory.CreateStyledText(label + "Label", parent, label, 34, FontStyles.Bold, TextAlignmentOptions.Left, uiThemeConfig, UiTextRole.Body, theme);
            UIFactory.SetAnchors((RectTransform)labelText.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(-168f, anchoredPosition.y), new Vector2(220f, 50f));

            Button minusButton = UIFactory.CreateStyledButton(label + "Minus", parent, "-", uiThemeConfig, UiSurfaceRole.SecondaryButton, theme, null);
            UIFactory.SetAnchors((RectTransform)minusButton.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(30f, anchoredPosition.y), new Vector2(82f, 62f));

            Button plusButton = UIFactory.CreateStyledButton(label + "Plus", parent, "+", uiThemeConfig, UiSurfaceRole.SecondaryButton, theme, null);
            UIFactory.SetAnchors((RectTransform)plusButton.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(314f, anchoredPosition.y), new Vector2(82f, 62f));

            TMP_Text valueText = UIFactory.CreateStyledText(label + "Value", parent, Mathf.RoundToInt(initialValue * 100f) + "%", 30, FontStyles.Normal, TextAlignmentOptions.Center, uiThemeConfig, UiTextRole.Secondary, theme);
            UIFactory.SetAnchors((RectTransform)valueText.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(172f, anchoredPosition.y), new Vector2(180f, 50f));

            System.Func<float> getter = label == (productionCopyConfig != null ? productionCopyConfig.MusicLabel : "Music")
                ? () => saveService.MusicVolume
                : () => saveService.SfxVolume;

            minusButton.onClick.AddListener(() => setter(Mathf.Clamp01(getter() - 0.1f)));
            plusButton.onClick.AddListener(() => setter(Mathf.Clamp01(getter() + 0.1f)));
            return new VolumeRow(valueText);
        }

        private void ToggleVibration()
        {
            saveService.SetVibrationEnabled(!saveService.VibrationEnabled);
        }

        private void SelectTheme(string themeId)
        {
            saveService.SetSelectedThemeId(themeId);
        }

        private void Refresh()
        {
            if (saveService == null)
            {
                return;
            }

            musicRow.ValueText.text = Mathf.RoundToInt(saveService.MusicVolume * 100f) + "%";
            sfxRow.ValueText.text = Mathf.RoundToInt(saveService.SfxVolume * 100f) + "%";
            if (vibrationButtonLabel != null)
            {
                vibrationButtonLabel.text = saveService.VibrationEnabled
                    ? (productionCopyConfig != null ? productionCopyConfig.OnLabel : "On")
                    : (productionCopyConfig != null ? productionCopyConfig.OffLabel : "Off");
            }

            for (int i = 0; i < themeRows.Count; i++)
            {
                ThemeRow row = themeRows[i];
                bool isUnlocked = saveService.IsThemeUnlocked(row.ThemeId);
                bool isSelected = saveService.SelectedThemeId == row.ThemeId;

                if (row.Button != null)
                {
                    row.Button.interactable = isUnlocked;
                }

                if (row.ButtonImage != null)
                {
                    if (isSelected)
                    {
                        UIFactory.ApplySurfaceStyle(row.ButtonImage, uiThemeConfig, UiSurfaceRole.PrimaryButton, row.Theme);
                    }
                    else
                    {
                        row.ButtonImage.color = isUnlocked
                            ? uiThemeConfig.SecondaryButtonColor
                            : new Color(0.12f, 0.12f, 0.16f, 1f);
                    }
                }

                if (row.LabelText != null)
                {
                    row.LabelText.text = row.Theme.DisplayName;
                    row.LabelText.color = isSelected ? new Color(0.04f, 0.07f, 0.12f, 1f) : (uiThemeConfig != null ? uiThemeConfig.TextPrimaryColor : Color.white);
                }

                if (row.StatusText != null)
                {
                    if (isSelected)
                    {
                        row.StatusText.text = productionCopyConfig != null ? productionCopyConfig.SelectedThemeStatusLabel : "Selected";
                    }
                    else if (isUnlocked)
                    {
                        row.StatusText.text = productionCopyConfig != null ? productionCopyConfig.UnlockedThemeStatusLabel : "Unlocked";
                    }
                    else
                    {
                        row.StatusText.text = productionCopyConfig != null ? productionCopyConfig.FormatLockedThemeRequirement(row.Theme.UnlockBestScoreThreshold) : $"Best {row.Theme.UnlockBestScoreThreshold}";
                    }

                    row.StatusText.color = isSelected
                        ? new Color(0.04f, 0.07f, 0.12f, 0.94f)
                        : (isUnlocked ? row.Theme.PlayerAccentColor : new Color(1f, 1f, 1f, 0.7f));
                }
            }
        }
    }
}

