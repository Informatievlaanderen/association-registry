namespace AssociationRegistry.Vereniging;

using Acties.VoegLidmaatschapToe;
using Acties.WijzigLidmaatschap;

public record Lidmaatschap
{
    public LidmaatschapId LidmaatschapId { get; init; }
    public VCode AndereVereniging { get; init; }
    public Geldigheidsperiode Geldigheidsperiode { get; init; }
    public string Identificatie { get; init; }
    public string Beschrijving { get; init; }

    private Lidmaatschap(
        LidmaatschapId lidmaatschapId,
        VCode andereVereniging,
        Geldigheidsperiode geldigheidsperiode,
        string identificatie,
        string beschrijving)
    {
        LidmaatschapId = lidmaatschapId;
        AndereVereniging = andereVereniging;
        Geldigheidsperiode = geldigheidsperiode;
        Identificatie = identificatie;
        Beschrijving = beschrijving;
    }

    public static Lidmaatschap Hydrate(
        int lidmaatschapId,
        VCode andereVereniging,
        Geldigheidsperiode geldigheidsperiode,
        string identificatie,
        string beschrijving)
        => new(new LidmaatschapId(lidmaatschapId), andereVereniging, geldigheidsperiode, identificatie, beschrijving);

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

    public static Lidmaatschap Create(
        LidmaatschapId lidmaatschapId,
        VoegLidmaatschapToeCommand.ToeTeVoegenLidmaatschap lidmaatschap)
        => new(lidmaatschapId,
               lidmaatschap.AndereVereniging,
               lidmaatschap.Geldigheidsperiode,
               lidmaatschap.Identificatie,
               lidmaatschap.Beschrijving);

    public Lidmaatschap Wijzig(WijzigLidmaatschapCommand.TeWijzigenLidmaatschap teWijzigenLidmaatschap)
        => this with
        {
            Geldigheidsperiode = teWijzigenLidmaatschap.Geldigheidsperiode,
            Identificatie = teWijzigenLidmaatschap.Identificatie,
            Beschrijving = teWijzigenLidmaatschap.Beschrijving,
        };
}
