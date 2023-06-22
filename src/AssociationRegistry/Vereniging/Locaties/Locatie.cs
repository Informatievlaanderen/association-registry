namespace AssociationRegistry.Vereniging;

public record Locatie
{
    public int LocatieId { get; set; }
    public string? Naam { get; }
    public bool Hoofdlocatie { get; }
    public string Locatietype { get; }
    public AdresId? AdresId { get; }
    public Adres? Adres { get; }

    private Locatie(string? naam, bool hoofdlocatie, string locatietype, AdresId? adresId, Adres? adres)
    {
        Naam = naam;
        AdresId = adresId;
        Adres = adres;
        Hoofdlocatie = hoofdlocatie;
        Locatietype = locatietype;
    }

    public static Locatie Create(string? naam, bool hoofdlocatie, string locatieType, AdresId? adresId = null, Adres? adres = null)
        => new(naam, hoofdlocatie, locatieType, adresId, adres);

    public static Locatie Hydrate(int locatieId, string? naam, string straatnaam, string huisnummer, string? busnummer, string postcode, string gemeente, string land, bool hoofdlocatie, string locatieType, AdresId? adresId = null)
        => new(naam, hoofdlocatie, locatieType, adresId, Adres.Create(straatnaam, huisnummer, busnummer, postcode, gemeente, land)) { LocatieId = locatieId };

    public virtual bool Equals(Locatie? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        return Naam == other.Naam &&
               Locatietype == other.Locatietype &&
               Equals(
                   AdresId,
                   other.AdresId);
    }

    public override int GetHashCode()
        => HashCode.Combine(LocatieId, Naam, Hoofdlocatie, Locatietype, AdresId, Adres);

}
