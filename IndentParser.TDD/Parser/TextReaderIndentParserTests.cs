using System;
using System.IO;
using System.Linq;
using System.Text;
using IndentParser.Strategy;
using IndentParser.TestHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace IndentParser.Parser
{
    [TestClass]
    public class TextReaderIndentParserTests
    {
        private const string AnyIndentString = "<Indent>";
        private const string AnyDedndentString = "<Dedent>";

        // Not static so that each test can reuse it.
        private readonly string[] AnyLines = new[] { "AnyLine", "AnotherAnyLine", "LineYaay", "MultiIndentLine" };

        [TestMethod]
        public void ConstructorThrowsOnNullStrategy()
        {
            ExceptionHelper.AssertArgumentException<ArgumentNullException>(
                () => new TextReaderIndentParser(null, "AnyString", "AnotherAnyString"),
                "strategy");
        }

        [TestMethod]
        public void NoIndentsOrDedentsReturnsOriginalText()
        {
            var indentStrategy = new Mock<IIndentDetectionStrategy>(MockBehavior.Strict);
            indentStrategy.Setup(s => s.CasuedIndents(It.IsAny<string>())).Returns(0);
            var anyTextWithNoIndentsOrDedents = @"Text
with some value
and more";

            var parser = new TextReaderIndentParser(indentStrategy.Object, AnyIndentString, AnyDedndentString);
            RunTest(parser, anyTextWithNoIndentsOrDedents, anyTextWithNoIndentsOrDedents);
        }

        [TestMethod]
        public void IndentStringReplacesIndents()
        {
            var anyNumberOfIndents = new[] { 1, 2, 0, 4 };
            RunIndentTest(anyNumberOfIndents, AnyIndentString);
        }

        [TestMethod]
        public void DedentStringReplacesDedentds()
        {
            var anyNumberOfIndents = new[] { -1, -2, 0, -4 };
            RunIndentTest(anyNumberOfIndents, AnyDedndentString);
        }

        [TestMethod]
        public void NullIndentAndDedentsAreTreatedAsEmptyStrings()
        {
            var anyNumIndents = new[] { -1, -2, 1, 4 };
            var indentStrategy = CreateStrategy(anyNumIndents);
            var parser = new TextReaderIndentParser(indentStrategy, null, null);
            var anyInputText = string.Join(Environment.NewLine, AnyLines);
            RunTest(parser, anyInputText, anyInputText);
        }

        private void RunIndentTest(int[] numIndents, string replacement)
        {
            IIndentDetectionStrategy indentStrategy = CreateStrategy(numIndents);
            var input = string.Join(Environment.NewLine, AnyLines);
            for (var i = 0; i < AnyLines.Length; i++)
            {
                var numTimes = numIndents[i];
                if (numTimes < 0) numTimes = -numTimes;
                AnyLines[i] = string.Concat(Enumerable.Repeat(replacement, numTimes)) + AnyLines[i];
            }

            var expectedOutput = string.Join(Environment.NewLine, AnyLines);
            var parser = new TextReaderIndentParser(indentStrategy, AnyIndentString, AnyDedndentString);
            RunTest(parser, input, expectedOutput);
        }

        private IIndentDetectionStrategy CreateStrategy(int[] numIndents)
        {
            var indentStrategy = new Mock<IIndentDetectionStrategy>(MockBehavior.Strict);
            for (var i = 0; i < AnyLines.Length; i++)
            {
                indentStrategy.Setup(s => s.CasuedIndents(AnyLines[i])).Returns(numIndents[i]);
            }

            return indentStrategy.Object;
        }

        private static void RunTest(TextReaderIndentParser parser, string input, string expectedOutput)
        {
            string result;
            using (var resultStream = parser.ParseIndents(input))
            {
                result = resultStream.ReadToEnd();
            }

            Assert.AreEqual(expectedOutput + Environment.NewLine, result, "Wrong output for string indent parsing");
            using (var stream = new MemoryStream(Encoding.Unicode.GetBytes(input)))
            using (var reader = new StreamReader(stream, Encoding.Unicode))
            using (var resultStream = parser.ParseIndents(reader))
            {
                result = resultStream.ReadToEnd();
            }

            Assert.AreEqual(expectedOutput + Environment.NewLine, result, "Wrong output for string text reader parsing");
        }
    }
}
