using System;
using System.Collections.Generic;
using UnityEngine;
using Voltline.Utilities;

namespace Voltline.Data
{
    public enum PlayerVisualPresentationStateId
    {
        Idle = 0,
        Flip = 1,
        NearMiss = 2,
        Score = 3,
        Milestone = 4,
        Death = 5,
    }

    [Serializable]
    public sealed class PlayerVisualStateDefinition
    {
        [SerializeField] private PlayerVisualPresentationStateId stateId = PlayerVisualPresentationStateId.Idle;
        [SerializeField] private Sprite sprite;
        [SerializeField] private Material overrideMaterial;
        [SerializeField] private float durationSeconds;
        [SerializeField] private Vector2 visibleBounds = new(0.68f, 0.68f);
        [SerializeField] private float rotationSpeedDegreesPerSecond;
        [SerializeField] private float pulseAmplitude;
        [SerializeField] private float pulseFrequency = 6f;

        public PlayerVisualStateDefinition()
        {
        }

        public PlayerVisualStateDefinition(
            PlayerVisualPresentationStateId resolvedStateId,
            float resolvedDurationSeconds,
            Vector2 resolvedVisibleBounds,
            float resolvedRotationSpeedDegreesPerSecond,
            float resolvedPulseAmplitude,
            float resolvedPulseFrequency)
        {
            stateId = resolvedStateId;
            durationSeconds = resolvedDurationSeconds;
            visibleBounds = resolvedVisibleBounds;
            rotationSpeedDegreesPerSecond = resolvedRotationSpeedDegreesPerSecond;
            pulseAmplitude = resolvedPulseAmplitude;
            pulseFrequency = resolvedPulseFrequency;
        }

        public PlayerVisualPresentationStateId StateId => stateId;
        public Sprite Sprite => sprite;
        public Material OverrideMaterial => overrideMaterial;
        public float DurationSeconds => durationSeconds;
        public Vector2 VisibleBounds => visibleBounds;
        public float RotationSpeedDegreesPerSecond => rotationSpeedDegreesPerSecond;
        public float PulseAmplitude => pulseAmplitude;
        public float PulseFrequency => pulseFrequency;
    }

    [CreateAssetMenu(fileName = "CFG_PlayerVisual_Default", menuName = "Voltline/Config/Player Visual")]
    public sealed class PlayerVisualConfig : ScriptableObject
    {
        [SerializeField] private GameObject visualPrefab;
        [SerializeField] private Sprite fallbackSprite;
        [SerializeField] private Material overrideMaterial;
        [SerializeField] private Color fallbackColor = Color.white;
        [SerializeField] private bool applyRuntimeTint = true;
        [SerializeField] private Vector2 visibleBoundsScale = new(0.68f, 0.68f);
        [SerializeField] private Vector2 localOffset = Vector2.zero;
        [SerializeField] private float baseRotationOffset;
        [SerializeField] private int sortingOrder = 4;
        [SerializeField] private Vector2 menuPreviewSize = new(88f, 88f);
        [SerializeField] private Vector2 menuPreviewOffset = new(84f, 116f);
        [SerializeField] private List<PlayerVisualStateDefinition> stateDefinitions = new();

        public GameObject VisualPrefab => visualPrefab;
        public Sprite FallbackSprite => fallbackSprite != null ? fallbackSprite : RuntimeSpriteFactory.WhiteSprite;
        public Material OverrideMaterial => overrideMaterial;
        public Color FallbackColor => fallbackColor;
        public bool ApplyRuntimeTint => applyRuntimeTint;
        public Vector2 VisibleBoundsScale => visibleBoundsScale;
        public Vector2 LocalOffset => localOffset;
        public float BaseRotationOffset => baseRotationOffset;
        public int SortingOrder => sortingOrder;
        public Vector2 MenuPreviewSize => menuPreviewSize;
        public Vector2 MenuPreviewOffset => menuPreviewOffset;
        public IReadOnlyList<PlayerVisualStateDefinition> StateDefinitions => stateDefinitions;
        public float VisibleHalfWidth => visibleBoundsScale.x * 0.5f;
        public float VisibleHalfHeight => visibleBoundsScale.y * 0.5f;

        public PlayerVisualStateDefinition ResolveStateDefinition(PlayerVisualPresentationStateId stateId)
        {
            if (stateDefinitions != null)
            {
                for (int i = 0; i < stateDefinitions.Count; i++)
                {
                    if (stateDefinitions[i] != null && stateDefinitions[i].StateId == stateId)
                    {
                        return stateDefinitions[i];
                    }
                }
            }

            return CreateFallbackStateDefinition(stateId, visibleBoundsScale);
        }

        public Vector2 ResolveTargetVisibleBounds(PlayerVisualStateDefinition definition)
        {
            if (definition == null)
            {
                return visibleBoundsScale;
            }

            return definition.VisibleBounds.x > 0f && definition.VisibleBounds.y > 0f
                ? definition.VisibleBounds
                : visibleBoundsScale;
        }

        private static PlayerVisualStateDefinition CreateFallbackStateDefinition(PlayerVisualPresentationStateId stateId, Vector2 fallbackVisibleBounds)
        {
            return stateId switch
            {
                PlayerVisualPresentationStateId.Flip => new PlayerVisualStateDefinition(stateId, 0.16f, fallbackVisibleBounds, 0f, 0.18f, 8f),
                PlayerVisualPresentationStateId.NearMiss => new PlayerVisualStateDefinition(stateId, 0.18f, fallbackVisibleBounds, 0f, 0.14f, 7f),
                PlayerVisualPresentationStateId.Score => new PlayerVisualStateDefinition(stateId, 0.12f, fallbackVisibleBounds, 0f, 0f, 0f),
                PlayerVisualPresentationStateId.Milestone => new PlayerVisualStateDefinition(stateId, 0.24f, fallbackVisibleBounds, 10f, 0f, 0f),
                PlayerVisualPresentationStateId.Death => new PlayerVisualStateDefinition(stateId, 0f, fallbackVisibleBounds, 0f, 0f, 0f),
                _ => new PlayerVisualStateDefinition(stateId, 0f, fallbackVisibleBounds, 5f, 0f, 0f),
            };
        }
    }
}
