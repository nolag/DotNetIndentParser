using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static IndentParser.Strategy.StrategyTestHelper;

namespace IndentParser.Strategy
{
    /// <summary>
    /// Contains tests for basic strategies that only look at a specific string for indents.
    /// </summary>
    /// <remarks>
    /// This class assumes that all alphabetic characters and new line cahracters are not an indent.
    /// It also assumes that indents are detected only after new lines.
    /// </remarks>
    public abstract class BasicIndentStrategyTestBase
    {
        private const string IndentReplacement = "{INDENT}";

        protected abstract string[] Indents { get; }

        [TestMethod]
        public void InputWithNoTabsCauseNoIndentsOrDedents()
        {
            RunStrategyTestForEachIndent(
                Tuple.Create("AnyNoIndentString", 0),
                Tuple.Create("NoIndentAnyValue", 0),
                Tuple.Create("NotIndentedValue", 0),
                Tuple.Create("StillNotIndented", 0));
        }

        [TestMethod]
        public void IndentsDetectedAndSameLevelSkipped()
        {
            RunStrategyTestForEachIndent(
                Tuple.Create("ValueAny", 0),
                Tuple.Create($"{IndentReplacement}AnyValue", 1),
                Tuple.Create($"{IndentReplacement}{IndentReplacement}DoIndented", 1),
                Tuple.Create($"{IndentReplacement}{IndentReplacement}NotIndented", 0),
                Tuple.Create($"{IndentReplacement}{IndentReplacement}{IndentReplacement}Indented", 1));
        }


        [TestMethod]
        public void IndentsNotDetectedInMiddleOfLine()
        {
            RunStrategyTestForEachIndent(
                Tuple.Create("NotIndented", 0),
                Tuple.Create($"Lala${IndentReplacement}Nope", 0),
                Tuple.Create($"{IndentReplacement}Cool", 1));
        }

        [TestMethod]
        public void DedendentsAndReindentsDetected()
        {
            RunStrategyTestForEachIndent(
                Tuple.Create("NotIndented", 0),
                Tuple.Create($"{IndentReplacement}Indented", 1),
                Tuple.Create($"{IndentReplacement}{IndentReplacement}adsfjs", 1),
                Tuple.Create($"{IndentReplacement}Bob", -1),
                Tuple.Create($"AllIndentsGone", -1),
                Tuple.Create($"{IndentReplacement}Bob", 1));
        }

        [TestMethod]
        public void BlankLinesWithOnlyIndentsAreIgnored()
        {
            RunStrategyTestForEachIndent(
                Tuple.Create($"{IndentReplacement}", 0),
                Tuple.Create($"{IndentReplacement}value", 1),
                Tuple.Create(string.Empty, 0),
                Tuple.Create($"{IndentReplacement}{IndentReplacement}{IndentReplacement}", 0),
                Tuple.Create($"{IndentReplacement}{IndentReplacement}V", 1));
        }

        [TestMethod]
        public void MultiLevelDedentsAreDetected()
        {

            RunStrategyTestForEachIndent(
                Tuple.Create($"Ryan", 0),
                Tuple.Create($"{IndentReplacement}Ryan", 1),
                Tuple.Create($"{IndentReplacement}{IndentReplacement}Bryan", 1),
                Tuple.Create($"Jow", -2),
                Tuple.Create($"{IndentReplacement}Ryan", 1));
        }

        [TestMethod]
        public void MultipleIndentsBecomeIndentLevel()
        {
            RunStrategyTestForEachIndent(
                Tuple.Create($"Ryan", 0),
                Tuple.Create($"{IndentReplacement}{IndentReplacement}Ryan", 1),
                Tuple.Create($"{IndentReplacement}{IndentReplacement}Bryan", 0),
                Tuple.Create($"Jow", -1),
                Tuple.Create($"{IndentReplacement}{IndentReplacement}Ryan", 1));
        }

        [TestMethod]
        public void InvaidIdentationThrownWhenDedentsDontMatchIndents()
        {
            RunThrowingStrategyTestForEachIndent(
                "Invlaid dedent level",
                3,
                "NoIndent",
                $"{IndentReplacement}{IndentReplacement}TwoIndent",
                $"{IndentReplacement}OMG1Indent");
        }

        [TestMethod]
        public void NullLinesAreIgnored()
        {
            RunStrategyTestForEachIndent(
                Tuple.Create($"{IndentReplacement}", 0),
                Tuple.Create($"{IndentReplacement}value", 1),
                Tuple.Create((string)null, 0),
                Tuple.Create($"{IndentReplacement}{IndentReplacement}{IndentReplacement}", 0),
                Tuple.Create($"{IndentReplacement}{IndentReplacement}V", 1));
        }

        protected abstract IIndentDetectionStrategy GetStrategy();

        protected void RunStrategyTestForEachIndent(params Tuple<string, int>[] linesAndIndents)
        {
            foreach (var indent in Indents)
            {
                var transformed = linesAndIndents
                    .Select(l => Tuple.Create(l.Item1?.Replace(IndentReplacement, indent), l.Item2));
                StrategyTestHelper.RunStrategyTest(this.GetStrategy(), transformed);

            }
        }

        protected void RunThrowingStrategyTestForEachIndent(string message, int line, params string[] lines)
        {
            foreach (var indent in Indents)
            {
                var lineOn = 0;
                var transformed = lines.Select(l => l?.Replace(IndentReplacement, indent)).ToList();
                try
                {
                    var strategy = GetStrategy();
                    for (; lineOn < lines.Length; lineOn++) strategy.CasuedIndents(transformed[lineOn]);
                    Assert.Fail($"Expected invalid indentation with message: {message}");
                }
                catch (InvalidIndentationException e)
                {
                    Assert.AreEqual(line, e.Line, "Unexpected line in exception");
                    Assert.AreEqual($"{message} on line {line}", e.Message, "Wrong reason");
                }
            }

        }
    }
}
