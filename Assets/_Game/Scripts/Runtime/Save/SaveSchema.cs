using System.Collections.Generic;

namespace Voltline.Save
{
    public static class SaveSchema
    {
        public const int CurrentVersion = 1;

        public static PlayerProfileSaveData CreateDefaultProfile(string defaultThemeId)
        {
            return new PlayerProfileSaveData
            {
                version = CurrentVersion,
                bestScore = 0,
                selectedThemeId = defaultThemeId,
                unlockedThemeIds = string.IsNullOrWhiteSpace(defaultThemeId)
                    ? new List<string>()
                    : new List<string> { defaultThemeId },
                musicVolume = 1f,
                sfxVolume = 1f,
                vibrationEnabled = true,
                hasSeenFirstLaunchHint = false,
                dailyChallenge = new DailyChallengeSaveData
                {
                    lastPlayedDate = null,
                    lastSubmittedSeedId = null,
                    bestDailyScore = 0,
                },
            };
        }
    }
}