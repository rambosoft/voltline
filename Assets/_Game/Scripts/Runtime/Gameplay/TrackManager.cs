using UnityEngine;
using Voltline.Data;

namespace Voltline.Gameplay
{
    public sealed class TrackManager : MonoBehaviour
    {
        private const float CameraSize = 8f;
        private const float LineWidth = 0.14f;
        private const float LineZ = 0f;

        private GameBalanceConfig gameBalance;
        private ThemeConfig theme;
        private Camera gameplayCamera;
        private LineRenderer lineRenderer;
        private Material lineMaterial;
        private Transform worldRoot;
        private Transform playerRoot;
        private Transform hazardsRoot;

        public float TravelDistance { get; private set; }
        public float PlayerAnchorY => -gameplayCamera.orthographicSize * 0.68f;
        public float VisibleDistance => gameplayCamera.orthographicSize + 4f;
        public float LowerDespawnY => -gameplayCamera.orthographicSize - 2f;
        public Transform PlayerRoot => playerRoot;
        public Transform HazardsRoot => hazardsRoot;

        public void Initialize(GameBalanceConfig balanceConfig, ThemeConfig activeTheme, Camera targetCamera)
        {
            gameBalance = balanceConfig;
            theme = activeTheme;
            gameplayCamera = targetCamera;
            gameplayCamera.orthographic = true;
            gameplayCamera.orthographicSize = CameraSize;
            gameplayCamera.backgroundColor = new Color(
                (theme.BackgroundTopColor.r + theme.BackgroundBottomColor.r) * 0.5f,
                (theme.BackgroundTopColor.g + theme.BackgroundBottomColor.g) * 0.5f,
                (theme.BackgroundTopColor.b + theme.BackgroundBottomColor.b) * 0.5f,
                1f);

            EnsureRuntimeHierarchy();
            EnsureLineRenderer();
            ResetRun();
        }

        public void ResetRun()
        {
            TravelDistance = 0f;
            UpdateLineGeometry();
        }

        public void Advance(float deltaTime, float speed)
        {
            TravelDistance += speed * deltaTime;
        }

        public float GetSideX(PlayerSide side)
        {
            return side == PlayerSide.Top ? gameBalance.SideOffset : -gameBalance.SideOffset;
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

            lineRenderer.positionCount = 2;
            lineRenderer.useWorldSpace = true;
            lineRenderer.alignment = LineAlignment.TransformZ;
            lineRenderer.numCapVertices = 8;
            lineRenderer.textureMode = LineTextureMode.Stretch;
            lineRenderer.startWidth = LineWidth;
            lineRenderer.endWidth = LineWidth;
            lineRenderer.sortingOrder = 0;

            Shader shader = Shader.Find("Sprites/Default") ?? Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default");
            lineMaterial = new Material(shader);
            lineRenderer.material = lineMaterial;
            lineRenderer.startColor = theme.LineCoreColor;
            lineRenderer.endColor = theme.LineGlowColor;
            UpdateLineGeometry();
        }

        private void UpdateLineGeometry()
        {
            if (lineRenderer == null)
            {
                return;
            }

            float extent = gameplayCamera.orthographicSize + 3f;
            lineRenderer.SetPosition(0, new Vector3(0f, -extent, LineZ));
            lineRenderer.SetPosition(1, new Vector3(0f, extent, LineZ));
        }
    }
}