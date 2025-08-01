using System.Collections.Generic;

namespace Maze.StateFoundry
{
    interface IStatechartNavigator
    {
        IStateData FindCommonAncestor(IStateData first, IStateData second);
        IEnumerable<IStateData> FindPathToAncestor(IStateData data, IStateData ancestor);
        IEnumerable<IStateData> FindPathFromAncestor(IStateData data, IStateData ancestor);
        IEnumerable<IStateData> FindPathFromRoot(IStateData data);
        IEnumerable<IStateData> FindPathToRoot(IStateData data);
    }
}
