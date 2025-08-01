using System;

namespace Maze.StateFoundry
{
    sealed class StateDataFactory : IStateDataFactory
    {
        public IStateData Build(IStateMeta meta, IBlackboard blackboard, IStateFactory stateFactory)
        {
            EnsureArgumentsAreNotNull(meta, blackboard, stateFactory);
            return new StateData(meta, blackboard, stateFactory);
        }

        void EnsureArgumentsAreNotNull(params object[] args)
        {
            foreach (var arg in args)
            {
                if (arg == null)
                {
                    throw new ArgumentNullException();
                }
            }
        }
    }
}
