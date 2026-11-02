namespace AssociationRegistry.DecentraalBeheer.Vereniging.Samenvoeging;

using Events;
using Gegevens = Events.SamenvoegingDubbeleVereniging;

/// <summary>
///     Bepaalt welke basisgegevens (korte naam, korte beschrijving, startdatum, doelgroep) van de dubbele vereniging
///     op de authentieke vereniging terechtkomen.
///     Een attribuut wordt alleen overgenomen als het op de authentieke vereniging leeg is.
///     Een bestaande waarde op de authentieke vereniging wordt nooit overschreven.
/// </summary>
public static class BasisgegevensSamenvoeging
{
    public static (
        Gegevens.OvergenomenBasisgegevens Overgenomen,
        Gegevens.NietOvergenomenBasisgegevens NietOvergenomen
    ) Merge(VerenigingState authentiekeVereniging, VerenigingState dubbeleVereniging)
    {
        var korteNaam = Merge(
            Gegevens.Velden.KorteNaam,
            authentiekIsEmpty: string.IsNullOrWhiteSpace(authentiekeVereniging.KorteNaam),
            authentiek: authentiekeVereniging.KorteNaam ?? string.Empty,
            dubbelIsEmpty: string.IsNullOrWhiteSpace(dubbeleVereniging.KorteNaam),
            dubbel: dubbeleVereniging.KorteNaam ?? string.Empty
        );

        var korteBeschrijving = Merge(
            Gegevens.Velden.KorteBeschrijving,
            authentiekIsEmpty: string.IsNullOrWhiteSpace(authentiekeVereniging.KorteBeschrijving),
            authentiek: authentiekeVereniging.KorteBeschrijving ?? string.Empty,
            dubbelIsEmpty: string.IsNullOrWhiteSpace(dubbeleVereniging.KorteBeschrijving),
            dubbel: dubbeleVereniging.KorteBeschrijving ?? string.Empty
        );

        var startdatum = Merge(
            Gegevens.Velden.Startdatum,
            authentiekIsEmpty: authentiekeVereniging.Startdatum is null,
            authentiek: authentiekeVereniging.Startdatum?.Value ?? default,
            dubbelIsEmpty: dubbeleVereniging.Startdatum is null,
            dubbel: dubbeleVereniging.Startdatum?.Value ?? default
        );

        var doelgroep = Merge(
            Gegevens.Velden.Doelgroep,
            authentiekIsEmpty: IsEmpty(authentiekeVereniging.Doelgroep),
            authentiek: ToEventData(authentiekeVereniging.Doelgroep),
            dubbelIsEmpty: IsEmpty(dubbeleVereniging.Doelgroep),
            dubbel: ToEventData(dubbeleVereniging.Doelgroep)
        );

        return (
            new Gegevens.OvergenomenBasisgegevens(
                korteNaam.Overgenomen,
                korteBeschrijving.Overgenomen,
                startdatum.Overgenomen,
                doelgroep.Overgenomen
            ),
            new Gegevens.NietOvergenomenBasisgegevens(
                korteNaam.NietOvergenomen,
                korteBeschrijving.NietOvergenomen,
                startdatum.NietOvergenomen,
                doelgroep.NietOvergenomen
            )
        );
    }

    private static Result<T> Merge<T>(string veld, bool authentiekIsEmpty, T authentiek, bool dubbelIsEmpty, T dubbel)
    {
        if (dubbelIsEmpty)
            return new Result<T>(null, null);

        if (authentiekIsEmpty)
            return new Result<T>(Gegevens.Verrijkt(veld, dubbel), null);

        if (Equals(authentiek, dubbel))
            return new Result<T>(null, Gegevens.Gededupliceerd(veld, dubbel));

        return new Result<T>(null, Gegevens.NietOvergenomen(veld, dubbel));
    }

    /// <summary>De standaard doelgroep (0 tot 150 jaar) geldt als leeg.</summary>
    private static bool IsEmpty(Doelgroep? doelgroep) => doelgroep is null || doelgroep == Doelgroep.Null;

    private static Registratiedata.Doelgroep ToEventData(Doelgroep? doelgroep) =>
        doelgroep is null
            ? new Registratiedata.Doelgroep(Doelgroep.StandaardMinimumleeftijd, Doelgroep.StandaardMaximumleeftijd)
            : new Registratiedata.Doelgroep(doelgroep.Minimumleeftijd, doelgroep.Maximumleeftijd);

    private record Result<T>(
        Gegevens.OvergenomenGegeven<T>? Overgenomen,
        Gegevens.NietOvergenomenGegeven<T>? NietOvergenomen
    );
}
