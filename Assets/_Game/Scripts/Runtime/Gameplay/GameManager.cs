using System;
using UnityEngine;
using Voltline.Data;
using Voltline.Input;

namespace Voltline.Gameplay
{
    public sealed class GameManager : MonoBehaviour
    {
        private const float StartingDurationSeconds = 0.18f;
        private const float DeathSettleDurationSeconds = 0.42f;

        private GameplayInputReader inputReader;
        private DifficultyDirector difficultyDirector;
        private TrackManager trackManager;
        private PlayerController playerController;
        private HazardManager hazardManager;
        private ScoreSystem scoreSystem;
        private int baseSeed;
        private int runCounter;
        private float stateElapsed;

        public event Action<RunState> RunStateChanged;

        public RunState CurrentState { get; private set; }
        public float CurrentSpeed { get; private set; }

        public void Initialize(
            GameBalanceConfig balanceConfig,
            GameplayInputReader gameplayInputReader,
            DifficultyDirector director,
            TrackManager track,
            PlayerController player,
            HazardManager hazards,
            ScoreSystem scores,
            int initialSeed)
        {
            inputReader = gameplayInputReader;
            difficultyDirector = director;
            trackManager = track;
            playerController = player;
            hazardManager = hazards;
            scoreSystem = scores;
            baseSeed = initialSeed;
            runCounter = 0;
            BeginRun();
        }

        public void DebugHandleTap()
        {
            HandleTap();
        }

        public void RequestRestart()
        {
            if (CurrentState == RunState.Restarting)
            {
                return;
            }

            SetState(RunState.Restarting);
            BeginRun();
        }

        private void Update()
        {
            if (inputReader == null)
            {
                return;
            }

            if (inputReader.ConsumeDebugRestartPressed())
            {
                RequestRestart();
                return;
            }

            if (CurrentState == RunState.Results)
            {
                if (inputReader.ConsumeTapPressed())
                {
                    HandleTap();
                }

                return;
            }

            if (CurrentState == RunState.Starting)
            {
                stateElapsed += Time.deltaTime;
                playerController.Tick(Time.deltaTime);
                if (stateElapsed >= StartingDurationSeconds)
                {
                    SetState(RunState.Active);
                }

                return;
            }

            if (CurrentState == RunState.Active)
            {
                if (inputReader.ConsumeTapPressed())
                {
                    HandleTap();
                }

                TickActiveRun(Time.deltaTime);
                return;
            }

            if (CurrentState == RunState.Dying)
            {
                stateElapsed += Time.deltaTime;
                playerController.Tick(Time.deltaTime);
                if (stateElapsed >= DeathSettleDurationSeconds)
                {
                    SetState(RunState.Results);
                }
            }
        }

        private void BeginRun()
        {
            int seed = baseSeed + (runCounter * 17);
            runCounter++;

            scoreSystem.ResetRun();
            trackManager.ResetRun();
            playerController.ResetRun();
            hazardManager.ResetRun(seed);
            CurrentSpeed = difficultyDirector.GetCurrentSpeed(0);
            SetState(RunState.Ready);
            SetState(RunState.Starting);
        }

        private void TickActiveRun(float deltaTime)
        {
            CurrentSpeed = difficultyDirector.GetCurrentSpeed(scoreSystem.CurrentScore);
            trackManager.Advance(deltaTime, CurrentSpeed);
            playerController.Tick(deltaTime);

            int clearedBeats = hazardManager.Tick(scoreSystem.CurrentScore, playerController, out ObstacleConfig collisionConfig);
            for (int i = 0; i < clearedBeats; i++)
            {
                scoreSystem.RegisterClearedBeat();
            }

            if (collisionConfig != null)
            {
                playerController.MarkDead();
                SetState(RunState.Dying);
            }
        }

        private void HandleTap()
        {
            if (CurrentState == RunState.Results)
            {
                RequestRestart();
                return;
            }

            if (CurrentState == RunState.Active)
            {
                playerController.RequestFlip();
            }
        }

        private void SetState(RunState newState)
        {
            CurrentState = newState;
            stateElapsed = 0f;
            RunStateChanged?.Invoke(newState);
        }
    }
}