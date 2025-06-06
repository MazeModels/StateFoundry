using System;

namespace Maze.StateFoundry
{
    public abstract class State : IInternalState, IDisposable
    {
        internal event Action<ITrigger> OnEventSent;
        internal event Action<When> OnLifecycleEvent;

        IBlackboard IInternalState.Blackboard { get; set; }


        public State()
        {
            ((IInternalState) this).InternalOnCreate();
        }

        public void Dispose()
        {
            ((IInternalState) this).InternalOnDispose();
        }

        public virtual void OnEnter()
        {
            ;
        }

        public virtual void OnExit()
        {
            ;
        }

        public virtual void OnCreate()
        {
            ;
        }

        public virtual void OnDispose()
        {
            ;
        }

        public override string ToString()
        {
            return GetType().Name;
        }

        public void Send<TTrigger>(TTrigger trigger) where TTrigger : struct, ITrigger
        {
            OnEventSent?.Invoke(trigger);
        }
        
        public void Add<T>(T component)
        {
            ((IInternalState) this).Blackboard.Add(component);
        }

        public T Get<T>()
        {
            return ((IInternalState) this).Blackboard.Get<T>();
        }

        void IInternalState.InternalOnEnter()
        {
            LogSpecialMethod(nameof(OnEnter));
            OnEnter();
            OnLifecycleEvent?.Invoke(When.OnEnter);
        }

        void IInternalState.InternalOnExit()
        {
            LogSpecialMethod(nameof(OnExit));
            OnExit();
            OnLifecycleEvent?.Invoke(When.OnExit);
        }

        void IInternalState.InternalOnCreate()
        {
            LogSpecialMethod(nameof(OnCreate));
            OnCreate();
            OnLifecycleEvent?.Invoke(When.OnCreate);
        }

        void IInternalState.InternalOnDispose()
        {
            LogSpecialMethod(nameof(OnDispose));
            OnDispose();
            OnLifecycleEvent?.Invoke(When.OnDispose);
        }

        void LogSpecialMethod(string methodName)
        {
            // TODO: Implement verbosity toggle
            // Debug.Log($"{ToString()}: {methodName}");
        }
    }
}