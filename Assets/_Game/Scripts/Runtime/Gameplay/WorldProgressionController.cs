using UnityEngine;
using Voltline.Data;

namespace Voltline.Gameplay
{
    public sealed class WorldProgressionController : MonoBehaviour
    {
        private WorldProgressionConfig config;
        private ThemeConfig activeTheme;
        private WorldDistrictStateDefinition currentDistrict;
        private bool isInitialized;

        public WorldProgressionConfig Config => config;
        public ThemeConfig ActiveTheme => activeTheme;
        public WorldDistrictStateDefinition CurrentDistrict => currentDistrict;
        public string CurrentDistrictId => currentDistrict != null ? currentDistrict.DistrictId : string.Empty;
        public int ActivatedTransitionCount { get; private set; }
        public int ActivatedMilestoneReactionCount { get; private set; }

        public event System.Action<WorldDistrictStateDefinition, float, bool> DistrictChanged;
        public event System.Action<WorldMilestoneReactionDefinition, int> MilestoneReactionTriggered;

        public void Initialize(ThemeConfig theme)
        {
            activeTheme = theme;
            config = theme != null ? theme.ResolveWorldProgressionConfig() : null;
            currentDistrict = null;
            ActivatedTransitionCount = 0;
            ActivatedMilestoneReactionCount = 0;
            isInitialized = true;
        }

        public void ResetForRun(int startingScore)
        {
            EnsureInitialized();
            currentDistrict = null;
            ActivatedTransitionCount = 0;
            ActivatedMilestoneReactionCount = 0;
            UpdateDistrictForScore(startingScore, 0f, true);
        }

        public void UpdateDistrictForScore(int score, float transitionDurationSeconds, bool force)
        {
            EnsureInitialized();
            if (config == null || !config.TryGetDistrictForScore(score, out WorldDistrictStateDefinition district))
            {
                return;
            }

            bool changed = currentDistrict == null || currentDistrict.DistrictId != district.DistrictId;
            if (!force && !changed)
            {
                return;
            }

            currentDistrict = district;
            if (changed || force)
            {
                ActivatedTransitionCount++;
            }

            DistrictChanged?.Invoke(district, transitionDurationSeconds, force);
        }

        public void TriggerMilestoneReaction(int scoreThreshold)
        {
            EnsureInitialized();
            if (config == null || !config.TryGetMilestoneReaction(scoreThreshold, out WorldMilestoneReactionDefinition reaction))
            {
                return;
            }

            ActivatedMilestoneReactionCount++;
            MilestoneReactionTriggered?.Invoke(reaction, scoreThreshold);
        }

        private void EnsureInitialized()
        {
            if (!isInitialized)
            {
                Initialize(null);
            }
        }
    }
}
