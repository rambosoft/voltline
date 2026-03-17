using System.Collections;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Voltline.Core;
using Voltline.Data;
using Voltline.Gameplay;
using Voltline.Save;
using Voltline.UI;

namespace Voltline.Tests.PlayMode
{
    public sealed class CoreUiFlowPlayModeTests
    {
        [UnityTest]
        public IEnumerator MainMenuScene_OptionalExtrasAndPlayFlowWork()
        {
            string tempPath = Path.Combine(Application.temporaryCachePath, "voltline-core-ui-phase7-menu.json");
            SaveStorage.SetOverridePathForTests(tempPath);
            SaveStorage.DeleteProfile();
            SaveService.ResetInstanceForTests();
            yield return null;

            SceneManager.LoadScene(SceneCatalog.MainMenu, LoadSceneMode.Single);
            yield return null;
            yield return null;

            MainMenuView mainMenuView = Object.FindFirstObjectByType<MainMenuView>();
            Assert.That(mainMenuView, Is.Not.Null);
            Assert.That(mainMenuView.PublicTitle, Is.EqualTo("Voltline"));
            Assert.That(mainMenuView.Subtitle, Is.EqualTo("Keep the grid alive"));
            Assert.That(mainMenuView.PrimaryMenuHint, Is.EqualTo("Tap to flip sides"));
            Assert.That(mainMenuView.GetMenuEntryState(BrandingMenuEntryId.Daily), Is.EqualTo(BrandingMenuEntryState.Disabled));
            Assert.That(mainMenuView.GetMenuEntryState(BrandingMenuEntryId.Themes), Is.EqualTo(BrandingMenuEntryState.Enabled));
            Assert.That(mainMenuView.GetMenuEntryState(BrandingMenuEntryId.Best), Is.EqualTo(BrandingMenuEntryState.Enabled));
            Assert.That(mainMenuView.GetMenuEntryState(BrandingMenuEntryId.Settings), Is.EqualTo(BrandingMenuEntryState.Enabled));
            Assert.That(mainMenuView.HasMenuEntryButton(BrandingMenuEntryId.Daily), Is.True);
            Assert.That(mainMenuView.HasMenuEntryButton(BrandingMenuEntryId.Themes), Is.True);
            Assert.That(mainMenuView.HasMenuEntryButton(BrandingMenuEntryId.Best), Is.True);
            Assert.That(mainMenuView.HasMenuEntryButton(BrandingMenuEntryId.Settings), Is.True);

            mainMenuView.HandleOpenGridStatusPressed();
            yield return null;
            Assert.That(mainMenuView.IsGridStatusVisible, Is.True);

            mainMenuView.HandleCloseGridStatusPressed();
            yield return null;
            Assert.That(mainMenuView.IsGridStatusVisible, Is.False);

            mainMenuView.HandleOpenSettingsPressed();
            yield return null;
            Assert.That(mainMenuView.IsSettingsVisible, Is.True);
            Assert.That(mainMenuView.SettingsHasThemeSelectionSection, Is.False);

            mainMenuView.HandleCloseSettingsPressed();
            yield return null;
            Assert.That(mainMenuView.IsSettingsVisible, Is.False);

            mainMenuView.HandlePlayPressed();
            yield return null;
            Assert.That(mainMenuView.IsTutorialVisible, Is.True);
            Assert.That(SceneManager.GetActiveScene().name, Is.EqualTo(SceneCatalog.MainMenu));

            mainMenuView.HandleConfirmTutorialPressed();
            yield return null;
            yield return null;

            Assert.That(SceneManager.GetActiveScene().name, Is.EqualTo(SceneCatalog.Gameplay));

            SaveService.ResetInstanceForTests();
            SaveStorage.DeleteProfile();
            SaveStorage.ClearOverridePathForTests();
            yield return null;
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
        public IEnumerator GameplayScene_ResultPanelPrioritizesRetryFlowAndShareSurface()
        {
            SceneManager.LoadScene(SceneCatalog.Gameplay, LoadSceneMode.Single);
            yield return null;
            yield return null;

            GameManager gameManager = Object.FindFirstObjectByType<GameManager>();
            UIStateCoordinator coordinator = Object.FindFirstObjectByType<UIStateCoordinator>();
            ResultPanelView resultPanelView = Object.FindFirstObjectByType<ResultPanelView>();

            Assert.That(gameManager, Is.Not.Null);
            Assert.That(coordinator, Is.Not.Null);
            Assert.That(resultPanelView, Is.Not.Null);

            yield return WaitForState(gameManager, RunState.Results, 4f);
            Assert.That(coordinator.IsResultVisible, Is.True);
            Assert.That(resultPanelView.HasShareButton, Is.True);

            coordinator.HandleOpenSharePressed();
            yield return null;
            Assert.That(coordinator.IsShareVisible, Is.True);

            coordinator.HandleCloseSharePressed();
            yield return null;
            Assert.That(coordinator.IsShareVisible, Is.False);

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
