using System.Collections.Generic;
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
        private readonly HashSet<string> consumedThemeIds = new();

        private ThemeCatalog themeCatalog;
        private ThemeSequenceConfig themeSequenceConfig;
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
        private ThemeConfig currentTheme;
        private bool isInitialized;

        public string CurrentThemeId => currentTheme != null ? currentTheme.ThemeId : string.Empty;
        public int ActivatedTransitionCount { get; private set; }

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
            themeSequenceConfig = sequenceConfig;
            saveService = service;
            gameManager = manager;
            scoreSystem = scores;
            backgroundController = background;
            defaultBackgroundConfig = backgroundConfig;
            trackManager = track;
            playerController = player;
            hazardManager = hazards;
            uiStateCoordinator = ui;
            audioService = audio;
            vfxService = vfx;
            currentTheme = startingTheme;

            scoreSystem.ScoreChanged += HandleScoreChanged;
            gameManager.RunStateChanged += HandleRunStateChanged;
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
            }
        }

        private void HandleRunStateChanged(RunState state)
        {
            if (state != RunState.Starting)
            {
                return;
            }

            consumedThemeIds.Clear();
            ActivatedTransitionCount = 0;
            ThemeConfig selectedTheme = saveService.ResolveSelectedTheme(themeCatalog) ?? themeCatalog.DefaultTheme;
            ApplyTheme(selectedTheme, 0f, false);
        }

        private void HandleScoreChanged(int score)
        {
            if (gameManager == null
                || gameManager.CurrentState != RunState.Active
                || gameManager.IsPaused
                || themeSequenceConfig == null
                || !themeSequenceConfig.EnableRuntimeTransitions)
            {
                return;
            }

            IReadOnlyList<int> milestones = scoreSystem.MilestoneThresholds;
            bool isMilestone = false;
            for (int i = 0; i < milestones.Count; i++)
            {
                if (milestones[i] == score)
                {
                    isMilestone = true;
                    break;
                }
            }

            if (!isMilestone || score < themeSequenceConfig.MinimumScoreForTransitions)
            {
                return;
            }

            IReadOnlyList<ThemeSequenceEntry> entries = themeSequenceConfig.Entries;
            for (int i = 0; i < entries.Count; i++)
            {
                ThemeSequenceEntry entry = entries[i];
                if (entry == null || score < entry.ScoreThreshold || consumedThemeIds.Contains(entry.ThemeId))
                {
                    continue;
                }

                if (!themeCatalog.TryGetTheme(entry.ThemeId, out ThemeConfig targetTheme) || targetTheme == null)
                {
                    consumedThemeIds.Add(entry.ThemeId);
                    continue;
                }

                if (themeSequenceConfig.RequireUnlockedTheme && !saveService.IsThemeUnlocked(targetTheme.ThemeId))
                {
                    continue;
                }

                if (!targetTheme.AllowRuntimeSequenceSelection || targetTheme.ThemeId == CurrentThemeId)
                {
                    consumedThemeIds.Add(entry.ThemeId);
                    continue;
                }

                float configuredDuration = Mathf.Max(targetTheme.PreferredTransitionDuration, entry.TransitionDurationSeconds);
                float duration = Mathf.Clamp(configuredDuration, themeSequenceConfig.MinimumTransitionDurationSeconds, themeSequenceConfig.MaximumTransitionDurationSeconds);
                ApplyTheme(targetTheme, duration, true);
                consumedThemeIds.Add(entry.ThemeId);
                break;
            }
        }

        private void ApplyTheme(ThemeConfig theme, float durationSeconds, bool countAsTransition)
        {
            if (theme == null)
            {
                return;
            }

            currentTheme = theme;
            BackgroundPresentationConfig backgroundConfig = theme.ResolveBackgroundPresentation(defaultBackgroundConfig);
            backgroundController?.ApplyTheme(theme, backgroundConfig, durationSeconds);
            trackManager?.ApplyTheme(theme, durationSeconds);
            playerController?.ApplyTheme(theme);
            hazardManager?.ApplyTheme(theme);
            uiStateCoordinator?.ApplyTheme(theme);
            audioService?.ApplyTheme(theme);
            vfxService?.ApplyTheme(theme);

            if (countAsTransition)
            {
                ActivatedTransitionCount++;
            }
        }
    }
}
