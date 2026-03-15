using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Voltline.Core;
using Voltline.Gameplay;
using Voltline.UI;

namespace Voltline.Tests.PlayMode
{
    public sealed class CoreUiFlowPlayModeTests
    {
        [UnityTest]
        public IEnumerator MainMenuScene_SettingsToggleAndPlayFlowWork()
        {
            SceneManager.LoadScene(SceneCatalog.MainMenu, LoadSceneMode.Single);
            yield return null;
            yield return null;

            MainMenuView mainMenuView = Object.FindFirstObjectByType<MainMenuView>();
            Assert.That(mainMenuView, Is.Not.Null);

            mainMenuView.HandleOpenSettingsPressed();
            yield return null;
            Assert.That(mainMenuView.IsSettingsVisible, Is.True);

            mainMenuView.HandleCloseSettingsPressed();
            yield return null;
            Assert.That(mainMenuView.IsSettingsVisible, Is.False);

            mainMenuView.HandlePlayPressed();
            yield return null;
            yield return null;

            Assert.That(SceneManager.GetActiveScene().name, Is.EqualTo(SceneCatalog.Gameplay));
        }

        [UnityTest]
        public IEnumerator GameplayScene_PauseResumeAndHomeFlowWork()
        {
            SceneManager.LoadScene(SceneCatalog.Gameplay, LoadSceneMode.Single);
            yield return null;
            yield return null;

            GameManager gameManager = Object.FindFirstObjectByType<GameManager>();
            UIStateCoordinator coordinator = Object.FindFirstObjectByType<UIStateCoordinator>();

            Assert.That(gameManager, Is.Not.Null);
            Assert.That(coordinator, Is.Not.Null);

            yield return WaitForState(gameManager, RunState.Active, 1.5f);

            coordinator.HandlePausePressed();
            yield return null;
            Assert.That(gameManager.IsPaused, Is.True);
            Assert.That(coordinator.IsPauseVisible, Is.True);

            coordinator.HandleResumePressed();
            yield return null;
            Assert.That(gameManager.IsPaused, Is.False);
            Assert.That(coordinator.IsPauseVisible, Is.False);

            coordinator.HandlePausePressed();
            yield return null;
            coordinator.HandleHomePressed();
            yield return null;
            yield return null;

            Assert.That(SceneManager.GetActiveScene().name, Is.EqualTo(SceneCatalog.MainMenu));
        }

        [UnityTest]
        public IEnumerator GameplayScene_ResultPanelPrioritizesRetryFlow()
        {
            SceneManager.LoadScene(SceneCatalog.Gameplay, LoadSceneMode.Single);
            yield return null;
            yield return null;

            GameManager gameManager = Object.FindFirstObjectByType<GameManager>();
            UIStateCoordinator coordinator = Object.FindFirstObjectByType<UIStateCoordinator>();

            Assert.That(gameManager, Is.Not.Null);
            Assert.That(coordinator, Is.Not.Null);

            yield return WaitForState(gameManager, RunState.Results, 4f);
            Assert.That(coordinator.IsResultVisible, Is.True);

            coordinator.HandleRetryPressed();
            yield return null;
            yield return WaitForState(gameManager, RunState.Active, 1.5f);
            Assert.That(coordinator.IsResultVisible, Is.False);
        }

        private static IEnumerator WaitForState(GameManager gameManager, RunState state, float timeoutSeconds)
        {
            float elapsed = 0f;
            while (gameManager.CurrentState != state && elapsed < timeoutSeconds)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }

            Assert.That(gameManager.CurrentState, Is.EqualTo(state));
        }
    }
}
