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
            public SpriteRenderer TelegraphRenderer;
            public PlayerSide DangerousSide;
            public float HitDistance;
            public float CollisionHalfWidth;
            public float CollisionHalfHeight;
            public bool ScoreAwarded;
        }

        private readonly List<ObstacleConfig> eligibleBuffer = new();
        private readonly List<HazardRuntime> activeHazards = new();

        private GameBalanceConfig gameBalance;
        private DifficultyDirector difficultyDirector;
        private TrackManager trackManager;
        private ThemeConfig theme;
        private System.Random random;
        private float nextHitDistance;
        private float lastSpawnHitDistance;
        private float lastSpawnVisualHalfHeight;
        private int spawnCount;

        public void Initialize(
            GameBalanceConfig balanceConfig,
            ObstacleCatalog catalog,
            DifficultyDirector director,
            TrackManager track,
            ThemeConfig activeTheme)
        {
            gameBalance = balanceConfig;
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

            float introDistance = difficultyDirector.GetCurrentSpeed(0) * gameBalance.SafeStartWindowSeconds;
            nextHitDistance = trackManager.TravelDistance + introDistance + GetNextSpacing(0);
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
                float hazardX = trackManager.GetSideX(hazard.DangerousSide);
                hazard.Root.transform.position = new Vector3(hazardX, worldY, 0f);
                UpdateVisuals(hazard, worldY);

                if (collisionConfig == null && IsCollision(hazard, hazardX, worldY, playerController))
                {
                    collisionConfig = hazard.Config;
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

                HazardLayoutProfile layoutProfile = HazardFamilyPresentation.GetLayoutProfile(config.Family);
                if (lastSpawnHitDistance > float.NegativeInfinity)
                {
                    float minimumReadableHitDistance = lastSpawnHitDistance
                        + HazardFamilyPresentation.GetRequiredHitDistanceSeparation(lastSpawnVisualHalfHeight, layoutProfile.VisualHalfHeight);
                    nextHitDistance = Mathf.Max(nextHitDistance, minimumReadableHitDistance);
                }

                PlayerSide side = spawnCount % 2 == 0 ? PlayerSide.Top : PlayerSide.Bottom;
                if (spawnCount == 0)
                {
                    side = PlayerSide.Top;
                }

                SpawnHazard(config, layoutProfile, side, nextHitDistance);
                lastSpawnHitDistance = nextHitDistance;
                lastSpawnVisualHalfHeight = layoutProfile.VisualHalfHeight;
                spawnCount++;
                nextHitDistance += GetNextSpacing(currentScore);
            }
        }

        private ObstacleConfig SelectObstacleConfig(int currentScore)
        {
            difficultyDirector.PopulateEligibleObstacleConfigs(currentScore, eligibleBuffer);

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

            return eligibleBuffer.Count > 0 ? eligibleBuffer[eligibleBuffer.Count - 1] : null;
        }

        private float GetNextSpacing(int currentScore)
        {
            DifficultyCurveConfig.DifficultyBandDefinition band = difficultyDirector.GetBand(currentScore);
            float min = band != null ? band.MinSpawnSpacing : 2.2f;
            float max = band != null ? band.MaxSpawnSpacing : 2.8f;
            double sample = random != null ? random.NextDouble() : 0.5d;
            return Mathf.Lerp(min, max, (float)sample);
        }

        private void SpawnHazard(ObstacleConfig config, HazardLayoutProfile layoutProfile, PlayerSide dangerousSide, float hitDistance)
        {
            GameObject root = new($"Hazard_{spawnCount}_{config.DisplayName}");
            root.transform.SetParent(trackManager.HazardsRoot, false);

            SpriteRenderer mainRenderer = root.AddComponent<SpriteRenderer>();
            mainRenderer.sprite = RuntimeSpriteFactory.WhiteSprite;
            mainRenderer.sortingOrder = 3;

            SpriteRenderer telegraphRenderer = null;
            if (layoutProfile.UsesTelegraph)
            {
                GameObject telegraph = new("Telegraph");
                telegraph.transform.SetParent(root.transform, false);
                telegraphRenderer = telegraph.AddComponent<SpriteRenderer>();
                telegraphRenderer.sprite = RuntimeSpriteFactory.WhiteSprite;
                telegraphRenderer.sortingOrder = 2;
            }

            HazardRuntime runtime = new()
            {
                Config = config,
                Root = root,
                MainRenderer = mainRenderer,
                TelegraphRenderer = telegraphRenderer,
                DangerousSide = dangerousSide,
                HitDistance = hitDistance,
                CollisionHalfWidth = layoutProfile.CollisionHalfWidth,
                CollisionHalfHeight = layoutProfile.CollisionHalfHeight,
                ScoreAwarded = false,
            };

            ConfigureVisual(runtime, layoutProfile);
            activeHazards.Add(runtime);
        }

        private void ConfigureVisual(HazardRuntime hazard, HazardLayoutProfile layoutProfile)
        {
            hazard.Root.transform.localScale = new Vector3(layoutProfile.MainScale.x, layoutProfile.MainScale.y, 1f);

            switch (hazard.Config.Family)
            {
                case ObstacleFamily.ElectricGates:
                    hazard.MainRenderer.color = new Color(1f, 0.86f, 0.28f, 0.98f);
                    if (hazard.TelegraphRenderer != null)
                    {
                        hazard.TelegraphRenderer.transform.localScale = new Vector3(layoutProfile.TelegraphScale.x, layoutProfile.TelegraphScale.y, 1f);
                        hazard.TelegraphRenderer.color = new Color(1f, 0.95f, 0.35f, 0.28f);
                    }
                    break;

                default:
                    hazard.MainRenderer.color = theme.DangerColor;
                    break;
            }
        }

        private void UpdateVisuals(HazardRuntime hazard, float worldY)
        {
            if (hazard.TelegraphRenderer != null)
            {
                float distanceToPlayer = Mathf.Abs(worldY - trackManager.PlayerAnchorY);
                float telegraphDistance = Mathf.Max(1f, difficultyDirector.GetCurrentSpeed(0) * hazard.Config.MinimumTelegraphSeconds);
                float telegraphStrength = 1f - Mathf.Clamp01(distanceToPlayer / telegraphDistance);
                Color telegraphColor = new(1f, 0.95f, 0.35f, Mathf.Lerp(0.2f, 0.58f, telegraphStrength));
                hazard.TelegraphRenderer.color = telegraphColor;
                hazard.MainRenderer.color = Color.Lerp(new Color(1f, 0.86f, 0.28f, 0.98f), theme.DangerColor, telegraphStrength);
            }
        }

        private bool IsCollision(HazardRuntime hazard, float hazardX, float worldY, PlayerController playerController)
        {
            float verticalThreshold = hazard.CollisionHalfHeight + playerController.CollisionHalfHeight;
            if (Mathf.Abs(worldY - trackManager.PlayerAnchorY) > verticalThreshold)
            {
                return false;
            }

            float horizontalThreshold = hazard.CollisionHalfWidth + playerController.CollisionHalfWidth;
            if (Mathf.Abs(hazardX - playerController.CurrentX) > horizontalThreshold)
            {
                return false;
            }

            return true;
        }
    }
}
