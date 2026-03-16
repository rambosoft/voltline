#if UNITY_EDITOR
using NUnit.Framework;
using UnityEditor;
using Voltline.Data;
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
    }
}
#endif
