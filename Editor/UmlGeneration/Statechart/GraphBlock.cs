namespace Maze.StateFoundry.Editor
{
    sealed class GraphBlock : Statechart<Idle>
    {
    }

    class Idle : State, IGet<ScriptImported, BlockEvaluation>
    {
    }

    class Error : State, IGet<ErrorHandled, Idle>
    {
    }

    class Update : State, IGet<ErrorRaised, Error>
    {
    }

    class BlockEvaluation : Update, IGet<ScriptIsBlock, HierarchyAnalysis>, IGet<ScriptIsNotBlock, Idle>
    {
    }

    class Generation : Update
    {
    }

    class HierarchyAnalysis : Generation, IGet<HierarchyAnalyzed, TextGeneration>
    {
    }

    class TextGeneration : Generation, IGet<TextGenerated, UmlPrinting>
    {
    }

    class UmlPrinting : Generation, IGet<UmlPrinted, Idle>
    {
    }
}