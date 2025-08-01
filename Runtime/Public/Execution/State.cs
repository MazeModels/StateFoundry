using System;

namespace Maze.StateFoundry
{
    public abstract class State : IInternalState
    {
        event Action<ITrigger> OnInternalEventSent;
        event Action<When> OnInternalLifecycleEvent;

        event Action<ITrigger> IInternalState.OnEventSent
        {
            add => OnInternalEventSent += value;
            remove => OnInternalEventSent -= value;
        }

        event Action<When> IInternalState.OnLifecycleEvent
        {
            add => OnInternalLifecycleEvent += value;
            remove => OnInternalLifecycleEvent -= value;
        }

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
            OnInternalEventSent?.Invoke(trigger);
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
            OnInternalLifecycleEvent?.Invoke(When.OnEnter);
        }

        void IInternalState.InternalOnExit()
        {
            LogSpecialMethod(nameof(OnExit));
            OnExit();
            OnInternalLifecycleEvent?.Invoke(When.OnExit);
        }

        void IInternalState.InternalOnCreate()
        {
            LogSpecialMethod(nameof(OnCreate));
            OnCreate();
            OnInternalLifecycleEvent?.Invoke(When.OnCreate);
        }

        void IInternalState.InternalOnDispose()
        {
            LogSpecialMethod(nameof(OnDispose));
            OnDispose();
            OnInternalLifecycleEvent?.Invoke(When.OnDispose);
        }

        void LogSpecialMethod(string methodName)
        {
            // TODO: Implement verbosity toggle
            // Debug.Log($"{ToString()}: {methodName}");
        }
    }
}