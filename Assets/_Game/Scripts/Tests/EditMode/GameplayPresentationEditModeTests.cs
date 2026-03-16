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
            HazardLayoutProfile blockersLayout = layoutCatalog.GetRequiredProfile(ObstacleFamily.SideBlockers);
            ObstacleVisualProfile blockersVisual = visualCatalog.GetRequiredProfile(ObstacleFamily.SideBlockers);

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
            float widestVisualHalfWidth = visualCatalog.GetRequiredProfile(ObstacleFamily.SideBlockers).VisualHalfWidth;
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
            float widestVisualHalfWidth = visualCatalog.GetRequiredProfile(ObstacleFamily.SideBlockers).VisualHalfWidth;
            float requiredClearance = (gameplayPresentation.TrackLineWidth * 0.5f) + widestVisualHalfWidth + 0.08f;

            Assert.That(backgroundPresentationConfig.LaneQuietZoneHalfWidth, Is.GreaterThan(requiredClearance));
            Assert.That(backgroundPresentationConfig.MaxRuntimeSpriteCount, Is.LessThanOrEqualTo(3));
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

