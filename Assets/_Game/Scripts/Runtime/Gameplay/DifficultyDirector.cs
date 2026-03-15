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

        public float GetNextSpawnSpacing(int score)
        {
            DifficultyCurveConfig.DifficultyBandDefinition band = GetBand(score);
            if (band == null)
            {
                return 2.4f;
            }

            return Random.Range(band.MinSpawnSpacing, band.MaxSpawnSpacing);
        }

        public bool IsSupportedFamily(ObstacleFamily family)
        {
            return family == ObstacleFamily.Spikes ||
                   family == ObstacleFamily.ElectricGates ||
                   family == ObstacleFamily.SideBlockers;
        }
    }
}