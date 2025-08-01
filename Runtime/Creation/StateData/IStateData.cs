using System;
using System.Collections.Generic;

namespace Maze.StateFoundry
{
    interface IStateData : IDisposable
    {
        Type Type { get; }
        IInternalState State { get; }
        IReadOnlyList<IStateData> Children { get; }
        IReadOnlyDictionary<Type, IStateData> Transitions { get; }
        IStateData Parent { get; }

        void SetParent(IStateData parent);
        void AddTransition(Type trigger, IStateData destination);
        void AddChild(IStateData child);
    }
}
