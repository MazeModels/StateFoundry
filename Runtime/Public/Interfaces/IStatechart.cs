using System;

namespace Maze.StateFoundry
{
    public interface IStatechart
    {
        public void Send<TTrigger>(TTrigger trigger) where TTrigger : struct, ITrigger;
        public void Listen<TTrigger>(Action<TTrigger> callback) where TTrigger : struct, ITrigger;
        
        public void OnEnter<TState>(Action<TState> callback) where TState : State, new();
        public void OnExit<TState>(Action<TState> callback) where TState : State, new();
        public void OnCreate<TState>(Action<TState> callback) where TState : State, new();
        public void OnDispose<TState>(Action<TState> callback) where TState : State, new();
    }
}