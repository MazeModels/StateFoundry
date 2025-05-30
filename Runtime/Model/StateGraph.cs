using System;
using System.Collections.Generic;
using System.Linq;

namespace Maze.StateFoundry
{
    sealed class StateGraph
    {
        public IReadOnlyDictionary<Type, StateMeta> States => m_states;
        readonly Dictionary<Type, StateMeta> m_states;


        public StateGraph(Type initialState)
        {
            EnsureConcreteState(initialState);
            m_states = new Dictionary<Type, StateMeta>();
            BuildMetaGraph(initialState);
        }


        void BuildMetaGraph(Type currentType)
        {
            BuildMetaGraphRecursive(currentType, m_states);
        }

        static void BuildMetaGraphRecursive(Type currentType, Dictionary<Type, StateMeta> visited)
        {
            if (visited.ContainsKey(currentType))
            {
                return;
            }

            StateMeta current = BuildMeta(currentType, visited);
            BuildAncestors(visited, currentType.BaseType, current);
            BuildConnections(currentType, visited);
        }

        static StateMeta BuildMeta(Type currentType, Dictionary<Type, StateMeta> visited)
        {
            var currentData = new StateMeta(currentType);
            visited[currentType] = currentData;
            return currentData;
        }

        static void BuildAncestors(Dictionary<Type, StateMeta> visited, Type baseType, StateMeta currentMeta)
        {
            if (baseType == null || baseType == typeof(State))
            {
                return;
            }

            BuildMetaGraphRecursive(baseType, visited);
            StateMeta baseMeta = visited[baseType];
            currentMeta.SetParent(baseMeta);
            baseMeta.AddChild(currentMeta);
        }

        static void BuildConnections(Type current, Dictionary<Type, StateMeta> visited)
        {
            List<Type> lineage = GetInheritanceChain(current).Reverse().ToList();
            BuildDirectTransitions(visited, lineage);
            BuildTransitions(visited, lineage);
        }

        static void BuildTransitions(Dictionary<Type, StateMeta> visited, List<Type> lineage)
        {
            var transitions = new Dictionary<Type, StateMeta>();
            foreach (Type state in lineage)
            {
                StateMeta meta = visited[state];
                foreach (KeyValuePair<Type, StateMeta> kvp in meta.DirectTransition)
                {
                    transitions[kvp.Key] = kvp.Value;
                }

                foreach (KeyValuePair<Type, StateMeta> kvp in transitions)
                {
                    visited[state].AddTransition(kvp.Key, kvp.Value);
                }
            }
        }

        static void BuildDirectTransitions(Dictionary<Type, StateMeta> visited, List<Type> lineage)
        {
            foreach (Type state in lineage)
            {
                IEnumerable<Type> getInterfaces = GetDirectIGetInterfaces(state);

                foreach (Type iface in getInterfaces)
                {
                    Type trigger = iface.GetGenericArguments()[0];
                    Type destination = iface.GetGenericArguments()[1];
                    BuildMetaGraphRecursive(destination, visited);
                    visited[state].AddDirectTransition(trigger, visited[destination]);
                }
            }
        }

        static IEnumerable<Type> GetInheritanceChain(Type type)
        {
            Type current = type;
            while (current != null && current != typeof(State))
            {
                yield return current;
                current = current.BaseType;
            }
        }

        static IEnumerable<Type> GetDirectIGetInterfaces(Type type)
        {
            IEnumerable<Type> all = type.GetInterfaces().Where(ImplementsIGet);
            IEnumerable<Type> inherited = type.BaseType?.GetInterfaces().Where(ImplementsIGet) ?? Type.EmptyTypes;
            return all.Except(inherited);
        }

        static bool ImplementsIGet(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IGet<,>);
        }

        static void EnsureConcreteState(Type type)
        {
            if (!typeof(State).IsAssignableFrom(type))
            {
                throw new ArgumentException($"{type.FullName} must inherit from {nameof(State)}.");
            }

            if (type.IsAbstract)
            {
                throw new ArgumentException($"{type.FullName} cannot be abstract.");
            }
        }
    }
}