using System.Collections.Generic;

namespace Voltline.Save
{
    public static class SaveSchema
    {
        public const int CurrentVersion = 1;
        public const string DefaultThemeId = "theme.live-wire-city";

        public static PlayerProfileSaveData CreateDefaultProfile(string defaultThemeId)
        {
            string resolvedThemeId = string.IsNullOrWhiteSpace(defaultThemeId) ? DefaultThemeId : defaultThemeId;
            return new PlayerProfileSaveData
            {
                version = CurrentVersion,
                bestScore = 0,
                selectedThemeId = resolvedThemeId,
                unlockedThemeIds = new List<string> { resolvedThemeId },
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

        public static PlayerProfileSaveData UpgradeToCurrent(PlayerProfileSaveData profile, string defaultThemeId)
        {
            int originalVersion = profile != null ? profile.version : 0;
            PlayerProfileSaveData upgraded = profile ?? CreateDefaultProfile(defaultThemeId);
            string resolvedThemeId = string.IsNullOrWhiteSpace(defaultThemeId) ? DefaultThemeId : defaultThemeId;

            upgraded.version = CurrentVersion;
            upgraded.bestScore = System.Math.Max(0, upgraded.bestScore);
            upgraded.selectedThemeId = string.IsNullOrWhiteSpace(upgraded.selectedThemeId) ? resolvedThemeId : upgraded.selectedThemeId;
            upgraded.unlockedThemeIds ??= new List<string>();
            if (!upgraded.unlockedThemeIds.Contains(upgraded.selectedThemeId))
            {
                upgraded.unlockedThemeIds.Add(upgraded.selectedThemeId);
            }

            upgraded.musicVolume = originalVersion < 1 && upgraded.musicVolume <= 0f ? 1f : UnityEngine.Mathf.Clamp01(upgraded.musicVolume);
            upgraded.sfxVolume = originalVersion < 1 && upgraded.sfxVolume <= 0f ? 1f : UnityEngine.Mathf.Clamp01(upgraded.sfxVolume);
            upgraded.dailyChallenge ??= new DailyChallengeSaveData();
            upgraded.dailyChallenge.bestDailyScore = System.Math.Max(0, upgraded.dailyChallenge.bestDailyScore);
            return upgraded;
        }
    }
}
