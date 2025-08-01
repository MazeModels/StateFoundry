using System;

namespace Maze.StateFoundry
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class CaptionAttribute : Attribute
    {
        public string Caption { get; }

        public CaptionAttribute(string caption)
        {
            EnsureIsValidCaption(caption);
            Caption = caption;
        }

        public CaptionAttribute(When when, string what)
        {
            EnsureIsValidCaption(what);
            Caption = $"{when} → {what}";
        }

        static void EnsureIsValidCaption(string caption)
        {
            if (string.IsNullOrEmpty(caption))
            {
                throw new ArgumentNullException(nameof(caption));
            }
        }
    }
}