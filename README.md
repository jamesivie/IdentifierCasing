# Overview
IdentifierCasing is a .NET library that provides tools for translating identifiers between different casing styles in order to comply with diverse contextual casing styles.

## Casing Styles
Style | Description | Outputtable
---|---|---
Mixed | Not valid as an output format, but indicates that a string that was read has two or more of the following: mixed case, a dash, an underscore. The system will do its best to translate these, though the same issues exist here with single-character words.  The system will assume that anything between dashes and underscores is a single word, unless the case changes from lower to upper. | N
Pascal | First letter capitalized and the first letter of each subsequent word also capitalized (widely applicable). | Y
Camel | First letter lower-case and the first letter of each subsequent word capitalized (most applicable for Java or JavaScript function names, variable names in most languages).   | Y
Kebab | All lower-case with dashes indicating word breaks (most applicable in formats like JSON, and XML). | Y
Cobol | All upper-case with dashes indicating word breaks (most applicable in formats like YML, JSON, and XML). | Y
Snake | All lower-case with underscores indicating word breaks (most applicable for filenames). | Y
Macro | All upper-case with underscores indicating word breaks (most applicable for constants). | Y
Lower | Lower case, with a space between each word and all lower-case (widely applicable, but invalid in most programming languages). | Y
Upper | Upper case, with a space between each word and all upper-case (widely applicable, but invalid in most programming languages). | Y
Train | Pascal casing with dashes in addition to casing transitions: First letter capitalized with a dash followed by the first letter of each subsequent word also capitalized (HTTP Header names use this format). Note that the dashes in this format are redundant--they are not needed to detect word breaks. | Y
Spreadsheet | Pascal casing with spaces in addition to casing transitions: First letter capitalized with a space followed by the first letter of each subsequent word also capitalized (CSV headers use this format). Note that the spaces in this format are redundant--they are not needed to detect word breaks. | Y

All casing styles except for `Mixed` can be losslessly converted between different styles.

### Sample Usage
[//]: # (TranslationSample)
```csharp
class DataInput
{
    /// <summary>
    /// Translates the keys of a list of name-value pairs to a desired casing style.
    /// </summary>
    /// <param name="input">The input of <see cref="KeyValuePair{TKey, TValue}"/>s.</param>
    /// <param name="desiredStyle">The <see cref="IdentifierCasing.CasingStyle"/> to translate keys to.</param>
    public static IEnumerable<KeyValuePair<string, string>> TranslateKeyValuePairs(IEnumerable<KeyValuePair<string, string>> input, IdentifierCasing.CasingStyle desiredStyle)
    {
        foreach (KeyValuePair<string, string> pair in input)
        {
            yield return new(IdentifierCasing.CasingTranslator.Translate(pair.Key, desiredStyle), pair.Value);
        }
    }
    /// <summary>
    /// Translates a streaming list of identifiers to a desired casing style.
    /// </summary>
    /// <param name="writer">A <see cref="TextWriter"/> to write the results into.</param>
    /// <param name="reader">A <see cref="TextReader"/> to read identifiers from.</param>
    /// <param name="desiredStyle">The <see cref="IdentifierCasing.CasingStyle"/> to translate keys to.</param>
    public static void StreamingTranslate(TextWriter writer, TextReader reader, IdentifierCasing.CasingStyle desiredStyle)
    {
        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            IdentifierCasing.CasingTranslator.WriteCasedIdentifier(writer, desiredStyle, line);
            writer.WriteLine();
        }
    }
}
```

# Library Information

## Author and License
AmbientServices is written and maintained by James Ivie.

AmbientServices is licensed under [MIT](https://opensource.org/licenses/MIT).

## Language and Tools
AmbientServices is written in C#, using .NET Standard 2.0, .NET Core 3.1, and .NET 5.0.  Unit tests are written in .NET 5.0.

The code can be built using either Microsoft Visual Studio 2017+, Microsoft Visual Studio Code, or .NET Core command-line utilities.

Binaries are available at https://www.nuget.org/packages/AmbientServices.

## Contributions
Contributions are welcome under the following conditions:
1. enhancements are consistent with the overall scope of the project
2. no new assembly dependencies are introduced
3. code coverage by unit tests cover all new lines and conditions whenever possible
4. documentation (both inline and here) is updated appropriately
5. style for code and documentation contributions remains consistent

## Status
[![.NET](https://github.com/AmbientServices/AmbientServices/actions/workflows/dotnet.yml/badge.svg)](https://github.com/AmbientServices/AmbientServices/actions/workflows/dotnet.yml)