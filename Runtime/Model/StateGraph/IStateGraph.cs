using System;
using System.Collections.Generic;

namespace Maze.StateFoundry
{
    interface IStateGraph
    {
        public IReadOnlyDictionary<Type, IStateMeta> States {  get; }
    }
}
