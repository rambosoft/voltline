using System.Collections.Generic;
using UnityEngine;
using Voltline.Data;

namespace Voltline.Gameplay
{
    public readonly struct PlayerVisualState
    {
        public PlayerVisualState(Vector3 worldPosition, Color tint, float zRotation)
        {
            WorldPosition = worldPosition;
            Tint = tint;
            ZRotation = zRotation;
        }

        public Vector3 WorldPosition { get; }
        public Color Tint { get; }
        public float ZRotation { get; }
    }

    public sealed class PlayerVisualView : MonoBehaviour
    {
        private PlayerVisualConfig config;
        private Transform modelRoot;
        private readonly List<SpriteRenderer> renderers = new();

        public bool IsInitialized => config != null && modelRoot != null && renderers.Count > 0;

        public void Initialize(Transform parent, PlayerVisualConfig visualConfig)
        {
            config = visualConfig;
            transform.SetParent(parent, false);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;

            EnsureModelHierarchy();
            ApplyStaticConfiguration();
        }

        public void Apply(PlayerVisualState state)
        {
            if (!IsInitialized)
            {
                return;
            }

            transform.position = state.WorldPosition;
            transform.rotation = Quaternion.Euler(0f, 0f, state.ZRotation + config.BaseRotationOffset);
            modelRoot.localPosition = new Vector3(config.LocalOffset.x, config.LocalOffset.y, 0f);
            modelRoot.localScale = new Vector3(config.VisibleBoundsScale.x, config.VisibleBoundsScale.y, 1f);

            Color tint = config.ApplyRuntimeTint ? state.Tint : config.FallbackColor;
            for (int i = 0; i < renderers.Count; i++)
            {
                if (renderers[i] != null)
                {
                    renderers[i].color = tint;
                }
            }
        }

        private void EnsureModelHierarchy()
        {
            if (modelRoot != null)
            {
                return;
            }

            if (config != null && config.VisualPrefab != null)
            {
                modelRoot = Instantiate(config.VisualPrefab, transform).transform;
                modelRoot.name = config.VisualPrefab.name;
            }
            else
            {
                modelRoot = new GameObject("PlayerSprite").transform;
                modelRoot.SetParent(transform, false);
                SpriteRenderer renderer = modelRoot.gameObject.AddComponent<SpriteRenderer>();
                renderer.sprite = config != null ? config.FallbackSprite : null;
            }

            renderers.Clear();
            renderers.AddRange(modelRoot.GetComponentsInChildren<SpriteRenderer>(true));
            if (renderers.Count == 0)
            {
                SpriteRenderer fallbackRenderer = modelRoot.gameObject.AddComponent<SpriteRenderer>();
                fallbackRenderer.sprite = config != null ? config.FallbackSprite : null;
                renderers.Add(fallbackRenderer);
            }
        }

        private void ApplyStaticConfiguration()
        {
            for (int i = 0; i < renderers.Count; i++)
            {
                SpriteRenderer renderer = renderers[i];
                if (renderer == null)
                {
                    continue;
                }

                if (renderer.sprite == null)
                {
                    renderer.sprite = config.FallbackSprite;
                }

                if (config.OverrideMaterial != null)
                {
                    renderer.sharedMaterial = config.OverrideMaterial;
                }

                if (renderers.Count == 1)
                {
                    renderer.sortingOrder = config.SortingOrder;
                }
            }
        }
    }
}
