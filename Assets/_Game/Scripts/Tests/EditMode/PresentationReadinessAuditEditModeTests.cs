#if UNITY_EDITOR
using NUnit.Framework;
using Voltline.Editor;

namespace Voltline.Tests.EditMode
{
    public sealed class PresentationReadinessAuditEditModeTests
    {
        [Test]
        public void PresentationReadinessAudit_HasNoBlockingErrors_AndReflectsUnlockedPipelines()
        {
            PresentationReadinessAuditResult result = PresentationReadinessAudit.Validate();

            Assert.That(result.Errors, Is.Empty);
            Assert.That(result.Warnings, Has.None.Contains("VfxService and AudioService still lean on procedural fallback content"));
            Assert.That(result.Notes, Has.Some.Contains("Theme-owned VFX and audio variation is now routed through ThemeVfxProfile and ThemeAudioProfile"));
            Assert.That(result.Notes, Has.Some.Contains("Player, obstacle, background, theme-transition, VFX, and audio refresh slices are now structurally unlocked"));
        }
    }
}
#endif
