using System;
using UnityEditor;

namespace Maze.StateFoundry.Editor
{
    sealed class ScriptWatcher : AssetPostprocessor
    {
        const string SCRIPT_EXTENSION = ".cs";

        public static event Action<string> OnChange;
        public static event Action<string> OnDeletion;

        static void OnPostprocessAllAssets(string[] imported, string[] deleted, string[] moved, string[] movedFromAssetPaths)
        {
            ProcessAssets(imported, OnChange);
            ProcessAssets(deleted, OnDeletion);
            ProcessAssets(movedFromAssetPaths, OnDeletion);
        }

        static void ProcessAssets(string[] assets, Action<string> ev)
        {
            foreach (string asset in assets)
            {
                if (asset.EndsWith(SCRIPT_EXTENSION))
                {
                    ev?.Invoke(asset);
                }
            }
        }
    }
}
