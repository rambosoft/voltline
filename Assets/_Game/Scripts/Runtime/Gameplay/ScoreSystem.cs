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

        public void Initialize(GameBalanceConfig balanceConfig)
        {
            gameBalance = balanceConfig;
            ResetRun();
        }

        public void ResetRun()
        {
            CurrentScore = 0;
            nextMilestoneIndex = 0;
            ScoreChanged?.Invoke(CurrentScore);
        }

        public void RegisterClearedBeat()
        {
            CurrentScore++;
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