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

        public StateData Send<TEvent>(TEvent ev) where TEvent : struct, IEvent
        {
            StateData currentData = m_pool.CurrentData;
            return SendRecurse(currentData, ev);
        }

        public void Listen<T>(Action<T> callback)
        {
            Type type = typeof(T);
            if (m_callbacks.TryGetValue(type, out Delegate existing))
            {
                m_callbacks[type] = Delegate.Combine(existing, callback);
            }
            else
            {
                m_callbacks[type] = callback;
            }
        }

        StateData SendRecurse<TEvent>(StateData data, TEvent ev) where TEvent : struct, IEvent
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
                Type evType = genericArgs[0];
                Type targetType = genericArgs[1];

                if (evType != typeof(TEvent))
                {
                    continue;
                }

                if (!m_pool.States.TryGetValue(targetType, out StateData instance))
                {
                    throw new ArgumentException($"Type {targetType} is not registered");
                }

                Type intType = typeof(IGet<,>).MakeGenericType(typeof(TEvent), targetType);
                MethodInfo method = stateType.GetInterfaceMap(intType).TargetMethods.FirstOrDefault(m => m.Name.Contains("Get"));
                method?.Invoke(data.State, new object[] { ev });

                return instance;
            }

            return SendRecurse(data.Parent, ev);
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

        void OnSend(IEvent output)
        {
            if (m_callbacks.TryGetValue(output.GetType(), out Delegate del))
            {
                del.DynamicInvoke(output);
            }
        }
    }
}