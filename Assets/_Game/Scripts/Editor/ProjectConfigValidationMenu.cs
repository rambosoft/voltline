#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Voltline.Data;

namespace Voltline.Editor
{
    public static class ProjectConfigValidationMenu
    {
        [MenuItem("Tools/Voltline/Validate Config")]
        public static void ValidateConfig()
        {
            GameBalanceConfig gameBalance = AssetDatabase.LoadAssetAtPath<GameBalanceConfig>(ProjectConfigAssetPaths.GameBalance);
            DifficultyCurveConfig difficultyCurve = AssetDatabase.LoadAssetAtPath<DifficultyCurveConfig>(ProjectConfigAssetPaths.DifficultyCurve);
            GameplayPresentationConfig gameplayPresentation = AssetDatabase.LoadAssetAtPath<GameplayPresentationConfig>(ProjectConfigAssetPaths.GameplayPresentation);
            BackgroundPresentationConfig backgroundPresentationConfig = AssetDatabase.LoadAssetAtPath<BackgroundPresentationConfig>(ProjectConfigAssetPaths.BackgroundPresentation);
            PlayerVisualConfig playerVisualConfig = AssetDatabase.LoadAssetAtPath<PlayerVisualConfig>(ProjectConfigAssetPaths.PlayerVisualConfig);
            HazardPresentationCatalog hazardPresentationCatalog = AssetDatabase.LoadAssetAtPath<HazardPresentationCatalog>(ProjectConfigAssetPaths.HazardPresentationCatalog);
            ObstacleVisualCatalog obstacleVisualCatalog = AssetDatabase.LoadAssetAtPath<ObstacleVisualCatalog>(ProjectConfigAssetPaths.ObstacleVisualCatalog);
            ObstacleCatalog obstacleCatalog = AssetDatabase.LoadAssetAtPath<ObstacleCatalog>(ProjectConfigAssetPaths.ObstacleCatalog);
            ThemeCatalog themeCatalog = AssetDatabase.LoadAssetAtPath<ThemeCatalog>(ProjectConfigAssetPaths.ThemeCatalog);
            ThemeSequenceConfig themeSequenceConfig = AssetDatabase.LoadAssetAtPath<ThemeSequenceConfig>(ProjectConfigAssetPaths.ThemeSequence);
            PresentationRolloutPlanConfig presentationRolloutPlan = AssetDatabase.LoadAssetAtPath<PresentationRolloutPlanConfig>(ProjectConfigAssetPaths.PresentationRolloutPlan);
            AudioCueCatalog audioCueCatalog = AssetDatabase.LoadAssetAtPath<AudioCueCatalog>(ProjectConfigAssetPaths.AudioCueCatalog);
            VfxCatalog vfxCatalog = AssetDatabase.LoadAssetAtPath<VfxCatalog>(ProjectConfigAssetPaths.VfxCatalog);

            ConfigValidationResult result = ProjectConfigValidator.ValidateProject(
                gameBalance,
                difficultyCurve,
                gameplayPresentation,
                backgroundPresentationConfig,
                playerVisualConfig,
                hazardPresentationCatalog,
                obstacleVisualCatalog,
                obstacleCatalog,
                themeCatalog,
                themeSequenceConfig,
                presentationRolloutPlan,
                audioCueCatalog,
                vfxCatalog);

            if (result.IsValid)
            {
                Debug.Log("Voltline config validation passed.");
                return;
            }

            Debug.LogError($"Voltline config validation failed:\n{result}");
        }
    }
}
#endif
