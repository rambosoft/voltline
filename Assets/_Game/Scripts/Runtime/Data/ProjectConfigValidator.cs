using System.Collections.Generic;
using System.Linq;
using Voltline.Audio;
using Voltline.VFX;

namespace Voltline.Data
{
    public static class ProjectConfigValidator
    {
        public static ConfigValidationResult ValidateProject(
            GameBalanceConfig gameBalance,
            DifficultyCurveConfig difficultyCurve,
            GameplayPresentationConfig gameplayPresentation,
            PlayerVisualConfig playerVisualConfig,
            HazardPresentationCatalog hazardPresentationCatalog,
            ObstacleVisualCatalog obstacleVisualCatalog,
            ObstacleCatalog obstacleCatalog,
            ThemeCatalog themeCatalog,
            AudioCueCatalog audioCueCatalog,
            VfxCatalog vfxCatalog)
        {
            ConfigValidationResult result = new();

            ValidateGameBalance(gameBalance, result);
            ValidateDifficultyCurve(difficultyCurve, result);
            ValidateGameplayPresentation(gameplayPresentation, result);
            ValidatePlayerVisualConfig(playerVisualConfig, result);
            ValidateHazardPresentationCatalog(hazardPresentationCatalog, result);
            ValidateObstacleVisualCatalog(obstacleVisualCatalog, result);
            ValidateObstacleCatalog(obstacleCatalog, result);
            ValidateThemeCatalog(themeCatalog, result);
            ValidateAudioCueCatalog(audioCueCatalog, result);
            ValidateVfxCatalog(vfxCatalog, result);
            ValidatePresentationReadinessRelationships(gameBalance, gameplayPresentation, playerVisualConfig, hazardPresentationCatalog, obstacleVisualCatalog, obstacleCatalog, result);

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

        public static void ValidateGameplayPresentation(GameplayPresentationConfig config, ConfigValidationResult result)
        {
            if (config == null)
            {
                result.Add("Missing GameplayPresentationConfig asset.");
                return;
            }

            if (config.TrackCameraSize <= 0f) result.Add("GameplayPresentationConfig track camera size must be positive.");
            if (config.TrackLineWidth <= 0f) result.Add("GameplayPresentationConfig track line width must be positive.");
            if (config.TrackCurvePrimaryAmplitude < 0f) result.Add("GameplayPresentationConfig primary curve amplitude must be non-negative.");
            if (config.TrackCurvePrimaryWavelength <= 0f) result.Add("GameplayPresentationConfig primary curve wavelength must be positive.");
            if (config.TrackCurveSecondaryAmplitude < 0f) result.Add("GameplayPresentationConfig secondary curve amplitude must be non-negative.");
            if (config.TrackCurveSecondaryWavelength <= 0f) result.Add("GameplayPresentationConfig secondary curve wavelength must be positive.");
            if (config.PlayerVisualScale <= 0f) result.Add("GameplayPresentationConfig frozen player visual scale must be positive.");
            if (config.PlayerLineClearance < 0f) result.Add("GameplayPresentationConfig player line clearance must be non-negative.");
            if (config.PlayerCollisionHalfWidth <= 0f) result.Add("GameplayPresentationConfig player collision half-width must be positive.");
            if (config.PlayerCollisionHalfHeight <= 0f) result.Add("GameplayPresentationConfig player collision half-height must be positive.");
        }

        public static void ValidatePlayerVisualConfig(PlayerVisualConfig config, ConfigValidationResult result)
        {
            if (config == null)
            {
                result.Add("Missing PlayerVisualConfig asset.");
                return;
            }

            if (config.VisibleBoundsScale.x <= 0f || config.VisibleBoundsScale.y <= 0f) result.Add("PlayerVisualConfig visible bounds must be positive.");
            if (config.MenuPreviewSize.x <= 0f || config.MenuPreviewSize.y <= 0f) result.Add("PlayerVisualConfig menu preview size must be positive.");
        }

        public static void ValidateHazardPresentationCatalog(HazardPresentationCatalog catalog, ConfigValidationResult result)
        {
            if (catalog == null)
            {
                result.Add("Missing HazardPresentationCatalog asset.");
                return;
            }

            IReadOnlyList<HazardPresentationDefinition> entries = catalog.Entries;
            if (entries == null || entries.Count == 0)
            {
                result.Add("HazardPresentationCatalog must contain entries for every approved hazard family.");
                return;
            }

            HashSet<ObstacleFamily> families = new();
            for (int i = 0; i < entries.Count; i++)
            {
                HazardPresentationDefinition entry = entries[i];
                if (entry == null)
                {
                    result.Add($"HazardPresentationCatalog entry at index {i} is missing.");
                    continue;
                }

                if (!families.Add(entry.Family))
                {
                    result.Add($"HazardPresentationCatalog contains duplicate presentation entry for family '{entry.Family}'.");
                }

                if (entry.VisualBoundsScale.x <= 0f || entry.VisualBoundsScale.y <= 0f)
                {
                    result.Add($"HazardPresentationCatalog family '{entry.Family}' visual bounds must be positive.");
                }

                if (entry.CollisionBoundsScale.x <= 0f || entry.CollisionBoundsScale.y <= 0f)
                {
                    result.Add($"HazardPresentationCatalog family '{entry.Family}' collision bounds must be positive.");
                }

                if (entry.CollisionBoundsScale.x > entry.VisualBoundsScale.x)
                {
                    result.Add($"HazardPresentationCatalog family '{entry.Family}' collision width must not exceed visual width.");
                }

                if (entry.CollisionBoundsScale.y > entry.VisualBoundsScale.y)
                {
                    result.Add($"HazardPresentationCatalog family '{entry.Family}' collision height must not exceed visual height.");
                }

                if (entry.TelegraphBoundsScale.x < 0f || entry.TelegraphBoundsScale.y < 0f)
                {
                    result.Add($"HazardPresentationCatalog family '{entry.Family}' telegraph bounds must be non-negative.");
                }

                if (entry.MinimumReadableGapPadding < 0f)
                {
                    result.Add($"HazardPresentationCatalog family '{entry.Family}' readable gap padding must be non-negative.");
                }
            }

            foreach (ObstacleFamily family in System.Enum.GetValues(typeof(ObstacleFamily)))
            {
                if (!families.Contains(family))
                {
                    result.Add($"HazardPresentationCatalog is missing a presentation entry for family '{family}'.");
                }
            }
        }

        public static void ValidateObstacleVisualCatalog(ObstacleVisualCatalog catalog, ConfigValidationResult result)
        {
            if (catalog == null)
            {
                result.Add("Missing ObstacleVisualCatalog asset.");
                return;
            }

            IReadOnlyList<ObstacleVisualDefinition> entries = catalog.Entries;
            if (entries == null || entries.Count == 0)
            {
                result.Add("ObstacleVisualCatalog must contain entries for every approved hazard family.");
                return;
            }

            HashSet<ObstacleFamily> families = new();
            for (int i = 0; i < entries.Count; i++)
            {
                ObstacleVisualDefinition entry = entries[i];
                if (entry == null)
                {
                    result.Add($"ObstacleVisualCatalog entry at index {i} is missing.");
                    continue;
                }

                if (!families.Add(entry.Family))
                {
                    result.Add($"ObstacleVisualCatalog contains duplicate entry for family '{entry.Family}'.");
                }

                if (entry.VisualBoundsScale.x <= 0f || entry.VisualBoundsScale.y <= 0f)
                {
                    result.Add($"ObstacleVisualCatalog family '{entry.Family}' visual bounds must be positive.");
                }

                if (entry.TelegraphBoundsScale.x < 0f || entry.TelegraphBoundsScale.y < 0f)
                {
                    result.Add($"ObstacleVisualCatalog family '{entry.Family}' telegraph bounds must be non-negative.");
                }
            }

            foreach (ObstacleFamily family in System.Enum.GetValues(typeof(ObstacleFamily)))
            {
                if (!families.Contains(family))
                {
                    result.Add($"ObstacleVisualCatalog is missing a visual entry for family '{family}'.");
                }
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
                if (theme.UnlockBestScoreThreshold < 0) result.Add($"ThemeConfig '{theme.ThemeId}' unlock best score threshold must be non-negative.");
                if (catalog.DefaultTheme == theme && !theme.UnlockedByDefault) result.Add("ThemeCatalog default theme must be unlocked by default.");
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

            if (catalog.Entries == null || catalog.Entries.Count == 0)
            {
                result.Add("AudioCueCatalog must contain the core cue definitions.");
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

            ValidateRequiredIds(ids, result, AudioCueIds.Flip, AudioCueIds.Score, AudioCueIds.NearMiss, AudioCueIds.Milestone, AudioCueIds.Death, AudioCueIds.UiClick, AudioCueIds.MainLoop);
        }

        public static void ValidateVfxCatalog(VfxCatalog catalog, ConfigValidationResult result)
        {
            if (catalog == null)
            {
                result.Add("Missing VfxCatalog asset.");
                return;
            }

            if (catalog.Entries == null || catalog.Entries.Count == 0)
            {
                result.Add("VfxCatalog must contain the core effect definitions.");
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
                if (entry.SpawnMode == VfxCatalog.VfxSpawnMode.Prefab && entry.Prefab == null) result.Add($"VfxCatalog prefab entry '{entry.VfxId}' is missing a prefab reference.");
            }

            ValidateRequiredIds(ids, result, VfxCueIds.Flip, VfxCueIds.NearMiss, VfxCueIds.Death, VfxCueIds.Milestone);
        }

        private static void ValidatePresentationReadinessRelationships(
            GameBalanceConfig gameBalance,
            GameplayPresentationConfig gameplayPresentation,
            PlayerVisualConfig playerVisualConfig,
            HazardPresentationCatalog hazardPresentationCatalog,
            ObstacleVisualCatalog obstacleVisualCatalog,
            ObstacleCatalog obstacleCatalog,
            ConfigValidationResult result)
        {
            if (gameBalance == null || gameplayPresentation == null || playerVisualConfig == null || hazardPresentationCatalog == null || obstacleVisualCatalog == null)
            {
                return;
            }

            float requiredPlayerClearance = (gameplayPresentation.TrackLineWidth * 0.5f)
                + playerVisualConfig.VisibleHalfWidth
                + gameplayPresentation.PlayerLineClearance;
            if (gameBalance.SideOffset <= requiredPlayerClearance)
            {
                result.Add("GameBalanceConfig side offset must keep the player visual visibly clear of the line.");
            }

            if (playerVisualConfig.VisibleHalfWidth < gameplayPresentation.PlayerCollisionHalfWidth)
            {
                result.Add("PlayerVisualConfig visible width must remain wider than the player collision half-width.");
            }

            if (playerVisualConfig.VisibleHalfHeight < gameplayPresentation.PlayerCollisionHalfHeight)
            {
                result.Add("PlayerVisualConfig visible height must remain taller than the player collision half-height.");
            }

            float widestVisualHalfWidth = 0f;
            foreach (ObstacleFamily family in System.Enum.GetValues(typeof(ObstacleFamily)))
            {
                if (hazardPresentationCatalog.TryGetProfile(family, out HazardLayoutProfile layout)
                    && obstacleVisualCatalog.TryGetProfile(family, out ObstacleVisualProfile visual))
                {
                    widestVisualHalfWidth = System.Math.Max(widestVisualHalfWidth, visual.VisualHalfWidth);

                    if (visual.VisualBoundsScale.x < layout.CollisionBoundsScale.x)
                    {
                        result.Add($"ObstacleVisualCatalog family '{family}' visible width must remain at least as wide as the collision width.");
                    }

                    if (visual.VisualBoundsScale.y < layout.CollisionBoundsScale.y)
                    {
                        result.Add($"ObstacleVisualCatalog family '{family}' visible height must remain at least as tall as the collision height.");
                    }
                }
            }

            float requiredHazardClearance = (gameplayPresentation.TrackLineWidth * 0.5f) + widestVisualHalfWidth + 0.08f;
            if (gameBalance.SideOffset <= requiredHazardClearance)
            {
                result.Add("GameBalanceConfig side offset must keep the widest obstacle visual profile visibly clear of the line.");
            }

            if (obstacleCatalog == null)
            {
                return;
            }

            IReadOnlyList<ObstacleConfig> obstacles = obstacleCatalog.Obstacles;
            for (int i = 0; i < obstacles.Count; i++)
            {
                ObstacleConfig obstacle = obstacles[i];
                if (obstacle == null)
                {
                    continue;
                }

                if (!hazardPresentationCatalog.TryGetProfile(obstacle.Family, out _))
                {
                    result.Add($"ObstacleConfig '{obstacle.ObstacleId}' has no matching hazard presentation profile for family '{obstacle.Family}'.");
                }

                if (!obstacleVisualCatalog.TryGetProfile(obstacle.Family, out _))
                {
                    result.Add($"ObstacleConfig '{obstacle.ObstacleId}' has no matching obstacle visual profile for family '{obstacle.Family}'.");
                }
            }
        }

        private static void ValidateRequiredIds(HashSet<string> ids, ConfigValidationResult result, params string[] requiredIds)
        {
            for (int i = 0; i < requiredIds.Length; i++)
            {
                if (!ids.Contains(requiredIds[i]))
                {
                    result.Add($"Missing required config id '{requiredIds[i]}'.");
                }
            }
        }
    }
}
