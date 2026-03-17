using System;
using System.Collections.Generic;
using UnityEngine;
using Voltline.Data;

namespace Voltline.Gameplay
{
    public sealed class ScoreSystem : MonoBehaviour
    {
        private GameBalanceConfig gameBalance;
        private int nextMilestoneIndex;

        public event Action<int> ScoreChanged;
        public event Action<int> MilestoneReached;

        public int CurrentScore { get; private set; }
        public IReadOnlyList<int> MilestoneThresholds => gameBalance != null && gameBalance.MilestoneThresholds != null
            ? gameBalance.MilestoneThresholds
            : Array.Empty<int>();

        public void Initialize(GameBalanceConfig balanceConfig)
        {
            gameBalance = balanceConfig;
            ResetRun();
        }

        public void ResetRun(int startingScore = 0)
        {
            CurrentScore = Mathf.Max(0, startingScore);
            nextMilestoneIndex = 0;

            IReadOnlyList<int> milestones = gameBalance != null ? gameBalance.MilestoneThresholds : null;
            while (milestones != null && nextMilestoneIndex < milestones.Count && CurrentScore >= milestones[nextMilestoneIndex])
            {
                nextMilestoneIndex++;
            }

            ScoreChanged?.Invoke(CurrentScore);
        }

        public void RegisterClearedBeat()
        {
            CurrentScore++;
            ScoreChanged?.Invoke(CurrentScore);
            ProcessMilestones();
        }

        public void SetDebugScore(int score)
        {
            int clampedScore = Mathf.Max(0, score);
            if (clampedScore == CurrentScore)
            {
                return;
            }

            bool isIncrease = clampedScore > CurrentScore;
            CurrentScore = clampedScore;

            if (!isIncrease)
            {
                nextMilestoneIndex = 0;
                IReadOnlyList<int> milestones = gameBalance != null ? gameBalance.MilestoneThresholds : null;
                while (milestones != null && nextMilestoneIndex < milestones.Count && CurrentScore >= milestones[nextMilestoneIndex])
                {
                    nextMilestoneIndex++;
                }

                ScoreChanged?.Invoke(CurrentScore);
                return;
            }

            ScoreChanged?.Invoke(CurrentScore);
            ProcessMilestones();
        }

        private void ProcessMilestones()
        {
            IReadOnlyList<int> milestones = gameBalance != null ? gameBalance.MilestoneThresholds : null;
            while (milestones != null && nextMilestoneIndex < milestones.Count && CurrentScore >= milestones[nextMilestoneIndex])
            {
                MilestoneReached?.Invoke(milestones[nextMilestoneIndex]);
                nextMilestoneIndex++;
            }
        }
    }
}
