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
            HazardPresentationCatalog hazardPresentationCatalog = AssetDatabase.LoadAssetAtPath<HazardPresentationCatalog>(ProjectConfigAssetPaths.HazardPresentationCatalog);
            ObstacleCatalog obstacleCatalog = AssetDatabase.LoadAssetAtPath<ObstacleCatalog>(ProjectConfigAssetPaths.ObstacleCatalog);
            ThemeCatalog themeCatalog = AssetDatabase.LoadAssetAtPath<ThemeCatalog>(ProjectConfigAssetPaths.ThemeCatalog);
            AudioCueCatalog audioCueCatalog = AssetDatabase.LoadAssetAtPath<AudioCueCatalog>(ProjectConfigAssetPaths.AudioCueCatalog);
            VfxCatalog vfxCatalog = AssetDatabase.LoadAssetAtPath<VfxCatalog>(ProjectConfigAssetPaths.VfxCatalog);

            ConfigValidationResult configValidation = ProjectConfigValidator.ValidateProject(
                gameBalance,
                difficultyCurve,
                gameplayPresentation,
                hazardPresentationCatalog,
                obstacleCatalog,
                themeCatalog,
                audioCueCatalog,
                vfxCatalog);

            foreach (string error in configValidation.Errors)
            {
                result.AddError(error);
            }

            ValidateGameplaySceneInstaller(result);
            ValidateMainMenuThemeDependencies(result);
            AddDependencyNotes(result, gameplayPresentation, hazardPresentationCatalog);
            AddCurrentBlockers(result);
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
            ValidateObjectReference(serializedInstaller, "hazardPresentationCatalog", result);
            ValidateObjectReference(serializedInstaller, "difficultyCurve", result);
            ValidateObjectReference(serializedInstaller, "obstacleCatalog", result);
            ValidateObjectReference(serializedInstaller, "themeCatalog", result);
        }

        private static void ValidateMainMenuThemeDependencies(PresentationReadinessAuditResult result)
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
        }

        private static void AddDependencyNotes(
            PresentationReadinessAuditResult result,
            GameplayPresentationConfig gameplayPresentation,
            HazardPresentationCatalog hazardPresentationCatalog)
        {
            result.AddNote($"Presentation dependency audit: player runtime is owned by {nameof(PlayerController)} and now reads {nameof(GameplayPresentationConfig)}.");
            result.AddNote($"Presentation dependency audit: hazard runtime is owned by {nameof(HazardManager)} and now reads {nameof(HazardPresentationCatalog)} plus obstacle data.");
            result.AddNote($"Presentation dependency audit: track runtime is owned by {nameof(TrackManager)} and now reads {nameof(GameplayPresentationConfig)} for line/camera/path assumptions.");
            result.AddNote($"Presentation dependency audit: theme state remains data-driven through {nameof(ThemeCatalog)} and save-driven selection.");

            if (gameplayPresentation != null)
            {
                result.AddNote($"Frozen player baseline: scale {gameplayPresentation.PlayerVisualScale:0.##}, collision {gameplayPresentation.PlayerCollisionHalfWidth:0.##} x {gameplayPresentation.PlayerCollisionHalfHeight:0.##}, line width {gameplayPresentation.TrackLineWidth:0.##}.");
            }

            if (hazardPresentationCatalog != null)
            {
                result.AddNote($"Frozen hazard baseline: {hazardPresentationCatalog.Entries.Count} family profiles captured in config.");
            }
        }

        private static void AddCurrentBlockers(PresentationReadinessAuditResult result)
        {
            result.AddWarning("Player visuals remain procedural in PlayerController. Direct player art swaps stay blocked until the player visual root is decoupled from collision and anchoring.");
            result.AddWarning("Hazard visuals remain procedural in HazardManager. Direct obstacle art swaps stay blocked until obstacle visuals are decoupled from collision and spacing.");
            result.AddWarning("TrackManager still lacks a dedicated background presentation layer. Background refresh and motion spectacle stay blocked until background architecture and budgets are defined.");
            result.AddWarning("ThemeConfig is still color-oriented and theme application is still mostly initialization-time. Broad theme visual expansion and dynamic theme switching remain blocked.");
            result.AddWarning("VfxService and AudioService still lean on procedural fallback content. Broad presentation replacement remains blocked until dedicated VFX and audio refresh pipelines are established.");
        }

        private static void AddContentReadinessNotes(PresentationReadinessAuditResult result)
        {
            string artRoot = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "_Game", "Art");
            string audioRoot = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "_Game", "Audio");
            int artAssetCount = CountAuthoredAssets(artRoot);
            int audioAssetCount = CountAuthoredAssets(audioRoot);

            result.AddNote($"Presentation content snapshot: {artAssetCount} non-meta assets under Assets/_Game/Art and {audioAssetCount} non-meta assets under Assets/_Game/Audio.");

            if (artAssetCount <= 2)
            {
                result.AddWarning("Authored art folders remain sparse. Treat player, obstacle, and background asset replacement as blocked until the later refresh phases are ready.");
            }

            if (audioAssetCount <= 3)
            {
                result.AddWarning("Authored audio folders remain sparse. Treat broad cue/music replacement as blocked until the audio refresh pipeline phase is complete.");
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
