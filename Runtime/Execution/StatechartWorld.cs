using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Maze.StateFoundry
{
    sealed class StatechartWorld : IStatechart, IDisposable
    {
        readonly HashSet<IInternalStatechart> m_statecharts;
        readonly Dictionary<IStatechartRunner, Action<ITrigger>> m_callbacks;

        public StatechartWorld(IList<IStatechart> charts)
        {
            m_statecharts = charts.Select(statechart => statechart as IInternalStatechart).ToHashSet();
            m_callbacks = new Dictionary<IStatechartRunner, Action<ITrigger>>();
            SubscribeToTriggers();
        }

        public void Dispose()
        {
            UnsubscribeToTriggers();
        }

        public void Send<TTrigger>(TTrigger trigger) where TTrigger : struct, ITrigger
        {
            foreach (IInternalStatechart statechart in m_statecharts)
            {
                statechart.Send(trigger);
            }
        }

        public void Listen<TTrigger>(Action<TTrigger> callback) where TTrigger : struct, ITrigger
        {
            foreach (IInternalStatechart statechart in m_statecharts)
            {
                statechart.Listen(callback);
            }
        }

        public void OnEnter<TState>(Action<TState> callback) where TState : State, new()
        {
            foreach (IInternalStatechart statechart in m_statecharts)
            {
                statechart.OnEnter(callback);
            }
        }

        public void OnExit<TState>(Action<TState> callback) where TState : State, new()
        {
            foreach (IInternalStatechart statechart in m_statecharts)
            {
                statechart.OnExit(callback);
            }
        }

        public void OnCreate<TState>(Action<TState> callback) where TState : State, new()
        {
            foreach (IInternalStatechart statechart in m_statecharts)
            {
                statechart.OnCreate(callback);
            }
        }

        public void OnDispose<TState>(Action<TState> callback) where TState : State, new()
        {
            foreach (IInternalStatechart statechart in m_statecharts)
            {
                statechart.OnDispose(callback);
            }
        }

        void SubscribeToTriggers()
        {
            foreach (IInternalStatechart statechart in m_statecharts)
            {
                IStatechartRunner runner = statechart.Runner;
                m_callbacks[runner] = trigger => OnInternalTrigger(trigger, statechart);
                runner.OnTrigger += m_callbacks[runner];
            }
        }

        void UnsubscribeToTriggers()
        {
            foreach (KeyValuePair<IStatechartRunner, Action<ITrigger>> kvp in m_callbacks)
            {
                kvp.Key.OnTrigger -= kvp.Value;
            }
        }

        void OnInternalTrigger(ITrigger trigger, IStatechart chart)
        {
            foreach (IInternalStatechart statechart in m_statecharts)
            {
                if (statechart == chart)
                {
                    continue;
                }

                MethodInfo method = statechart.GetType().GetMethod(nameof(Send));
                MethodInfo genericMethod = method?.MakeGenericMethod(trigger.GetType());
                genericMethod?.Invoke(statechart, new object[] { trigger });
            }
        }
    }
}