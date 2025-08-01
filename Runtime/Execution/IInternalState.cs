using System;

namespace Maze.StateFoundry
{
    interface IInternalState : IState, IDisposable
    {
        internal event Action<ITrigger> OnEventSent;
        internal event Action<When> OnLifecycleEvent;
        
        internal IBlackboard Blackboard { get; set; }

        internal void InternalOnEnter();
        internal void InternalOnExit();
        internal void InternalOnCreate();
        internal void InternalOnDispose();
    }
}