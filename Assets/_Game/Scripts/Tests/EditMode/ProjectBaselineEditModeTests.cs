#if UNITY_EDITOR
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine.InputSystem;
using Voltline.Core;

namespace Voltline.Tests.EditMode
{
    public sealed class ProjectBaselineEditModeTests
    {
        private const string InputActionsAssetPath = "Assets/_Game/Settings/Input/VoltlineInputActions.inputactions";

        [Test]
        public void BuildSettings_UseDocumentedSceneOrder()
        {
            var enabledScenePaths = EditorBuildSettings.scenes
                .Where(scene => scene.enabled)
                .Select(scene => scene.path)
                .ToArray();

            CollectionAssert.AreEqual(SceneCatalog.BuildOrder, enabledScenePaths);
        }

        [Test]
        public void InputActionAsset_UsesDocumentedMinimumActions()
        {
            var inputActions = AssetDatabase.LoadAssetAtPath<InputActionAsset>(InputActionsAssetPath);

            Assert.That(inputActions, Is.Not.Null, "Voltline input actions asset should exist in the documented settings path.");
            Assert.That(inputActions.FindAction("Tap", true), Is.Not.Null);
            Assert.That(inputActions.FindAction("Pause", true), Is.Not.Null);
            Assert.That(inputActions.FindAction("DebugRestart", true), Is.Not.Null);
            Assert.That(inputActions.FindAction("NavigateUI", true), Is.Not.Null);
            Assert.That(inputActions.FindAction("SubmitUI", true), Is.Not.Null);
            Assert.That(inputActions.FindAction("CancelUI", true), Is.Not.Null);
        }
    }
}
#endif