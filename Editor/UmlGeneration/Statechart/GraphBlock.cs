namespace Maze.StateFoundry.Editor
{
    sealed class GraphBlock : Statechart<Idle>
    {
    }

    [UmlNote(Method.OnEnter, nameof(Ready))]
    class Idle : State, IGet<ScriptImported, BlockEvaluation>
    {
        public override void OnEnter()
        {
            Send(new Ready());
        }
    }

    [UmlNote(Method.OnEnter, nameof(RaiseError))]
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

    [UmlNote(Method.OnEnter, nameof(CheckIfBlock))]
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

    [UmlNote(Method.OnEnter, nameof(AnalyzeHierarchy))]
    class HierarchyAnalysis : Generation, IGet<HierarchyAnalyzed, TextGeneration>
    {
        public override void OnEnter()
        {
            Send(new AnalyzeHierarchy());
        }
    }

    [UmlNote(Method.OnEnter, nameof(GenerateText))]
    class TextGeneration : Generation, IGet<TextGenerated, UmlPrinting>
    {
        public override void OnEnter()
        {
            Send(new GenerateText());
        }
    }

    [UmlNote(Method.OnEnter, nameof(PrintUml))]
    class UmlPrinting : Generation, IGet<UmlPrinted, Idle>
    {
        public override void OnEnter()
        {
            Send(new PrintUml());
        }
    }

    [UmlNote(Method.OnEnter, nameof(DeleteGraph))]
    class GraphDeletion : Update, IGet<GraphDeleted, Idle>
    {
        public override void OnEnter()
        {
            Send(new DeleteGraph());
        }
    }
}