using System;
using UnityEngine;

namespace Maze.StateFoundry
{
    public abstract class State : IDisposable
    {
        internal event Action<ITrigger> OnEventSent;
        internal event Action<When> OnLifecycleEvent;

        public State()
        {
            InternalOnCreate();
        }

        public void Dispose()
        {
            InternalOnDispose();
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

        internal void InternalOnEnter()
        {
            LogSpecialMethod(nameof(OnEnter));
            OnEnter();
            OnLifecycleEvent?.Invoke(When.OnEnter);
        }

        internal void InternalOnExit()
        {
            LogSpecialMethod(nameof(OnExit));
            OnExit();
            OnLifecycleEvent?.Invoke(When.OnExit);
        }

        internal void InternalOnCreate()
        {
            LogSpecialMethod(nameof(OnCreate));
            OnCreate();
            OnLifecycleEvent?.Invoke(When.OnCreate);
        }

        internal void InternalOnDispose()
        {
            LogSpecialMethod(nameof(OnDispose));
            OnDispose();
            OnLifecycleEvent?.Invoke(When.OnDispose);
        }

        void LogSpecialMethod(string methodName)
        {
            Debug.Log($"{ToString()}: {methodName}");
        }
    }
}