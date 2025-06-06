namespace Maze.StateFoundry
{
    interface IInternalStatechart : IStatechart, IBlackboard
    {
        internal IStatechartRunner Runner { get; } 
    }
}