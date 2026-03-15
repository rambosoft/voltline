using System;
using System.Collections.Generic;
using UnityEngine;

namespace Voltline.Data
{
    [CreateAssetMenu(fileName = "CFG_DifficultyCurve_Main", menuName = "Voltline/Config/Difficulty Curve")]
    public sealed class DifficultyCurveConfig : ScriptableObject
    {
        [Serializable]
        public sealed class DifficultyBandDefinition
        {
            [SerializeField] private string bandId = "band.0";
            [SerializeField] private int minScore;
            [SerializeField] private int maxScoreInclusive = -1;
            [SerializeField] private float speedMultiplier = 1f;
            [SerializeField] private float minSpawnSpacing = 2.5f;
            [SerializeField] private float maxSpawnSpacing = 3f;
            [SerializeField] private float minimumTelegraphSeconds = 0.75f;

            public string BandId => bandId;
            public int MinScore => minScore;
            public int MaxScoreInclusive => maxScoreInclusive;
            public float SpeedMultiplier => speedMultiplier;
            public float MinSpawnSpacing => minSpawnSpacing;
            public float MaxSpawnSpacing => maxSpawnSpacing;
            public float MinimumTelegraphSeconds => minimumTelegraphSeconds;

            public bool MatchesScore(int score)
            {
                if (score < minScore)
                {
                    return false;
                }

                return maxScoreInclusive < 0 || score <= maxScoreInclusive;
            }
        }

        [SerializeField] private List<DifficultyBandDefinition> bands = new();

        public IReadOnlyList<DifficultyBandDefinition> Bands => bands;

        public bool TryGetBandForScore(int score, out DifficultyBandDefinition band)
        {
            for (int i = 0; i < bands.Count; i++)
            {
                DifficultyBandDefinition candidate = bands[i];
                if (candidate != null && candidate.MatchesScore(score))
                {
                    band = candidate;
                    return true;
                }
            }

            band = null;
            return false;
        }
    }
}