using System;
using UnityEngine;

namespace Maze.StateFoundry
{
    public abstract class State : IDisposable
    {
        internal event Action<ITrigger> OnEventSent;

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

        public void Send<TEvent>(TEvent ev) where TEvent : struct, ITrigger
        {
            OnEventSent?.Invoke(ev);
        }

        internal void InternalOnEnter()
        {
            LogSpecialMethod(nameof(OnEnter));
            OnEnter();
        }

        internal void InternalOnExit()
        {
            LogSpecialMethod(nameof(OnExit));
            OnExit();
        }

        internal void InternalOnCreate()
        {
            LogSpecialMethod(nameof(OnCreate));
            OnCreate();
        }

        internal void InternalOnDispose()
        {
            LogSpecialMethod(nameof(OnDispose));
            OnDispose();
        }

        void LogSpecialMethod(string methodName)
        {
            Debug.Log($"{ToString()}: {methodName}");
        }
    }
}