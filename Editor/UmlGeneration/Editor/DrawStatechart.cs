using UnityEditor;
using UnityEngine;

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
            }
        }
    }

}

