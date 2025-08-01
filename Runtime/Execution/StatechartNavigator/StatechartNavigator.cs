using System;
using System.Collections.Generic;
using System.Linq;

namespace Maze.StateFoundry
{
    sealed class StatechartNavigator : IStatechartNavigator
    {
        public IStateData FindCommonAncestor(IStateData first, IStateData second)
        {
            EnsureStatesAreNotNull(first, second);
            var lineage = new HashSet<IStateData>(FindPathToRoot(first));
            return FindPathToRoot(second).FirstOrDefault(state => lineage.Contains(state));
        }

        public IEnumerable<IStateData> FindPathToAncestor(IStateData data, IStateData ancestor)
        {
            EnsureStatesAreNotNull(data, ancestor);
            return FindPathToRoot(data).TakeWhile(s => ancestor != s);
        }

        public IEnumerable<IStateData> FindPathFromAncestor(IStateData data, IStateData ancestor)
        {
            EnsureStatesAreNotNull(data, ancestor);
            return FindPathToAncestor(data, ancestor).Reverse();
        }

        public IEnumerable<IStateData> FindPathFromRoot(IStateData data)
        {
            EnsureStatesAreNotNull(data);
            return FindPathToRoot(data).Reverse();
        }

        public IEnumerable<IStateData> FindPathToRoot(IStateData data)
        {
            EnsureStatesAreNotNull(data);
            IStateData current = data;
            while (current != null)
            {
                yield return current;
                current = current.Parent;
            }
        }

        static void EnsureStatesAreNotNull(params IStateData[] states)
        {
            foreach (IStateData state in states)
            {
                if (state == null)
                {
                    throw new ArgumentNullException("Null input state");
                }
            }
        }
    }
}