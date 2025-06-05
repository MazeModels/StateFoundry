using System;
using System.Collections.Generic;
using System.Reflection;

namespace Maze.StateFoundry
{
    sealed class StatechartWorld : IStatechart, IDisposable
    {
        readonly HashSet<IStatechart> m_charts;
        readonly Dictionary<ITriggerSink, Action<ITrigger>> m_callbacks;

        public StatechartWorld(IList<IStatechart> charts)
        {
            m_charts = new HashSet<IStatechart>(charts);
            m_callbacks = new Dictionary<ITriggerSink, Action<ITrigger>>();
            SubscribeToTriggers();
        }

        public void Dispose()
        {
            UnsubscribeToTriggers();
        }

        public void Send<TTrigger>(TTrigger trigger) where TTrigger : struct, ITrigger
        {
            foreach (IStatechart statechart in m_charts)
            {
                statechart.Send(trigger);
            }
        }

        public void Listen<TTrigger>(Action<TTrigger> callback) where TTrigger : struct, ITrigger
        {
            foreach (IStatechart statechart in m_charts)
            {
                statechart.Listen(callback);
            }
        }

        public void OnEnter<TState>(Action<TState> callback) where TState : State, new()
        {
            foreach (IStatechart statechart in m_charts)
            {
                statechart.OnEnter(callback);
            }
        }

        public void OnExit<TState>(Action<TState> callback) where TState : State, new()
        {
            foreach (IStatechart statechart in m_charts)
            {
                statechart.OnExit(callback);
            }
        }

        public void OnCreate<TState>(Action<TState> callback) where TState : State, new()
        {
            foreach (IStatechart statechart in m_charts)
            {
                statechart.OnCreate(callback);
            }
        }

        public void OnDispose<TState>(Action<TState> callback) where TState : State, new()
        {
            foreach (IStatechart statechart in m_charts)
            {
                statechart.OnDispose(callback);
            }
        }

        void SubscribeToTriggers()
        {
            foreach (IStatechart chart in m_charts)
            {
                ITriggerSink sink = GetSink(chart);
                m_callbacks[sink] = trigger => OnInternalTrigger(trigger, chart);
                sink.OnTrigger += m_callbacks[sink];
            }
        }

        void UnsubscribeToTriggers()
        {
            foreach (KeyValuePair<ITriggerSink, Action<ITrigger>> kvp in m_callbacks)
            {
                kvp.Key.OnTrigger -= kvp.Value;
            }
        }

        static ITriggerSink GetSink(IStatechart statechart)
        {
            Type type = statechart.GetType();
            FieldInfo runnerField = type.GetField("Runner", BindingFlags.NonPublic | BindingFlags.Instance);
            return runnerField?.GetValue(statechart) as ITriggerSink;
        }

        void OnInternalTrigger(ITrigger trigger, IStatechart chart)
        {
            foreach (IStatechart statechart in m_charts)
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