using System;

namespace Maze.StateFoundry.Editor
{
    sealed class Installer : IDisposable
    {
        readonly Container m_container;
        GraphGeneration m_graphGeneration;

        public Installer()
        {
            m_container = new Container();
            RegisterAll();
        }

        public void Dispose()
        {
            m_container?.Dispose();
            m_graphGeneration?.Dispose();
        }

        public void Run(string filePath)
        {
            m_graphGeneration = m_container.Get<GraphGeneration>();
            m_graphGeneration.Draw(filePath);
        }

        void RegisterAll()
        {
            m_container.Register<GraphBlock, GraphBlock>();
            m_container.Register<BlockFinder, BlockFinder>();
            m_container.Register<BlockAnalyzer, BlockAnalyzer>();
            m_container.Register<TextGenerator, TextGenerator>();
            m_container.Register<InitialStateFinder, InitialStateFinder>();
            m_container.Register<CaptionFinder, CaptionFinder>();
            m_container.Register<UmlPrinter, UmlPrinter>();

            m_container.Register<GraphGeneration, GraphGeneration>();
        }
    }
}