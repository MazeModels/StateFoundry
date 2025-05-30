namespace Maze.StateFoundry.Editor
{
    readonly struct ScriptImported : ITrigger { }
    readonly struct ErrorRaised : ITrigger { }
    readonly struct ErrorHandled : ITrigger { }
    readonly struct ScriptIsBlock : ITrigger { }
    readonly struct ScriptIsNotBlock : ITrigger { }
    readonly struct HierarchyAnalyzed : ITrigger { }
    readonly struct TextGenerated : ITrigger { }
    readonly struct UmlPrinted : ITrigger { }
    readonly struct GraphDeleted : ITrigger { }
}