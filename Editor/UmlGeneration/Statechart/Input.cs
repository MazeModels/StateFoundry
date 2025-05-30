namespace Maze.StateFoundry.Editor
{
    readonly struct ScriptImported : IEvent { }
    readonly struct ErrorRaised : IEvent { }
    readonly struct ErrorHandled : IEvent { }
    readonly struct ScriptIsBlock : IEvent { }
    readonly struct ScriptIsNotBlock : IEvent { }
    readonly struct HierarchyAnalyzed : IEvent { }
    readonly struct TextGenerated : IEvent { }
    readonly struct UmlPrinted : IEvent { }
    readonly struct GraphDeleted : IEvent { }
}