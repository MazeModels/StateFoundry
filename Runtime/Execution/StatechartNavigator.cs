using System.Collections.Generic;
using System.Linq;

namespace Maze.StateFoundry
{
    sealed class StatechartNavigator
    {
        public StateData FindCommonAncestor(StateData first, StateData second)
        {
            var lineage = new HashSet<StateData>(GetStateLineage(first));
            return GetStateLineage(second).FirstOrDefault(state => lineage.Contains(state));
        }

        public IEnumerable<StateData> FindPathToAncestor(StateData data, StateData ancestor)
        {
            return GetStateLineage(data).TakeWhile(s => ancestor != s);
        }

        public IEnumerable<StateData> FindPathFromAncestor(StateData data, StateData ancestor)
        {
            return FindPathToAncestor(data, ancestor).Reverse();
        }

        public IEnumerable<StateData> FindPathFromRoot(StateData data)
        {
            return GetStateLineage(data).Reverse();
        }

        static IEnumerable<StateData> GetStateLineage(StateData data)
        {
            StateData current = data;
            while (current != null)
            {
                yield return current;
                current = current.Parent;
            }
        }
    }
}