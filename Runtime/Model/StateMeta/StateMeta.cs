using System;
using System.Collections.Generic;

namespace Maze.StateFoundry
{
    sealed class StateMeta : IStateMeta
    {
        public Type Type => m_type;
        public IReadOnlyList<IStateMeta> Children => m_children;
        public IReadOnlyDictionary<Type, IStateMeta> Transitions => m_transitions;
        public IReadOnlyDictionary<Type, IStateMeta> DirectTransitions => m_directTransitions;
        public IReadOnlyList<string> Captions => m_captions;
        public IStateMeta Parent { get; private set; }

        readonly Type m_type;
        readonly List<IStateMeta> m_children;
        readonly Dictionary<Type, IStateMeta> m_transitions;
        readonly Dictionary<Type, IStateMeta> m_directTransitions;
        readonly List<string> m_captions;


        public StateMeta(Type type)
        {
            m_type = type;
            m_children = new List<IStateMeta>();
            m_transitions = new Dictionary<Type, IStateMeta>();
            m_directTransitions = new Dictionary<Type, IStateMeta>();
            m_captions = new List<string>();
        }

        public void SetParent(IStateMeta parent)
        {
            if (parent == null)
            {
                throw new ArgumentNullException(nameof(parent));
            }

            Parent = parent;
        }

        public void AddChild(IStateMeta child)
        {
            if (child == null)
            {
                throw new ArgumentNullException(nameof(child));
            }

            m_children.Add(child);
        }

        public void AddTransition(Type trigger, IStateMeta destination)
        {
            if (trigger == null)
            {
                throw new ArgumentNullException(nameof(trigger));
            }

            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }


            m_transitions[trigger] = destination;
        }

        public void AddDirectTransition(Type trigger, IStateMeta destination)
        {
            if (trigger == null)
            {
                throw new ArgumentNullException(nameof(trigger));
            }

            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            m_directTransitions[trigger] = destination;
        }

        public void AddCaption(string note)
        {
            if (note == null)
            {
                throw new ArgumentNullException(nameof(note));
            }

            m_captions.Add(note);
        }

        public override string ToString()
        {
            return Type.Name;
        }
    }
}