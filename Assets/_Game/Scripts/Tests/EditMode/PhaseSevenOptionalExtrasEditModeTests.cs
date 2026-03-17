#if UNITY_EDITOR
using NUnit.Framework;
using UnityEditor;
using Voltline.Data;

namespace Voltline.Tests.EditMode
{
    public sealed class PhaseSevenOptionalExtrasEditModeTests
    {
        [Test]
        public void ProductionCopyConfig_FormatsGridStatusAndShareCopyForOptionalExtras()
        {
            ProductionCopyConfig copy = AssetDatabase.LoadAssetAtPath<ProductionCopyConfig>(ProjectConfigAssetPaths.ProductionCopy);
            Assert.That(copy, Is.Not.Null);
            Assert.That(copy.FormatGridStatusSummary(24, 3), Is.EqualTo("Best 24 | 3 districts online"));
            Assert.That(copy.FormatGridStatusLockedRequirement(20), Is.EqualTo("Reach 20"));
            Assert.That(copy.FormatGridStatusScoreBand(10, 19), Is.EqualTo("10-19"));
            Assert.That(copy.FormatGridStatusScoreBand(40, -1), Is.EqualTo("40+"));
            Assert.That(copy.FormatShareSummary(35, "Surge City"), Is.EqualTo("Score 35 | Surge City"));
            Assert.That(copy.FormatShareFlavor(4, "Surge City"), Is.EqualTo("4 districts online in Surge City."));
            Assert.That(copy.TutorialTitle, Is.EqualTo("KEEP THE GRID ALIVE"));
            Assert.That(copy.TutorialActionLabel, Is.EqualTo("Start Run"));
        }
    }
}
#endif
