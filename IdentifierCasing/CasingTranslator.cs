using System;
using System.ComponentModel.Design;
using System.Diagnostics;
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
        CasingStyle style = CasingStyle.None;
        // *not* composed entirely of punctuation?  count as 'none' if it is (it's ambiguous)
        if (leadingPunctuation >= 0)
        {
            int lastNonPunctuation = identifier.LastIndexOfNotPunctuationOrWhitespace();
            System.Diagnostics.Debug.Assert(lastNonPunctuation >= 0);
            if (Char.IsUpper(identifier[leadingPunctuation])) style |= CasingStyle.FirstCharUpper;
            bool hasUpper = false;
            bool hasLower = false;
            bool hasPunct = false;
//            bool hasOtherPunctuation = false;
            bool wordLeadingCharIsUpper = false;
            bool wordLeadingCharIsLower = false;
            bool wordInternalCharIsUpper = false;
            bool wordInternalCharIsLower = false;
            bool prevDefiniteWordBreak = false;
            bool prevLeadingWordChar = false;
            for (int offset = leadingPunctuation + 1; offset < lastNonPunctuation; ++offset)
            {
                char ch = identifier[offset];
                bool isUpper = Char.IsUpper(ch);
                bool isLower = Char.IsLower(ch);
                bool isPunct = Char.IsWhiteSpace(ch) || Char.IsPunctuation(ch) || Char.IsSymbol(ch);
                bool isDash = (ch == '-');
                bool isSpace = (ch == ' ');
                bool isUnderscore = (ch == '_');
                if (isDash) style |= CasingStyle.WordSeparatorDash;
                else if (isSpace) style |= CasingStyle.WordSeparatorSpace;
                else if (isUnderscore) style |= CasingStyle.WordSeparatorUnderscore;
//                else if (isPunct) hasOtherPunctuation = true;
                hasUpper = hasUpper || isUpper;
                hasLower = hasLower || isLower;
                hasPunct = hasPunct || isPunct;
                // is this definitive information about leading word character casing?
                if (prevDefiniteWordBreak && isLower) wordLeadingCharIsLower = true;
                if (prevDefiniteWordBreak && isUpper) wordLeadingCharIsUpper = true;
                // is this definitive information about internal word character casing?
                if (prevLeadingWordChar && isLower) wordInternalCharIsLower = true;
                if (prevLeadingWordChar && isUpper) wordInternalCharIsUpper = true;
                prevLeadingWordChar = prevDefiniteWordBreak;
                prevDefiniteWordBreak = isPunct;
            }
            // if there is conflicting information about word leading character casing, return None because we can't tell
            if (wordLeadingCharIsLower && wordLeadingCharIsUpper) style = CasingStyle.None;
            else if (wordLeadingCharIsLower && hasUpper) style |= CasingStyle.WordSeparatorCaseSwitch;
            else if (wordLeadingCharIsUpper && hasLower) style |= CasingStyle.WordSeparatorCaseSwitch;
            else if (hasLower && hasUpper) style |= CasingStyle.WordSeparatorCaseSwitch;    // in this case, we can't be certain, but either we're correct or the string is indeterminate
            // if there is conflicting information about word internal character casing, return None because we can't tell
            if (wordInternalCharIsLower && wordInternalCharIsUpper) style = CasingStyle.None;
            else if (wordInternalCharIsLower) style &= ~CasingStyle.WordBodyCharsUpper;
            else if (wordInternalCharIsUpper) style |= CasingStyle.WordBodyCharsUpper;
        }
#if DEBUG
        Debug.Assert(style == OldDetectIdentifierCasing(identifier));
#endif
        return style;
    }
#if DEBUG
    /// <summary>
    /// Detects the casing of the specified identifier.
    /// </summary>
    /// <param name="identifier">The identifier to check.</param>
    /// <returns>The <see cref="CasingStyle"/> of the specified string.</returns>
    private static CasingStyle OldDetectIdentifierCasing(string identifier)
    {
        if (string.IsNullOrEmpty(identifier)) throw new ArgumentException("The specified identifier is empty.  Identifier strings must have at least one character.");
        int leadingPunctuation = identifier.IndexOfNotPunctuationWhitespaceOrSymbol();
        // composed entirely of punctuation?  count that as 'mixed' (it's ambiguous)
        if (leadingPunctuation < 0) return CasingStyle.None;
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
        if (conditions >= 2) return CasingStyle.None;
        // NOTE that there is a special edge case here where there are characters that are in the identifier that are neither upper nor lower case--that's OK, we'll just consider it all upper-case in that situation.
        if (hasDashes)
        {
            return hasLower ? CasingStyle.Kebab : CasingStyle.Cobol;
        }
        return hasLower ? CasingStyle.Snake : CasingStyle.Macro;
    }
#endif
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
            System.Diagnostics.Debug.Assert(DetectIdentifierCasing(systemCasedIdentifier) == CasingStyle.Pascal);
            writer.Write(systemCasedIdentifier);
            return;
        }
        // invalid casing?
        System.Diagnostics.Debug.Assert(desiredStyle != CasingStyle.Pascal);  // this should have been handled above
        if (desiredStyle == CasingStyle.None) throw new ArgumentException("The specified IdentifierCasing is not supported as target casing format!", nameof(desiredStyle));
        
        bool firstCharUpper = (desiredStyle & CasingStyle.FirstCharUpper) != 0;
        bool wordBodyUpperCase = (desiredStyle & CasingStyle.WordBodyCharsUpper) != 0;
        bool wordStartUpperCase = (desiredStyle & CasingStyle.WordSeparatorCaseSwitch) != 0 ? !wordBodyUpperCase : wordBodyUpperCase;
        char ch = systemCasedIdentifier[0];
        writer.Write(firstCharUpper ? Char.ToUpperInvariant(ch) : Char.ToLowerInvariant(ch));
        for (int offset = 1; offset < systemCasedIdentifier.Length; ++offset)
        {
            ch = systemCasedIdentifier[offset];
            // is this character the first in a new word?
            if (Char.IsUpper(ch))
            {
                System.Diagnostics.Debug.Assert(Char.IsUpper(ch));
                if ((desiredStyle & CasingStyle.WordSeparatorDash) != 0) writer.Write('-');
                if ((desiredStyle & CasingStyle.WordSeparatorSpace) != 0) writer.Write(' ');
                if ((desiredStyle & CasingStyle.WordSeparatorUnderscore) != 0) writer.Write('_');
                writer.Write(wordStartUpperCase ? ch : Char.ToLowerInvariant(ch));
            }
            else if (Char.IsLower(ch)) // not a word break
            {
                System.Diagnostics.Debug.Assert(!Char.IsUpper(ch));
                writer.Write(wordBodyUpperCase ? Char.ToUpperInvariant(ch) : ch);
            }
            else // other chars just get copied (these may screw up translation to/from some formats)
            {
                writer.Write(ch);
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
        if (desiredStyle == CasingStyle.None) throw new ArgumentException("The specified IdentifierCasing is not supported as target casing format!", nameof(desiredStyle));
        // find out what casing it's currently in
        CasingStyle casing = DetectIdentifierCasing(identifier);
        // is it already in the correct casing?
        if (casing == desiredStyle) return identifier;
        // allocate an output buffer with enough space for most cases
        StringBuilder output = new(identifier.Length * 4 / 3);
        bool firstCharUpper = (desiredStyle & CasingStyle.FirstCharUpper) != 0;
        bool wordBodyUpperCase = (desiredStyle & CasingStyle.WordBodyCharsUpper) != 0;
        bool wordStartUpperCase = (desiredStyle & CasingStyle.WordSeparatorCaseSwitch) != 0 ? !wordBodyUpperCase : wordBodyUpperCase;
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
                // handle the first non-punctuation character specially
                char ch = identifier[offset];
                output.Append(firstCharUpper ? Char.ToUpperInvariant(ch) : Char.ToLowerInvariant(ch));
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
                else if (Char.IsUpper(c) && (casing == CasingStyle.Pascal || casing == CasingStyle.Train || casing == CasingStyle.Spreadsheet || casing == CasingStyle.Camel || (casing == CasingStyle.None && nonUpperCaseCharactersSincePunctuation)))
                {
                    // this is also a word break
                    wordBreak = true;
                }
                // keep track of whether or not we've seen any non-upper-case characters since the last punctuated break
                if (!Char.IsUpper(c)) nonUpperCaseCharactersSincePunctuation = true;
                // word break here?
                if (wordBreak)
                {
                    if ((desiredStyle & CasingStyle.WordSeparatorDash) != 0) output.Append('-');
                    if ((desiredStyle & CasingStyle.WordSeparatorSpace) != 0) output.Append(' ');
                    if ((desiredStyle & CasingStyle.WordSeparatorUnderscore) != 0) output.Append('_');
                    output.Append(wordStartUpperCase ? Char.ToUpperInvariant(c) : Char.ToLowerInvariant(c));
                }
                else // middle of word
                {
                    output.Append(wordBodyUpperCase ? Char.ToUpperInvariant(c) : Char.ToLowerInvariant(c));
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
        return output.ToString();
    }
}
