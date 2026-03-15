using System.Collections;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Voltline.Save;

namespace Voltline.Tests.PlayMode
{
    public sealed class SavePersistencePlayModeTests
    {
        [UnityTest]
        public IEnumerator SaveService_BestScoreAndSettingsPersistAcrossRecreation()
        {
            string tempPath = Path.Combine(Application.temporaryCachePath, "voltline-save-persistence-test.json");
            SaveStorage.SetOverridePathForTests(tempPath);
            SaveStorage.DeleteProfile();
            SaveService.ResetInstanceForTests();
            yield return null;

            SaveService firstService = SaveService.EnsureExists();
            firstService.RecordRunScore(20);
            firstService.SetMusicVolume(0.35f);
            firstService.SetSfxVolume(0.65f);
            firstService.SetVibrationEnabled(false);
            firstService.CurrentProfile.unlockedThemeIds.Add("theme.candy-pop");
            firstService.SetSelectedThemeId("theme.candy-pop");
            yield return null;

            SaveService.ResetInstanceForTests();
            yield return null;

            SaveService secondService = SaveService.EnsureExists();
            Assert.That(secondService.BestScore, Is.EqualTo(20));
            Assert.That(secondService.MusicVolume, Is.EqualTo(0.35f).Within(0.001f));
            Assert.That(secondService.SfxVolume, Is.EqualTo(0.65f).Within(0.001f));
            Assert.That(secondService.VibrationEnabled, Is.False);
            Assert.That(secondService.SelectedThemeId, Is.EqualTo("theme.candy-pop"));
            Assert.That(secondService.CurrentProfile.unlockedThemeIds, Does.Contain("theme.candy-pop"));

            SaveService.ResetInstanceForTests();
            SaveStorage.DeleteProfile();
            SaveStorage.ClearOverridePathForTests();
            yield return null;
        }
    }
}
