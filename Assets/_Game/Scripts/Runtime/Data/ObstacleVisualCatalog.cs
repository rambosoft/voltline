using System;
using System.Collections.Generic;
using UnityEngine;
using Voltline.Utilities;

namespace Voltline.Data
{
    [Serializable]
    public sealed class ObstacleVisualDefinition
    {
        [SerializeField] private ObstacleFamily family;
        [SerializeField] private GameObject visualPrefab;
        [SerializeField] private Sprite mainSprite;
        [SerializeField] private Sprite secondarySprite;
        [SerializeField] private Sprite accentSprite;
        [SerializeField] private Sprite telegraphSprite;
        [SerializeField] private Material sharedMaterial;
        [SerializeField] private int baseSortingOrder = 3;
        [SerializeField] private Vector2 visualBoundsScale = new(0.42f, 0.84f);
        [SerializeField] private Vector2 telegraphBoundsScale = Vector2.zero;

        public ObstacleFamily Family => family;
        public GameObject VisualPrefab => visualPrefab;
        public Sprite MainSprite => mainSprite != null ? mainSprite : RuntimeSpriteFactory.WhiteSprite;
        public Sprite SecondarySprite => secondarySprite != null ? secondarySprite : RuntimeSpriteFactory.WhiteSprite;
        public Sprite AccentSprite => accentSprite != null ? accentSprite : RuntimeSpriteFactory.WhiteSprite;
        public Sprite TelegraphSprite => telegraphSprite != null ? telegraphSprite : RuntimeSpriteFactory.WhiteSprite;
        public Material SharedMaterial => sharedMaterial;
        public int BaseSortingOrder => baseSortingOrder;
        public Vector2 VisualBoundsScale => visualBoundsScale;
        public Vector2 TelegraphBoundsScale => telegraphBoundsScale;
        public bool UsesTelegraph => telegraphBoundsScale.x > 0f && telegraphBoundsScale.y > 0f;
    }

    public readonly struct ObstacleVisualProfile
    {
        public ObstacleVisualProfile(
            GameObject visualPrefab,
            Sprite mainSprite,
            Sprite secondarySprite,
            Sprite accentSprite,
            Sprite telegraphSprite,
            Material sharedMaterial,
            int baseSortingOrder,
            Vector2 visualBoundsScale,
            Vector2 telegraphBoundsScale)
        {
            VisualPrefab = visualPrefab;
            MainSprite = mainSprite;
            SecondarySprite = secondarySprite;
            AccentSprite = accentSprite;
            TelegraphSprite = telegraphSprite;
            SharedMaterial = sharedMaterial;
            BaseSortingOrder = baseSortingOrder;
            VisualBoundsScale = visualBoundsScale;
            TelegraphBoundsScale = telegraphBoundsScale;
        }

        public GameObject VisualPrefab { get; }
        public Sprite MainSprite { get; }
        public Sprite SecondarySprite { get; }
        public Sprite AccentSprite { get; }
        public Sprite TelegraphSprite { get; }
        public Material SharedMaterial { get; }
        public int BaseSortingOrder { get; }
        public Vector2 VisualBoundsScale { get; }
        public Vector2 TelegraphBoundsScale { get; }
        public float VisualHalfWidth => VisualBoundsScale.x * 0.5f;
        public float VisualHalfHeight => VisualBoundsScale.y * 0.5f;
        public bool UsesTelegraph => TelegraphBoundsScale.x > 0f && TelegraphBoundsScale.y > 0f;
    }

    [CreateAssetMenu(fileName = "CAT_ObstacleVisualCatalog_Main", menuName = "Voltline/Config/Obstacle Visual Catalog")]
    public sealed class ObstacleVisualCatalog : ScriptableObject
    {
        [SerializeField] private List<ObstacleVisualDefinition> entries = new();

        public IReadOnlyList<ObstacleVisualDefinition> Entries => entries;

        public bool TryGetProfile(ObstacleFamily family, out ObstacleVisualProfile profile)
        {
            for (int i = 0; i < entries.Count; i++)
            {
                ObstacleVisualDefinition entry = entries[i];
                if (entry != null && entry.Family == family)
                {
                    profile = new ObstacleVisualProfile(
                        entry.VisualPrefab,
                        entry.MainSprite,
                        entry.SecondarySprite,
                        entry.AccentSprite,
                        entry.TelegraphSprite,
                        entry.SharedMaterial,
                        entry.BaseSortingOrder,
                        entry.VisualBoundsScale,
                        entry.TelegraphBoundsScale);
                    return true;
                }
            }

            profile = default;
            return false;
        }

        public ObstacleVisualProfile GetRequiredProfile(ObstacleFamily family)
        {
            if (TryGetProfile(family, out ObstacleVisualProfile profile))
            {
                return profile;
            }

            throw new InvalidOperationException($"Missing obstacle visual profile for family '{family}'.");
        }
    }
}
