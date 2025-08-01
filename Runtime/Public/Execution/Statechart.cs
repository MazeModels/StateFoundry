using System;

namespace Maze.StateFoundry
{
    public abstract class Statechart<TInitialState> : IInternalStatechart, IDisposable where TInitialState : State, new()
    {
        IStatechartRunner IInternalStatechart.Runner => m_runner;
        readonly IStatechartRunner m_runner;

        public Statechart()
        {
            m_runner = StatechartRunnerFactory.Build<TInitialState>(GetType());
        }

        public void Dispose()
        {
            m_runner.Dispose();
        }


        public void Start()
        {
            m_runner.Start();
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

        public void Add<T>(T component)
        {
            m_runner.Add(component);
        }

        public T Get<T>()
        {
            return m_runner.Get<T>();
        }
    }
}