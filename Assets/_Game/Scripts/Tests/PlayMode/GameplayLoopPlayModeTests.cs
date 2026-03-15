using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Voltline.Core;
using Voltline.Gameplay;

namespace Voltline.Tests.PlayMode
{
    public sealed class GameplayLoopPlayModeTests
    {
        [UnityTest]
        public IEnumerator GameplayScene_ReachesActiveStateQuickly()
        {
            SceneManager.LoadScene(SceneCatalog.Gameplay, LoadSceneMode.Single);
            yield return null;
            yield return null;

            GameManager gameManager = Object.FindFirstObjectByType<GameManager>();
            Assert.That(gameManager, Is.Not.Null);

            yield return WaitForState(gameManager, RunState.Active, 1.5f);
        }

        [UnityTest]
        public IEnumerator GameplayScene_FirstBeatCanBeCleared_AndRestartResetsScore()
        {
            SceneManager.LoadScene(SceneCatalog.Gameplay, LoadSceneMode.Single);
            yield return null;
            yield return null;

            GameManager gameManager = Object.FindFirstObjectByType<GameManager>();
            ScoreSystem scoreSystem = Object.FindFirstObjectByType<ScoreSystem>();

            yield return WaitForState(gameManager, RunState.Active, 1.5f);

            gameManager.DebugHandleTap();
            yield return new WaitForSeconds(2.25f);

            Assert.That(scoreSystem.CurrentScore, Is.GreaterThanOrEqualTo(1));

            gameManager.RequestRestart();
            yield return null;
            yield return WaitForState(gameManager, RunState.Active, 1.5f);
            Assert.That(scoreSystem.CurrentScore, Is.EqualTo(0));
        }

        [UnityTest]
        public IEnumerator GameplayScene_NoTapEventuallyDies_AndTapRetries()
        {
            SceneManager.LoadScene(SceneCatalog.Gameplay, LoadSceneMode.Single);
            yield return null;
            yield return null;

            GameManager gameManager = Object.FindFirstObjectByType<GameManager>();
            Assert.That(gameManager, Is.Not.Null);

            yield return WaitForState(gameManager, RunState.Results, 4f);
            gameManager.DebugHandleTap();
            yield return WaitForState(gameManager, RunState.Active, 1.5f);
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