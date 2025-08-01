using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Maze.StateFoundry
{
    sealed class StatePool<TInitialState> : IStatePool<TInitialState> where TInitialState : State, new()
    {
        readonly Dictionary<Type, IStateData> m_states;
        readonly IStatechartNavigator m_nav;
        readonly IStateDataFactory m_stateDataFactory;
        
        IStateData m_currentData;

        public StatePool(IBlackboard blackboard, IStatechartNavigator navigator, IStateGraphFactory graphFactory, IStateFactory stateFactory, IStateDataFactory stateDataFactory)
        {
            EnsureArgumentsAreNotNull(blackboard, navigator, graphFactory, stateFactory, stateDataFactory);

            m_states = InstantiateStates(blackboard, graphFactory, stateFactory, stateDataFactory);
            m_nav = navigator;
            m_stateDataFactory = stateDataFactory;
            SetCurrentState(m_states[typeof(TInitialState)]);
        }

        public void Dispose()
        {
            foreach (IStateData data in m_states.Values)
            {
                data.Dispose();
            }
        }

        public IReadOnlyDictionary<Type, IStateData> GetStates()
        {
            return m_states;
        }

        public void SetCurrentState(IStateData newData)
        {
            IStateData oldData = m_currentData;
            m_currentData = newData;
            CallbackChain(newData, oldData);
        }

        public IStateData GetCurrentState()
        {
            return m_currentData;
        }


        static Dictionary<Type, IStateData> InstantiateStates(IBlackboard blackboard, IStateGraphFactory graphFactory, IStateFactory stateFactory, IStateDataFactory stateDataFactory)
        {
            var metaGraph = graphFactory.Build(typeof(TInitialState));
            EnsureOnlyParameterlessConstructor(metaGraph);

            Dictionary<Type, IStateData> dataGraph = InstantiateStates(metaGraph, blackboard, stateFactory, stateDataFactory);
            SetupGraphConnections(metaGraph, dataGraph);

            return dataGraph;
        }

        static void EnsureOnlyParameterlessConstructor(IStateGraph metaGraph)
        {
            foreach (Type stateType in metaGraph.States.Keys)
            {
                ConstructorInfo[] constructors = stateType.GetConstructors();
                if (IsParameterlessConstructor(constructors))
                {
                    continue;
                }

                throw new InvalidOperationException($"Only parameterless constructor supported: {stateType.FullName}");
            }
        }

        static bool IsParameterlessConstructor(ConstructorInfo[] constructors)
        {
            return constructors.Length == 1 && constructors[0].GetParameters().Length == 0;
        }

        static Dictionary<Type, IStateData> InstantiateStates(IStateGraph metaGraph, IBlackboard blackboard, IStateFactory stateFactory, IStateDataFactory stateDataFactory)
        {
            return metaGraph.States.ToDictionary(pair => pair.Key, pair => stateDataFactory.Build(pair.Value, blackboard, stateFactory));
        }

        static void SetupGraphConnections(IStateGraph metaGraph, Dictionary<Type, IStateData> dataGraph)
        {
            foreach ((Type type, IStateData stateData) in dataGraph)
            {
                IStateMeta meta = metaGraph.States[type];
                SetupParent(stateData, dataGraph, meta);
                SetupChildren(stateData, dataGraph, meta);
                SetupTransitions(stateData, dataGraph, meta);
            }
        }

        static void SetupParent(IStateData stateData, Dictionary<Type, IStateData> dataGraph, IStateMeta meta)
        {
            if (meta.Parent == null)
            {
                return;
            }

            IStateMeta parentMeta = meta.Parent;
            stateData.SetParent(dataGraph[parentMeta.Type]);
        }

        static void SetupChildren(IStateData stateData, Dictionary<Type, IStateData> dataGraph, IStateMeta meta)
        {
            foreach (IStateMeta childMeta in meta.Children)
            {
                stateData.AddChild(dataGraph[childMeta.Type]);
            }
        }

        static void SetupTransitions(IStateData stateData, Dictionary<Type, IStateData> dataGraph, IStateMeta meta)
        {
            foreach (KeyValuePair<Type, IStateMeta> pair in meta.Transitions)
            {
                stateData.AddTransition(pair.Key, dataGraph[pair.Value.Type]);
            }
        }


        void CallbackChain(IStateData newData, IStateData oldData)
        {
            if (newData == oldData)
            {
                return;
            }

            if (oldData == null)
            {
                OnEnterFromRoot(newData);
                return;
            }

            IStateData ancestor = m_nav.FindCommonAncestor(newData, oldData);
            if (ancestor == null)
            {
                OnExitToRoot(oldData);
                OnEnterFromRoot(newData);
                return;
            }

            foreach (IStateData data in m_nav.FindPathToAncestor(oldData, ancestor))
            {
                data.State.InternalOnExit();
            }

            foreach (IStateData data in m_nav.FindPathFromAncestor(newData, ancestor))
            {
                data.State.InternalOnEnter();
            }
        }

        void OnEnterFromRoot(IStateData newData)
        {
            foreach (IStateData data in m_nav.FindPathFromRoot(newData))
            {
                data.State.InternalOnEnter();
            }
        }

        void OnExitToRoot(IStateData oldData)
        {
            foreach (IStateData data in m_nav.FindPathToRoot(oldData))
            {
                data.State.InternalOnExit();
            }
        }

        void EnsureArgumentsAreNotNull(params object[] arguments)
        {
            foreach (var arg in arguments)
            {
                if (arg == null)
                {
                    throw new ArgumentNullException();
                }
            }
        }
    }
}