#if UNITY_EDITOR
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using Voltline.Data;
using Voltline.Gameplay;
using Voltline.Utilities;

namespace Voltline.Tests.EditMode
{
    public sealed class GameplayPresentationEditModeTests
    {
        [Test]
        public void RuntimeWhiteSprite_UsesSingleWorldUnitSprite()
        {
            Assert.That(RuntimeSpriteFactory.WhiteSprite.pixelsPerUnit, Is.EqualTo(1f));
            Assert.That(RuntimeSpriteFactory.WhiteSprite.rect.width, Is.EqualTo(1f));
            Assert.That(RuntimeSpriteFactory.WhiteSprite.rect.height, Is.EqualTo(1f));
        }

        [Test]
        public void PlayerOffset_ClearsTheLineWithReadableMargin()
        {
            GameBalanceConfig gameBalance = AssetDatabase.LoadAssetAtPath<GameBalanceConfig>(ProjectConfigAssetPaths.GameBalance);
            GameplayPresentationConfig gameplayPresentation = AssetDatabase.LoadAssetAtPath<GameplayPresentationConfig>(ProjectConfigAssetPaths.GameplayPresentation);
            PlayerVisualConfig playerVisualConfig = AssetDatabase.LoadAssetAtPath<PlayerVisualConfig>(ProjectConfigAssetPaths.PlayerVisualConfig);
            float requiredClearance = (gameplayPresentation.TrackLineWidth * 0.5f)
                + playerVisualConfig.VisibleHalfWidth
                + gameplayPresentation.PlayerLineClearance;

            Assert.That(gameBalance.SideOffset, Is.GreaterThan(requiredClearance));
        }

        [Test]
        public void PlayerCollisionProfile_StaysInsideVisibleVisualBounds()
        {
            GameplayPresentationConfig gameplayPresentation = AssetDatabase.LoadAssetAtPath<GameplayPresentationConfig>(ProjectConfigAssetPaths.GameplayPresentation);
            PlayerVisualConfig playerVisualConfig = AssetDatabase.LoadAssetAtPath<PlayerVisualConfig>(ProjectConfigAssetPaths.PlayerVisualConfig);

            Assert.That(gameplayPresentation.PlayerCollisionHalfWidth, Is.GreaterThan(0f));
            Assert.That(gameplayPresentation.PlayerCollisionHalfHeight, Is.GreaterThan(0f));
            Assert.That(gameplayPresentation.PlayerCollisionHalfWidth, Is.LessThan(playerVisualConfig.VisibleHalfWidth));
            Assert.That(gameplayPresentation.PlayerCollisionHalfHeight, Is.LessThanOrEqualTo(playerVisualConfig.VisibleHalfHeight));
        }

        [Test]
        public void PlayerVisualConfig_DefinesEveryApprovedPresentationState()
        {
            PlayerVisualConfig playerVisualConfig = AssetDatabase.LoadAssetAtPath<PlayerVisualConfig>(ProjectConfigAssetPaths.PlayerVisualConfig);

            foreach (PlayerVisualPresentationStateId stateId in System.Enum.GetValues(typeof(PlayerVisualPresentationStateId)))
            {
                PlayerVisualStateDefinition definition = playerVisualConfig.ResolveStateDefinition(stateId);
                Assert.That(definition, Is.Not.Null, stateId.ToString());
                Assert.That(definition.VisibleBounds.x, Is.GreaterThan(0f), stateId.ToString());
                Assert.That(definition.VisibleBounds.y, Is.GreaterThan(0f), stateId.ToString());
                Assert.That(definition.DurationSeconds, Is.GreaterThanOrEqualTo(0f), stateId.ToString());
            }
        }

        [Test]
        public void PlayerVisualStates_DeclareStableDirectVisibleDimensions()
        {
            PlayerVisualConfig playerVisualConfig = AssetDatabase.LoadAssetAtPath<PlayerVisualConfig>(ProjectConfigAssetPaths.PlayerVisualConfig);

            foreach (PlayerVisualPresentationStateId stateId in System.Enum.GetValues(typeof(PlayerVisualPresentationStateId)))
            {
                PlayerVisualStateDefinition definition = playerVisualConfig.ResolveStateDefinition(stateId);
                Assert.That(definition.VisibleBounds.x, Is.EqualTo(playerVisualConfig.VisibleBoundsScale.x).Within(0.0001f), stateId.ToString());
                Assert.That(definition.VisibleBounds.y, Is.EqualTo(playerVisualConfig.VisibleBoundsScale.y).Within(0.0001f), stateId.ToString());
            }
        }

        [Test]
        public void PlayerVisualView_NormalizesImportedSpriteToConfiguredVisibleBounds()
        {
            PlayerVisualConfig playerVisualConfig = AssetDatabase.LoadAssetAtPath<PlayerVisualConfig>(ProjectConfigAssetPaths.PlayerVisualConfig);
            GameObject root = new("PlayerVisualViewTests");

            try
            {
                PlayerVisualView view = root.AddComponent<PlayerVisualView>();
                view.Initialize(root.transform, playerVisualConfig);
                view.Apply(new PlayerVisualState(Vector3.zero, Color.white, 0f));

                SpriteRenderer renderer = root.GetComponentInChildren<SpriteRenderer>();
                Assert.That(renderer, Is.Not.Null);
                Assert.That(renderer.bounds.size.x, Is.EqualTo(playerVisualConfig.VisibleBoundsScale.x).Within(0.02f));
                Assert.That(renderer.bounds.size.y, Is.EqualTo(playerVisualConfig.VisibleBoundsScale.y).Within(0.02f));
            }
            finally
            {
                Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void PlayerVisualStates_StayCloseToApprovedVisibleFootprint()
        {
            PlayerVisualConfig playerVisualConfig = AssetDatabase.LoadAssetAtPath<PlayerVisualConfig>(ProjectConfigAssetPaths.PlayerVisualConfig);
            GameObject root = new("PlayerVisualStateSizingTests");

            try
            {
                PlayerVisualView view = root.AddComponent<PlayerVisualView>();
                view.Initialize(root.transform, playerVisualConfig);
                SpriteRenderer renderer = root.GetComponentInChildren<SpriteRenderer>();
                Assert.That(renderer, Is.Not.Null);

                foreach (PlayerVisualPresentationStateId stateId in System.Enum.GetValues(typeof(PlayerVisualPresentationStateId)))
                {
                    PlayerVisualStateDefinition definition = playerVisualConfig.ResolveStateDefinition(stateId);
                    view.PlayPresentationState(stateId);
                    view.Advance(0.05f);
                    view.Apply(new PlayerVisualState(Vector3.zero, Color.white, 0f));

                    Assert.That(renderer.bounds.size.x, Is.EqualTo(definition.VisibleBounds.x).Within(0.02f), stateId.ToString());
                    Assert.That(renderer.bounds.size.y, Is.EqualTo(definition.VisibleBounds.y).Within(0.02f), stateId.ToString());
                }
            }
            finally
            {
                Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void PlayerVisualFallbackStates_DoNotScaleIdleArtBeyondBaseline()
        {
            PlayerVisualConfig playerVisualConfig = AssetDatabase.LoadAssetAtPath<PlayerVisualConfig>(ProjectConfigAssetPaths.PlayerVisualConfig);
            GameObject root = new("PlayerVisualFallbackStateTests");

            try
            {
                PlayerVisualView view = root.AddComponent<PlayerVisualView>();
                view.Initialize(root.transform, playerVisualConfig);
                SpriteRenderer renderer = root.GetComponentInChildren<SpriteRenderer>();
                Assert.That(renderer, Is.Not.Null);

                view.Apply(new PlayerVisualState(Vector3.zero, Color.white, 0f));
                float baselineWidth = renderer.bounds.size.x;
                float baselineHeight = renderer.bounds.size.y;

                foreach (PlayerVisualPresentationStateId stateId in new[]
                {
                    PlayerVisualPresentationStateId.Score,
                    PlayerVisualPresentationStateId.Milestone,
                    PlayerVisualPresentationStateId.Death,
                })
                {
                    view.PlayPresentationState(stateId);
                    view.Advance(0.05f);
                    view.Apply(new PlayerVisualState(Vector3.zero, Color.white, 0f));

                    Assert.That(renderer.bounds.size.x, Is.EqualTo(baselineWidth).Within(0.02f), stateId.ToString());
                    Assert.That(renderer.bounds.size.y, Is.EqualTo(baselineHeight).Within(0.02f), stateId.ToString());
                }
            }
            finally
            {
                Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void PlayerVisualStateSwap_DoesNotInheritPreviousRuntimeScale()
        {
            PlayerVisualConfig playerVisualConfig = AssetDatabase.LoadAssetAtPath<PlayerVisualConfig>(ProjectConfigAssetPaths.PlayerVisualConfig);
            GameObject root = new("PlayerVisualStateSwapTests");

            try
            {
                PlayerVisualView view = root.AddComponent<PlayerVisualView>();
                view.Initialize(root.transform, playerVisualConfig);
                SpriteRenderer renderer = root.GetComponentInChildren<SpriteRenderer>();
                Assert.That(renderer, Is.Not.Null);

                view.PlayPresentationState(PlayerVisualPresentationStateId.Flip);
                view.Advance(0.05f);
                view.Apply(new PlayerVisualState(Vector3.zero, Color.white, 0f));
                float flipWidth = renderer.bounds.size.x;
                float flipHeight = renderer.bounds.size.y;

                view.PlayPresentationState(PlayerVisualPresentationStateId.NearMiss);
                view.Advance(0.05f);
                view.Apply(new PlayerVisualState(Vector3.zero, Color.white, 0f));

                Assert.That(renderer.bounds.size.x, Is.EqualTo(playerVisualConfig.ResolveStateDefinition(PlayerVisualPresentationStateId.NearMiss).VisibleBounds.x).Within(0.02f));
                Assert.That(renderer.bounds.size.y, Is.EqualTo(playerVisualConfig.ResolveStateDefinition(PlayerVisualPresentationStateId.NearMiss).VisibleBounds.y).Within(0.02f));
                Assert.That(Mathf.Abs(renderer.bounds.size.x - flipWidth), Is.LessThanOrEqualTo(0.04f));
                Assert.That(Mathf.Abs(renderer.bounds.size.y - flipHeight), Is.LessThanOrEqualTo(0.04f));
            }
            finally
            {
                Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void HazardVisualView_NormalizesSpriteAssetsToConfiguredBounds()
        {
            ObstacleVisualCatalog visualCatalog = AssetDatabase.LoadAssetAtPath<ObstacleVisualCatalog>(ProjectConfigAssetPaths.ObstacleVisualCatalog);

            foreach (ObstacleFamily family in System.Enum.GetValues(typeof(ObstacleFamily)))
            {
                ObstacleVisualProfile profile = visualCatalog.GetRequiredProfile(family);
                GameObject root = new($"HazardVisualView_{family}_Tests");

                try
                {
                    HazardVisualView view = root.AddComponent<HazardVisualView>();
                    view.Initialize(root.transform, profile);
                    view.Apply(new HazardVisualState(
                        new SpriteLayerState(true, Vector3.zero, profile.VisualBoundsScale * 0.5f, Color.white, 0f),
                        SpriteLayerState.Hidden,
                        SpriteLayerState.Hidden,
                        profile.UsesTelegraph
                            ? new SpriteLayerState(true, Vector3.zero, profile.TelegraphBoundsScale * 0.5f, Color.white, 0f)
                            : SpriteLayerState.Hidden));
                    view.Apply(new HazardVisualState(
                        new SpriteLayerState(true, Vector3.zero, profile.VisualBoundsScale, Color.white, 0f),
                        SpriteLayerState.Hidden,
                        SpriteLayerState.Hidden,
                        profile.UsesTelegraph
                            ? new SpriteLayerState(true, Vector3.zero, profile.TelegraphBoundsScale, Color.white, 0f)
                            : SpriteLayerState.Hidden));

                    SpriteRenderer mainRenderer = FindRenderer(root, "Main");
                    Assert.That(mainRenderer, Is.Not.Null, family.ToString());
                    Assert.That(mainRenderer.bounds.size.x, Is.EqualTo(profile.VisualBoundsScale.x).Within(0.02f), family.ToString());
                    Assert.That(mainRenderer.bounds.size.y, Is.EqualTo(profile.VisualBoundsScale.y).Within(0.02f), family.ToString());

                    SpriteRenderer telegraphRenderer = FindRenderer(root, "Telegraph");
                    if (profile.UsesTelegraph)
                    {
                        Assert.That(telegraphRenderer, Is.Not.Null, family + " telegraph");
                        Assert.That(telegraphRenderer.bounds.size.x, Is.EqualTo(profile.TelegraphBoundsScale.x).Within(0.02f), family + " telegraph");
                        Assert.That(telegraphRenderer.bounds.size.y, Is.EqualTo(profile.TelegraphBoundsScale.y).Within(0.02f), family + " telegraph");
                    }
                }
                finally
                {
                    Object.DestroyImmediate(root);
                }
            }
        }

        [Test]
        public void HazardProfiles_KeepReadableSpacingBetweenVisibleBounds()
        {
            HazardPresentationCatalog layoutCatalog = AssetDatabase.LoadAssetAtPath<HazardPresentationCatalog>(ProjectConfigAssetPaths.HazardPresentationCatalog);
            ObstacleVisualCatalog visualCatalog = AssetDatabase.LoadAssetAtPath<ObstacleVisualCatalog>(ProjectConfigAssetPaths.ObstacleVisualCatalog);
            HazardLayoutProfile blockersLayout = layoutCatalog.GetRequiredProfile(ObstacleFamily.SidePressureHazards);
            ObstacleVisualProfile blockersVisual = visualCatalog.GetRequiredProfile(ObstacleFamily.SidePressureHazards);

            Assert.That(blockersVisual.VisualBoundsScale.y, Is.GreaterThan(0f));
            Assert.That(layoutCatalog.GetRequiredHitDistanceSeparation(blockersLayout, blockersLayout), Is.GreaterThan(blockersVisual.VisualBoundsScale.y));
        }

        [Test]
        public void HazardCollisionProfiles_StayInsideVisibleBounds()
        {
            HazardPresentationCatalog layoutCatalog = AssetDatabase.LoadAssetAtPath<HazardPresentationCatalog>(ProjectConfigAssetPaths.HazardPresentationCatalog);
            ObstacleVisualCatalog visualCatalog = AssetDatabase.LoadAssetAtPath<ObstacleVisualCatalog>(ProjectConfigAssetPaths.ObstacleVisualCatalog);

            foreach (ObstacleFamily family in System.Enum.GetValues(typeof(ObstacleFamily)))
            {
                HazardLayoutProfile layout = layoutCatalog.GetRequiredProfile(family);
                ObstacleVisualProfile visual = visualCatalog.GetRequiredProfile(family);
                Assert.That(layout.CollisionHalfWidth, Is.GreaterThan(0f), family.ToString());
                Assert.That(layout.CollisionHalfHeight, Is.GreaterThan(0f), family.ToString());
                Assert.That(layout.CollisionBoundsScale.x, Is.LessThanOrEqualTo(visual.VisualBoundsScale.x), family.ToString());
                Assert.That(layout.CollisionBoundsScale.y, Is.LessThanOrEqualTo(visual.VisualBoundsScale.y), family.ToString());
            }
        }

        [Test]
        public void HazardProfiles_ClearTheLineHorizontally()
        {
            GameBalanceConfig gameBalance = AssetDatabase.LoadAssetAtPath<GameBalanceConfig>(ProjectConfigAssetPaths.GameBalance);
            GameplayPresentationConfig gameplayPresentation = AssetDatabase.LoadAssetAtPath<GameplayPresentationConfig>(ProjectConfigAssetPaths.GameplayPresentation);
            ObstacleVisualCatalog visualCatalog = AssetDatabase.LoadAssetAtPath<ObstacleVisualCatalog>(ProjectConfigAssetPaths.ObstacleVisualCatalog);
            float widestVisualHalfWidth = visualCatalog.GetRequiredProfile(ObstacleFamily.SidePressureHazards).VisualHalfWidth;
            float requiredClearance = (gameplayPresentation.TrackLineWidth * 0.5f) + widestVisualHalfWidth + 0.08f;

            Assert.That(gameBalance.SideOffset, Is.GreaterThan(requiredClearance));
        }

        [Test]
        public void BackgroundPresentation_QuietZonePreservesReadableLane()
        {
            GameBalanceConfig gameBalance = AssetDatabase.LoadAssetAtPath<GameBalanceConfig>(ProjectConfigAssetPaths.GameBalance);
            GameplayPresentationConfig gameplayPresentation = AssetDatabase.LoadAssetAtPath<GameplayPresentationConfig>(ProjectConfigAssetPaths.GameplayPresentation);
            BackgroundPresentationConfig backgroundPresentationConfig = AssetDatabase.LoadAssetAtPath<BackgroundPresentationConfig>(ProjectConfigAssetPaths.BackgroundPresentation);
            ObstacleVisualCatalog visualCatalog = AssetDatabase.LoadAssetAtPath<ObstacleVisualCatalog>(ProjectConfigAssetPaths.ObstacleVisualCatalog);
            float widestVisualHalfWidth = visualCatalog.GetRequiredProfile(ObstacleFamily.SidePressureHazards).VisualHalfWidth;
            float requiredClearance = (gameplayPresentation.TrackLineWidth * 0.5f) + widestVisualHalfWidth + 0.08f;

            Assert.That(backgroundPresentationConfig.LaneQuietZoneHalfWidth, Is.GreaterThan(requiredClearance));
            Assert.That(backgroundPresentationConfig.MaxRuntimeSpriteCount, Is.LessThanOrEqualTo(12));
        }


        [Test]
        public void BackgroundPresentationController_KeepsCenteredAtmosphereLayersCenteredWhenQuietZoneIsDisabled()
        {
            Texture2D texture = new(8, 8, TextureFormat.RGBA32, false);
            Color[] pixels = new Color[8 * 8];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = Color.white;
            }

            texture.SetPixels(pixels);
            texture.Apply();
            Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, 8f, 8f), new Vector2(0.5f, 0.5f), 8f);
            BackgroundLayerDefinition layer = new();
            SetPrivateField(layer, "layerId", "layer.test.centered");
            SetPrivateField(layer, "sprite", sprite);
            SetPrivateField(layer, "size", new Vector2(4f, 2f));
            SetPrivateField(layer, "contentFill", Vector2.one);
            SetPrivateField(layer, "anchorOffset", new Vector2(0f, 0.5f));
            SetPrivateField(layer, "enforceLaneQuietZone", false);
            SetPrivateField(layer, "alpha", 0.04f);
            SetPrivateField(layer, "verticalTravelMultiplier", 0f);
            SetPrivateField(layer, "verticalLoopDistance", 8f);
            SetPrivateField(layer, "velocityResponseMultiplier", 0f);
            SetPrivateField(layer, "horizontalOscillationAmplitude", 0f);
            SetPrivateField(layer, "horizontalOscillationFrequency", 0f);
            SetPrivateField(layer, "sortingOrder", -6);

            BackgroundPresentationConfig config = ScriptableObject.CreateInstance<BackgroundPresentationConfig>();
            SetPrivateField(config, "maxRuntimeSpriteCount", 1);
            SetPrivateField(config, "maxExpectedDrawCalls", 1);
            SetPrivateField(config, "laneQuietZoneHalfWidth", 1.55f);
            SetPrivateField(config, "maximumAllowedLayerAlpha", 0.16f);
            SetPrivateField(config, "layers", new System.Collections.Generic.List<BackgroundLayerDefinition> { layer });

            GameObject root = new("BackgroundCenteredLayerTests");
            GameObject cameraObject = new("BackgroundCenteredLayerCamera");

            try
            {
                Camera camera = cameraObject.AddComponent<Camera>();
                camera.orthographic = true;
                camera.orthographicSize = 6f;

                BackgroundPresentationController controller = root.AddComponent<BackgroundPresentationController>();
                controller.Initialize(config, null, camera, null);
                InvokePrivateMethod(controller, "AnimateLayers");

                Transform runtimeLayer = root.transform.Find("BackgroundPresentationRoot/BackgroundLayer_0_layer.test.centered");
                Assert.That(runtimeLayer, Is.Not.Null);
                Assert.That(runtimeLayer.position.x, Is.EqualTo(0f).Within(0.02f));
            }
            finally
            {
                Object.DestroyImmediate(root);
                Object.DestroyImmediate(cameraObject);
                Object.DestroyImmediate(config);
                Object.DestroyImmediate(sprite);
                Object.DestroyImmediate(texture);
            }
        }

        [Test]
        public void BackgroundPresentationController_PromotesBeaconLayerAcrossScoreThresholds()
        {
            Texture2D beaconTexture = new(8, 8, TextureFormat.RGBA32, false);
            Color[] pixels = new Color[8 * 8];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = Color.white;
            }

            beaconTexture.SetPixels(pixels);
            beaconTexture.Apply();
            Sprite beaconSprite = Sprite.Create(beaconTexture, new Rect(0f, 0f, 8f, 8f), new Vector2(0.5f, 0.5f), 8f);

            BackgroundLayerDistrictVariantDefinition beaconVariant = new();
            SetPrivateField(beaconVariant, "minimumRestoredDistrictCount", 1);
            SetPrivateField(beaconVariant, "minimumScoreThreshold", 15);
            SetPrivateField(beaconVariant, "sprite", beaconSprite);
            SetPrivateField(beaconVariant, "contentFill", Vector2.one);
            SetPrivateField(beaconVariant, "alphaMultiplier", 0.86f);
            SetPrivateField(beaconVariant, "tintColor", new Color(0.96f, 0.985f, 1f, 1f));
            SetPrivateField(beaconVariant, "tintStrength", 0.78f);

            BackgroundLayerDistrictVariantDefinition beaconVariantMid = new();
            SetPrivateField(beaconVariantMid, "minimumRestoredDistrictCount", 1);
            SetPrivateField(beaconVariantMid, "minimumScoreThreshold", 25);
            SetPrivateField(beaconVariantMid, "sprite", beaconSprite);
            SetPrivateField(beaconVariantMid, "contentFill", Vector2.one);
            SetPrivateField(beaconVariantMid, "alphaMultiplier", 1.26f);
            SetPrivateField(beaconVariantMid, "tintColor", new Color(1f, 1f, 1f, 1f));
            SetPrivateField(beaconVariantMid, "tintStrength", 1f);

            BackgroundLayerDistrictVariantDefinition beaconVariantHigh = new();
            SetPrivateField(beaconVariantHigh, "minimumRestoredDistrictCount", 1);
            SetPrivateField(beaconVariantHigh, "minimumScoreThreshold", 35);
            SetPrivateField(beaconVariantHigh, "sprite", beaconSprite);
            SetPrivateField(beaconVariantHigh, "contentFill", Vector2.one);
            SetPrivateField(beaconVariantHigh, "alphaMultiplier", 1.45f);
            SetPrivateField(beaconVariantHigh, "tintColor", new Color(1f, 1f, 1f, 1f));
            SetPrivateField(beaconVariantHigh, "tintStrength", 1f);

            BackgroundLayerDefinition layer = new();
            SetPrivateField(layer, "layerId", "layer.test.beacon");
            SetPrivateField(layer, "colorRole", BackgroundLayerColorRole.Milestone);
            SetPrivateField(layer, "sprite", null);
            SetPrivateField(layer, "size", new Vector2(4f, 4f));
            SetPrivateField(layer, "contentFill", Vector2.one);
            SetPrivateField(layer, "districtVariants", new System.Collections.Generic.List<BackgroundLayerDistrictVariantDefinition> { beaconVariant, beaconVariantMid, beaconVariantHigh });
            SetPrivateField(layer, "anchorOffset", new Vector2(-2f, 0.5f));
            SetPrivateField(layer, "alpha", 0.11f);
            SetPrivateField(layer, "verticalTravelMultiplier", 0f);
            SetPrivateField(layer, "verticalLoopDistance", 8f);
            SetPrivateField(layer, "velocityResponseMultiplier", 0f);
            SetPrivateField(layer, "horizontalOscillationAmplitude", 0f);
            SetPrivateField(layer, "horizontalOscillationFrequency", 0f);
            SetPrivateField(layer, "sortingOrder", -9);

            BackgroundPresentationConfig config = ScriptableObject.CreateInstance<BackgroundPresentationConfig>();
            SetPrivateField(config, "maxRuntimeSpriteCount", 1);
            SetPrivateField(config, "maxExpectedDrawCalls", 1);
            SetPrivateField(config, "laneQuietZoneHalfWidth", 1.55f);
            SetPrivateField(config, "maximumAllowedLayerAlpha", 0.16f);
            SetPrivateField(config, "layers", new System.Collections.Generic.List<BackgroundLayerDefinition> { layer });

            ThemeCatalog themeCatalog = AssetDatabase.LoadAssetAtPath<ThemeCatalog>(ProjectConfigAssetPaths.ThemeCatalog);
            Assert.That(themeCatalog.TryGetTheme("theme.live-wire-city", out ThemeConfig theme), Is.True);

            GameObject root = new("BackgroundBeaconLayerTests");
            GameObject cameraObject = new("BackgroundBeaconLayerCamera");

            try
            {
                Camera camera = cameraObject.AddComponent<Camera>();
                camera.orthographic = true;
                camera.orthographicSize = 6f;

                BackgroundPresentationController controller = root.AddComponent<BackgroundPresentationController>();
                controller.Initialize(config, theme, camera, null);
                controller.ApplyWorldDistrict(theme.ResolveWorldProgressionConfig().GetRequiredDistrictForScore(10), 0f);
                controller.ApplyScore(10);

                SpriteRenderer renderer = root.GetComponentInChildren<SpriteRenderer>(true);
                Assert.That(renderer, Is.Not.Null);
                Assert.That(renderer.enabled, Is.False);

                controller.ApplyWorldDistrict(theme.ResolveWorldProgressionConfig().GetRequiredDistrictForScore(15), 0f);
                controller.ApplyScore(15);
                Assert.That(renderer.enabled, Is.True);
                Assert.That(renderer.sprite, Is.EqualTo(beaconSprite));
                float alphaAtFifteen = renderer.color.a;
                float greenAtFifteen = renderer.color.g;

                controller.ApplyWorldDistrict(theme.ResolveWorldProgressionConfig().GetRequiredDistrictForScore(25), 0f);
                controller.ApplyScore(25);
                Assert.That(renderer.color.a, Is.GreaterThan(alphaAtFifteen));
                Assert.That(renderer.color.g, Is.GreaterThan(greenAtFifteen));
                float alphaAtTwentyFive = renderer.color.a;

                controller.ApplyWorldDistrict(theme.ResolveWorldProgressionConfig().GetRequiredDistrictForScore(35), 0f);
                controller.ApplyScore(35);
                Assert.That(renderer.color.a, Is.GreaterThan(alphaAtTwentyFive));
                Assert.That(renderer.color.r, Is.GreaterThan(renderer.color.b));
            }
            finally
            {
                Object.DestroyImmediate(root);
                Object.DestroyImmediate(cameraObject);
                Object.DestroyImmediate(config);
                Object.DestroyImmediate(beaconSprite);
                Object.DestroyImmediate(beaconTexture);
            }
        }

        [Test]
        public void BackgroundPresentationController_PromotesDistrictWindowVariantWhenCityProgresses()
        {
            Texture2D baseTexture = new(8, 8, TextureFormat.RGBA32, false);
            Texture2D denseTexture = new(8, 8, TextureFormat.RGBA32, false);
            Color[] pixels = new Color[8 * 8];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = Color.white;
            }

            baseTexture.SetPixels(pixels);
            baseTexture.Apply();
            denseTexture.SetPixels(pixels);
            denseTexture.Apply();
            Sprite baseSprite = Sprite.Create(baseTexture, new Rect(0f, 0f, 8f, 8f), new Vector2(0.5f, 0.5f), 8f);
            Sprite denseSprite = Sprite.Create(denseTexture, new Rect(0f, 0f, 8f, 8f), new Vector2(0.5f, 0.5f), 8f);

            BackgroundLayerDistrictVariantDefinition denseVariant = new();
            SetPrivateField(denseVariant, "minimumRestoredDistrictCount", 2);
            SetPrivateField(denseVariant, "sprite", denseSprite);
            SetPrivateField(denseVariant, "contentFill", Vector2.one);
            SetPrivateField(denseVariant, "alphaMultiplier", 1.4f);
            SetPrivateField(denseVariant, "tintColor", new Color(1f, 0.86f, 0.32f, 1f));
            SetPrivateField(denseVariant, "tintStrength", 0.82f);

            BackgroundLayerDefinition layer = new();
            SetPrivateField(layer, "layerId", "layer.test.windows");
            SetPrivateField(layer, "colorRole", BackgroundLayerColorRole.WindowLights);
            SetPrivateField(layer, "sprite", baseSprite);
            SetPrivateField(layer, "size", new Vector2(4f, 4f));
            SetPrivateField(layer, "contentFill", Vector2.one);
            SetPrivateField(layer, "districtVariants", new System.Collections.Generic.List<BackgroundLayerDistrictVariantDefinition> { denseVariant });
            SetPrivateField(layer, "anchorOffset", new Vector2(-2f, 0.5f));
            SetPrivateField(layer, "alpha", 0.08f);
            SetPrivateField(layer, "verticalTravelMultiplier", 0f);
            SetPrivateField(layer, "verticalLoopDistance", 8f);
            SetPrivateField(layer, "velocityResponseMultiplier", 0f);
            SetPrivateField(layer, "horizontalOscillationAmplitude", 0f);
            SetPrivateField(layer, "horizontalOscillationFrequency", 0f);
            SetPrivateField(layer, "sortingOrder", -12);

            BackgroundPresentationConfig config = ScriptableObject.CreateInstance<BackgroundPresentationConfig>();
            SetPrivateField(config, "maxRuntimeSpriteCount", 1);
            SetPrivateField(config, "maxExpectedDrawCalls", 1);
            SetPrivateField(config, "laneQuietZoneHalfWidth", 1.55f);
            SetPrivateField(config, "maximumAllowedLayerAlpha", 0.16f);
            SetPrivateField(config, "layers", new System.Collections.Generic.List<BackgroundLayerDefinition> { layer });

            ThemeCatalog themeCatalog = AssetDatabase.LoadAssetAtPath<ThemeCatalog>(ProjectConfigAssetPaths.ThemeCatalog);
            Assert.That(themeCatalog.TryGetTheme("theme.live-wire-city", out ThemeConfig theme), Is.True);
            Assert.That(theme.ResolveWorldProgressionConfig(), Is.Not.Null);

            GameObject root = new("BackgroundVariantLayerTests");
            GameObject cameraObject = new("BackgroundVariantLayerCamera");

            try
            {
                Camera camera = cameraObject.AddComponent<Camera>();
                camera.orthographic = true;
                camera.orthographicSize = 6f;

                BackgroundPresentationController controller = root.AddComponent<BackgroundPresentationController>();
                controller.Initialize(config, theme, camera, null);
                controller.ApplyWorldDistrict(theme.ResolveWorldProgressionConfig().GetRequiredDistrictForScore(0), 0f);

                SpriteRenderer renderer = root.GetComponentInChildren<SpriteRenderer>(true);
                Assert.That(renderer, Is.Not.Null);
                Assert.That(renderer.sprite, Is.EqualTo(baseSprite));

                controller.ApplyWorldDistrict(theme.ResolveWorldProgressionConfig().GetRequiredDistrictForScore(10), 0f);
                Assert.That(renderer.sprite, Is.EqualTo(denseSprite));
                Assert.That(renderer.color.a, Is.GreaterThan(0.08f));
                Assert.That(renderer.color.g, Is.GreaterThan(renderer.color.b));
            }
            finally
            {
                Object.DestroyImmediate(root);
                Object.DestroyImmediate(cameraObject);
                Object.DestroyImmediate(config);
                Object.DestroyImmediate(baseSprite);
                Object.DestroyImmediate(denseSprite);
                Object.DestroyImmediate(baseTexture);
                Object.DestroyImmediate(denseTexture);
            }
        }

        [Test]
        public void ScoreSystem_DebugSetScorePromotesMilestonesAndAllowsTargetedThemeTesting()
        {
            GameBalanceConfig gameBalance = AssetDatabase.LoadAssetAtPath<GameBalanceConfig>(ProjectConfigAssetPaths.GameBalance);
            GameObject root = new("ScoreSystemDebugJumpTests");

            try
            {
                ScoreSystem scoreSystem = root.AddComponent<ScoreSystem>();
                scoreSystem.Initialize(gameBalance);
                int milestoneCount = 0;
                int lastScore = -1;
                scoreSystem.ScoreChanged += score => lastScore = score;
                scoreSystem.MilestoneReached += _ => milestoneCount++;

                scoreSystem.SetDebugScore(35);
                Assert.That(scoreSystem.CurrentScore, Is.EqualTo(35));
                Assert.That(lastScore, Is.EqualTo(35));
                Assert.That(milestoneCount, Is.EqualTo(3));

                scoreSystem.SetDebugScore(15);
                Assert.That(scoreSystem.CurrentScore, Is.EqualTo(15));

                scoreSystem.SetDebugScore(45);
                Assert.That(scoreSystem.CurrentScore, Is.EqualTo(45));
                Assert.That(milestoneCount, Is.EqualTo(4));
            }
            finally
            {
                Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void ThemePresentationController_BootstrapsWorldProgressionDuringFirstRunInitialization()
        {
            GameBalanceConfig gameBalance = AssetDatabase.LoadAssetAtPath<GameBalanceConfig>(ProjectConfigAssetPaths.GameBalance);
            ThemeCatalog themeCatalog = AssetDatabase.LoadAssetAtPath<ThemeCatalog>(ProjectConfigAssetPaths.ThemeCatalog);
            Assert.That(themeCatalog.TryGetTheme("theme.live-wire-city", out ThemeConfig theme), Is.True);

            GameObject root = new("ThemePresentationBootstrapTests");
            try
            {
                ScoreSystem scoreSystem = root.AddComponent<ScoreSystem>();
                scoreSystem.Initialize(gameBalance);
                GameManager gameManager = root.AddComponent<GameManager>();
                WorldProgressionController worldProgressionController = root.AddComponent<WorldProgressionController>();
                ThemePresentationController controller = root.AddComponent<ThemePresentationController>();

                controller.Initialize(
                    themeCatalog,
                    null,
                    null,
                    gameManager,
                    scoreSystem,
                    null,
                    null,
                    null,
                    null,
                    null,
                    worldProgressionController,
                    null,
                    null,
                    null,
                    theme);

                Assert.That(worldProgressionController.Config, Is.Not.Null);
                Assert.That(worldProgressionController.CurrentDistrict, Is.Not.Null);
                Assert.That(worldProgressionController.CurrentDistrict.DisplayName, Is.EqualTo("Failing Grid"));
            }
            finally
            {
                Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void BackgroundPresentationController_NormalizesAuthoredSpriteVisibleContentToConfiguredLayerSize()
        {
            Texture2D texture = new(8, 16, TextureFormat.RGBA32, false);
            Color[] pixels = new Color[8 * 16];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = Color.white;
            }

            texture.SetPixels(pixels);
            texture.Apply();
            Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, 8f, 16f), new Vector2(0.5f, 0.5f), 8f);
            Vector2 configuredSize = new(6.4f, 9.2f);
            Vector2 contentFill = new(0.5f, 0.5f);
            BackgroundLayerDefinition layer = new();
            SetPrivateField(layer, "layerId", "layer.test.authored");
            SetPrivateField(layer, "sprite", sprite);
            SetPrivateField(layer, "size", configuredSize);
            SetPrivateField(layer, "contentFill", contentFill);
            SetPrivateField(layer, "anchorOffset", new Vector2(-2.7f, 0.4f));
            SetPrivateField(layer, "alpha", 0.08f);
            SetPrivateField(layer, "verticalTravelMultiplier", 0f);
            SetPrivateField(layer, "verticalLoopDistance", 8f);
            SetPrivateField(layer, "horizontalOscillationAmplitude", 0f);
            SetPrivateField(layer, "horizontalOscillationFrequency", 0f);
            SetPrivateField(layer, "sortingOrder", -13);

            BackgroundPresentationConfig config = ScriptableObject.CreateInstance<BackgroundPresentationConfig>();
            SetPrivateField(config, "maxRuntimeSpriteCount", 1);
            SetPrivateField(config, "maxExpectedDrawCalls", 1);
            SetPrivateField(config, "laneQuietZoneHalfWidth", 1.55f);
            SetPrivateField(config, "maximumAllowedLayerAlpha", 0.16f);
            SetPrivateField(config, "layers", new System.Collections.Generic.List<BackgroundLayerDefinition> { layer });

            GameObject root = new("BackgroundPresentationControllerTests");
            GameObject cameraObject = new("BackgroundPresentationCamera");

            try
            {
                Camera camera = cameraObject.AddComponent<Camera>();
                camera.orthographic = true;
                camera.orthographicSize = 6f;

                BackgroundPresentationController controller = root.AddComponent<BackgroundPresentationController>();
                controller.Initialize(config, null, camera, null);

                SpriteRenderer renderer = root.GetComponentInChildren<SpriteRenderer>(true);
                Assert.That(renderer, Is.Not.Null);
                Assert.That(renderer.sprite, Is.EqualTo(sprite));
                Assert.That(renderer.bounds.size.x * contentFill.x, Is.EqualTo(configuredSize.x).Within(0.02f));
                Assert.That(renderer.bounds.size.y * contentFill.y, Is.EqualTo(configuredSize.y).Within(0.02f));
            }
            finally
            {
                Object.DestroyImmediate(root);
                Object.DestroyImmediate(cameraObject);
                Object.DestroyImmediate(config);
                Object.DestroyImmediate(sprite);
                Object.DestroyImmediate(texture);
            }
        }

        private static void InvokePrivateMethod(object target, string methodName)
        {
            System.Reflection.MethodInfo method = target.GetType().GetMethod(methodName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            Assert.That(method, Is.Not.Null, methodName);
            method.Invoke(target, null);
        }

        private static void SetPrivateField<TTarget>(TTarget target, string fieldName, object value)
        {
            System.Reflection.FieldInfo field = typeof(TTarget).GetField(fieldName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null, fieldName);
            field.SetValue(target, value);
        }
        private static SpriteRenderer FindRenderer(GameObject root, string name)
        {
            foreach (SpriteRenderer renderer in root.GetComponentsInChildren<SpriteRenderer>(true))
            {
                if (renderer != null && renderer.name == name)
                {
                    return renderer;
                }
            }

            return null;
        }
    }
}
#endif




