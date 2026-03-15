#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using Voltline.Core;
using Voltline.Data;
using Voltline.Gameplay;
using Voltline.UI;

namespace Voltline.Editor
{
    public sealed class ReleaseAuditResult
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

    public static class ReleaseReadinessAudit
    {
        private sealed class MixerGroupBlock
        {
            public string FileId;
            public string Name;
            public int ChildrenLineIndex;
            public int BlockEndExclusive;
        }

        private static readonly HashSet<string> ApprovedDirectDependencies = new()
        {
            "com.unity.2d.sprite",
            "com.unity.inputsystem",
            "com.unity.render-pipelines.universal",
            "com.unity.test-framework",
            "com.unity.ugui",
            "com.unity.modules.accessibility",
            "com.unity.modules.adaptiveperformance",
            "com.unity.modules.ai",
            "com.unity.modules.androidjni",
            "com.unity.modules.animation",
            "com.unity.modules.assetbundle",
            "com.unity.modules.audio",
            "com.unity.modules.cloth",
            "com.unity.modules.director",
            "com.unity.modules.imageconversion",
            "com.unity.modules.imgui",
            "com.unity.modules.jsonserialize",
            "com.unity.modules.particlesystem",
            "com.unity.modules.physics",
            "com.unity.modules.physics2d",
            "com.unity.modules.screencapture",
            "com.unity.modules.terrain",
            "com.unity.modules.terrainphysics",
            "com.unity.modules.tilemap",
            "com.unity.modules.ui",
            "com.unity.modules.uielements",
            "com.unity.modules.umbra",
            "com.unity.modules.unityanalytics",
            "com.unity.modules.unitywebrequest",
            "com.unity.modules.unitywebrequestassetbundle",
            "com.unity.modules.unitywebrequestaudio",
            "com.unity.modules.unitywebrequesttexture",
            "com.unity.modules.unitywebrequestwww",
            "com.unity.modules.vectorgraphics",
            "com.unity.modules.vehicles",
            "com.unity.modules.video",
            "com.unity.modules.vr",
            "com.unity.modules.wind",
            "com.unity.modules.xr"
        };

        private static readonly string[] RequiredInputActions =
        {
            "Tap",
            "Pause",
            "DebugRestart",
            "NavigateUI",
            "SubmitUI",
            "CancelUI"
        };

        [MenuItem("Tools/Voltline/Run Release Audit")]
        private static void RunFromMenu()
        {
            ReleaseAuditResult result = Validate();

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

            Debug.Log("Voltline release audit passed with no blocking errors.");
        }

        public static ReleaseAuditResult Validate()
        {
            ReleaseAuditResult result = new();

            ValidateConfigAssets(result);
            ValidateBuildSceneOrder(result);
            ValidateManifest(result);
            ValidatePlayerSettings(result);
            ValidateInputAsset(result);
            ValidateMainMenuScene(result);
            ValidateGameplaySceneInstaller(result);
            ValidateAudioMixerRouting(result);
            MeasureProjectFootprint(result);

            return result;
        }

        private static void ValidateConfigAssets(ReleaseAuditResult result)
        {
            GameBalanceConfig gameBalance = AssetDatabase.LoadAssetAtPath<GameBalanceConfig>(ProjectConfigAssetPaths.GameBalance);
            DifficultyCurveConfig difficultyCurve = AssetDatabase.LoadAssetAtPath<DifficultyCurveConfig>(ProjectConfigAssetPaths.DifficultyCurve);
            ObstacleCatalog obstacleCatalog = AssetDatabase.LoadAssetAtPath<ObstacleCatalog>(ProjectConfigAssetPaths.ObstacleCatalog);
            ThemeCatalog themeCatalog = AssetDatabase.LoadAssetAtPath<ThemeCatalog>(ProjectConfigAssetPaths.ThemeCatalog);
            AudioCueCatalog audioCueCatalog = AssetDatabase.LoadAssetAtPath<AudioCueCatalog>(ProjectConfigAssetPaths.AudioCueCatalog);
            VfxCatalog vfxCatalog = AssetDatabase.LoadAssetAtPath<VfxCatalog>(ProjectConfigAssetPaths.VfxCatalog);

            ConfigValidationResult validation = ProjectConfigValidator.ValidateProject(
                gameBalance,
                difficultyCurve,
                obstacleCatalog,
                themeCatalog,
                audioCueCatalog,
                vfxCatalog);

            foreach (string error in validation.Errors)
            {
                result.AddError(error);
            }
        }

        private static void ValidateBuildSceneOrder(ReleaseAuditResult result)
        {
            string[] enabledScenePaths = EditorBuildSettings.scenes
                .Where(scene => scene.enabled)
                .Select(scene => scene.path)
                .ToArray();

            if (!SceneCatalog.BuildOrder.SequenceEqual(enabledScenePaths))
            {
                result.AddError("Build settings do not match the documented Bootstrap/MainMenu/Gameplay order.");
            }
        }

        private static void ValidateManifest(ReleaseAuditResult result)
        {
            string manifestPath = Path.Combine(Directory.GetCurrentDirectory(), "Packages", "manifest.json");
            string manifestJson = File.ReadAllText(manifestPath);
            MatchCollection matches = Regex.Matches(manifestJson, "\"([^\"]+)\"\\s*:\\s*\"[^\"]+\"");

            HashSet<string> directDependencies = new();
            foreach (Match match in matches)
            {
                if (match.Groups.Count < 2)
                {
                    continue;
                }

                directDependencies.Add(match.Groups[1].Value);
            }

            directDependencies.Remove("dependencies");

            foreach (string dependencyId in directDependencies.OrderBy(static value => value))
            {
                if (!ApprovedDirectDependencies.Contains(dependencyId))
                {
                    result.AddError($"Manifest contains an unapproved direct dependency '{dependencyId}'.");
                }
            }
        }

        private static void ValidatePlayerSettings(ReleaseAuditResult result)
        {
            string projectSettingsPath = Path.Combine(Directory.GetCurrentDirectory(), "ProjectSettings", "ProjectSettings.asset");
            string contents = File.ReadAllText(projectSettingsPath);

            RequireSetting(contents, "companyName: Voltline", "Project company name must remain Voltline.", result);
            RequireSetting(contents, "productName: Voltline", "Project product name must remain Voltline.", result);
            RequireSetting(contents, "defaultScreenOrientation: 1", "Project must remain portrait-first.", result);
            RequireSetting(contents, "allowedAutorotateToPortrait: 1", "Portrait autorotation must stay enabled.", result);
            RequireSetting(contents, "allowedAutorotateToLandscapeRight: 0", "Landscape autorotation must remain disabled.", result);
            RequireSetting(contents, "allowedAutorotateToLandscapeLeft: 0", "Landscape autorotation must remain disabled.", result);
            RequireSetting(contents, "useOSAutorotation: 0", "OS autorotation must remain disabled.", result);
            RequireSetting(contents, "runInBackground: 0", "runInBackground must remain disabled for release posture.", result);
            RequireSetting(contents, "activeInputHandler: 1", "Input System must remain the active input handler.", result);
        }

        private static void ValidateInputAsset(ReleaseAuditResult result)
        {
            InputActionAsset inputActions = AssetDatabase.LoadAssetAtPath<InputActionAsset>(ProjectConfigAssetPaths.InputActions);
            if (inputActions == null)
            {
                result.AddError("Project input actions asset is missing.");
                return;
            }

            for (int i = 0; i < RequiredInputActions.Length; i++)
            {
                if (inputActions.FindAction(RequiredInputActions[i]) == null)
                {
                    result.AddError($"Input action asset is missing required action '{RequiredInputActions[i]}'.");
                }
            }
        }

        private static void ValidateMainMenuScene(ReleaseAuditResult result)
        {
            EditorSceneManager.OpenScene(ProjectConfigAssetPaths.MainMenuScene, OpenSceneMode.Single);
            MainMenuView mainMenuView = UnityEngine.Object.FindFirstObjectByType<MainMenuView>();
            if (mainMenuView == null)
            {
                result.AddError("MainMenu scene is missing MainMenuView.");
                return;
            }

            SerializedObject serializedMainMenu = new(mainMenuView);
            ValidateObjectReference(serializedMainMenu, "themeCatalog", result, true);
            ValidateObjectReference(serializedMainMenu, "audioCueCatalog", result, true);
            ValidateObjectReference(serializedMainMenu, "audioMixer", result, false);
        }

        private static void ValidateGameplaySceneInstaller(ReleaseAuditResult result)
        {
            EditorSceneManager.OpenScene(ProjectConfigAssetPaths.GameplayScene, OpenSceneMode.Single);
            GameplaySceneInstaller installer = UnityEngine.Object.FindFirstObjectByType<GameplaySceneInstaller>();
            if (installer == null)
            {
                result.AddError("Gameplay scene is missing GameplaySceneInstaller.");
                return;
            }

            SerializedObject serializedInstaller = new(installer);
            ValidateObjectReference(serializedInstaller, "gameBalance", result, true);
            ValidateObjectReference(serializedInstaller, "difficultyCurve", result, true);
            ValidateObjectReference(serializedInstaller, "obstacleCatalog", result, true);
            ValidateObjectReference(serializedInstaller, "themeCatalog", result, true);
            ValidateObjectReference(serializedInstaller, "audioCueCatalog", result, true);
            ValidateObjectReference(serializedInstaller, "vfxCatalog", result, true);
            ValidateObjectReference(serializedInstaller, "audioMixer", result, false);
            ValidateObjectReference(serializedInstaller, "inputActions", result, true);

            SerializedProperty startingScoreProperty = serializedInstaller.FindProperty("debugStartingScore");
            if (startingScoreProperty == null || startingScoreProperty.intValue != 0)
            {
                result.AddError("Gameplay scene debugStartingScore must remain zero in the release baseline.");
            }
        }

        private static void ValidateAudioMixerRouting(ReleaseAuditResult result)
        {
            AudioMixer mixer = AssetDatabase.LoadAssetAtPath<AudioMixer>(ProjectConfigAssetPaths.AudioMixer);
            if (mixer == null)
            {
                result.AddWarning("No AudioMixer asset was found. Run Tools > Voltline > Ensure Audio Mixer before final release-candidate sign-off.");
                return;
            }

            ValidateMixerGroup(mixer, "Music", result);
            ValidateMixerGroup(mixer, "SFX", result);
            ValidateMixerGroup(mixer, "Gameplay", result);
            ValidateMixerGroup(mixer, "UI", result);
            ValidateMixerHierarchy(result);
        }

        private static void ValidateMixerGroup(AudioMixer mixer, string groupName, ReleaseAuditResult result)
        {
            AudioMixerGroup group = mixer.FindMatchingGroups(groupName)
                .FirstOrDefault(candidate => candidate != null && candidate.name == groupName);

            if (group == null)
            {
                result.AddWarning($"AudioMixer is missing required group '{groupName}'. Run Tools > Voltline > Ensure Audio Mixer.");
            }
        }

        private static void ValidateMixerHierarchy(ReleaseAuditResult result)
        {
            string mixerPath = Path.Combine(Directory.GetCurrentDirectory(), ProjectConfigAssetPaths.AudioMixer.Replace('/', Path.DirectorySeparatorChar));
            if (!File.Exists(mixerPath))
            {
                return;
            }

            var lines = File.ReadAllLines(mixerPath).ToList();
            var groups = ParseMixerGroups(lines);
            if (!groups.TryGetValue("Master", out MixerGroupBlock masterGroup)
                || !groups.TryGetValue("Music", out MixerGroupBlock musicGroup)
                || !groups.TryGetValue("SFX", out MixerGroupBlock sfxGroup)
                || !groups.TryGetValue("Gameplay", out MixerGroupBlock gameplayGroup)
                || !groups.TryGetValue("UI", out MixerGroupBlock uiGroup))
            {
                result.AddWarning("AudioMixer hierarchy is incomplete. Run Tools > Voltline > Ensure Audio Mixer.");
                return;
            }

            bool hasMasterChildren = BlockContainsChildren(lines, masterGroup, musicGroup.FileId, sfxGroup.FileId);
            bool hasSfxChildren = BlockContainsChildren(lines, sfxGroup, gameplayGroup.FileId, uiGroup.FileId);
            if (!hasMasterChildren || !hasSfxChildren)
            {
                result.AddWarning("AudioMixer group routing does not match the documented Master > Music/SFX > Gameplay/UI hierarchy. Run Tools > Voltline > Ensure Audio Mixer.");
            }
        }

        private static Dictionary<string, MixerGroupBlock> ParseMixerGroups(List<string> lines)
        {
            Dictionary<string, MixerGroupBlock> groups = new(StringComparer.Ordinal);
            for (int i = 0; i < lines.Count; i++)
            {
                if (!lines[i].StartsWith("--- !u!243 &", StringComparison.Ordinal))
                {
                    continue;
                }

                string fileId = lines[i].Substring("--- !u!243 &".Length).Trim();
                MixerGroupBlock block = new() { FileId = fileId };

                int j = i + 1;
                for (; j < lines.Count; j++)
                {
                    if (lines[j].StartsWith("--- !u!", StringComparison.Ordinal))
                    {
                        break;
                    }

                    if (lines[j].StartsWith("  m_Name: ", StringComparison.Ordinal))
                    {
                        block.Name = lines[j].Substring("  m_Name: ".Length);
                    }
                    else if (lines[j].StartsWith("  m_Children:", StringComparison.Ordinal))
                    {
                        block.ChildrenLineIndex = j;
                    }
                }

                block.BlockEndExclusive = j;
                if (!string.IsNullOrWhiteSpace(block.Name) && block.ChildrenLineIndex > 0)
                {
                    groups[block.Name] = block;
                }
            }

            return groups;
        }

        private static bool BlockContainsChildren(List<string> lines, MixerGroupBlock block, params string[] expectedChildren)
        {
            HashSet<string> actualChildren = new(StringComparer.Ordinal);
            for (int i = block.ChildrenLineIndex + 1; i < block.BlockEndExclusive; i++)
            {
                string line = lines[i].Trim();
                if (!line.StartsWith("- {fileID:", StringComparison.Ordinal))
                {
                    break;
                }

                int start = line.IndexOf(':') + 1;
                int end = line.IndexOf('}');
                if (start <= 0 || end <= start)
                {
                    continue;
                }

                actualChildren.Add(line.Substring(start, end - start).Trim());
            }

            return expectedChildren.All(actualChildren.Contains);
        }

        private static void MeasureProjectFootprint(ReleaseAuditResult result)
        {
            long projectBytes = GetDirectorySize(Path.Combine(Directory.GetCurrentDirectory(), "Assets", "_Game"));
            result.AddNote($"Voltline asset footprint snapshot: {FormatBytes(projectBytes)} under Assets/_Game.");
        }

        private static void ValidateObjectReference(SerializedObject serializedObject, string propertyName, ReleaseAuditResult result, bool blocking)
        {
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            if (property != null && property.objectReferenceValue != null)
            {
                return;
            }

            string message = $"Serialized scene reference '{propertyName}' is missing in '{serializedObject.targetObject.name}'.";
            if (blocking)
            {
                result.AddError(message);
            }
            else
            {
                result.AddWarning(message);
            }
        }

        private static void RequireSetting(string fileContents, string expectedLine, string errorMessage, ReleaseAuditResult result)
        {
            if (!fileContents.Contains(expectedLine))
            {
                result.AddError(errorMessage);
            }
        }

        private static long GetDirectorySize(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                return 0L;
            }

            long total = 0L;
            string[] files = Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].EndsWith(".meta"))
                {
                    continue;
                }

                total += new FileInfo(files[i]).Length;
            }

            return total;
        }

        private static string FormatBytes(long byteCount)
        {
            string[] units = { "B", "KB", "MB", "GB" };
            double size = byteCount;
            int unitIndex = 0;
            while (size >= 1024d && unitIndex < units.Length - 1)
            {
                size /= 1024d;
                unitIndex++;
            }

            return $"{size:0.##} {units[unitIndex]}";
        }
    }
}
#endif
