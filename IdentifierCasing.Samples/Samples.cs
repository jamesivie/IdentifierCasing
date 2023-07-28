#region TranslationSample
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
#endregion
