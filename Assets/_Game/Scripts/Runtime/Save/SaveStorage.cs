using System.IO;
using UnityEngine;

namespace Voltline.Save
{
    public static class SaveStorage
    {
        private static string overridePath;

        public static string ProfilePath => string.IsNullOrWhiteSpace(overridePath)
            ? Path.Combine(Application.persistentDataPath, "voltline-profile.json")
            : overridePath;

        public static bool TryReadProfile(out string json)
        {
            string path = ProfilePath;
            if (!File.Exists(path))
            {
                json = null;
                return false;
            }

            json = File.ReadAllText(path);
            return !string.IsNullOrWhiteSpace(json);
        }

        public static void WriteProfile(string json)
        {
            string path = ProfilePath;
            string directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(path, json);
        }

        public static void DeleteProfile()
        {
            string path = ProfilePath;
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public static void SetOverridePathForTests(string path)
        {
            overridePath = path;
        }

        public static void ClearOverridePathForTests()
        {
            overridePath = null;
        }
    }
}
