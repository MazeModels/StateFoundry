namespace Maze.StateFoundry
{
    public interface IGet<in TTrigger, TNextState> where TTrigger : struct, ITrigger where TNextState : State, new()
    {
        void Get(TTrigger trigger)
        {
        }
    }
}