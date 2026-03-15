using System;
using System.Collections.Generic;

namespace Voltline.Save
{
    [Serializable]
    public sealed class DailyChallengeSaveData
    {
        public string lastPlayedDate;
        public string lastSubmittedSeedId;
        public int bestDailyScore;
    }

    [Serializable]
    public sealed class PlayerProfileSaveData
    {
        public int version;
        public int bestScore;
        public string selectedThemeId;
        public List<string> unlockedThemeIds = new();
        public float musicVolume = 1f;
        public float sfxVolume = 1f;
        public bool vibrationEnabled = true;
        public bool hasSeenFirstLaunchHint;
        public DailyChallengeSaveData dailyChallenge = new();
    }
}