using System;

namespace Maze.StateFoundry
{
    interface IStatechartRunner : IStatechart, IBlackboard, IDisposable
    {
        event Action<ITrigger> OnTrigger;
    }
}