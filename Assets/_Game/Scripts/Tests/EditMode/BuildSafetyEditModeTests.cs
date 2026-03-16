#if UNITY_EDITOR
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Voltline.Data;
using Voltline.Gameplay;

namespace Voltline.Tests.EditMode
{
    public sealed class BuildSafetyEditModeTests
    {
        [Test]
        public void GameplaySceneInstaller_DebugStartingScoreDefaultsToZero()
        {
            EditorSceneManager.OpenScene(ProjectConfigAssetPaths.GameplayScene, OpenSceneMode.Single);

            GameplaySceneInstaller installer = Object.FindFirstObjectByType<GameplaySceneInstaller>();
            Assert.That(installer, Is.Not.Null);

            SerializedObject serializedObject = new(installer);
            Assert.That(serializedObject.FindProperty("debugStartingScore").intValue, Is.EqualTo(0));
            Assert.That(serializedObject.FindProperty("gameplayPresentation").objectReferenceValue, Is.Not.Null);
            Assert.That(serializedObject.FindProperty("backgroundPresentationConfig").objectReferenceValue, Is.Not.Null);
            Assert.That(serializedObject.FindProperty("playerVisualConfig").objectReferenceValue, Is.Not.Null);
            Assert.That(serializedObject.FindProperty("hazardPresentationCatalog").objectReferenceValue, Is.Not.Null);
            Assert.That(serializedObject.FindProperty("obstacleVisualCatalog").objectReferenceValue, Is.Not.Null);
            Assert.That(serializedObject.FindProperty("themeSequenceConfig").objectReferenceValue, Is.Not.Null);
            Assert.That(serializedObject.FindProperty("inputActions").objectReferenceValue, Is.Not.Null);
            Assert.That(serializedObject.FindProperty("audioCueCatalog").objectReferenceValue, Is.Not.Null);
            Assert.That(serializedObject.FindProperty("vfxCatalog").objectReferenceValue, Is.Not.Null);
            Assert.That(installer.DebugStartingScore, Is.EqualTo(0));
        }

        [Test]
        public void PresentationRolloutPlanAsset_ExistsAtDocumentedPath()
        {
            PresentationRolloutPlanConfig rolloutPlan = AssetDatabase.LoadAssetAtPath<PresentationRolloutPlanConfig>(ProjectConfigAssetPaths.PresentationRolloutPlan);
            Assert.That(rolloutPlan, Is.Not.Null);
        }
    }
}
#endif
