using TMPro;
using UnityEngine;

namespace Voltline.Data
{
    public enum UiTextRole
    {
        Display,
        Heading,
        Body,
        Secondary,
        Status,
        Score,
        ButtonLabel,
        Micro,
    }

    public enum UiSurfaceRole
    {
        Overlay,
        Panel,
        HighlightPanel,
        Chip,
        PrimaryButton,
        SecondaryButton,
        DestructiveButton,
        AccentButton,
    }

    [CreateAssetMenu(fileName = "CFG_UITheme_LiveWireCity", menuName = "Voltline/Config/UI Theme")]
    public sealed class UIThemeConfig : ScriptableObject
    {
        [SerializeField] private string themeId = "ui.live-wire-city";
        [SerializeField] private TMP_FontAsset displayFont;
        [SerializeField] private TMP_FontAsset bodyFont;
        [SerializeField] private Color overlayColor = new(0f, 0f, 0f, 0.56f);
        [SerializeField] private Color panelColor = new(0.031f, 0.071f, 0.122f, 0.92f);
        [SerializeField] private Color highlightPanelColor = new(1f, 1f, 1f, 0.04f);
        [SerializeField] private Color chipColor = new(0.043f, 0.094f, 0.161f, 0.96f);
        [SerializeField] private Color primaryButtonColor = new(0.06f, 0.68f, 1f, 1f);
        [SerializeField] private Color secondaryButtonColor = new(0.055f, 0.08f, 0.12f, 0.96f);
        [SerializeField] private Color destructiveButtonColor = new(0.75f, 0.18f, 0.35f, 0.96f);
        [SerializeField] private Color accentButtonColor = new(0.12f, 0.2f, 0.3f, 0.96f);
        [SerializeField] private Color borderColor = new(0.302f, 0.953f, 1f, 0.22f);
        [SerializeField] private Color textPrimaryColor = new(0.917f, 0.956f, 1f, 1f);
        [SerializeField] private Color textSecondaryColor = new(0.82f, 0.88f, 0.95f, 1f);
        [SerializeField] private Color textMutedColor = new(0.498f, 0.584f, 0.667f, 1f);
        [SerializeField] private Color shadowColor = new(0.012f, 0.02f, 0.04f, 0.66f);
        [SerializeField] private Vector2 shadowDistance = new(0f, -6f);
        [SerializeField] private float panelOutlineWidth = 2f;
        [SerializeField] private float buttonOutlineWidth = 2f;
        [SerializeField] private float textShadowDistance = 2f;
        [SerializeField] private float displayCharacterSpacing = 2f;
        [SerializeField] private float statusCharacterSpacing = 1f;

        public string ThemeId => themeId;
        public TMP_FontAsset DisplayFont => displayFont;
        public TMP_FontAsset BodyFont => bodyFont;
        public Color OverlayColor => overlayColor;
        public Color PanelColor => panelColor;
        public Color HighlightPanelColor => highlightPanelColor;
        public Color ChipColor => chipColor;
        public Color PrimaryButtonColor => primaryButtonColor;
        public Color SecondaryButtonColor => secondaryButtonColor;
        public Color DestructiveButtonColor => destructiveButtonColor;
        public Color AccentButtonColor => accentButtonColor;
        public Color BorderColor => borderColor;
        public Color TextPrimaryColor => textPrimaryColor;
        public Color TextSecondaryColor => textSecondaryColor;
        public Color TextMutedColor => textMutedColor;
        public Color ShadowColor => shadowColor;
        public Vector2 ShadowDistance => shadowDistance;
        public float PanelOutlineWidth => panelOutlineWidth;
        public float ButtonOutlineWidth => buttonOutlineWidth;
        public float TextShadowDistance => textShadowDistance;
        public float DisplayCharacterSpacing => displayCharacterSpacing;
        public float StatusCharacterSpacing => statusCharacterSpacing;
    }
}
