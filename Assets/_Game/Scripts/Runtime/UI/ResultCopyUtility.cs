using System.Collections.Generic;
using Voltline.Data;

namespace Voltline.UI
{
    public readonly struct ResultPresentationCopy
    {
        public ResultPresentationCopy(string title, string message)
        {
            Title = title;
            Message = message;
        }

        public string Title { get; }
        public string Message { get; }
    }

    public static class ResultCopyUtility
    {
        private static readonly string[] FallbackTitles = { "GRID FAILURE", "SIGNAL LOST", "POWER CUT" };
        private static readonly string[] FallbackNewBestTitles = { "GRID RECORD" };
        private static readonly string[] FallbackZeroScore = { "The line died before the city woke." };
        private static readonly string[] FallbackLowScore = { "The district dimmed out." };
        private static readonly string[] FallbackGeneric = { "Keep the grid alive." };
        private static readonly string[] FallbackNewBestMessages = { "Best transfer yet." };

        public static ResultPresentationCopy BuildCopy(int score, bool isNewBest, ObstacleFamily? failureFamily, IReadOnlyList<int> milestoneThresholds, WorldProgressionConfig worldProgressionConfig, ProductionCopyConfig copyConfig)
        {
            if (isNewBest)
            {
                return new ResultPresentationCopy(
                    SelectMessage(copyConfig?.NewBestResultTitles, FallbackNewBestTitles, score),
                    SelectMessage(copyConfig?.NewBestMessages, FallbackNewBestMessages, score));
            }

            if (failureFamily.HasValue && copyConfig != null && copyConfig.TryGetFailureResultCopy(failureFamily.Value, score, out string failureTitle, out string failureMessage))
            {
                return new ResultPresentationCopy(failureTitle, failureMessage);
            }

            string title = SelectMessage(copyConfig?.ResultTitles, FallbackTitles, score);
            if (score <= 0)
            {
                return new ResultPresentationCopy(title, SelectMessage(copyConfig?.ZeroScoreMessages, FallbackZeroScore, score));
            }

            if (TryGetNextMilestone(score, milestoneThresholds, out int nextMilestone) && nextMilestone - score <= 2)
            {
                return new ResultPresentationCopy(title, copyConfig != null ? copyConfig.FormatAlmostMilestone(nextMilestone) : $"Almost {nextMilestone}");
            }

            if (IsExactMilestone(score, milestoneThresholds) && copyConfig != null && copyConfig.TryGetLatestMilestoneMessage(score, out string milestoneMessage))
            {
                return new ResultPresentationCopy(title, milestoneMessage);
            }

            if (worldProgressionConfig != null && copyConfig != null && score >= 10)
            {
                return new ResultPresentationCopy(title, copyConfig.FormatDistrictsRestored(worldProgressionConfig.GetRestoredDistrictCount(score)));
            }

            if (score < 5)
            {
                return new ResultPresentationCopy(title, SelectMessage(copyConfig?.LowScoreMessages, FallbackLowScore, score));
            }

            return new ResultPresentationCopy(title, SelectMessage(copyConfig?.GenericMessages, FallbackGeneric, score));
        }

        private static string SelectMessage(IReadOnlyList<string> configuredMessages, IReadOnlyList<string> fallbackMessages, int seed)
        {
            IReadOnlyList<string> source = configuredMessages != null && configuredMessages.Count > 0 ? configuredMessages : fallbackMessages;
            if (source == null || source.Count == 0)
            {
                return string.Empty;
            }

            int index = seed;
            if (index < 0)
            {
                index = -index;
            }

            return source[index % source.Count];
        }

        private static bool TryGetNextMilestone(int score, IReadOnlyList<int> milestoneThresholds, out int nextMilestone)
        {
            nextMilestone = 0;
            if (milestoneThresholds == null)
            {
                return false;
            }

            for (int i = 0; i < milestoneThresholds.Count; i++)
            {
                if (milestoneThresholds[i] > score)
                {
                    nextMilestone = milestoneThresholds[i];
                    return true;
                }
            }

            return false;
        }

        private static bool IsExactMilestone(int score, IReadOnlyList<int> milestoneThresholds)
        {
            if (milestoneThresholds == null)
            {
                return false;
            }

            for (int i = 0; i < milestoneThresholds.Count; i++)
            {
                if (milestoneThresholds[i] == score)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
