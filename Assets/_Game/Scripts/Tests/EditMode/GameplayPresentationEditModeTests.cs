#if UNITY_EDITOR
using NUnit.Framework;
using UnityEditor;
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
            float requiredClearance = (GameplayPresentationTuning.TrackLineWidth * 0.5f)
                + (GameplayPresentationTuning.PlayerScale * 0.5f)
                + GameplayPresentationTuning.PlayerLineClearance;

            Assert.That(gameBalance.SideOffset, Is.GreaterThan(requiredClearance));
        }

        [Test]
        public void PlayerCollisionProfile_StaysInsideVisibleDot()
        {
            float visualHalfExtent = GameplayPresentationTuning.PlayerScale * 0.5f;

            Assert.That(GameplayPresentationTuning.PlayerCollisionHalfWidth, Is.GreaterThan(0f));
            Assert.That(GameplayPresentationTuning.PlayerCollisionHalfHeight, Is.GreaterThan(0f));
            Assert.That(GameplayPresentationTuning.PlayerCollisionHalfWidth, Is.LessThan(visualHalfExtent));
            Assert.That(GameplayPresentationTuning.PlayerCollisionHalfHeight, Is.LessThanOrEqualTo(visualHalfExtent));
        }

        [Test]
        public void HazardProfiles_KeepReadableSpacingBetweenVisibleBounds()
        {
            HazardLayoutProfile spikes = HazardFamilyPresentation.GetLayoutProfile(ObstacleFamily.Spikes);
            HazardLayoutProfile blockers = HazardFamilyPresentation.GetLayoutProfile(ObstacleFamily.SideBlockers);

            Assert.That(spikes.MainScale.y, Is.GreaterThan(0f));
            Assert.That(blockers.MainScale.y, Is.GreaterThan(spikes.MainScale.y));
            Assert.That(HazardFamilyPresentation.GetRequiredHitDistanceSeparation(blockers.VisualHalfHeight, blockers.VisualHalfHeight),
                Is.GreaterThan(blockers.MainScale.y));
        }

        [Test]
        public void HazardCollisionProfiles_StayInsideVisibleBounds()
        {
            foreach (ObstacleFamily family in System.Enum.GetValues(typeof(ObstacleFamily)))
            {
                HazardLayoutProfile layout = HazardFamilyPresentation.GetLayoutProfile(family);
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
            float widestVisualHalfWidth = HazardFamilyPresentation.GetLayoutProfile(ObstacleFamily.SideBlockers).VisualHalfWidth;
            float requiredClearance = (GameplayPresentationTuning.TrackLineWidth * 0.5f) + widestVisualHalfWidth + 0.08f;

            Assert.That(gameBalance.SideOffset, Is.GreaterThan(requiredClearance));
        }
    }
}
#endif
