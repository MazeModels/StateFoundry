using System;
using System.IO;
using System.Linq;

namespace Maze.StateFoundry.Editor
{
    sealed class UmlPrinter
    {
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
            string relativePath = Path.GetDirectoryName(m_finder.FilePath);

            if (relativePath == null)
            {
                throw new DirectoryNotFoundException($"Directory not found in path: {m_finder.FilePath}");
            }

            string absolutePath = Path.GetFullPath(relativePath);
            return Path.Combine(absolutePath, $"{root.Name}{FILE_EXTENSION}");
        }
    }
}