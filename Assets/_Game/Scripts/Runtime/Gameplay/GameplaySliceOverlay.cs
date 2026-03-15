using UnityEngine;

namespace Voltline.Gameplay
{
    public sealed class GameplaySliceOverlay : MonoBehaviour
    {
        private GUIStyle scoreStyle;
        private GUIStyle messageStyle;
        private GUIStyle resultStyle;

        private GameManager gameManager;
        private ScoreSystem scoreSystem;

        public void Initialize(GameManager manager, ScoreSystem scores)
        {
            gameManager = manager;
            scoreSystem = scores;
            EnsureStyles();
        }

        private void OnGUI()
        {
            if (gameManager == null || scoreSystem == null)
            {
                return;
            }

            EnsureStyles();
            GUI.Label(new Rect(18f, 18f, 280f, 40f), $"Score {scoreSystem.CurrentScore}", scoreStyle);

            if (gameManager.CurrentState == RunState.Starting || (gameManager.CurrentState == RunState.Active && scoreSystem.CurrentScore == 0))
            {
                GUI.Label(new Rect(18f, 54f, 360f, 32f), "Tap to flip to the other side of the line", messageStyle);
            }

            if (gameManager.CurrentState == RunState.Results)
            {
                Rect box = new((Screen.width * 0.5f) - 170f, (Screen.height * 0.5f) - 70f, 340f, 140f);
                GUI.Box(box, GUIContent.none);
                GUI.Label(new Rect(box.x + 22f, box.y + 18f, box.width - 44f, 40f), "Run over", resultStyle);
                GUI.Label(new Rect(box.x + 22f, box.y + 58f, box.width - 44f, 30f), $"Score {scoreSystem.CurrentScore}", messageStyle);
                GUI.Label(new Rect(box.x + 22f, box.y + 90f, box.width - 44f, 30f), "Tap or press R to retry", messageStyle);
            }
        }

        private void EnsureStyles()
        {
            scoreStyle ??= new GUIStyle(GUI.skin.label)
            {
                fontSize = 24,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.white },
            };

            messageStyle ??= new GUIStyle(GUI.skin.label)
            {
                fontSize = 18,
                normal = { textColor = new Color(0.86f, 0.92f, 1f, 1f) },
            };

            resultStyle ??= new GUIStyle(GUI.skin.label)
            {
                fontSize = 28,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft,
                normal = { textColor = Color.white },
            };
        }
    }
}