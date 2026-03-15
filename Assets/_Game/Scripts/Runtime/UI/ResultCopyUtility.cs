using System.Collections.Generic;

namespace Voltline.UI
{
    public static class ResultCopyUtility
    {
        public static string BuildMessage(int score, bool isNewBest, IReadOnlyList<int> milestoneThresholds)
        {
            if (isNewBest)
            {
                return "New Best";
            }

            if (score <= 0)
            {
                return "One more run";
            }

            if (TryGetNextMilestone(score, milestoneThresholds, out int nextMilestone) && nextMilestone - score <= 2)
            {
                return $"Almost {nextMilestone}";
            }

            if (score < 5)
            {
                return "So close";
            }

            return $"You survived {score}";
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
    }
}
