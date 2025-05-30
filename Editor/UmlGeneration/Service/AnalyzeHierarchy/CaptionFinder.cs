using System;
using System.Collections.Generic;
using System.Linq;

namespace Maze.StateFoundry.Editor
{
    sealed class CaptionFinder
    {
        public StateGraph FindCaptions(StateGraph tree)
        {
            foreach (KeyValuePair<Type, StateMeta> pair in tree.States)
            {
                CacheCaption(pair);
            }

            return tree;
        }

        static void CacheCaption(KeyValuePair<Type, StateMeta> pair)
        {
            foreach (string caption in GetCaptions(pair.Key))
            {
                pair.Value.AddCaption(caption);
            }
        }

        static IEnumerable<string> GetCaptions(Type type)
        {
            return type.GetCustomAttributes(typeof(CaptionAttribute), false).Cast<CaptionAttribute>().Select(attr => attr.Caption);
        }
    }
}