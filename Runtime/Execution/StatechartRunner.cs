using System;
using System.Reflection;
using UnityEngine;

namespace Maze.StateFoundry
{
    sealed class StatechartRunner<TInitialState> : IStatechartRunner, IBlackboard, IDisposable where TInitialState : State, new()
    {
        public event Action<ITrigger> OnTrigger;

        readonly Type m_statechartType;
        readonly StatePool<TInitialState> m_pool;
        readonly StatechartEvents<TInitialState> m_events;
        readonly StatechartBlackboard m_blackboard;

        public StatechartRunner(Type statechartType)
        {
            m_statechartType = statechartType;
            m_blackboard = new StatechartBlackboard();
            m_pool = new StatePool<TInitialState>(m_blackboard);
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
            StateData newData = m_events.Send(trigger);

            if (newData == null)
            {
                return;
            }

            LogTransition(m_pool.CurrentData, newData, trigger);
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


        public void OnEnter<TState>(Action<TState> callback) where TState : State, new()
        {
            m_events.OnLifecycleEvent(When.OnEnter, callback);
        }

        public void OnExit<TState>(Action<TState> callback) where TState : State, new()
        {
            m_events.OnLifecycleEvent(When.OnExit, callback);
        }

        public void OnCreate<TState>(Action<TState> callback) where TState : State, new()
        {
            m_events.OnLifecycleEvent(When.OnCreate, callback);
        }

        public void OnDispose<TState>(Action<TState> callback) where TState : State, new()
        {
            m_events.OnLifecycleEvent(When.OnDispose, callback);
        }

        public void Add<T>(T component)
        {
            m_blackboard.Add(component);
        }

        public T Get<T>()
        {
            return m_blackboard.Get<T>();
        }


        void SubscribeToEvents()
        {
            foreach (StateData data in m_pool.States.Values)
            {
                data.State.OnEventSent += InternalSend;
            }
        }

        void UnsubscribeToEvents()
        {
            foreach (StateData data in m_pool.States.Values)
            {
                data.State.OnEventSent -= InternalSend;
            }
        }

        void InternalSend(ITrigger trigger)
        {
            MethodInfo method = GetType().GetMethod(nameof(Send));
            MethodInfo genericMethod = method?.MakeGenericMethod(trigger.GetType());
            genericMethod?.Invoke(this, new object[] { trigger });
            OnTrigger?.Invoke(trigger);
        }

        void LogTransition(StateData oldData, StateData newData, ITrigger trigger)
        {
            Debug.Log($"<color=cyan>{{R}}|[{m_statechartType.Name}]: [{oldData}] --{trigger.GetType().Name}--> [{newData}]</color>");
        }

        void LogEventReceived<TTrigger>() where TTrigger : struct, ITrigger
        {
            Debug.Log($"<color=magenta>{{R}}|{m_statechartType.Name}|: {typeof(TTrigger).Name}</color>");
        }
    }
}