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
    public sealed class SmokeRegressionPlayModeTests
    {
        [UnityTest]
        public IEnumerator GameplayScene_DefaultStartingScoreIsZero_AndTapFlipsPlayer()
        {
            SceneManager.LoadScene(SceneCatalog.Gameplay, LoadSceneMode.Single);
            yield return null;
            yield return null;

            GameManager gameManager = Object.FindFirstObjectByType<GameManager>();
            GameplaySceneInstaller installer = Object.FindFirstObjectByType<GameplaySceneInstaller>();
            ScoreSystem scoreSystem = Object.FindFirstObjectByType<ScoreSystem>();
            PlayerController playerController = Object.FindFirstObjectByType<PlayerController>();

            Assert.That(gameManager, Is.Not.Null);
            Assert.That(installer, Is.Not.Null);
            Assert.That(scoreSystem, Is.Not.Null);
            Assert.That(playerController, Is.Not.Null);

            yield return WaitForState(gameManager, RunState.Active, 1.5f);

            Assert.That(installer.DebugStartingScore, Is.EqualTo(0));
            Assert.That(scoreSystem.CurrentScore, Is.EqualTo(0));

            PlayerSide initialSide = playerController.CurrentSide;
            gameManager.DebugHandleTap();
            yield return new WaitForSeconds(0.18f);

            Assert.That(playerController.CurrentSide, Is.Not.EqualTo(initialSide));
            LogAssert.NoUnexpectedReceived();
        }

        [UnityTest]
        public IEnumerator GameplayScene_PauseFreezesTrackProgress_ThenResumeRestoresMotion()
        {
            SceneManager.LoadScene(SceneCatalog.Gameplay, LoadSceneMode.Single);
            yield return null;
            yield return null;

            GameManager gameManager = Object.FindFirstObjectByType<GameManager>();
            TrackManager trackManager = Object.FindFirstObjectByType<TrackManager>();
            UIStateCoordinator coordinator = Object.FindFirstObjectByType<UIStateCoordinator>();

            Assert.That(gameManager, Is.Not.Null);
            Assert.That(trackManager, Is.Not.Null);
            Assert.That(coordinator, Is.Not.Null);

            yield return WaitForState(gameManager, RunState.Active, 1.5f);
            float initialDistance = trackManager.TravelDistance;
            yield return new WaitForSeconds(0.25f);
            Assert.That(trackManager.TravelDistance, Is.GreaterThan(initialDistance));

            coordinator.HandlePausePressed();
            yield return null;
            float pausedDistance = trackManager.TravelDistance;
            yield return new WaitForSeconds(0.25f);
            Assert.That(trackManager.TravelDistance, Is.EqualTo(pausedDistance).Within(0.001f));

            coordinator.HandleResumePressed();
            yield return new WaitForSeconds(0.25f);
            Assert.That(trackManager.TravelDistance, Is.GreaterThan(pausedDistance));
            LogAssert.NoUnexpectedReceived();
        }

        [UnityTest]
        public IEnumerator GameplayScene_ResultHomeReturnsSafelyToMainMenu()
        {
            SceneManager.LoadScene(SceneCatalog.Gameplay, LoadSceneMode.Single);
            yield return null;
            yield return null;

            GameManager gameManager = Object.FindFirstObjectByType<GameManager>();
            UIStateCoordinator coordinator = Object.FindFirstObjectByType<UIStateCoordinator>();

            Assert.That(gameManager, Is.Not.Null);
            Assert.That(coordinator, Is.Not.Null);

            yield return WaitForState(gameManager, RunState.Results, 4f);
            coordinator.HandleHomePressed();
            yield return null;
            yield return null;

            Assert.That(SceneManager.GetActiveScene().name, Is.EqualTo(SceneCatalog.MainMenu));
            LogAssert.NoUnexpectedReceived();
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
