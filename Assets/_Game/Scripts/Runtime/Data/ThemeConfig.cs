using UnityEngine;

namespace Voltline.Data
{
    [CreateAssetMenu(fileName = "THM_LiveWireCity", menuName = "Voltline/Config/Theme")]
    public sealed class ThemeConfig : ScriptableObject
    {
        [SerializeField] private string themeId = "theme.live-wire-city";
        [SerializeField] private string displayName = "Live Wire City";
        [SerializeField] private bool unlockedByDefault = true;
        [SerializeField] private int unlockBestScoreThreshold;
        [SerializeField] private Color backgroundTopColor = new(0.012f, 0.027f, 0.06f, 1f);
        [SerializeField] private Color backgroundBottomColor = new(0.024f, 0.048f, 0.104f, 1f);
        [SerializeField] private Color lineCoreColor = new(0.188f, 0.941f, 1f, 1f);
        [SerializeField] private Color lineGlowColor = new(0.067f, 0.565f, 1f, 1f);
        [SerializeField] private Color playerAccentColor = new(0.918f, 0.957f, 1f, 1f);
        [SerializeField] private Color dangerColor = new(1f, 0.239f, 0.557f, 1f);
        [SerializeField] private Color milestoneColor = new(1f, 0.847f, 0.357f, 1f);
        [SerializeField] private PlayerVisualConfig playerVisualOverride;
        [SerializeField] private ObstacleVisualCatalog obstacleVisualOverride;
        [SerializeField] private BackgroundPresentationConfig backgroundPresentationOverride;
        [SerializeField] private ThemeVfxProfile themeVfxProfile;
        [SerializeField] private ThemeAudioProfile themeAudioProfile;
        [SerializeField] private WorldProgressionConfig worldProgressionConfig;
        [SerializeField] private bool allowRuntimeSequenceSelection;
        [SerializeField] private float preferredTransitionDuration = 0.28f;

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
        public WorldProgressionConfig WorldProgressionConfig => worldProgressionConfig;
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

        public WorldProgressionConfig ResolveWorldProgressionConfig(WorldProgressionConfig fallback = null)
        {
            return worldProgressionConfig != null ? worldProgressionConfig : fallback;
        }
    }
}
