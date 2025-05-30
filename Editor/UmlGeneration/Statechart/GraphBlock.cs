namespace Maze.StateFoundry.Editor
{
    sealed class GraphBlock : Statechart<Idle>
    {
    }

    [UmlNote(When.OnEnter, nameof(Ready))]
    class Idle : State, IGet<ScriptImported, BlockEvaluation>
    {
        public override void OnEnter()
        {
            Send(new Ready());
        }
    }

    [UmlNote(When.OnEnter, nameof(RaiseError))]
    class Error : State, IGet<ErrorHandled, Idle>
    {
        public override void OnEnter()
        {
            Send(new RaiseError());
        }
    }

    class Update : State, IGet<ErrorRaised, Error>
    {
    }

    [UmlNote(When.OnEnter, nameof(CheckIfBlock))]
    class BlockEvaluation : Update, IGet<ScriptIsBlock, HierarchyAnalysis>, IGet<ScriptIsNotBlock, Idle>
    {
        public override void OnEnter()
        {
            Send(new CheckIfBlock());
        }
    }

    class Generation : Update
    {
    }

    [UmlNote(When.OnEnter, nameof(AnalyzeHierarchy))]
    class HierarchyAnalysis : Generation, IGet<HierarchyAnalyzed, TextGeneration>
    {
        public override void OnEnter()
        {
            Send(new AnalyzeHierarchy());
        }
    }

    [UmlNote(When.OnEnter, nameof(GenerateText))]
    class TextGeneration : Generation, IGet<TextGenerated, UmlPrinting>
    {
        public override void OnEnter()
        {
            Send(new GenerateText());
        }
    }

    [UmlNote(When.OnEnter, nameof(PrintUml))]
    class UmlPrinting : Generation, IGet<UmlPrinted, Idle>
    {
        public override void OnEnter()
        {
            Send(new PrintUml());
        }
    }

    [UmlNote(When.OnEnter, nameof(DeleteGraph))]
    class GraphDeletion : Update, IGet<GraphDeleted, Idle>
    {
        public override void OnEnter()
        {
            Send(new DeleteGraph());
        }
    }
}