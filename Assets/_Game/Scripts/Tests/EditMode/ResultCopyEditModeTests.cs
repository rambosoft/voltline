#if UNITY_EDITOR
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using Voltline.Data;
using Voltline.UI;
namespace Voltline.Tests.EditMode
{
    public sealed class ResultCopyEditModeTests
    {
        private static readonly IReadOnlyList<int> Milestones = new List<int> { 10, 20, 30, 40, 50 };
        [Test]
        public void ResultCopy_NewBestUsesConfiguredCityRecordCopy()
        {
            ProductionCopyConfig copyConfig = AssetDatabase.LoadAssetAtPath<ProductionCopyConfig>(ProjectConfigAssetPaths.ProductionCopy);
            WorldProgressionConfig worldProgressionConfig = AssetDatabase.LoadAssetAtPath<WorldProgressionConfig>(ProjectConfigAssetPaths.WorldProgression);
            ResultPresentationCopy copy = ResultCopyUtility.BuildCopy(21, true, null, Milestones, worldProgressionConfig, copyConfig);
            Assert.That(copy.Title, Is.EqualTo("BEST TRANSFER"));
            Assert.That(copy.Message, Is.EqualTo("Best transfer in the city yet."));
        }
        [Test]
        public void ResultCopy_NearMilestoneUsesConfiguredAlmostCopy()
        {
            ProductionCopyConfig copyConfig = AssetDatabase.LoadAssetAtPath<ProductionCopyConfig>(ProjectConfigAssetPaths.ProductionCopy);
            WorldProgressionConfig worldProgressionConfig = AssetDatabase.LoadAssetAtPath<WorldProgressionConfig>(ProjectConfigAssetPaths.WorldProgression);
            ResultPresentationCopy copy = ResultCopyUtility.BuildCopy(18, false, null, Milestones, worldProgressionConfig, copyConfig);
            Assert.That(copy.Message, Is.EqualTo("Almost 20"));
        }
        [Test]
        public void ResultCopy_ExactMilestoneUsesConfiguredMilestoneLibrary()
        {
            ProductionCopyConfig copyConfig = AssetDatabase.LoadAssetAtPath<ProductionCopyConfig>(ProjectConfigAssetPaths.ProductionCopy);
            WorldProgressionConfig worldProgressionConfig = AssetDatabase.LoadAssetAtPath<WorldProgressionConfig>(ProjectConfigAssetPaths.WorldProgression);
            ResultPresentationCopy copy = ResultCopyUtility.BuildCopy(20, false, null, Milestones, worldProgressionConfig, copyConfig);
            Assert.That(copy.Message, Is.EqualTo("GRID STABLE"));
        }
        [Test]
        public void ResultCopy_DistrictProgressUsesConfiguredDistrictRestoreCopy()
        {
            ProductionCopyConfig copyConfig = AssetDatabase.LoadAssetAtPath<ProductionCopyConfig>(ProjectConfigAssetPaths.ProductionCopy);
            WorldProgressionConfig worldProgressionConfig = AssetDatabase.LoadAssetAtPath<WorldProgressionConfig>(ProjectConfigAssetPaths.WorldProgression);
            ResultPresentationCopy copy = ResultCopyUtility.BuildCopy(35, false, null, Milestones, worldProgressionConfig, copyConfig);
            Assert.That(copy.Message, Is.EqualTo("You restored 4 districts."));
        }
        [Test]
        public void ResultCopy_ZeroScoreUsesConfiguredFailureCopy()
        {
            ProductionCopyConfig copyConfig = AssetDatabase.LoadAssetAtPath<ProductionCopyConfig>(ProjectConfigAssetPaths.ProductionCopy);
            WorldProgressionConfig worldProgressionConfig = AssetDatabase.LoadAssetAtPath<WorldProgressionConfig>(ProjectConfigAssetPaths.WorldProgression);
            ResultPresentationCopy copy = ResultCopyUtility.BuildCopy(0, false, null, Milestones, worldProgressionConfig, copyConfig);
            Assert.That(copy.Title, Is.EqualTo("GRID FAILURE"));
            Assert.That(copy.Message, Is.EqualTo("The line died before the city woke."));
        }
        [Test]
        public void ResultCopy_FailureFamilyUsesFamilySpecificPowerGridCopy()
        {
            ProductionCopyConfig copyConfig = AssetDatabase.LoadAssetAtPath<ProductionCopyConfig>(ProjectConfigAssetPaths.ProductionCopy);
            WorldProgressionConfig worldProgressionConfig = AssetDatabase.LoadAssetAtPath<WorldProgressionConfig>(ProjectConfigAssetPaths.WorldProgression);
            ResultPresentationCopy copy = ResultCopyUtility.BuildCopy(14, false, ObstacleFamily.ActiveElectricHazards, Milestones, worldProgressionConfig, copyConfig);
            Assert.That(copy.Title, Is.EqualTo("OVERLOAD"));
            Assert.That(copy.Message, Is.EqualTo("The transfer spiked out in an electric surge."));
        }
    }
}
#endif

