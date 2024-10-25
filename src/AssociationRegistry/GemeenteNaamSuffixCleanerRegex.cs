namespace AssociationRegistry;

using System.Text.RegularExpressions;

public class GemeenteNaamSuffixCleanerRegex
{
    private readonly Regex _regex;
    private const string RemoveBracketsAndContent = "\\(.*?\\)";

    private GemeenteNaamSuffixCleanerRegex()
    {
        _regex = new Regex(RemoveBracketsAndContent);
    }

    public static GemeenteNaamSuffixCleanerRegex Instance = new();

    public string Clean(string gemeentenaam)
        => _regex.Replace(gemeentenaam, "").Trim();
}
