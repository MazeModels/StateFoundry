using System;

namespace Maze.StateFoundry.Editor
{
    sealed class BlockAnalyzer
    {
        readonly InitialStateFinder m_initial;
        readonly CaptionFinder m_caption;
        readonly IStateGraphFactory m_graphFactory;

        Type m_initialState;
        IStateGraph m_tree;

        public BlockAnalyzer(InitialStateFinder initial, CaptionFinder caption, IStateGraphFactory graphFactory)
        {
            m_initial = initial;
            m_caption = caption;
            m_graphFactory = graphFactory;
        }

        public void BuildTree()
        {
            m_initialState = m_initial.FindInitialState();
            m_tree = m_graphFactory.Build(m_initialState);
            m_tree = m_caption.FindCaptions(m_tree);
        }

        public Type GetInitialState()
        {
            return m_initialState;
        }

        public IStateGraph GetTree()
        {
            return m_tree;
        }
    }
}