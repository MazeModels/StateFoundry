using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Maze.StateFoundry
{
    sealed class StatechartEvents<TInitialState> : IDisposable where TInitialState : State, new()
    {
        readonly StatePool<TInitialState> m_pool;
        readonly Dictionary<Type, Delegate> m_callbacks;

        public StatechartEvents(StatePool<TInitialState> pool)
        {
            m_pool = pool;
            m_callbacks = new Dictionary<Type, Delegate>();
            SubscribeToEvents();
        }

        public void Dispose()
        {
            UnsubscribeToEvents();
        }

        public StateData Send<TTrigger>(TTrigger trigger) where TTrigger : struct, ITrigger
        {
            StateData currentData = m_pool.CurrentData;
            return SendRecurse(currentData, trigger);
        }

        public void Listen<TTrigger>(Action<TTrigger> callback) where TTrigger : struct, ITrigger
        {
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
            State state = EnsureStateIsRegistered<TState>();
            state.OnLifecycleEvent += actual => OnLifecycleEvent(expected, actual, state, callback);
        }


        StateData SendRecurse<TTrigger>(StateData data, TTrigger trigger) where TTrigger : struct, ITrigger
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

                if (!m_pool.States.TryGetValue(targetType, out StateData instance))
                {
                    throw new ArgumentException($"Type {targetType} is not registered");
                }

                Type intType = typeof(IGet<,>).MakeGenericType(typeof(TTrigger), targetType);
                MethodInfo method = stateType.GetInterfaceMap(intType).TargetMethods.FirstOrDefault(m => m.Name.Contains("Get"));
                method?.Invoke(data.State, new object[] { trigger });

                return instance;
            }

            return SendRecurse(data.Parent, trigger);
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

        void OnSend(ITrigger output)
        {
            if (m_callbacks.TryGetValue(output.GetType(), out Delegate del))
            {
                del.DynamicInvoke(output);
            }
        }

        void OnLifecycleEvent<TState>(When expected, When actual, State state, Action<TState> callback) where TState : State, new()
        {
            if (expected == actual)
            {
                callback?.Invoke(state as TState);
            }
        }

        State EnsureStateIsRegistered<TState>() where TState : State, new()
        {
            if (!m_pool.States.TryGetValue(typeof(TState), out StateData data))
            {
                throw new ArgumentException($"State {typeof(TState).FullName} is not registered");
            }
            return data.State;
        }
    }
}