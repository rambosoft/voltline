using System.Collections.Generic;
using System.Linq;

namespace Voltline.Data
{
    public static class ProjectConfigValidator
    {
        public static ConfigValidationResult ValidateProject(
            GameBalanceConfig gameBalance,
            DifficultyCurveConfig difficultyCurve,
            ObstacleCatalog obstacleCatalog,
            ThemeCatalog themeCatalog,
            AudioCueCatalog audioCueCatalog,
            VfxCatalog vfxCatalog)
        {
            ConfigValidationResult result = new();

            ValidateGameBalance(gameBalance, result);
            ValidateDifficultyCurve(difficultyCurve, result);
            ValidateObstacleCatalog(obstacleCatalog, result);
            ValidateThemeCatalog(themeCatalog, result);
            ValidateAudioCueCatalog(audioCueCatalog, result);
            ValidateVfxCatalog(vfxCatalog, result);

            return result;
        }

        public static void ValidateGameBalance(GameBalanceConfig config, ConfigValidationResult result)
        {
            if (config == null)
            {
                result.Add("Missing GameBalanceConfig asset.");
                return;
            }

            if (config.BaseSpeed <= 0f) result.Add("GameBalanceConfig base speed must be positive.");
            if (config.EarlySpeedRampPerPoint < 0f) result.Add("GameBalanceConfig speed ramp must be non-negative.");
            if (config.FlipDurationSeconds <= 0f) result.Add("GameBalanceConfig flip duration must be positive.");
            if (config.SideOffset <= 0f) result.Add("GameBalanceConfig side offset must be positive.");
            if (config.NearMissThreshold <= 0f) result.Add("GameBalanceConfig near-miss threshold must be positive.");
            if (config.SafeStartWindowSeconds <= 0f) result.Add("GameBalanceConfig safe-start window must be positive.");
            if (config.MinimumReadableTelegraphSeconds <= 0f) result.Add("GameBalanceConfig minimum telegraph must be positive.");
            if (config.ResultPanelRevealDelaySeconds < 0f) result.Add("GameBalanceConfig result-panel reveal delay must be non-negative.");
            if (config.RetryAvailabilityDelaySeconds < 0f) result.Add("GameBalanceConfig retry availability delay must be non-negative.");

            IReadOnlyList<int> milestones = config.MilestoneThresholds;
            if (milestones == null || milestones.Count == 0)
            {
                result.Add("GameBalanceConfig milestone thresholds must not be empty.");
                return;
            }

            int previous = -1;
            for (int i = 0; i < milestones.Count; i++)
            {
                int current = milestones[i];
                if (current <= 0)
                {
                    result.Add($"GameBalanceConfig milestone at index {i} must be positive.");
                }

                if (current <= previous)
                {
                    result.Add("GameBalanceConfig milestone thresholds must be strictly ascending.");
                    break;
                }

                previous = current;
            }
        }

        public static void ValidateDifficultyCurve(DifficultyCurveConfig config, ConfigValidationResult result)
        {
            if (config == null)
            {
                result.Add("Missing DifficultyCurveConfig asset.");
                return;
            }

            IReadOnlyList<DifficultyCurveConfig.DifficultyBandDefinition> bands = config.Bands;
            if (bands == null || bands.Count == 0)
            {
                result.Add("DifficultyCurveConfig must define at least one band.");
                return;
            }

            int expectedMinScore = 0;
            for (int i = 0; i < bands.Count; i++)
            {
                DifficultyCurveConfig.DifficultyBandDefinition band = bands[i];
                if (band == null)
                {
                    result.Add($"DifficultyCurveConfig band at index {i} is missing.");
                    continue;
                }

                if (!StableIdUtility.IsValid(band.BandId)) result.Add($"DifficultyCurveConfig band id '{band.BandId}' is invalid.");
                if (band.MinScore != expectedMinScore) result.Add($"DifficultyCurveConfig band '{band.BandId}' should start at score {expectedMinScore}.");
                if (band.MaxScoreInclusive >= 0 && band.MaxScoreInclusive < band.MinScore) result.Add($"DifficultyCurveConfig band '{band.BandId}' has an invalid score range.");
                if (band.SpeedMultiplier <= 0f) result.Add($"DifficultyCurveConfig band '{band.BandId}' speed multiplier must be positive.");
                if (band.MinSpawnSpacing <= 0f || band.MaxSpawnSpacing <= 0f || band.MaxSpawnSpacing < band.MinSpawnSpacing) result.Add($"DifficultyCurveConfig band '{band.BandId}' has an invalid spawn spacing range.");
                if (band.MinimumTelegraphSeconds <= 0f) result.Add($"DifficultyCurveConfig band '{band.BandId}' minimum telegraph must be positive.");

                if (band.MaxScoreInclusive < 0)
                {
                    if (i != bands.Count - 1)
                    {
                        result.Add($"DifficultyCurveConfig open-ended band '{band.BandId}' must be the final band.");
                    }

                    expectedMinScore = band.MinScore;
                    break;
                }

                expectedMinScore = band.MaxScoreInclusive + 1;
            }
        }

        public static void ValidateObstacleCatalog(ObstacleCatalog catalog, ConfigValidationResult result)
        {
            if (catalog == null)
            {
                result.Add("Missing ObstacleCatalog asset.");
                return;
            }

            IReadOnlyList<ObstacleConfig> obstacles = catalog.Obstacles;
            if (obstacles == null || obstacles.Count == 0)
            {
                result.Add("ObstacleCatalog must reference at least one obstacle config.");
                return;
            }

            HashSet<string> ids = new();
            for (int i = 0; i < obstacles.Count; i++)
            {
                ObstacleConfig obstacle = obstacles[i];
                if (obstacle == null)
                {
                    result.Add($"ObstacleCatalog entry at index {i} is missing.");
                    continue;
                }

                if (!StableIdUtility.IsValid(obstacle.ObstacleId)) result.Add($"ObstacleConfig '{obstacle.name}' has an invalid obstacle id '{obstacle.ObstacleId}'.");
                if (!ids.Add(obstacle.ObstacleId)) result.Add($"ObstacleCatalog contains duplicate obstacle id '{obstacle.ObstacleId}'.");
                if (string.IsNullOrWhiteSpace(obstacle.DisplayName)) result.Add($"ObstacleConfig '{obstacle.ObstacleId}' must have a display name.");
                if (obstacle.AllowedToScore >= 0 && obstacle.AllowedToScore < obstacle.AllowedFromScore) result.Add($"ObstacleConfig '{obstacle.ObstacleId}' has an invalid allowed score range.");
                if (obstacle.MinSpawnSpacing <= 0f || obstacle.MaxSpawnSpacing <= 0f || obstacle.MaxSpawnSpacing < obstacle.MinSpawnSpacing) result.Add($"ObstacleConfig '{obstacle.ObstacleId}' has an invalid spawn spacing range.");
                if (obstacle.RequiresTelegraph && obstacle.MinimumTelegraphSeconds <= 0f) result.Add($"ObstacleConfig '{obstacle.ObstacleId}' requires a positive telegraph duration.");
                if (obstacle.Weight <= 0f) result.Add($"ObstacleConfig '{obstacle.ObstacleId}' weight must be positive.");
            }
        }

        public static void ValidateThemeCatalog(ThemeCatalog catalog, ConfigValidationResult result)
        {
            if (catalog == null)
            {
                result.Add("Missing ThemeCatalog asset.");
                return;
            }

            if (catalog.DefaultTheme == null)
            {
                result.Add("ThemeCatalog must define a default theme.");
            }

            IReadOnlyList<ThemeConfig> themes = catalog.Themes;
            if (themes == null || themes.Count == 0)
            {
                result.Add("ThemeCatalog must reference at least one theme.");
                return;
            }

            HashSet<string> ids = new();
            bool foundDefaultTheme = false;
            for (int i = 0; i < themes.Count; i++)
            {
                ThemeConfig theme = themes[i];
                if (theme == null)
                {
                    result.Add($"ThemeCatalog entry at index {i} is missing.");
                    continue;
                }

                if (!StableIdUtility.IsValid(theme.ThemeId)) result.Add($"ThemeConfig '{theme.name}' has an invalid theme id '{theme.ThemeId}'.");
                if (!ids.Add(theme.ThemeId)) result.Add($"ThemeCatalog contains duplicate theme id '{theme.ThemeId}'.");
                if (string.IsNullOrWhiteSpace(theme.DisplayName)) result.Add($"ThemeConfig '{theme.ThemeId}' must have a display name.");
                if (catalog.DefaultTheme == theme) foundDefaultTheme = true;
            }

            if (catalog.DefaultTheme != null && !foundDefaultTheme)
            {
                result.Add("ThemeCatalog default theme must also exist in the catalog list.");
            }
        }

        public static void ValidateAudioCueCatalog(AudioCueCatalog catalog, ConfigValidationResult result)
        {
            if (catalog == null)
            {
                result.Add("Missing AudioCueCatalog asset.");
                return;
            }

            HashSet<string> ids = new();
            foreach (AudioCueCatalog.AudioCueDefinition entry in catalog.Entries ?? Enumerable.Empty<AudioCueCatalog.AudioCueDefinition>())
            {
                if (entry == null)
                {
                    result.Add("AudioCueCatalog contains a missing entry.");
                    continue;
                }

                if (!StableIdUtility.IsValid(entry.CueId)) result.Add($"AudioCueCatalog entry id '{entry.CueId}' is invalid.");
                if (!ids.Add(entry.CueId)) result.Add($"AudioCueCatalog contains duplicate cue id '{entry.CueId}'.");
                if (entry.MaxVolume < entry.MinVolume || entry.MinVolume < 0f) result.Add($"AudioCueCatalog entry '{entry.CueId}' has an invalid volume range.");
                if (entry.MaxPitch < entry.MinPitch || entry.MinPitch <= 0f) result.Add($"AudioCueCatalog entry '{entry.CueId}' has an invalid pitch range.");
                if (entry.MaxSimultaneousInstances <= 0) result.Add($"AudioCueCatalog entry '{entry.CueId}' max simultaneous instances must be positive.");
                if (entry.Clips != null && entry.Clips.Any(static clip => clip == null)) result.Add($"AudioCueCatalog entry '{entry.CueId}' contains a missing clip reference.");
            }
        }

        public static void ValidateVfxCatalog(VfxCatalog catalog, ConfigValidationResult result)
        {
            if (catalog == null)
            {
                result.Add("Missing VfxCatalog asset.");
                return;
            }

            HashSet<string> ids = new();
            foreach (VfxCatalog.VfxDefinition entry in catalog.Entries ?? Enumerable.Empty<VfxCatalog.VfxDefinition>())
            {
                if (entry == null)
                {
                    result.Add("VfxCatalog contains a missing entry.");
                    continue;
                }

                if (!StableIdUtility.IsValid(entry.VfxId)) result.Add($"VfxCatalog entry id '{entry.VfxId}' is invalid.");
                if (!ids.Add(entry.VfxId)) result.Add($"VfxCatalog contains duplicate effect id '{entry.VfxId}'.");
                if (entry.Scale <= 0f) result.Add($"VfxCatalog entry '{entry.VfxId}' scale must be positive.");
                if (entry.Prefab == null && catalog.Entries.Count > 0) result.Add($"VfxCatalog entry '{entry.VfxId}' is missing a prefab reference.");
            }
        }
    }
}