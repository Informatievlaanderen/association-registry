namespace AssociationRegistry.Test.StringNormalizerTests;

using Normalizers;
using Vereniging;
using Xunit;

public class WhenNormalizingStrings
{
    [InlineData("België", "België")]
    [InlineData("Kerks-traat", "Kerkstraat")]
    [InlineData("Kerkstraat", "Kerkstraat")]
    [InlineData("Kerkstr^^aat", "Kerkstraat")]
    [InlineData("Kerkstraat---", "KerKstraat")]
    [InlineData("Kerkstraat", "Kerkstraat")] // Identical strings
    [InlineData("Kerkstraat", "kerkstraat")] // Different casing
    [InlineData("Kerkstraat", "KerkStraat")] // Mixed casing
    [InlineData("Kerkstraat", "KERKSTRAAT")] // Uppercase
    [InlineData("Kerkstraat", "Kerkstraat ")] // Trailing space
    [InlineData("Kerkstraat", " Kerkstraat")] // Leading space
    [InlineData("Kerkstraat", "Kerkstraat  ")] // Multiple trailing spaces
    [InlineData("Kerkstraat", "  Kerkstraat")] // Multiple leading spaces
    [InlineData("Kerkstraat", "Kerk straat")] // Space in the middle
    [InlineData("Kerkstraat", "Kerk-straat")] // Hyphen inserted
    [InlineData("Kerkstraat", "Kerkstraat!")] // Exclamation mark added
    [InlineData("Kerkstraat", "Kerkstraat?")] // Question mark added
    [InlineData("Kerkstraat", "Kerkstraat.")] // Period added
    [InlineData("Kerkstraat", "Kerkstraat,")] // Comma added
    [InlineData("Kerkstra'at", "Kerkstraat")] // Apostrophe and 's' added
    [InlineData("Kerkstraat", "Kerkstr#aat")] // Special character '#' inserted
    [InlineData("Kerkstraat", "Kerkstraat_")] // Underscore appended
    [InlineData("Kerkstraat", "Kerkstraat-")] // Hyphen appended
    [InlineData("Kerkstraat", "Kerkstraat ")] // Trailing space
    [InlineData("Kerkstraat", "Kerkstraat  ")] // Multiple trailing spaces
    [InlineData("Kerkstraat", "Kerkstraat\n")] // Newline character appended
    [InlineData("Kerkstraat", "Kerkstraat\t")] // Tab character appended
    [InlineData("Kerkstraat", "Kerkstraat\r\n")] // Carriage return and newline
    [InlineData("Kerkstraat", "Kerkstraat\x00")] // Null character appended
    [InlineData("Kerkstraat", "Kérkstraat")] // Accented 'é' character
    [InlineData("Kerkstraat", "Kerkstrààt")] // Accented 'à' characters
    [InlineData("Kerkstraat", "Kerkstráát")] // Accented 'á' characters
    [InlineData("Kerkstraat", "Kerkstrâat")] // Accented 'â' character
    [InlineData("Kerkstraat", "Kerksträat")] // Umlaut 'ä' character
    [InlineData("Kerkstraat", "Kerkstraat!@#")] // Special characters appended
    [InlineData("Kerkstraat", "Kerkstraat   ")] // Multiple trailing spaces
    [InlineData("Kerkstraat", "   Kerkstraat")] // Multiple leading spaces
    [InlineData("Kerkstraat", "K e r k s t r a a t")] // Spaces between all letters
    [InlineData("Kerkstraat", "KERKSTRAAT")] // All uppercase
    [InlineData("Kerkstraat", "kerkstraat")] // All lowercase
    [InlineData("Kerkstraat", "Kerkstraat\r")] // Carriage return appended
    [InlineData("Kerkstraat", "Kerkstraat\f")] // Form feed character appended
    [InlineData("Kerkstraat", "Kerkstraat\b")] // Backspace character appended
    [InlineData("Kerkstraat", "Kerkstraat\"")] // Double quote appended
    [InlineData("Kerkstraat", "Kerkstraat'")] // Single quote appended
    [InlineData("Kerkstraat", "Kerkstraat\\")] // Backslash appended
    [Theory]
    public void Then_Equalizing_Returns_True(string input1, string input2)
    {
        var normalizer = new AdresComponentNormalizer();
        var normalizedString1 = normalizer.NormalizeString(input1);
        var normalizedString2 = normalizer.NormalizeString(input2);

        Assert.True(normalizedString1 == normalizedString2);
    }
}
