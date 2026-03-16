using System.Collections.Generic;
using UnityEngine;

namespace Voltline.Data
{
    public enum PresentationRolloutSliceId
    {
        PlayerRefresh = 0,
        ObstacleRefresh = 1,
        BackgroundRefresh = 2,
        StaticThemeRefresh = 3,
        DynamicThemeTransitions = 4,
        VfxRefresh = 5,
        AudioRefresh = 6,
    }

    [System.Serializable]
    public sealed class PresentationRolloutSliceDefinition
    {
        [SerializeField] private PresentationRolloutSliceId sliceId = PresentationRolloutSliceId.PlayerRefresh;
        [SerializeField] private bool requiresCollisionReview = true;
        [SerializeField] private bool requiresAutomatedChecks = true;
        [SerializeField] private bool requiresManualReadabilityReview = true;
        [SerializeField] private bool requiresDevicePerformanceCheck = true;
        [SerializeField] private bool requiresNoConsoleNoiseCheck = true;
        [SerializeField] private bool blocksNextSliceUntilApproved = true;

        public PresentationRolloutSliceId SliceId => sliceId;
        public bool RequiresCollisionReview => requiresCollisionReview;
        public bool RequiresAutomatedChecks => requiresAutomatedChecks;
        public bool RequiresManualReadabilityReview => requiresManualReadabilityReview;
        public bool RequiresDevicePerformanceCheck => requiresDevicePerformanceCheck;
        public bool RequiresNoConsoleNoiseCheck => requiresNoConsoleNoiseCheck;
        public bool BlocksNextSliceUntilApproved => blocksNextSliceUntilApproved;
    }

    [CreateAssetMenu(fileName = "CFG_PresentationRolloutPlan_Main", menuName = "Voltline/Config/Presentation Rollout Plan")]
    public sealed class PresentationRolloutPlanConfig : ScriptableObject
    {
        [SerializeField] private bool requireConfigValidation = true;
        [SerializeField] private bool requireReleaseAudit = true;
        [SerializeField] private bool requirePresentationReadinessAudit = true;
        [SerializeField] private bool requireEditModeSuite = true;
        [SerializeField] private bool requirePlayModeSuite = true;
        [SerializeField] private bool requireManualCollisionReview = true;
        [SerializeField] private bool requireManualReadabilityReview = true;
        [SerializeField] private bool requireDevicePerformanceCheck = true;
        [SerializeField] private bool requireSaveAndThemePersistenceCheck = true;
        [SerializeField] private bool requireBuildSizeReview = true;
        [SerializeField] private bool requireNoConsoleNoise = true;
        [SerializeField] private List<PresentationRolloutSliceDefinition> slices = new();

        public bool RequireConfigValidation => requireConfigValidation;
        public bool RequireReleaseAudit => requireReleaseAudit;
        public bool RequirePresentationReadinessAudit => requirePresentationReadinessAudit;
        public bool RequireEditModeSuite => requireEditModeSuite;
        public bool RequirePlayModeSuite => requirePlayModeSuite;
        public bool RequireManualCollisionReview => requireManualCollisionReview;
        public bool RequireManualReadabilityReview => requireManualReadabilityReview;
        public bool RequireDevicePerformanceCheck => requireDevicePerformanceCheck;
        public bool RequireSaveAndThemePersistenceCheck => requireSaveAndThemePersistenceCheck;
        public bool RequireBuildSizeReview => requireBuildSizeReview;
        public bool RequireNoConsoleNoise => requireNoConsoleNoise;
        public IReadOnlyList<PresentationRolloutSliceDefinition> Slices => slices;
    }
}
