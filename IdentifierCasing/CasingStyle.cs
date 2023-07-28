
namespace IdentifierCasing;

/// <summary>
/// An enumeration of possible identifier casing styles.
/// </summary>
/// <remarks>
/// Note that in each case, the word breaks can be determined programatically, and simple code can be used to translate between formats with no hinting required.  This is crucial for interoperability.
/// Note that single-word scenarios can be ambiguous.  For example "ABC" could be Pascal casing for A-B-C, or it could be UpperDash or UpperUnderscore casing for Abc.  
/// Given the apparent benefits of this system and relative obscurity of this edge case, we hereby forbid the use of identifiers consisting of only one-letter words, so "ABC" becomes definitively the UpperDash or UpperUnderscore representation of Abc.
/// </remarks>
public enum CasingStyle
{
    /// <summary>
    /// Not valid as an output format, but indicates that a string that was read has two or more of the following: mixed case, a dash, an underscore.
    /// The system will do its best to translate these, though the same issues exist here with single-character words.  The system will assume that anything between dashes and underscores is a single word, unless the case changes from lower to upper.
    /// </summary>
    Mixed = -1,
    /// <summary>
    /// Indicates that strings should be converted to pascal casing, with the first letter capitalized and the first letter of each subsequent word also capitalized (widely applicable).
    /// </summary>
    Pascal,
    /// <summary>
    /// Indicates that strings should be converted to camel casing, with the first letter lower-case and the first letter of each subsequent word capitalized (most applicable for Java or JavaScript function names, variable names in most languages).  
    /// </summary>
    Camel,
    /// <summary>
    /// Indicates that strings should be converted to all lower-case with dashes indicating word breaks (most applicable in formats like JSON and XML).
    /// </summary>
    Kebab,
    /// <summary>
    /// Indicates that strings should be converted to all upper-case with dashes indicating word breaks (most applicable in formats like JSON and XML).
    /// </summary>
    Cobol,
    /// <summary>
    /// Indicates that strings should be converted to all lower-case with underscores indicating word breaks (most applicable for filenames).
    /// </summary>
    Snake,
    /// <summary>
    /// Indicates that strings should be converted to all upper-case with underscores indicating word breaks (most applicable for constants).
    /// </summary>
    Macro,
    /// <summary>
    /// Indicates that strings should be converted to lower casing, with a space between each word and all lower-case (widely applicable, but invalid in most programming languages).
    /// </summary>
    Lower,
    /// <summary>
    /// Indicates that strings should be converted to upper casing, with a space between each word and all upper-case (widely applicable, but invalid in most programming languages).
    /// </summary>
    Upper,
    /// <summary>
    /// Indicates that strings should be converted to pascal casing with dashes, with the first letter capitalized and a dash followed by the first letter of each subsequent word also capitalized (HTTP Header names use this format).
    /// Note that the dashes in this format are redundant--they are not needed to detect word breaks.
    /// </summary>
    Train,
    /// <summary>
    /// Indicates that strings should be converted to pascal casing with spaces, with the first letter capitalized and a space followed by the first letter of each subsequent word also capitalized (CSV headers use this format).
    /// Note that the spaces in this format are redundant--they are not needed to detect word breaks.
    /// </summary>
    Spreadsheet,
}
