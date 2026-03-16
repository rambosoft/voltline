using UnityEngine;
using Voltline.Data;
using Voltline.Utilities;

namespace Voltline.Gameplay
{
    public sealed class BackgroundPresentationController : MonoBehaviour
    {
        private sealed class RuntimeLayer
        {
            public BackgroundLayerDefinition Definition;
            public Transform Transform;
            public SpriteRenderer Renderer;
        }

        private readonly System.Collections.Generic.List<RuntimeLayer> runtimeLayers = new();

        private BackgroundPresentationConfig baseConfig;
        private BackgroundPresentationConfig activeConfig;
        private ThemeConfig currentTheme;
        private ThemeConfig targetTheme;
        private Camera targetCamera;
        private TrackManager trackManager;
        private Transform root;
        private float themeTransitionDuration;
        private float themeTransitionElapsed;
        private float lastTravelDistance;

        public int RuntimeLayerCount => runtimeLayers.Count;
        public int ConfiguredSpriteBudget => activeConfig != null ? activeConfig.MaxRuntimeSpriteCount : 0;
        public BackgroundPresentationConfig ActiveConfig => activeConfig;

        public void Initialize(
            BackgroundPresentationConfig defaultConfig,
            ThemeConfig activeTheme,
            Camera gameplayCamera,
            TrackManager track)
        {
            baseConfig = defaultConfig;
            targetCamera = gameplayCamera;
            trackManager = track;
            currentTheme = activeTheme;
            targetTheme = activeTheme;
            activeConfig = activeTheme != null ? activeTheme.ResolveBackgroundPresentation(defaultConfig) : defaultConfig;
            EnsureRoot();
            RebuildLayers(activeConfig);
            ApplyTheme(activeTheme, activeConfig, 0f);
        }

        public void ApplyTheme(ThemeConfig theme, BackgroundPresentationConfig config, float durationSeconds)
        {
            targetTheme = theme;
            currentTheme ??= theme;
            themeTransitionDuration = Mathf.Max(0f, durationSeconds);
            themeTransitionElapsed = 0f;

            BackgroundPresentationConfig resolvedConfig = config != null ? config : baseConfig;
            if (resolvedConfig != activeConfig)
            {
                activeConfig = resolvedConfig;
                RebuildLayers(activeConfig);
            }

            if (themeTransitionDuration <= 0f)
            {
                currentTheme = theme;
            }

            ApplyVisualState();
        }

        private void LateUpdate()
        {
            if (targetCamera == null || activeConfig == null)
            {
                return;
            }

            if (themeTransitionDuration > 0f && themeTransitionElapsed < themeTransitionDuration)
            {
                themeTransitionElapsed += Time.deltaTime;
                if (themeTransitionElapsed >= themeTransitionDuration)
                {
                    currentTheme = targetTheme;
                }
            }

            ApplyVisualState();
            AnimateLayers();
        }

        private void EnsureRoot()
        {
            root ??= new GameObject("BackgroundPresentationRoot").transform;
            root.SetParent(transform, false);
        }

        private void RebuildLayers(BackgroundPresentationConfig config)
        {
            for (int i = 0; i < runtimeLayers.Count; i++)
            {
                if (runtimeLayers[i].Transform != null)
                {
                    Destroy(runtimeLayers[i].Transform.gameObject);
                }
            }

            runtimeLayers.Clear();
            if (config == null)
            {
                return;
            }

            int maxCount = Mathf.Min(config.MaxRuntimeSpriteCount, config.Layers.Count);
            for (int i = 0; i < maxCount; i++)
            {
                BackgroundLayerDefinition definition = config.Layers[i];
                if (definition == null)
                {
                    continue;
                }

                GameObject layerObject = new($"BackgroundLayer_{i}_{definition.LayerId}");
                layerObject.transform.SetParent(root, false);
                SpriteRenderer renderer = layerObject.AddComponent<SpriteRenderer>();
                renderer.sprite = RuntimeSpriteFactory.WhiteSprite;
                renderer.sortingOrder = definition.SortingOrder;
                renderer.drawMode = SpriteDrawMode.Sliced;
                renderer.size = definition.Size;

                runtimeLayers.Add(new RuntimeLayer
                {
                    Definition = definition,
                    Transform = layerObject.transform,
                    Renderer = renderer,
                });
            }
        }

        private void ApplyVisualState()
        {
            ThemeConfig fromTheme = currentTheme ?? targetTheme;
            ThemeConfig toTheme = targetTheme ?? currentTheme;
            float t = themeTransitionDuration <= 0f ? 1f : Mathf.Clamp01(themeTransitionElapsed / themeTransitionDuration);
            Color backgroundColor = Color.Lerp(GetBackgroundColor(fromTheme), GetBackgroundColor(toTheme), t);
            targetCamera.backgroundColor = backgroundColor;

            for (int i = 0; i < runtimeLayers.Count; i++)
            {
                RuntimeLayer layer = runtimeLayers[i];
                Color fromColor = ResolveLayerColor(fromTheme, layer.Definition.ColorRole);
                Color toColor = ResolveLayerColor(toTheme, layer.Definition.ColorRole);
                Color finalColor = Color.Lerp(fromColor, toColor, t);
                finalColor.a = Mathf.Min(layer.Definition.Alpha, activeConfig.MaximumAllowedLayerAlpha);
                layer.Renderer.color = finalColor;
            }
        }

        private void AnimateLayers()
        {
            float travelDistance = trackManager != null ? trackManager.TravelDistance : 0f;
            float travelDelta = travelDistance - lastTravelDistance;
            lastTravelDistance = travelDistance;
            float cameraY = targetCamera.transform.position.y;
            float cameraZ = targetCamera.transform.position.z + 20f;

            for (int i = 0; i < runtimeLayers.Count; i++)
            {
                RuntimeLayer layer = runtimeLayers[i];
                BackgroundLayerDefinition definition = layer.Definition;
                float loopDistance = Mathf.Max(1f, definition.VerticalLoopDistance);
                float verticalOffset = Mathf.Repeat(travelDistance * definition.VerticalTravelMultiplier, loopDistance * 2f) - loopDistance;
                float oscillation = Mathf.Sin((Time.time * definition.HorizontalOscillationFrequency) + i) * definition.HorizontalOscillationAmplitude;
                float anchorX = definition.AnchorOffset.x;
                if (Mathf.Abs(anchorX) < activeConfig.LaneQuietZoneHalfWidth)
                {
                    anchorX = Mathf.Sign(anchorX == 0f ? (i % 2 == 0 ? -1f : 1f) : anchorX) * activeConfig.LaneQuietZoneHalfWidth;
                }

                layer.Transform.position = new Vector3(anchorX + oscillation, cameraY + definition.AnchorOffset.y + verticalOffset + travelDelta * 0.15f, cameraZ + (i * 0.01f));
            }
        }

        private static Color GetBackgroundColor(ThemeConfig theme)
        {
            if (theme == null)
            {
                return new Color(0.03f, 0.04f, 0.08f, 1f);
            }

            return Color.Lerp(theme.BackgroundTopColor, theme.BackgroundBottomColor, 0.5f);
        }

        private static Color ResolveLayerColor(ThemeConfig theme, BackgroundLayerColorRole role)
        {
            if (theme == null)
            {
                return Color.white;
            }

            return role switch
            {
                BackgroundLayerColorRole.LineGlow => theme.LineGlowColor,
                BackgroundLayerColorRole.Accent => theme.PlayerAccentColor,
                BackgroundLayerColorRole.Milestone => theme.MilestoneColor,
                _ => Color.Lerp(theme.BackgroundTopColor, theme.BackgroundBottomColor, 0.5f),
            };
        }
    }
}
