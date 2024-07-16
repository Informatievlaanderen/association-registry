namespace AssociationRegistry.Vereniging;

using Events;
using Exceptions;
using Framework;

public record Locatie
{
    public int LocatieId { get; init; }
    public string Naam { get; init; }
    public bool IsPrimair { get; init; }
    public Locatietype Locatietype { get; init; }
    public AdresId? AdresId { get; init; }
    public Adres? Adres { get; init; }

    private Locatie(Locatienaam naam, bool isPrimair, string locatietype, AdresId? adresId, Adres? adres)
    {
        Naam = naam;
        AdresId = adresId;
        Adres = adres;
        IsPrimair = isPrimair;
        Locatietype = locatietype;
    }

    public static Locatie Create(Locatienaam naam, bool isPrimair, string locatieType, AdresId? adresId = null, Adres? adres = null)
    {
        Throw<AdresOfAdresIdMoetAanwezigZijn>.If(adresId is null && adres is null);

        return new Locatie(naam, isPrimair, locatieType, adresId, adres);
    }

    public static Locatie Hydrate(int locatieId, string naam, bool isPrimair, string locatieType, Adres? adres, AdresId? adresId)
        => new(Locatienaam.Hydrate(naam), isPrimair, locatieType, adresId, adres) { LocatieId = locatieId };

    public bool IsEquivalentTo(Locatie other)
    {
        if (Naam != other.Naam)
            return false;

        if (Locatietype != other.Locatietype)
            return false;

        return HasSameAdresId(other.AdresId) ||
               HasSameAdres(other.Adres);
    }

    public virtual bool Equals(Locatie? other)
    {
        if (ReferenceEquals(objA: null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        if (Naam != other.Naam)
            return false;

        if (Locatietype != other.Locatietype)
            return false;

        if (IsPrimair != other.IsPrimair)
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

    public Locatie Wijzig(string? naam = null, Locatietype? locatietype = null, bool? isPrimair = null, AdresId? adresId = null, Adres? adres = null)
    {
        if (adres is null && adresId is null)
            return Create(Locatienaam.Create(naam ?? Naam), isPrimair ?? IsPrimair, locatietype ?? Locatietype, AdresId, Adres) with { LocatieId = LocatieId };

        return Create(Locatienaam.Create(naam ?? Naam), isPrimair ?? IsPrimair, locatietype ?? Locatietype, adresId, adres) with { LocatieId = LocatieId };
    }

    public Locatie Wijzig(string? naam, bool? isPrimair)
        => Create(Locatienaam.Create(naam ?? Naam), isPrimair ?? IsPrimair, Locatietype, AdresId, Adres) with { LocatieId = LocatieId };

    public Locatie DecorateWithAdresDetail(AdresDetailUitAdressenregister decoratedAdres)
        => this with
        {
            Adres = Adres.Create(
                straatnaam: decoratedAdres.Adres.Straatnaam,
                huisnummer: decoratedAdres.Adres.Huisnummer,
                busnummer: decoratedAdres.Adres.Busnummer,
                postcode: decoratedAdres.Adres.Postcode,
                gemeente: decoratedAdres.Adres.Gemeente,
                Adres.Belgie
            ),
        };
}

public record Locatienaam
{
    public string Naam { get; }
    public static Locatienaam Empty => new(string.Empty);

    private Locatienaam(string naam)
    {
        Naam = naam ?? throw new ArgumentNullException(nameof(naam));
    }

    public static Locatienaam Create(string naam)
        => new(naam ?? string.Empty);

    public static Locatienaam Hydrate(string naam)
        => new(naam);
    public static implicit operator string(Locatienaam locatienaam) => locatienaam.Naam;
}
