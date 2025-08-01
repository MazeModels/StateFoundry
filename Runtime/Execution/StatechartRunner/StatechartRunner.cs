using System;
using System.Reflection;
using UnityEngine;

namespace Maze.StateFoundry
{
    sealed class StatechartRunner<TInitialState> : IStatechartRunner where TInitialState : State, new()
    {
        public event Action<ITrigger> OnTrigger;

        readonly Type m_statechartType;
        readonly IStatePool<TInitialState> m_pool;
        readonly IStatechartEvents<TInitialState> m_events;
        readonly IBlackboard m_blackboard;

        bool m_isRunning;

        public StatechartRunner(Type statechartType, IBlackboard blackboard, IStatePool<TInitialState> pool, IStatechartEvents<TInitialState> events)
        {
            m_statechartType = statechartType;
            m_blackboard = blackboard;
            m_pool = pool;
            m_events = events;
            CheckPrecondtions();
            SubscribeToEvents();
        }

        public void Dispose()
        {
            UnsubscribeToEvents();
            m_pool.Dispose();
        }


        public void Start()
        {
            m_isRunning = true;

            foreach (IStateData state in m_pool.GetStates().Values)
            {
                state.State.InternalOnCreate();
            }
        }

        public void Send<TTrigger>(TTrigger trigger) where TTrigger : struct, ITrigger
        {
            EnsureIsRunning();
            IStateData newData = m_events.Send(trigger);

            if (newData == null)
            {
                return;
            }

            LogTransition(m_pool.GetCurrentState(), newData, trigger);
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
            foreach (IStateData data in m_pool.GetStates().Values)
            {
                data.State.OnEventSent += InternalSend;
            }
        }

        void UnsubscribeToEvents()
        {
            foreach (IStateData data in m_pool.GetStates().Values)
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

        void LogTransition(IStateData oldData, IStateData newData, ITrigger trigger)
        {
            Debug.Log($"<color=cyan>{{R}}|[{m_statechartType.Name}]: [{oldData}] --{trigger.GetType().Name}--> [{newData}]</color>");
        }

        void LogEventReceived<TTrigger>() where TTrigger : struct, ITrigger
        {
            Debug.Log($"<color=magenta>{{R}}|{m_statechartType.Name}|: {typeof(TTrigger).Name}</color>");
        }

        void EnsureIsRunning()
        {
            if (!m_isRunning)
            {
                throw new InvalidOperationException("The state machine is not running.");
            }
        }

        void CheckPrecondtions()
        {
            EnsureArgumentsAreNotNull(m_blackboard, m_pool, m_events);
            EnsureTypeIsNotNull(m_statechartType);
            EnsureIsNotAbstract(m_statechartType);
            EnsureIsStatechartType(m_statechartType);
        }

        static void EnsureTypeIsNotNull(Type statechartType)
        {
            if (statechartType == null)
            {
                throw new ArgumentNullException(nameof(statechartType));
            }
        }

        static void EnsureIsNotAbstract(Type statechartType)
        {
            if (statechartType.IsAbstract)
            {
                throw new ArgumentException($"The provided type '{statechartType.FullName}' must not be abstract.");
            }
        }

        static void EnsureIsStatechartType(Type statechartType)
        {
            if (!typeof(Statechart<TInitialState>).IsAssignableFrom(statechartType))
            {
                throw new ArgumentException($"The provided type '{statechartType.FullName}' must be assignable to '{typeof(Statechart<TInitialState>).FullName}'.");
            }
        }
        static void EnsureArgumentsAreNotNull(params object[] args)
        {
            foreach (var arg in args)
            {
                if (arg == null)
                {
                    throw new ArgumentNullException();
                }
            }
        }
    }
}