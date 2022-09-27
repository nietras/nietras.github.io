using System.Globalization;

namespace ValidIdentifierSeparators;

// Initially I thought removing well-known non-useable characters would be
// interesting but decided against it to instead just brute forcing it.
static class CharExtensions
{
    // https://www.unicode.org/Public/15.0.0/ucd/UnicodeData.txt
    // The set of Unicode character categories containing non-rendering,
    // unknown, or incomplete characters.
    // !! Unicode.Format and Unicode.PrivateUse can NOT be included in
    // !! this set, because they may be (private-use) or do (format)
    // !! contain at least *some* rendering characters.
    static readonly UnicodeCategory[] NonRenderingCategories = new UnicodeCategory[]
    {
        UnicodeCategory.Control,
        UnicodeCategory.OtherNotAssigned,
        UnicodeCategory.Surrogate
    };

    // Char.IsWhiteSpace() includes the ASCII whitespace characters that
    // are categorized as control characters. Any other character is
    // printable, unless it falls into the non-rendering categories.
    public static bool IsMaybeUseable(this char c) =>
        !char.IsWhiteSpace(c) && !NonRenderingCategories.Contains(char.GetUnicodeCategory(c));
}
