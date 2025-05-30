namespace Maze.StateFoundry.Editor
{
    sealed class GraphBlock : Statechart<Idle>
    {
    }

    [Caption(When.OnEnter, nameof(Ready))]
    class Idle : State, IGet<ScriptImported, BlockEvaluation>
    {
        public override void OnEnter()
        {
            Send(new Ready());
        }
    }

    [Caption(When.OnEnter, nameof(RaiseError))]
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

    [Caption(When.OnEnter, nameof(CheckIfBlock))]
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

    [Caption(When.OnEnter, nameof(AnalyzeHierarchy))]
    class HierarchyAnalysis : Generation, IGet<HierarchyAnalyzed, TextGeneration>
    {
        public override void OnEnter()
        {
            Send(new AnalyzeHierarchy());
        }
    }

    [Caption(When.OnEnter, nameof(GenerateText))]
    class TextGeneration : Generation, IGet<TextGenerated, UmlPrinting>
    {
        public override void OnEnter()
        {
            Send(new GenerateText());
        }
    }

    [Caption(When.OnEnter, nameof(PrintUml))]
    class UmlPrinting : Generation, IGet<UmlPrinted, Idle>
    {
        public override void OnEnter()
        {
            Send(new PrintUml());
        }
    }

    [Caption(When.OnEnter, nameof(DeleteGraph))]
    class GraphDeletion : Update, IGet<GraphDeleted, Idle>
    {
        public override void OnEnter()
        {
            Send(new DeleteGraph());
        }
    }
}