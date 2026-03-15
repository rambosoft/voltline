#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Voltline.Data;

namespace Voltline.Editor
{
    internal static class ProjectConfigValidationMenu
    {
        [MenuItem("Tools/Voltline/Validate Config")]
        private static void ValidateConfig()
        {
            GameBalanceConfig gameBalance = AssetDatabase.LoadAssetAtPath<GameBalanceConfig>(ProjectConfigAssetPaths.GameBalance);
            DifficultyCurveConfig difficultyCurve = AssetDatabase.LoadAssetAtPath<DifficultyCurveConfig>(ProjectConfigAssetPaths.DifficultyCurve);
            GameplayPresentationConfig gameplayPresentation = AssetDatabase.LoadAssetAtPath<GameplayPresentationConfig>(ProjectConfigAssetPaths.GameplayPresentation);
            PlayerVisualConfig playerVisualConfig = AssetDatabase.LoadAssetAtPath<PlayerVisualConfig>(ProjectConfigAssetPaths.PlayerVisualConfig);
            HazardPresentationCatalog hazardPresentationCatalog = AssetDatabase.LoadAssetAtPath<HazardPresentationCatalog>(ProjectConfigAssetPaths.HazardPresentationCatalog);
            ObstacleVisualCatalog obstacleVisualCatalog = AssetDatabase.LoadAssetAtPath<ObstacleVisualCatalog>(ProjectConfigAssetPaths.ObstacleVisualCatalog);
            ObstacleCatalog obstacleCatalog = AssetDatabase.LoadAssetAtPath<ObstacleCatalog>(ProjectConfigAssetPaths.ObstacleCatalog);
            ThemeCatalog themeCatalog = AssetDatabase.LoadAssetAtPath<ThemeCatalog>(ProjectConfigAssetPaths.ThemeCatalog);
            AudioCueCatalog audioCueCatalog = AssetDatabase.LoadAssetAtPath<AudioCueCatalog>(ProjectConfigAssetPaths.AudioCueCatalog);
            VfxCatalog vfxCatalog = AssetDatabase.LoadAssetAtPath<VfxCatalog>(ProjectConfigAssetPaths.VfxCatalog);

            ConfigValidationResult result = ProjectConfigValidator.ValidateProject(
                gameBalance,
                difficultyCurve,
                gameplayPresentation,
                playerVisualConfig,
                hazardPresentationCatalog,
                obstacleVisualCatalog,
                obstacleCatalog,
                themeCatalog,
                audioCueCatalog,
                vfxCatalog);

            if (result.IsValid)
            {
                Debug.Log("Voltline config validation passed.");
                return;
            }

            foreach (string error in result.Errors)
            {
                Debug.LogError(error);
            }
        }
    }
}
#endif
