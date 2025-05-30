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

        public void Send<TEvent>(TEvent ev) where TEvent : struct, IEvent
        {
            LogEventSent<TEvent>();
            StateData newData = m_events.Send(ev);

            if (newData == null)
            {
                return;
            }

            LogTransition(m_pool.CurrentData, newData);
            m_pool.SetCurrentState(newData);
        }

        public void Listen<TEvent>(Action<TEvent> callback) where TEvent : struct, IEvent
        {
            m_events.Listen<TEvent>(output =>
            {
                LogEventReceived<TEvent>();
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

        void OnSend(IEvent ev)
        {
            MethodInfo method = GetType().GetMethod(nameof(Send));
            MethodInfo genericMethod = method?.MakeGenericMethod(ev.GetType());
            genericMethod?.Invoke(this, new object[] { ev });
        }

        void LogEventSent<TEvent>() where TEvent : struct, IEvent
        {
            Debug.Log($"<color=yellow>[S]|{m_statechartType.Name}|: {typeof(TEvent).Name}</color>");
        }

        static void LogTransition(StateData oldData, StateData newData)
        {
            Debug.Log($"<color=cyan>[T]: |{oldData}| => |{newData}|</color>");
        }

        void LogEventReceived<TEvent>() where TEvent : struct, IEvent
        {
            Debug.Log($"<color=magenta>[R]|{m_statechartType.Name}|: {typeof(TEvent).Name}</color>");
        }
    }
}