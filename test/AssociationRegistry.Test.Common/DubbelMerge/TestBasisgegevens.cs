namespace AssociationRegistry.Test.Common.DubbelMerge;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Events;

/// <summary>
///     Vaste basisgegevens voor de samenvoeging van een dubbele vereniging, zodat een test toont welke waarde
///     overgenomen wordt en welke niet. Elke set heeft in elk veld een andere waarde.
/// </summary>
public record TestBasisgegevens(
    string KorteNaam,
    string KorteBeschrijving,
    DateOnly? Startdatum,
    Registratiedata.Doelgroep Doelgroep
)
{
    public static readonly TestBasisgegevens Empty = new(
        KorteNaam: "",
        KorteBeschrijving: "",
        Startdatum: null,
        Doelgroep: new Registratiedata.Doelgroep(
            DecentraalBeheer.Vereniging.Doelgroep.StandaardMinimumleeftijd,
            DecentraalBeheer.Vereniging.Doelgroep.StandaardMaximumleeftijd
        )
    );

    public static readonly TestBasisgegevens Fellowship = new(
        KorteNaam: "Fellowship",
        KorteBeschrijving: "Het reisgezelschap van de ring",
        Startdatum: new DateOnly(2001, 12, 19),
        Doelgroep: new Registratiedata.Doelgroep(11, 111)
    );

    public static readonly TestBasisgegevens Rivendell = new(
        KorteNaam: "Rivendell",
        KorteBeschrijving: "Het huis van Elrond",
        Startdatum: new DateOnly(1954, 7, 29),
        Doelgroep: new Registratiedata.Doelgroep(18, 99)
    );
}

public static class TestBasisgegevensExtensions
{
    public static FeitelijkeVerenigingWerdGeregistreerd WithBasisgegevens(
        this FeitelijkeVerenigingWerdGeregistreerd @event,
        TestBasisgegevens basisgegevens
    ) =>
        @event with
        {
            KorteNaam = basisgegevens.KorteNaam,
            KorteBeschrijving = basisgegevens.KorteBeschrijving,
            Startdatum = basisgegevens.Startdatum,
            Doelgroep = basisgegevens.Doelgroep,
        };

    public static VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd WithBasisgegevens(
        this VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd @event,
        TestBasisgegevens basisgegevens
    ) =>
        @event with
        {
            KorteNaam = basisgegevens.KorteNaam,
            KorteBeschrijving = basisgegevens.KorteBeschrijving,
            Startdatum = basisgegevens.Startdatum,
            Doelgroep = basisgegevens.Doelgroep,
        };
}
