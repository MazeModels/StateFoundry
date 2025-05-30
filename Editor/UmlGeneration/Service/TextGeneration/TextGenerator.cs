using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze.StateFoundry.Editor
{
    sealed class TextGenerator
    {
        const int TAB_MULTIPLIER = 2;

        readonly BlockFinder m_finder;
        readonly BlockAnalyzer m_analyzer;
        readonly HashSet<string> m_fullNames;

        string m_generatedText;


        public TextGenerator(BlockFinder finder, BlockAnalyzer analyzer)
        {
            m_finder = finder;
            m_analyzer = analyzer;
            m_fullNames = new HashSet<string>();
        }


        public void GenerateText()
        {
            Type block = m_finder.GetBlocks().First();
            Type initialState = m_analyzer.GetInitialState();
            StateGraph tree = m_analyzer.GetTree();

            CacheCompleteNames(block, tree);
            m_generatedText = CreateFullText(block, tree, initialState);
        }

        public string GetText()
        {
            return m_generatedText;
        }

        void CacheCompleteNames(Type block, StateGraph tree)
        {
            string blockName = GetStartingName(block);
            BuildFullNames(tree, blockName, m_fullNames);
        }

        string CreateFullText(Type block, StateGraph tree, Type initialState)
        {
            var sb = new StringBuilder();
            AddHeader(block, sb);

            AddState(block, tree, sb, m_fullNames);
            AddTransitions(tree, sb, m_fullNames);
            AddNotes(tree, sb, m_fullNames);
            AddStartingPoint(initialState, sb, m_fullNames);

            AddFooter(sb);
            return sb.ToString();
        }

        static void BuildFullNames(StateGraph tree, string currentName, HashSet<string> fullNames)
        {
            fullNames.Add(currentName);
            foreach (StateMeta state in tree.States.Values)
            {
                if (state.Parent != null)
                {
                    continue;
                }

                string fullName = EnqueueName(currentName, state.ToString());
                fullNames.Add(fullName);

                foreach (StateMeta child in state.Children)
                {
                    BuildFullNamesRecursive(child, fullName, fullNames);
                }
            }
        }

        static void BuildFullNamesRecursive(StateMeta state, string currentName, HashSet<string> fullNames)
        {
            string fullName = EnqueueName(currentName, state.Type.Name);
            fullNames.Add(fullName);

            foreach (StateMeta child in state.Children)
            {
                BuildFullNamesRecursive(child, fullName, fullNames);
            }
        }

        static void AddHeader(Type blockType, StringBuilder sb)
        {
            sb.AppendLine($"@startuml {blockType.Name}");
        }

        static void AddState(Type block, StateGraph tree, StringBuilder sb, HashSet<string> fullNames)
        {
            string fullName = FindFullName(block, fullNames);
            sb.AppendLine($"state \"{block.Name}\" as {fullName} {{");

            foreach (StateMeta state in tree.States.Values)
            {
                if (state.Parent != null)
                {
                    continue;
                }

                AddStateRecursive(state, sb, fullNames, 1);
            }

            sb.AppendLine("}");
        }

        static void AddStateRecursive(StateMeta node, StringBuilder sb, HashSet<string> fullNames, int indentationLevel)
        {
            string spaces = new(' ', indentationLevel * TAB_MULTIPLIER);
            string fullName = FindFullName(node.Type, fullNames);

            sb.AppendLine($"{spaces}state \"{node.Type.Name}\" as {fullName} {{");
            foreach (StateMeta child in node.Children)
            {
                AddStateRecursive(child, sb, fullNames, indentationLevel + 1);
            }

            sb.AppendLine($"{spaces}}}");
        }

        static void AddTransitions(StateGraph tree, StringBuilder sb, HashSet<string> fullNames)
        {
            foreach (StateMeta state in tree.States.Values)
            {
                string startNode = FindFullName(state.Type, fullNames);

                foreach (KeyValuePair<Type, StateMeta> transition in state.DirectTransition)
                {
                    string endNode = FindFullName(transition.Value.Type, fullNames);
                    string input = transition.Key.Name;
                    sb.AppendLine($"{startNode} --> {endNode} : {input}");
                }
            }
        }

        static void AddNotes(StateGraph tree, StringBuilder sb, HashSet<string> fullNames)
        {
            foreach (StateMeta state in tree.States.Values)
            {
                string node = FindFullName(state.Type, fullNames);
                foreach (string note in state.Notes)
                {
                    sb.AppendLine($"{node} : {note}");
                }
            }
        }

        static void AddStartingPoint(Type initialState, StringBuilder sb, HashSet<string> fullNames)
        {
            string name = FindFullName(initialState, fullNames);
            sb.AppendLine($"[*] --> {name}");
        }

        static void AddFooter(StringBuilder sb)
        {
            sb.AppendLine("@enduml");
        }

        static string GetStartingName(Type blockType)
        {
            string commaSeparatedName = $"{blockType.Namespace}.{blockType.Name}";
            return commaSeparatedName.Replace(".", "_");
        }

        static string EnqueueName(string baseName, string newName)
        {
            return $"{baseName}_{newName}";
        }

        static string FindFullName(Type type, HashSet<string> fullNames)
        {
            return fullNames.FirstOrDefault(fullName => fullName.EndsWith($"_{type.Name}"));
        }
    }
}