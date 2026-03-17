using UnityEngine;
using UnityEngine.InputSystem;
using Voltline.Core;

namespace Voltline.Gameplay
{
    public sealed class GameplayDebugScoreOverlay : MonoBehaviour
    {
        private static readonly int[] PresetScores = { 10, 15, 25, 35, 45 };

        private ScoreSystem scoreSystem;
        private WorldProgressionController worldProgressionController;
        private GameManager gameManager;
        private Rect panelRect = new(14f, 14f, 220f, 220f);

        public void Initialize(ScoreSystem scores, WorldProgressionController progression, GameManager manager)
        {
            scoreSystem = scores;
            worldProgressionController = progression;
            gameManager = manager;
        }

        private void Update()
        {
            if (!RuntimeBuildFlags.GameplayDebugToolsEnabled)
            {
                enabled = false;
                return;
            }

            Keyboard keyboard = Keyboard.current;
            if (keyboard == null)
            {
                return;
            }

            if (keyboard.digit1Key.wasPressedThisFrame) ApplyPresetScore(10);
            else if (keyboard.digit2Key.wasPressedThisFrame) ApplyPresetScore(15);
            else if (keyboard.digit3Key.wasPressedThisFrame) ApplyPresetScore(25);
            else if (keyboard.digit4Key.wasPressedThisFrame) ApplyPresetScore(35);
            else if (keyboard.digit5Key.wasPressedThisFrame) ApplyPresetScore(45);
            else if (keyboard.backquoteKey.wasPressedThisFrame) gameManager?.RequestRestart();
        }

        private void OnGUI()
        {
            if (!RuntimeBuildFlags.GameplayDebugToolsEnabled || scoreSystem == null)
            {
                return;
            }

            GUILayout.BeginArea(panelRect, GUI.skin.box);
            GUILayout.Label("Debug Score Jump");
            GUILayout.Label($"Score: {scoreSystem.CurrentScore}");
            GUILayout.Label($"District: {worldProgressionController?.CurrentDistrict?.DisplayName ?? "Unknown"}");
            GUILayout.Label("Keys: 1=10 2=15 3=25 4=35 5=45");
            GUILayout.Label("` = restart");

            for (int i = 0; i < PresetScores.Length; i++)
            {
                int preset = PresetScores[i];
                if (GUILayout.Button($"Jump to {preset}"))
                {
                    ApplyPresetScore(preset);
                }
            }

            if (GUILayout.Button("Restart Run"))
            {
                gameManager?.RequestRestart();
            }

            GUILayout.EndArea();
        }

        private void ApplyPresetScore(int score)
        {
            if (scoreSystem == null)
            {
                return;
            }

            RunState state = gameManager != null ? gameManager.CurrentState : RunState.Active;
            if (state == RunState.Dying || state == RunState.Results || state == RunState.Restarting)
            {
                return;
            }

            scoreSystem.SetDebugScore(score);
        }
    }
}
