using System;
using System.Collections.Generic;

namespace Maze.StateFoundry
{
    sealed class StateData : IDisposable
    {
        public IReadOnlyList<StateData> Children => m_children;
        public IReadOnlyDictionary<Type, StateData> Transitions => m_transitions;
        public StateData Parent { get; private set; }

        public readonly Type Type;
        public readonly State State;

        readonly List<StateData> m_children;
        readonly Dictionary<Type, StateData> m_transitions;


        public StateData(StateMeta meta)
        {
            Type = meta.Type;
            m_children = new List<StateData>();
            m_transitions = new Dictionary<Type, StateData>();
            State = (State) Activator.CreateInstance(Type);
        }

        public void Dispose()
        {
            State.Dispose();
        }

        public void SetParent(StateData parent)
        {
            Parent = parent;
        }

        public void AddTransition(Type trigger, StateData destination)
        {
            m_transitions[trigger] = destination;
        }

        public void AddChild(StateData child)
        {
            m_children.Add(child);
        }

        public override string ToString()
        {
            return State.ToString();
        }
    }
}