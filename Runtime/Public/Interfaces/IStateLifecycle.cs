using System;

namespace Maze.StateFoundry
{
    public interface IStateLifecycle
    {
        public void OnEnter<TState>(Action<TState> callback) where TState : State, new();
        public void OnExit<TState>(Action<TState> callback) where TState : State, new();
        public void OnCreate<TState>(Action<TState> callback) where TState : State, new();
        public void OnDispose<TState>(Action<TState> callback) where TState : State, new();
    }
}