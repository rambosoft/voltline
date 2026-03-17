using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Voltline.Data
{
    [Serializable]
    public sealed class MilestoneCopyEntry
    {
        [SerializeField] private int scoreThreshold;
        [SerializeField] private string message = string.Empty;

        public int ScoreThreshold => scoreThreshold;
        public string Message => message;
    }

    [Serializable]
    public sealed class FailureResultCopyDefinition
    {
        [SerializeField] private ObstacleFamily family = ObstacleFamily.SharpUtilityHazards;
        [SerializeField] private List<string> titles = new();
        [SerializeField] private List<string> messages = new();

        public ObstacleFamily Family => family;
        public IReadOnlyList<string> Titles => titles;
        public IReadOnlyList<string> Messages => messages;
    }

    [CreateAssetMenu(fileName = "CFG_ProductionCopy_LiveWireCity", menuName = "Voltline/Config/Production Copy")]
    public sealed class ProductionCopyConfig : ScriptableObject
    {
        [SerializeField] private string menuHintPrimary = "Tap to flip sides";
        [SerializeField] private string menuHintSecondary = "Keep the charge alive";
        [SerializeField] private string playButtonLabel = "Play";
        [SerializeField] private string dailyButtonLabel = "Daily";
        [SerializeField] private string themesButtonLabel = "Themes";
        [SerializeField] private string gridStatusButtonLabel = "Grid";
        [SerializeField] private string bestButtonLabel = "Best";
        [SerializeField] private string settingsButtonLabel = "Settings";
        [SerializeField] private string retryButtonLabel = "Retry";
        [SerializeField] private string shareButtonLabel = "Share";
        [SerializeField] private string settingsTitle = "Settings";
        [SerializeField] private string pauseTitle = "PAUSED";
        [SerializeField] private string resumeButtonLabel = "Resume";
        [SerializeField] private string restartButtonLabel = "Restart";
        [SerializeField] private string homeButtonLabel = "Home";
        [SerializeField] private string closeButtonLabel = "Close";
        [SerializeField] private string musicLabel = "Music";
        [SerializeField] private string sfxLabel = "SFX";
        [SerializeField] private string vibrationLabel = "Haptics";
        [SerializeField] private string themeLabel = "Themes";
        [SerializeField] private string themeSelectionHint = "More themes come online in a later rollout.";
        [SerializeField] private string selectedThemeStatusLabel = "Selected";
        [SerializeField] private string unlockedThemeStatusLabel = "Unlocked";
        [SerializeField] private string lockedThemeRequirementFormat = "Best {0}";
        [SerializeField] private string onLabel = "On";
        [SerializeField] private string offLabel = "Off";
        [SerializeField] private string bestScoreFormat = "Best {0}";
        [SerializeField] private string newBestScoreFormat = "New Best {0}";
        [SerializeField] private string almostMilestoneFormat = "Almost {0}";
        [SerializeField] private string comingSoonStatusLabel = "SOON";
        [SerializeField] private string bestPanelTitle = "GRID RECORD";
        [SerializeField] private string gridStatusTitle = "GRID STATUS";
        [SerializeField] private string gridStatusSummaryFormat = "{0} | {1}";
        [SerializeField] private string gridStatusCurrentLabel = "CURRENT";
        [SerializeField] private string gridStatusOnlineLabel = "ONLINE";
        [SerializeField] private string gridStatusLockedRequirementFormat = "Reach {0}";
        [SerializeField] private string gridStatusScoreBandFormat = "{0}-{1}";
        [SerializeField] private string gridStatusScoreBandOpenEndedFormat = "{0}+";
        [SerializeField] private string gridStatusOnlineDistrictsSingularFormat = "{0} district online";
        [SerializeField] private string gridStatusOnlineDistrictsPluralFormat = "{0} districts online";
        [SerializeField] private string shareTitle = "SHARE STATUS";
        [SerializeField] private string shareCopyButtonLabel = "Copy";
        [SerializeField] private string shareCopiedStatusLabel = "Copied";
        [SerializeField] private string shareSummaryFormat = "Score {0} | {1}";
        [SerializeField] private string shareFlavorSingularFormat = "{0} district online in {1}.";
        [SerializeField] private string shareFlavorPluralFormat = "{0} districts online in {1}.";
        [SerializeField] private string tutorialTitle = "KEEP THE GRID ALIVE";
        [SerializeField] private string tutorialPrimaryLine = "Tap to flip to the safer side of the conduit.";
        [SerializeField] private string tutorialSecondaryLine = "Stay ahead of the hazards and keep the transfer alive.";
        [SerializeField] private string tutorialActionLabel = "Start Run";
        [SerializeField] private string districtsRestoredSingularFormat = "You restored {0} district.";
        [SerializeField] private string districtsRestoredPluralFormat = "You restored {0} districts.";
        [SerializeField] private List<string> resultTitles = new() { "GRID FAILURE", "SIGNAL LOST", "DISTRICT DOWN", "POWER CUT", "TRANSFER FAILED" };
        [SerializeField] private List<string> newBestResultTitles = new() { "GRID RECORD", "BEST TRANSFER" };
        [SerializeField] private List<string> zeroScoreMessages = new() { "The line died before the city woke." };
        [SerializeField] private List<string> lowScoreMessages = new() { "The district dimmed out.", "Retry the transfer." };
        [SerializeField] private List<string> genericMessages = new() { "Keep the grid alive.", "The city needs another run.", "The transfer almost held." };
        [SerializeField] private List<string> newBestMessages = new() { "Record charge across the grid.", "Best transfer in the city yet." };
        [SerializeField] private List<MilestoneCopyEntry> milestoneMessages = new();
        [SerializeField] private List<FailureResultCopyDefinition> failureResultCopies = new();

        public string MenuHintPrimary => menuHintPrimary;
        public string MenuHintSecondary => menuHintSecondary;
        public string PlayButtonLabel => playButtonLabel;
        public string DailyButtonLabel => dailyButtonLabel;
        public string ThemesButtonLabel => themesButtonLabel;
        public string GridStatusButtonLabel => gridStatusButtonLabel;
        public string BestButtonLabel => bestButtonLabel;
        public string SettingsButtonLabel => settingsButtonLabel;
        public string RetryButtonLabel => retryButtonLabel;
        public string ShareButtonLabel => shareButtonLabel;
        public string SettingsTitle => settingsTitle;
        public string PauseTitle => pauseTitle;
        public string ResumeButtonLabel => resumeButtonLabel;
        public string RestartButtonLabel => restartButtonLabel;
        public string HomeButtonLabel => homeButtonLabel;
        public string CloseButtonLabel => closeButtonLabel;
        public string MusicLabel => musicLabel;
        public string SfxLabel => sfxLabel;
        public string VibrationLabel => vibrationLabel;
        public string ThemeLabel => themeLabel;
        public string ThemeSelectionHint => themeSelectionHint;
        public string SelectedThemeStatusLabel => selectedThemeStatusLabel;
        public string UnlockedThemeStatusLabel => unlockedThemeStatusLabel;
        public string LockedThemeRequirementFormat => lockedThemeRequirementFormat;
        public string OnLabel => onLabel;
        public string OffLabel => offLabel;
        public string BestScoreFormat => bestScoreFormat;
        public string NewBestScoreFormat => newBestScoreFormat;
        public string AlmostMilestoneFormat => almostMilestoneFormat;
        public string ComingSoonStatusLabel => comingSoonStatusLabel;
        public string BestPanelTitle => bestPanelTitle;
        public string GridStatusTitle => gridStatusTitle;
        public string GridStatusCurrentLabel => gridStatusCurrentLabel;
        public string GridStatusOnlineLabel => gridStatusOnlineLabel;
        public string ShareTitle => shareTitle;
        public string ShareCopyButtonLabel => shareCopyButtonLabel;
        public string ShareCopiedStatusLabel => shareCopiedStatusLabel;
        public string TutorialTitle => tutorialTitle;
        public string TutorialPrimaryLine => tutorialPrimaryLine;
        public string TutorialSecondaryLine => tutorialSecondaryLine;
        public string TutorialActionLabel => tutorialActionLabel;
        public IReadOnlyList<string> ResultTitles => resultTitles;
        public IReadOnlyList<string> NewBestResultTitles => newBestResultTitles;
        public IReadOnlyList<string> ZeroScoreMessages => zeroScoreMessages;
        public IReadOnlyList<string> LowScoreMessages => lowScoreMessages;
        public IReadOnlyList<string> GenericMessages => genericMessages;
        public IReadOnlyList<string> NewBestMessages => newBestMessages;
        public IReadOnlyList<MilestoneCopyEntry> MilestoneMessages => milestoneMessages;
        public IReadOnlyList<FailureResultCopyDefinition> FailureResultCopies => failureResultCopies;

        public string FormatBestScore(int score)
        {
            return string.Format(BestScoreFormat, score);
        }

        public string FormatNewBestScore(int score)
        {
            return string.Format(NewBestScoreFormat, score);
        }

        public string FormatAlmostMilestone(int score)
        {
            return string.Format(AlmostMilestoneFormat, score);
        }

        public string FormatLockedThemeRequirement(int bestScore)
        {
            return string.Format(LockedThemeRequirementFormat, bestScore);
        }

        public string FormatDistrictsRestored(int districts)
        {
            string format = districts == 1 ? districtsRestoredSingularFormat : districtsRestoredPluralFormat;
            return string.Format(format, Mathf.Max(0, districts));
        }

        public string FormatGridStatusSummary(int bestScore, int districtsOnline)
        {
            return string.Format(gridStatusSummaryFormat, FormatBestScore(bestScore), FormatGridStatusOnlineDistricts(districtsOnline));
        }

        public string FormatGridStatusOnlineDistricts(int districtsOnline)
        {
            string format = districtsOnline == 1 ? gridStatusOnlineDistrictsSingularFormat : gridStatusOnlineDistrictsPluralFormat;
            return string.Format(format, Mathf.Max(0, districtsOnline));
        }

        public string FormatGridStatusLockedRequirement(int requiredScore)
        {
            return string.Format(gridStatusLockedRequirementFormat, Mathf.Max(0, requiredScore));
        }

        public string FormatGridStatusScoreBand(int minScore, int maxScoreInclusive)
        {
            return maxScoreInclusive < 0
                ? string.Format(gridStatusScoreBandOpenEndedFormat, Mathf.Max(0, minScore))
                : string.Format(gridStatusScoreBandFormat, Mathf.Max(0, minScore), Mathf.Max(minScore, maxScoreInclusive));
        }

        public string FormatShareSummary(int score, string districtName)
        {
            return string.Format(shareSummaryFormat, Mathf.Max(0, score), districtName ?? string.Empty);
        }

        public string FormatShareFlavor(int districtsOnline, string districtName)
        {
            string format = districtsOnline == 1 ? shareFlavorSingularFormat : shareFlavorPluralFormat;
            return string.Format(format, Mathf.Max(0, districtsOnline), districtName ?? string.Empty);
        }

        public bool TryGetLatestMilestoneMessage(int score, out string message)
        {
            message = string.Empty;
            int bestThreshold = int.MinValue;
            if (milestoneMessages == null)
            {
                return false;
            }

            for (int i = 0; i < milestoneMessages.Count; i++)
            {
                MilestoneCopyEntry entry = milestoneMessages[i];
                if (entry == null || string.IsNullOrWhiteSpace(entry.Message))
                {
                    continue;
                }

                if (entry.ScoreThreshold <= score && entry.ScoreThreshold > bestThreshold)
                {
                    bestThreshold = entry.ScoreThreshold;
                    message = entry.Message;
                }
            }

            return bestThreshold != int.MinValue;
        }

        public bool TryGetFailureResultCopy(ObstacleFamily family, int seed, out string title, out string message)
        {
            title = string.Empty;
            message = string.Empty;
            if (failureResultCopies == null)
            {
                return false;
            }

            for (int i = 0; i < failureResultCopies.Count; i++)
            {
                FailureResultCopyDefinition entry = failureResultCopies[i];
                if (entry == null || entry.Family != family)
                {
                    continue;
                }

                title = SelectSeeded(entry.Titles, seed);
                message = SelectSeeded(entry.Messages, seed + 1);
                return !string.IsNullOrWhiteSpace(title) && !string.IsNullOrWhiteSpace(message);
            }

            return false;
        }

        public string GetMenuEntryLabel(BrandingMenuEntryId entryId)
        {
            return entryId switch
            {
                BrandingMenuEntryId.Daily => dailyButtonLabel,
                BrandingMenuEntryId.Themes => themesButtonLabel,
                BrandingMenuEntryId.Best => bestButtonLabel,
                BrandingMenuEntryId.Settings => settingsButtonLabel,
                _ => string.Empty,
            };
        }

        private static string SelectSeeded(IReadOnlyList<string> values, int seed)
        {
            if (values == null || values.Count == 0)
            {
                return string.Empty;
            }

            int index = seed;
            if (index < 0)
            {
                index = -index;
            }

            return values[index % values.Count];
        }
    }
}
