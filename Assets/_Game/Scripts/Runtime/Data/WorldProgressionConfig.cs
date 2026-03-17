using System;
using System.Collections.Generic;
using UnityEngine;

namespace Voltline.Data
{
    [Serializable]
    public sealed class WorldDistrictStateDefinition
    {
        [SerializeField] private string districtId = "district.failing-grid";
        [SerializeField] private string displayName = "Failing Grid";
        [SerializeField] private string statusLabel = "FAILING GRID";
        [SerializeField] private int minScore;
        [SerializeField] private int maxScoreInclusive = -1;
        [SerializeField] private Color backgroundTopColor = new(0.015f, 0.028f, 0.07f, 1f);
        [SerializeField] private Color backgroundBottomColor = new(0.03f, 0.005f, 0.085f, 1f);
        [SerializeField] private Color lineCoreColor = new(0.2f, 0.95f, 1f, 1f);
        [SerializeField] private Color lineGlowColor = new(0.06f, 0.68f, 1f, 1f);
        [SerializeField] private Color accentColor = new(0.92f, 0.96f, 1f, 1f);
        [SerializeField] private Color dangerColor = new(1f, 0.23f, 0.43f, 1f);
        [SerializeField] private Color milestoneColor = new(1f, 0.85f, 0.36f, 1f);
        [SerializeField] private float lineWidthMultiplier = 1f;
        [SerializeField] private float linePulseSpeedMultiplier = 1f;
        [SerializeField] private float backgroundAlphaMultiplier = 1f;

        public string DistrictId => districtId;
        public string DisplayName => displayName;
        public string StatusLabel => statusLabel;
        public int MinScore => minScore;
        public int MaxScoreInclusive => maxScoreInclusive;
        public Color BackgroundTopColor => backgroundTopColor;
        public Color BackgroundBottomColor => backgroundBottomColor;
        public Color LineCoreColor => lineCoreColor;
        public Color LineGlowColor => lineGlowColor;
        public Color AccentColor => accentColor;
        public Color DangerColor => dangerColor;
        public Color MilestoneColor => milestoneColor;
        public float LineWidthMultiplier => lineWidthMultiplier;
        public float LinePulseSpeedMultiplier => linePulseSpeedMultiplier;
        public float BackgroundAlphaMultiplier => backgroundAlphaMultiplier;

        public bool MatchesScore(int score)
        {
            if (score < minScore)
            {
                return false;
            }

            return maxScoreInclusive < 0 || score <= maxScoreInclusive;
        }
    }

    [Serializable]
    public sealed class WorldMilestoneReactionDefinition
    {
        [SerializeField] private int scoreThreshold = 10;
        [SerializeField] private float transitionDurationSeconds = 0.28f;
        [SerializeField] private float linePulseIntensity = 1f;
        [SerializeField] private float backgroundFlashStrength = 0.18f;

        public int ScoreThreshold => scoreThreshold;
        public float TransitionDurationSeconds => transitionDurationSeconds;
        public float LinePulseIntensity => linePulseIntensity;
        public float BackgroundFlashStrength => backgroundFlashStrength;
    }

    [CreateAssetMenu(fileName = "CFG_WorldProgression_LiveWireCity", menuName = "Voltline/Config/World Progression")]
    public sealed class WorldProgressionConfig : ScriptableObject
    {
        [SerializeField] private string progressionId = "world.live-wire-city";
        [SerializeField] private string displayName = "Live Wire City";
        [SerializeField] private List<WorldDistrictStateDefinition> districtStates = new();
        [SerializeField] private List<WorldMilestoneReactionDefinition> milestoneReactions = new();

        public string ProgressionId => progressionId;
        public string DisplayName => displayName;
        public IReadOnlyList<WorldDistrictStateDefinition> DistrictStates => districtStates;
        public IReadOnlyList<WorldMilestoneReactionDefinition> MilestoneReactions => milestoneReactions;

        public bool TryGetDistrictForScore(int score, out WorldDistrictStateDefinition district)
        {
            for (int i = 0; i < districtStates.Count; i++)
            {
                WorldDistrictStateDefinition candidate = districtStates[i];
                if (candidate != null && candidate.MatchesScore(score))
                {
                    district = candidate;
                    return true;
                }
            }

            district = null;
            return false;
        }

        public WorldDistrictStateDefinition GetRequiredDistrictForScore(int score)
        {
            if (TryGetDistrictForScore(score, out WorldDistrictStateDefinition district))
            {
                return district;
            }

            throw new InvalidOperationException($"World progression '{progressionId}' has no district covering score {score}.");
        }

        public bool TryGetMilestoneReaction(int scoreThreshold, out WorldMilestoneReactionDefinition reaction)
        {
            for (int i = 0; i < milestoneReactions.Count; i++)
            {
                WorldMilestoneReactionDefinition candidate = milestoneReactions[i];
                if (candidate != null && candidate.ScoreThreshold == scoreThreshold)
                {
                    reaction = candidate;
                    return true;
                }
            }

            reaction = null;
            return false;
        }

        public int GetRestoredDistrictCount(int score)
        {
            int count = 0;
            for (int i = 0; i < districtStates.Count; i++)
            {
                WorldDistrictStateDefinition district = districtStates[i];
                if (district != null && score >= district.MinScore)
                {
                    count++;
                }
            }

            return Mathf.Max(0, count);
        }
    }
}
