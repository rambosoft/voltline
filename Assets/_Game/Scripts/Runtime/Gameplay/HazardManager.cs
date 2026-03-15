using System.Collections.Generic;
using UnityEngine;
using Voltline.Data;
using Voltline.Utilities;

namespace Voltline.Gameplay
{
    public sealed class HazardManager : MonoBehaviour
    {
        private sealed class HazardRuntime
        {
            public ObstacleConfig Config;
            public GameObject Root;
            public SpriteRenderer MainRenderer;
            public SpriteRenderer SecondaryRenderer;
            public SpriteRenderer AccentRenderer;
            public SpriteRenderer TelegraphRenderer;
            public PlayerSide DangerousSide;
            public HazardLayoutProfile LayoutProfile;
            public float HitDistance;
            public float AnimationSeed;
            public bool ScoreAwarded;
            public bool NearMissTriggered;
        }

        private readonly List<ObstacleConfig> eligibleBuffer = new();
        private readonly List<HazardRuntime> activeHazards = new();

        private GameBalanceConfig gameBalance;
        private HazardPresentationCatalog hazardPresentationCatalog;
        private DifficultyDirector difficultyDirector;
        private TrackManager trackManager;
        private ThemeConfig theme;
        private System.Random random;
        private float nextHitDistance;
        private float lastSpawnHitDistance;
        private float lastSpawnVisualHalfHeight;
        private float lastSpawnGapPadding;
        private PlayerSide lastDangerousSide;
        private int sameSideStreak;
        private int spawnCount;

        public event System.Action<Vector3> NearMissTriggered;

        public void Initialize(
            GameBalanceConfig balanceConfig,
            ObstacleCatalog catalog,
            HazardPresentationCatalog presentationCatalog,
            DifficultyDirector director,
            TrackManager track,
            ThemeConfig activeTheme)
        {
            gameBalance = balanceConfig;
            hazardPresentationCatalog = presentationCatalog;
            difficultyDirector = director;
            trackManager = track;
            theme = activeTheme;
        }

        public void ResetRun(int seed)
        {
            for (int i = 0; i < activeHazards.Count; i++)
            {
                if (activeHazards[i].Root != null)
                {
                    Destroy(activeHazards[i].Root);
                }
            }

            activeHazards.Clear();
            random = new System.Random(seed);
            spawnCount = 0;
            lastSpawnHitDistance = float.NegativeInfinity;
            lastSpawnVisualHalfHeight = 0f;
            lastSpawnGapPadding = 0f;
            lastDangerousSide = PlayerSide.Top;
            sameSideStreak = 0;

            float introDistance = difficultyDirector.GetCurrentSpeed(0) * gameBalance.SafeStartWindowSeconds;
            nextHitDistance = trackManager.TravelDistance + introDistance + GetNextSpacing(0, null);
        }

        public int Tick(int currentScore, PlayerController playerController, out ObstacleConfig collisionConfig)
        {
            collisionConfig = null;
            SpawnAhead(currentScore);

            int clearedBeats = 0;
            for (int i = activeHazards.Count - 1; i >= 0; i--)
            {
                HazardRuntime hazard = activeHazards[i];
                float worldY = trackManager.GetWorldYForHitDistance(hazard.HitDistance);
                float trackCenterX = trackManager.GetTrackCenterX(worldY);
                float anchorX = GetAnchorX(hazard, trackCenterX);
                hazard.Root.transform.position = new Vector3(anchorX, worldY, 0f);
                UpdateVisuals(hazard, currentScore, worldY, trackCenterX);

                if (collisionConfig == null && IsCollision(hazard, trackCenterX, worldY, playerController))
                {
                    collisionConfig = hazard.Config;
                }
                else if (!hazard.NearMissTriggered && IsNearMiss(hazard, trackCenterX, worldY, playerController))
                {
                    hazard.NearMissTriggered = true;
                    NearMissTriggered?.Invoke(new Vector3((anchorX + playerController.CurrentX) * 0.5f, trackManager.PlayerAnchorY, 0f));
                }

                if (!hazard.ScoreAwarded && trackManager.TravelDistance > hazard.HitDistance + 0.45f)
                {
                    hazard.ScoreAwarded = true;
                    clearedBeats++;
                }

                if (worldY < trackManager.LowerDespawnY)
                {
                    Destroy(hazard.Root);
                    activeHazards.RemoveAt(i);
                }
            }

            return clearedBeats;
        }

        private void SpawnAhead(int currentScore)
        {
            float maxVisibleHitDistance = trackManager.TravelDistance + trackManager.VisibleDistance;
            while (nextHitDistance <= maxVisibleHitDistance)
            {
                ObstacleConfig config = SelectObstacleConfig(currentScore);
                if (config == null)
                {
                    return;
                }

                HazardLayoutProfile layoutProfile = hazardPresentationCatalog.GetRequiredProfile(config.Family);
                if (lastSpawnHitDistance > float.NegativeInfinity)
                {
                    HazardLayoutProfile previousProfile = new(
                        new Vector2(0f, lastSpawnVisualHalfHeight * 2f),
                        Vector2.zero,
                        Vector2.zero,
                        lastSpawnGapPadding);
                    float minimumReadableHitDistance = lastSpawnHitDistance
                        + hazardPresentationCatalog.GetRequiredHitDistanceSeparation(previousProfile, layoutProfile);
                    nextHitDistance = Mathf.Max(nextHitDistance, minimumReadableHitDistance);
                }

                PlayerSide side = ChooseDangerousSide(currentScore);
                SpawnHazard(config, layoutProfile, side, nextHitDistance);
                lastSpawnHitDistance = nextHitDistance;
                lastSpawnVisualHalfHeight = layoutProfile.VisualHalfHeight;
                lastSpawnGapPadding = layoutProfile.MinimumReadableGapPadding;
                spawnCount++;
                nextHitDistance += GetNextSpacing(currentScore, config);
            }
        }

        private ObstacleConfig SelectObstacleConfig(int currentScore)
        {
            difficultyDirector.PopulateEligibleObstacleConfigs(currentScore, eligibleBuffer);
            if (eligibleBuffer.Count == 0)
            {
                return null;
            }

            if (spawnCount < 2)
            {
                for (int i = 0; i < eligibleBuffer.Count; i++)
                {
                    if (eligibleBuffer[i].Family == ObstacleFamily.Spikes)
                    {
                        return eligibleBuffer[i];
                    }
                }
            }

            float totalWeight = 0f;
            for (int i = 0; i < eligibleBuffer.Count; i++)
            {
                totalWeight += Mathf.Max(0.01f, eligibleBuffer[i].Weight);
            }

            float pick = (float)random.NextDouble() * totalWeight;
            float cursor = 0f;
            for (int i = 0; i < eligibleBuffer.Count; i++)
            {
                ObstacleConfig candidate = eligibleBuffer[i];
                cursor += Mathf.Max(0.01f, candidate.Weight);
                if (pick <= cursor)
                {
                    return candidate;
                }
            }

            return eligibleBuffer[eligibleBuffer.Count - 1];
        }

        private PlayerSide ChooseDangerousSide(int currentScore)
        {
            if (spawnCount == 0)
            {
                lastDangerousSide = PlayerSide.Top;
                sameSideStreak = 1;
                return PlayerSide.Top;
            }

            if (spawnCount == 1 || currentScore < 8)
            {
                PlayerSide alternating = Opposite(lastDangerousSide);
                lastDangerousSide = alternating;
                sameSideStreak = 1;
                return alternating;
            }

            PlayerSide candidate = random.NextDouble() >= 0.5d ? PlayerSide.Top : PlayerSide.Bottom;
            if (sameSideStreak >= 2 && candidate == lastDangerousSide)
            {
                candidate = Opposite(lastDangerousSide);
            }
            else if (sameSideStreak >= 1 && candidate == lastDangerousSide && currentScore < 16)
            {
                candidate = Opposite(lastDangerousSide);
            }

            sameSideStreak = candidate == lastDangerousSide ? sameSideStreak + 1 : 1;
            lastDangerousSide = candidate;
            return candidate;
        }

        private static PlayerSide Opposite(PlayerSide side)
        {
            return side == PlayerSide.Top ? PlayerSide.Bottom : PlayerSide.Top;
        }

        private float GetNextSpacing(int currentScore, ObstacleConfig obstacle)
        {
            difficultyDirector.GetSpawnSpacingRange(currentScore, obstacle, out float minSpacing, out float maxSpacing);
            double sample = random != null ? random.NextDouble() : 0.5d;
            return Mathf.Lerp(minSpacing, maxSpacing, (float)sample);
        }

        private void SpawnHazard(ObstacleConfig config, HazardLayoutProfile layoutProfile, PlayerSide dangerousSide, float hitDistance)
        {
            GameObject root = new($"Hazard_{spawnCount}_{config.DisplayName}");
            root.transform.SetParent(trackManager.HazardsRoot, false);

            HazardRuntime runtime = new()
            {
                Config = config,
                Root = root,
                MainRenderer = CreateRenderer("Main", root.transform, 3),
                SecondaryRenderer = CreateRenderer("Secondary", root.transform, 4),
                AccentRenderer = CreateRenderer("Accent", root.transform, 5),
                TelegraphRenderer = CreateRenderer("Telegraph", root.transform, 2),
                DangerousSide = dangerousSide,
                LayoutProfile = layoutProfile,
                HitDistance = hitDistance,
                AnimationSeed = (float)random.NextDouble() * 10f,
                ScoreAwarded = false,
                NearMissTriggered = false,
            };

            ConfigureBaseVisuals(runtime);
            activeHazards.Add(runtime);
        }

        private static SpriteRenderer CreateRenderer(string name, Transform parent, int sortingOrder)
        {
            GameObject child = new(name);
            child.transform.SetParent(parent, false);
            SpriteRenderer renderer = child.AddComponent<SpriteRenderer>();
            renderer.sprite = RuntimeSpriteFactory.WhiteSprite;
            renderer.sortingOrder = sortingOrder;
            return renderer;
        }

        private void ConfigureBaseVisuals(HazardRuntime hazard)
        {
            SetRenderer(hazard.MainRenderer, true, Vector3.zero, hazard.LayoutProfile.VisualBoundsScale, theme.DangerColor, 0f);
            SetRenderer(hazard.SecondaryRenderer, false, Vector3.zero, Vector2.one, Color.clear, 0f);
            SetRenderer(hazard.AccentRenderer, false, Vector3.zero, Vector2.one, Color.clear, 0f);
            SetRenderer(hazard.TelegraphRenderer, hazard.LayoutProfile.UsesTelegraph, Vector3.zero, hazard.LayoutProfile.TelegraphBoundsScale, new Color(theme.LineGlowColor.r, theme.LineGlowColor.g, theme.LineGlowColor.b, 0.16f), 0f);
        }

        private void UpdateVisuals(HazardRuntime hazard, int currentScore, float worldY, float trackCenterX)
        {
            float speed = difficultyDirector.GetCurrentSpeed(currentScore);
            float distanceToPlayer = Mathf.Abs(worldY - trackManager.PlayerAnchorY);
            float telegraphSeconds = difficultyDirector.GetEffectiveTelegraphSeconds(currentScore, hazard.Config);
            float telegraphDistance = Mathf.Max(0.75f, speed * telegraphSeconds);
            float telegraphStrength = 1f - Mathf.Clamp01(distanceToPlayer / telegraphDistance);
            float sideSign = hazard.DangerousSide == PlayerSide.Top ? 1f : -1f;

            switch (hazard.Config.Family)
            {
                case ObstacleFamily.Spikes:
                    ApplySpikeVisuals(hazard, telegraphStrength);
                    break;

                case ObstacleFamily.RotatingCutters:
                    ApplyRotatingCutterVisuals(hazard, telegraphStrength);
                    break;

                case ObstacleFamily.ElectricGates:
                    ApplyElectricGateVisuals(hazard, telegraphStrength, trackCenterX);
                    break;

                case ObstacleFamily.BrokenLineGaps:
                    ApplyGapVisuals(hazard, telegraphStrength);
                    break;

                case ObstacleFamily.SideBlockers:
                    ApplySideBlockerVisuals(hazard, telegraphStrength, sideSign);
                    break;
            }
        }

        private void ApplySpikeVisuals(HazardRuntime hazard, float telegraphStrength)
        {
            Color core = Color.Lerp(theme.DangerColor, Color.white, telegraphStrength * 0.12f);
            Vector2 visual = hazard.LayoutProfile.VisualBoundsScale;
            SetRenderer(hazard.MainRenderer, true, Vector3.zero, visual, core, 0f);
            SetRenderer(hazard.SecondaryRenderer, true, new Vector3(0f, visual.y * 0.18f, 0f), new Vector2(visual.x * 0.72f, visual.y * 0.36f), core, 0f);
            SetRenderer(hazard.AccentRenderer, true, new Vector3(0f, -visual.y * 0.18f, 0f), new Vector2(visual.x * 0.72f, visual.y * 0.36f), core, 0f);
            hazard.TelegraphRenderer.enabled = false;
        }

        private void ApplyRotatingCutterVisuals(HazardRuntime hazard, float telegraphStrength)
        {
            float angle = (Time.time * 220f) + (hazard.AnimationSeed * 38f);
            Color bladeColor = Color.Lerp(theme.DangerColor, new Color(1f, 0.9f, 0.95f, 1f), telegraphStrength * 0.2f);
            Vector2 visual = hazard.LayoutProfile.VisualBoundsScale;
            Vector2 telegraph = hazard.LayoutProfile.TelegraphBoundsScale;
            SetRenderer(hazard.MainRenderer, true, Vector3.zero, new Vector2(visual.x * 0.28f, visual.y * 1.12f), bladeColor, angle);
            SetRenderer(hazard.SecondaryRenderer, true, Vector3.zero, new Vector2(visual.x * 0.21f, visual.y * 0.81f), bladeColor, -angle * 0.82f);
            SetRenderer(hazard.AccentRenderer, true, Vector3.zero, new Vector2(visual.x * 0.42f, visual.x * 0.42f), theme.PlayerAccentColor, 0f);
            SetRenderer(hazard.TelegraphRenderer, telegraph.x > 0f && telegraph.y > 0f, Vector3.zero, telegraph, new Color(theme.LineGlowColor.r, theme.LineGlowColor.g, theme.LineGlowColor.b, Mathf.Lerp(0.08f, 0.22f, telegraphStrength)), 0f);
        }

        private void ApplyElectricGateVisuals(HazardRuntime hazard, float telegraphStrength, float trackCenterX)
        {
            float directionToCenter = trackCenterX - hazard.Root.transform.position.x;
            float beamLength = Mathf.Abs(directionToCenter) + 0.18f;
            float beamOffset = directionToCenter * 0.5f;
            float arcPulse = 0.55f + (Mathf.Sin((Time.time + hazard.AnimationSeed) * 18f) * 0.45f);
            Vector2 visual = hazard.LayoutProfile.VisualBoundsScale;
            Vector2 telegraph = hazard.LayoutProfile.TelegraphBoundsScale;

            SetRenderer(hazard.MainRenderer, true, Vector3.zero, visual, new Color(1f, 0.85f, 0.28f, 1f), 0f);
            SetRenderer(hazard.SecondaryRenderer, true, new Vector3(beamOffset, 0f, 0f), new Vector2(beamLength, 0.12f + telegraphStrength * 0.05f), new Color(1f, 0.94f, 0.4f, Mathf.Lerp(0.3f, 0.75f, telegraphStrength * arcPulse)), 0f);
            SetRenderer(hazard.AccentRenderer, true, Vector3.zero, new Vector2(visual.x * 0.65f, visual.x * 0.65f), new Color(1f, 0.96f, 0.6f, 0.95f), 0f);
            SetRenderer(hazard.TelegraphRenderer, telegraph.x > 0f && telegraph.y > 0f, new Vector3(beamOffset, 0f, 0f), new Vector2(beamLength + 0.22f, telegraph.y), new Color(1f, 0.95f, 0.35f, Mathf.Lerp(0.08f, 0.24f, telegraphStrength)), 0f);
        }

        private void ApplyGapVisuals(HazardRuntime hazard, float telegraphStrength)
        {
            PlayerSide safeSide = Opposite(hazard.DangerousSide);
            float safeSideOffset = safeSide == PlayerSide.Top ? gameBalance.SideOffset : -gameBalance.SideOffset;
            Color gapCutout = Color.Lerp(theme.BackgroundTopColor, theme.BackgroundBottomColor, 0.5f);
            gapCutout.a = 1f;

            SetRenderer(hazard.MainRenderer, true, Vector3.zero, hazard.LayoutProfile.VisualBoundsScale, gapCutout, 0f);
            SetRenderer(hazard.SecondaryRenderer, true, new Vector3(0f, 0.44f, 0f), new Vector2(0.48f, 0.08f), theme.LineGlowColor, 0f);
            SetRenderer(hazard.AccentRenderer, true, new Vector3(0f, -0.44f, 0f), new Vector2(0.48f, 0.08f), theme.LineGlowColor, 0f);
            SetRenderer(hazard.TelegraphRenderer, hazard.LayoutProfile.UsesTelegraph, new Vector3(safeSideOffset, 0f, 0f), hazard.LayoutProfile.TelegraphBoundsScale, new Color(theme.PlayerAccentColor.r, theme.PlayerAccentColor.g, theme.PlayerAccentColor.b, Mathf.Lerp(0.24f, 0.8f, telegraphStrength)), 0f);
        }

        private void ApplySideBlockerVisuals(HazardRuntime hazard, float telegraphStrength, float sideSign)
        {
            Vector2 visual = hazard.LayoutProfile.VisualBoundsScale;
            Vector2 telegraph = hazard.LayoutProfile.TelegraphBoundsScale;
            SetRenderer(hazard.MainRenderer, true, Vector3.zero, visual, theme.DangerColor, 0f);
            SetRenderer(hazard.SecondaryRenderer, true, new Vector3(-sideSign * visual.x * 0.22f, 0f, 0f), new Vector2(visual.x * 0.11f, visual.y * 0.94f), new Color(1f, 1f, 1f, 0.42f), 0f);
            SetRenderer(hazard.AccentRenderer, true, new Vector3(0f, visual.y * 0.24f, 0f), new Vector2(visual.x * 0.46f, visual.y * 0.1f), new Color(1f, 0.82f, 0.9f, 0.75f), 0f);
            SetRenderer(hazard.TelegraphRenderer, telegraph.x > 0f && telegraph.y > 0f, Vector3.zero, telegraph, new Color(theme.DangerColor.r, theme.DangerColor.g, theme.DangerColor.b, Mathf.Lerp(0.08f, 0.22f, telegraphStrength)), 0f);
        }

        private static void SetRenderer(SpriteRenderer renderer, bool enabled, Vector3 localPosition, Vector2 scale, Color color, float rotationDegrees)
        {
            if (renderer == null)
            {
                return;
            }

            renderer.enabled = enabled;
            if (!enabled)
            {
                return;
            }

            renderer.transform.localPosition = localPosition;
            renderer.transform.localScale = new Vector3(scale.x, scale.y, 1f);
            renderer.transform.localRotation = Quaternion.Euler(0f, 0f, rotationDegrees);
            renderer.color = color;
        }

        private float GetAnchorX(HazardRuntime hazard, float trackCenterX)
        {
            return hazard.Config.Family == ObstacleFamily.BrokenLineGaps
                ? trackCenterX
                : (hazard.DangerousSide == PlayerSide.Top ? gameBalance.SideOffset : -gameBalance.SideOffset) + trackCenterX;
        }

        private bool IsCollision(HazardRuntime hazard, float trackCenterX, float worldY, PlayerController playerController)
        {
            float verticalThreshold = hazard.LayoutProfile.CollisionHalfHeight + playerController.CollisionHalfHeight;
            if (Mathf.Abs(worldY - trackManager.PlayerAnchorY) > verticalThreshold)
            {
                return false;
            }

            if (hazard.Config.Family == ObstacleFamily.BrokenLineGaps)
            {
                float signedOffset = playerController.CurrentX - trackCenterX;
                float centerTolerance = playerController.CollisionHalfWidth * 0.3f;
                return hazard.DangerousSide == PlayerSide.Top
                    ? signedOffset > -centerTolerance
                    : signedOffset < centerTolerance;
            }

            float hazardX = GetAnchorX(hazard, trackCenterX);
            float horizontalThreshold = hazard.LayoutProfile.CollisionHalfWidth + playerController.CollisionHalfWidth;
            return Mathf.Abs(hazardX - playerController.CurrentX) <= horizontalThreshold;
        }

        private bool IsNearMiss(HazardRuntime hazard, float trackCenterX, float worldY, PlayerController playerController)
        {
            if (hazard.Config.Family == ObstacleFamily.BrokenLineGaps)
            {
                return false;
            }

            float verticalThreshold = hazard.LayoutProfile.CollisionHalfHeight + playerController.CollisionHalfHeight;
            if (Mathf.Abs(worldY - trackManager.PlayerAnchorY) > verticalThreshold)
            {
                return false;
            }

            float hazardX = GetAnchorX(hazard, trackCenterX);
            float collisionThreshold = hazard.LayoutProfile.CollisionHalfWidth + playerController.CollisionHalfWidth;
            float horizontalDistance = Mathf.Abs(hazardX - playerController.CurrentX);
            return horizontalDistance > collisionThreshold
                && horizontalDistance <= collisionThreshold + gameBalance.NearMissThreshold;
        }
    }
}
