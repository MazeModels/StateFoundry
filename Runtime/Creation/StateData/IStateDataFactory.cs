namespace Maze.StateFoundry
{
    interface IStateDataFactory
    {
        IStateData Build(IStateMeta meta, IBlackboard blackboard, IStateFactory stateFactory);
    }
}
