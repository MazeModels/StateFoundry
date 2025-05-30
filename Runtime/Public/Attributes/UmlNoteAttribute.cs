using System;

namespace Maze.StateFoundry
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class UmlNoteAttribute : Attribute
    {
        public string Message { get; }

        public UmlNoteAttribute(string message)
        {
            Message = message;
        }

        public UmlNoteAttribute(Method method, string msg)
        {
            Message = $"{method}() → {msg}";
        }
    }
}