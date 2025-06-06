namespace Maze.StateFoundry
{
    interface IInternalState
    {
        internal void InternalOnEnter();
        internal void InternalOnExit();
        internal void InternalOnCreate();
        internal void InternalOnDispose();
    }
}