#if UNITY_EDITOR
using NUnit.Framework;
using UnityEditor;
using Voltline.Data;
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
    }
}
#endif