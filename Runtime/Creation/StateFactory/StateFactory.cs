using System;

namespace Maze.StateFoundry
{
    sealed class StateFactory : IStateFactory
    {
        public IInternalState Build(Type stateType)
        {
            CheckPreconditions(stateType);
            return (State) Activator.CreateInstance(stateType);
        }

        static void CheckPreconditions(Type stateType)
        {
            EnsureIsNotNull(stateType);
            EnsureIsNotAbstract(stateType);
            EnsureIsStateType(stateType);
        }

        static void EnsureIsNotNull(Type stateType)
        {
            if (stateType == null)
            {
                throw new ArgumentNullException(nameof(stateType));
            }
        }

        static void EnsureIsNotAbstract(Type stateType)
        {
            if (stateType.IsAbstract)
            {
                throw new ArgumentException($"The provided type '{stateType.FullName}' must not be abstract.");
            }
        }

        static void EnsureIsStateType(Type stateType)
        {
            if (!typeof(State).IsAssignableFrom(stateType))
            {
                throw new ArgumentException($"The provided type '{stateType?.FullName}' must be assignable to '{typeof(State).FullName}'.");
            }
        }
    }
}