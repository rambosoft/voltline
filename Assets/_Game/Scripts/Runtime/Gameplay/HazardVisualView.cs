using UnityEngine;
using Voltline.Data;

namespace Voltline.Gameplay
{
    public readonly struct SpriteLayerState
    {
        public SpriteLayerState(bool enabled, Vector3 localPosition, Vector2 scale, Color color, float rotationDegrees)
        {
            Enabled = enabled;
            LocalPosition = localPosition;
            Scale = scale;
            Color = color;
            RotationDegrees = rotationDegrees;
        }

        public bool Enabled { get; }
        public Vector3 LocalPosition { get; }
        public Vector2 Scale { get; }
        public Color Color { get; }
        public float RotationDegrees { get; }

        public static SpriteLayerState Hidden => new(false, Vector3.zero, Vector2.one, Color.clear, 0f);
    }

    public readonly struct HazardVisualState
    {
        public HazardVisualState(SpriteLayerState main, SpriteLayerState secondary, SpriteLayerState accent, SpriteLayerState telegraph)
        {
            Main = main;
            Secondary = secondary;
            Accent = accent;
            Telegraph = telegraph;
        }

        public SpriteLayerState Main { get; }
        public SpriteLayerState Secondary { get; }
        public SpriteLayerState Accent { get; }
        public SpriteLayerState Telegraph { get; }
    }

    public sealed class HazardVisualView : MonoBehaviour
    {
        private ObstacleVisualProfile profile;
        private Transform modelRoot;
        private SpriteRenderer mainRenderer;
        private SpriteRenderer secondaryRenderer;
        private SpriteRenderer accentRenderer;
        private SpriteRenderer telegraphRenderer;

        public bool IsInitialized => modelRoot != null && mainRenderer != null && secondaryRenderer != null && accentRenderer != null && telegraphRenderer != null;

        public void Initialize(Transform parent, ObstacleVisualProfile visualProfile)
        {
            profile = visualProfile;
            transform.SetParent(parent, false);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;

            EnsureHierarchy();
            ApplyStaticConfiguration();
        }

        public void Apply(HazardVisualState state)
        {
            if (!IsInitialized)
            {
                return;
            }

            ApplyLayer(mainRenderer, state.Main);
            ApplyLayer(secondaryRenderer, state.Secondary);
            ApplyLayer(accentRenderer, state.Accent);
            ApplyLayer(telegraphRenderer, state.Telegraph);
        }

        private void EnsureHierarchy()
        {
            if (modelRoot != null)
            {
                return;
            }

            if (profile.VisualPrefab != null)
            {
                modelRoot = Instantiate(profile.VisualPrefab, transform).transform;
                modelRoot.name = profile.VisualPrefab.name;
            }
            else
            {
                modelRoot = new GameObject("HazardVisual").transform;
                modelRoot.SetParent(transform, false);
            }

            mainRenderer = FindOrCreateRenderer("Main", profile.BaseSortingOrder + 0, profile.MainSprite);
            secondaryRenderer = FindOrCreateRenderer("Secondary", profile.BaseSortingOrder + 1, profile.SecondarySprite);
            accentRenderer = FindOrCreateRenderer("Accent", profile.BaseSortingOrder + 2, profile.AccentSprite);
            telegraphRenderer = FindOrCreateRenderer("Telegraph", profile.BaseSortingOrder - 1, profile.TelegraphSprite);
        }

        private SpriteRenderer FindOrCreateRenderer(string name, int sortingOrder, Sprite defaultSprite)
        {
            SpriteRenderer renderer = null;
            SpriteRenderer[] existing = modelRoot.GetComponentsInChildren<SpriteRenderer>(true);
            for (int i = 0; i < existing.Length; i++)
            {
                if (existing[i] != null && existing[i].name == name)
                {
                    renderer = existing[i];
                    break;
                }
            }

            if (renderer == null)
            {
                GameObject child = new(name);
                child.transform.SetParent(modelRoot, false);
                renderer = child.AddComponent<SpriteRenderer>();
            }

            if (renderer.sprite == null)
            {
                renderer.sprite = defaultSprite;
            }

            if (profile.SharedMaterial != null)
            {
                renderer.sharedMaterial = profile.SharedMaterial;
            }

            if (profile.VisualPrefab == null)
            {
                renderer.sortingOrder = sortingOrder;
            }

            return renderer;
        }

        private void ApplyStaticConfiguration()
        {
            if (profile.SharedMaterial == null)
            {
                return;
            }

            mainRenderer.sharedMaterial = profile.SharedMaterial;
            secondaryRenderer.sharedMaterial = profile.SharedMaterial;
            accentRenderer.sharedMaterial = profile.SharedMaterial;
            telegraphRenderer.sharedMaterial = profile.SharedMaterial;
        }

        private static void ApplyLayer(SpriteRenderer renderer, SpriteLayerState state)
        {
            if (renderer == null)
            {
                return;
            }

            renderer.enabled = state.Enabled;
            if (!state.Enabled)
            {
                return;
            }

            Transform rendererTransform = renderer.transform;
            rendererTransform.localPosition = state.LocalPosition;
            rendererTransform.localScale = new Vector3(state.Scale.x, state.Scale.y, 1f);
            rendererTransform.localRotation = Quaternion.Euler(0f, 0f, state.RotationDegrees);
            renderer.color = state.Color;
        }
    }
}
