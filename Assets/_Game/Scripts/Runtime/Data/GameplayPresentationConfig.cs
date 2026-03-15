using UnityEngine;

namespace Voltline.Data
{
    [CreateAssetMenu(fileName = "CFG_GameplayPresentation_Default", menuName = "Voltline/Config/Gameplay Presentation")]
    public sealed class GameplayPresentationConfig : ScriptableObject
    {
        [Header("Track")]
        [SerializeField] private float trackCameraSize = 7.25f;
        [SerializeField] private float trackLineWidth = 0.22f;
        [SerializeField] private float trackCurvePrimaryAmplitude = 0.34f;
        [SerializeField] private float trackCurvePrimaryWavelength = 6.6f;
        [SerializeField] private float trackCurveSecondaryAmplitude = 0.12f;
        [SerializeField] private float trackCurveSecondaryWavelength = 3.4f;

        [Header("Player")]
        [SerializeField] private float playerVisualScale = 0.68f;
        [SerializeField] private float playerLineClearance = 0.1f;
        [SerializeField] private float playerCollisionHalfWidth = 0.28f;
        [SerializeField] private float playerCollisionHalfHeight = 0.32f;

        public float TrackCameraSize => trackCameraSize;
        public float TrackLineWidth => trackLineWidth;
        public float TrackCurvePrimaryAmplitude => trackCurvePrimaryAmplitude;
        public float TrackCurvePrimaryWavelength => trackCurvePrimaryWavelength;
        public float TrackCurveSecondaryAmplitude => trackCurveSecondaryAmplitude;
        public float TrackCurveSecondaryWavelength => trackCurveSecondaryWavelength;
        public float PlayerVisualScale => playerVisualScale;
        public float PlayerLineClearance => playerLineClearance;
        public float PlayerCollisionHalfWidth => playerCollisionHalfWidth;
        public float PlayerCollisionHalfHeight => playerCollisionHalfHeight;
    }
}
