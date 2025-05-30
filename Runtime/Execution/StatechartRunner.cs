using System;
using System.Reflection;
using UnityEngine;

namespace Maze.StateFoundry
{
    sealed class StatechartRunner<TInitialState> : IDisposable where TInitialState : State, new()
    {
        readonly Type m_statechartType;
        readonly StatePool<TInitialState> m_pool;
        readonly StatechartEvents<TInitialState> m_events;

        public StatechartRunner(Type statechartType)
        {
            m_statechartType = statechartType;
            m_pool = new StatePool<TInitialState>();
            m_events = new StatechartEvents<TInitialState>(m_pool);
            SubscribeToEvents();
        }

        public void Dispose()
        {
            UnsubscribeToEvents();
            m_pool.Dispose();
        }

        public void Send<TTrigger>(TTrigger trigger) where TTrigger : struct, ITrigger
        {
            LogEventSent<TTrigger>();
            StateData newData = m_events.Send(trigger);

            if (newData == null)
            {
                return;
            }

            LogTransition(m_pool.CurrentData, newData);
            m_pool.SetCurrentState(newData);
        }

        public void Listen<TTrigger>(Action<TTrigger> callback) where TTrigger : struct, ITrigger
        {
            m_events.Listen<TTrigger>(output =>
            {
                LogEventReceived<TTrigger>();
                callback?.Invoke(output);
            });
        }

        void SubscribeToEvents()
        {
            foreach (StateData data in m_pool.States.Values)
            {
                data.State.OnEventSent += OnSend;
            }
        }

        void UnsubscribeToEvents()
        {
            foreach (StateData data in m_pool.States.Values)
            {
                data.State.OnEventSent -= OnSend;
            }
        }

        void OnSend(ITrigger trigger)
        {
            MethodInfo method = GetType().GetMethod(nameof(Send));
            MethodInfo genericMethod = method?.MakeGenericMethod(trigger.GetType());
            genericMethod?.Invoke(this, new object[] { trigger });
        }

        void LogEventSent<TTrigger>() where TTrigger : struct, ITrigger
        {
            Debug.Log($"<color=yellow>[S]|{m_statechartType.Name}|: {typeof(TTrigger).Name}</color>");
        }

        static void LogTransition(StateData oldData, StateData newData)
        {
            Debug.Log($"<color=cyan>[T]: |{oldData}| => |{newData}|</color>");
        }

        void LogEventReceived<TTrigger>() where TTrigger : struct, ITrigger
        {
            Debug.Log($"<color=magenta>[R]|{m_statechartType.Name}|: {typeof(TTrigger).Name}</color>");
        }
    }
}