using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Maze.StateFoundry.Editor
{
    sealed class BlockFinder
    {
        const string CLASS_REGEX = @"\bclass\s+(\w+)";
        static readonly Type s_baseBlockType = typeof(Statechart<>);

        List<Type> m_blocks;
        public string FilePath;


        public void SetPath(string filePath)
        {
            FilePath = filePath;
        }

        public void FindBlocks()
        {
            EnsurePathIsValid(FilePath);

            List<string> classNames = ExtractClassNamesFromFile(FilePath);
            List<Type> loadedTypes = GetAllLoadedTypes();
            m_blocks = GetTypesInheritingFromBase(classNames, loadedTypes);
        }

        public List<Type> GetBlocks()
        {
            return m_blocks ?? new List<Type>();
        }

        public int BlockCount()
        {
            return m_blocks?.Count ?? 0;
        }


        static void EnsurePathIsValid(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException(nameof(filePath), "File path not specified");
            }

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("File not found", filePath);
            }
        }

        static List<string> ExtractClassNamesFromFile(string filePath)
        {
            MatchCollection matches = ExtractMatches(filePath);
            return ExtractClassNames(matches);
        }

        static MatchCollection ExtractMatches(string filePath)
        {
            string fileContent = File.ReadAllText(filePath);
            var classRegex = new Regex(CLASS_REGEX, RegexOptions.Compiled);
            return classRegex.Matches(fileContent);
        }

        static List<string> ExtractClassNames(MatchCollection matches)
        {
            var classNames = new List<string>();
            foreach (Match match in matches)
            {
                string className = match.Groups[1].Value;
                classNames.Add(className);
            }

            return classNames;
        }

        static List<Type> GetAllLoadedTypes()
        {
            Assembly[] allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            var allTypes = new List<Type>();

            foreach (Assembly assembly in allAssemblies)
            {
                IEnumerable<Type> typesInAssembly = GetTypesFromAssemblyNotThrow(assembly);
                allTypes.AddRange(typesInAssembly);
            }

            return allTypes;
        }

        static IEnumerable<Type> GetTypesFromAssemblyNotThrow(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch
            {
                return Array.Empty<Type>();
            }
        }

        static List<Type> GetTypesInheritingFromBase(IEnumerable<string> classNames, List<Type> loadedTypes)
        {
            var matchingTypes = new List<Type>();

            foreach (string className in classNames)
            {
                if (!TryFindTypeByName(loadedTypes, className, out Type type))
                {
                    continue;
                }


                if (InheritsFromArtLogicBlock(type))
                {
                    matchingTypes.Add(type);
                }
            }

            return matchingTypes;
        }

        static bool TryFindTypeByName(IEnumerable<Type> types, string className, out Type type)
        {
            type = types.FirstOrDefault(t => t.Name == className);
            return type != null;
        }

        static bool InheritsFromArtLogicBlock(Type type)
        {
            return InheritsFromGeneric(type, s_baseBlockType);
        }

        static bool InheritsFromGeneric(Type candidateType, Type genericBaseType)
        {
            Type currentType = candidateType;

            while (currentType != null && currentType != typeof(object))
            {
                if (IsGenericTypeMatch(currentType, genericBaseType))
                {
                    return true;
                }

                if (TryBaseTypeNoThrow(currentType, out Type baseType))
                {
                    currentType = baseType;
                }
            }

            return false;
        }

        static bool IsGenericTypeMatch(Type type, Type genericBaseType)
        {
            if (!type.IsGenericType)
            {
                return false;
            }

            Type genericTypeDefinition = type.GetGenericTypeDefinition();
            return genericTypeDefinition == genericBaseType;
        }

        static bool TryBaseTypeNoThrow(Type type, out Type baseType)
        {
            try
            {
                baseType = type.BaseType;
                return true;
            }
            catch
            {
                baseType = null;
                return false;
            }
        }
    }
}