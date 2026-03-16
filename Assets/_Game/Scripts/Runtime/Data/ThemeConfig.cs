using UnityEngine;

namespace Voltline.Data
{
    [CreateAssetMenu(fileName = "THM_NeonNight", menuName = "Voltline/Config/Theme")]
    public sealed class ThemeConfig : ScriptableObject
    {
        [SerializeField] private string themeId = "theme.neon-night";
        [SerializeField] private string displayName = "Neon Night";
        [SerializeField] private bool unlockedByDefault = true;
        [SerializeField] private int unlockBestScoreThreshold;
        [SerializeField] private Color backgroundTopColor = new(0.015f, 0.028f, 0.07f, 1f);
        [SerializeField] private Color backgroundBottomColor = new(0.03f, 0.005f, 0.085f, 1f);
        [SerializeField] private Color lineCoreColor = new(0.2f, 0.95f, 1f, 1f);
        [SerializeField] private Color lineGlowColor = new(0.06f, 0.68f, 1f, 1f);
        [SerializeField] private Color playerAccentColor = new(1f, 0.92f, 0.28f, 1f);
        [SerializeField] private Color dangerColor = new(1f, 0.23f, 0.43f, 1f);
        [SerializeField] private Color milestoneColor = new(0.62f, 0.44f, 1f, 1f);
        [SerializeField] private PlayerVisualConfig playerVisualOverride;
        [SerializeField] private ObstacleVisualCatalog obstacleVisualOverride;
        [SerializeField] private BackgroundPresentationConfig backgroundPresentationOverride;
        [SerializeField] private ThemeVfxProfile themeVfxProfile;
        [SerializeField] private ThemeAudioProfile themeAudioProfile;
        [SerializeField] private bool allowRuntimeSequenceSelection = true;
        [SerializeField] private float preferredTransitionDuration = 0.32f;

        public string ThemeId => themeId;
        public string DisplayName => displayName;
        public bool UnlockedByDefault => unlockedByDefault;
        public int UnlockBestScoreThreshold => unlockBestScoreThreshold;
        public Color BackgroundTopColor => backgroundTopColor;
        public Color BackgroundBottomColor => backgroundBottomColor;
        public Color LineCoreColor => lineCoreColor;
        public Color LineGlowColor => lineGlowColor;
        public Color PlayerAccentColor => playerAccentColor;
        public Color DangerColor => dangerColor;
        public Color MilestoneColor => milestoneColor;
        public PlayerVisualConfig PlayerVisualOverride => playerVisualOverride;
        public ObstacleVisualCatalog ObstacleVisualOverride => obstacleVisualOverride;
        public BackgroundPresentationConfig BackgroundPresentationOverride => backgroundPresentationOverride;
        public ThemeVfxProfile ThemeVfxProfile => themeVfxProfile;
        public ThemeAudioProfile ThemeAudioProfile => themeAudioProfile;
        public bool AllowRuntimeSequenceSelection => allowRuntimeSequenceSelection;
        public float PreferredTransitionDuration => preferredTransitionDuration;

        public PlayerVisualConfig ResolvePlayerVisual(PlayerVisualConfig fallback)
        {
            return playerVisualOverride != null ? playerVisualOverride : fallback;
        }

        public ObstacleVisualCatalog ResolveObstacleVisualCatalog(ObstacleVisualCatalog fallback)
        {
            return obstacleVisualOverride != null ? obstacleVisualOverride : fallback;
        }

        public BackgroundPresentationConfig ResolveBackgroundPresentation(BackgroundPresentationConfig fallback)
        {
            return backgroundPresentationOverride != null ? backgroundPresentationOverride : fallback;
        }

        public ThemeVfxProfile ResolveThemeVfxProfile(ThemeVfxProfile fallback)
        {
            return themeVfxProfile != null ? themeVfxProfile : fallback;
        }

        public ThemeAudioProfile ResolveThemeAudioProfile(ThemeAudioProfile fallback)
        {
            return themeAudioProfile != null ? themeAudioProfile : fallback;
        }
    }
}
