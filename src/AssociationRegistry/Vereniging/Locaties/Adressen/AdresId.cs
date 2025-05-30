namespace AssociationRegistry.Vereniging;

using Events;
using Exceptions;
using Framework;

public record AdresId : IAdresId
{
    public const string DataVlaanderenAdresPrefix = "https://data.vlaanderen.be/id/adres/";
    public IAdresbron Adresbron { get; }
    public string Bronwaarde { get; }

    private AdresId(Adresbron adresbron, string bronwaarde)
    {
        Adresbron = adresbron;
        Bronwaarde = bronwaarde;
    }

    public static AdresId Create(Adresbron adresbron, string bronwaarde)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        Throw<AdresIdIsIncompleet>.If(adresbron is null);
        Throw<AdresIdIsIncompleet>.If(string.IsNullOrWhiteSpace(bronwaarde));

        Throw<BronwaardeVoorAdresIsOngeldig>.If(adresbron == AssociationRegistry.Vereniging.Adresbron.AR && !IsValidArBronwaarde(bronwaarde));

        return new AdresId(adresbron!, bronwaarde);
    }

    private static bool IsValidArBronwaarde(string bronwaarde)
        => bronwaarde.StartsWith(DataVlaanderenAdresPrefix);

    public static AdresId Hydrate(Adresbron adresbron, string bronwaarde)
        => new(adresbron, bronwaarde);
    public static AdresId Hydrate(Registratiedata.AdresId adresId)
    {
        var (broncode, bronwaarde) = adresId;

        return Hydrate(broncode, bronwaarde);
    }

    public bool Equals(Registratiedata.AdresId adresId)
        => this == adresId;

    public static bool operator ==(AdresId first, Registratiedata.AdresId second)
        => first.Adresbron.Code == second.Broncode && first.Bronwaarde == second.Bronwaarde;

    public static bool operator !=(AdresId first, Registratiedata.AdresId second)
        => first.Adresbron.Code != second.Broncode || first.Bronwaarde != second.Bronwaarde;

    public static AdresId Hydrate(AdresWerdOvergenomenUitAdressenregister adres)
        => Create(adres.AdresId.Broncode, adres.AdresId.Bronwaarde);

    public override string ToString()
        => new Uri(Bronwaarde).Segments.Last();
}
