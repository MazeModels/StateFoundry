using System;
using System.Collections.Generic;
using System.Linq;

namespace Maze.StateFoundry.Editor
{
    sealed class OutputFinder
    {
        public StateGraph FindOutputs(StateGraph tree)
        {
            foreach (KeyValuePair<Type, StateMeta> pair in tree.States)
            {
                CacheNotes(pair);
            }

            return tree;
        }

        static void CacheNotes(KeyValuePair<Type, StateMeta> pair)
        {
            foreach (string note in GetNotes(pair.Key))
            {
                pair.Value.AddNote(note);
            }
        }

        static IEnumerable<string> GetNotes(Type type)
        {
            return type.GetCustomAttributes(typeof(UmlNoteAttribute), false).Cast<UmlNoteAttribute>().Select(attr => attr.Message);
        }
    }
}