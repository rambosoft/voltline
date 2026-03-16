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
        public void DifficultyCurve_ReturnsExpectedBandsForRepresentativeScores()
        {
            DifficultyCurveConfig difficultyCurve = AssetDatabase.LoadAssetAtPath<DifficultyCurveConfig>(ProjectConfigAssetPaths.DifficultyCurve);

            Assert.That(difficultyCurve.TryGetBandForScore(0, out DifficultyCurveConfig.DifficultyBandDefinition band0), Is.True);
            Assert.That(band0.BandId, Is.EqualTo("band.0"));

            Assert.That(difficultyCurve.TryGetBandForScore(9, out DifficultyCurveConfig.DifficultyBandDefinition band1), Is.True);
            Assert.That(band1.BandId, Is.EqualTo("band.1"));

            Assert.That(difficultyCurve.TryGetBandForScore(42, out DifficultyCurveConfig.DifficultyBandDefinition band4), Is.True);
            Assert.That(band4.BandId, Is.EqualTo("band.4"));
        }

        [Test]
        public void DifficultyDirector_UnlocksApprovedFamiliesInReadableOrder()
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
                Assert.That(ContainsFamily(eligible, ObstacleFamily.Spikes), Is.True);
                Assert.That(ContainsFamily(eligible, ObstacleFamily.RotatingCutters), Is.False);
                Assert.That(ContainsFamily(eligible, ObstacleFamily.ElectricGates), Is.False);
                Assert.That(ContainsFamily(eligible, ObstacleFamily.SideBlockers), Is.False);
                Assert.That(ContainsFamily(eligible, ObstacleFamily.BrokenLineGaps), Is.False);

                director.PopulateEligibleObstacleConfigs(8, eligible);
                Assert.That(ContainsFamily(eligible, ObstacleFamily.RotatingCutters), Is.True);
                Assert.That(ContainsFamily(eligible, ObstacleFamily.ElectricGates), Is.True);

                director.PopulateEligibleObstacleConfigs(12, eligible);
                Assert.That(ContainsFamily(eligible, ObstacleFamily.SideBlockers), Is.True);

                director.PopulateEligibleObstacleConfigs(18, eligible);
                Assert.That(ContainsFamily(eligible, ObstacleFamily.BrokenLineGaps), Is.True);
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
                Assert.That(profile.MainSprite, Is.Not.Null, family.ToString());
            }
        }

        [Test]
        public void BackgroundPresentationConfig_UsesSmallReadableBudget()
        {
            GameBalanceConfig gameBalance = AssetDatabase.LoadAssetAtPath<GameBalanceConfig>(ProjectConfigAssetPaths.GameBalance);
            PlayerVisualConfig playerVisualConfig = AssetDatabase.LoadAssetAtPath<PlayerVisualConfig>(ProjectConfigAssetPaths.PlayerVisualConfig);
            BackgroundPresentationConfig backgroundPresentationConfig = AssetDatabase.LoadAssetAtPath<BackgroundPresentationConfig>(ProjectConfigAssetPaths.BackgroundPresentation);

            Assert.That(backgroundPresentationConfig.MaxRuntimeSpriteCount, Is.LessThanOrEqualTo(3));
            Assert.That(backgroundPresentationConfig.MaxExpectedDrawCalls, Is.LessThanOrEqualTo(3));
            Assert.That(backgroundPresentationConfig.LaneQuietZoneHalfWidth, Is.GreaterThan(gameBalance.SideOffset + playerVisualConfig.VisibleHalfWidth));
        }

        [Test]
        public void ThemeCatalog_ContainsLaunchThemeUnlockSet_AndPipelineProfiles()
        {
            ThemeCatalog themeCatalog = AssetDatabase.LoadAssetAtPath<ThemeCatalog>(ProjectConfigAssetPaths.ThemeCatalog);

            Assert.That(themeCatalog.Themes.Count, Is.GreaterThanOrEqualTo(2));
            Assert.That(themeCatalog.DefaultThemeId, Is.EqualTo("theme.neon-night"));
            Assert.That(themeCatalog.TryGetTheme("theme.candy-pop", out ThemeConfig candyPop), Is.True);
            Assert.That(candyPop.UnlockedByDefault, Is.False);
            Assert.That(candyPop.UnlockBestScoreThreshold, Is.EqualTo(20));
            Assert.That(candyPop.BackgroundPresentationOverride, Is.Not.Null);
            Assert.That(candyPop.AllowRuntimeSequenceSelection, Is.True);
            Assert.That(candyPop.ThemeVfxProfile, Is.Not.Null);
            Assert.That(candyPop.ThemeAudioProfile, Is.Not.Null);
        }

        [Test]
        public void ThemeSequenceConfig_TargetsCandyPopOnMilestoneWithoutRuntimeVisualSwaps()
        {
            ThemeCatalog themeCatalog = AssetDatabase.LoadAssetAtPath<ThemeCatalog>(ProjectConfigAssetPaths.ThemeCatalog);
            ThemeSequenceConfig themeSequenceConfig = AssetDatabase.LoadAssetAtPath<ThemeSequenceConfig>(ProjectConfigAssetPaths.ThemeSequence);

            Assert.That(themeSequenceConfig.EnableRuntimeTransitions, Is.True);
            Assert.That(themeSequenceConfig.Entries.Count, Is.EqualTo(1));
            ThemeSequenceEntry entry = themeSequenceConfig.Entries[0];
            Assert.That(entry.ScoreThreshold, Is.EqualTo(20));
            Assert.That(entry.ThemeId, Is.EqualTo("theme.candy-pop"));
            Assert.That(themeCatalog.TryGetTheme(entry.ThemeId, out ThemeConfig theme), Is.True);
            Assert.That(theme.PlayerVisualOverride, Is.Null);
            Assert.That(theme.ObstacleVisualOverride, Is.Null);
            Assert.That(theme.ThemeVfxProfile, Is.Not.Null);
            Assert.That(theme.ThemeAudioProfile, Is.Not.Null);
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
                Assert.That(theme.ThemeVfxProfile.Entries.Count, Is.EqualTo(4), theme.ThemeId);
                Assert.That(theme.ThemeAudioProfile.Entries.Count, Is.EqualTo(7), theme.ThemeId);
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
