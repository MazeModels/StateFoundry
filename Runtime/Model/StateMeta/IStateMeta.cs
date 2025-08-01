using System;
using System.Collections.Generic;

namespace Maze.StateFoundry
{
    interface IStateMeta
    {
        Type Type { get; }
        IReadOnlyList<IStateMeta> Children { get; }
        IReadOnlyDictionary<Type, IStateMeta> Transitions { get; }
        IReadOnlyDictionary<Type, IStateMeta> DirectTransition { get; }
        IReadOnlyList<string> Captions { get; }
        IStateMeta Parent { get; }


        void SetParent(IStateMeta parent);
        void AddChild(IStateMeta child);
        void AddTransition(Type trigger, IStateMeta destination);
        void AddDirectTransition(Type trigger, IStateMeta destination);
        void AddCaption(string note);
    }
}
