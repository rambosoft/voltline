using UnityEngine;
using Voltline.Audio;
using Voltline.Data;
using Voltline.Save;
using Voltline.UI;
using Voltline.VFX;

namespace Voltline.Gameplay
{
    public sealed class ThemePresentationController : MonoBehaviour
    {
        private ThemeCatalog themeCatalog;
        private SaveService saveService;
        private GameManager gameManager;
        private ScoreSystem scoreSystem;
        private BackgroundPresentationController backgroundController;
        private BackgroundPresentationConfig defaultBackgroundConfig;
        private TrackManager trackManager;
        private PlayerController playerController;
        private HazardManager hazardManager;
        private UIStateCoordinator uiStateCoordinator;
        private AudioService audioService;
        private VfxService vfxService;
        private WorldProgressionController worldProgressionController;
        private ThemeConfig currentTheme;
        private bool isInitialized;

        public string CurrentThemeId => currentTheme != null ? currentTheme.ThemeId : string.Empty;
        public string CurrentDistrictId => worldProgressionController != null ? worldProgressionController.CurrentDistrictId : string.Empty;
        public int ActivatedTransitionCount => worldProgressionController != null ? worldProgressionController.ActivatedTransitionCount : 0;
        public int ActivatedMilestoneReactionCount => worldProgressionController != null ? worldProgressionController.ActivatedMilestoneReactionCount : 0;

        public void Initialize(
            ThemeCatalog catalog,
            ThemeSequenceConfig sequenceConfig,
            SaveService service,
            GameManager manager,
            ScoreSystem scores,
            BackgroundPresentationController background,
            BackgroundPresentationConfig backgroundConfig,
            TrackManager track,
            PlayerController player,
            HazardManager hazards,
            WorldProgressionController progression,
            UIStateCoordinator ui,
            AudioService audio,
            VfxService vfx,
            ThemeConfig startingTheme)
        {
            if (isInitialized)
            {
                return;
            }

            themeCatalog = catalog;
            saveService = service;
            gameManager = manager;
            scoreSystem = scores;
            backgroundController = background;
            defaultBackgroundConfig = backgroundConfig;
            trackManager = track;
            playerController = player;
            hazardManager = hazards;
            worldProgressionController = progression;
            uiStateCoordinator = ui;
            audioService = audio;
            vfxService = vfx;
            currentTheme = startingTheme;

            if (worldProgressionController == null)
            {
                worldProgressionController = GetComponent<WorldProgressionController>();
                if (worldProgressionController == null)
                {
                    worldProgressionController = gameObject.AddComponent<WorldProgressionController>();
                }
            }

            worldProgressionController.DistrictChanged += HandleDistrictChanged;
            worldProgressionController.MilestoneReactionTriggered += HandleMilestoneReactionTriggered;

            scoreSystem.ScoreChanged += HandleScoreChanged;
            scoreSystem.MilestoneReached += HandleMilestoneReached;
            gameManager.RunStateChanged += HandleRunStateChanged;

            // Bootstrap the active theme and district immediately so the first run does not wait for a later restart.
            ApplyTheme(currentTheme, 0f);
            backgroundController?.ApplyScore(scoreSystem != null ? scoreSystem.CurrentScore : 0);
            worldProgressionController?.ResetForRun(scoreSystem != null ? scoreSystem.CurrentScore : 0);
            isInitialized = true;
        }

        private void OnDestroy()
        {
            if (worldProgressionController != null)
            {
                worldProgressionController.DistrictChanged -= HandleDistrictChanged;
                worldProgressionController.MilestoneReactionTriggered -= HandleMilestoneReactionTriggered;
            }

            if (scoreSystem != null)
            {
                scoreSystem.ScoreChanged -= HandleScoreChanged;
                scoreSystem.MilestoneReached -= HandleMilestoneReached;
            }

            if (gameManager != null)
            {
                gameManager.RunStateChanged -= HandleRunStateChanged;
            }
        }

        private void HandleRunStateChanged(RunState state)
        {
            if (state != RunState.Starting)
            {
                return;
            }

            ThemeConfig selectedTheme = saveService.ResolveSelectedTheme(themeCatalog) ?? themeCatalog.DefaultTheme;
            ApplyTheme(selectedTheme, 0f);
            backgroundController?.ApplyScore(scoreSystem != null ? scoreSystem.CurrentScore : 0);
            worldProgressionController?.ResetForRun(scoreSystem != null ? scoreSystem.CurrentScore : 0);
        }

        private void HandleScoreChanged(int score)
        {
            if (gameManager == null || gameManager.CurrentState != RunState.Active || gameManager.IsPaused)
            {
                return;
            }

            backgroundController?.ApplyScore(score);
            worldProgressionController?.UpdateDistrictForScore(score, ResolveDistrictTransitionDuration(score), false);
        }

        private void HandleMilestoneReached(int milestone)
        {
            worldProgressionController?.TriggerMilestoneReaction(milestone);
        }

        private void HandleDistrictChanged(WorldDistrictStateDefinition district, float durationSeconds, bool force)
        {
            trackManager?.ApplyWorldDistrict(district, durationSeconds);
            backgroundController?.ApplyWorldDistrict(district, durationSeconds);
            hazardManager?.ApplyWorldDistrict(district);
            vfxService?.ApplyWorldDistrict(district);
            uiStateCoordinator?.ApplyWorldDistrict(district);
        }

        private void HandleMilestoneReactionTriggered(WorldMilestoneReactionDefinition reaction, int milestone)
        {
            Color milestoneColor = worldProgressionController != null && worldProgressionController.CurrentDistrict != null
                ? worldProgressionController.CurrentDistrict.MilestoneColor
                : (currentTheme != null ? currentTheme.MilestoneColor : Color.white);
            backgroundController?.PlayMilestonePulse(milestoneColor, reaction.BackgroundFlashStrength);
            uiStateCoordinator?.ShowMilestoneMessage(milestone);
        }

        private void ApplyTheme(ThemeConfig theme, float durationSeconds)
        {
            if (theme == null)
            {
                return;
            }

            currentTheme = theme;
            worldProgressionController?.Initialize(theme);
            BackgroundPresentationConfig backgroundConfig = theme.ResolveBackgroundPresentation(defaultBackgroundConfig);
            backgroundController?.ApplyTheme(theme, backgroundConfig, durationSeconds);
            trackManager?.ApplyTheme(theme, durationSeconds);
            playerController?.ApplyTheme(theme);
            hazardManager?.ApplyTheme(theme);
            uiStateCoordinator?.ApplyTheme(theme);
            audioService?.ApplyTheme(theme);
            vfxService?.ApplyTheme(theme);
        }

        private float ResolveDistrictTransitionDuration(int score)
        {
            WorldProgressionConfig worldProgressionConfig = currentTheme != null ? currentTheme.ResolveWorldProgressionConfig() : null;
            if (worldProgressionConfig == null || !worldProgressionConfig.TryGetMilestoneReaction(score, out WorldMilestoneReactionDefinition reaction))
            {
                return currentTheme != null ? Mathf.Max(0.16f, currentTheme.PreferredTransitionDuration) : 0.24f;
            }

            return reaction.TransitionDurationSeconds;
        }
    }
}
