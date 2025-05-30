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
    }
}