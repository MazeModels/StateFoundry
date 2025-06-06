using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Maze.StateFoundry
{
    sealed class StatePool<TInitialState> : IDisposable where TInitialState : State, new()
    {
        public StateData CurrentData { get; private set; }

        public IReadOnlyDictionary<Type, StateData> States => m_states;

        readonly Dictionary<Type, StateData> m_states;
        readonly StatechartNavigator m_nav;


        public StatePool(IBlackboard blackboard)
        {
            m_states = InstantiateStates(blackboard);
            m_nav = new StatechartNavigator();
            SetCurrentState(m_states[typeof(TInitialState)]);
        }

        public void Dispose()
        {
            foreach (StateData data in m_states.Values)
            {
                data.Dispose();
            }
        }

        public IReadOnlyDictionary<Type, StateData> GetStates()
        {
            return m_states;
        }

        public void SetCurrentState(StateData newData)
        {
            StateData oldData = CurrentData;
            CurrentData = newData;
            CallbackChain(newData, oldData);
        }


        static Dictionary<Type, StateData> InstantiateStates(IBlackboard blackboard)
        {
            var metaGraph = new StateGraph(typeof(TInitialState));
            EnsureOnlyParameterlessConstructor(metaGraph);

            Dictionary<Type, StateData> dataGraph = InstantiateStates(metaGraph, blackboard);
            SetupGraphConnections(metaGraph, dataGraph);

            return dataGraph;
        }

        static void EnsureOnlyParameterlessConstructor(StateGraph metaGraph)
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

        static Dictionary<Type, StateData> InstantiateStates(StateGraph metaGraph, IBlackboard blackboard)
        {
            return metaGraph.States.ToDictionary(pair => pair.Key, pair => new StateData(pair.Value, blackboard));
        }

        static void SetupGraphConnections(StateGraph metaGraph, Dictionary<Type, StateData> dataGraph)
        {
            foreach ((Type type, StateData stateData) in dataGraph)
            {
                StateMeta meta = metaGraph.States[type];
                SetupParent(stateData, dataGraph, meta);
                SetupChildren(stateData, dataGraph, meta);
                SetupTransitions(stateData, dataGraph, meta);
            }
        }

        static void SetupParent(StateData stateData, Dictionary<Type, StateData> dataGraph, StateMeta meta)
        {
            if (meta.Parent == null)
            {
                return;
            }

            StateMeta parentMeta = meta.Parent;
            stateData.SetParent(dataGraph[parentMeta.Type]);
        }

        static void SetupChildren(StateData stateData, Dictionary<Type, StateData> dataGraph, StateMeta meta)
        {
            foreach (StateMeta childMeta in meta.Children)
            {
                stateData.AddChild(dataGraph[childMeta.Type]);
            }
        }

        static void SetupTransitions(StateData stateData, Dictionary<Type, StateData> dataGraph, StateMeta meta)
        {
            foreach (KeyValuePair<Type, StateMeta> pair in meta.Transitions)
            {
                stateData.AddTransition(pair.Key, dataGraph[pair.Value.Type]);
            }
        }


        void CallbackChain(StateData newData, StateData oldData)
        {
            if (oldData == null)
            {
                StartingFromRoot(newData);
                return;
            }

            StateData ancestor = m_nav.FindCommonAncestor(newData, oldData);
            foreach (StateData data in m_nav.FindPathToAncestor(oldData, ancestor))
            {
                ((IInternalState) data.State).InternalOnExit();
            }

            foreach (StateData data in m_nav.FindPathFromAncestor(newData, ancestor))
            {
                ((IInternalState) data.State).InternalOnEnter();
            }
        }

        void StartingFromRoot(StateData newData)
        {
            foreach (StateData data in m_nav.FindPathFromRoot(newData))
            {
                ((IInternalState) data.State).InternalOnEnter();
            }
        }
    }
}