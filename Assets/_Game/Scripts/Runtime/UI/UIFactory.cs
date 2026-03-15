using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using Voltline.Audio;
using Voltline.Data;
using Voltline.Utilities;

namespace Voltline.UI
{
    public static class UIFactory
    {
        public static Canvas CreateCanvas(string name, Transform parent)
        {
            GameObject root = new(name, typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            root.transform.SetParent(parent, false);

            Canvas canvas = root.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.pixelPerfect = false;
            canvas.sortingOrder = 100;

            CanvasScaler scaler = root.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080f, 1920f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            return canvas;
        }

        public static RectTransform CreateSafeAreaRoot(Canvas canvas)
        {
            GameObject root = new("SafeAreaRoot", typeof(RectTransform), typeof(SafeAreaFitter));
            root.transform.SetParent(canvas.transform, false);
            RectTransform rect = (RectTransform)root.transform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            return rect;
        }

        public static RectTransform CreatePanel(string name, Transform parent, Color color)
        {
            GameObject panel = new(name, typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(parent, false);
            Image image = panel.GetComponent<Image>();
            image.sprite = RuntimeSpriteFactory.WhiteSprite;
            image.type = Image.Type.Sliced;
            image.color = color;
            return (RectTransform)panel.transform;
        }

        public static TMP_Text CreateText(string name, Transform parent, string text, int fontSize, FontStyles fontStyles, TextAlignmentOptions alignment, Color color)
        {
            GameObject textRoot = new(name, typeof(RectTransform), typeof(TextMeshProUGUI));
            textRoot.transform.SetParent(parent, false);
            TMP_Text textComponent = textRoot.GetComponent<TMP_Text>();
            textComponent.text = text;
            textComponent.fontSize = fontSize;
            textComponent.fontStyle = fontStyles;
            textComponent.alignment = alignment;
            textComponent.color = color;
            textComponent.textWrappingMode = TextWrappingModes.NoWrap;
            return textComponent;
        }

        public static Button CreateButton(string name, Transform parent, string label, Color fillColor, Color textColor, System.Action onClick)
        {
            GameObject buttonRoot = new(name, typeof(RectTransform), typeof(Image), typeof(Button));
            buttonRoot.transform.SetParent(parent, false);

            Image image = buttonRoot.GetComponent<Image>();
            image.sprite = RuntimeSpriteFactory.WhiteSprite;
            image.type = Image.Type.Sliced;
            image.color = fillColor;

            Button button = buttonRoot.GetComponent<Button>();
            ColorBlock colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1f, 1f, 1f, 0.96f);
            colors.pressedColor = new Color(0.92f, 0.92f, 0.92f, 0.9f);
            colors.selectedColor = colors.highlightedColor;
            colors.disabledColor = new Color(1f, 1f, 1f, 0.45f);
            button.colors = colors;
            if (onClick != null)
            {
                button.onClick.AddListener(() => AudioService.EnsureExists().PlayButtonClick());
                button.onClick.AddListener(() => onClick());
            }

            TMP_Text labelText = CreateText("Label", buttonRoot.transform, label, 40, FontStyles.Bold, TextAlignmentOptions.Center, textColor);
            RectTransform labelRect = (RectTransform)labelText.transform;
            Stretch(labelRect, 0f);
            return button;
        }

        public static void EnsureEventSystem()
        {
            if (Object.FindFirstObjectByType<EventSystem>() != null)
            {
                return;
            }

            GameObject eventSystemRoot = new("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            Object.DontDestroyOnLoad(eventSystemRoot);
        }

        public static void Stretch(RectTransform rectTransform, float padding)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = new Vector2(padding, padding);
            rectTransform.offsetMax = new Vector2(-padding, -padding);
        }

        public static void SetAnchors(RectTransform rectTransform, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 sizeDelta)
        {
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.sizeDelta = sizeDelta;
        }

        public static Color PanelColor(float alpha = 0.82f)
        {
            return new Color(0.07f, 0.09f, 0.15f, alpha);
        }

        public static Color AccentColor(ThemeConfig theme)
        {
            return theme != null ? theme.LineGlowColor : new Color(0.06f, 0.68f, 1f, 1f);
        }

        public static Color DangerColor(ThemeConfig theme)
        {
            return theme != null ? theme.DangerColor : new Color(1f, 0.23f, 0.43f, 1f);
        }
    }
}
