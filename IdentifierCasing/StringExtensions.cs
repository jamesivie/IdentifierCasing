using System;

namespace IdentifierCasing.Utility;

public static class StringExtensions
{
#if false
    /// <summary>
    /// Finds the index of the first character *not* punctuation or whitespace.
    /// </summary>
    /// <param name="source">The source string.</param>
    /// <param name="start">The index to start at.</param>
    /// <param name="count">The number of characters to look at (defaults to the end of the string).</param>
    /// <returns>The index of the first character not a punctuation or whitespace character, or -1 if the string is empty or composed entirely of punctuation and whitespace characters.</returns>
    public static int IndexOfNotPunctuationOrWhitespace(this string source, int start = 0, int count = -1)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        int stopChar = (count < 0 || start + count > source.Length) ? source.Length : start + count;
        for (int index = start; index < stopChar; ++index)
        {
            char c = source[index];
            // is this char NOT punctuation?
            if (!Char.IsWhiteSpace(c) && !Char.IsPunctuation(c))
            {
                return index;
            }
        }
        // if we get here, all characters were characters in the list, so we return -1
        return -1;
    }
#endif
    /// <summary>
    /// Finds the index of the last character *not* punctuation or whitespace.
    /// </summary>
    /// <param name="source">The source string.</param>
    /// <param name="start">The index to start at.</param>
    /// <param name="count">The number of characters to look at (defaults to the end of the string).</param>
    /// <returns>The index of the last character not a punctuation or whitespace character, or -1 if the string is empty or composed entirely of punctuation and whitespace characters.</returns>
    public static int LastIndexOfNotPunctuationOrWhitespace(this string source, int start = 0, int count = -1)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        int stopChar = (count < 0 || start + count > source.Length) ? source.Length : start + count;
        for (int index = stopChar - 1; index >= start; --index)
        {
            char c = source[index];
            // is this char NOT punctuation?
            if (!Char.IsWhiteSpace(c) && !Char.IsPunctuation(c))
            {
                return index;
            }
        }
        // if we get here, all characters were characters in the list, so we return -1
        return -1;
    }
    /// <summary>
    /// Finds the index of the first character *not* punctuation, whitespace, or symbol.
    /// </summary>
    /// <param name="source">The source string.</param>
    /// <param name="start">The index to start at.</param>
    /// <param name="count">The number of characters to look at (defaults to the end of the string).</param>
    /// <returns>The index of the first character not a punctuation, whitespace, or symbol character, or -1 if the string is empty or composed entirely of punctuation, whitespace, and symbol characters.</returns>
    public static int IndexOfNotPunctuationWhitespaceOrSymbol(this string source, int start = 0, int count = -1)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        int stopChar = (count < 0 || start + count > source.Length) ? source.Length : start + count;
        for (int index = start; index < stopChar; ++index)
        {
            char c = source[index];
            // is this char NOT punctuation, whitespace, or symbol?
            if (!Char.IsWhiteSpace(c) && !Char.IsPunctuation(c) && !Char.IsSymbol(c))
            {
                return index;
            }
        }
        // if we get here, all characters were characters in the list, so we return -1
        return -1;
    }
    /// <summary>
    /// Finds the index of the last character *not* punctuation, whitespace, or symbol.
    /// </summary>
    /// <param name="source">The source string.</param>
    /// <param name="start">The index to start at.</param>
    /// <param name="count">The number of characters to look at (defaults to the end of the string).</param>
    /// <returns>The index of the last character not a punctuation, whitespace, or symbol character, or -1 if the string is empty or composed entirely of punctuation, whitespace, and symbol characters.</returns>
    public static int LastIndexOfNotPunctuationWhitespaceOrSymbol(this string source, int start = 0, int count = -1)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        int stopChar = (count < 0 || start + count > source.Length) ? source.Length : start + count;
        for (int index = stopChar - 1; index >= start; --index)
        {
            char c = source[index];
            // is this char NOT punctuation?
            if (!Char.IsWhiteSpace(c) && !Char.IsPunctuation(c) && !Char.IsSymbol(c))
            {
                return index;
            }
        }
        // if we get here, all characters were characters in the list, so we return -1
        return -1;
    }
}
