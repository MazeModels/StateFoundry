using System;

namespace Maze.StateFoundry.Editor
{
    sealed class GraphGeneration : IDisposable
    {
        readonly GraphBlock m_block;
        readonly BlockFinder m_finder;
        readonly BlockAnalyzer m_analizer;
        readonly TextGenerator m_textGenerator;
        readonly UmlPrinter m_printer;


        public GraphGeneration(GraphBlock block,
                               BlockFinder finder,
                               BlockAnalyzer analizer,
                               TextGenerator textGenerator,
                               UmlPrinter printer)
        {
            m_block = block;
            m_finder = finder;
            m_analizer = analizer;
            m_textGenerator = textGenerator;
            m_printer = printer;

            m_block.Listen<CheckIfBlock>(OnCheckIfBlock);
            m_block.Listen<AnalyzeHierarchy>(OnAnalyzeHierarchy);
            m_block.Listen<GenerateText>(OnGenerateText);
            m_block.Listen<PrintUml>(OnPrintUml);

            ScriptWatcher.OnChange += OnScriptChanged;
            ScriptWatcher.OnDeletion += OnScriptDeleted;
        }

        public void Dispose()
        {
            ScriptWatcher.OnChange -= OnScriptChanged;
            ScriptWatcher.OnDeletion -= OnScriptDeleted;
            m_block.Dispose();
        }

        void OnScriptChanged(string fileFullPath)
        {
            m_finder.SetPath(fileFullPath);
            m_block.Send(new ScriptImported());
        }

        void OnScriptDeleted(string fileFullPath)
        {
            m_block.Send(new ScriptDeleted());
        }

        void OnCheckIfBlock(CheckIfBlock ev)
        {
            m_finder.FindBlocks();

            if (m_finder.BlockCount() > 1)
            {
                m_block.Send(new ErrorRaised());
            }

            if (m_finder.BlockCount() == 0)
            {
                m_block.Send(new ScriptIsNotBlock());
                return;
            }

            m_block.Send(new ScriptIsBlock());
        }

        void OnAnalyzeHierarchy(AnalyzeHierarchy ev)
        {
            m_analizer.BuildTree();
            m_block.Send(new HierarchyAnalyzed());
        }

        void OnGenerateText(GenerateText ev)
        {
            m_textGenerator.GenerateText();

            if (string.IsNullOrEmpty(m_textGenerator.GetText()))
            {
                m_block.Send(new ErrorRaised());
                return;
            }

            m_block.Send(new TextGenerated());
        }

        void OnPrintUml(PrintUml ev)
        {
            m_printer.Print();
            m_block.Send(new UmlPrinted());
        }
    }
}