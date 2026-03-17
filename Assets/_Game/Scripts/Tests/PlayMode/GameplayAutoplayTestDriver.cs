using System.Collections.Generic;
using UnityEngine;
using Voltline.Gameplay;

namespace Voltline.Tests.PlayMode
{
    internal readonly struct GameplayAutoplayScenario
    {
        public GameplayAutoplayScenario(int targetScore, float maxUnscaledSeconds, float timeScale, float flipLeadDistance, int minimumEncounteredFamilies)
        {
            TargetScore = targetScore;
            MaxUnscaledSeconds = maxUnscaledSeconds;
            TimeScale = timeScale;
            FlipLeadDistance = flipLeadDistance;
            MinimumEncounteredFamilies = minimumEncounteredFamilies;
        }

        public int TargetScore { get; }
        public float MaxUnscaledSeconds { get; }
        public float TimeScale { get; }
        public float FlipLeadDistance { get; }
        public int MinimumEncounteredFamilies { get; }
    }

    internal static class GameplayAutoplayTestDriver
    {
        public static readonly GameplayAutoplayScenario MarathonForty = new(40, 30f, 1.5f, 1.5f, 3);

        public static void Tick(GameManager gameManager, PlayerController playerController, HazardManager hazardManager, float playerAnchorY, GameplayAutoplayScenario scenario, HashSet<Voltline.Data.ObstacleFamily> encounteredFamilies, List<HazardDebugSnapshot> snapshots)
        {
            if (gameManager == null || playerController == null || hazardManager == null)
            {
                return;
            }

            hazardManager.GetDebugActiveHazardSnapshots(snapshots);
            for (int i = 0; i < snapshots.Count; i++)
            {
                encounteredFamilies.Add(snapshots[i].Family);
            }

            if (playerController.IsDead || playerController.IsFlipping)
            {
                return;
            }

            if (!TryGetNearestUpcomingThreat(snapshots, playerAnchorY, out HazardDebugSnapshot threat, out float distanceToPlayer))
            {
                return;
            }

            PlayerSide desiredSafeSide = threat.DangerousSide == PlayerSide.Top ? PlayerSide.Bottom : PlayerSide.Top;
            if (playerController.CurrentSide != desiredSafeSide && distanceToPlayer <= scenario.FlipLeadDistance)
            {
                gameManager.DebugHandleTap();
            }
        }

        private static bool TryGetNearestUpcomingThreat(List<HazardDebugSnapshot> snapshots, float playerAnchorY, out HazardDebugSnapshot threat, out float distanceToPlayer)
        {
            threat = default;
            distanceToPlayer = float.MaxValue;
            bool found = false;
            for (int i = 0; i < snapshots.Count; i++)
            {
                HazardDebugSnapshot candidate = snapshots[i];
                if (candidate.ScoreAwarded)
                {
                    continue;
                }

                float candidateDistance = candidate.WorldY - playerAnchorY;
                if (candidateDistance < 0.05f)
                {
                    continue;
                }

                if (!found || candidateDistance < distanceToPlayer)
                {
                    found = true;
                    threat = candidate;
                    distanceToPlayer = candidateDistance;
                }
            }

            return found;
        }
    }
}
