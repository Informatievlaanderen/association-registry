namespace AssociationRegistry.Vereniging;

using Exceptions;
using Framework;

public record Locatie
{
    public int LocatieId { get; init; }
    public string? Naam { get; init; }
    public bool IsPrimair { get; init; }
    public Locatietype Locatietype { get; init; }
    public AdresId? AdresId { get; init; }
    public Adres? Adres { get; init; }

    private Locatie(string? naam, bool isPrimair, string locatietype, AdresId? adresId, Adres? adres)
    {
        Naam = naam;
        AdresId = adresId;
        Adres = adres;
        IsPrimair = isPrimair;
        Locatietype = locatietype;
    }

    public static Locatie Create(string? naam, bool isPrimair, string locatieType, AdresId? adresId = null, Adres? adres = null)
    {
        Throw<MissingAdres>.If(adresId is null && adres is null);

        return new Locatie(naam, isPrimair, locatieType, adresId, adres);
    }

    public static Locatie Hydrate(int locatieId, string? naam, bool isPrimair, string locatieType, Adres? adres, AdresId? adresId)
        => new(naam, isPrimair, locatieType, adresId, adres) { LocatieId = locatieId };

    public virtual bool Equals(Locatie? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        if (Naam != other.Naam)
            return false;

        if (Locatietype != other.Locatietype)
            return false;

        return HasSameAdresId(other.AdresId) ||
               HasSameAdres(other.Adres);
    }

    private bool HasSameAdresId(AdresId? other)
    {
        if (AdresId is null && other is null) return false;

        return AdresId == other;
    }

    private bool HasSameAdres(Adres? other)
    {
        if (Adres is null && other is null) return false;

        return Adres == other;
    }

    public override int GetHashCode()
        => HashCode.Combine(LocatieId, Naam, IsPrimair, Locatietype, AdresId, Adres);
}
