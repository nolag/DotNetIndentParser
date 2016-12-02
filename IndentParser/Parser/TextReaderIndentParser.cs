using System;
using System.IO;
using System.Text;
using IndentParser.IO;
using IndentParser.Strategy;

namespace IndentParser.Parser
{
    public class TextReaderIndentParser : IIndentParser<TextReader>
    {
        private readonly IIndentDetectionStrategy strategy;
        private readonly string indent;
        private readonly string dedent;

        public TextReaderIndentParser(IIndentDetectionStrategy strategy, string indent, string dedent)
        {
            if (strategy == null)
            {
                throw new ArgumentNullException(nameof(strategy));
            }

            this.strategy = strategy;
            this.indent = indent ?? string.Empty;
            this.dedent = dedent ?? string.Empty;
        }

        public TextReader ParseIndents(TextReader input)
        {
            return new LineReader(input, ConvertLine);
        }

        public TextReader ParseIndents(string input)
        {
            var stream = new MemoryStream(Encoding.Unicode.GetBytes(input));
            stream.Position = 0;
            return ParseIndents(new StreamReader(stream, Encoding.Unicode));
        }

        private string ConvertLine(string line)
        {
            var numIndents =  strategy.CasuedIndents(line);
            var sb = new StringBuilder();
            for (int i = 0; i < numIndents; i++)
            {
                sb.Append(indent);
            }

            for (int i = 0; i > numIndents; i--)
            {
                sb.Append(dedent);
            }

            return sb.Append(line).ToString();
        }
    }
}
