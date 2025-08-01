using System;

namespace Maze.StateFoundry
{
    interface IStateFactory
    {
        IInternalState Build(Type type);
    }
}