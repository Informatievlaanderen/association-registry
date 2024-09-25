namespace AssociationRegistry.Normalizers;

using System.Text;
using System.Text.RegularExpressions;

public class StringStringNormalizer : IStringNormalizer
{
    public string NormalizeString(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        // Convert to lowercase
        input = input.ToLowerInvariant();

        // Normalize to decompose accented characters
        input = input.Normalize(NormalizationForm.FormD);

        // Remove diacritics
        input = Regex.Replace(input, @"\p{Mn}", "");

        // Remove all non-alphanumeric characters
        input = Regex.Replace(input, "[^a-z0-9]", string.Empty);

        return input;
    }
}

public interface IStringNormalizer
{
    public string NormalizeString(string input);
}


