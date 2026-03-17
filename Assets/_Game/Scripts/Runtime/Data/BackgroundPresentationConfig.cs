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
        SkylineSilhouette = 4,
        WindowLights = 5,
        UtilityInfrastructure = 6,
        AtmosphereHaze = 7,
        EnergyStreaks = 8,
    }

    [Serializable]
    public sealed class BackgroundLayerDistrictVariantDefinition
    {
        [SerializeField] private int minimumRestoredDistrictCount = 1;
        [SerializeField] private int minimumScoreThreshold = -1;
        [SerializeField] private Sprite sprite;
        [SerializeField] private Vector2 contentFill = Vector2.one;
        [SerializeField, Range(0.5f, 1.5f)] private float alphaMultiplier = 1f;
        [SerializeField] private Color tintColor = Color.white;
        [SerializeField, Range(0f, 1f)] private float tintStrength;

        public int MinimumRestoredDistrictCount => minimumRestoredDistrictCount;
        public int MinimumScoreThreshold => minimumScoreThreshold;
        public Sprite Sprite => sprite;
        public Vector2 ContentFill => contentFill;
        public float AlphaMultiplier => alphaMultiplier;
        public Color TintColor => tintColor;
        public float TintStrength => tintStrength;
    }

    [Serializable]
    public sealed class BackgroundLayerDefinition
    {
        [SerializeField] private string layerId = "layer.far";
        [SerializeField] private BackgroundLayerColorRole colorRole = BackgroundLayerColorRole.BackgroundBlend;
        [SerializeField] private Sprite sprite;
        [SerializeField] private Vector2 size = new(7.5f, 10f);
        [SerializeField] private Vector2 contentFill = Vector2.one;
        [SerializeField] private List<BackgroundLayerDistrictVariantDefinition> districtVariants = new();
        [SerializeField] private Vector2 anchorOffset = new(-2.2f, 0f);
        [SerializeField] private bool enforceLaneQuietZone = true;
        [SerializeField, Range(0.01f, 0.18f)] private float alpha = 0.08f;
        [SerializeField] private float verticalTravelMultiplier = 0.14f;
        [SerializeField] private float verticalLoopDistance = 7f;
        [SerializeField] private float velocityResponseMultiplier = 0.06f;
        [SerializeField] private float horizontalOscillationAmplitude = 0.2f;
        [SerializeField] private float horizontalOscillationFrequency = 0.28f;
        [SerializeField] private int sortingOrder = -10;

        public string LayerId => layerId;
        public BackgroundLayerColorRole ColorRole => colorRole;
        public Sprite Sprite => sprite;
        public Vector2 Size => size;
        public Vector2 ContentFill => contentFill;
        public IReadOnlyList<BackgroundLayerDistrictVariantDefinition> DistrictVariants => districtVariants;
        public Vector2 AnchorOffset => anchorOffset;
        public bool EnforceLaneQuietZone => enforceLaneQuietZone;
        public float Alpha => alpha;
        public float VerticalTravelMultiplier => verticalTravelMultiplier;
        public float VerticalLoopDistance => verticalLoopDistance;
        public float VelocityResponseMultiplier => velocityResponseMultiplier;
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
