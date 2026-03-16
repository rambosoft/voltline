#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Voltline.Gameplay;

namespace Voltline.Editor
{
    public static class GameplayDebugMenu
    {
        [MenuItem("Tools/Voltline/Debug/Set Session Starting Score/0")]
        private static void SetStartingScoreZero()
        {
            ApplyStartingScoreOverride(0);
        }

        [MenuItem("Tools/Voltline/Debug/Set Session Starting Score/15")]
        private static void SetStartingScoreFifteen()
        {
            ApplyStartingScoreOverride(15);
        }

        [MenuItem("Tools/Voltline/Debug/Clear Session Starting Score Override")]
        private static void ClearStartingScoreOverride()
        {
            GameplaySceneInstaller.ClearEditorSessionDebugStartingScoreOverride();
            Debug.Log("Voltline cleared the editor session starting score override. Gameplay will use the release baseline again.");
        }

        private static void ApplyStartingScoreOverride(int score)
        {
            GameplaySceneInstaller.EditorSessionDebugStartingScore = score;
            GameplaySceneInstaller.EditorSessionDebugStartingScoreOverrideEnabled = true;
            Debug.Log($"Voltline editor session starting score override set to {GameplaySceneInstaller.EditorSessionDebugStartingScore}.");
        }
    }
}
#endif
