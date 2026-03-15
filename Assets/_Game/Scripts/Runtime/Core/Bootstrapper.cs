using UnityEngine;
using UnityEngine.SceneManagement;

namespace Voltline.Core
{
    public sealed class Bootstrapper : MonoBehaviour
    {
        private bool hasLoadedInitialScene;

        private void Start()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            ValidateBuildConfiguration();
#endif

            if (hasLoadedInitialScene)
            {
                return;
            }

            hasLoadedInitialScene = true;
            SceneManager.LoadScene(SceneCatalog.MainMenu, LoadSceneMode.Single);
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        private static void ValidateBuildConfiguration()
        {
            if (SceneUtility.GetBuildIndexByScenePath(SceneCatalog.BootstrapScenePath) != 0 ||
                SceneUtility.GetBuildIndexByScenePath(SceneCatalog.MainMenuScenePath) != 1 ||
                SceneUtility.GetBuildIndexByScenePath(SceneCatalog.GameplayScenePath) != 2)
            {
                Debug.LogWarning("Voltline build settings do not match the documented Bootstrap/MainMenu/Gameplay order.");
            }
        }
#endif
    }
}