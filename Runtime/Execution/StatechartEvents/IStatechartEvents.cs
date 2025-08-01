using System;

namespace Maze.StateFoundry
{
    interface IStatechartEvents<TInitialState> : IDisposable where TInitialState : State, new()
    {
        IStateData Send<TTrigger>(TTrigger trigger) where TTrigger : struct, ITrigger;
        void Listen<TTrigger>(Action<TTrigger> callback) where TTrigger : struct, ITrigger;
        void OnLifecycleEvent<TState>(When expected, Action<TState> callback) where TState : State, new();
    }
}
