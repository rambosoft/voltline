#if UNITY_EDITOR
using NUnit.Framework;
using Voltline.Editor;

namespace Voltline.Tests.EditMode
{
    public sealed class PresentationRefreshApprovalAuditEditModeTests
    {
        [Test]
        public void PresentationRefreshApprovalAudit_HasNoBlockingErrors()
        {
            PresentationRefreshApprovalAuditResult result = PresentationRefreshApprovalAudit.Validate();

            Assert.That(result.Errors, Is.Empty);
            Assert.That(result.Notes, Has.Some.Contains("Refresh approval gate requires config validation"));
            Assert.That(result.Notes, Has.Some.Contains("Controlled rollout is staged across 7 slices"));
        }
    }
}
#endif
