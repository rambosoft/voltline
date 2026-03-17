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
        private ThemeCatalog themeCatalog;
        private SaveService saveService;
        private ProductionCopyConfig productionCopyConfig;
        private BrandingPresentationConfig brandingPresentationConfig;
        private HudView hudView;
        private PauseOverlayView pauseOverlayView;
        private ResultPanelView resultPanelView;
        private SettingsOverlayView settingsOverlayView;
        private ShareOverlayView shareOverlayView;
        private SharePresentationCopy currentShareCopy;
        private bool hasShareCopy;
        private bool isInitialized;

        public bool IsPauseVisible => pauseOverlayView != null && pauseOverlayView.IsVisible;
        public bool IsResultVisible => resultPanelView != null && resultPanelView.IsVisible;
        public bool IsSettingsVisible => settingsOverlayView != null && settingsOverlayView.IsVisible;
        public bool IsShareVisible => shareOverlayView != null && shareOverlayView.IsVisible;

        public void Initialize(GameManager manager, ScoreSystem scores, ThemeConfig activeTheme, ThemeCatalog catalog, SaveService service)
        {
            if (isInitialized)
            {
                return;
            }

            gameManager = manager;
            scoreSystem = scores;
            theme = activeTheme;
            themeCatalog = catalog;
            saveService = service;
            productionCopyConfig = themeCatalog != null ? themeCatalog.ProductionCopyConfig : null;
            brandingPresentationConfig = themeCatalog != null ? themeCatalog.BrandingPresentationConfig : null;

            UIFactory.EnsureEventSystem();
            Canvas canvas = UIFactory.CreateCanvas("GameplayCanvas", transform);
            RectTransform safeAreaRoot = UIFactory.CreateSafeAreaRoot(canvas);

            hudView = gameObject.AddComponent<HudView>();
            hudView.Initialize(safeAreaRoot, theme, themeCatalog, HandlePausePressed);
            hudView.SetScore(scoreSystem.CurrentScore);
            hudView.SetBestScore(saveService.BestScore);

            pauseOverlayView = gameObject.AddComponent<PauseOverlayView>();
            pauseOverlayView.Initialize(safeAreaRoot, theme, themeCatalog, HandleResumePressed, HandleRetryPressed, HandleHomePressed, HandleOpenSettingsPressed);

            resultPanelView = gameObject.AddComponent<ResultPanelView>();
            resultPanelView.Initialize(safeAreaRoot, theme, themeCatalog, HandleRetryPressed, HandleHomePressed, brandingPresentationConfig != null && brandingPresentationConfig.ShowShareEntry ? HandleOpenSharePressed : null);

            settingsOverlayView = gameObject.AddComponent<SettingsOverlayView>();
            settingsOverlayView.Initialize(safeAreaRoot, theme, themeCatalog, saveService, HandleCloseSettingsPressed);

            if (brandingPresentationConfig != null && brandingPresentationConfig.ShowShareEntry)
            {
                shareOverlayView = gameObject.AddComponent<ShareOverlayView>();
                shareOverlayView.Initialize(safeAreaRoot, theme, themeCatalog, HandleCloseSharePressed);
            }

            scoreSystem.ScoreChanged += HandleScoreChanged;
            scoreSystem.MilestoneReached += HandleMilestoneReached;
            gameManager.RunStateChanged += HandleRunStateChanged;
            gameManager.PauseChanged += HandlePauseChanged;
            saveService.ProfileChanged += HandleProfileChanged;

            isInitialized = true;
        }

        public void ApplyTheme(ThemeConfig activeTheme)
        {
            theme = activeTheme;
            hudView?.ApplyTheme(activeTheme);
            pauseOverlayView?.ApplyTheme(activeTheme);
            resultPanelView?.ApplyTheme(activeTheme);
            settingsOverlayView?.ApplyTheme(activeTheme);
            shareOverlayView?.ApplyTheme(activeTheme);
        }

        public void ApplyWorldDistrict(WorldDistrictStateDefinition districtState)
        {
            if (districtState == null)
            {
                return;
            }

            hudView?.SetDistrictStatus(districtState.StatusLabel);
        }

        public void ShowMilestoneMessage(int milestone)
        {
            if (productionCopyConfig != null && productionCopyConfig.TryGetLatestMilestoneMessage(milestone, out string message))
            {
                hudView?.ShowMilestoneMessage(message);
            }
        }

        private void OnDestroy()
        {
            if (scoreSystem != null)
            {
                scoreSystem.ScoreChanged -= HandleScoreChanged;
                scoreSystem.MilestoneReached -= HandleMilestoneReached;
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
            shareOverlayView?.Hide();
            gameManager?.SetPaused(false);
        }

        public void HandleRetryPressed()
        {
            settingsOverlayView.Hide();
            pauseOverlayView.Hide();
            resultPanelView.Hide();
            shareOverlayView?.Hide();
            gameManager?.RequestRestart();
        }

        public void HandleHomePressed()
        {
            settingsOverlayView.Hide();
            pauseOverlayView.Hide();
            resultPanelView.Hide();
            shareOverlayView?.Hide();
            gameManager?.SetPaused(false);
            SceneManager.LoadScene(SceneCatalog.MainMenu, LoadSceneMode.Single);
        }

        public void HandleOpenSettingsPressed()
        {
            shareOverlayView?.Hide();
            settingsOverlayView.Show();
        }

        public void HandleCloseSettingsPressed()
        {
            settingsOverlayView.Hide();
        }

        public void HandleOpenSharePressed()
        {
            if (shareOverlayView == null || !hasShareCopy)
            {
                return;
            }

            shareOverlayView.Show(currentShareCopy);
        }

        public void HandleCloseSharePressed()
        {
            shareOverlayView?.Hide();
        }

        private void HandleScoreChanged(int score)
        {
            hudView.SetScore(score);
            if (score > 0)
            {
                hudView.PlayScorePop();
            }
        }

        private void HandleMilestoneReached(int milestone)
        {
            hudView.PlayMilestonePulse();
            ShowMilestoneMessage(milestone);
        }

        private void HandleRunStateChanged(RunState state)
        {
            if (state == RunState.Results)
            {
                int previousBest = saveService.BestScore;
                int finalScore = scoreSystem.CurrentScore;
                bool isNewBest = finalScore > previousBest;
                saveService.RecordRunScore(finalScore);
                saveService.SynchronizeThemeUnlocks(themeCatalog);
                WorldProgressionConfig worldProgressionConfig = theme != null ? theme.ResolveWorldProgressionConfig() : null;
                ResultPresentationCopy copy = ResultCopyUtility.BuildCopy(finalScore, isNewBest, gameManager != null ? gameManager.LastFailureFamily : null, scoreSystem.MilestoneThresholds, worldProgressionConfig, productionCopyConfig);
                resultPanelView.Show(finalScore, saveService.BestScore, isNewBest, copy);
                currentShareCopy = BuildShareCopy(finalScore, worldProgressionConfig);
                hasShareCopy = brandingPresentationConfig != null && brandingPresentationConfig.ShowShareEntry;
                pauseOverlayView.Hide();
                settingsOverlayView.Hide();
                shareOverlayView?.Hide();
                return;
            }

            if (state == RunState.Starting || state == RunState.Active || state == RunState.Ready || state == RunState.Restarting)
            {
                resultPanelView.Hide();
                shareOverlayView?.Hide();
                hasShareCopy = false;
            }
        }

        private void HandlePauseChanged(bool isPaused)
        {
            if (isPaused)
            {
                shareOverlayView?.Hide();
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

        private SharePresentationCopy BuildShareCopy(int finalScore, WorldProgressionConfig worldProgressionConfig)
        {
            WorldDistrictStateDefinition district = worldProgressionConfig != null ? worldProgressionConfig.GetRequiredDistrictForScore(finalScore) : null;
            int districtsOnline = worldProgressionConfig != null ? worldProgressionConfig.GetRestoredDistrictCount(finalScore) : 0;
            string districtName = district != null ? district.DisplayName : "Failing Grid";
            string summary = productionCopyConfig != null
                ? productionCopyConfig.FormatShareSummary(finalScore, districtName)
                : $"Score {finalScore} | {districtName}";
            string flavor = productionCopyConfig != null
                ? productionCopyConfig.FormatShareFlavor(districtsOnline, districtName)
                : $"{districtsOnline} districts online in {districtName}.";
            string clipboard = $"Voltline\n{summary}\n{flavor}";
            return new SharePresentationCopy(summary, flavor, clipboard);
        }
    }
}
