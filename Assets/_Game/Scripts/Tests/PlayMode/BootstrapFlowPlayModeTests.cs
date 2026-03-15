using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Voltline.Core;

namespace Voltline.Tests.PlayMode
{
    public sealed class BootstrapFlowPlayModeTests
    {
        [UnityTest]
        public IEnumerator BootstrapScene_RoutesToMainMenu()
        {
            SceneManager.LoadScene(SceneCatalog.Bootstrap, LoadSceneMode.Single);

            yield return null;
            yield return null;

            Assert.That(SceneManager.GetActiveScene().name, Is.EqualTo(SceneCatalog.MainMenu));
        }

        [UnityTest]
        public IEnumerator BootstrapScene_AppliesRuntimePerformanceBaseline()
        {
            SceneManager.LoadScene(SceneCatalog.Bootstrap, LoadSceneMode.Single);

            yield return null;
            yield return null;

            Assert.That(Application.targetFrameRate, Is.EqualTo(60));
            Assert.That(QualitySettings.vSyncCount, Is.EqualTo(0));
            LogAssert.NoUnexpectedReceived();
        }
    }
}
