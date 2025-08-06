namespace AssociationRegistry;

using DecentraalBeheer.Vereniging.Adressen;
using System.Text.RegularExpressions;
using Vereniging;

public class GemeenteNaamSuffixCleanerRegex
{
    private readonly Regex _regex;
    private const string RemoveBracketsAndContent = "\\(.*?\\)";

    private GemeenteNaamSuffixCleanerRegex()
    {
        _regex = new Regex(RemoveBracketsAndContent);
    }

    public static GemeenteNaamSuffixCleanerRegex Instance = new();

    public string Clean(Gemeentenaam gemeentenaam)
        => _regex.Replace(gemeentenaam.Naam, "").Trim();
}
