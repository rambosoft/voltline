#if UNITY_EDITOR
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using Voltline.Data;
using Voltline.Gameplay;
using Voltline.Save;
namespace Voltline.Tests.EditMode
{
    public sealed class DataBackboneEditModeTests
    {
        [Test]
        public void ProjectConfigAssets_ValidateWithoutErrors()
        {
            GameBalanceConfig gameBalance = AssetDatabase.LoadAssetAtPath<GameBalanceConfig>(ProjectConfigAssetPaths.GameBalance);
            DifficultyCurveConfig difficultyCurve = AssetDatabase.LoadAssetAtPath<DifficultyCurveConfig>(ProjectConfigAssetPaths.DifficultyCurve);
            GameplayPresentationConfig gameplayPresentation = AssetDatabase.LoadAssetAtPath<GameplayPresentationConfig>(ProjectConfigAssetPaths.GameplayPresentation);
            BackgroundPresentationConfig backgroundPresentationConfig = AssetDatabase.LoadAssetAtPath<BackgroundPresentationConfig>(ProjectConfigAssetPaths.BackgroundPresentation);
            PlayerVisualConfig playerVisualConfig = AssetDatabase.LoadAssetAtPath<PlayerVisualConfig>(ProjectConfigAssetPaths.PlayerVisualConfig);
            HazardPresentationCatalog hazardPresentationCatalog = AssetDatabase.LoadAssetAtPath<HazardPresentationCatalog>(ProjectConfigAssetPaths.HazardPresentationCatalog);
            ObstacleVisualCatalog obstacleVisualCatalog = AssetDatabase.LoadAssetAtPath<ObstacleVisualCatalog>(ProjectConfigAssetPaths.ObstacleVisualCatalog);
            ObstacleCatalog obstacleCatalog = AssetDatabase.LoadAssetAtPath<ObstacleCatalog>(ProjectConfigAssetPaths.ObstacleCatalog);
            ThemeCatalog themeCatalog = AssetDatabase.LoadAssetAtPath<ThemeCatalog>(ProjectConfigAssetPaths.ThemeCatalog);
            ThemeSequenceConfig themeSequenceConfig = AssetDatabase.LoadAssetAtPath<ThemeSequenceConfig>(ProjectConfigAssetPaths.ThemeSequence);
            PresentationRolloutPlanConfig presentationRolloutPlan = AssetDatabase.LoadAssetAtPath<PresentationRolloutPlanConfig>(ProjectConfigAssetPaths.PresentationRolloutPlan);
            AudioCueCatalog audioCueCatalog = AssetDatabase.LoadAssetAtPath<AudioCueCatalog>(ProjectConfigAssetPaths.AudioCueCatalog);
            VfxCatalog vfxCatalog = AssetDatabase.LoadAssetAtPath<VfxCatalog>(ProjectConfigAssetPaths.VfxCatalog);
            ConfigValidationResult result = ProjectConfigValidator.ValidateProject(
                gameBalance,
                difficultyCurve,
                gameplayPresentation,
                backgroundPresentationConfig,
                playerVisualConfig,
                hazardPresentationCatalog,
                obstacleVisualCatalog,
                obstacleCatalog,
                themeCatalog,
                themeSequenceConfig,
                presentationRolloutPlan,
                audioCueCatalog,
                vfxCatalog);
            Assert.That(result.IsValid, Is.True, result.ToString());
        }
        [Test]
        public void DifficultyCurve_ReturnsExpectedDistrictBandsForRepresentativeScores()
        {
            DifficultyCurveConfig difficultyCurve = AssetDatabase.LoadAssetAtPath<DifficultyCurveConfig>(ProjectConfigAssetPaths.DifficultyCurve);
            Assert.That(difficultyCurve.TryGetBandForScore(0, out DifficultyCurveConfig.DifficultyBandDefinition band0), Is.True);
            Assert.That(band0.BandId, Is.EqualTo("band.0"));
            Assert.That(difficultyCurve.TryGetBandForScore(15, out DifficultyCurveConfig.DifficultyBandDefinition band1), Is.True);
            Assert.That(band1.BandId, Is.EqualTo("band.1"));
            Assert.That(difficultyCurve.TryGetBandForScore(42, out DifficultyCurveConfig.DifficultyBandDefinition band4), Is.True);
            Assert.That(band4.BandId, Is.EqualTo("band.4"));
        }

        [Test]
        public void DifficultyOpening_RemainsGentleBeforeScoreTenThenStepsUpCleanly()
        {
            GameBalanceConfig gameBalance = AssetDatabase.LoadAssetAtPath<GameBalanceConfig>(ProjectConfigAssetPaths.GameBalance);
            DifficultyCurveConfig difficultyCurve = AssetDatabase.LoadAssetAtPath<DifficultyCurveConfig>(ProjectConfigAssetPaths.DifficultyCurve);
            GameObject root = new("DifficultyOpeningTests");

            try
            {
                DifficultyDirector director = root.AddComponent<DifficultyDirector>();
                director.Initialize(gameBalance, difficultyCurve, null);

                float speedAtZero = director.GetCurrentSpeed(0);
                float speedAtNine = director.GetCurrentSpeed(9);
                float speedAtTen = director.GetCurrentSpeed(10);
                director.GetSpawnSpacingRange(0, null, out float minSpacingAtZero, out float maxSpacingAtZero);
                director.GetSpawnSpacingRange(10, null, out float minSpacingAtTen, out float maxSpacingAtTen);

                Assert.That(gameBalance.SafeStartWindowSeconds, Is.GreaterThanOrEqualTo(1.55f));
                Assert.That(speedAtZero, Is.LessThan(5.5f));
                Assert.That(speedAtNine, Is.GreaterThan(speedAtZero));
                Assert.That(speedAtTen, Is.GreaterThan(speedAtNine));
                Assert.That(speedAtTen - speedAtNine, Is.LessThan(0.5f));
                Assert.That(minSpacingAtZero, Is.GreaterThan(minSpacingAtTen));
                Assert.That(maxSpacingAtZero, Is.GreaterThan(maxSpacingAtTen));
                Assert.That(minSpacingAtZero, Is.GreaterThanOrEqualTo(3f));
            }
            finally
            {
                Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void DifficultyDirector_UnlocksLiveWireCityHazardFamiliesInReadableOrder()
        {
            GameBalanceConfig gameBalance = AssetDatabase.LoadAssetAtPath<GameBalanceConfig>(ProjectConfigAssetPaths.GameBalance);
            DifficultyCurveConfig difficultyCurve = AssetDatabase.LoadAssetAtPath<DifficultyCurveConfig>(ProjectConfigAssetPaths.DifficultyCurve);
            ObstacleCatalog obstacleCatalog = AssetDatabase.LoadAssetAtPath<ObstacleCatalog>(ProjectConfigAssetPaths.ObstacleCatalog);
            GameObject root = new("DifficultyDirectorTests");
            try
            {
                DifficultyDirector director = root.AddComponent<DifficultyDirector>();
                director.Initialize(gameBalance, difficultyCurve, obstacleCatalog);
                List<ObstacleConfig> eligible = new();
                director.PopulateEligibleObstacleConfigs(0, eligible);
                Assert.That(ContainsFamily(eligible, ObstacleFamily.SharpUtilityHazards), Is.True);
                Assert.That(ContainsFamily(eligible, ObstacleFamily.GroundedBlockers), Is.False);
                Assert.That(ContainsFamily(eligible, ObstacleFamily.ActiveElectricHazards), Is.False);
                Assert.That(ContainsFamily(eligible, ObstacleFamily.RotatingIndustrialHazards), Is.False);
                Assert.That(ContainsFamily(eligible, ObstacleFamily.SidePressureHazards), Is.False);
                Assert.That(ContainsFamily(eligible, ObstacleFamily.BrokenConduitSections), Is.False);
                director.PopulateEligibleObstacleConfigs(8, eligible);
                Assert.That(ContainsFamily(eligible, ObstacleFamily.GroundedBlockers), Is.True);
                Assert.That(ContainsFamily(eligible, ObstacleFamily.ActiveElectricHazards), Is.True);
                Assert.That(ContainsFamily(eligible, ObstacleFamily.RotatingIndustrialHazards), Is.True);
                director.PopulateEligibleObstacleConfigs(12, eligible);
                Assert.That(ContainsFamily(eligible, ObstacleFamily.SidePressureHazards), Is.True);
                director.PopulateEligibleObstacleConfigs(18, eligible);
                Assert.That(ContainsFamily(eligible, ObstacleFamily.BrokenConduitSections), Is.True);
            }
            finally
            {
                Object.DestroyImmediate(root);
            }
        }
        [Test]
        public void HazardPresentationCatalog_CoversEveryApprovedFamily()
        {
            HazardPresentationCatalog catalog = AssetDatabase.LoadAssetAtPath<HazardPresentationCatalog>(ProjectConfigAssetPaths.HazardPresentationCatalog);
            foreach (ObstacleFamily family in System.Enum.GetValues(typeof(ObstacleFamily)))
            {
                Assert.That(catalog.TryGetProfile(family, out HazardLayoutProfile profile), Is.True, family.ToString());
                Assert.That(profile.VisualBoundsScale.x, Is.GreaterThan(0f), family.ToString());
                Assert.That(profile.CollisionBoundsScale.x, Is.GreaterThan(0f), family.ToString());
            }
        }
        [Test]
        public void ObstacleVisualCatalog_CoversEveryApprovedFamily()
        {
            ObstacleVisualCatalog catalog = AssetDatabase.LoadAssetAtPath<ObstacleVisualCatalog>(ProjectConfigAssetPaths.ObstacleVisualCatalog);
            foreach (ObstacleFamily family in System.Enum.GetValues(typeof(ObstacleFamily)))
            {
                Assert.That(catalog.TryGetProfile(family, out ObstacleVisualProfile profile), Is.True, family.ToString());
                Assert.That(profile.VisualBoundsScale.x, Is.GreaterThan(0f), family.ToString());
            }
            Assert.That(catalog.GetRequiredProfile(ObstacleFamily.GroundedBlockers).MainSprite, Is.Not.Null);
        }
        [Test]
        public void BackgroundPresentationConfig_UsesSmallReadableBudget()
        {
            GameBalanceConfig gameBalance = AssetDatabase.LoadAssetAtPath<GameBalanceConfig>(ProjectConfigAssetPaths.GameBalance);
            PlayerVisualConfig playerVisualConfig = AssetDatabase.LoadAssetAtPath<PlayerVisualConfig>(ProjectConfigAssetPaths.PlayerVisualConfig);
            BackgroundPresentationConfig backgroundPresentationConfig = AssetDatabase.LoadAssetAtPath<BackgroundPresentationConfig>(ProjectConfigAssetPaths.BackgroundPresentation);
            Assert.That(backgroundPresentationConfig.MaxRuntimeSpriteCount, Is.LessThanOrEqualTo(12));
            Assert.That(backgroundPresentationConfig.MaxExpectedDrawCalls, Is.LessThanOrEqualTo(12));
            Assert.That(backgroundPresentationConfig.LaneQuietZoneHalfWidth, Is.GreaterThan(gameBalance.SideOffset + playerVisualConfig.VisibleHalfWidth));
            Assert.That(backgroundPresentationConfig.Layers.Count, Is.EqualTo(12));

            Assert.That(backgroundPresentationConfig.Layers[0].LayerId, Is.EqualTo("layer.skyline.left"));
            Assert.That(backgroundPresentationConfig.Layers[1].LayerId, Is.EqualTo("layer.tower.boost.left"));
            Assert.That(backgroundPresentationConfig.Layers[2].LayerId, Is.EqualTo("layer.skyline.left.windows"));
            Assert.That(backgroundPresentationConfig.Layers[3].LayerId, Is.EqualTo("layer.skyline.right"));
            Assert.That(backgroundPresentationConfig.Layers[4].LayerId, Is.EqualTo("layer.tower.boost.right"));
            Assert.That(backgroundPresentationConfig.Layers[5].LayerId, Is.EqualTo("layer.skyline.right.windows"));
            Assert.That(backgroundPresentationConfig.Layers[6].LayerId, Is.EqualTo("layer.beacon.left"));
            Assert.That(backgroundPresentationConfig.Layers[7].LayerId, Is.EqualTo("layer.beacon.right"));
            Assert.That(backgroundPresentationConfig.Layers[8].LayerId, Is.EqualTo("layer.utility.band.left"));
            Assert.That(backgroundPresentationConfig.Layers[9].LayerId, Is.EqualTo("layer.utility.band.right"));
            Assert.That(backgroundPresentationConfig.Layers[10].LayerId, Is.EqualTo("layer.atmosphere.haze"));
            Assert.That(backgroundPresentationConfig.Layers[11].LayerId, Is.EqualTo("layer.atmosphere.streaks"));

            Assert.That(backgroundPresentationConfig.Layers[1].Sprite, Is.Null);
            Assert.That(backgroundPresentationConfig.Layers[1].ColorRole, Is.EqualTo(BackgroundLayerColorRole.SkylineSilhouette));
            Assert.That(backgroundPresentationConfig.Layers[1].Alpha, Is.EqualTo(0.105f).Within(0.0001f));
            Assert.That(backgroundPresentationConfig.Layers[1].ContentFill.x, Is.EqualTo(0.539f).Within(0.001f));
            Assert.That(backgroundPresentationConfig.Layers[1].ContentFill.y, Is.EqualTo(0.419f).Within(0.001f));
            Assert.That(backgroundPresentationConfig.Layers[1].DistrictVariants.Count, Is.EqualTo(2));
            Assert.That(backgroundPresentationConfig.Layers[1].DistrictVariants[0].MinimumScoreThreshold, Is.EqualTo(25));
            Assert.That(backgroundPresentationConfig.Layers[1].DistrictVariants[1].MinimumScoreThreshold, Is.EqualTo(35));
            Assert.That(backgroundPresentationConfig.Layers[1].DistrictVariants[1].AlphaMultiplier, Is.EqualTo(1.18f).Within(0.001f));
            Assert.That(backgroundPresentationConfig.Layers[1].DistrictVariants[0].Sprite, Is.Not.Null);

            Assert.That(backgroundPresentationConfig.Layers[4].Sprite, Is.Null);
            Assert.That(backgroundPresentationConfig.Layers[4].ColorRole, Is.EqualTo(BackgroundLayerColorRole.SkylineSilhouette));
            Assert.That(backgroundPresentationConfig.Layers[4].Alpha, Is.EqualTo(0.105f).Within(0.0001f));
            Assert.That(backgroundPresentationConfig.Layers[4].ContentFill.x, Is.EqualTo(0.598f).Within(0.001f));
            Assert.That(backgroundPresentationConfig.Layers[4].ContentFill.y, Is.EqualTo(0.41f).Within(0.001f));
            Assert.That(backgroundPresentationConfig.Layers[4].DistrictVariants.Count, Is.EqualTo(2));
            Assert.That(backgroundPresentationConfig.Layers[4].DistrictVariants[0].MinimumScoreThreshold, Is.EqualTo(25));
            Assert.That(backgroundPresentationConfig.Layers[4].DistrictVariants[1].MinimumScoreThreshold, Is.EqualTo(35));
            Assert.That(backgroundPresentationConfig.Layers[4].DistrictVariants[1].AlphaMultiplier, Is.EqualTo(1.18f).Within(0.001f));
            Assert.That(backgroundPresentationConfig.Layers[4].DistrictVariants[0].Sprite, Is.Not.Null);

            Assert.That(backgroundPresentationConfig.Layers[6].DistrictVariants.Count, Is.EqualTo(3));
            Assert.That(backgroundPresentationConfig.Layers[7].DistrictVariants.Count, Is.EqualTo(3));
            Assert.That(backgroundPresentationConfig.Layers[8].Sprite, Is.Not.Null);
            Assert.That(backgroundPresentationConfig.Layers[9].Sprite, Is.Not.Null);
            Assert.That(backgroundPresentationConfig.Layers[10].Sprite, Is.Not.Null);
            Assert.That(backgroundPresentationConfig.Layers[11].Sprite, Is.Not.Null);
        }

        [Test]
        public void ThemeCatalog_ContainsSingleLiveWireCityTheme_WithWorldProgressionAndPipelineProfiles()
        {
            ThemeCatalog themeCatalog = AssetDatabase.LoadAssetAtPath<ThemeCatalog>(ProjectConfigAssetPaths.ThemeCatalog);
            Assert.That(themeCatalog.Themes.Count, Is.EqualTo(1));
            Assert.That(themeCatalog.DefaultThemeId, Is.EqualTo("theme.live-wire-city"));
            Assert.That(themeCatalog.BrandingPresentationConfig, Is.Not.Null);
            Assert.That(themeCatalog.BrandingPresentationConfig.PublicTitle, Is.EqualTo("Voltline"));
            Assert.That(themeCatalog.BrandingPresentationConfig.Subtitle, Is.EqualTo("Keep the grid alive"));
            Assert.That(themeCatalog.BrandingPresentationConfig.GetMenuEntryState(BrandingMenuEntryId.Daily), Is.EqualTo(BrandingMenuEntryState.Disabled));
            Assert.That(themeCatalog.BrandingPresentationConfig.ShowSplashLogoMoment, Is.True);
            Assert.That(themeCatalog.BrandingPresentationConfig.ShowGridStatusEntry, Is.True);
            Assert.That(themeCatalog.BrandingPresentationConfig.ShowShareEntry, Is.True);
            Assert.That(themeCatalog.BrandingPresentationConfig.ShowExpandedTutorialHint, Is.True);
            Assert.That(themeCatalog.BrandingPresentationConfig.GetMenuEntryState(BrandingMenuEntryId.Themes), Is.EqualTo(BrandingMenuEntryState.Enabled));
            Assert.That(themeCatalog.BrandingPresentationConfig.GetMenuEntryState(BrandingMenuEntryId.Best), Is.EqualTo(BrandingMenuEntryState.Enabled));
            Assert.That(themeCatalog.BrandingPresentationConfig.GetMenuEntryState(BrandingMenuEntryId.Settings), Is.EqualTo(BrandingMenuEntryState.Enabled));
            Assert.That(themeCatalog.ProductionCopyConfig, Is.Not.Null);
            Assert.That(themeCatalog.UiThemeConfig, Is.Not.Null);
            Assert.That(themeCatalog.ShouldShowThemeSelectionInSettings, Is.False);
            Assert.That(themeCatalog.TryGetTheme("theme.live-wire-city", out ThemeConfig liveWireCity), Is.True);
            Assert.That(liveWireCity.UnlockedByDefault, Is.True);
            Assert.That(liveWireCity.ThemeVfxProfile, Is.Not.Null);
            Assert.That(liveWireCity.ThemeAudioProfile, Is.Not.Null);
            Assert.That(liveWireCity.ThemeVfxProfile.ProfileId, Is.EqualTo("theme.live-wire-city.vfx"));
            Assert.That(liveWireCity.ThemeAudioProfile.ProfileId, Is.EqualTo("theme.live-wire-city.audio"));
            Assert.That(liveWireCity.ResolveWorldProgressionConfig(), Is.Not.Null);
            Assert.That(liveWireCity.AllowRuntimeSequenceSelection, Is.False);
        }
        [Test]
        public void ThemeSequenceConfig_RemainsDormantCompatibilityAsset()
        {
            ThemeSequenceConfig themeSequenceConfig = AssetDatabase.LoadAssetAtPath<ThemeSequenceConfig>(ProjectConfigAssetPaths.ThemeSequence);
            Assert.That(themeSequenceConfig.EnableRuntimeTransitions, Is.False);
            Assert.That(themeSequenceConfig.Entries, Is.Empty);
        }
        [Test]
        public void WorldProgressionConfig_DefinesFiveDistrictsAndFiveMilestoneReactions()
        {
            WorldProgressionConfig config = AssetDatabase.LoadAssetAtPath<WorldProgressionConfig>(ProjectConfigAssetPaths.WorldProgression);
            Assert.That(config, Is.Not.Null);
            Assert.That(config.ProgressionId, Is.EqualTo("world.live-wire-city"));
            Assert.That(config.DistrictStates.Count, Is.EqualTo(5));
            Assert.That(config.GetRequiredDistrictForScore(0).DisplayName, Is.EqualTo("Failing Grid"));
            Assert.That(config.GetRequiredDistrictForScore(10).DisplayName, Is.EqualTo("Local Power Restored"));
            Assert.That(config.GetRequiredDistrictForScore(20).DisplayName, Is.EqualTo("Grid Stabilization"));
            Assert.That(config.GetRequiredDistrictForScore(30).DisplayName, Is.EqualTo("Surge City"));
            Assert.That(config.GetRequiredDistrictForScore(40).DisplayName, Is.EqualTo("Overclock City"));
            Assert.That(config.MilestoneReactions.Count, Is.EqualTo(5));
            Assert.That(config.TryGetMilestoneReaction(10, out _), Is.True);
            Assert.That(config.TryGetMilestoneReaction(20, out _), Is.True);
            Assert.That(config.TryGetMilestoneReaction(30, out _), Is.True);
            Assert.That(config.TryGetMilestoneReaction(40, out _), Is.True);
            Assert.That(config.TryGetMilestoneReaction(50, out _), Is.True);
        }
        [Test]
        public void ThemeProfiles_DefineRequiredSemanticEntries()
        {
            ThemeCatalog themeCatalog = AssetDatabase.LoadAssetAtPath<ThemeCatalog>(ProjectConfigAssetPaths.ThemeCatalog);
            for (int i = 0; i < themeCatalog.Themes.Count; i++)
            {
                ThemeConfig theme = themeCatalog.Themes[i];
                Assert.That(theme, Is.Not.Null);
                Assert.That(theme.ThemeVfxProfile, Is.Not.Null, theme.ThemeId);
                Assert.That(theme.ThemeAudioProfile, Is.Not.Null, theme.ThemeId);
                Assert.That(theme.ThemeVfxProfile.ProfileId, Is.EqualTo("theme.live-wire-city.vfx"), theme.ThemeId);
                Assert.That(theme.ThemeAudioProfile.ProfileId, Is.EqualTo("theme.live-wire-city.audio"), theme.ThemeId);
                Assert.That(theme.ThemeVfxProfile.Entries.Count, Is.GreaterThanOrEqualTo(4), theme.ThemeId);
                Assert.That(theme.ThemeAudioProfile.Entries.Count, Is.GreaterThanOrEqualTo(7), theme.ThemeId);
            }
        }
        [Test]
        public void PresentationRolloutPlan_UsesApprovedSliceOrder()
        {
            PresentationRolloutPlanConfig rolloutPlan = AssetDatabase.LoadAssetAtPath<PresentationRolloutPlanConfig>(ProjectConfigAssetPaths.PresentationRolloutPlan);
            Assert.That(rolloutPlan, Is.Not.Null);
            Assert.That(rolloutPlan.RequireConfigValidation, Is.True);
            Assert.That(rolloutPlan.RequireReleaseAudit, Is.True);
            Assert.That(rolloutPlan.RequirePresentationReadinessAudit, Is.True);
            Assert.That(rolloutPlan.Slices.Count, Is.EqualTo(7));
            Assert.That(rolloutPlan.Slices[0].SliceId, Is.EqualTo(PresentationRolloutSliceId.PlayerRefresh));
            Assert.That(rolloutPlan.Slices[5].SliceId, Is.EqualTo(PresentationRolloutSliceId.VfxRefresh));
            Assert.That(rolloutPlan.Slices[6].SliceId, Is.EqualTo(PresentationRolloutSliceId.AudioRefresh));
            Assert.That(rolloutPlan.Slices[6].RequiresCollisionReview, Is.False);
        }
        [Test]
        public void PhaseThreeToFive_ConfigAssetsExistAtDocumentedPaths()
        {
            BrandingPresentationConfig branding = AssetDatabase.LoadAssetAtPath<BrandingPresentationConfig>(ProjectConfigAssetPaths.BrandingPresentation);
            ProductionCopyConfig copy = AssetDatabase.LoadAssetAtPath<ProductionCopyConfig>(ProjectConfigAssetPaths.ProductionCopy);
            UIThemeConfig uiTheme = AssetDatabase.LoadAssetAtPath<UIThemeConfig>(ProjectConfigAssetPaths.UiTheme);
            WorldProgressionConfig progression = AssetDatabase.LoadAssetAtPath<WorldProgressionConfig>(ProjectConfigAssetPaths.WorldProgression);
            ThemeAudioProfile audioProfile = AssetDatabase.LoadAssetAtPath<ThemeAudioProfile>(ProjectConfigAssetPaths.ThemeAudioProfile);
            ThemeVfxProfile vfxProfile = AssetDatabase.LoadAssetAtPath<ThemeVfxProfile>(ProjectConfigAssetPaths.ThemeVfxProfile);
            Assert.That(branding, Is.Not.Null);
            Assert.That(copy, Is.Not.Null);
            Assert.That(uiTheme, Is.Not.Null);
            Assert.That(progression, Is.Not.Null);
            Assert.That(audioProfile, Is.Not.Null);
            Assert.That(vfxProfile, Is.Not.Null);
            Assert.That(branding.PublicTitle, Is.EqualTo("Voltline"));
            Assert.That(copy.GridStatusButtonLabel, Is.EqualTo("Grid"));
            Assert.That(copy.ShareButtonLabel, Is.EqualTo("Share"));
            Assert.That(copy.TutorialTitle, Is.EqualTo("KEEP THE GRID ALIVE"));
            Assert.That(copy.TryGetLatestMilestoneMessage(40, out string milestoneMessage), Is.True);
            Assert.That(milestoneMessage, Is.EqualTo("FULL CHARGE"));
            Assert.That(copy.TryGetFailureResultCopy(ObstacleFamily.ActiveElectricHazards, 14, out string failureTitle, out string failureMessage), Is.True);
            Assert.That(failureTitle, Is.EqualTo("OVERLOAD"));
            Assert.That(failureMessage, Is.EqualTo("The transfer spiked out in an electric surge."));
            Assert.That(uiTheme.ThemeId, Is.EqualTo("ui.live-wire-city"));
            Assert.That(audioProfile.ProfileId, Is.EqualTo("theme.live-wire-city.audio"));
            Assert.That(vfxProfile.ProfileId, Is.EqualTo("theme.live-wire-city.vfx"));
        }
        [Test]
        public void DefaultProfile_UsesVersionOneAndDefaultTheme()
        {
            ThemeCatalog themeCatalog = AssetDatabase.LoadAssetAtPath<ThemeCatalog>(ProjectConfigAssetPaths.ThemeCatalog);
            PlayerProfileSaveData profile = SaveSchema.CreateDefaultProfile(themeCatalog.DefaultThemeId);
            Assert.That(profile.version, Is.EqualTo(SaveSchema.CurrentVersion));
            Assert.That(profile.selectedThemeId, Is.EqualTo(themeCatalog.DefaultThemeId));
            Assert.That(profile.unlockedThemeIds.Count, Is.EqualTo(1));
            Assert.That(profile.unlockedThemeIds[0], Is.EqualTo(themeCatalog.DefaultThemeId));
            Assert.That(profile.musicVolume, Is.EqualTo(1f));
            Assert.That(profile.sfxVolume, Is.EqualTo(1f));
            Assert.That(profile.vibrationEnabled, Is.True);
            Assert.That(profile.hasSeenFirstLaunchHint, Is.False);
        }
        private static bool ContainsFamily(List<ObstacleConfig> eligible, ObstacleFamily family)
        {
            for (int i = 0; i < eligible.Count; i++)
            {
                if (eligible[i] != null && eligible[i].Family == family)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
#endif





