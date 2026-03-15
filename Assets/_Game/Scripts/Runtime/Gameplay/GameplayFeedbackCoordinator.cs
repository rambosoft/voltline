using Voltline.Audio;
using Voltline.Data;
using Voltline.VFX;
using UnityEngine;

namespace Voltline.Gameplay
{
    public sealed class GameplayFeedbackCoordinator : MonoBehaviour
    {
        private GameManager gameManager;
        private PlayerController playerController;
        private HazardManager hazardManager;
        private ScoreSystem scoreSystem;
        private TrackManager trackManager;
        private AudioService audioService;
        private VfxService vfxService;
        private int lastScore;
        private bool isInitialized;

        public void Initialize(
            GameManager manager,
            PlayerController player,
            HazardManager hazards,
            ScoreSystem scores,
            TrackManager track,
            AudioService audio,
            VfxService vfx)
        {
            if (isInitialized)
            {
                return;
            }

            gameManager = manager;
            playerController = player;
            hazardManager = hazards;
            scoreSystem = scores;
            trackManager = track;
            audioService = audio;
            vfxService = vfx;
            lastScore = scoreSystem.CurrentScore;

            playerController.Flipped += HandlePlayerFlipped;
            hazardManager.NearMissTriggered += HandleNearMissTriggered;
            scoreSystem.ScoreChanged += HandleScoreChanged;
            scoreSystem.MilestoneReached += HandleMilestoneReached;
            gameManager.RunStateChanged += HandleRunStateChanged;
            isInitialized = true;
        }

        private void OnDestroy()
        {
            if (playerController != null)
            {
                playerController.Flipped -= HandlePlayerFlipped;
            }

            if (hazardManager != null)
            {
                hazardManager.NearMissTriggered -= HandleNearMissTriggered;
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

        private void HandlePlayerFlipped(Vector3 position)
        {
            if (gameManager == null || gameManager.CurrentState != RunState.Active)
            {
                return;
            }

            audioService.PlayCue(AudioCueIds.Flip);
            vfxService.PlayEffect(VfxCueIds.Flip, position);
            trackManager?.PlayLinePulse(0.5f);
        }

        private void HandleNearMissTriggered(Vector3 position)
        {
            if (gameManager == null || gameManager.CurrentState != RunState.Active)
            {
                return;
            }

            audioService.PlayCue(AudioCueIds.NearMiss);
            vfxService.PlayEffect(VfxCueIds.NearMiss, position);
        }

        private void HandleScoreChanged(int score)
        {
            if (score > lastScore)
            {
                audioService.PlayCue(AudioCueIds.Score);
            }

            lastScore = score;
        }

        private void HandleMilestoneReached(int milestone)
        {
            audioService.PlayCue(AudioCueIds.Milestone);
            vfxService.PlayEffect(VfxCueIds.Milestone, playerController.WorldPosition, 1f + (milestone * 0.005f));
            trackManager?.PlayLinePulse(1f);
        }

        private void HandleRunStateChanged(RunState state)
        {
            if (state == RunState.Dying)
            {
                audioService.PlayCue(AudioCueIds.Death);
                vfxService.PlayEffect(VfxCueIds.Death, playerController.WorldPosition);
                return;
            }

            if (state == RunState.Starting)
            {
                lastScore = 0;
            }
        }
    }
}
