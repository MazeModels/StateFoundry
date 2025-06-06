using System;

namespace Maze.StateFoundry
{
    interface IStatechartRunner : IStatechart
    {
        event Action<ITrigger> OnTrigger;
    }
}