namespace Maze.StateFoundry.Editor
{
    readonly struct Ready : IEvent { }
    readonly struct RaiseError : IEvent { }
    readonly struct CheckIfBlock : IEvent { }
    readonly struct DeleteGraph : IEvent { }
    readonly struct AnalyzeHierarchy : IEvent { }
    readonly struct GenerateText : IEvent { }
    readonly struct PrintUml : IEvent { }
}