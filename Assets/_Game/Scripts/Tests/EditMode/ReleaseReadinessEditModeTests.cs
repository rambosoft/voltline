#if UNITY_EDITOR
using NUnit.Framework;
using Voltline.Editor;

namespace Voltline.Tests.EditMode
{
    public sealed class ReleaseReadinessEditModeTests
    {
        [Test]
        public void ReleaseAudit_HasNoBlockingErrors()
        {
            ReleaseAuditResult result = ReleaseReadinessAudit.Validate();

            Assert.That(result.Errors, Is.Empty);
        }
    }
}
#endif
