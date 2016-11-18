namespace IndentParser.Strategy
{
    /// <summary>
    /// An interface for classes that can detect indentation
    /// </summary>
    public interface IIndentDetectionStrategy
    {
        /// <summary>
        /// Determines the number of indentation levels caused by <paramref name="nextLine"/>.
        /// </summary>
        /// <param name="nextLine">The next line.</param>
        /// <returns>The number of indentation levels caused by <paramref name="nextLine"/>.</returns>
        /// <remarks>
        /// -ve values indicates dedents, while +ve values indicates indents.
        /// </remarks>
        int CasuedIndents(string nextLine);
    }
}
