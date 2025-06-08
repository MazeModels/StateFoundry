using System;
using System.IO;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Maze.StateFoundry.Editor
{
    class DrawStatechart
    {
        const string MENU_TEXT = "Assets/Statechart/Update Statechart";

        [MenuItem(MENU_TEXT, true)]
        static bool ValidateDrawStatechartTrigger()
        {
            foreach (Object obj in Selection.objects)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                if (!path.EndsWith(".cs"))
                {
                    return false;
                }

            }
            return Selection.objects.Length > 0;
        }

        [MenuItem(MENU_TEXT)]
        static void DrawStatechartTrigger()
        {
            using var installer = new Installer();
            foreach (Object obj in Selection.objects)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                installer.Run(path);
                ImportNewAsset(path);
            }
        }

        static void ImportNewAsset(string pathToScript)
        {
            if (TryGetNewAsset(pathToScript, out string pathToAsset))
            {
                AssetDatabase.ImportAsset(pathToAsset);
            }
        }

        static bool TryGetNewAsset(string pathToScript, out string pathToAsset)
        {
            if (string.IsNullOrEmpty(pathToScript))
            {
                pathToAsset = null;
                return false;
            }

            if (!TryGetAssetNameNoThrow(pathToScript, out pathToAsset))
            {
                return false;
            }

            pathToAsset = Path.Combine(Path.GetDirectoryName(pathToScript), pathToAsset);
            return true;
        }

        static bool TryGetAssetNameNoThrow(string pathToScript, out string pathToAsset)
        {
            try
            {
                pathToAsset = $"{Path.GetFileNameWithoutExtension(pathToScript)}{UmlPrinter.FILE_EXTENSION}";
                return true;
            }
            catch (Exception)
            {
                pathToAsset = null;
                return false;
            }
        }
    }

}

