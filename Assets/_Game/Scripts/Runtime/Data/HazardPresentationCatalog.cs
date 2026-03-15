using System;
using System.Collections.Generic;
using UnityEngine;

namespace Voltline.Data
{
    [Serializable]
    public sealed class HazardPresentationDefinition
    {
        [SerializeField] private ObstacleFamily family;
        [SerializeField] private Vector2 visualBoundsScale = new(0.42f, 0.84f);
        [SerializeField] private Vector2 telegraphBoundsScale = Vector2.zero;
        [SerializeField] private Vector2 collisionBoundsScale = new(0.34f, 0.84f);
        [SerializeField] private float minimumReadableGapPadding = 0.8f;

        public ObstacleFamily Family => family;
        public Vector2 VisualBoundsScale => visualBoundsScale;
        public Vector2 TelegraphBoundsScale => telegraphBoundsScale;
        public Vector2 CollisionBoundsScale => collisionBoundsScale;
        public float MinimumReadableGapPadding => minimumReadableGapPadding;
        public bool UsesTelegraph => telegraphBoundsScale.x > 0f && telegraphBoundsScale.y > 0f;
    }

    public readonly struct HazardLayoutProfile
    {
        public HazardLayoutProfile(Vector2 visualBoundsScale, Vector2 telegraphBoundsScale, Vector2 collisionBoundsScale, float minimumReadableGapPadding)
        {
            VisualBoundsScale = visualBoundsScale;
            TelegraphBoundsScale = telegraphBoundsScale;
            CollisionBoundsScale = collisionBoundsScale;
            MinimumReadableGapPadding = minimumReadableGapPadding;
        }

        public Vector2 VisualBoundsScale { get; }
        public Vector2 TelegraphBoundsScale { get; }
        public Vector2 CollisionBoundsScale { get; }
        public float MinimumReadableGapPadding { get; }
        public float VisualHalfWidth => VisualBoundsScale.x * 0.5f;
        public float VisualHalfHeight => VisualBoundsScale.y * 0.5f;
        public float CollisionHalfWidth => CollisionBoundsScale.x * 0.5f;
        public float CollisionHalfHeight => CollisionBoundsScale.y * 0.5f;
        public bool UsesTelegraph => TelegraphBoundsScale.x > 0f && TelegraphBoundsScale.y > 0f;
    }

    [CreateAssetMenu(fileName = "CAT_HazardPresentationCatalog_Main", menuName = "Voltline/Config/Hazard Presentation Catalog")]
    public sealed class HazardPresentationCatalog : ScriptableObject
    {
        [SerializeField] private List<HazardPresentationDefinition> entries = new();

        public IReadOnlyList<HazardPresentationDefinition> Entries => entries;

        public bool TryGetProfile(ObstacleFamily family, out HazardLayoutProfile profile)
        {
            for (int i = 0; i < entries.Count; i++)
            {
                HazardPresentationDefinition entry = entries[i];
                if (entry != null && entry.Family == family)
                {
                    profile = new HazardLayoutProfile(
                        entry.VisualBoundsScale,
                        entry.TelegraphBoundsScale,
                        entry.CollisionBoundsScale,
                        entry.MinimumReadableGapPadding);
                    return true;
                }
            }

            profile = default;
            return false;
        }

        public HazardLayoutProfile GetRequiredProfile(ObstacleFamily family)
        {
            if (TryGetProfile(family, out HazardLayoutProfile profile))
            {
                return profile;
            }

            throw new InvalidOperationException($"Missing hazard presentation profile for family '{family}'.");
        }

        public float GetRequiredHitDistanceSeparation(HazardLayoutProfile previous, HazardLayoutProfile next)
        {
            return previous.VisualHalfHeight
                + next.VisualHalfHeight
                + Mathf.Max(previous.MinimumReadableGapPadding, next.MinimumReadableGapPadding);
        }
    }
}
