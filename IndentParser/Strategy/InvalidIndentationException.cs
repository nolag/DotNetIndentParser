using System;

namespace IndentParser.Strategy
{
    public class InvalidIndentationException : Exception
    {
        public int Line { get; }
        public InvalidIndentationException(string reason, int line) : base($"{reason} on line {line}")
        {
            Line = line;
        }
    }
}
