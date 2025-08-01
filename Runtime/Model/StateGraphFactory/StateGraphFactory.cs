using System;

namespace Maze.StateFoundry
{
    sealed class StateGraphFactory : IStateGraphFactory
    {
        public IStateGraph Build(Type type)
        {
            EnsureIsNotNull(type);
            EnsureConcreteState(type);
            EnsureIsAssignableFromState(type);

            return new StateGraph(type);
        }

        static void EnsureIsNotNull(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type), $"Type cannot be null.");
            }
        }

        static void EnsureConcreteState(Type type)
        {

            if (type.IsAbstract)
            {
                throw new ArgumentException($"{type.FullName} cannot be abstract.");
            }
        }

        static void EnsureIsAssignableFromState(Type type)
        {
            if (!typeof(State).IsAssignableFrom(type))
            {
                throw new ArgumentException($"{type.FullName} must inherit from {nameof(State)}.");
            }
        }
    }
}
