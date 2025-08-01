using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Maze.StateFoundry
{
    sealed class StatechartEvents<TInitialState> : IStatechartEvents<TInitialState> where TInitialState : State, new()
    {
        readonly IStatePool<TInitialState> m_pool;
        readonly Dictionary<Type, Delegate> m_callbacks;

        public StatechartEvents(IStatePool<TInitialState> pool)
        {
            m_pool = pool;
            m_callbacks = new Dictionary<Type, Delegate>();
            SubscribeToEvents();
        }

        public void Dispose()
        {
            UnsubscribeToEvents();
        }

        public IStateData Send<TTrigger>(TTrigger trigger) where TTrigger : struct, ITrigger
        {
            IStateData currentData = m_pool.GetCurrentState();
            return SendRecurse(currentData, trigger);
        }

        public void Listen<TTrigger>(Action<TTrigger> callback) where TTrigger : struct, ITrigger
        {
            EnsureIsNotNull(callback);
            Type type = typeof(TTrigger);
            if (m_callbacks.TryGetValue(type, out Delegate existing))
            {
                m_callbacks[type] = Delegate.Combine(existing, callback);
            }
            else
            {
                m_callbacks[type] = callback;
            }
        }

        public void OnLifecycleEvent<TState>(When expected, Action<TState> callback) where TState : State, new()
        {
            EnsureIsNotNull(callback);
            IInternalState state = EnsureStateIsRegistered<TState>();
            state.OnLifecycleEvent += actual => OnLifecycleEvent(expected, actual, state, callback);
        }


        IStateData SendRecurse<TTrigger>(IStateData data, TTrigger trigger) where TTrigger : struct, ITrigger
        {
            if (data == null)
            {
                return null;
            }

            Type stateType = data.State.GetType();
            Type[] interfaces = stateType.GetInterfaces();

            foreach (Type iface in interfaces)
            {
                if (!iface.IsGenericType || iface.GetGenericTypeDefinition() != typeof(IGet<,>))
                {
                    continue;
                }

                Type[] genericArgs = iface.GetGenericArguments();
                Type triggerType = genericArgs[0];
                Type targetType = genericArgs[1];

                if (triggerType != typeof(TTrigger))
                {
                    continue;
                }

                if (!m_pool.GetStates().TryGetValue(targetType, out IStateData instance))
                {
                    throw new ArgumentException($"Type {targetType} is not registered");
                }

                Type intType = typeof(IGet<,>).MakeGenericType(typeof(TTrigger), targetType);
                MethodInfo method = stateType.GetInterfaceMap(intType).TargetMethods.FirstOrDefault(m => m.Name.Contains("Get"));
                method?.Invoke(data.State, new object[] { trigger });

                OnSend(trigger);

                return instance;
            }

            return SendRecurse(data.Parent, trigger);
        }

        void SubscribeToEvents()
        {
            foreach (IStateData data in m_pool.GetStates().Values)
            {
                data.State.OnEventSent += OnSend;
            }
        }

        void UnsubscribeToEvents()
        {
            foreach (IStateData data in m_pool.GetStates().Values)
            {
                data.State.OnEventSent -= OnSend;
            }
        }

        void OnSend(ITrigger output)
        {
            if (m_callbacks.TryGetValue(output.GetType(), out Delegate del))
            {
                del.DynamicInvoke(output);
            }
        }

        void OnLifecycleEvent<TState>(When expected, When actual, IInternalState state, Action<TState> callback) where TState : State, new()
        {
            if (expected == actual)
            {
                callback?.Invoke(state as TState);
            }
        }

        IInternalState EnsureStateIsRegistered<TState>() where TState : State, new()
        {
            if (!m_pool.GetStates().TryGetValue(typeof(TState), out IStateData data))
            {
                throw new ArgumentException($"State {typeof(TState).FullName} is not registered");
            }

            return data.State;
        }

        void EnsureIsNotNull<T>(T instance) where T : class
        {
            if (instance == null)
            {
                throw new ArgumentNullException();
            }
        }
    }
}