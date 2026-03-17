using UnityEngine;
using Voltline.Data;

namespace Voltline.Gameplay
{
    public sealed class TrackManager : MonoBehaviour
    {
        private const float LineZ = 0f;
        private const int LinePointCount = 64;

        private GameBalanceConfig gameBalance;
        private GameplayPresentationConfig presentationConfig;
        private ThemeConfig theme;
        private WorldDistrictStateDefinition currentDistrictState;
        private Camera gameplayCamera;
        private LineRenderer lineRenderer;
        private Material lineMaterial;
        private Transform worldRoot;
        private Transform playerRoot;
        private Transform hazardsRoot;
        private float pulseTime;
        private float pulseStrength;
        private Color displayedLineCoreColor;
        private Color displayedLineGlowColor;
        private float displayedLineWidthMultiplier = 1f;
        private float displayedPulseSpeedMultiplier = 1f;
        private Color transitionFromLineCoreColor;
        private Color transitionFromLineGlowColor;
        private Color transitionToLineCoreColor;
        private Color transitionToLineGlowColor;
        private float transitionFromLineWidthMultiplier = 1f;
        private float transitionToLineWidthMultiplier = 1f;
        private float transitionFromPulseSpeedMultiplier = 1f;
        private float transitionToPulseSpeedMultiplier = 1f;
        private float themeTransitionDuration;
        private float themeTransitionElapsed;

        public float TravelDistance { get; private set; }
        public float PlayerAnchorY => -gameplayCamera.orthographicSize * 0.68f;
        public float TrackLineWidth => presentationConfig != null ? presentationConfig.TrackLineWidth : 0f;
        public float VisibleDistance => gameplayCamera.orthographicSize + 4f;
        public float LowerDespawnY => -gameplayCamera.orthographicSize - 2f;
        public Transform PlayerRoot => playerRoot;
        public Transform HazardsRoot => hazardsRoot;

        public void Initialize(
            GameBalanceConfig balanceConfig,
            GameplayPresentationConfig gameplayPresentation,
            ThemeConfig activeTheme,
            Camera targetCamera)
        {
            gameBalance = balanceConfig;
            presentationConfig = gameplayPresentation;
            theme = activeTheme;
            gameplayCamera = targetCamera;
            gameplayCamera.orthographic = true;
            gameplayCamera.orthographicSize = presentationConfig.TrackCameraSize;

            EnsureRuntimeHierarchy();
            EnsureLineRenderer();
            ApplyTheme(activeTheme, 0f);
            ResetRun();
        }

        public void ApplyTheme(ThemeConfig activeTheme, float transitionDurationSeconds)
        {
            theme = activeTheme;
            ApplyLineTargets(ResolveLineCoreColor(), ResolveLineGlowColor(), ResolveLineWidthMultiplier(), ResolvePulseSpeedMultiplier(), transitionDurationSeconds);
        }

        public void ApplyWorldDistrict(WorldDistrictStateDefinition districtState, float transitionDurationSeconds)
        {
            currentDistrictState = districtState;
            ApplyLineTargets(ResolveLineCoreColor(), ResolveLineGlowColor(), ResolveLineWidthMultiplier(), ResolvePulseSpeedMultiplier(), transitionDurationSeconds);
        }

        private void Update()
        {
            if (lineRenderer == null)
            {
                return;
            }

            pulseTime += Time.deltaTime * displayedPulseSpeedMultiplier;
            pulseStrength = Mathf.Max(0f, pulseStrength - (Time.deltaTime * 2.4f));
            UpdateThemeTransition(Time.deltaTime);

            float idlePulse = 0.5f + (Mathf.Sin(pulseTime * 2.25f) * 0.5f);
            float width = presentationConfig.TrackLineWidth * displayedLineWidthMultiplier * (1f + (idlePulse * 0.08f) + (pulseStrength * 0.22f));
            lineRenderer.startWidth = width;
            lineRenderer.endWidth = width;
            lineRenderer.startColor = Color.Lerp(displayedLineCoreColor, displayedLineGlowColor, 0.28f + (idlePulse * 0.12f) + (pulseStrength * 0.25f));
            lineRenderer.endColor = Color.Lerp(displayedLineGlowColor, Color.white, idlePulse * 0.08f + pulseStrength * 0.22f);
        }

        public void ResetRun()
        {
            TravelDistance = 0f;
            pulseStrength = 0f;
            UpdateLineGeometry();
        }

        public void Advance(float deltaTime, float speed)
        {
            TravelDistance += speed * deltaTime;
            UpdateLineGeometry();
        }

        public void PlayLinePulse(float intensity = 1f)
        {
            pulseStrength = Mathf.Clamp01(Mathf.Max(pulseStrength, intensity));
        }

        public float GetTrackCenterX(float worldY)
        {
            float distanceFromPlayer = worldY - PlayerAnchorY;
            float pathDistance = TravelDistance + distanceFromPlayer;
            return EvaluateTrackCenterX(pathDistance);
        }

        public float GetSideX(PlayerSide side)
        {
            return GetTrackCenterX(PlayerAnchorY) + (side == PlayerSide.Top ? gameBalance.SideOffset : -gameBalance.SideOffset);
        }

        public float GetWorldYForHitDistance(float hitDistance)
        {
            return PlayerAnchorY + (hitDistance - TravelDistance);
        }

        private void EnsureRuntimeHierarchy()
        {
            worldRoot ??= new GameObject("GameplayWorld").transform;
            worldRoot.SetParent(transform, false);

            playerRoot ??= new GameObject("PlayerRoot").transform;
            playerRoot.SetParent(worldRoot, false);

            hazardsRoot ??= new GameObject("HazardsRoot").transform;
            hazardsRoot.SetParent(worldRoot, false);
        }

        private void EnsureLineRenderer()
        {
            Transform existing = worldRoot.Find("TrackLine");
            if (existing == null)
            {
                existing = new GameObject("TrackLine").transform;
                existing.SetParent(worldRoot, false);
            }

            lineRenderer = existing.GetComponent<LineRenderer>();
            if (lineRenderer == null)
            {
                lineRenderer = existing.gameObject.AddComponent<LineRenderer>();
            }

            lineRenderer.positionCount = LinePointCount;
            lineRenderer.useWorldSpace = true;
            lineRenderer.alignment = LineAlignment.TransformZ;
            lineRenderer.numCapVertices = 8;
            lineRenderer.textureMode = LineTextureMode.Stretch;
            lineRenderer.startWidth = presentationConfig.TrackLineWidth;
            lineRenderer.endWidth = presentationConfig.TrackLineWidth;
            lineRenderer.sortingOrder = 0;

            Shader shader = Shader.Find("Sprites/Default") ?? Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default");
            lineMaterial = new Material(shader);
            lineRenderer.material = lineMaterial;
            UpdateLineGeometry();
        }

        private void UpdateLineGeometry()
        {
            if (lineRenderer == null)
            {
                return;
            }

            float extent = gameplayCamera.orthographicSize + 3f;
            for (int i = 0; i < LinePointCount; i++)
            {
                float t = LinePointCount > 1 ? i / (LinePointCount - 1f) : 0f;
                float worldY = Mathf.Lerp(-extent, extent, t);
                lineRenderer.SetPosition(i, new Vector3(GetTrackCenterX(worldY), worldY, LineZ));
            }
        }

        private void UpdateThemeTransition(float deltaTime)
        {
            if (themeTransitionDuration <= 0f)
            {
                return;
            }

            themeTransitionElapsed += deltaTime;
            float t = Mathf.Clamp01(themeTransitionElapsed / themeTransitionDuration);
            displayedLineCoreColor = Color.Lerp(transitionFromLineCoreColor, transitionToLineCoreColor, t);
            displayedLineGlowColor = Color.Lerp(transitionFromLineGlowColor, transitionToLineGlowColor, t);
            displayedLineWidthMultiplier = Mathf.Lerp(transitionFromLineWidthMultiplier, transitionToLineWidthMultiplier, t);
            displayedPulseSpeedMultiplier = Mathf.Lerp(transitionFromPulseSpeedMultiplier, transitionToPulseSpeedMultiplier, t);
            if (t >= 1f)
            {
                themeTransitionDuration = 0f;
            }
        }

        private void ApplyLineTargets(Color targetCore, Color targetGlow, float targetWidthMultiplier, float targetPulseSpeedMultiplier, float transitionDurationSeconds)
        {
            transitionFromLineCoreColor = displayedLineCoreColor == default ? targetCore : displayedLineCoreColor;
            transitionFromLineGlowColor = displayedLineGlowColor == default ? targetGlow : displayedLineGlowColor;
            transitionFromLineWidthMultiplier = displayedLineWidthMultiplier <= 0f ? targetWidthMultiplier : displayedLineWidthMultiplier;
            transitionFromPulseSpeedMultiplier = displayedPulseSpeedMultiplier <= 0f ? targetPulseSpeedMultiplier : displayedPulseSpeedMultiplier;
            transitionToLineCoreColor = targetCore;
            transitionToLineGlowColor = targetGlow;
            transitionToLineWidthMultiplier = targetWidthMultiplier;
            transitionToPulseSpeedMultiplier = targetPulseSpeedMultiplier;
            themeTransitionDuration = Mathf.Max(0f, transitionDurationSeconds);
            themeTransitionElapsed = 0f;

            if (themeTransitionDuration <= 0f)
            {
                displayedLineCoreColor = transitionToLineCoreColor;
                displayedLineGlowColor = transitionToLineGlowColor;
                displayedLineWidthMultiplier = transitionToLineWidthMultiplier;
                displayedPulseSpeedMultiplier = transitionToPulseSpeedMultiplier;
            }
        }

        private Color ResolveLineCoreColor()
        {
            if (currentDistrictState != null)
            {
                return currentDistrictState.LineCoreColor;
            }

            return theme != null ? theme.LineCoreColor : Color.white;
        }

        private Color ResolveLineGlowColor()
        {
            if (currentDistrictState != null)
            {
                return currentDistrictState.LineGlowColor;
            }

            return theme != null ? theme.LineGlowColor : Color.white;
        }

        private float ResolveLineWidthMultiplier()
        {
            return currentDistrictState != null ? Mathf.Max(0.75f, currentDistrictState.LineWidthMultiplier) : 1f;
        }

        private float ResolvePulseSpeedMultiplier()
        {
            return currentDistrictState != null ? Mathf.Max(0.5f, currentDistrictState.LinePulseSpeedMultiplier) : 1f;
        }

        private float EvaluateTrackCenterX(float pathDistance)
        {
            float primary = Mathf.Sin((pathDistance / presentationConfig.TrackCurvePrimaryWavelength) + 0.45f)
                * presentationConfig.TrackCurvePrimaryAmplitude;
            float secondary = Mathf.Sin((pathDistance / presentationConfig.TrackCurveSecondaryWavelength) + 1.1f)
                * presentationConfig.TrackCurveSecondaryAmplitude;
            return primary + secondary;
        }
    }
}
