namespace AssociationRegistry.Grar.Models;

using System.Text.RegularExpressions;

public record PostalInformationResponse(
    string Postcode,
    string Gemeentenaam,
    string[] Postnamen)
{
    public bool HasSinglePostnaam
        => Postnamen.Length == 1;

    public string? GetPostnaamWhenEqualsGemeentenaam(string origineleGemeenteNaamClean) => Postnamen.SingleOrDefault(
        sod => sod.Equals(origineleGemeenteNaamClean, StringComparison.InvariantCultureIgnoreCase));
}

public static class GemeentenaamSuffixCleaner
{
    public static string RemovePartsBetweenBraces(string input)
    {
        return new Regex("\\(.*?\\)").Replace(input, "").Trim();}
    }
}
