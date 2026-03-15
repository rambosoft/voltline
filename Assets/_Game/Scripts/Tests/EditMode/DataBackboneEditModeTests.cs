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
            ObstacleCatalog obstacleCatalog = AssetDatabase.LoadAssetAtPath<ObstacleCatalog>(ProjectConfigAssetPaths.ObstacleCatalog);
            ThemeCatalog themeCatalog = AssetDatabase.LoadAssetAtPath<ThemeCatalog>(ProjectConfigAssetPaths.ThemeCatalog);
            AudioCueCatalog audioCueCatalog = AssetDatabase.LoadAssetAtPath<AudioCueCatalog>(ProjectConfigAssetPaths.AudioCueCatalog);
            VfxCatalog vfxCatalog = AssetDatabase.LoadAssetAtPath<VfxCatalog>(ProjectConfigAssetPaths.VfxCatalog);

            ConfigValidationResult result = ProjectConfigValidator.ValidateProject(gameBalance, difficultyCurve, obstacleCatalog, themeCatalog, audioCueCatalog, vfxCatalog);

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
        public void ThemeCatalog_ContainsLaunchThemeUnlockSet()
        {
            ThemeCatalog themeCatalog = AssetDatabase.LoadAssetAtPath<ThemeCatalog>(ProjectConfigAssetPaths.ThemeCatalog);

            Assert.That(themeCatalog.Themes.Count, Is.GreaterThanOrEqualTo(2));
            Assert.That(themeCatalog.DefaultThemeId, Is.EqualTo("theme.neon-night"));
            Assert.That(themeCatalog.TryGetTheme("theme.candy-pop", out ThemeConfig candyPop), Is.True);
            Assert.That(candyPop.UnlockedByDefault, Is.False);
            Assert.That(candyPop.UnlockBestScoreThreshold, Is.EqualTo(20));
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
