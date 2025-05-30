using System;
using System.IO;
using System.Linq;

namespace Maze.StateFoundry.Editor
{
    sealed class UmlPrinter
    {
        // FIXME
        const string PATH = @"C:\Users\StefanoPittalis\Desktop\temp\";
        const string FILE_EXTENSION = ".g.puml";

        readonly BlockFinder m_finder;
        readonly TextGenerator m_textGenerator;

        public UmlPrinter(BlockFinder finder, TextGenerator textGenerator)
        {
            m_finder = finder;
            m_textGenerator = textGenerator;
        }

        public void Print()
        {
            string text = m_textGenerator.GetText();
            File.WriteAllText(GetFileAbsolutePath(), text);
        }

        string GetFileAbsolutePath()
        {
            Type root = m_finder.GetBlocks().First();
            return Path.Combine(PATH, $"{root.Name}{FILE_EXTENSION}");
        }
    }
}
