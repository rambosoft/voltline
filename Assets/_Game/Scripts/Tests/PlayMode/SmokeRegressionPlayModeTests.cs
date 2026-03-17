using System.Collections;
using System.Collections.Generic;
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
        public IEnumerator GameplayScene_WorldProgressionMovesDistrictsWithoutChangingBaseThemeIdentity()
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
            WorldProgressionController worldProgressionController = Object.FindFirstObjectByType<WorldProgressionController>();
            BackgroundPresentationController backgroundPresentationController = Object.FindFirstObjectByType<BackgroundPresentationController>();
            PlayerVisualView playerVisualView = Object.FindFirstObjectByType<PlayerVisualView>();
            VfxService vfxService = Object.FindFirstObjectByType<VfxService>();
            AudioService audioService = AudioService.EnsureExists();

            Assert.That(gameManager, Is.Not.Null);
            Assert.That(installer, Is.Not.Null);
            Assert.That(scoreSystem, Is.Not.Null);
            Assert.That(themePresentationController, Is.Not.Null);
            Assert.That(worldProgressionController, Is.Not.Null);
            Assert.That(backgroundPresentationController, Is.Not.Null);
            Assert.That(playerVisualView, Is.Not.Null);
            Assert.That(vfxService, Is.Not.Null);
            Assert.That(audioService, Is.Not.Null);

            SaveService saveService = SaveService.EnsureExists();
            saveService.SynchronizeThemeUnlocks(installer.ThemeCatalog);
            saveService.SetSelectedThemeId(installer.ThemeCatalog.DefaultThemeId);

            gameManager.RequestRestart();
            yield return null;
            yield return WaitForState(gameManager, RunState.Active, 1.5f);

            Assert.That(themePresentationController.CurrentThemeId, Is.EqualTo("theme.live-wire-city"));
            Assert.That(themePresentationController.CurrentDistrictId, Is.EqualTo("district.failing-grid"));
            Assert.That(backgroundPresentationController.RuntimeLayerCount, Is.LessThanOrEqualTo(backgroundPresentationController.ConfiguredSpriteBudget));
            Assert.That(vfxService.CurrentThemeVfxProfileId, Is.EqualTo("theme.live-wire-city.vfx"));
            Assert.That(audioService.CurrentThemeAudioProfileId, Is.EqualTo("theme.live-wire-city.audio"));

            for (int i = scoreSystem.CurrentScore; i < 40; i++)
            {
                scoreSystem.RegisterClearedBeat();
            }

            yield return null;

            Assert.That(themePresentationController.CurrentThemeId, Is.EqualTo("theme.live-wire-city"));
            Assert.That(themePresentationController.CurrentDistrictId, Is.EqualTo("district.overclock-city"));
            Assert.That(worldProgressionController.ActivatedTransitionCount, Is.GreaterThanOrEqualTo(5));
            Assert.That(worldProgressionController.ActivatedMilestoneReactionCount, Is.GreaterThanOrEqualTo(4));
            Assert.That(backgroundPresentationController.RuntimeLayerCount, Is.LessThanOrEqualTo(backgroundPresentationController.ConfiguredSpriteBudget));
            Assert.That(playerVisualView.CurrentPresentationState, Is.EqualTo(PlayerVisualPresentationStateId.Milestone));
            LogAssert.NoUnexpectedReceived();
        }

        [UnityTest]
        public IEnumerator GameplayScene_DebugProgressionReachesFortyPlusWhileEncounteringMultipleHazardFamilies()
        {
#if UNITY_EDITOR
            GameplaySceneInstaller.ClearEditorSessionDebugStartingScoreOverride();
#endif
            SceneManager.LoadScene(SceneCatalog.Gameplay, LoadSceneMode.Single);
            yield return null;
            yield return null;

            GameManager gameManager = Object.FindFirstObjectByType<GameManager>();
            ScoreSystem scoreSystem = Object.FindFirstObjectByType<ScoreSystem>();
            HazardManager hazardManager = Object.FindFirstObjectByType<HazardManager>();
            DifficultyDirector difficultyDirector = Object.FindFirstObjectByType<DifficultyDirector>();
            ThemePresentationController themePresentationController = Object.FindFirstObjectByType<ThemePresentationController>();
            PlayerController playerController = Object.FindFirstObjectByType<PlayerController>();
            TrackManager trackManager = Object.FindFirstObjectByType<TrackManager>();
            Assert.That(gameManager, Is.Not.Null);
            Assert.That(scoreSystem, Is.Not.Null);
            Assert.That(hazardManager, Is.Not.Null);
            Assert.That(difficultyDirector, Is.Not.Null);
            Assert.That(themePresentationController, Is.Not.Null);
            Assert.That(playerController, Is.Not.Null);
            Assert.That(trackManager, Is.Not.Null);

            GameplayAutoplayScenario scenario = GameplayAutoplayTestDriver.MarathonForty;
            float originalTimeScale = Time.timeScale;
            Time.timeScale = scenario.TimeScale;

            try
            {
                yield return WaitForState(gameManager, RunState.Active, 2f);

                HashSet<ObstacleFamily> encounteredFamilies = new();
                List<HazardDebugSnapshot> snapshots = new();
                float elapsed = 0f;
                while (elapsed < scenario.MaxUnscaledSeconds && scoreSystem.CurrentScore < scenario.TargetScore)
                {
                    Assert.That(gameManager.CurrentState, Is.EqualTo(RunState.Active));
                    GameplayAutoplayTestDriver.Tick(gameManager, playerController, hazardManager, trackManager.PlayerAnchorY, scenario, encounteredFamilies, snapshots);
                    elapsed += Time.unscaledDeltaTime;
                    yield return null;
                }

                Assert.That(scoreSystem.CurrentScore, Is.GreaterThanOrEqualTo(scenario.TargetScore));
                Assert.That(themePresentationController.CurrentDistrictId, Is.EqualTo("district.overclock-city"));
                Assert.That(encounteredFamilies.Count, Is.GreaterThanOrEqualTo(scenario.MinimumEncounteredFamilies));
                Assert.That(encounteredFamilies.Contains(ObstacleFamily.SharpUtilityHazards), Is.True);
                Assert.That(encounteredFamilies.Overlaps(new[]
                {
                    ObstacleFamily.ActiveElectricHazards,
                    ObstacleFamily.RotatingIndustrialHazards,
                    ObstacleFamily.SidePressureHazards,
                    ObstacleFamily.BrokenConduitSections,
                    ObstacleFamily.GroundedBlockers,
                }), Is.True);

                List<ObstacleConfig> eligibleLateGame = new();
                difficultyDirector.PopulateEligibleObstacleConfigs(scenario.TargetScore, eligibleLateGame);
                HashSet<ObstacleFamily> eligibleFamilies = new();
                for (int i = 0; i < eligibleLateGame.Count; i++)
                {
                    if (eligibleLateGame[i] != null)
                    {
                        eligibleFamilies.Add(eligibleLateGame[i].Family);
                    }
                }

                Assert.That(eligibleFamilies.Contains(ObstacleFamily.ActiveElectricHazards), Is.True);
                Assert.That(eligibleFamilies.Contains(ObstacleFamily.RotatingIndustrialHazards), Is.True);
                Assert.That(eligibleFamilies.Contains(ObstacleFamily.SidePressureHazards), Is.True);
                Assert.That(eligibleFamilies.Contains(ObstacleFamily.BrokenConduitSections), Is.True);
                LogAssert.NoUnexpectedReceived();
            }
            finally
            {
                Time.timeScale = originalTimeScale;
            }
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


