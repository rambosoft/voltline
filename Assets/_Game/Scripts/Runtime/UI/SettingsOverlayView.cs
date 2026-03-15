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
        private RectTransform root;
        private VolumeRow musicRow;
        private VolumeRow sfxRow;
        private Button vibrationButton;
        private TMP_Text vibrationButtonLabel;
        private OverlayTransitionController transitionController;
        private readonly List<ThemeRow> themeRows = new();

        public bool IsVisible => transitionController != null && transitionController.IsVisible;

        public void Initialize(Transform parent, ThemeConfig theme, ThemeCatalog catalog, SaveService service, System.Action closeAction)
        {
            saveService = service;
            themeCatalog = catalog;
            root = UIFactory.CreatePanel("SettingsOverlay", parent, new Color(0f, 0f, 0f, 0.56f));
            UIFactory.Stretch(root, 0f);

            RectTransform panel = UIFactory.CreatePanel("SettingsPanel", root, UIFactory.PanelColor(0.96f));
            UIFactory.SetAnchors(panel, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(760f, 1080f));

            TMP_Text title = UIFactory.CreateText("Title", panel, "Settings", 52, FontStyles.Bold, TextAlignmentOptions.Center, Color.white);
            UIFactory.SetAnchors((RectTransform)title.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -82f), new Vector2(560f, 80f));

            musicRow = CreateVolumeRow(panel, "Music", new Vector2(0f, -190f), saveService.MusicVolume, saveService.SetMusicVolume, theme);
            sfxRow = CreateVolumeRow(panel, "SFX", new Vector2(0f, -320f), saveService.SfxVolume, saveService.SetSfxVolume, theme);

            TMP_Text vibrationLabel = UIFactory.CreateText("VibrationLabel", panel, "Vibration", 34, FontStyles.Bold, TextAlignmentOptions.Left, Color.white);
            UIFactory.SetAnchors((RectTransform)vibrationLabel.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(-168f, -448f), new Vector2(220f, 50f));

            vibrationButton = UIFactory.CreateButton("VibrationButton", panel, "On", theme.PlayerAccentColor, new Color(0.08f, 0.08f, 0.12f, 1f), ToggleVibration);
            UIFactory.SetAnchors((RectTransform)vibrationButton.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(164f, -448f), new Vector2(188f, 62f));
            vibrationButtonLabel = vibrationButton.GetComponentInChildren<TMP_Text>();
            if (vibrationButtonLabel != null)
            {
                vibrationButtonLabel.fontSize = 28;
            }

            if (themeCatalog != null && themeCatalog.Themes != null && themeCatalog.Themes.Count > 1)
            {
                TMP_Text themeLabel = UIFactory.CreateText("ThemeLabel", panel, "Theme", 34, FontStyles.Bold, TextAlignmentOptions.Left, Color.white);
                UIFactory.SetAnchors((RectTransform)themeLabel.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(-168f, -600f), new Vector2(220f, 50f));

                TMP_Text themeHint = UIFactory.CreateText("ThemeHint", panel, "Unlocked themes only. Applies on next run.", 22, FontStyles.Normal, TextAlignmentOptions.Left, theme.PlayerAccentColor);
                UIFactory.SetAnchors((RectTransform)themeHint.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(60f, -602f), new Vector2(420f, 42f));

                RectTransform themeSection = UIFactory.CreatePanel("ThemeSection", panel, new Color(1f, 1f, 1f, 0.045f));
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
                    Button themeButton = UIFactory.CreateButton(
                        $"ThemeButton_{i}",
                        content,
                        themeEntry.DisplayName,
                        UIFactory.PanelColor(1f),
                        Color.white,
                        () => SelectTheme(themeId));
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

                    TMP_Text statusText = UIFactory.CreateText($"ThemeStatus_{i}", themeButton.transform, string.Empty, 22, FontStyles.Normal, TextAlignmentOptions.MidlineRight, theme.PlayerAccentColor);
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

            Button closeButton = UIFactory.CreateButton("CloseButton", panel, "Close", theme.DangerColor, Color.white, closeAction);
            UIFactory.SetAnchors((RectTransform)closeButton.transform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 72f), new Vector2(420f, 92f));

            transitionController = gameObject.AddComponent<OverlayTransitionController>();
            transitionController.Initialize(root, panel, -24f);

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
            TMP_Text labelText = UIFactory.CreateText(label + "Label", parent, label, 34, FontStyles.Bold, TextAlignmentOptions.Left, Color.white);
            UIFactory.SetAnchors((RectTransform)labelText.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(-168f, anchoredPosition.y), new Vector2(220f, 50f));

            Button minusButton = UIFactory.CreateButton(label + "Minus", parent, "-", UIFactory.PanelColor(1f), Color.white, null);
            UIFactory.SetAnchors((RectTransform)minusButton.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(30f, anchoredPosition.y), new Vector2(82f, 62f));

            Button plusButton = UIFactory.CreateButton(label + "Plus", parent, "+", UIFactory.PanelColor(1f), Color.white, null);
            UIFactory.SetAnchors((RectTransform)plusButton.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(314f, anchoredPosition.y), new Vector2(82f, 62f));

            TMP_Text valueText = UIFactory.CreateText(label + "Value", parent, Mathf.RoundToInt(initialValue * 100f) + "%", 30, FontStyles.Normal, TextAlignmentOptions.Center, theme.PlayerAccentColor);
            UIFactory.SetAnchors((RectTransform)valueText.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(172f, anchoredPosition.y), new Vector2(180f, 50f));

            System.Func<float> getter = label == "Music"
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
                vibrationButtonLabel.text = saveService.VibrationEnabled ? "On" : "Off";
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
                    row.ButtonImage.color = isSelected
                        ? row.Theme.LineGlowColor
                        : (isUnlocked ? UIFactory.PanelColor(1f) : new Color(0.12f, 0.12f, 0.16f, 1f));
                }

                if (row.LabelText != null)
                {
                    row.LabelText.text = row.Theme.DisplayName;
                    row.LabelText.color = isSelected ? new Color(0.04f, 0.07f, 0.12f, 1f) : Color.white;
                }

                if (row.StatusText != null)
                {
                    if (isSelected)
                    {
                        row.StatusText.text = "Selected";
                    }
                    else if (isUnlocked)
                    {
                        row.StatusText.text = "Unlocked";
                    }
                    else
                    {
                        row.StatusText.text = $"Best {row.Theme.UnlockBestScoreThreshold}";
                    }

                    row.StatusText.color = isSelected
                        ? new Color(0.04f, 0.07f, 0.12f, 0.94f)
                        : (isUnlocked ? row.Theme.PlayerAccentColor : new Color(1f, 1f, 1f, 0.7f));
                }
            }
        }
    }
}
