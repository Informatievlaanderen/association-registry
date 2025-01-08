namespace AssociationRegistry.Vereniging;

using Acties.Lidmaatschappen.VoegLidmaatschapToe;
using Acties.Lidmaatschappen.WijzigLidmaatschap;

public record Lidmaatschap
{
    public LidmaatschapId LidmaatschapId { get; init; }
    public VCode AndereVereniging { get; init; }
    public string AndereVerenigingNaam { get; init; }
    public Geldigheidsperiode Geldigheidsperiode { get; init; }
    public LidmaatschapIdentificatie Identificatie { get; init; }
    public LidmaatschapBeschrijving Beschrijving { get; init; }

    private Lidmaatschap(
        LidmaatschapId lidmaatschapId,
        VCode andereVereniging,
        string andereVerenigingNaam,
        Geldigheidsperiode geldigheidsperiode,
        LidmaatschapIdentificatie identificatie,
        LidmaatschapBeschrijving beschrijving)
    {
        LidmaatschapId = lidmaatschapId;
        AndereVereniging = andereVereniging;
        AndereVerenigingNaam = andereVerenigingNaam;
        Geldigheidsperiode = geldigheidsperiode;
        Identificatie = identificatie;
        Beschrijving = beschrijving;
    }

    public static Lidmaatschap Hydrate(
        int lidmaatschapId,
        VCode andereVereniging,
        string andereVerenigingNaam,
        Geldigheidsperiode geldigheidsperiode,
        string identificatie,
        string beschrijving)
        => new(new LidmaatschapId(lidmaatschapId), andereVereniging,
               andereVerenigingNaam,
               geldigheidsperiode,
               LidmaatschapIdentificatie.Hydrate(identificatie),
               LidmaatschapBeschrijving.Hydrate(beschrijving));

    public virtual bool Equals(Lidmaatschap? other)
    {
        if (ReferenceEquals(objA: null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        if (AndereVereniging != other.AndereVereniging)
            return false;

        if (AndereVerenigingNaam != other.AndereVerenigingNaam)
            return false;

        if (!Geldigheidsperiode.Van.Equals(other.Geldigheidsperiode.Van))
            return false;

        if (!Geldigheidsperiode.Tot.Equals(other.Geldigheidsperiode.Tot))
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
               lidmaatschap.AndereVerenigingNaam,
               lidmaatschap.Geldigheidsperiode,
               lidmaatschap.Identificatie,
               lidmaatschap.Beschrijving);

    public Lidmaatschap Wijzig(WijzigLidmaatschapCommand.TeWijzigenLidmaatschap teWijzigenLidmaatschap)
        => this with
        {
            Geldigheidsperiode = new Geldigheidsperiode(
                teWijzigenLidmaatschap.GeldigVan is null ? Geldigheidsperiode.Van : new GeldigVan(teWijzigenLidmaatschap.GeldigVan),
                teWijzigenLidmaatschap.GeldigTot is null ? Geldigheidsperiode.Tot : new GeldigTot(teWijzigenLidmaatschap.GeldigTot)),
            Identificatie =  teWijzigenLidmaatschap.Identificatie ?? Identificatie,
            Beschrijving = teWijzigenLidmaatschap.Beschrijving ?? Beschrijving,
        };
}
