using System;
using System.Collections.Generic;

namespace Maze.StateFoundry
{
    sealed class StateMeta
    {
        public IReadOnlyList<StateMeta> Children => m_children;
        public IReadOnlyDictionary<Type, StateMeta> Transitions => m_transitions;
        public IReadOnlyDictionary<Type, StateMeta> DirectTransition => m_directTransition;
        public IReadOnlyList<string> Captions => m_captions;
        public StateMeta Parent { get; private set; }

        public readonly Type Type;

        readonly List<StateMeta> m_children;
        readonly Dictionary<Type, StateMeta> m_transitions;
        readonly Dictionary<Type, StateMeta> m_directTransition;
        readonly List<string> m_captions;


        public StateMeta(Type type)
        {
            Type = type;
            m_children = new List<StateMeta>();
            m_transitions = new Dictionary<Type, StateMeta>();
            m_directTransition = new Dictionary<Type, StateMeta>();
            m_captions = new List<string>();
        }

        public void SetParent(StateMeta parent)
        {
            Parent = parent;
        }

        public void AddChild(StateMeta child)
        {
            m_children.Add(child);
        }

        public void AddTransition(Type ev, StateMeta destination)
        {
            m_transitions[ev] = destination;
        }

        public void AddDirectTransition(Type ev, StateMeta destination)
        {
            m_directTransition[ev] = destination;
        }

        public void AddCaption(string note)
        {
            m_captions.Add(note);
        }

        public override string ToString()
        {
            return Type.Name;
        }
    }
}