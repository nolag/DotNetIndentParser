using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IndentParser.Strategy
{
    public static class StrategyTestHelper
    {
        public static void RunStrategyTest(
            IIndentDetectionStrategy strategy,
            IEnumerable<Tuple<string, int>> linesAndIndents)
        {
            foreach (var lineAndIndents in linesAndIndents)
            {
                Assert.AreEqual(
                    lineAndIndents.Item2,
                    strategy.CasuedIndents(lineAndIndents.Item1),
                    "Unexpected amount of indents or dedents");
            }
        }
    }
}
