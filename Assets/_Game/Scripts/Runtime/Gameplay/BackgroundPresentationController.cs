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
            public Sprite ActiveSprite;
            public Vector2 ActiveContentFill;
            public float ActiveAlphaMultiplier = 1f;
            public Color ActiveTintColor = Color.white;
            public float ActiveTintStrength;
        }

        private readonly System.Collections.Generic.List<RuntimeLayer> runtimeLayers = new();

        private BackgroundPresentationConfig baseConfig;
        private BackgroundPresentationConfig activeConfig;
        private ThemeConfig currentTheme;
        private ThemeConfig targetTheme;
        private WorldDistrictStateDefinition currentDistrictState;
        private WorldDistrictStateDefinition targetDistrictState;
        private Camera targetCamera;
        private TrackManager trackManager;
        private Transform root;
        private int currentScore;
        private int targetScore;
        private float themeTransitionDuration;
        private float themeTransitionElapsed;
        private float lastTravelDistance;
        private float milestoneFlashElapsed;
        private float milestoneFlashDuration;
        private float milestoneFlashStrength;
        private Color milestoneFlashColor;

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

        public void ApplyWorldDistrict(WorldDistrictStateDefinition districtState, float durationSeconds)
        {
            targetDistrictState = districtState;
            currentDistrictState ??= districtState;
            themeTransitionDuration = Mathf.Max(themeTransitionDuration, durationSeconds);
            themeTransitionElapsed = 0f;

            if (durationSeconds <= 0f)
            {
                currentDistrictState = districtState;
            }

            ApplyVisualState();
        }

        public void ApplyScore(int score)
        {
            targetScore = Mathf.Max(0, score);
            currentScore = targetScore;
            ApplyVisualState();
        }

        public void PlayMilestonePulse(Color flashColor, float flashStrength)
        {
            milestoneFlashColor = flashColor;
            milestoneFlashStrength = Mathf.Clamp01(flashStrength);
            milestoneFlashElapsed = 0f;
            milestoneFlashDuration = 0.32f;
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
                    currentDistrictState = targetDistrictState;
                }
            }

            if (milestoneFlashDuration > 0f)
            {
                milestoneFlashElapsed += Time.deltaTime;
                if (milestoneFlashElapsed >= milestoneFlashDuration)
                {
                    milestoneFlashDuration = 0f;
                    milestoneFlashElapsed = 0f;
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
                renderer.sortingOrder = definition.SortingOrder;

                RuntimeLayer runtimeLayer = new()
                {
                    Definition = definition,
                    Transform = layerObject.transform,
                    Renderer = renderer,
                    ActiveContentFill = definition.ContentFill,
                };

                ApplyResolvedLayerSprite(runtimeLayer, 0, 0);
                runtimeLayers.Add(runtimeLayer);
            }
        }

        private void ApplyVisualState()
        {
            ThemeConfig fromTheme = currentTheme ?? targetTheme;
            ThemeConfig toTheme = targetTheme ?? currentTheme;
            WorldDistrictStateDefinition fromDistrict = currentDistrictState ?? targetDistrictState;
            WorldDistrictStateDefinition toDistrict = targetDistrictState ?? currentDistrictState;
            float t = themeTransitionDuration <= 0f ? 1f : Mathf.Clamp01(themeTransitionElapsed / themeTransitionDuration);
            Color backgroundColor = Color.Lerp(GetBackgroundColor(fromTheme, fromDistrict), GetBackgroundColor(toTheme, toDistrict), t);
            backgroundColor = ApplyMilestoneFlash(backgroundColor);
            targetCamera.backgroundColor = backgroundColor;

            int resolvedScore = Mathf.Max(currentScore, targetScore);
            int restoredDistrictCount = ResolveRestoredDistrictCount(toTheme ?? fromTheme, toDistrict ?? fromDistrict);

            for (int i = 0; i < runtimeLayers.Count; i++)
            {
                RuntimeLayer layer = runtimeLayers[i];
                ApplyResolvedLayerSprite(layer, restoredDistrictCount, resolvedScore);
                Color fromColor = ResolveLayerColor(fromTheme, fromDistrict, layer.Definition.ColorRole);
                Color toColor = ResolveLayerColor(toTheme, toDistrict, layer.Definition.ColorRole);
                Color finalColor = Color.Lerp(fromColor, toColor, t);
                finalColor = ApplyVariantTint(finalColor, layer.ActiveTintColor, layer.ActiveTintStrength);
                finalColor = ApplyMilestoneFlash(finalColor);
                float districtAlphaMultiplier = Mathf.Lerp(GetBackgroundAlphaMultiplier(fromDistrict), GetBackgroundAlphaMultiplier(toDistrict), t);
                finalColor.a = Mathf.Min(layer.Definition.Alpha * layer.ActiveAlphaMultiplier * districtAlphaMultiplier, activeConfig.MaximumAllowedLayerAlpha);
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
                if (definition.EnforceLaneQuietZone && Mathf.Abs(anchorX) < activeConfig.LaneQuietZoneHalfWidth)
                {
                    anchorX = Mathf.Sign(anchorX == 0f ? (i % 2 == 0 ? -1f : 1f) : anchorX) * activeConfig.LaneQuietZoneHalfWidth;
                }

                float velocityResponse = travelDelta * definition.VelocityResponseMultiplier;
                layer.Transform.position = new Vector3(anchorX + oscillation, cameraY + definition.AnchorOffset.y + verticalOffset + velocityResponse, cameraZ + (i * 0.01f));
            }
        }

        private void ApplyResolvedLayerSprite(RuntimeLayer layer, int restoredDistrictCount, int score)
        {
            BackgroundLayerDistrictVariantDefinition variant = ResolveDistrictVariant(layer.Definition, restoredDistrictCount, score);
            Sprite resolvedSprite = variant != null && variant.Sprite != null ? variant.Sprite : layer.Definition.Sprite;
            Vector2 resolvedContentFill = variant != null ? variant.ContentFill : layer.Definition.ContentFill;
            float resolvedAlphaMultiplier = variant != null ? Mathf.Clamp(variant.AlphaMultiplier, 0.5f, 1.5f) : 1f;
            Color resolvedTintColor = variant != null ? variant.TintColor : Color.white;
            float resolvedTintStrength = variant != null ? Mathf.Clamp01(variant.TintStrength) : 0f;
            bool shouldRender = resolvedSprite != null;

            if (layer.Renderer.enabled == shouldRender
                && layer.Renderer.sprite == resolvedSprite
                && layer.ActiveContentFill == resolvedContentFill
                && Mathf.Approximately(layer.ActiveAlphaMultiplier, resolvedAlphaMultiplier)
                && layer.ActiveTintColor == resolvedTintColor
                && Mathf.Approximately(layer.ActiveTintStrength, resolvedTintStrength))
            {
                return;
            }

            layer.Renderer.enabled = shouldRender;
            layer.Renderer.sprite = resolvedSprite;
            if (shouldRender)
            {
                layer.Renderer.drawMode = SpriteDrawMode.Simple;
                ApplyLayerScale(layer.Transform, resolvedSprite, layer.Definition.Size, resolvedContentFill);
            }

            layer.ActiveSprite = resolvedSprite;
            layer.ActiveContentFill = resolvedContentFill;
            layer.ActiveAlphaMultiplier = resolvedAlphaMultiplier;
            layer.ActiveTintColor = resolvedTintColor;
            layer.ActiveTintStrength = resolvedTintStrength;
        }

        private static void ApplyLayerScale(Transform layerTransform, Sprite sprite, Vector2 targetSize, Vector2 contentFill)
        {
            Vector2 spriteSize = sprite != null ? sprite.bounds.size : Vector2.one;
            float fillX = Mathf.Clamp(contentFill.x, 0.01f, 1f);
            float fillY = Mathf.Clamp(contentFill.y, 0.01f, 1f);
            float width = spriteSize.x > 0f ? targetSize.x / (spriteSize.x * fillX) : targetSize.x;
            float height = spriteSize.y > 0f ? targetSize.y / (spriteSize.y * fillY) : targetSize.y;
            layerTransform.localScale = new Vector3(width, height, 1f);
        }

        private static BackgroundLayerDistrictVariantDefinition ResolveDistrictVariant(BackgroundLayerDefinition definition, int restoredDistrictCount, int score)
        {
            BackgroundLayerDistrictVariantDefinition bestMatch = null;
            int bestPriority = int.MinValue;
            for (int i = 0; i < definition.DistrictVariants.Count; i++)
            {
                BackgroundLayerDistrictVariantDefinition candidate = definition.DistrictVariants[i];
                if (candidate == null || candidate.Sprite == null)
                {
                    continue;
                }

                bool usesScoreThreshold = candidate.MinimumScoreThreshold >= 0;
                if (usesScoreThreshold)
                {
                    if (score < candidate.MinimumScoreThreshold)
                    {
                        continue;
                    }

                    if (bestMatch == null || bestPriority < candidate.MinimumScoreThreshold)
                    {
                        bestMatch = candidate;
                        bestPriority = candidate.MinimumScoreThreshold;
                    }

                    continue;
                }

                if (candidate.MinimumRestoredDistrictCount > restoredDistrictCount)
                {
                    continue;
                }

                if (bestMatch == null || bestPriority < candidate.MinimumRestoredDistrictCount)
                {
                    bestMatch = candidate;
                    bestPriority = candidate.MinimumRestoredDistrictCount;
                }
            }

            return bestMatch;
        }

        private static int ResolveRestoredDistrictCount(ThemeConfig theme, WorldDistrictStateDefinition districtState)
        {
            WorldProgressionConfig progression = theme != null ? theme.ResolveWorldProgressionConfig() : null;
            if (progression == null || districtState == null)
            {
                return 0;
            }

            return progression.GetRestoredDistrictCount(districtState.MinScore);
        }


        private static Color ApplyVariantTint(Color baseColor, Color tintColor, float tintStrength)
        {
            if (tintStrength <= 0f)
            {
                return baseColor;
            }

            Color tinted = Color.Lerp(baseColor, tintColor, Mathf.Clamp01(tintStrength));
            tinted.a = baseColor.a;
            return tinted;
        }

        private Color ApplyMilestoneFlash(Color color)
        {
            if (milestoneFlashDuration <= 0f)
            {
                return color;
            }

            float pulse = 1f - Mathf.Clamp01(milestoneFlashElapsed / milestoneFlashDuration);
            return Color.Lerp(color, milestoneFlashColor, milestoneFlashStrength * pulse);
        }

        private static Color GetBackgroundColor(ThemeConfig theme, WorldDistrictStateDefinition districtState)
        {
            if (districtState != null)
            {
                return Color.Lerp(districtState.BackgroundTopColor, districtState.BackgroundBottomColor, 0.5f);
            }

            if (theme == null)
            {
                return new Color(0.03f, 0.04f, 0.08f, 1f);
            }

            return Color.Lerp(theme.BackgroundTopColor, theme.BackgroundBottomColor, 0.5f);
        }

        private static float GetBackgroundAlphaMultiplier(WorldDistrictStateDefinition districtState)
        {
            return districtState != null ? Mathf.Max(0.35f, districtState.BackgroundAlphaMultiplier) : 1f;
        }

        private static Color ResolveSkylineSilhouetteColor(Color topColor, Color bottomColor, Color accentColor)
        {
            Color baseColor = Color.Lerp(topColor, bottomColor, 0.52f);
            Color liftedColor = Color.Lerp(baseColor, accentColor, 0.24f);
            return Color.Lerp(liftedColor, Color.white, 0.08f);
        }

        private static Color ResolveWindowLightColor(Color lineGlowColor, Color accentColor, Color milestoneColor)
        {
            Color baseColor = Color.Lerp(Color.white, accentColor, 0.14f);
            Color energizedColor = Color.Lerp(baseColor, lineGlowColor, 0.14f);
            return Color.Lerp(energizedColor, milestoneColor, 0.05f);
        }

        private static Color ResolveUtilityInfrastructureColor(Color topColor, Color bottomColor, Color accentColor, Color lineGlowColor)
        {
            Color baseColor = Color.Lerp(topColor, bottomColor, 0.45f);
            Color liftedColor = Color.Lerp(baseColor, accentColor, 0.36f);
            return Color.Lerp(liftedColor, lineGlowColor, 0.1f);
        }

        private static Color ResolveAtmosphereHazeColor(Color topColor, Color bottomColor, Color accentColor)
        {
            Color baseColor = Color.Lerp(topColor, bottomColor, 0.35f);
            return Color.Lerp(baseColor, accentColor, 0.18f);
        }

        private static Color ResolveEnergyStreakColor(Color lineGlowColor, Color accentColor, Color milestoneColor)
        {
            Color baseColor = Color.Lerp(Color.white, lineGlowColor, 0.22f);
            Color energized = Color.Lerp(baseColor, accentColor, 0.12f);
            return Color.Lerp(energized, milestoneColor, 0.08f);
        }

        private static Color ResolveLayerColor(ThemeConfig theme, WorldDistrictStateDefinition districtState, BackgroundLayerColorRole role)
        {
            if (districtState != null)
            {
                return role switch
                {
                    BackgroundLayerColorRole.LineGlow => districtState.LineGlowColor,
                    BackgroundLayerColorRole.Accent => districtState.AccentColor,
                    BackgroundLayerColorRole.Milestone => districtState.MilestoneColor,
                    BackgroundLayerColorRole.SkylineSilhouette => ResolveSkylineSilhouetteColor(districtState.BackgroundTopColor, districtState.BackgroundBottomColor, districtState.AccentColor),
                    BackgroundLayerColorRole.WindowLights => ResolveWindowLightColor(districtState.LineGlowColor, districtState.AccentColor, districtState.MilestoneColor),
                    BackgroundLayerColorRole.UtilityInfrastructure => ResolveUtilityInfrastructureColor(districtState.BackgroundTopColor, districtState.BackgroundBottomColor, districtState.AccentColor, districtState.LineGlowColor),
                    BackgroundLayerColorRole.AtmosphereHaze => ResolveAtmosphereHazeColor(districtState.BackgroundTopColor, districtState.BackgroundBottomColor, districtState.AccentColor),
                    BackgroundLayerColorRole.EnergyStreaks => ResolveEnergyStreakColor(districtState.LineGlowColor, districtState.AccentColor, districtState.MilestoneColor),
                    _ => Color.Lerp(districtState.BackgroundTopColor, districtState.BackgroundBottomColor, 0.5f),
                };
            }

            if (theme == null)
            {
                return Color.white;
            }

            return role switch
            {
                BackgroundLayerColorRole.LineGlow => theme.LineGlowColor,
                BackgroundLayerColorRole.Accent => theme.PlayerAccentColor,
                BackgroundLayerColorRole.Milestone => theme.MilestoneColor,
                BackgroundLayerColorRole.SkylineSilhouette => ResolveSkylineSilhouetteColor(theme.BackgroundTopColor, theme.BackgroundBottomColor, theme.PlayerAccentColor),
                BackgroundLayerColorRole.WindowLights => ResolveWindowLightColor(theme.LineGlowColor, theme.PlayerAccentColor, theme.MilestoneColor),
                BackgroundLayerColorRole.UtilityInfrastructure => ResolveUtilityInfrastructureColor(theme.BackgroundTopColor, theme.BackgroundBottomColor, theme.PlayerAccentColor, theme.LineGlowColor),
                BackgroundLayerColorRole.AtmosphereHaze => ResolveAtmosphereHazeColor(theme.BackgroundTopColor, theme.BackgroundBottomColor, theme.PlayerAccentColor),
                BackgroundLayerColorRole.EnergyStreaks => ResolveEnergyStreakColor(theme.LineGlowColor, theme.PlayerAccentColor, theme.MilestoneColor),
                _ => Color.Lerp(theme.BackgroundTopColor, theme.BackgroundBottomColor, 0.5f),
            };
        }
    }
}

