namespace Maze.StateFoundry
{
    interface IInternalState
    {
        internal IBlackboard Blackboard { get; set; }
        
        internal void InternalOnEnter();
        internal void InternalOnExit();
        internal void InternalOnCreate();
        internal void InternalOnDispose();
    }
}