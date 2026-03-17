#if UNITY_EDITOR
using NUnit.Framework;
using Voltline.Editor;

namespace Voltline.Tests.EditMode
{
    public sealed class PresentationReadinessAuditEditModeTests
    {
        [Test]
        public void PresentationReadinessAudit_HasNoBlockingErrors_AndReflectsLiveWireCityRollout()
        {
            PresentationReadinessAuditResult result = PresentationReadinessAudit.Validate();

            Assert.That(result.Errors, Is.Empty);
            Assert.That(result.Notes, Has.Some.Contains("Theme-owned VFX and audio variation is now routed through ThemeVfxProfile and ThemeAudioProfile"));
            Assert.That(result.Notes, Has.Some.Contains("Branding and UI production support is now routed through BrandingPresentationConfig, ProductionCopyConfig, and UIThemeConfig references on ThemeCatalog"));
            Assert.That(result.Notes, Has.Some.Contains("Live Wire City world progression"));
            Assert.That(result.Notes, Has.Some.Contains("theme application is now owned by ThemePresentationController via ThemeCatalog and WorldProgressionController"));
            Assert.That(result.Notes, Has.Some.Contains("Grid promoted through the former Themes slot"));
            Assert.That(result.Notes, Has.Some.Contains("Optional extras now remain additive inside the existing scene model"));
        }
    }
}
#endif
