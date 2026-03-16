using System;
using System.Collections.Generic;
using UnityEngine;

namespace Voltline.Data
{
    public enum BackgroundLayerColorRole
    {
        BackgroundBlend = 0,
        LineGlow = 1,
        Accent = 2,
        Milestone = 3,
    }

    [Serializable]
    public sealed class BackgroundLayerDefinition
    {
        [SerializeField] private string layerId = "layer.far";
        [SerializeField] private BackgroundLayerColorRole colorRole = BackgroundLayerColorRole.BackgroundBlend;
        [SerializeField] private Vector2 size = new(7.5f, 10f);
        [SerializeField] private Vector2 anchorOffset = new(-2.2f, 0f);
        [SerializeField, Range(0.01f, 0.18f)] private float alpha = 0.08f;
        [SerializeField] private float verticalTravelMultiplier = 0.14f;
        [SerializeField] private float verticalLoopDistance = 7f;
        [SerializeField] private float horizontalOscillationAmplitude = 0.2f;
        [SerializeField] private float horizontalOscillationFrequency = 0.28f;
        [SerializeField] private int sortingOrder = -10;

        public string LayerId => layerId;
        public BackgroundLayerColorRole ColorRole => colorRole;
        public Vector2 Size => size;
        public Vector2 AnchorOffset => anchorOffset;
        public float Alpha => alpha;
        public float VerticalTravelMultiplier => verticalTravelMultiplier;
        public float VerticalLoopDistance => verticalLoopDistance;
        public float HorizontalOscillationAmplitude => horizontalOscillationAmplitude;
        public float HorizontalOscillationFrequency => horizontalOscillationFrequency;
        public int SortingOrder => sortingOrder;
    }

    [CreateAssetMenu(fileName = "CFG_BackgroundPresentation_Default", menuName = "Voltline/Config/Background Presentation")]
    public sealed class BackgroundPresentationConfig : ScriptableObject
    {
        [SerializeField] private int maxRuntimeSpriteCount = 3;
        [SerializeField] private int maxExpectedDrawCalls = 3;
        [SerializeField] private float laneQuietZoneHalfWidth = 1.25f;
        [SerializeField, Range(0.04f, 0.2f)] private float maximumAllowedLayerAlpha = 0.18f;
        [SerializeField] private List<BackgroundLayerDefinition> layers = new();

        public int MaxRuntimeSpriteCount => maxRuntimeSpriteCount;
        public int MaxExpectedDrawCalls => maxExpectedDrawCalls;
        public float LaneQuietZoneHalfWidth => laneQuietZoneHalfWidth;
        public float MaximumAllowedLayerAlpha => maximumAllowedLayerAlpha;
        public IReadOnlyList<BackgroundLayerDefinition> Layers => layers;
    }
}
