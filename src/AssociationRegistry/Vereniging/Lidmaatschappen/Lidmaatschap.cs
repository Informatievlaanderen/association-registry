namespace AssociationRegistry.Vereniging;

public record Lidmaatschap
{
    public int LidmaatschapId { get; init; }
    public VCode AndereVereniging { get; init; }
    public Geldigheidsperiode Geldigheidsperiode { get; init; }
    public string Identificatie { get; init; }
    public string Beschrijving { get; init; }

    private Lidmaatschap(VCode andereVereniging, Geldigheidsperiode geldigheidsperiode, string identificatie, string beschrijving)
    {
        AndereVereniging = andereVereniging;
        Geldigheidsperiode = geldigheidsperiode;
        Identificatie = identificatie;
        Beschrijving = beschrijving;
    }

    public static Lidmaatschap Create(
        VCode andereVereniging,
        Geldigheidsperiode geldigheidsperiode,
        string identificatie,
        string beschrijving)
    {
        return new Lidmaatschap(andereVereniging, geldigheidsperiode, identificatie, beschrijving);
    }

    public static Lidmaatschap Hydrate(
        int lidmaatschapId,
        VCode andereVereniging,
        Geldigheidsperiode geldigheidsperiode,
        string identificatie,
        string beschrijving)
        => new(andereVereniging, geldigheidsperiode, identificatie, beschrijving)
        {
            LidmaatschapId = lidmaatschapId
        };

    public virtual bool Equals(Lidmaatschap? other)
    {
        if (ReferenceEquals(objA: null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        if (AndereVereniging != other.AndereVereniging)
            return false;

        if (Geldigheidsperiode != other.Geldigheidsperiode)
            return false;

        if (Identificatie != other.Identificatie)
            return false;

        if (Beschrijving != other.Beschrijving)
            return false;

        return true;
    }

    public override int GetHashCode()
        => HashCode.Combine(LidmaatschapId, AndereVereniging, Geldigheidsperiode, Identificatie, Beschrijving);
}
