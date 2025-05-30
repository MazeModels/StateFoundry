using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Maze.StateFoundry.Editor
{
    sealed class Container : IDisposable
    {
        readonly Dictionary<Type, Type> m_registrations = new();
        readonly Dictionary<Type, object> m_instances = new();
        bool m_disposed;

        public void Dispose()
        {
            if (m_disposed)
            {
                return;
            }

            DisposeAllCreatedInstances();
            m_instances.Clear();
            m_disposed = true;
        }

        public void Register<TService, TImplementation>() where TImplementation : TService
        {
            m_registrations[typeof(TService)] = typeof(TImplementation);
        }

        public T Get<T>()
        {
            return (T) Resolve(typeof(T));
        }

        object Resolve(Type type)
        {
            if (TryGetExistingInstance(type, out object instance))
            {
                return instance;
            }

            Type implementationType = GetImplementationType(type);
            ValidateImplementationType(implementationType);

            object[] parameters = GetConstructorParameters(implementationType);
            instance = CreateInstance(implementationType, parameters);

            m_instances[type] = instance;
            return instance;
        }

        bool TryGetExistingInstance(Type type, out object instance)
        {
            return m_instances.TryGetValue(type, out instance);
        }

        Type GetImplementationType(Type serviceType)
        {
            return m_registrations.GetValueOrDefault(serviceType, serviceType);
        }

        static void ValidateImplementationType(Type type)
        {
            if (type.IsAbstract || type.IsInterface)
            {
                throw new InvalidOperationException($"Cannot instantiate abstract/interface type: {type.FullName}");
            }
        }

        object[] GetConstructorParameters(Type type)
        {
            ConstructorInfo ctor = type.GetConstructors().First();
            return ctor.GetParameters().Select(p => Resolve(p.ParameterType)).ToArray();
        }

        static object CreateInstance(Type type, object[] parameters)
        {
            return Activator.CreateInstance(type, parameters)!;
        }

        void DisposeAllCreatedInstances()
        {
            foreach (IDisposable instance in m_instances.Values.OfType<IDisposable>())
            {
                instance.Dispose();
            }
        }
    }
}