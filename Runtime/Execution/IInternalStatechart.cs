namespace Maze.StateFoundry
{
    interface IInternalStatechart : IStatechart
    {
        internal IStatechartRunner Runner { get; } 
    }
}