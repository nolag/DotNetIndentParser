using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IndentParser.Strategy
{
    [TestClass]
    public class SingleCharacterIndentStrategyTests : BasicIndentStrategyTestBase
    {
        private const char IndentTestCharacter = '\t';
        protected override string[] Indents { get; } = new[] { IndentTestCharacter.ToString() };

        protected override IIndentDetectionStrategy GetStrategy()
        {
            return new SingleCharacterIndentStrategy(IndentTestCharacter);
        }
    }
}
