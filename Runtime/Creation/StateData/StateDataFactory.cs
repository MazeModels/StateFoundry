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

        static void EnsureArgumentsAreNotNull(IStateMeta meta, IBlackboard blackboard, IStateFactory stateFactory)
        {
            if (meta == null)
            {
                throw new ArgumentNullException(nameof(meta));
            }

            if (blackboard == null)
            {
                throw new ArgumentNullException(nameof(blackboard));
            }

            if (stateFactory == null)
            {
                throw new ArgumentNullException(nameof(stateFactory));
            }
        }
    }
}
