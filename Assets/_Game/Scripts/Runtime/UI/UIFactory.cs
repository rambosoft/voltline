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

        public static RectTransform CreateSurface(string name, Transform parent, UIThemeConfig uiTheme, UiSurfaceRole role, ThemeConfig theme)
        {
            RectTransform rect = CreatePanel(name, parent, ResolveSurfaceColor(uiTheme, role, theme));
            ApplySurfaceStyle(rect.GetComponent<Image>(), uiTheme, role, theme);
            return rect;
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

        public static TMP_Text CreateStyledText(string name, Transform parent, string text, int fontSize, FontStyles fontStyles, TextAlignmentOptions alignment, UIThemeConfig uiTheme, UiTextRole role, ThemeConfig theme)
        {
            TMP_Text textComponent = CreateText(name, parent, text, fontSize, fontStyles, alignment, ResolveTextColor(uiTheme, role, theme));
            ApplyTextStyle(textComponent, uiTheme, role, theme);
            return textComponent;
        }

        public static Image CreateSpriteImage(string name, Transform parent, Sprite sprite, Color color, bool preserveAspect = true)
        {
            RectTransform rect = CreatePanel(name, parent, color);
            Image image = rect.GetComponent<Image>();
            image.sprite = sprite;
            image.preserveAspect = preserveAspect;
            return image;
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

        public static Button CreateStyledButton(string name, Transform parent, string label, UIThemeConfig uiTheme, UiSurfaceRole role, ThemeConfig theme, System.Action onClick)
        {
            Button button = CreateButton(name, parent, label, ResolveSurfaceColor(uiTheme, role, theme), ResolveTextColor(uiTheme, UiTextRole.ButtonLabel, theme), onClick);
            ApplyButtonStyle(button, uiTheme, role, theme);
            TMP_Text labelText = button.GetComponentInChildren<TMP_Text>();
            if (labelText != null)
            {
                ApplyTextStyle(labelText, uiTheme, UiTextRole.ButtonLabel, theme);
            }

            return button;
        }

        public static void ApplyTextStyle(TMP_Text text, UIThemeConfig uiTheme, UiTextRole role, ThemeConfig theme)
        {
            if (text == null)
            {
                return;
            }

            if (uiTheme != null)
            {
                TMP_FontAsset preferredFont = role == UiTextRole.Display || role == UiTextRole.Heading || role == UiTextRole.Score
                    ? uiTheme.DisplayFont
                    : uiTheme.BodyFont;
                if (preferredFont != null)
                {
                    text.font = preferredFont;
                }

                text.color = ResolveTextColor(uiTheme, role, theme);
                text.characterSpacing = role == UiTextRole.Display || role == UiTextRole.Score
                    ? uiTheme.DisplayCharacterSpacing
                    : (role == UiTextRole.Status ? uiTheme.StatusCharacterSpacing : 0f);

                Shadow shadow = GetOrAddComponent<Shadow>(text.gameObject);
                shadow.effectColor = uiTheme.ShadowColor;
                shadow.effectDistance = new Vector2(uiTheme.TextShadowDistance, -uiTheme.TextShadowDistance);
                shadow.useGraphicAlpha = true;
            }
        }

        public static void ApplySurfaceStyle(Image image, UIThemeConfig uiTheme, UiSurfaceRole role, ThemeConfig theme)
        {
            if (image == null)
            {
                return;
            }

            image.color = ResolveSurfaceColor(uiTheme, role, theme);
            if (uiTheme == null)
            {
                return;
            }

            if (role == UiSurfaceRole.Overlay)
            {
                RemoveComponentIfPresent<Outline>(image.gameObject);
                RemoveComponentIfPresent<Shadow>(image.gameObject);
                return;
            }

            Outline outline = GetOrAddComponent<Outline>(image.gameObject);
            outline.effectColor = uiTheme.BorderColor;
            float outlineWidth = role == UiSurfaceRole.Panel || role == UiSurfaceRole.HighlightPanel || role == UiSurfaceRole.Chip
                ? uiTheme.PanelOutlineWidth
                : uiTheme.ButtonOutlineWidth;
            outline.effectDistance = new Vector2(outlineWidth, outlineWidth);
            outline.useGraphicAlpha = true;

            Shadow shadow = GetOrAddComponent<Shadow>(image.gameObject);
            shadow.effectColor = uiTheme.ShadowColor;
            shadow.effectDistance = uiTheme.ShadowDistance;
            shadow.useGraphicAlpha = true;
        }

        public static void ApplyButtonStyle(Button button, UIThemeConfig uiTheme, UiSurfaceRole role, ThemeConfig theme)
        {
            if (button == null)
            {
                return;
            }

            Image image = button.GetComponent<Image>();
            ApplySurfaceStyle(image, uiTheme, role, theme);

            ColorBlock colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1f, 1f, 1f, 0.985f);
            colors.pressedColor = new Color(0.9f, 0.9f, 0.9f, 0.92f);
            colors.selectedColor = colors.highlightedColor;
            colors.disabledColor = new Color(1f, 1f, 1f, 0.45f);
            button.colors = colors;
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

        private static Color ResolveSurfaceColor(UIThemeConfig uiTheme, UiSurfaceRole role, ThemeConfig theme)
        {
            if (uiTheme == null)
            {
                return role switch
                {
                    UiSurfaceRole.Overlay => new Color(0f, 0f, 0f, 0.56f),
                    UiSurfaceRole.PrimaryButton => AccentColor(theme),
                    UiSurfaceRole.DestructiveButton => DangerColor(theme),
                    UiSurfaceRole.AccentButton => theme != null ? theme.PlayerAccentColor : new Color(1f, 0.92f, 0.28f, 1f),
                    _ => PanelColor(0.95f),
                };
            }

            return role switch
            {
                UiSurfaceRole.Overlay => uiTheme.OverlayColor,
                UiSurfaceRole.Panel => uiTheme.PanelColor,
                UiSurfaceRole.HighlightPanel => uiTheme.HighlightPanelColor,
                UiSurfaceRole.Chip => uiTheme.ChipColor,
                UiSurfaceRole.PrimaryButton => Color.Lerp(uiTheme.PrimaryButtonColor, AccentColor(theme), 0.35f),
                UiSurfaceRole.SecondaryButton => uiTheme.SecondaryButtonColor,
                UiSurfaceRole.DestructiveButton => Color.Lerp(uiTheme.DestructiveButtonColor, DangerColor(theme), 0.45f),
                UiSurfaceRole.AccentButton => Color.Lerp(uiTheme.AccentButtonColor, theme != null ? theme.PlayerAccentColor : uiTheme.AccentButtonColor, 0.35f),
                _ => uiTheme.PanelColor,
            };
        }

        private static Color ResolveTextColor(UIThemeConfig uiTheme, UiTextRole role, ThemeConfig theme)
        {
            if (uiTheme == null)
            {
                return role switch
                {
                    UiTextRole.Secondary => theme != null ? theme.PlayerAccentColor : Color.white,
                    UiTextRole.Status => theme != null ? theme.PlayerAccentColor : Color.white,
                    _ => Color.white,
                };
            }

            return role switch
            {
                UiTextRole.Secondary => Color.Lerp(uiTheme.TextSecondaryColor, theme != null ? theme.PlayerAccentColor : uiTheme.TextSecondaryColor, 0.3f),
                UiTextRole.Status => Color.Lerp(uiTheme.TextMutedColor, theme != null ? theme.PlayerAccentColor : uiTheme.TextMutedColor, 0.4f),
                UiTextRole.Micro => uiTheme.TextMutedColor,
                _ => uiTheme.TextPrimaryColor,
            };
        }

        private static T GetOrAddComponent<T>(GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>();
            return component != null ? component : gameObject.AddComponent<T>();
        }

        private static void RemoveComponentIfPresent<T>(GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>();
            if (component != null)
            {
                Object.Destroy(component);
            }
        }
    }
}
