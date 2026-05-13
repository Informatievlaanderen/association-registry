namespace AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;

using DecentraalBeheer.Vereniging.Exceptions;
using Framework;

public sealed record ErkenningsPeriode
{
    public DateOnly? Startdatum { get; }
    public DateOnly? Einddatum { get; }

    private ErkenningsPeriode(DateOnly? startdatum, DateOnly? einddatum)
    {
        Startdatum = startdatum;
        Einddatum = einddatum;
    }

    public static ErkenningsPeriode Create(DateOnly? startdatum, DateOnly? einddatum)
    {
        Throw<StartdatumLigtNaEinddatum>.If(
            startdatum.HasValue && einddatum.HasValue && startdatum.Value > einddatum.Value
        );

        return new ErkenningsPeriode(startdatum, einddatum);
    }

    public bool OverlapsWith(ErkenningsPeriode toeTeVoegenErkenningsPeriode)
    {
        var bestaandeStartDatum = Startdatum ?? DateOnly.MinValue;
        var bestaandeEinddatum = Einddatum ?? DateOnly.MaxValue;

        var toeTeVoegenStartDatum = toeTeVoegenErkenningsPeriode.Startdatum ?? DateOnly.MinValue;
        var toeTeVoegenEinddatum = toeTeVoegenErkenningsPeriode.Einddatum ?? DateOnly.MaxValue;

        return toeTeVoegenStartDatum <= bestaandeEinddatum && toeTeVoegenEinddatum >= bestaandeStartDatum;
    }

    public static ErkenningsPeriode Hydrate(DateOnly? startdatum, DateOnly? einddatum) => new(startdatum, einddatum);

    public static ErkenningsPeriode CreateFrom(TeCorrigerenErkenning teCorrigerenErkenning, Erkenning erkenning)
    {
        var startdatum = teCorrigerenErkenning.StartDatum.IsNull
            ? erkenning.ErkenningsPeriode.Startdatum
            : teCorrigerenErkenning.StartDatum.Value;

        var einddatum = teCorrigerenErkenning.EindDatum.IsNull
            ? erkenning.ErkenningsPeriode.Einddatum
            : teCorrigerenErkenning.EindDatum.Value;

        return Create(startdatum, einddatum);
    }
}
