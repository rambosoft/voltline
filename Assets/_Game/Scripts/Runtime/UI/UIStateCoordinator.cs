using UnityEngine;
using UnityEngine.SceneManagement;
using Voltline.Core;
using Voltline.Data;
using Voltline.Gameplay;
using Voltline.Save;

namespace Voltline.UI
{
    public sealed class UIStateCoordinator : MonoBehaviour
    {
        private GameManager gameManager;
        private ScoreSystem scoreSystem;
        private ThemeConfig theme;
        private SaveService saveService;
        private HudView hudView;
        private PauseOverlayView pauseOverlayView;
        private ResultPanelView resultPanelView;
        private SettingsOverlayView settingsOverlayView;
        private bool isInitialized;

        public bool IsPauseVisible => pauseOverlayView != null && pauseOverlayView.IsVisible;
        public bool IsResultVisible => resultPanelView != null && resultPanelView.IsVisible;
        public bool IsSettingsVisible => settingsOverlayView != null && settingsOverlayView.IsVisible;

        public void Initialize(GameManager manager, ScoreSystem scores, ThemeConfig activeTheme, SaveService service)
        {
            if (isInitialized)
            {
                return;
            }

            gameManager = manager;
            scoreSystem = scores;
            theme = activeTheme;
            saveService = service;

            UIFactory.EnsureEventSystem();
            Canvas canvas = UIFactory.CreateCanvas("GameplayCanvas", transform);
            RectTransform safeAreaRoot = UIFactory.CreateSafeAreaRoot(canvas);

            hudView = gameObject.AddComponent<HudView>();
            hudView.Initialize(safeAreaRoot, theme, HandlePausePressed);
            hudView.SetScore(scoreSystem.CurrentScore);
            hudView.SetBestScore(saveService.BestScore);

            pauseOverlayView = gameObject.AddComponent<PauseOverlayView>();
            pauseOverlayView.Initialize(safeAreaRoot, theme, HandleResumePressed, HandleRetryPressed, HandleHomePressed, HandleOpenSettingsPressed);

            resultPanelView = gameObject.AddComponent<ResultPanelView>();
            resultPanelView.Initialize(safeAreaRoot, theme, HandleRetryPressed, HandleHomePressed);

            settingsOverlayView = gameObject.AddComponent<SettingsOverlayView>();
            settingsOverlayView.Initialize(safeAreaRoot, theme, saveService, HandleCloseSettingsPressed);

            scoreSystem.ScoreChanged += HandleScoreChanged;
            gameManager.RunStateChanged += HandleRunStateChanged;
            gameManager.PauseChanged += HandlePauseChanged;
            saveService.ProfileChanged += HandleProfileChanged;

            isInitialized = true;
        }

        private void OnDestroy()
        {
            if (scoreSystem != null)
            {
                scoreSystem.ScoreChanged -= HandleScoreChanged;
            }

            if (gameManager != null)
            {
                gameManager.RunStateChanged -= HandleRunStateChanged;
                gameManager.PauseChanged -= HandlePauseChanged;
            }

            if (saveService != null)
            {
                saveService.ProfileChanged -= HandleProfileChanged;
            }
        }

        public void HandlePausePressed()
        {
            if (gameManager == null || gameManager.CurrentState != RunState.Active || (settingsOverlayView != null && settingsOverlayView.IsVisible))
            {
                return;
            }

            gameManager.SetPaused(true);
        }

        public void HandleResumePressed()
        {
            settingsOverlayView.Hide();
            gameManager?.SetPaused(false);
        }

        public void HandleRetryPressed()
        {
            settingsOverlayView.Hide();
            pauseOverlayView.Hide();
            resultPanelView.Hide();
            gameManager?.RequestRestart();
        }

        public void HandleHomePressed()
        {
            settingsOverlayView.Hide();
            pauseOverlayView.Hide();
            resultPanelView.Hide();
            gameManager?.SetPaused(false);
            SceneManager.LoadScene(SceneCatalog.MainMenu, LoadSceneMode.Single);
        }

        public void HandleOpenSettingsPressed()
        {
            settingsOverlayView.Show();
        }

        public void HandleCloseSettingsPressed()
        {
            settingsOverlayView.Hide();
        }

        private void HandleScoreChanged(int score)
        {
            hudView.SetScore(score);
        }

        private void HandleRunStateChanged(RunState state)
        {
            if (state == RunState.Results)
            {
                int previousBest = saveService.BestScore;
                int finalScore = scoreSystem.CurrentScore;
                bool isNewBest = finalScore > previousBest;
                saveService.RecordRunScore(finalScore);
                resultPanelView.Show(finalScore, saveService.BestScore, isNewBest, BuildResultMessage(finalScore, isNewBest));
                pauseOverlayView.Hide();
                settingsOverlayView.Hide();
                return;
            }

            if (state == RunState.Starting || state == RunState.Active || state == RunState.Ready || state == RunState.Restarting)
            {
                resultPanelView.Hide();
            }
        }

        private void HandlePauseChanged(bool isPaused)
        {
            if (isPaused)
            {
                pauseOverlayView.Show();
                return;
            }

            pauseOverlayView.Hide();
            settingsOverlayView.Hide();
        }

        private void HandleProfileChanged()
        {
            hudView.SetBestScore(saveService.BestScore);
        }

        private static string BuildResultMessage(int score, bool isNewBest)
        {
            if (isNewBest)
            {
                return "New Best";
            }

            if (score <= 0)
            {
                return "One more run";
            }

            if (score < 5)
            {
                return "So close";
            }

            if (score < 15)
            {
                return $"You survived {score}";
            }

            return "Keep the line";
        }
    }
}
