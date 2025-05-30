using System;

namespace Maze.StateFoundry.Editor
{
    sealed class BlockAnalyzer
    {
        readonly InitialStateFinder m_initial;
        readonly CaptionFinder m_caption;

        Type m_initialState;
        StateGraph m_tree;

        public BlockAnalyzer(InitialStateFinder initial, CaptionFinder caption)
        {
            m_initial = initial;
            m_caption = caption;
        }

        public void BuildTree()
        {
            m_initialState = m_initial.FindInitialState();
            m_tree = new StateGraph(m_initialState);
            m_tree = m_caption.FindCaptions(m_tree);
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