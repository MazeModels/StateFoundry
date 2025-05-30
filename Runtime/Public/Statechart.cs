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


        public void Send<TEvent>(TEvent ev) where TEvent : struct, ITrigger
        {
            m_runner.Send(ev);
        }

        public void Listen<TEvent>(Action<TEvent> callback) where TEvent : struct, ITrigger
        {
            m_runner.Listen(callback);
        }
    }
}