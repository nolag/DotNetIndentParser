using System;
using System.IO;
using System.Threading.Tasks;

namespace IndentParser.IO
{
    public class LineReader : TextReader
    {
        private readonly TextReader baseReader;
        private readonly Func<string, string> lineCallback;

        private string nextLine = null;
        private int on = 0;

        public LineReader(TextReader baseReader, Func<string, string> lineCallback)
        {
            this.baseReader = TOrThrow(baseReader, nameof(baseReader));
            this.lineCallback = TOrThrow(lineCallback, nameof(lineCallback));
        }

        public override int Read()
        {
            var value = Peek();
            on++;
            return value;
        }

        public override int Peek()
        {
            Prepare();
            if (nextLine == null)
            {
                return -1;
            }

            return nextLine[on];
        }

        private void Prepare()
        {
            if (nextLine == null || on == nextLine.Length)
            {
                var nextReadLine = baseReader.ReadLine();
                if (nextReadLine == null)
                {
                    nextLine = null;
                    return;
                }

                nextLine = lineCallback(nextReadLine) + Environment.NewLine;
                on = 0;
            }
        }

        private T TOrThrow<T>(T value, string name)
        {
            if (value == null)
            {
                throw new ArgumentNullException(name);
            }

            return value;
        }
    }
}
