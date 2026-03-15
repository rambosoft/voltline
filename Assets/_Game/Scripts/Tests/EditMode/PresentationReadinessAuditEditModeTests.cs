#if UNITY_EDITOR
using NUnit.Framework;
using Voltline.Editor;

namespace Voltline.Tests.EditMode
{
    public sealed class PresentationReadinessAuditEditModeTests
    {
        [Test]
        public void PresentationReadinessAudit_HasNoBlockingErrors_AndKeepsCurrentBlockedWorkVisible()
        {
            PresentationReadinessAuditResult result = PresentationReadinessAudit.Validate();

            Assert.That(result.Errors, Is.Empty);
            Assert.That(result.Warnings, Has.Some.Contains("Player visuals remain procedural"));
            Assert.That(result.Warnings, Has.Some.Contains("Hazard visuals remain procedural"));
            Assert.That(result.Warnings, Has.Some.Contains("TrackManager still lacks a dedicated background presentation layer"));
        }
    }
}
#endif
