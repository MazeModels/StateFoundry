using System;

namespace Maze.StateFoundry
{
    interface IStateGraphFactory
    {
        IStateGraph Build(Type type);
    }
}
