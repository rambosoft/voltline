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
            Assert.That(upgraded.hasSeenFirstLaunchHint, Is.False);
            Assert.That(upgraded.dailyChallenge, Is.Not.Null);
        }

        [Test]
        public void SynchronizeThemeUnlocks_KeepsSingleLiveWireCityThemeUnlocked()
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

                Assert.That(saveService.IsThemeUnlocked("theme.live-wire-city"), Is.True);
                Assert.That(saveService.SelectedThemeId, Is.EqualTo("theme.live-wire-city"));

                saveService.RecordRunScore(50);
                saveService.SetHasSeenFirstLaunchHint(true);
                saveService.SynchronizeThemeUnlocks(themeCatalog);
                Assert.That(saveService.CurrentProfile.unlockedThemeIds.Count, Is.EqualTo(1));
                Assert.That(saveService.CurrentProfile.unlockedThemeIds[0], Is.EqualTo("theme.live-wire-city"));
                Assert.That(saveService.HasSeenFirstLaunchHint, Is.True);
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
