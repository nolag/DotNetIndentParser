using System;
using System.IO;
using System.Linq;
using System.Text;
using IndentParser.TestHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace IndentParser.IO
{
    [TestClass]
    public class LineReaderTests
    {
        [TestMethod]
        public void Constructor_ThrowsForNullBaseReader()
        {
            Func<string, string> anyLineCallback = s => s;
            ExceptionHelper.AssertArgumentException<ArgumentNullException>(
                () => new LineReader(null, anyLineCallback),
                "baseReader");
        }

        [TestMethod]
        public void Constructor_ThrowsForNullCallback()
        {
            var anyBaseReader = new Mock<TextReader>(MockBehavior.Strict).Object;
            ExceptionHelper.AssertArgumentException<ArgumentNullException>(
                () => new LineReader(anyBaseReader, null),
                "lineCallback");
        }

        [TestMethod]
        public void Peek_DoesNotCauseIncrementInPosition()
        {
            Func<string, string> anyStringTransform = s => s;
            var input = @"asdf
asdf";
            using (var stream = new MemoryStream(Encoding.Unicode.GetBytes(input)))
            using (var reader = new StreamReader(stream, Encoding.Unicode))
            {
                var lineReader = new LineReader(reader, anyStringTransform);
                Assert.AreEqual(input[0], lineReader.Peek());
                Assert.AreEqual(input[0], lineReader.Peek());
                Assert.AreEqual(input[0], lineReader.Read());
                Assert.AreEqual(input[1], lineReader.Peek());
                Assert.AreEqual(input.Substring(1) + Environment.NewLine, lineReader.ReadToEnd(), "Wrong value retured");
            }
        }

        [TestMethod]
        public void Read_ReturnsCorrectValues()
        {
            var anyLines = new[] { "AnyValue", "woohoovalueAnything", "Line" };
            var input = string.Join(Environment.NewLine, anyLines);
            var anyReturnValues = anyLines.Select(l => Guid.NewGuid().ToString()).ToList();
            var expectedValue = string.Join(Environment.NewLine, anyReturnValues) + Environment.NewLine;
            var on = 0;


            Func<string, string> anyStringTransform = s =>
            {
                Assert.IsTrue(on < anyLines.Length, "Too many callbacks");
                Assert.AreEqual(anyLines[on], s, "Wrong line on callback");
                return anyReturnValues[on++];
            };

            using (var stream = new MemoryStream(Encoding.Unicode.GetBytes(input)))
            using (var reader = new StreamReader(stream, Encoding.Unicode))
            {
                var lineReader = new LineReader(reader, anyStringTransform);
                Assert.AreEqual(expectedValue, lineReader.ReadToEnd(), "Wrong value retured");
            }
        }
    }
}
