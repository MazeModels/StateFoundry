namespace Maze.StateFoundry
{
    public interface IGet<in TEvent, TState> where TEvent : struct, IEvent where TState : State, new()
    {
        void Get(TEvent ev)
        {
        }
    }
}