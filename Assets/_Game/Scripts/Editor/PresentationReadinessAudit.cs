#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Voltline.Data;
using Voltline.Gameplay;
using Voltline.UI;

namespace Voltline.Editor
{
    public sealed class PresentationReadinessAuditResult
    {
        private readonly List<string> errors = new();
        private readonly List<string> warnings = new();
        private readonly List<string> notes = new();

        public IReadOnlyList<string> Errors => errors;
        public IReadOnlyList<string> Warnings => warnings;
        public IReadOnlyList<string> Notes => notes;
        public bool HasErrors => errors.Count > 0;

        public void AddError(string message) => errors.Add(message);
        public void AddWarning(string message) => warnings.Add(message);
        public void AddNote(string message) => notes.Add(message);
    }

    public static class PresentationReadinessAudit
    {
        [MenuItem("Tools/Voltline/Run Presentation Readiness Audit")]
        private static void RunFromMenu()
        {
            PresentationReadinessAuditResult result = Validate();

            foreach (string note in result.Notes)
            {
                Debug.Log(note);
            }

            foreach (string warning in result.Warnings)
            {
                Debug.LogWarning(warning);
            }

            if (result.HasErrors)
            {
                foreach (string error in result.Errors)
                {
                    Debug.LogError(error);
                }

                return;
            }

            Debug.Log("Voltline presentation readiness audit passed with no blocking errors.");
        }

        public static PresentationReadinessAuditResult Validate()
        {
            PresentationReadinessAuditResult result = new();

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

            ConfigValidationResult configValidation = ProjectConfigValidator.ValidateProject(
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

            foreach (string error in configValidation.Errors)
            {
                result.AddError(error);
            }

            ValidateGameplaySceneInstaller(result);
            ValidateMainMenuDependencies(result);
            AddDependencyNotes(result, gameplayPresentation, backgroundPresentationConfig, playerVisualConfig, hazardPresentationCatalog, obstacleVisualCatalog, themeSequenceConfig);
            AddPipelineNotes(result, themeCatalog, presentationRolloutPlan);
            AddContentReadinessNotes(result);
            return result;
        }

        private static void ValidateGameplaySceneInstaller(PresentationReadinessAuditResult result)
        {
            EditorSceneManager.OpenScene(ProjectConfigAssetPaths.GameplayScene, OpenSceneMode.Single);
            GameplaySceneInstaller installer = Object.FindFirstObjectByType<GameplaySceneInstaller>();
            if (installer == null)
            {
                result.AddError("Gameplay scene is missing GameplaySceneInstaller.");
                return;
            }

            SerializedObject serializedInstaller = new(installer);
            ValidateObjectReference(serializedInstaller, "gameBalance", result);
            ValidateObjectReference(serializedInstaller, "gameplayPresentation", result);
            ValidateObjectReference(serializedInstaller, "backgroundPresentationConfig", result);
            ValidateObjectReference(serializedInstaller, "playerVisualConfig", result);
            ValidateObjectReference(serializedInstaller, "hazardPresentationCatalog", result);
            ValidateObjectReference(serializedInstaller, "obstacleVisualCatalog", result);
            ValidateObjectReference(serializedInstaller, "difficultyCurve", result);
            ValidateObjectReference(serializedInstaller, "obstacleCatalog", result);
            ValidateObjectReference(serializedInstaller, "themeCatalog", result);
            ValidateObjectReference(serializedInstaller, "themeSequenceConfig", result);
            ValidateObjectReference(serializedInstaller, "audioCueCatalog", result);
            ValidateObjectReference(serializedInstaller, "vfxCatalog", result);
        }

        private static void ValidateMainMenuDependencies(PresentationReadinessAuditResult result)
        {
            EditorSceneManager.OpenScene(ProjectConfigAssetPaths.MainMenuScene, OpenSceneMode.Single);
            MainMenuView mainMenu = Object.FindFirstObjectByType<MainMenuView>();
            if (mainMenu == null)
            {
                result.AddError("MainMenu scene is missing MainMenuView.");
                return;
            }

            SerializedObject serializedMainMenu = new(mainMenu);
            ValidateObjectReference(serializedMainMenu, "themeCatalog", result);
            ValidateObjectReference(serializedMainMenu, "playerVisualConfig", result);
            ValidateObjectReference(serializedMainMenu, "obstacleVisualCatalog", result);
            ValidateObjectReference(serializedMainMenu, "audioCueCatalog", result);
        }

        private static void AddDependencyNotes(
            PresentationReadinessAuditResult result,
            GameplayPresentationConfig gameplayPresentation,
            BackgroundPresentationConfig backgroundPresentationConfig,
            PlayerVisualConfig playerVisualConfig,
            HazardPresentationCatalog hazardPresentationCatalog,
            ObstacleVisualCatalog obstacleVisualCatalog,
            ThemeSequenceConfig themeSequenceConfig)
        {
            result.AddNote($"Presentation dependency audit: player gameplay is owned by {nameof(PlayerController)} and visuals are now owned by {nameof(PlayerVisualView)} via {nameof(PlayerVisualConfig)}.");
            result.AddNote($"Presentation dependency audit: hazard gameplay is owned by {nameof(HazardManager)} and visuals are now owned by {nameof(HazardVisualView)} via {nameof(ObstacleVisualCatalog)}.");
            result.AddNote($"Presentation dependency audit: background runtime is now owned by {nameof(BackgroundPresentationController)} via {nameof(BackgroundPresentationConfig)}.");
            result.AddNote($"Presentation dependency audit: theme application is now owned by {nameof(ThemePresentationController)} via {nameof(ThemeCatalog)} and {nameof(ThemeSequenceConfig)}.");
            result.AddNote($"Presentation dependency audit: track runtime remains owned by {nameof(TrackManager)} for line/path behavior only.");

            if (gameplayPresentation != null && playerVisualConfig != null)
            {
                result.AddNote($"Frozen player baseline: line width {gameplayPresentation.TrackLineWidth:0.##}, collision {gameplayPresentation.PlayerCollisionHalfWidth:0.##} x {gameplayPresentation.PlayerCollisionHalfHeight:0.##}, runtime visual {playerVisualConfig.VisibleBoundsScale.x:0.##} x {playerVisualConfig.VisibleBoundsScale.y:0.##}.");
            }

            if (hazardPresentationCatalog != null && obstacleVisualCatalog != null)
            {
                result.AddNote($"Frozen hazard baseline: {hazardPresentationCatalog.Entries.Count} collision/spacing profiles and {obstacleVisualCatalog.Entries.Count} visual profiles captured in config.");
            }

            if (backgroundPresentationConfig != null)
            {
                result.AddNote($"Background budget baseline: up to {backgroundPresentationConfig.MaxRuntimeSpriteCount} runtime layers and {backgroundPresentationConfig.MaxExpectedDrawCalls} expected draw calls with a lane quiet-zone of {backgroundPresentationConfig.LaneQuietZoneHalfWidth:0.##}.");
            }

            if (themeSequenceConfig != null)
            {
                result.AddNote($"Runtime theme transitions are milestone-gated through {themeSequenceConfig.Entries.Count} configured theme-sequence entries.");
            }
        }

        private static void AddPipelineNotes(PresentationReadinessAuditResult result, ThemeCatalog themeCatalog, PresentationRolloutPlanConfig presentationRolloutPlan)
        {
            if (themeCatalog != null)
            {
                result.AddNote($"Theme-owned VFX and audio variation is now routed through {nameof(ThemeVfxProfile)} and {nameof(ThemeAudioProfile)} references on {nameof(ThemeConfig)}.");
                result.AddNote($"Presentation feedback runtime remains centralized through {nameof(Voltline.VFX.VfxService)} and {nameof(Voltline.Audio.AudioService)} with semantic cue IDs.");
            }

            if (presentationRolloutPlan != null)
            {
                result.AddNote($"Broad refresh rollout is now governed by {nameof(PresentationRolloutPlanConfig)} with {presentationRolloutPlan.Slices.Count} ordered stop/go slices.");
            }
        }

        private static void AddContentReadinessNotes(PresentationReadinessAuditResult result)
        {
            string artRoot = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "_Game", "Art");
            string audioRoot = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "_Game", "Audio");
            int artAssetCount = CountAuthoredAssets(artRoot);
            int audioAssetCount = CountAuthoredAssets(audioRoot);

            result.AddNote($"Presentation content snapshot: {artAssetCount} non-meta assets under Assets/_Game/Art and {audioAssetCount} non-meta assets under Assets/_Game/Audio.");
            result.AddNote("Player, obstacle, background, theme-transition, VFX, and audio refresh slices are now structurally unlocked. Broad refresh rollout remains gated by the approval audit and staged rollout plan.");

            if (artAssetCount <= 2)
            {
                result.AddWarning("Authored art folders remain sparse. Presentation rollout is structurally ready, but later refresh slices still need real authored assets.");
            }

            if (audioAssetCount <= 3)
            {
                result.AddWarning("Authored audio folders remain sparse. Audio refresh rollout is structurally ready, but it still needs real authored cue and music assets.");
            }
        }

        private static int CountAuthoredAssets(string root)
        {
            if (!Directory.Exists(root))
            {
                return 0;
            }

            return Directory.GetFiles(root, "*", SearchOption.AllDirectories)
                .Count(path => !path.EndsWith(".meta"));
        }

        private static void ValidateObjectReference(SerializedObject serializedObject, string propertyName, PresentationReadinessAuditResult result)
        {
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            if (property != null && property.objectReferenceValue != null)
            {
                return;
            }

            result.AddError($"Serialized scene reference '{propertyName}' is missing in '{serializedObject.targetObject.name}'.");
        }
    }
}
#endif

