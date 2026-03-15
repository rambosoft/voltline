#if UNITY_EDITOR
using System.IO;
using NUnit.Framework;
using UnityEditor;
using Voltline.Data;
using Voltline.Save;

namespace Voltline.Tests.EditMode
{
    public sealed class SavePersistenceEditModeTests
    {
        [Test]
        public void UpgradeToCurrent_FillsMissingThemeAndUnlocksSelectedTheme()
        {
            PlayerProfileSaveData profile = new()
            {
                version = 0,
                bestScore = -5,
                selectedThemeId = null,
                unlockedThemeIds = null,
                musicVolume = 0f,
                sfxVolume = 0f,
                dailyChallenge = null,
            };

            PlayerProfileSaveData upgraded = SaveSchema.UpgradeToCurrent(profile, SaveSchema.DefaultThemeId);

            Assert.That(upgraded.version, Is.EqualTo(SaveSchema.CurrentVersion));
            Assert.That(upgraded.bestScore, Is.EqualTo(0));
            Assert.That(upgraded.selectedThemeId, Is.EqualTo(SaveSchema.DefaultThemeId));
            Assert.That(upgraded.unlockedThemeIds, Does.Contain(SaveSchema.DefaultThemeId));
            Assert.That(upgraded.musicVolume, Is.EqualTo(1f));
            Assert.That(upgraded.sfxVolume, Is.EqualTo(1f));
            Assert.That(upgraded.dailyChallenge, Is.Not.Null);
        }

        [Test]
        public void SynchronizeThemeUnlocks_UnlocksCandyPopAtBestScoreTwenty()
        {
            string tempPath = Path.Combine(Path.GetTempPath(), "voltline-theme-unlock-editmode-test.json");
            SaveStorage.SetOverridePathForTests(tempPath);
            SaveStorage.DeleteProfile();
            SaveService.ResetInstanceForTests();

            try
            {
                ThemeCatalog themeCatalog = AssetDatabase.LoadAssetAtPath<ThemeCatalog>(ProjectConfigAssetPaths.ThemeCatalog);
                SaveService saveService = SaveService.EnsureExists();
                saveService.SynchronizeThemeUnlocks(themeCatalog);

                Assert.That(saveService.IsThemeUnlocked("theme.neon-night"), Is.True);
                Assert.That(saveService.IsThemeUnlocked("theme.candy-pop"), Is.False);

                saveService.RecordRunScore(20);
                saveService.SynchronizeThemeUnlocks(themeCatalog);
                Assert.That(saveService.IsThemeUnlocked("theme.candy-pop"), Is.True);

                saveService.SetSelectedThemeId("theme.candy-pop");
                Assert.That(saveService.SelectedThemeId, Is.EqualTo("theme.candy-pop"));
            }
            finally
            {
                SaveService.ResetInstanceForTests();
                SaveStorage.DeleteProfile();
                SaveStorage.ClearOverridePathForTests();
            }
        }
    }
}
#endif
