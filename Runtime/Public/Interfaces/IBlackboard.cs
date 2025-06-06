namespace Maze.StateFoundry
{
    public interface IBlackboard
    {
        void Add<T>(T component);
        T Get<T>();
    }
}