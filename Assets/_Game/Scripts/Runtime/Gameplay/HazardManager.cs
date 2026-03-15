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
            public float HalfHeight;
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
                hazard.Root.transform.position = new Vector3(trackManager.GetSideX(hazard.DangerousSide), worldY, 0f);
                UpdateVisuals(hazard, worldY);

                if (collisionConfig == null && IsCollision(hazard, worldY, playerController))
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
                PlayerSide side = spawnCount % 2 == 0 ? PlayerSide.Top : PlayerSide.Bottom;
                if (spawnCount == 0)
                {
                    side = PlayerSide.Top;
                }

                SpawnHazard(config, side, nextHitDistance);
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

        private void SpawnHazard(ObstacleConfig config, PlayerSide dangerousSide, float hitDistance)
        {
            if (config == null)
            {
                return;
            }

            GameObject root = new($"Hazard_{spawnCount}_{config.DisplayName}");
            root.transform.SetParent(trackManager.HazardsRoot, false);

            SpriteRenderer mainRenderer = root.AddComponent<SpriteRenderer>();
            mainRenderer.sprite = RuntimeSpriteFactory.WhiteSprite;
            mainRenderer.sortingOrder = 3;

            SpriteRenderer telegraphRenderer = null;
            if (config.RequiresTelegraph)
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
                HalfHeight = GetHalfHeight(config.Family),
                ScoreAwarded = false,
            };

            ConfigureVisual(runtime);
            activeHazards.Add(runtime);
        }

        private void ConfigureVisual(HazardRuntime hazard)
        {
            switch (hazard.Config.Family)
            {
                case ObstacleFamily.ElectricGates:
                    hazard.Root.transform.localScale = new Vector3(0.32f, 1.05f, 1f);
                    hazard.MainRenderer.color = new Color(1f, 0.86f, 0.28f, 0.95f);
                    if (hazard.TelegraphRenderer != null)
                    {
                        hazard.TelegraphRenderer.transform.localScale = new Vector3(0.72f, 1.55f, 1f);
                        hazard.TelegraphRenderer.color = new Color(1f, 0.95f, 0.35f, 0.18f);
                    }
                    break;

                case ObstacleFamily.SideBlockers:
                    hazard.Root.transform.localScale = new Vector3(0.55f, 1.15f, 1f);
                    hazard.MainRenderer.color = theme.DangerColor;
                    break;

                default:
                    hazard.Root.transform.localScale = new Vector3(0.38f, 0.92f, 1f);
                    hazard.MainRenderer.color = theme.DangerColor;
                    break;
            }
        }

        private void UpdateVisuals(HazardRuntime hazard, float worldY)
        {
            float distanceToPlayer = Mathf.Abs(worldY - trackManager.PlayerAnchorY);
            if (hazard.TelegraphRenderer != null)
            {
                float telegraphDistance = Mathf.Max(1f, difficultyDirector.GetCurrentSpeed(0) * hazard.Config.MinimumTelegraphSeconds);
                float telegraphStrength = 1f - Mathf.Clamp01(distanceToPlayer / telegraphDistance);
                Color telegraphColor = new(1f, 0.95f, 0.35f, Mathf.Lerp(0.12f, 0.45f, telegraphStrength));
                hazard.TelegraphRenderer.color = telegraphColor;
                hazard.MainRenderer.color = Color.Lerp(new Color(1f, 0.86f, 0.28f, 0.95f), theme.DangerColor, telegraphStrength);
            }
        }

        private bool IsCollision(HazardRuntime hazard, float worldY, PlayerController playerController)
        {
            float verticalThreshold = hazard.HalfHeight + playerController.Radius;
            if (Mathf.Abs(worldY - trackManager.PlayerAnchorY) > verticalThreshold)
            {
                return false;
            }

            bool onDangerousSide = hazard.DangerousSide == PlayerSide.Top
                ? playerController.CurrentX >= 0f
                : playerController.CurrentX <= 0f;

            return onDangerousSide;
        }

        private static float GetHalfHeight(ObstacleFamily family)
        {
            return family switch
            {
                ObstacleFamily.SideBlockers => 0.62f,
                ObstacleFamily.ElectricGates => 0.55f,
                _ => 0.48f,
            };
        }
    }
}