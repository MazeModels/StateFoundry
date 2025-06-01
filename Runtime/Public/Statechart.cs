using System;

namespace Maze.StateFoundry
{
    public abstract class Statechart<TInitialState> : IDisposable where TInitialState : State, new()
    {
        readonly StatechartRunner<TInitialState> m_runner;

        public Statechart()
        {
            m_runner = new StatechartRunner<TInitialState>(GetType());
        }

        public void Dispose()
        {
            m_runner.Dispose();
        }


        public void Send<TTrigger>(TTrigger trigger) where TTrigger : struct, ITrigger
        {
            m_runner.Send(trigger);
        }

        public void Listen<TTrigger>(Action<TTrigger> callback) where TTrigger : struct, ITrigger
        {
            m_runner.Listen(callback);
        }


        public void OnEnter<TState>(Action<TState> callback) where TState : State, new()
        {
            m_runner.OnEnter(callback);
        }

        public void OnExit<TState>(Action<TState> callback) where TState : State, new()
        {
            m_runner.OnExit(callback);
        }

        public void OnCreate<TState>(Action<TState> callback) where TState : State, new()
        {
            m_runner.OnCreate(callback);
        }

        public void OnDispose<TState>(Action<TState> callback) where TState : State, new()
        {
            m_runner.OnDispose(callback);
        }
    }
}