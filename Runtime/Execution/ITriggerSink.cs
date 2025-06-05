using System;

namespace Maze.StateFoundry
{
    interface ITriggerSink
    {
        event Action<ITrigger> OnTrigger;
    }
}