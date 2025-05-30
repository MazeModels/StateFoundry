using System;

namespace Maze.StateFoundry
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class CaptionAttribute : Attribute
    {
        public string Caption { get; }

        public CaptionAttribute(string caption)
        {
            Caption = caption;
        }

        public CaptionAttribute(When when, string what)
        {
            Caption = $"{when} → {what}";
        }
    }
}