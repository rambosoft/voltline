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
            public readonly System.Action<float> Setter;
            public readonly System.Func<float> Getter;

            public VolumeRow(TMP_Text valueText, System.Func<float> getter, System.Action<float> setter)
            {
                ValueText = valueText;
                Getter = getter;
                Setter = setter;
            }
        }

        private SaveService saveService;
        private RectTransform root;
        private VolumeRow musicRow;
        private VolumeRow sfxRow;
        private TMP_Text vibrationValue;

        public bool IsVisible => root != null && root.gameObject.activeSelf;

        public void Initialize(Transform parent, ThemeConfig theme, SaveService service, System.Action closeAction)
        {
            saveService = service;
            root = UIFactory.CreatePanel("SettingsOverlay", parent, new Color(0f, 0f, 0f, 0.56f));
            UIFactory.Stretch(root, 0f);
            root.gameObject.SetActive(false);

            RectTransform panel = UIFactory.CreatePanel("SettingsPanel", root, UIFactory.PanelColor(0.94f));
            UIFactory.SetAnchors(panel, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(720f, 780f));

            TMP_Text title = UIFactory.CreateText("Title", panel, "Settings", 52, FontStyles.Bold, TextAlignmentOptions.Center, Color.white);
            UIFactory.SetAnchors((RectTransform)title.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -88f), new Vector2(540f, 80f));

            musicRow = CreateVolumeRow(panel, "Music", new Vector2(0f, -220f), saveService.MusicVolume, saveService.SetMusicVolume, theme);
            sfxRow = CreateVolumeRow(panel, "SFX", new Vector2(0f, -360f), saveService.SfxVolume, saveService.SetSfxVolume, theme);

            TMP_Text vibrationLabel = UIFactory.CreateText("VibrationLabel", panel, "Vibration", 34, FontStyles.Bold, TextAlignmentOptions.Left, Color.white);
            UIFactory.SetAnchors((RectTransform)vibrationLabel.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(-160f, -500f), new Vector2(220f, 50f));

            Button toggleButton = UIFactory.CreateButton("VibrationButton", panel, "Toggle", theme.PlayerAccentColor, new Color(0.08f, 0.08f, 0.12f, 1f), ToggleVibration);
            UIFactory.SetAnchors((RectTransform)toggleButton.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(160f, -500f), new Vector2(180f, 62f));
            vibrationValue = UIFactory.CreateText("VibrationValue", panel, string.Empty, 28, FontStyles.Normal, TextAlignmentOptions.Left, theme.PlayerAccentColor);
            UIFactory.SetAnchors((RectTransform)vibrationValue.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(-60f, -560f), new Vector2(320f, 42f));

            Button closeButton = UIFactory.CreateButton("CloseButton", panel, "Close", theme.DangerColor, Color.white, closeAction);
            UIFactory.SetAnchors((RectTransform)closeButton.transform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 86f), new Vector2(420f, 92f));

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
            if (root != null)
            {
                Refresh();
                root.gameObject.SetActive(true);
            }
        }

        public void Hide()
        {
            if (root != null)
            {
                root.gameObject.SetActive(false);
            }
        }

        private VolumeRow CreateVolumeRow(Transform parent, string label, Vector2 anchoredPosition, float initialValue, System.Action<float> setter, ThemeConfig theme)
        {
            TMP_Text labelText = UIFactory.CreateText(label + "Label", parent, label, 34, FontStyles.Bold, TextAlignmentOptions.Left, Color.white);
            UIFactory.SetAnchors((RectTransform)labelText.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(-160f, anchoredPosition.y), new Vector2(220f, 50f));

            Button minusButton = UIFactory.CreateButton(label + "Minus", parent, "-", UIFactory.PanelColor(1f), Color.white, null);
            UIFactory.SetAnchors((RectTransform)minusButton.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(20f, anchoredPosition.y), new Vector2(78f, 62f));

            Button plusButton = UIFactory.CreateButton(label + "Plus", parent, "+", UIFactory.PanelColor(1f), Color.white, null);
            UIFactory.SetAnchors((RectTransform)plusButton.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(300f, anchoredPosition.y), new Vector2(78f, 62f));

            TMP_Text valueText = UIFactory.CreateText(label + "Value", parent, Mathf.RoundToInt(initialValue * 100f) + "%", 30, FontStyles.Normal, TextAlignmentOptions.Center, theme.PlayerAccentColor);
            UIFactory.SetAnchors((RectTransform)valueText.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(160f, anchoredPosition.y), new Vector2(180f, 50f));

            System.Func<float> getter = label == "Music"
                ? () => saveService.MusicVolume
                : () => saveService.SfxVolume;

            minusButton.onClick.AddListener(() => setter(Mathf.Clamp01(getter() - 0.1f)));
            plusButton.onClick.AddListener(() => setter(Mathf.Clamp01(getter() + 0.1f)));
            return new VolumeRow(valueText, getter, setter);
        }

        private void ToggleVibration()
        {
            saveService.SetVibrationEnabled(!saveService.VibrationEnabled);
        }

        private void Refresh()
        {
            if (saveService == null)
            {
                return;
            }

            musicRow.ValueText.text = Mathf.RoundToInt(saveService.MusicVolume * 100f) + "%";
            sfxRow.ValueText.text = Mathf.RoundToInt(saveService.SfxVolume * 100f) + "%";
            vibrationValue.text = saveService.VibrationEnabled ? "On" : "Off";
        }
    }
}
