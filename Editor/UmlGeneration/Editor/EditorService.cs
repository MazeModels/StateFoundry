using UnityEditor;

namespace Maze.StateFoundry.Editor
{
    [InitializeOnLoad]
    static class EditorService
    {
        static readonly Installer s_installer;

        static EditorService()
        {
            EditorApplication.quitting += Dispose;
            AssemblyReloadEvents.beforeAssemblyReload += Dispose;
            s_installer = new Installer();
            s_installer.Run();
        }

        static void Dispose()
        {
            s_installer?.Dispose();
        }
    }
}