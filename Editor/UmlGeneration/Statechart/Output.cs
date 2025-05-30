namespace Maze.StateFoundry.Editor
{
    readonly struct Ready : ITrigger { }
    readonly struct RaiseError : ITrigger { }
    readonly struct CheckIfBlock : ITrigger { }
    readonly struct DeleteGraph : ITrigger { }
    readonly struct AnalyzeHierarchy : ITrigger { }
    readonly struct GenerateText : ITrigger { }
    readonly struct PrintUml : ITrigger { }
}