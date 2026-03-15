#if UNITY_EDITOR
using System.Collections.Generic;
using NUnit.Framework;
using Voltline.UI;

namespace Voltline.Tests.EditMode
{
    public sealed class ResultCopyEditModeTests
    {
        private static readonly IReadOnlyList<int> Milestones = new List<int> { 10, 20, 30, 50 };

        [Test]
        public void ResultCopy_NewBestTakesPriority()
        {
            Assert.That(ResultCopyUtility.BuildMessage(21, true, Milestones), Is.EqualTo("New Best"));
        }

        [Test]
        public void ResultCopy_NearMilestoneUsesAlmostCopy()
        {
            Assert.That(ResultCopyUtility.BuildMessage(18, false, Milestones), Is.EqualTo("Almost 20"));
        }

        [Test]
        public void ResultCopy_EarlyFailureUsesSoClose()
        {
            Assert.That(ResultCopyUtility.BuildMessage(3, false, Milestones), Is.EqualTo("So close"));
        }

        [Test]
        public void ResultCopy_MidRunUsesSurvivedCopy()
        {
            Assert.That(ResultCopyUtility.BuildMessage(12, false, Milestones), Is.EqualTo("You survived 12"));
        }
    }
}
#endif
