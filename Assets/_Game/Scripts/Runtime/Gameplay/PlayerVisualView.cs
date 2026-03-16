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
        private readonly Dictionary<SpriteRenderer, Sprite> defaultSprites = new();
        private Vector2 sourceVisualSize = Vector2.one;
        private PlayerVisualPresentationStateId currentPresentationState = PlayerVisualPresentationStateId.Idle;
        private float presentationStateElapsed;
        private bool presentationStateLocked;
        private float accumulatedEffectTime;
        private PlayerVisualState lastAppliedState;
        private bool hasLastAppliedState;

        public bool IsInitialized => config != null && modelRoot != null && renderers.Count > 0;
        public PlayerVisualPresentationStateId CurrentPresentationState => currentPresentationState;

        public void Initialize(Transform parent, PlayerVisualConfig visualConfig)
        {
            config = visualConfig;
            transform.SetParent(parent, false);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;

            EnsureModelHierarchy();
            ApplyStaticConfiguration();
            RecalculateSourceVisualSize();
            SetPresentationState(PlayerVisualPresentationStateId.Idle, true);
        }

        public void ResetPresentationState()
        {
            SetPresentationState(PlayerVisualPresentationStateId.Idle, true);
        }

        public void PlayPresentationState(PlayerVisualPresentationStateId stateId)
        {
            SetPresentationState(stateId, stateId == PlayerVisualPresentationStateId.Death);
        }

        public void Advance(float deltaTime)
        {
            if (!IsInitialized)
            {
                return;
            }

            float safeDelta = Mathf.Max(0f, deltaTime);
            accumulatedEffectTime += safeDelta;
            presentationStateElapsed += safeDelta;

            if (presentationStateLocked)
            {
                return;
            }

            PlayerVisualStateDefinition definition = config.ResolveStateDefinition(currentPresentationState);
            if (definition.DurationSeconds > 0f && presentationStateElapsed >= definition.DurationSeconds)
            {
                SetPresentationState(PlayerVisualPresentationStateId.Idle, false);
            }
        }

        public void Apply(PlayerVisualState state)
        {
            if (!IsInitialized)
            {
                return;
            }

            lastAppliedState = state;
            hasLastAppliedState = true;

            PlayerVisualStateDefinition definition = config.ResolveStateDefinition(currentPresentationState);
            Vector2 targetVisibleBounds = config.ResolveTargetVisibleBounds(definition);
            float widthScale = ResolveNormalizedScale(targetVisibleBounds.x, sourceVisualSize.x);
            float heightScale = ResolveNormalizedScale(targetVisibleBounds.y, sourceVisualSize.y);
            float effectRotation = definition.RotationSpeedDegreesPerSecond * accumulatedEffectTime;
            float pulse = definition.PulseAmplitude > 0f && definition.PulseFrequency > 0f
                ? Mathf.Abs(Mathf.Sin(accumulatedEffectTime * definition.PulseFrequency * Mathf.PI * 2f)) * definition.PulseAmplitude
                : 0f;

            transform.position = state.WorldPosition;
            transform.rotation = Quaternion.Euler(0f, 0f, state.ZRotation + config.BaseRotationOffset + effectRotation);
            modelRoot.localPosition = new Vector3(config.LocalOffset.x, config.LocalOffset.y, 0f);
            modelRoot.localScale = new Vector3(widthScale, heightScale, 1f);

            Color tint = config.ApplyRuntimeTint ? state.Tint : config.FallbackColor;
            float brightness = 1f - (pulse * 0.35f);
            Color effectColor = new(
                tint.r * brightness,
                tint.g * brightness,
                tint.b * brightness,
                tint.a);

            for (int i = 0; i < renderers.Count; i++)
            {
                SpriteRenderer renderer = renderers[i];
                if (renderer == null)
                {
                    continue;
                }

                renderer.color = effectColor;
            }
        }

        private void SetPresentationState(PlayerVisualPresentationStateId stateId, bool lockState)
        {
            currentPresentationState = stateId;
            presentationStateLocked = lockState;
            presentationStateElapsed = 0f;
            accumulatedEffectTime = 0f;

            if (modelRoot != null)
            {
                modelRoot.localScale = Vector3.one;
            }

            ApplyPresentationDefinition(config.ResolveStateDefinition(stateId));
            RecalculateSourceVisualSize();

            if (hasLastAppliedState)
            {
                Apply(lastAppliedState);
            }
        }

        private void ApplyPresentationDefinition(PlayerVisualStateDefinition definition)
        {
            Sprite resolvedSprite = definition.Sprite != null ? definition.Sprite : config.FallbackSprite;
            Material resolvedMaterial = definition.OverrideMaterial != null ? definition.OverrideMaterial : config.OverrideMaterial;

            for (int i = 0; i < renderers.Count; i++)
            {
                SpriteRenderer renderer = renderers[i];
                if (renderer == null)
                {
                    continue;
                }

                if (renderers.Count == 1)
                {
                    renderer.sprite = resolvedSprite;
                    renderer.sortingOrder = config.SortingOrder;
                }
                else if (definition.Sprite != null)
                {
                    renderer.sprite = resolvedSprite;
                }
                else if (defaultSprites.TryGetValue(renderer, out Sprite defaultSprite) && defaultSprite != null)
                {
                    renderer.sprite = defaultSprite;
                }

                if (resolvedMaterial != null)
                {
                    renderer.sharedMaterial = resolvedMaterial;
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

            defaultSprites.Clear();
            for (int i = 0; i < renderers.Count; i++)
            {
                if (renderers[i] != null)
                {
                    defaultSprites[renderers[i]] = renderers[i].sprite;
                }
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
                    defaultSprites[renderer] = renderer.sprite;
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

        private void RecalculateSourceVisualSize()
        {
            Vector2 min = new(float.PositiveInfinity, float.PositiveInfinity);
            Vector2 max = new(float.NegativeInfinity, float.NegativeInfinity);
            bool foundAnySprite = false;

            for (int i = 0; i < renderers.Count; i++)
            {
                SpriteRenderer renderer = renderers[i];
                if (renderer == null || renderer.sprite == null)
                {
                    continue;
                }

                Vector3 authoredScale = ResolveAuthoredRelativeScale(renderer.transform);
                Vector3 localCenter3 = modelRoot != null
                    ? modelRoot.InverseTransformPoint(renderer.transform.position)
                    : renderer.transform.localPosition;
                Bounds spriteBounds = renderer.sprite.bounds;
                Vector2 halfSize = new(
                    Mathf.Abs(spriteBounds.size.x * authoredScale.x) * 0.5f,
                    Mathf.Abs(spriteBounds.size.y * authoredScale.y) * 0.5f);
                Vector2 center = new(localCenter3.x, localCenter3.y);

                Vector2 rendererMin = center - halfSize;
                Vector2 rendererMax = center + halfSize;

                min = Vector2.Min(min, rendererMin);
                max = Vector2.Max(max, rendererMax);
                foundAnySprite = true;
            }

            if (!foundAnySprite)
            {
                sourceVisualSize = Vector2.one;
                return;
            }

            Vector2 size = max - min;
            sourceVisualSize = new Vector2(Mathf.Max(0.0001f, size.x), Mathf.Max(0.0001f, size.y));
        }

        private Vector3 ResolveAuthoredRelativeScale(Transform leaf)
        {
            Vector3 scale = Vector3.one;
            Transform current = leaf;
            while (current != null && current != modelRoot)
            {
                Vector3 localScale = current.localScale;
                scale = new Vector3(scale.x * localScale.x, scale.y * localScale.y, scale.z * localScale.z);
                current = current.parent;
            }

            return scale;
        }

        private static float ResolveNormalizedScale(float targetSize, float sourceSize)
        {
            return sourceSize <= 0.0001f ? targetSize : targetSize / sourceSize;
        }
    }
}
