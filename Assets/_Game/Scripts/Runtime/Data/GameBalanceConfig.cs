using System.Collections.Generic;
using UnityEngine;

namespace Voltline.Data
{
    [CreateAssetMenu(fileName = "CFG_GameBalance_Default", menuName = "Voltline/Config/Game Balance")]
    public sealed class GameBalanceConfig : ScriptableObject
    {
        [SerializeField] private float baseSpeed = 6f;
        [SerializeField] private float earlySpeedRampPerPoint = 0.12f;
        [SerializeField] private float flipDurationSeconds = 0.09f;
        [SerializeField] private float sideOffset = 0.34f;
        [SerializeField] private float nearMissThreshold = 0.18f;
        [SerializeField] private float safeStartWindowSeconds = 1.2f;
        [SerializeField] private float minimumReadableTelegraphSeconds = 0.75f;
        [SerializeField] private float resultPanelRevealDelaySeconds = 0.6f;
        [SerializeField] private float retryAvailabilityDelaySeconds = 1f;
        [SerializeField] private List<int> milestoneThresholds = new();

        public float BaseSpeed => baseSpeed;
        public float EarlySpeedRampPerPoint => earlySpeedRampPerPoint;
        public float FlipDurationSeconds => flipDurationSeconds;
        public float SideOffset => sideOffset;
        public float NearMissThreshold => nearMissThreshold;
        public float SafeStartWindowSeconds => safeStartWindowSeconds;
        public float MinimumReadableTelegraphSeconds => minimumReadableTelegraphSeconds;
        public float ResultPanelRevealDelaySeconds => resultPanelRevealDelaySeconds;
        public float RetryAvailabilityDelaySeconds => retryAvailabilityDelaySeconds;
        public IReadOnlyList<int> MilestoneThresholds => milestoneThresholds;
    }
}