using System;
using System.Collections.Generic;

namespace Maze.StateFoundry
{
    sealed class StatechartBlackboard : IBlackboard
    {
        Dictionary<Type, object> m_data;

        public StatechartBlackboard()
        {
            m_data = new Dictionary<Type, object>();
        }

        public void Add<T>(T component)
        {
            m_data[typeof(T)] = component;
        }

        public T Get<T>()
        {
            if (!m_data.ContainsKey(typeof(T)))
            {
                return default;
            }

            return (T) m_data[typeof(T)];
        }
    }
}