namespace Maze.StateFoundry
{
    public interface IState : IBlackboard
    {
        void OnEnter();
        void OnExit();
        void OnCreate();
        void OnDispose();

        void Send<TTrigger>(TTrigger trigger) where TTrigger : struct, ITrigger;
    }
}