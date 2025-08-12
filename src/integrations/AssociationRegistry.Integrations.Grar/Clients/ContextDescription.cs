namespace AssociationRegistry.Integrations.Grar.Clients;

public class ContextDescription
{
    private string Description { get; init; }

    public ContextDescription(string description)
    {
        Description = description;
    }

    public override string ToString() => Description;

    public static ContextDescription PostInfoDetail(string postcode) => new($"PostInfoDetail with postcode {postcode}");
    public static ContextDescription PostInfoLijst(string offset, string limit) => new($"PostInfoLijst with offset {offset} and limit {limit}");
    public static ContextDescription GetAddressById(string adresId) => new($"GetAddressById {adresId}");

    public static ContextDescription AdresMatch(string straatnaam, string huisnummer, string busnummer, string postcode, string gemeentenaam)
        => new($"AdresMatch straatnaam:{straatnaam}, huisnummer:{huisnummer}, busnummer:{busnummer}, postcode:{postcode}, gemeentenaam:{gemeentenaam}");
}
