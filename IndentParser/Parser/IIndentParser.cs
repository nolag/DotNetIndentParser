using System.IO;

namespace IndentParser.Parser
{
    /// <summary>
    /// An interface for parsing indents
    /// </summary>
    /// <typeparam name="T">The type of object returned from parsing</typeparam>
    public interface IIndentParser<T>
    {
        /// <summary>
        /// Parses the indents in <paramref name="input"/>.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>A <typeparamref name="T"/></returns>
        T ParseIndents(string input);

        /// <summary>
        /// Parses the indents in <paramref name="input"/>.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>A <typeparamref name="T"/></returns>
        T ParseIndents(TextReader input);
    }
}
