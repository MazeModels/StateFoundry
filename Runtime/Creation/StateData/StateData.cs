using System;
using System.Collections.Generic;

namespace Maze.StateFoundry
{
    sealed class StateData : IStateData
    {
        public Type Type => m_type;
        public IInternalState State => m_state;
        public IReadOnlyList<IStateData> Children => m_children;
        public IReadOnlyDictionary<Type, IStateData> Transitions => m_transitions;
        public IStateData Parent { get; private set; }


        readonly Type m_type;
        readonly IInternalState m_state;
        readonly List<IStateData> m_children;
        readonly Dictionary<Type, IStateData> m_transitions;


        public StateData(IStateMeta meta, IBlackboard blackboard, IStateFactory factory)
        {
            m_type = meta.Type;
            m_state = factory.Build(Type);

            m_children = new List<IStateData>();
            m_transitions = new Dictionary<Type, IStateData>();
            State.Blackboard = blackboard;
        }

        public void Dispose()
        {
            State.Dispose();
        }

        public void SetParent(IStateData parent)
        {
            if (parent == null)
            {
                throw new ArgumentNullException(nameof(parent));
            }

            Parent = parent;
        }

        public void AddTransition(Type trigger, IStateData destination)
        {
            m_transitions[trigger] = destination;
        }

        public void AddChild(IStateData child)
        {
            if (child == null)
            {
                throw new ArgumentNullException(nameof(child));
            }

            m_children.Add(child);
        }

        public override string ToString()
        {
            return State.ToString();
        }
    }
}