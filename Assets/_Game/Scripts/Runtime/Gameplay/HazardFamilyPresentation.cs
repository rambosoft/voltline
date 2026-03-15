using UnityEngine;
using Voltline.Data;

namespace Voltline.Gameplay
{
    public readonly struct HazardLayoutProfile
    {
        public HazardLayoutProfile(Vector2 mainScale, Vector2 telegraphScale, Vector2 collisionScale)
        {
            MainScale = mainScale;
            TelegraphScale = telegraphScale;
            CollisionScale = collisionScale;
        }

        public Vector2 MainScale { get; }
        public Vector2 TelegraphScale { get; }
        public Vector2 CollisionScale { get; }
        public float VisualHalfWidth => MainScale.x * 0.5f;
        public float VisualHalfHeight => MainScale.y * 0.5f;
        public float CollisionHalfWidth => CollisionScale.x * 0.5f;
        public float CollisionHalfHeight => CollisionScale.y * 0.5f;
        public bool UsesTelegraph => TelegraphScale.x > 0f && TelegraphScale.y > 0f;
    }

    public static class HazardFamilyPresentation
    {
        public const float MinimumVisualGap = 0.8f;

        public static HazardLayoutProfile GetLayoutProfile(ObstacleFamily family)
        {
            return family switch
            {
                ObstacleFamily.RotatingCutters => new HazardLayoutProfile(new Vector2(0.58f, 1.06f), Vector2.zero, new Vector2(0.46f, 1.06f)),
                ObstacleFamily.ElectricGates => new HazardLayoutProfile(new Vector2(0.52f, 1.18f), new Vector2(0.92f, 1.62f), new Vector2(0.42f, 1.18f)),
                ObstacleFamily.BrokenLineGaps => new HazardLayoutProfile(new Vector2(0.62f, 0.92f), Vector2.zero, new Vector2(0.5f, 0.92f)),
                ObstacleFamily.SideBlockers => new HazardLayoutProfile(new Vector2(0.82f, 1.28f), Vector2.zero, new Vector2(0.68f, 1.28f)),
                _ => new HazardLayoutProfile(new Vector2(0.42f, 0.84f), Vector2.zero, new Vector2(0.34f, 0.84f)),
            };
        }

        public static float GetRequiredHitDistanceSeparation(float previousVisualHalfHeight, float nextVisualHalfHeight)
        {
            return previousVisualHalfHeight + nextVisualHalfHeight + MinimumVisualGap;
        }
    }
}
