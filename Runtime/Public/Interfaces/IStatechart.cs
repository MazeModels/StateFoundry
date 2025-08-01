namespace Maze.StateFoundry
{
    public interface IStatechart : IStatechartLifecycle, ITriggerChannel
    {
        void Start();
    }
}