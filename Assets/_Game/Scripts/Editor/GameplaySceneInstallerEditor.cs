#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Voltline.Gameplay;

namespace Voltline.Editor
{
    [CustomEditor(typeof(GameplaySceneInstaller))]
    public sealed class GameplaySceneInstallerEditor : UnityEditor.Editor
    {
        private SerializedProperty debugStartingScoreProperty;

        private void OnEnable()
        {
            debugStartingScoreProperty = serializedObject.FindProperty("debugStartingScore");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawPropertiesExcluding(serializedObject, "m_Script", "debugStartingScore");

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Debug Starting Score", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Keep the scene baseline at 0 for release safety. Use the editor session override below for local testing without dirtying Gameplay.unity.",
                MessageType.Info);

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.PropertyField(debugStartingScoreProperty, new GUIContent("Release Baseline Score"));
            }

            bool useSessionOverride = GameplaySceneInstaller.EditorSessionDebugStartingScoreOverrideEnabled;
            int sessionScore = GameplaySceneInstaller.EditorSessionDebugStartingScore;

            bool updatedUseSessionOverride = EditorGUILayout.Toggle("Use Session Override", useSessionOverride);
            int updatedSessionScore = Mathf.Max(0, EditorGUILayout.IntField("Session Starting Score", sessionScore));

            if (updatedUseSessionOverride != useSessionOverride)
            {
                GameplaySceneInstaller.EditorSessionDebugStartingScoreOverrideEnabled = updatedUseSessionOverride;
            }

            if (updatedSessionScore != sessionScore)
            {
                GameplaySceneInstaller.EditorSessionDebugStartingScore = updatedSessionScore;
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Set 0"))
            {
                GameplaySceneInstaller.EditorSessionDebugStartingScore = 0;
                GameplaySceneInstaller.EditorSessionDebugStartingScoreOverrideEnabled = true;
            }

            if (GUILayout.Button("Set 15"))
            {
                GameplaySceneInstaller.EditorSessionDebugStartingScore = 15;
                GameplaySceneInstaller.EditorSessionDebugStartingScoreOverrideEnabled = true;
            }

            if (GUILayout.Button("Clear Override"))
            {
                GameplaySceneInstaller.ClearEditorSessionDebugStartingScoreOverride();
            }
            EditorGUILayout.EndHorizontal();

            int effectiveScore = GameplaySceneInstaller.EditorSessionDebugStartingScoreOverrideEnabled
                ? GameplaySceneInstaller.EditorSessionDebugStartingScore
                : Mathf.Max(0, debugStartingScoreProperty != null ? debugStartingScoreProperty.intValue : 0);
            EditorGUILayout.HelpBox($"Effective Play Mode debug starting score: {effectiveScore}", MessageType.None);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
