using System.Collections.Generic;
using System.Linq;
using Voltline.Audio;
using Voltline.VFX;

namespace Voltline.Data
{
    public static class ProjectConfigValidator
    {
                private static readonly PresentationRolloutSliceId[] RequiredRolloutOrder =
        {
            PresentationRolloutSliceId.PlayerRefresh,
            PresentationRolloutSliceId.ObstacleRefresh,
            PresentationRolloutSliceId.BackgroundRefresh,
            PresentationRolloutSliceId.StaticThemeRefresh,
            PresentationRolloutSliceId.DynamicThemeTransitions,
            PresentationRolloutSliceId.VfxRefresh,
            PresentationRolloutSliceId.AudioRefresh,
        };

        private static readonly int[] RequiredLiveWireMilestones = { 10, 20, 30, 40, 50 };
        private static readonly string[] RequiredLiveWireAudioCueIds =
        {
            AudioCueIds.Flip,
            AudioCueIds.Score,
            AudioCueIds.NearMiss,
            AudioCueIds.Milestone,
            AudioCueIds.Death,
            AudioCueIds.UiClick,
            AudioCueIds.MainLoop,
            AudioCueIds.FlipLiveWireCity,
            AudioCueIds.ScoreLiveWireCity,
            AudioCueIds.NearMissLiveWireCity,
            AudioCueIds.MilestoneLiveWireCity,
            AudioCueIds.DeathLiveWireCityGrounded,
            AudioCueIds.DeathLiveWireCitySharp,
            AudioCueIds.DeathLiveWireCityElectric,
            AudioCueIds.DeathLiveWireCityRotating,
            AudioCueIds.DeathLiveWireCityBroken,
            AudioCueIds.DeathLiveWireCitySide,
            AudioCueIds.UiClickLiveWireCity,
            AudioCueIds.MainLoopLiveWireCity,
        };
        private static readonly string[] RequiredLiveWireVfxCueIds =
        {
            VfxCueIds.Flip,
            VfxCueIds.NearMiss,
            VfxCueIds.Death,
            VfxCueIds.Milestone,
            VfxCueIds.FlipLiveWireCity,
            VfxCueIds.NearMissLiveWireCity,
            VfxCueIds.MilestoneLiveWireCity,
            VfxCueIds.DeathLiveWireCityGrounded,
            VfxCueIds.DeathLiveWireCitySharp,
            VfxCueIds.DeathLiveWireCityElectric,
            VfxCueIds.DeathLiveWireCityRotating,
            VfxCueIds.DeathLiveWireCityBroken,
            VfxCueIds.DeathLiveWireCitySide,
        };

        public static ConfigValidationResult ValidateProject(
            GameBalanceConfig gameBalance,
            DifficultyCurveConfig difficultyCurve,
            GameplayPresentationConfig gameplayPresentation,
            BackgroundPresentationConfig backgroundPresentationConfig,
            PlayerVisualConfig playerVisualConfig,
            HazardPresentationCatalog hazardPresentationCatalog,
            ObstacleVisualCatalog obstacleVisualCatalog,
            ObstacleCatalog obstacleCatalog,
            ThemeCatalog themeCatalog,
            ThemeSequenceConfig themeSequenceConfig,
            PresentationRolloutPlanConfig presentationRolloutPlan,
            AudioCueCatalog audioCueCatalog,
            VfxCatalog vfxCatalog)
        {
            ConfigValidationResult result = new();

            ValidateGameBalance(gameBalance, result);
            ValidateDifficultyCurve(difficultyCurve, result);
            ValidateGameplayPresentation(gameplayPresentation, result);
            ValidateBackgroundPresentationConfig(backgroundPresentationConfig, result);
            ValidatePlayerVisualConfig(playerVisualConfig, result);
            ValidateHazardPresentationCatalog(hazardPresentationCatalog, result);
            ValidateObstacleVisualCatalog(obstacleVisualCatalog, result);
            ValidateObstacleCatalog(obstacleCatalog, result);
            ValidateThemeCatalog(themeCatalog, result);
            ValidateThemeSequenceConfig(themeCatalog, themeSequenceConfig, gameBalance, result);
            ValidatePresentationRolloutPlanConfig(presentationRolloutPlan, result);
            ValidateAudioCueCatalog(audioCueCatalog, result);
            ValidateVfxCatalog(vfxCatalog, result);
            ValidatePresentationReadinessRelationships(
                gameBalance,
                gameplayPresentation,
                backgroundPresentationConfig,
                playerVisualConfig,
                hazardPresentationCatalog,
                obstacleVisualCatalog,
                obstacleCatalog,
                themeCatalog,
                themeSequenceConfig,
                result);

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

            if (milestones.Count != RequiredLiveWireMilestones.Length)
            {
                result.Add("GameBalanceConfig milestone thresholds must match the Live Wire City release ladder 10/20/30/40/50.");
                return;
            }

            for (int i = 0; i < RequiredLiveWireMilestones.Length; i++)
            {
                if (milestones[i] != RequiredLiveWireMilestones[i])
                {
                    result.Add("GameBalanceConfig milestone thresholds must match the Live Wire City release ladder 10/20/30/40/50.");
                    break;
                }
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

        public static void ValidateBackgroundPresentationConfig(BackgroundPresentationConfig config, ConfigValidationResult result)
        {
            if (config == null)
            {
                result.Add("Missing BackgroundPresentationConfig asset.");
                return;
            }

            if (config.MaxRuntimeSpriteCount <= 0) result.Add("BackgroundPresentationConfig max runtime sprite count must be positive.");
            if (config.MaxExpectedDrawCalls <= 0) result.Add("BackgroundPresentationConfig max expected draw calls must be positive.");
            if (config.LaneQuietZoneHalfWidth <= 0f) result.Add("BackgroundPresentationConfig lane quiet-zone half width must be positive.");
            if (config.MaximumAllowedLayerAlpha <= 0f) result.Add("BackgroundPresentationConfig maximum allowed layer alpha must be positive.");

            IReadOnlyList<BackgroundLayerDefinition> layers = config.Layers;
            if (layers == null || layers.Count == 0)
            {
                result.Add("BackgroundPresentationConfig must define at least one background layer.");
                return;
            }

            if (config.MaxExpectedDrawCalls < System.Math.Min(config.MaxRuntimeSpriteCount, layers.Count))
            {
                result.Add("BackgroundPresentationConfig expected draw-call budget must cover the configured runtime layer count.");
            }

            HashSet<string> layerIds = new();
            for (int i = 0; i < layers.Count; i++)
            {
                BackgroundLayerDefinition layer = layers[i];
                if (layer == null)
                {
                    result.Add($"BackgroundPresentationConfig layer at index {i} is missing.");
                    continue;
                }

                if (!StableIdUtility.IsValid(layer.LayerId)) result.Add($"BackgroundPresentationConfig layer id '{layer.LayerId}' is invalid.");
                if (!layerIds.Add(layer.LayerId)) result.Add($"BackgroundPresentationConfig contains duplicate layer id '{layer.LayerId}'.");
                if (layer.Size.x <= 0f || layer.Size.y <= 0f) result.Add($"BackgroundPresentationConfig layer '{layer.LayerId}' size must be positive.");
                if (layer.ContentFill.x <= 0f || layer.ContentFill.x > 1f || layer.ContentFill.y <= 0f || layer.ContentFill.y > 1f) result.Add($"BackgroundPresentationConfig layer '{layer.LayerId}' content fill must stay within (0,1].");
                if (layer.Sprite != null && (layer.Sprite.bounds.size.x <= 0f || layer.Sprite.bounds.size.y <= 0f)) result.Add($"BackgroundPresentationConfig layer '{layer.LayerId}' sprite bounds must be positive.");
                for (int variantIndex = 0; variantIndex < layer.DistrictVariants.Count; variantIndex++)
                {
                    BackgroundLayerDistrictVariantDefinition variant = layer.DistrictVariants[variantIndex];
                    if (variant == null)
                    {
                        result.Add($"BackgroundPresentationConfig layer '{layer.LayerId}' contains a missing district variant at index {variantIndex}.");
                        continue;
                    }

                    if (variant.MinimumRestoredDistrictCount < 0) result.Add($"BackgroundPresentationConfig layer '{layer.LayerId}' district variant {variantIndex} must use a non-negative restored district count.");
                    if (variant.MinimumScoreThreshold < -1) result.Add($"BackgroundPresentationConfig layer '{layer.LayerId}' district variant {variantIndex} must use -1 or a non-negative score threshold.");
                    if (variant.Sprite == null) result.Add($"BackgroundPresentationConfig layer '{layer.LayerId}' district variant {variantIndex} must reference a sprite.");
                    if (variant.ContentFill.x <= 0f || variant.ContentFill.x > 1f || variant.ContentFill.y <= 0f || variant.ContentFill.y > 1f) result.Add($"BackgroundPresentationConfig layer '{layer.LayerId}' district variant {variantIndex} content fill must stay within (0,1].");
                    if (variant.AlphaMultiplier < 0.5f || variant.AlphaMultiplier > 1.5f) result.Add($"BackgroundPresentationConfig layer '{layer.LayerId}' district variant {variantIndex} alpha multiplier must stay within [0.5,1.5].");
                    if (variant.TintStrength < 0f || variant.TintStrength > 1f) result.Add($"BackgroundPresentationConfig layer '{layer.LayerId}' district variant {variantIndex} tint strength must stay within [0,1].");
                    if (variant.Sprite != null && (variant.Sprite.bounds.size.x <= 0f || variant.Sprite.bounds.size.y <= 0f)) result.Add($"BackgroundPresentationConfig layer '{layer.LayerId}' district variant {variantIndex} sprite bounds must be positive.");
                }
                if (layer.Alpha <= 0f || layer.Alpha > config.MaximumAllowedLayerAlpha) result.Add($"BackgroundPresentationConfig layer '{layer.LayerId}' alpha must be positive and stay within the configured maximum.");
                if (layer.VerticalTravelMultiplier < 0f) result.Add($"BackgroundPresentationConfig layer '{layer.LayerId}' vertical travel multiplier must be non-negative.");
                if (layer.VerticalLoopDistance <= 0f) result.Add($"BackgroundPresentationConfig layer '{layer.LayerId}' vertical loop distance must be positive.");
                if (layer.VelocityResponseMultiplier < 0f) result.Add($"BackgroundPresentationConfig layer '{layer.LayerId}' velocity response multiplier must be non-negative.");
                if (layer.HorizontalOscillationAmplitude < 0f) result.Add($"BackgroundPresentationConfig layer '{layer.LayerId}' horizontal oscillation amplitude must be non-negative.");
                if (layer.HorizontalOscillationFrequency < 0f) result.Add($"BackgroundPresentationConfig layer '{layer.LayerId}' horizontal oscillation frequency must be non-negative.");
            }
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

            IReadOnlyList<PlayerVisualStateDefinition> states = config.StateDefinitions;
            if (states == null || states.Count == 0)
            {
                result.Add("PlayerVisualConfig must define player presentation states.");
                return;
            }

            HashSet<PlayerVisualPresentationStateId> stateIds = new();
            for (int i = 0; i < states.Count; i++)
            {
                PlayerVisualStateDefinition state = states[i];
                if (state == null)
                {
                    result.Add($"PlayerVisualConfig state definition at index {i} is missing.");
                    continue;
                }

                if (!stateIds.Add(state.StateId)) result.Add($"PlayerVisualConfig contains duplicate state '{state.StateId}'.");
                if (state.VisibleBounds.x <= 0f || state.VisibleBounds.y <= 0f) result.Add($"PlayerVisualConfig state '{state.StateId}' visible bounds must be positive.");
                if (state.DurationSeconds < 0f) result.Add($"PlayerVisualConfig state '{state.StateId}' duration must be non-negative.");
                if (state.PulseAmplitude < 0f) result.Add($"PlayerVisualConfig state '{state.StateId}' pulse amplitude must be non-negative.");
                if (state.PulseFrequency < 0f) result.Add($"PlayerVisualConfig state '{state.StateId}' pulse frequency must be non-negative.");
            }

            foreach (PlayerVisualPresentationStateId requiredState in System.Enum.GetValues(typeof(PlayerVisualPresentationStateId)))
            {
                if (!stateIds.Contains(requiredState))
                {
                    result.Add($"PlayerVisualConfig must define state '{requiredState}'.");
                }
            }
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
            HashSet<ObstacleFamily> families = new();
            for (int i = 0; i < obstacles.Count; i++)
            {
                ObstacleConfig obstacle = obstacles[i];
                if (obstacle == null)
                {
                    result.Add($"ObstacleCatalog entry at index {i} is missing.");
                    continue;
                }

                families.Add(obstacle.Family);
                if (!StableIdUtility.IsValid(obstacle.ObstacleId)) result.Add($"ObstacleConfig '{obstacle.name}' has an invalid obstacle id '{obstacle.ObstacleId}'.");
                if (!ids.Add(obstacle.ObstacleId)) result.Add($"ObstacleCatalog contains duplicate obstacle id '{obstacle.ObstacleId}'.");
                if (string.IsNullOrWhiteSpace(obstacle.DisplayName)) result.Add($"ObstacleConfig '{obstacle.ObstacleId}' must have a display name.");
                if (obstacle.AllowedToScore >= 0 && obstacle.AllowedToScore < obstacle.AllowedFromScore) result.Add($"ObstacleConfig '{obstacle.ObstacleId}' has an invalid allowed score range.");
                if (obstacle.MinSpawnSpacing <= 0f || obstacle.MaxSpawnSpacing <= 0f || obstacle.MaxSpawnSpacing < obstacle.MinSpawnSpacing) result.Add($"ObstacleConfig '{obstacle.ObstacleId}' has an invalid spawn spacing range.");
                if (obstacle.RequiresTelegraph && obstacle.MinimumTelegraphSeconds <= 0f) result.Add($"ObstacleConfig '{obstacle.ObstacleId}' requires a positive telegraph duration.");
                if (obstacle.Weight <= 0f) result.Add($"ObstacleConfig '{obstacle.ObstacleId}' weight must be positive.");
            }

            foreach (ObstacleFamily family in System.Enum.GetValues(typeof(ObstacleFamily)))
            {
                if (!families.Contains(family)) result.Add($"ObstacleCatalog must define at least one obstacle config for family '{family}'.");
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

            if (catalog.BrandingPresentationConfig == null)
            {
                result.Add("ThemeCatalog must reference a BrandingPresentationConfig.");
            }
            else
            {
                ValidateBrandingPresentationConfig(catalog.BrandingPresentationConfig, result);
            }

            if (catalog.ProductionCopyConfig == null)
            {
                result.Add("ThemeCatalog must reference a ProductionCopyConfig.");
            }
            else
            {
                ValidateProductionCopyConfig(catalog.ProductionCopyConfig, result);
            }

            if (catalog.UiThemeConfig == null)
            {
                result.Add("ThemeCatalog must reference a UIThemeConfig.");
            }
            else
            {
                ValidateUiThemeConfig(catalog.UiThemeConfig, result);
            }

            IReadOnlyList<ThemeConfig> themes = catalog.Themes;
            if (themes == null || themes.Count == 0)
            {
                result.Add("ThemeCatalog must reference at least one theme.");
                return;
            }

            if (themes.Count != 1)
            {
                result.Add("ThemeCatalog must surface exactly one production theme for the Live Wire City first-release posture.");
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

                WorldProgressionConfig worldProgression = theme.ResolveWorldProgressionConfig();
                if (!StableIdUtility.IsValid(theme.ThemeId)) result.Add($"ThemeConfig '{theme.name}' has an invalid theme id '{theme.ThemeId}'.");
                if (!ids.Add(theme.ThemeId)) result.Add($"ThemeCatalog contains duplicate theme id '{theme.ThemeId}'.");
                if (string.IsNullOrWhiteSpace(theme.DisplayName)) result.Add($"ThemeConfig '{theme.ThemeId}' must have a display name.");
                if (theme.UnlockBestScoreThreshold < 0) result.Add($"ThemeConfig '{theme.ThemeId}' unlock best score threshold must be non-negative.");
                if (theme.PreferredTransitionDuration < 0f) result.Add($"ThemeConfig '{theme.ThemeId}' preferred transition duration must be non-negative.");
                if (theme.ThemeVfxProfile == null) result.Add($"ThemeConfig '{theme.ThemeId}' must reference a ThemeVfxProfile.");
                if (theme.ThemeAudioProfile == null) result.Add($"ThemeConfig '{theme.ThemeId}' must reference a ThemeAudioProfile.");
                if (worldProgression == null) result.Add($"ThemeConfig '{theme.ThemeId}' must reference a WorldProgressionConfig.");
                if (theme.AllowRuntimeSequenceSelection) result.Add($"ThemeConfig '{theme.ThemeId}' must not allow legacy runtime theme sequence selection in the Live Wire City shipping path.");
                if (theme.ThemeVfxProfile != null) ValidateThemeVfxProfile(theme.ThemeId, theme.ThemeVfxProfile, result);
                if (theme.ThemeId == "theme.live-wire-city" && theme.ThemeVfxProfile != null && theme.ThemeVfxProfile.ProfileId != "theme.live-wire-city.vfx") result.Add("ThemeConfig 'theme.live-wire-city' must use the Live Wire City VFX profile id 'theme.live-wire-city.vfx'.");
                if (theme.ThemeAudioProfile != null) ValidateThemeAudioProfile(theme.ThemeId, theme.ThemeAudioProfile, result);
                if (theme.ThemeId == "theme.live-wire-city" && theme.ThemeAudioProfile != null && theme.ThemeAudioProfile.ProfileId != "theme.live-wire-city.audio") result.Add("ThemeConfig 'theme.live-wire-city' must use the Live Wire City audio profile id 'theme.live-wire-city.audio'.");
                if (worldProgression != null) ValidateWorldProgressionConfig(theme.ThemeId, worldProgression, result);
                if (catalog.DefaultTheme == theme && !theme.UnlockedByDefault) result.Add("ThemeCatalog default theme must be unlocked by default.");
                if (catalog.DefaultTheme == theme) foundDefaultTheme = true;
            }

            if (catalog.DefaultTheme != null && !foundDefaultTheme)
            {
                result.Add("ThemeCatalog default theme must also exist in the catalog list.");
            }

            if (catalog.DefaultThemeId != "theme.live-wire-city")
            {
                result.Add("ThemeCatalog default theme id must be 'theme.live-wire-city' for the Live Wire City shipping posture.");
            }

            if (catalog.ShouldShowThemeSelectionInSettings)
            {
                result.Add("ThemeCatalog must keep theme selection hidden while only one production theme is live.");
            }
        }

                public static void ValidateThemeSequenceConfig(ThemeCatalog themeCatalog, ThemeSequenceConfig config, GameBalanceConfig gameBalance, ConfigValidationResult result)
        {
            if (config == null)
            {
                result.Add("Missing ThemeSequenceConfig asset.");
                return;
            }

            if (config.MinimumScoreForTransitions < 0) result.Add("ThemeSequenceConfig minimum score for transitions must be non-negative.");
            if (config.MinimumTransitionDurationSeconds < 0f) result.Add("ThemeSequenceConfig minimum transition duration must be non-negative.");
            if (config.MaximumTransitionDurationSeconds < config.MinimumTransitionDurationSeconds) result.Add("ThemeSequenceConfig maximum transition duration must not be lower than the minimum transition duration.");
            if (config.EnableRuntimeTransitions) result.Add("ThemeSequenceConfig runtime transitions must stay disabled once Live Wire City world progression is the shipping model.");
            if (config.Entries != null && config.Entries.Count > 0) result.Add("ThemeSequenceConfig must not define legacy transition entries in the Live Wire City shipping posture.");
        }

        public static void ValidateWorldProgressionConfig(string themeId, WorldProgressionConfig config, ConfigValidationResult result)
        {
            if (config == null)
            {
                result.Add($"ThemeConfig '{themeId}' world progression config is missing.");
                return;
            }

            if (!StableIdUtility.IsValid(config.ProgressionId)) result.Add($"ThemeConfig '{themeId}' world progression id '{config.ProgressionId}' is invalid.");
            if (string.IsNullOrWhiteSpace(config.DisplayName)) result.Add($"ThemeConfig '{themeId}' world progression must define a display name.");

            IReadOnlyList<WorldDistrictStateDefinition> districts = config.DistrictStates;
            if (districts == null || districts.Count != 5)
            {
                result.Add($"ThemeConfig '{themeId}' world progression must define the five Live Wire City districts.");
            }
            else
            {
                int[] expectedStarts = { 0, 10, 20, 30, 40 };
                int[] expectedEnds = { 9, 19, 29, 39, -1 };
                for (int i = 0; i < districts.Count; i++)
                {
                    WorldDistrictStateDefinition district = districts[i];
                    if (district == null)
                    {
                        result.Add($"ThemeConfig '{themeId}' world progression district at index {i} is missing.");
                        continue;
                    }

                    if (!StableIdUtility.IsValid(district.DistrictId)) result.Add($"ThemeConfig '{themeId}' world district id '{district.DistrictId}' is invalid.");
                    if (string.IsNullOrWhiteSpace(district.DisplayName)) result.Add($"ThemeConfig '{themeId}' world district '{district.DistrictId}' must have a display name.");
                    if (string.IsNullOrWhiteSpace(district.StatusLabel)) result.Add($"ThemeConfig '{themeId}' world district '{district.DistrictId}' must have a status label.");
                    if (district.MinScore != expectedStarts[i] || district.MaxScoreInclusive != expectedEnds[i]) result.Add($"ThemeConfig '{themeId}' world district '{district.DistrictId}' must follow the Live Wire City score bands 0-9, 10-19, 20-29, 30-39, 40+.");
                    if (district.LineWidthMultiplier <= 0f) result.Add($"ThemeConfig '{themeId}' world district '{district.DistrictId}' line width multiplier must be positive.");
                    if (district.LinePulseSpeedMultiplier <= 0f) result.Add($"ThemeConfig '{themeId}' world district '{district.DistrictId}' line pulse speed multiplier must be positive.");
                    if (district.BackgroundAlphaMultiplier <= 0f) result.Add($"ThemeConfig '{themeId}' world district '{district.DistrictId}' background alpha multiplier must be positive.");
                }
            }

            IReadOnlyList<WorldMilestoneReactionDefinition> reactions = config.MilestoneReactions;
            if (reactions == null || reactions.Count != RequiredLiveWireMilestones.Length)
            {
                result.Add($"ThemeConfig '{themeId}' world progression must define milestone reactions for 10/20/30/40/50.");
                return;
            }

            for (int i = 0; i < RequiredLiveWireMilestones.Length; i++)
            {
                WorldMilestoneReactionDefinition reaction = reactions[i];
                if (reaction == null)
                {
                    result.Add($"ThemeConfig '{themeId}' world milestone reaction at index {i} is missing.");
                    continue;
                }

                if (reaction.ScoreThreshold != RequiredLiveWireMilestones[i]) result.Add($"ThemeConfig '{themeId}' milestone reactions must match 10/20/30/40/50.");
                if (reaction.TransitionDurationSeconds <= 0f) result.Add($"ThemeConfig '{themeId}' milestone reaction {reaction.ScoreThreshold} transition duration must be positive.");
                if (reaction.LinePulseIntensity <= 0f) result.Add($"ThemeConfig '{themeId}' milestone reaction {reaction.ScoreThreshold} line pulse intensity must be positive.");
                if (reaction.BackgroundFlashStrength <= 0f) result.Add($"ThemeConfig '{themeId}' milestone reaction {reaction.ScoreThreshold} background flash strength must be positive.");
            }
        }

        public static void ValidatePresentationRolloutPlanConfig(PresentationRolloutPlanConfig config, ConfigValidationResult result)
        {
            if (config == null)
            {
                result.Add("Missing PresentationRolloutPlanConfig asset.");
                return;
            }

            if (!config.RequireConfigValidation) result.Add("PresentationRolloutPlanConfig must require config validation.");
            if (!config.RequireReleaseAudit) result.Add("PresentationRolloutPlanConfig must require the release audit.");
            if (!config.RequirePresentationReadinessAudit) result.Add("PresentationRolloutPlanConfig must require the presentation readiness audit.");
            if (!config.RequireEditModeSuite) result.Add("PresentationRolloutPlanConfig must require the Edit Mode suite.");
            if (!config.RequirePlayModeSuite) result.Add("PresentationRolloutPlanConfig must require the Play Mode suite.");
            if (!config.RequireManualCollisionReview) result.Add("PresentationRolloutPlanConfig must require manual collision review.");
            if (!config.RequireManualReadabilityReview) result.Add("PresentationRolloutPlanConfig must require manual readability review.");
            if (!config.RequireDevicePerformanceCheck) result.Add("PresentationRolloutPlanConfig must require device performance checks.");
            if (!config.RequireSaveAndThemePersistenceCheck) result.Add("PresentationRolloutPlanConfig must require save and theme persistence checks.");
            if (!config.RequireBuildSizeReview) result.Add("PresentationRolloutPlanConfig must require build-size review.");
            if (!config.RequireNoConsoleNoise) result.Add("PresentationRolloutPlanConfig must require a no-console-noise check.");

            IReadOnlyList<PresentationRolloutSliceDefinition> slices = config.Slices;
            if (slices == null || slices.Count != RequiredRolloutOrder.Length)
            {
                result.Add("PresentationRolloutPlanConfig must define the full ordered rollout slice list.");
                return;
            }

            for (int i = 0; i < RequiredRolloutOrder.Length; i++)
            {
                PresentationRolloutSliceDefinition slice = slices[i];
                if (slice == null)
                {
                    result.Add($"PresentationRolloutPlanConfig slice at index {i} is missing.");
                    continue;
                }

                if (slice.SliceId != RequiredRolloutOrder[i])
                {
                    result.Add($"PresentationRolloutPlanConfig slice at index {i} must be '{RequiredRolloutOrder[i]}'.");
                }

                if (!slice.RequiresAutomatedChecks) result.Add($"PresentationRolloutPlanConfig slice '{slice.SliceId}' must require automated checks.");
                if (!slice.RequiresManualReadabilityReview) result.Add($"PresentationRolloutPlanConfig slice '{slice.SliceId}' must require manual readability review.");
                if (!slice.RequiresDevicePerformanceCheck) result.Add($"PresentationRolloutPlanConfig slice '{slice.SliceId}' must require device performance checks.");
                if (!slice.RequiresNoConsoleNoiseCheck) result.Add($"PresentationRolloutPlanConfig slice '{slice.SliceId}' must require a no-console-noise check.");
                if (!slice.BlocksNextSliceUntilApproved) result.Add($"PresentationRolloutPlanConfig slice '{slice.SliceId}' must block the next slice until approval.");

                bool shouldRequireCollisionReview = slice.SliceId != PresentationRolloutSliceId.AudioRefresh;
                if (slice.RequiresCollisionReview != shouldRequireCollisionReview)
                {
                    result.Add($"PresentationRolloutPlanConfig slice '{slice.SliceId}' collision-review flag is not aligned with the approved rollout policy.");
                }
            }
        }

        public static void ValidateThemeVfxProfile(string themeId, ThemeVfxProfile profile, ConfigValidationResult result)
        {
            if (string.IsNullOrWhiteSpace(profile.ProfileId) || !StableIdUtility.IsValid(profile.ProfileId)) result.Add($"ThemeConfig '{themeId}' VFX profile id '{profile.ProfileId}' is invalid.");
            if (profile.MaxActiveTransientEffects <= 0) result.Add($"ThemeConfig '{themeId}' VFX profile must allow at least one active transient effect.");
            if (profile.MaxBurstCountPerEffect <= 0) result.Add($"ThemeConfig '{themeId}' VFX profile burst budget must be positive.");
            if (profile.MinimumReplayCooldownSeconds < 0f) result.Add($"ThemeConfig '{themeId}' VFX profile cooldown must be non-negative.");
            HashSet<string> cueIds = new();
            foreach (ThemeVfxProfile.ThemeVfxCueOverride entry in profile.Entries ?? Enumerable.Empty<ThemeVfxProfile.ThemeVfxCueOverride>())
            {
                if (entry == null)
                {
                    result.Add($"ThemeConfig '{themeId}' VFX profile contains a missing entry.");
                    continue;
                }
                if (!StableIdUtility.IsValid(entry.CueId)) result.Add($"ThemeConfig '{themeId}' VFX profile cue id '{entry.CueId}' is invalid.");
                if (!cueIds.Add(entry.CueId)) result.Add($"ThemeConfig '{themeId}' VFX profile contains duplicate cue id '{entry.CueId}'.");
                if (!string.IsNullOrWhiteSpace(entry.OverrideVfxId) && !StableIdUtility.IsValid(entry.OverrideVfxId)) result.Add($"ThemeConfig '{themeId}' VFX profile override id '{entry.OverrideVfxId}' is invalid.");
                if (entry.ScaleMultiplier <= 0f) result.Add($"ThemeConfig '{themeId}' VFX profile cue '{entry.CueId}' scale multiplier must be positive.");
                if (entry.AlphaMultiplier <= 0f || entry.AlphaMultiplier > 1f) result.Add($"ThemeConfig '{themeId}' VFX profile cue '{entry.CueId}' alpha multiplier must be within (0, 1].");
                if (entry.MaxBurstCount < 0) result.Add($"ThemeConfig '{themeId}' VFX profile cue '{entry.CueId}' max burst count must be non-negative.");
            }
            ValidateRequiredIds(cueIds, result, VfxCueIds.Flip, VfxCueIds.NearMiss, VfxCueIds.Death, VfxCueIds.Milestone);
        }
        public static void ValidateThemeAudioProfile(string themeId, ThemeAudioProfile profile, ConfigValidationResult result)
        {
            if (string.IsNullOrWhiteSpace(profile.ProfileId) || !StableIdUtility.IsValid(profile.ProfileId)) result.Add($"ThemeConfig '{themeId}' audio profile id '{profile.ProfileId}' is invalid.");
            if (profile.MaxConcurrentGameplayVoices <= 0) result.Add($"ThemeConfig '{themeId}' audio profile gameplay voice budget must be positive.");
            if (profile.MaxConcurrentUiVoices <= 0) result.Add($"ThemeConfig '{themeId}' audio profile UI voice budget must be positive.");
            if (profile.MinimumUiClickIntervalSeconds < 0f) result.Add($"ThemeConfig '{themeId}' audio profile UI click interval must be non-negative.");
            if (profile.MusicVolumeMultiplier <= 0f) result.Add($"ThemeConfig '{themeId}' audio profile music volume multiplier must be positive.");
            HashSet<string> cueIds = new();
            foreach (ThemeAudioProfile.ThemeAudioCueOverride entry in profile.Entries ?? Enumerable.Empty<ThemeAudioProfile.ThemeAudioCueOverride>())
            {
                if (entry == null)
                {
                    result.Add($"ThemeConfig '{themeId}' audio profile contains a missing entry.");
                    continue;
                }
                if (!StableIdUtility.IsValid(entry.CueId)) result.Add($"ThemeConfig '{themeId}' audio profile cue id '{entry.CueId}' is invalid.");
                if (!cueIds.Add(entry.CueId)) result.Add($"ThemeConfig '{themeId}' audio profile contains duplicate cue id '{entry.CueId}'.");
                if (!string.IsNullOrWhiteSpace(entry.OverrideCueId) && !StableIdUtility.IsValid(entry.OverrideCueId)) result.Add($"ThemeConfig '{themeId}' audio profile override cue id '{entry.OverrideCueId}' is invalid.");
                if (entry.VolumeMultiplier <= 0f) result.Add($"ThemeConfig '{themeId}' audio profile cue '{entry.CueId}' volume multiplier must be positive.");
                if (entry.PitchMultiplier <= 0f) result.Add($"ThemeConfig '{themeId}' audio profile cue '{entry.CueId}' pitch multiplier must be positive.");
            }
            ValidateRequiredIds(cueIds, result, AudioCueIds.Flip, AudioCueIds.Score, AudioCueIds.NearMiss, AudioCueIds.Milestone, AudioCueIds.Death, AudioCueIds.UiClick, AudioCueIds.MainLoop);
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
            ValidateRequiredIds(ids, result, RequiredLiveWireAudioCueIds);
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
            ValidateRequiredIds(ids, result, RequiredLiveWireVfxCueIds);
        }
        private static void ValidatePresentationReadinessRelationships(
            GameBalanceConfig gameBalance,
            GameplayPresentationConfig gameplayPresentation,
            BackgroundPresentationConfig backgroundPresentationConfig,
            PlayerVisualConfig playerVisualConfig,
            HazardPresentationCatalog hazardPresentationCatalog,
            ObstacleVisualCatalog obstacleVisualCatalog,
            ObstacleCatalog obstacleCatalog,
            ThemeCatalog themeCatalog,
            ThemeSequenceConfig themeSequenceConfig,
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
            if (backgroundPresentationConfig != null && backgroundPresentationConfig.LaneQuietZoneHalfWidth <= requiredPlayerClearance)
            {
                result.Add("BackgroundPresentationConfig quiet-zone half width must stay wider than the player readability clearance around the line.");
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
            if (backgroundPresentationConfig != null && backgroundPresentationConfig.LaneQuietZoneHalfWidth <= requiredHazardClearance)
            {
                result.Add("BackgroundPresentationConfig quiet-zone half width must stay wider than the widest obstacle readability clearance around the line.");
            }
            if (obstacleCatalog != null)
            {
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
            ThemeConfig defaultTheme = themeCatalog != null ? themeCatalog.DefaultTheme : null;
            if (defaultTheme != null && defaultTheme.BackgroundPresentationOverride != null
                && defaultTheme.BackgroundPresentationOverride.LaneQuietZoneHalfWidth <= requiredHazardClearance)
            {
                result.Add($"ThemeConfig '{defaultTheme.ThemeId}' background presentation override must preserve the line quiet-zone clearance.");
            }
        }
        public static void ValidateBrandingPresentationConfig(BrandingPresentationConfig config, ConfigValidationResult result)
        {
            if (config == null)
            {
                result.Add("Missing BrandingPresentationConfig asset.");
                return;
            }
            if (string.IsNullOrWhiteSpace(config.PublicTitle)) result.Add("BrandingPresentationConfig public title must not be empty.");
            if (string.IsNullOrWhiteSpace(config.Subtitle)) result.Add("BrandingPresentationConfig subtitle must not be empty.");
            if (config.SplashDurationSeconds <= 0f) result.Add("BrandingPresentationConfig splash duration must be positive.");
            if (config.ShowThemeSelectionInSettings) result.Add("BrandingPresentationConfig theme selection must remain hidden while only one production theme is live.");
            HashSet<BrandingMenuEntryId> entryIds = new();
            foreach (BrandingMenuEntryDefinition entry in config.MenuEntries ?? Enumerable.Empty<BrandingMenuEntryDefinition>())
            {
                if (entry == null)
                {
                    result.Add("BrandingPresentationConfig contains a missing menu entry definition.");
                    continue;
                }
                if (!entryIds.Add(entry.EntryId)) result.Add($"BrandingPresentationConfig contains duplicate menu entry '{entry.EntryId}'.");
            }
            foreach (BrandingMenuEntryId entryId in System.Enum.GetValues(typeof(BrandingMenuEntryId)))
            {
                if (!entryIds.Contains(entryId))
                {
                    result.Add($"BrandingPresentationConfig must define the menu entry '{entryId}'.");
                }
            }
            if (config.GetMenuEntryState(BrandingMenuEntryId.Daily) != BrandingMenuEntryState.Disabled) result.Add("BrandingPresentationConfig Daily entry must stay visible but disabled for the current first-release scope.");

            BrandingMenuEntryState themesState = config.GetMenuEntryState(BrandingMenuEntryId.Themes);
            if (config.ShowGridStatusEntry)
            {
                if (themesState != BrandingMenuEntryState.Enabled) result.Add("BrandingPresentationConfig Themes slot must be enabled when the optional Grid Status surface is promoted.");
            }
            else if (themesState != BrandingMenuEntryState.Disabled)
            {
                result.Add("BrandingPresentationConfig Themes entry must stay visible but disabled while only one production theme is live.");
            }

            if (config.GetMenuEntryState(BrandingMenuEntryId.Best) != BrandingMenuEntryState.Enabled) result.Add("BrandingPresentationConfig Best entry must be enabled in the Live Wire City front door shell.");
            if (config.GetMenuEntryState(BrandingMenuEntryId.Settings) != BrandingMenuEntryState.Enabled) result.Add("BrandingPresentationConfig Settings entry must be enabled in the Live Wire City front door shell.");
        }
        public static void ValidateProductionCopyConfig(ProductionCopyConfig config, ConfigValidationResult result)
        {
            if (config == null)
            {
                result.Add("Missing ProductionCopyConfig asset.");
                return;
            }
            if (string.IsNullOrWhiteSpace(config.MenuHintPrimary)) result.Add("ProductionCopyConfig primary menu hint must not be empty.");
            if (string.IsNullOrWhiteSpace(config.MenuHintSecondary)) result.Add("ProductionCopyConfig secondary menu hint must not be empty.");
            if (string.IsNullOrWhiteSpace(config.PlayButtonLabel)) result.Add("ProductionCopyConfig play button label must not be empty.");
            if (string.IsNullOrWhiteSpace(config.DailyButtonLabel)) result.Add("ProductionCopyConfig daily button label must not be empty.");
            if (string.IsNullOrWhiteSpace(config.ThemesButtonLabel)) result.Add("ProductionCopyConfig themes button label must not be empty.");
            if (string.IsNullOrWhiteSpace(config.GridStatusButtonLabel)) result.Add("ProductionCopyConfig grid-status button label must not be empty.");
            if (string.IsNullOrWhiteSpace(config.BestButtonLabel)) result.Add("ProductionCopyConfig best button label must not be empty.");
            if (string.IsNullOrWhiteSpace(config.SettingsButtonLabel)) result.Add("ProductionCopyConfig settings button label must not be empty.");
            if (string.IsNullOrWhiteSpace(config.RetryButtonLabel)) result.Add("ProductionCopyConfig retry button label must not be empty.");
            if (string.IsNullOrWhiteSpace(config.ShareButtonLabel)) result.Add("ProductionCopyConfig share button label must not be empty.");
            if (string.IsNullOrWhiteSpace(config.SettingsTitle)) result.Add("ProductionCopyConfig settings title must not be empty.");
            if (string.IsNullOrWhiteSpace(config.PauseTitle)) result.Add("ProductionCopyConfig pause title must not be empty.");
            if (string.IsNullOrWhiteSpace(config.ResumeButtonLabel)) result.Add("ProductionCopyConfig resume button label must not be empty.");
            if (string.IsNullOrWhiteSpace(config.RestartButtonLabel)) result.Add("ProductionCopyConfig restart button label must not be empty.");
            if (string.IsNullOrWhiteSpace(config.HomeButtonLabel)) result.Add("ProductionCopyConfig home button label must not be empty.");
            if (string.IsNullOrWhiteSpace(config.CloseButtonLabel)) result.Add("ProductionCopyConfig close button label must not be empty.");
            if (string.IsNullOrWhiteSpace(config.SelectedThemeStatusLabel)) result.Add("ProductionCopyConfig selected-theme status label must not be empty.");
            if (string.IsNullOrWhiteSpace(config.UnlockedThemeStatusLabel)) result.Add("ProductionCopyConfig unlocked-theme status label must not be empty.");
            if (string.IsNullOrWhiteSpace(config.LockedThemeRequirementFormat)) result.Add("ProductionCopyConfig locked-theme requirement format must not be empty.");
            if (string.IsNullOrWhiteSpace(config.ComingSoonStatusLabel)) result.Add("ProductionCopyConfig coming-soon status label must not be empty.");
            if (string.IsNullOrWhiteSpace(config.BestPanelTitle)) result.Add("ProductionCopyConfig best-panel title must not be empty.");
            if (string.IsNullOrWhiteSpace(config.GridStatusTitle)) result.Add("ProductionCopyConfig grid-status title must not be empty.");
            if (string.IsNullOrWhiteSpace(config.FormatGridStatusSummary(12, 2))) result.Add("ProductionCopyConfig grid-status summary format must not be empty.");
            if (string.IsNullOrWhiteSpace(config.GridStatusCurrentLabel)) result.Add("ProductionCopyConfig grid-status current label must not be empty.");
            if (string.IsNullOrWhiteSpace(config.GridStatusOnlineLabel)) result.Add("ProductionCopyConfig grid-status online label must not be empty.");
            if (string.IsNullOrWhiteSpace(config.FormatGridStatusLockedRequirement(10))) result.Add("ProductionCopyConfig grid-status locked requirement format must not be empty.");
            if (string.IsNullOrWhiteSpace(config.FormatGridStatusScoreBand(10, 19))) result.Add("ProductionCopyConfig grid-status score-band format must not be empty.");
            if (string.IsNullOrWhiteSpace(config.FormatGridStatusScoreBand(40, -1))) result.Add("ProductionCopyConfig grid-status open-ended score-band format must not be empty.");
            if (string.IsNullOrWhiteSpace(config.ShareTitle)) result.Add("ProductionCopyConfig share title must not be empty.");
            if (string.IsNullOrWhiteSpace(config.ShareCopyButtonLabel)) result.Add("ProductionCopyConfig share copy button label must not be empty.");
            if (string.IsNullOrWhiteSpace(config.ShareCopiedStatusLabel)) result.Add("ProductionCopyConfig share copied status label must not be empty.");
            if (string.IsNullOrWhiteSpace(config.FormatShareSummary(10, "Failing Grid"))) result.Add("ProductionCopyConfig share summary format must not be empty.");
            if (string.IsNullOrWhiteSpace(config.FormatShareFlavor(1, "Failing Grid"))) result.Add("ProductionCopyConfig share singular flavor format must not be empty.");
            if (string.IsNullOrWhiteSpace(config.FormatShareFlavor(2, "Surge City"))) result.Add("ProductionCopyConfig share plural flavor format must not be empty.");
            if (string.IsNullOrWhiteSpace(config.TutorialTitle)) result.Add("ProductionCopyConfig tutorial title must not be empty.");
            if (string.IsNullOrWhiteSpace(config.TutorialPrimaryLine)) result.Add("ProductionCopyConfig tutorial primary line must not be empty.");
            if (string.IsNullOrWhiteSpace(config.TutorialSecondaryLine)) result.Add("ProductionCopyConfig tutorial secondary line must not be empty.");
            if (string.IsNullOrWhiteSpace(config.TutorialActionLabel)) result.Add("ProductionCopyConfig tutorial action label must not be empty.");
            if (string.IsNullOrWhiteSpace(config.FormatDistrictsRestored(1))) result.Add("ProductionCopyConfig district-restored singular format must not be empty.");
            if (string.IsNullOrWhiteSpace(config.FormatDistrictsRestored(2))) result.Add("ProductionCopyConfig district-restored plural format must not be empty.");
            if (config.ResultTitles == null || config.ResultTitles.Count == 0) result.Add("ProductionCopyConfig must define at least one result title.");
            if (config.NewBestResultTitles == null || config.NewBestResultTitles.Count == 0) result.Add("ProductionCopyConfig must define at least one new-best result title.");
            if (config.ZeroScoreMessages == null || config.ZeroScoreMessages.Count == 0) result.Add("ProductionCopyConfig must define at least one zero-score result message.");
            if (config.LowScoreMessages == null || config.LowScoreMessages.Count == 0) result.Add("ProductionCopyConfig must define at least one low-score result message.");
            if (config.GenericMessages == null || config.GenericMessages.Count == 0) result.Add("ProductionCopyConfig must define at least one generic result message.");
            if (config.NewBestMessages == null || config.NewBestMessages.Count == 0) result.Add("ProductionCopyConfig must define at least one new-best result message.");
            int previousThreshold = -1;
            int milestoneIndex = 0;
            foreach (MilestoneCopyEntry entry in config.MilestoneMessages ?? Enumerable.Empty<MilestoneCopyEntry>())
            {
                if (entry == null)
                {
                    result.Add("ProductionCopyConfig contains a missing milestone message entry.");
                    continue;
                }
                if (entry.ScoreThreshold <= previousThreshold) result.Add("ProductionCopyConfig milestone messages must be strictly ascending.");
                if (entry.ScoreThreshold <= 0) result.Add("ProductionCopyConfig milestone message thresholds must be positive.");
                if (string.IsNullOrWhiteSpace(entry.Message)) result.Add($"ProductionCopyConfig milestone message for score {entry.ScoreThreshold} must not be empty.");
                if (milestoneIndex < RequiredLiveWireMilestones.Length && entry.ScoreThreshold != RequiredLiveWireMilestones[milestoneIndex]) result.Add("ProductionCopyConfig milestone message thresholds must match 10/20/30/40/50.");
                previousThreshold = entry.ScoreThreshold;
                milestoneIndex++;
            }
            if (config.MilestoneMessages == null || config.MilestoneMessages.Count != RequiredLiveWireMilestones.Length)
            {
                result.Add("ProductionCopyConfig must define milestone messages for 10/20/30/40/50.");
            }
            HashSet<ObstacleFamily> failureFamilies = new();
            foreach (FailureResultCopyDefinition entry in config.FailureResultCopies ?? Enumerable.Empty<FailureResultCopyDefinition>())
            {
                if (entry == null)
                {
                    result.Add("ProductionCopyConfig contains a missing failure result copy entry.");
                    continue;
                }
                if (!failureFamilies.Add(entry.Family)) result.Add($"ProductionCopyConfig contains duplicate failure result copy for family '{entry.Family}'.");
                if (entry.Titles == null || entry.Titles.Count == 0 || entry.Titles.Any(string.IsNullOrWhiteSpace)) result.Add($"ProductionCopyConfig failure result copy for family '{entry.Family}' must define non-empty titles.");
                if (entry.Messages == null || entry.Messages.Count == 0 || entry.Messages.Any(string.IsNullOrWhiteSpace)) result.Add($"ProductionCopyConfig failure result copy for family '{entry.Family}' must define non-empty messages.");
            }
            foreach (ObstacleFamily family in System.Enum.GetValues(typeof(ObstacleFamily)))
            {
                if (!failureFamilies.Contains(family))
                {
                    result.Add($"ProductionCopyConfig must define a failure result copy entry for family '{family}'.");
                }
            }
        }
        public static void ValidateUiThemeConfig(UIThemeConfig config, ConfigValidationResult result)
        {
            if (!StableIdUtility.IsValid(config.ThemeId)) result.Add($"UIThemeConfig theme id '{config.ThemeId}' is invalid.");
            if (config.PanelColor.a <= 0f) result.Add("UIThemeConfig panel color alpha must be positive.");
            if (config.PrimaryButtonColor.a <= 0f) result.Add("UIThemeConfig primary button color alpha must be positive.");
            if (config.SecondaryButtonColor.a <= 0f) result.Add("UIThemeConfig secondary button color alpha must be positive.");
            if (config.DestructiveButtonColor.a <= 0f) result.Add("UIThemeConfig destructive button color alpha must be positive.");
            if (config.TextPrimaryColor.a <= 0f) result.Add("UIThemeConfig primary text color alpha must be positive.");
            if (config.PanelOutlineWidth < 0f) result.Add("UIThemeConfig panel outline width must be non-negative.");
            if (config.ButtonOutlineWidth < 0f) result.Add("UIThemeConfig button outline width must be non-negative.");
            if (config.TextShadowDistance < 0f) result.Add("UIThemeConfig text shadow distance must be non-negative.");
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



