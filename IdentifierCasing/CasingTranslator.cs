using System;
using System.IO;
using System.Text;

using IdentifierCasing.Utility;

namespace IdentifierCasing;

/// <summary>
/// A static class that holds casing translation functions.
/// </summary>
public static class CasingTranslator
{
    /// <summary>
    /// Detects the casing of the specified identifier.
    /// </summary>
    /// <param name="identifier">The identifier to check.</param>
    /// <returns>The <see cref="CasingStyle"/> of the specified string.</returns>
    public static CasingStyle DetectIdentifierCasing(string identifier)
    {
        if (string.IsNullOrEmpty(identifier)) throw new ArgumentException("The specified identifier is empty.  Identifier strings must have at least one character.");
        int leadingPunctuation = identifier.IndexOfNotPunctuationWhitespaceOrSymbol();
        // composed entirely of punctuation?  count that as 'mixed' (it's ambiguous)
        if (leadingPunctuation < 0) return CasingStyle.Mixed;
        int lastNonPunctuation = identifier.LastIndexOfNotPunctuationOrWhitespace();
        System.Diagnostics.Debug.Assert(lastNonPunctuation >= 0);
        bool hasUpper = false;
        bool hasLower = false;
        bool hasDashes = false;
        bool hasSpaces = false;
        bool hasOtherPunctuation = false;
        bool hasUnderscores = false;
        bool dashBeforeEveryMiddleUpper = true;
        bool spaceBeforeEveryMiddleUpper = true;
        bool firstIsUpper = Char.IsUpper(identifier[leadingPunctuation]);
        bool prevDash = true;
        bool prevSpace = true;
        for (int offset = leadingPunctuation; offset < lastNonPunctuation; ++offset)
        {
            char ch = identifier[offset];
            bool isUpper = Char.IsUpper(ch);
            bool isLower = Char.IsLower(ch);
            bool isPunct = Char.IsWhiteSpace(ch) || Char.IsPunctuation(ch) || Char.IsSymbol(ch);
            bool isDash = (ch == '-');
            bool isSpace = (ch == ' ');
            bool isUnderscore = (ch == '_');
            hasUpper = hasUpper || isUpper;
            hasLower = hasLower || isLower;
            hasDashes = hasDashes || isDash;
            hasSpaces = hasSpaces || isSpace;
            hasOtherPunctuation = hasOtherPunctuation || (isPunct && !isDash && !isSpace && !isUnderscore);
            hasUnderscores = hasUnderscores || isUnderscore;
            dashBeforeEveryMiddleUpper = dashBeforeEveryMiddleUpper && (isLower || isDash || isUnderscore || (isUpper && prevDash));
            spaceBeforeEveryMiddleUpper = spaceBeforeEveryMiddleUpper && (isLower || isSpace || isUnderscore || (isUpper && prevSpace));
            prevDash = isDash;
            prevSpace = isSpace;
        }
        if (!hasDashes && !hasUnderscores && !hasSpaces)
        {
            if (hasUpper && hasLower)
            {
                return firstIsUpper ? CasingStyle.Pascal : CasingStyle.Camel;
            }
            return (hasUpper) ? CasingStyle.Cobol : CasingStyle.Kebab;
        }
        if (!hasUnderscores && !hasSpaces && hasDashes && hasLower && hasUpper && dashBeforeEveryMiddleUpper) return CasingStyle.Train;
        if (!hasUnderscores && hasSpaces && !hasDashes && hasLower && hasUpper && spaceBeforeEveryMiddleUpper) return CasingStyle.Spreadsheet;
        if (!hasUnderscores && hasSpaces && !hasDashes && hasLower != hasUpper) return (hasUpper) ? CasingStyle.Upper : CasingStyle.Lower;
        // if the string has two of mixed case, dashes, spaces, and underscores, it's mixed
        int conditions = ((hasLower && hasUpper) ? 1 : 0) + (hasDashes ? 1 : 0) + (hasSpaces ? 1 : 0) + (hasUnderscores ? 1 : 0);
        if (conditions >= 2) return CasingStyle.Mixed;
        // NOTE that there is a special edge case here where there are characters that are in the identifier that are neither upper nor lower case--that's OK, we'll just consider it all upper-case in that situation.
        if (hasDashes)
        {
            return hasLower ? CasingStyle.Kebab : CasingStyle.Cobol;
        }
        return hasLower ? CasingStyle.Snake : CasingStyle.Macro;
    }
    /// <summary>
    /// Writes a system-cased identifier to the specified <see cref="TextWriter"/> with the specified casing.
    /// </summary>
    /// <param name="writer">The <see cref="TextWriter"/> to write to.</param>
    /// <param name="desiredStyle">The <see cref="CasingStyle"/> to use when writing.</param>
    /// <param name="systemCasedIdentifier">The system-cased identifier.</param>
    public static void WriteCasedIdentifier(TextWriter writer, CasingStyle desiredStyle, string systemCasedIdentifier)
    {
        if (writer == null) throw new ArgumentNullException(nameof(writer));
        // no translation needed?
        if (desiredStyle == CasingStyle.Pascal || string.IsNullOrEmpty(systemCasedIdentifier))
        {
            writer.Write(systemCasedIdentifier);
            return;
        }
        // invalid casing?
        switch (desiredStyle)
        {
            case CasingStyle.Lower:
            case CasingStyle.Upper:
            case CasingStyle.Camel:
            case CasingStyle.Kebab:
            case CasingStyle.Snake:
            case CasingStyle.Cobol:
            case CasingStyle.Macro:
            case CasingStyle.Train:
            case CasingStyle.Spreadsheet:
                break;
            case CasingStyle.Pascal:    // this should have been handled above!
            default:
                throw new ArgumentException("The specified IdentifierCasing is not supported as target casing format!");
        }
        char ch = systemCasedIdentifier[0];
        switch (desiredStyle)
        {
            case CasingStyle.Lower:
            case CasingStyle.Camel:
            case CasingStyle.Kebab:
            case CasingStyle.Snake:
                writer.Write(Char.ToLowerInvariant(ch));
                break;
            case CasingStyle.Upper:
            case CasingStyle.Pascal:
            case CasingStyle.Train:
            case CasingStyle.Spreadsheet:
            case CasingStyle.Cobol:
            case CasingStyle.Macro:
                writer.Write(Char.ToUpperInvariant(ch));
                break;
        }
        for (int offset = 1; offset < systemCasedIdentifier.Length; ++offset)
        {
            ch = systemCasedIdentifier[offset];
            // word break?
            if (Char.IsUpper(ch))
            {
                switch (desiredStyle)
                {
                    case CasingStyle.Lower:
                        if (offset != 1) writer.Write(' ');
                        writer.Write(Char.ToLowerInvariant(ch));
                        break;
                    case CasingStyle.Camel:
                        writer.Write(Char.ToUpperInvariant(ch));
                        break;
                    case CasingStyle.Kebab:
                        if (offset != 1) writer.Write('-');
                        writer.Write(Char.ToLowerInvariant(ch));
                        break;
                    case CasingStyle.Snake:
                        if (offset != 1) writer.Write('_');
                        writer.Write(Char.ToLowerInvariant(ch));
                        break;
                    case CasingStyle.Upper:
                    case CasingStyle.Spreadsheet:
                        if (offset != 1) writer.Write(' ');
                        writer.Write(ch);
                        break;
                    case CasingStyle.Train:
                    case CasingStyle.Cobol:
                        if (offset != 1) writer.Write('-');
                        writer.Write(ch);
                        break;
                    case CasingStyle.Macro:
                        if (offset != 1) writer.Write('_');
                        writer.Write(ch);
                        break;
                }
            }
            else // not a word break
            {
                System.Diagnostics.Debug.Assert(!Char.IsUpper(ch));
                switch (desiredStyle)
                {
                    case CasingStyle.Lower:
                    case CasingStyle.Pascal:
                    case CasingStyle.Train:
                    case CasingStyle.Spreadsheet:
                    case CasingStyle.Camel:
                    case CasingStyle.Kebab:
                    case CasingStyle.Snake:
                        writer.Write(ch);
                        break;
                    case CasingStyle.Upper:
                    case CasingStyle.Cobol:
                    case CasingStyle.Macro:
                        writer.Write(Char.ToUpperInvariant(ch));
                        break;
                }
            }
        }
    }
    /// <summary>
    /// Reads an identifier from the specified <see cref="TextReader"/> and converts it to the system-default casing type, which is <see cref="CasingStyle.Pascal"/> for C#.
    /// </summary>
    /// <param name="input">The <see cref="TextReader"/> to read from.</param>
    /// <returns>The identifier in <see cref="CasingStyle.Pascal"/> format.</returns>
    public static string ReadNormalizedCaseIdentifier(TextReader input)
    {
        if (input == null) throw new ArgumentNullException(nameof(input));
        StringBuilder identifierBuilder = new();
        int c;
        while ((c = input.Peek()) > 0 && (Char.IsLetterOrDigit((char)c)) || c == '-' || c == '_' || c == ' ')
        {
            identifierBuilder.Append((char)input.Read());
        }
        string identifier = identifierBuilder.ToString();
        return Translate(identifier, CasingStyle.Pascal);
    }
    /// <summary>
    /// Reads an identifier from the specified string and converts it to the system-default casing type, which is <see cref="CasingStyle.Pascal"/> for C#.
    /// </summary>
    /// <param name="identifier">The identifier.</param>
    /// <returns>The identifier in <see cref="CasingStyle.Pascal"/> format.</returns>
    public static string? NormalizeIdentifierCasingNullable(string? identifier)
    {
        if (string.IsNullOrEmpty(identifier)) return identifier;
        return NormalizeIdentifierCasing(identifier!);
    }
    /// <summary>
    /// Reads an identifier from the specified string and converts it to the system-default casing type, which is <see cref="CasingStyle.Pascal"/> for C#.
    /// </summary>
    /// <param name="identifier">The identifier.</param>
    /// <returns>The identifier in <see cref="CasingStyle.Pascal"/> format.</returns>
    public static string NormalizeIdentifierCasing(string identifier)
    {
        return Translate(identifier, CasingStyle.Pascal);
    }
    /// <summary>
    /// Translates the specified identifier into the specified desired casing format.
    /// </summary>
    /// <param name="identifier">The identifier to translate.</param>
    /// <param name="desiredStyle">The <see cref="CasingStyle"/> indicating the desired casing.</param>
    /// <returns>The identifier translated into the desired casing format.</returns>
    public static string? TranslateNullable(string? identifier, CasingStyle desiredStyle)
    {
        if (string.IsNullOrEmpty(identifier)) return identifier;
        return Translate(identifier!, desiredStyle);
    }
    /// <summary>
    /// Translates the specified identifier into the specified desired casing format.
    /// </summary>
    /// <param name="identifier">The identifier to translate.</param>
    /// <param name="desiredStyle">The <see cref="CasingStyle"/> indicating the desired casing.</param>
    /// <returns>The identifier translated into the desired casing format.</returns>
    public static string Translate(string identifier, CasingStyle desiredStyle)
    {
        if (string.IsNullOrEmpty(identifier)) return identifier;
        switch (desiredStyle)
        {
            case CasingStyle.Lower:
            case CasingStyle.Upper:
            case CasingStyle.Pascal:
            case CasingStyle.Train:
            case CasingStyle.Spreadsheet:
            case CasingStyle.Camel:
            case CasingStyle.Kebab:
            case CasingStyle.Snake:
            case CasingStyle.Cobol:
            case CasingStyle.Macro:
                break;
            default:
                throw new ArgumentException("The specified IdentifierCasing is not supported as target format!");
        }
        // find out what casing it's currently in
        CasingStyle casing = DetectIdentifierCasing(identifier);
        // is it already in the correct casing?
        if (casing == desiredStyle) return identifier;
        // allocate an output buffer with enough space for most cases
        StringBuilder output = new(identifier.Length * 4 / 3);
        // handle leading punctuation
        int offset = 0;
        for (; offset < identifier.Length; ++offset)
        {
            char c = identifier[offset];
            // a punctuation or symbol?
            if (Char.IsWhiteSpace(c) || Char.IsPunctuation(c) || Char.IsSymbol(c))
            {
                // just append the punctuation/symbol
                output.Append(c);
            }
            else
            {
                // handle the first non-separator character specially
                char ch = identifier[offset];
                switch (desiredStyle)
                {
                    case CasingStyle.Lower:
                    case CasingStyle.Camel:
                    case CasingStyle.Kebab:
                    case CasingStyle.Snake:
                        output.Append(Char.ToLowerInvariant(ch));
                        break;
                    case CasingStyle.Upper:
                    case CasingStyle.Pascal:
                    case CasingStyle.Spreadsheet:
                    case CasingStyle.Train:
                    case CasingStyle.Cobol:
                    case CasingStyle.Macro:
                        output.Append(Char.ToUpperInvariant(ch));
                        break;
                }
                break;
            }
        }
        // are there "word" characters?
        int endIndex = identifier.LastIndexOfNotPunctuationWhitespaceOrSymbol();
        if (endIndex >= 0)
        {
            // now handle "word" characters
            bool nonUpperCaseCharactersSincePunctuation = false;
            for (++offset; offset <= endIndex; ++offset)
            {
                char c = identifier[offset];
                bool wordBreak = false;
                // a punctuation or symbol character?
                if (Char.IsWhiteSpace(c) || Char.IsPunctuation(c) || Char.IsSymbol(c))
                {
                    // this is a word break
                    wordBreak = true;
                    nonUpperCaseCharactersSincePunctuation = false;
                    // end of identifier??
                    if (++offset >= identifier.Length)
                    {
                        // just append the dash/underscore/space and break out of the loop
                        output.Append(c);
                        break;
                    }
                    // skip the dash/underscore/space and continue
                    c = identifier[offset];
                }
                // an upper-case character in a pascal, http, spreadsheet, camel, or mixed string with a case transition?
                else if (Char.IsUpper(c) && (casing == CasingStyle.Pascal || casing == CasingStyle.Train || casing == CasingStyle.Spreadsheet || casing == CasingStyle.Camel || (casing == CasingStyle.Mixed && nonUpperCaseCharactersSincePunctuation)))
                {
                    // this is also a word break
                    wordBreak = true;
                }
                // keep track of whether or not we've seen any non-upper-case characters since the last punctuated break
                if (!Char.IsUpper(c)) nonUpperCaseCharactersSincePunctuation = true;
                // word break here?
                if (wordBreak)
                {
                    switch (desiredStyle)
                    {
                        case CasingStyle.Pascal:
                        case CasingStyle.Camel:
                            output.Append(Char.ToUpperInvariant(c));
                            break;
                        case CasingStyle.Kebab:
                            output.Append('-');
                            output.Append(Char.ToLowerInvariant(c));
                            break;
                        case CasingStyle.Snake:
                            output.Append('_');
                            output.Append(Char.ToLowerInvariant(c));
                            break;
                        case CasingStyle.Train:
                        case CasingStyle.Cobol:
                            output.Append('-');
                            output.Append(Char.ToUpperInvariant(c));
                            break;
                        case CasingStyle.Lower:
                            output.Append(' ');
                            output.Append(Char.ToLowerInvariant(c));
                            break;
                        case CasingStyle.Spreadsheet:
                        case CasingStyle.Upper:
                            output.Append(' ');
                            output.Append(Char.ToUpperInvariant(c));
                            break;
                        case CasingStyle.Macro:
                            output.Append('_');
                            output.Append(Char.ToUpperInvariant(c));
                            break;
                    }
                }
                else // middle of word
                {
                    switch (desiredStyle)
                    {
                        case CasingStyle.Lower:
                        case CasingStyle.Pascal:
                        case CasingStyle.Train:
                        case CasingStyle.Spreadsheet:
                        case CasingStyle.Camel:
                        case CasingStyle.Kebab:
                        case CasingStyle.Snake:
                            output.Append(Char.ToLowerInvariant(c));
                            break;
                        case CasingStyle.Upper:
                        case CasingStyle.Cobol:
                        case CasingStyle.Macro:
                            output.Append(Char.ToUpperInvariant(c));
                            break;
                    }
                }
            }
            // handle trailing punctuation
            for (; offset < identifier.Length; ++offset)
            {
                char c = identifier[offset];
                // a punctuation or symbol character?
                if (Char.IsWhiteSpace(c) || Char.IsPunctuation(c) || Char.IsSymbol(c))
                {
                    // just append the punctuation/symbol
                    output.Append(c);
                }
            }
        }
        return output.ToString();
    }
}
