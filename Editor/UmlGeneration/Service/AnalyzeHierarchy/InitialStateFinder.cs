using System;
using System.Collections.Generic;
using System.Linq;

namespace Maze.StateFoundry.Editor
{
    sealed class InitialStateFinder
    {
        readonly BlockFinder m_finder;

        public InitialStateFinder(BlockFinder finder)
        {
            m_finder = finder;
        }

        public Type FindInitialState()
        {
            Type blockType = m_finder.GetBlocks()[0];
            return FindStatechartStartingState(blockType);
        }

        static Type FindStatechartStartingState(Type blockType)
        {
            Type statechartBase = GetTypeHierarchy(blockType).FirstOrDefault(IsBaseStatechartType);

            if (statechartBase == null)
            {
                throw new ArgumentException($"The block type '{blockType.FullName}' does not inherit from Statechart<>.");
            }

            return statechartBase.GetGenericArguments()[0];
        }

        static IEnumerable<Type> GetTypeHierarchy(Type type)
        {
            for (Type current = type; IsValidType(current); current = current?.BaseType)
            {
                yield return current;
            }
        }

        static bool IsValidType(Type current)
        {
            return current != null && current != typeof(object);
        }

        static bool IsBaseStatechartType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Statechart<>);
        }
    }
}