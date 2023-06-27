namespace AssociationRegistry.Vereniging;

using Exceptions;
using Framework;

public record AdresId
{
    public const string DataVlaanderenAdresPrefix = "https://data.vlaanderen.be/id/adres/";
    public Adresbron Adresbron { get; }
    public string Bronwaarde { get; }

    private AdresId(Adresbron adresbron, string bronwaarde)
    {
        Adresbron = adresbron;
        Bronwaarde = bronwaarde;
    }

    public static AdresId Create(Adresbron adresbron, string bronwaarde)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        Throw<IncompleteAdresId>.If(adresbron is null);
        Throw<IncompleteAdresId>.If(string.IsNullOrWhiteSpace(bronwaarde));

        Throw<InvalidBronwaardeForAR>.If(adresbron == Adresbron.AR && !IsValidArBronwaarde(bronwaarde));

        return new AdresId(adresbron!, bronwaarde);
    }

    private static bool IsValidArBronwaarde(string bronwaarde)
        => bronwaarde.StartsWith(DataVlaanderenAdresPrefix);

    public static AdresId? Hydrate(Adresbron adresbron, string bronwaarde)
        => new(adresbron, bronwaarde);
}
