using System;

namespace Maze.StateFoundry
{
    public sealed class Stateworld : IStatechart, IDisposable
    {
        readonly StatechartWorld m_internal;

        public Stateworld(params IStatechart[] charts)
        {
            m_internal = new StatechartWorld(charts);
        }
        
        public void Dispose()
        {
            m_internal.Dispose();
        }


        public void Start()
        {
            m_internal.Start();
        }

        public void Send<TTrigger>(TTrigger trigger) where TTrigger : struct, ITrigger
        {
            m_internal.Send(trigger);
        }

        public void Listen<TTrigger>(Action<TTrigger> callback) where TTrigger : struct, ITrigger
        {
            m_internal.Listen(callback);
        }

        public void OnEnter<TState>(Action<TState> callback) where TState : State, new()
        {
            m_internal.OnEnter(callback);
        }

        public void OnExit<TState>(Action<TState> callback) where TState : State, new()
        {
            m_internal.OnExit(callback);
        }

        public void OnCreate<TState>(Action<TState> callback) where TState : State, new()
        {
            m_internal.OnCreate(callback);
        }

        public void OnDispose<TState>(Action<TState> callback) where TState : State, new()
        {
            m_internal.OnDispose(callback);
        }
    }
}