using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Voltline.Audio;
using Voltline.Core;
using Voltline.Data;
using Voltline.Gameplay;
using Voltline.Save;
using Voltline.UI;
using Voltline.VFX;

namespace Voltline.Tests.PlayMode
{
    public sealed class SmokeRegressionPlayModeTests
    {
        [UnityTest]
        public IEnumerator GameplayScene_DefaultStartingScoreIsZero_AndTapFlipsPlayer()
        {
#if UNITY_EDITOR
            GameplaySceneInstaller.ClearEditorSessionDebugStartingScoreOverride();
#endif
            SceneManager.LoadScene(SceneCatalog.Gameplay, LoadSceneMode.Single);
            yield return null;
            yield return null;

            GameManager gameManager = Object.FindFirstObjectByType<GameManager>();
            GameplaySceneInstaller installer = Object.FindFirstObjectByType<GameplaySceneInstaller>();
            ScoreSystem scoreSystem = Object.FindFirstObjectByType<ScoreSystem>();
            PlayerController playerController = Object.FindFirstObjectByType<PlayerController>();
            PlayerVisualView playerVisualView = Object.FindFirstObjectByType<PlayerVisualView>();

            Assert.That(gameManager, Is.Not.Null);
            Assert.That(installer, Is.Not.Null);
            Assert.That(scoreSystem, Is.Not.Null);
            Assert.That(playerController, Is.Not.Null);
            Assert.That(playerVisualView, Is.Not.Null);

            yield return WaitForState(gameManager, RunState.Active, 1.5f);

            Assert.That(installer.DebugStartingScore, Is.EqualTo(0));
            Assert.That(scoreSystem.CurrentScore, Is.EqualTo(0));
            Assert.That(playerController.HasVisualView, Is.True);
            Assert.That(playerVisualView.CurrentPresentationState, Is.EqualTo(PlayerVisualPresentationStateId.Idle));

            PlayerSide initialSide = playerController.CurrentSide;
            gameManager.DebugHandleTap();
            yield return null;
            Assert.That(playerVisualView.CurrentPresentationState, Is.EqualTo(PlayerVisualPresentationStateId.Flip));
            yield return new WaitForSeconds(0.22f);

            Assert.That(playerController.CurrentSide, Is.Not.EqualTo(initialSide));
            Assert.That(playerVisualView.CurrentPresentationState, Is.EqualTo(PlayerVisualPresentationStateId.Idle));
            LogAssert.NoUnexpectedReceived();
        }

        [UnityTest]
        public IEnumerator GameplayScene_PauseFreezesTrackProgress_ThenResumeRestoresMotion()
        {
#if UNITY_EDITOR
            GameplaySceneInstaller.ClearEditorSessionDebugStartingScoreOverride();
#endif
            SceneManager.LoadScene(SceneCatalog.Gameplay, LoadSceneMode.Single);
            yield return null;
            yield return null;

            GameManager gameManager = Object.FindFirstObjectByType<GameManager>();
            TrackManager trackManager = Object.FindFirstObjectByType<TrackManager>();
            UIStateCoordinator coordinator = Object.FindFirstObjectByType<UIStateCoordinator>();
            HazardManager hazardManager = Object.FindFirstObjectByType<HazardManager>();

            Assert.That(gameManager, Is.Not.Null);
            Assert.That(trackManager, Is.Not.Null);
            Assert.That(coordinator, Is.Not.Null);
            Assert.That(hazardManager, Is.Not.Null);

            yield return WaitForState(gameManager, RunState.Active, 1.5f);
            yield return WaitForHazardVisuals(hazardManager, 2.5f);

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
#if UNITY_EDITOR
            GameplaySceneInstaller.ClearEditorSessionDebugStartingScoreOverride();
#endif
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

        [UnityTest]
        public IEnumerator GameplayScene_ThemeSequenceTransitionsAtConfiguredMilestoneWithinBackgroundAndFeedbackBudgets()
        {
#if UNITY_EDITOR
            GameplaySceneInstaller.ClearEditorSessionDebugStartingScoreOverride();
#endif
            SceneManager.LoadScene(SceneCatalog.Gameplay, LoadSceneMode.Single);
            yield return null;
            yield return null;

            GameManager gameManager = Object.FindFirstObjectByType<GameManager>();
            GameplaySceneInstaller installer = Object.FindFirstObjectByType<GameplaySceneInstaller>();
            ScoreSystem scoreSystem = Object.FindFirstObjectByType<ScoreSystem>();
            ThemePresentationController themePresentationController = Object.FindFirstObjectByType<ThemePresentationController>();
            BackgroundPresentationController backgroundPresentationController = Object.FindFirstObjectByType<BackgroundPresentationController>();
            PlayerVisualView playerVisualView = Object.FindFirstObjectByType<PlayerVisualView>();
            VfxService vfxService = Object.FindFirstObjectByType<VfxService>();
            AudioService audioService = AudioService.EnsureExists();

            Assert.That(gameManager, Is.Not.Null);
            Assert.That(installer, Is.Not.Null);
            Assert.That(scoreSystem, Is.Not.Null);
            Assert.That(themePresentationController, Is.Not.Null);
            Assert.That(backgroundPresentationController, Is.Not.Null);
            Assert.That(playerVisualView, Is.Not.Null);
            Assert.That(vfxService, Is.Not.Null);
            Assert.That(audioService, Is.Not.Null);

            SaveService saveService = SaveService.EnsureExists();
            saveService.RecordRunScore(20);
            saveService.SynchronizeThemeUnlocks(installer.ThemeCatalog);
            saveService.SetSelectedThemeId(installer.ThemeCatalog.DefaultThemeId);

            gameManager.RequestRestart();
            yield return null;
            yield return WaitForState(gameManager, RunState.Active, 1.5f);

            Assert.That(themePresentationController.CurrentThemeId, Is.EqualTo(installer.ThemeCatalog.DefaultThemeId));
            Assert.That(backgroundPresentationController.RuntimeLayerCount, Is.LessThanOrEqualTo(backgroundPresentationController.ConfiguredSpriteBudget));
            Assert.That(vfxService.CurrentThemeVfxProfileName, Does.Contain("NeonNight"));
            Assert.That(audioService.CurrentThemeAudioProfileName, Does.Contain("NeonNight"));

            for (int i = scoreSystem.CurrentScore; i < 20; i++)
            {
                scoreSystem.RegisterClearedBeat();
            }

            yield return null;

            Assert.That(themePresentationController.ActivatedTransitionCount, Is.EqualTo(1));
            Assert.That(themePresentationController.CurrentThemeId, Is.EqualTo("theme.candy-pop"));
            Assert.That(backgroundPresentationController.ActiveConfig, Is.Not.Null);
            Assert.That(backgroundPresentationController.ActiveConfig.name, Does.Contain("CandyPop"));
            Assert.That(backgroundPresentationController.RuntimeLayerCount, Is.LessThanOrEqualTo(backgroundPresentationController.ConfiguredSpriteBudget));
            Assert.That(playerVisualView.CurrentPresentationState, Is.EqualTo(PlayerVisualPresentationStateId.Milestone));
            Assert.That(vfxService.CurrentThemeVfxProfileName, Does.Contain("CandyPop"));
            Assert.That(audioService.CurrentThemeAudioProfileName, Does.Contain("CandyPop"));
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

        private static IEnumerator WaitForHazardVisuals(HazardManager hazardManager, float timeoutSeconds)
        {
            float elapsed = 0f;
            while (!hazardManager.AllActiveHazardsHaveVisualViews && elapsed < timeoutSeconds)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }

            Assert.That(hazardManager.AllActiveHazardsHaveVisualViews, Is.True);
        }
    }
}
