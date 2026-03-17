using System.Collections.Generic;
using UnityEngine;
using Voltline.Data;

namespace Voltline.Gameplay
{
    public sealed class DifficultyDirector : MonoBehaviour
    {
        private readonly List<ObstacleConfig> fallbackBuffer = new();

        private GameBalanceConfig gameBalance;
        private DifficultyCurveConfig difficultyCurve;
        private ObstacleCatalog obstacleCatalog;

        public void Initialize(GameBalanceConfig balanceConfig, DifficultyCurveConfig difficultyConfig, ObstacleCatalog catalog)
        {
            gameBalance = balanceConfig;
            difficultyCurve = difficultyConfig;
            obstacleCatalog = catalog;
        }

        public DifficultyCurveConfig.DifficultyBandDefinition GetBand(int score)
        {
            if (difficultyCurve != null && difficultyCurve.TryGetBandForScore(score, out DifficultyCurveConfig.DifficultyBandDefinition band))
            {
                return band;
            }

            IReadOnlyList<DifficultyCurveConfig.DifficultyBandDefinition> bands = difficultyCurve != null ? difficultyCurve.Bands : null;
            return bands != null && bands.Count > 0 ? bands[0] : null;
        }

        public float GetCurrentSpeed(int score)
        {
            DifficultyCurveConfig.DifficultyBandDefinition band = GetBand(score);
            float speedMultiplier = band != null ? band.SpeedMultiplier : 1f;
            return (gameBalance.BaseSpeed + (gameBalance.EarlySpeedRampPerPoint * Mathf.Max(0, score))) * speedMultiplier;
        }

        public void PopulateEligibleObstacleConfigs(int score, List<ObstacleConfig> results)
        {
            results.Clear();
            fallbackBuffer.Clear();

            if (obstacleCatalog == null)
            {
                return;
            }

            IReadOnlyList<ObstacleConfig> obstacles = obstacleCatalog.Obstacles;
            for (int i = 0; i < obstacles.Count; i++)
            {
                ObstacleConfig obstacle = obstacles[i];
                if (obstacle == null || !obstacle.EnabledForRuntime || !IsSupportedFamily(obstacle.Family))
                {
                    continue;
                }

                fallbackBuffer.Add(obstacle);

                if (score < obstacle.AllowedFromScore)
                {
                    continue;
                }

                if (obstacle.AllowedToScore >= 0 && score > obstacle.AllowedToScore)
                {
                    continue;
                }

                results.Add(obstacle);
            }

            if (results.Count == 0)
            {
                results.AddRange(fallbackBuffer);
            }
        }

        public void GetSpawnSpacingRange(int score, ObstacleConfig obstacle, out float minSpacing, out float maxSpacing)
        {
            DifficultyCurveConfig.DifficultyBandDefinition band = GetBand(score);
            float bandMin = band != null ? band.MinSpawnSpacing : 2.2f;
            float bandMax = band != null ? band.MaxSpawnSpacing : 2.8f;

            if (obstacle == null)
            {
                minSpacing = bandMin;
                maxSpacing = bandMax;
                return;
            }

            minSpacing = Mathf.Max(bandMin, obstacle.MinSpawnSpacing);
            maxSpacing = Mathf.Min(bandMax, obstacle.MaxSpawnSpacing);
            if (maxSpacing < minSpacing)
            {
                maxSpacing = minSpacing;
            }
        }

        public float GetEffectiveTelegraphSeconds(int score, ObstacleConfig obstacle)
        {
            DifficultyCurveConfig.DifficultyBandDefinition band = GetBand(score);
            float bandTelegraph = band != null ? band.MinimumTelegraphSeconds : gameBalance.MinimumReadableTelegraphSeconds;
            float obstacleTelegraph = obstacle != null && obstacle.RequiresTelegraph ? obstacle.MinimumTelegraphSeconds : 0f;
            return Mathf.Max(gameBalance.MinimumReadableTelegraphSeconds, bandTelegraph, obstacleTelegraph);
        }

        public bool IsSupportedFamily(ObstacleFamily family)
        {
            return family == ObstacleFamily.GroundedBlockers
                   || family == ObstacleFamily.SharpUtilityHazards
                   || family == ObstacleFamily.ActiveElectricHazards
                   || family == ObstacleFamily.RotatingIndustrialHazards
                   || family == ObstacleFamily.BrokenConduitSections
                   || family == ObstacleFamily.SidePressureHazards;
        }
    }
}
