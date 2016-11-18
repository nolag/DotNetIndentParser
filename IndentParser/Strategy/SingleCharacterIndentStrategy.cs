using System.Collections.Generic;

namespace IndentParser.Strategy
{
    public sealed class SingleCharacterIndentStrategy : IIndentDetectionStrategy
    {
        private readonly Dictionary<int, int> indentsSeen = new Dictionary<int, int> { [0] = 0 };
        private int currentNumIndents = 0;
        private int currentIndentLevel = 0;

        public char Indent { get; }

        public int Line { get; set; }

        public SingleCharacterIndentStrategy(char indent)
        {
            Indent = indent;
        }

        public int CasuedIndents(string nextLine)
        {
            Line++;

            if (nextLine == null) return 0;

            var numIndents = 0;
            for (; nextLine.Length > numIndents && nextLine[numIndents] == Indent; numIndents++);

            if (numIndents == nextLine.Length)
            {
                return 0;
            }

            if (numIndents > currentNumIndents)
            {
                indentsSeen[numIndents] = ++currentIndentLevel;
                currentNumIndents = numIndents;
                return 1;
            }
            else if (numIndents < currentNumIndents)
            {
                var prevIndents = currentIndentLevel;
                if (indentsSeen.TryGetValue(numIndents, out currentIndentLevel))
                {
                    currentIndentLevel = numIndents;
                    currentNumIndents = numIndents;
                    return currentIndentLevel - prevIndents;
                }
                else
                {
                    throw new InvalidIndentationException("Invlaid dedent level", Line);
                }
            }

            return 0;
        }
    }
}
