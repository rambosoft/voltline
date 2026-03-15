#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Voltline.Data;
using Voltline.Gameplay;
using Voltline.UI;

namespace Voltline.Editor
{
    internal static class AudioMixerSetupUtility
    {
        private sealed class MixerGroupBlock
        {
            public string FileId;
            public string Name;
            public int ChildrenLineIndex;
            public int BlockEndExclusive;
        }

        private const string MusicGroupName = "Music";
        private const string SfxGroupName = "SFX";
        private const string GameplayGroupName = "Gameplay";
        private const string UiGroupName = "UI";
        private const string MasterGroupName = "Master";

        [MenuItem("Tools/Voltline/Ensure Audio Mixer")]
        private static void EnsureAudioMixerMenu()
        {
            AudioMixer mixer = EnsureAudioMixerAsset();
            if (mixer == null)
            {
                Debug.LogError("Voltline could not create or load the project AudioMixer asset.");
                return;
            }

            SynchronizeSceneReferences(mixer);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Voltline audio mixer setup completed.");
        }

        public static AudioMixer EnsureAudioMixerAsset()
        {
            EnsureFolder("Assets/_Game/Audio/Mixers");

            AudioMixer mixer = AssetDatabase.LoadAssetAtPath<AudioMixer>(ProjectConfigAssetPaths.AudioMixer);
            if (mixer == null)
            {
                mixer = CreateMixerAsset();
            }

            if (mixer == null)
            {
                return null;
            }

            EnsureRoutingGroups(mixer);
            EditorUtility.SetDirty(mixer);
            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(ProjectConfigAssetPaths.AudioMixer, ImportAssetOptions.ForceUpdate);
            return AssetDatabase.LoadAssetAtPath<AudioMixer>(ProjectConfigAssetPaths.AudioMixer);
        }

        private static AudioMixer CreateMixerAsset()
        {
            Type controllerType = FindEditorObjectType("AudioMixerController");
            MethodInfo createMethod = controllerType?.GetMethod(
                "CreateMixerControllerAtPath",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

            if (createMethod == null)
            {
                Debug.LogError("Voltline could not locate Unity's internal AudioMixerController.CreateMixerControllerAtPath API.");
                return null;
            }

            try
            {
                createMethod.Invoke(null, new object[] { ProjectConfigAssetPaths.AudioMixer });
            }
            catch (Exception exception)
            {
                Debug.LogError($"Voltline failed to create the AudioMixer asset. {exception}");
                return null;
            }

            return AssetDatabase.LoadAssetAtPath<AudioMixer>(ProjectConfigAssetPaths.AudioMixer);
        }

        private static void EnsureRoutingGroups(AudioMixer mixer)
        {
            Type controllerType = FindEditorObjectType("AudioMixerController");
            if (controllerType == null)
            {
                Debug.LogWarning("Voltline could not reflect AudioMixerController. Mixer group setup was skipped.");
                return;
            }

            object controller = LoadControllerAsset(controllerType);
            if (controller == null)
            {
                Debug.LogWarning("Voltline could not load the internal mixer controller object. Mixer group setup was skipped.");
                return;
            }

            object masterGroup = GetMemberValue(controller, "masterGroup");
            if (masterGroup == null)
            {
                Debug.LogWarning("Voltline could not locate the mixer master group. Mixer group setup was skipped.");
                return;
            }

            FindOrCreateGroup(mixer, controller, MusicGroupName, masterGroup);
            object sfxGroup = FindOrCreateGroup(mixer, controller, SfxGroupName, masterGroup);
            FindOrCreateGroup(mixer, controller, GameplayGroupName, sfxGroup ?? masterGroup);
            FindOrCreateGroup(mixer, controller, UiGroupName, sfxGroup ?? masterGroup);

            RepairMixerHierarchyInFile();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            if (!ValidateMixerYamlHierarchy())
            {
                Debug.LogWarning("Voltline could not fully create the required mixer group hierarchy automatically. Please rerun the setup or inspect the mixer asset in Unity.");
            }
        }

        private static object FindOrCreateGroup(AudioMixer mixer, object controller, string groupName, object parentGroup)
        {
            AudioMixerGroup existingGroup = FindExactGroup(mixer, groupName);
            if (existingGroup != null)
            {
                return LoadGroupControllerByName(groupName) ?? existingGroup;
            }

            MethodInfo[] createMethods = controller.GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(static method => method.Name == "CreateNewGroup")
                .OrderBy(method => method.GetParameters().Length)
                .ToArray();

            for (int i = 0; i < createMethods.Length; i++)
            {
                if (!TryBuildCreateGroupArgs(createMethods[i].GetParameters(), groupName, parentGroup, out object[] args))
                {
                    continue;
                }

                try
                {
                    createMethods[i].Invoke(controller, args);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.ImportAsset(ProjectConfigAssetPaths.AudioMixer, ImportAssetOptions.ForceUpdate);
                    AudioMixerGroup createdGroup = FindExactGroup(AssetDatabase.LoadAssetAtPath<AudioMixer>(ProjectConfigAssetPaths.AudioMixer), groupName);
                    if (createdGroup != null)
                    {
                        return LoadGroupControllerByName(groupName) ?? createdGroup;
                    }
                }
                catch
                {
                    // Try the next signature.
                }
            }

            return null;
        }

        private static bool TryBuildCreateGroupArgs(ParameterInfo[] parameters, string groupName, object parentGroup, out object[] args)
        {
            args = new object[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterInfo parameter = parameters[i];

                if (parameter.ParameterType == typeof(string))
                {
                    args[i] = groupName;
                    continue;
                }

                if (parentGroup != null && parameter.ParameterType.IsInstanceOfType(parentGroup))
                {
                    args[i] = parentGroup;
                    continue;
                }

                if (parameter.ParameterType == typeof(bool))
                {
                    args[i] = false;
                    continue;
                }

                if (parameter.ParameterType == typeof(int))
                {
                    args[i] = 0;
                    continue;
                }

                if (parameter.HasDefaultValue)
                {
                    args[i] = parameter.DefaultValue;
                    continue;
                }

                if (!parameter.ParameterType.IsValueType)
                {
                    args[i] = null;
                    continue;
                }

                return false;
            }

            return true;
        }

        private static void RepairMixerHierarchyInFile()
        {
            string mixerPathOnDisk = GetAbsoluteProjectPath(ProjectConfigAssetPaths.AudioMixer);
            if (!File.Exists(mixerPathOnDisk))
            {
                return;
            }

            List<string> lines = File.ReadAllLines(mixerPathOnDisk).ToList();
            Dictionary<string, MixerGroupBlock> groups = ParseMixerGroups(lines);
            if (!groups.TryGetValue(MasterGroupName, out MixerGroupBlock masterGroup)
                || !groups.TryGetValue(MusicGroupName, out MixerGroupBlock musicGroup)
                || !groups.TryGetValue(SfxGroupName, out MixerGroupBlock sfxGroup)
                || !groups.TryGetValue(GameplayGroupName, out MixerGroupBlock gameplayGroup)
                || !groups.TryGetValue(UiGroupName, out MixerGroupBlock uiGroup))
            {
                return;
            }

            ReplaceChildren(lines, masterGroup, new[] { musicGroup.FileId, sfxGroup.FileId });
            ReplaceChildren(lines, sfxGroup, new[] { gameplayGroup.FileId, uiGroup.FileId });

            File.WriteAllLines(mixerPathOnDisk, lines);
            AssetDatabase.ImportAsset(ProjectConfigAssetPaths.AudioMixer, ImportAssetOptions.ForceUpdate);
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

        private static void ReplaceChildren(List<string> lines, MixerGroupBlock block, IReadOnlyList<string> childFileIds)
        {
            int removeIndex = block.ChildrenLineIndex + 1;
            while (removeIndex < block.BlockEndExclusive && lines[removeIndex].StartsWith("  - {fileID:", StringComparison.Ordinal))
            {
                lines.RemoveAt(removeIndex);
                block.BlockEndExclusive--;
            }

            lines[block.ChildrenLineIndex] = "  m_Children:";
            for (int i = 0; i < childFileIds.Count; i++)
            {
                lines.Insert(block.ChildrenLineIndex + 1 + i, $"  - {{fileID: {childFileIds[i]}}}");
            }
        }

        private static bool ValidateMixerYamlHierarchy()
        {
            string mixerPathOnDisk = GetAbsoluteProjectPath(ProjectConfigAssetPaths.AudioMixer);
            if (!File.Exists(mixerPathOnDisk))
            {
                return false;
            }

            List<string> lines = File.ReadAllLines(mixerPathOnDisk).ToList();
            Dictionary<string, MixerGroupBlock> groups = ParseMixerGroups(lines);
            if (!groups.TryGetValue(MasterGroupName, out MixerGroupBlock masterGroup)
                || !groups.TryGetValue(MusicGroupName, out MixerGroupBlock musicGroup)
                || !groups.TryGetValue(SfxGroupName, out MixerGroupBlock sfxGroup)
                || !groups.TryGetValue(GameplayGroupName, out MixerGroupBlock gameplayGroup)
                || !groups.TryGetValue(UiGroupName, out MixerGroupBlock uiGroup))
            {
                return false;
            }

            return BlockContainsChildren(lines, masterGroup, musicGroup.FileId, sfxGroup.FileId)
                && BlockContainsChildren(lines, sfxGroup, gameplayGroup.FileId, uiGroup.FileId);
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

        private static void SynchronizeSceneReferences(AudioMixer mixer)
        {
            ThemeCatalog themeCatalog = AssetDatabase.LoadAssetAtPath<ThemeCatalog>(ProjectConfigAssetPaths.ThemeCatalog);
            AudioCueCatalog audioCueCatalog = AssetDatabase.LoadAssetAtPath<AudioCueCatalog>(ProjectConfigAssetPaths.AudioCueCatalog);
            GameBalanceConfig gameBalance = AssetDatabase.LoadAssetAtPath<GameBalanceConfig>(ProjectConfigAssetPaths.GameBalance);
            DifficultyCurveConfig difficultyCurve = AssetDatabase.LoadAssetAtPath<DifficultyCurveConfig>(ProjectConfigAssetPaths.DifficultyCurve);
            ObstacleCatalog obstacleCatalog = AssetDatabase.LoadAssetAtPath<ObstacleCatalog>(ProjectConfigAssetPaths.ObstacleCatalog);
            VfxCatalog vfxCatalog = AssetDatabase.LoadAssetAtPath<VfxCatalog>(ProjectConfigAssetPaths.VfxCatalog);
            InputActionAsset inputActions = AssetDatabase.LoadAssetAtPath<InputActionAsset>(ProjectConfigAssetPaths.InputActions);

            Scene mainMenuScene = EditorSceneManager.OpenScene(ProjectConfigAssetPaths.MainMenuScene, OpenSceneMode.Single);
            MainMenuView mainMenuView = UnityEngine.Object.FindFirstObjectByType<MainMenuView>();
            if (mainMenuView != null)
            {
                SerializedObject serializedMainMenu = new(mainMenuView);
                SetObjectReference(serializedMainMenu, "themeCatalog", themeCatalog);
                SetObjectReference(serializedMainMenu, "audioCueCatalog", audioCueCatalog);
                SetObjectReference(serializedMainMenu, "audioMixer", mixer);
                serializedMainMenu.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(mainMenuView);
                EditorSceneManager.SaveScene(mainMenuScene);
            }

            Scene gameplayScene = EditorSceneManager.OpenScene(ProjectConfigAssetPaths.GameplayScene, OpenSceneMode.Single);
            GameplaySceneInstaller gameplayInstaller = UnityEngine.Object.FindFirstObjectByType<GameplaySceneInstaller>();
            if (gameplayInstaller != null)
            {
                SerializedObject serializedInstaller = new(gameplayInstaller);
                SetObjectReference(serializedInstaller, "gameBalance", gameBalance);
                SetObjectReference(serializedInstaller, "difficultyCurve", difficultyCurve);
                SetObjectReference(serializedInstaller, "obstacleCatalog", obstacleCatalog);
                SetObjectReference(serializedInstaller, "themeCatalog", themeCatalog);
                SetObjectReference(serializedInstaller, "audioCueCatalog", audioCueCatalog);
                SetObjectReference(serializedInstaller, "vfxCatalog", vfxCatalog);
                SetObjectReference(serializedInstaller, "audioMixer", mixer);
                SetObjectReference(serializedInstaller, "inputActions", inputActions);
                serializedInstaller.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(gameplayInstaller);
                EditorSceneManager.SaveScene(gameplayScene);
            }
        }

        private static void SetObjectReference(SerializedObject serializedObject, string propertyName, UnityEngine.Object value)
        {
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            if (property != null)
            {
                property.objectReferenceValue = value;
            }
        }

        private static AudioMixerGroup FindExactGroup(AudioMixer mixer, string groupName)
        {
            if (mixer == null || string.IsNullOrWhiteSpace(groupName))
            {
                return null;
            }

            return mixer.FindMatchingGroups(groupName)
                .FirstOrDefault(group => group != null && group.name == groupName);
        }

        private static object LoadControllerAsset(Type controllerType)
        {
            UnityEngine.Object[] assets = AssetDatabase.LoadAllAssetsAtPath(ProjectConfigAssetPaths.AudioMixer);
            for (int i = 0; i < assets.Length; i++)
            {
                if (assets[i] != null && controllerType.IsInstanceOfType(assets[i]))
                {
                    return assets[i];
                }
            }

            return null;
        }

        private static object LoadGroupControllerByName(string groupName)
        {
            Type groupControllerType = FindEditorObjectType("AudioMixerGroupController");
            if (groupControllerType == null)
            {
                return null;
            }

            UnityEngine.Object[] assets = AssetDatabase.LoadAllAssetsAtPath(ProjectConfigAssetPaths.AudioMixer);
            for (int i = 0; i < assets.Length; i++)
            {
                if (assets[i] != null && groupControllerType.IsInstanceOfType(assets[i]) && assets[i].name == groupName)
                {
                    return assets[i];
                }
            }

            return null;
        }

        private static object GetMemberValue(object target, string memberName)
        {
            if (target == null || string.IsNullOrWhiteSpace(memberName))
            {
                return null;
            }

            const BindingFlags Flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            PropertyInfo property = target.GetType().GetProperty(memberName, Flags);
            if (property != null)
            {
                return property.GetValue(target);
            }

            FieldInfo field = target.GetType().GetField(memberName, Flags);
            if (field != null)
            {
                return field.GetValue(target);
            }

            MethodInfo method = target.GetType().GetMethod(memberName, Flags, null, Type.EmptyTypes, null);
            if (method != null)
            {
                return method.Invoke(target, null);
            }

            return null;
        }

        private static Type FindEditorObjectType(string shortName)
        {
            return TypeCache.GetTypesDerivedFrom<UnityEngine.Object>()
                .FirstOrDefault(type => type.Name == shortName);
        }

        private static void EnsureFolder(string folderPath)
        {
            string[] segments = folderPath.Split('/');
            string current = segments[0];
            for (int i = 1; i < segments.Length; i++)
            {
                string next = current + "/" + segments[i];
                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, segments[i]);
                }

                current = next;
            }
        }

        private static string GetAbsoluteProjectPath(string assetPath)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), assetPath.Replace('/', Path.DirectorySeparatorChar));
        }
    }
}
#endif
