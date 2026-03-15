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
            float requiredClearance = (gameplayPresentation.TrackLineWidth * 0.5f)
                + (gameplayPresentation.PlayerVisualScale * 0.5f)
                + gameplayPresentation.PlayerLineClearance;

            Assert.That(gameBalance.SideOffset, Is.GreaterThan(requiredClearance));
        }

        [Test]
        public void PlayerCollisionProfile_StaysInsideVisibleDot()
        {
            GameplayPresentationConfig gameplayPresentation = AssetDatabase.LoadAssetAtPath<GameplayPresentationConfig>(ProjectConfigAssetPaths.GameplayPresentation);
            float visualHalfExtent = gameplayPresentation.PlayerVisualScale * 0.5f;

            Assert.That(gameplayPresentation.PlayerCollisionHalfWidth, Is.GreaterThan(0f));
            Assert.That(gameplayPresentation.PlayerCollisionHalfHeight, Is.GreaterThan(0f));
            Assert.That(gameplayPresentation.PlayerCollisionHalfWidth, Is.LessThan(visualHalfExtent));
            Assert.That(gameplayPresentation.PlayerCollisionHalfHeight, Is.LessThanOrEqualTo(visualHalfExtent));
        }

        [Test]
        public void HazardProfiles_KeepReadableSpacingBetweenVisibleBounds()
        {
            HazardPresentationCatalog catalog = AssetDatabase.LoadAssetAtPath<HazardPresentationCatalog>(ProjectConfigAssetPaths.HazardPresentationCatalog);
            HazardLayoutProfile spikes = catalog.GetRequiredProfile(ObstacleFamily.Spikes);
            HazardLayoutProfile blockers = catalog.GetRequiredProfile(ObstacleFamily.SideBlockers);

            Assert.That(spikes.VisualBoundsScale.y, Is.GreaterThan(0f));
            Assert.That(blockers.VisualBoundsScale.y, Is.GreaterThan(spikes.VisualBoundsScale.y));
            Assert.That(catalog.GetRequiredHitDistanceSeparation(blockers, blockers), Is.GreaterThan(blockers.VisualBoundsScale.y));
        }

        [Test]
        public void HazardCollisionProfiles_StayInsideVisibleBounds()
        {
            HazardPresentationCatalog catalog = AssetDatabase.LoadAssetAtPath<HazardPresentationCatalog>(ProjectConfigAssetPaths.HazardPresentationCatalog);

            foreach (ObstacleFamily family in System.Enum.GetValues(typeof(ObstacleFamily)))
            {
                HazardLayoutProfile layout = catalog.GetRequiredProfile(family);
                Assert.That(layout.CollisionHalfWidth, Is.GreaterThan(0f), family.ToString());
                Assert.That(layout.CollisionHalfHeight, Is.GreaterThan(0f), family.ToString());
                Assert.That(layout.CollisionHalfWidth, Is.LessThan(layout.VisualHalfWidth), family.ToString());
                Assert.That(layout.CollisionHalfHeight, Is.LessThanOrEqualTo(layout.VisualHalfHeight), family.ToString());
            }
        }

        [Test]
        public void HazardProfiles_ClearTheLineHorizontally()
        {
            GameBalanceConfig gameBalance = AssetDatabase.LoadAssetAtPath<GameBalanceConfig>(ProjectConfigAssetPaths.GameBalance);
            GameplayPresentationConfig gameplayPresentation = AssetDatabase.LoadAssetAtPath<GameplayPresentationConfig>(ProjectConfigAssetPaths.GameplayPresentation);
            HazardPresentationCatalog catalog = AssetDatabase.LoadAssetAtPath<HazardPresentationCatalog>(ProjectConfigAssetPaths.HazardPresentationCatalog);
            float widestVisualHalfWidth = catalog.GetRequiredProfile(ObstacleFamily.SideBlockers).VisualHalfWidth;
            float requiredClearance = (gameplayPresentation.TrackLineWidth * 0.5f) + widestVisualHalfWidth + 0.08f;

            Assert.That(gameBalance.SideOffset, Is.GreaterThan(requiredClearance));
        }
    }
}
#endif
