using System;
using System.Collections.Generic;

namespace Maze.StateFoundry
{
    interface IStatePool<TInitialState> : IDisposable where TInitialState : State, new()
    {
        IReadOnlyDictionary<Type, IStateData> GetStates();
        
        void SetCurrentState(IStateData newData);
        IStateData GetCurrentState();
    }
}