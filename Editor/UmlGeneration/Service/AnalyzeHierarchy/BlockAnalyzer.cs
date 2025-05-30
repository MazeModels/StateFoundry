using System;

namespace Maze.StateFoundry.Editor
{
    sealed class BlockAnalyzer
    {
        readonly InitialStateFinder m_initial;
        readonly OutputFinder m_output;

        Type m_initialState;
        StateGraph m_tree;

        public BlockAnalyzer(InitialStateFinder initial, OutputFinder output)
        {
            m_initial = initial;
            m_output = output;
        }

        public void BuildTree()
        {
            m_initialState = m_initial.FindInitialState();
            m_tree = new StateGraph(m_initialState);
            m_tree = m_output.FindOutputs(m_tree);
        }

        public Type GetInitialState()
        {
            return m_initialState;
        }

        public StateGraph GetTree()
        {
            return m_tree;
        }
    }
}