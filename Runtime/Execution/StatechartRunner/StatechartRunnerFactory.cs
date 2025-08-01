using System;

namespace Maze.StateFoundry
{
    static class StatechartRunnerFactory
    {
        public static IStatechartRunner Build<TInitialState>(Type statechartType) where TInitialState : State, new()
        {
            EnsureTypeIsNotNull(statechartType);

            var blackboard = new StatechartBlackboard();
            var pool = new StatePool<TInitialState>(blackboard, new StatechartNavigator(), new StateGraphFactory(), new StateFactory(), new StateDataFactory());
            var events = new StatechartEvents<TInitialState>(pool);

            return new StatechartRunner<TInitialState>(statechartType, blackboard, pool, events);
        }

        static void EnsureTypeIsNotNull(Type statechartType)
        {
            if (statechartType == null)
            {
                throw new ArgumentNullException(nameof(statechartType));
            }
        }
    }
}
