namespace AssociationRegistry.DecentraalBeheer.Vereniging.Samenvoeging;

using Erkenningen;
using Gegevens = Events.SamenvoegingDubbeleVereniging;

/// <summary>
///     Erkenningen van de dubbele vereniging worden enkel toegevoegd als ze voldoen aan de uniciteitsregel: geen andere
///     erkenning voor hetzelfde IPDC-product en dezelfde initiator (geregistreerd door) met een overlappende periode.
///     Bestaande erkenningen op de authentieke vereniging worden niet overschreven.
/// </summary>
public static class ErkenningenSamenvoeging
{
    public const string RedenOverlap =
        "De authentieke vereniging heeft al een erkenning voor hetzelfde IPDC-product en dezelfde initiator in een overlappende periode.";

    public static (
        Gegevens.OvergenomenGegeven<Gegevens.Erkenning>[] Overgenomen,
        Gegevens.NietOvergenomenGegeven<Gegevens.Erkenning>[] NietOvergenomen
    ) Merge(Erkenningen authentiekeVereniging, Erkenningen dubbeleVereniging)
    {
        var erkenningen = authentiekeVereniging;
        var resultaat = new SamenvoegingResultaat<Gegevens.Erkenning>();

        foreach (var erkenning in dubbeleVereniging.OrderBy(x => x.ErkenningId))
        {
            var description = $"Erkenning '{erkenning.IpdcProduct.Naam}'";
            var original = ToEventData(erkenning);

            var sameCombinatie = erkenningen.WithSameOvoCodeAndIpdcProduct(erkenning).ToArray();

            if (sameCombinatie.Any(x => x.ErkenningsPeriode == erkenning.ErkenningsPeriode))
            {
                resultaat.Gededupliceerd(description, original);

                continue;
            }

            var candidate = erkenning with { ErkenningId = erkenningen.NextId };

            if (!erkenningen.KanErkenningToevoegenMetCombinatie(candidate))
            {
                resultaat.NietOvergenomen(description, original, RedenOverlap);

                continue;
            }

            erkenningen = erkenningen.Hydrate(erkenningen.Append(candidate));

            resultaat.Toegevoegd(description, original, ToEventData(candidate));
        }

        return resultaat.Build();
    }

    private static Gegevens.Erkenning ToEventData(Erkenning erkenning) =>
        new(
            erkenning.ErkenningId,
            erkenning.IpdcProduct,
            erkenning.ErkenningsPeriode.Startdatum,
            erkenning.ErkenningsPeriode.Einddatum,
            erkenning.Hernieuwingsdatum?.Value,
            erkenning.HernieuwingsUrl.Value,
            erkenning.GeregistreerdDoor,
            erkenning.RedenSchorsing,
            erkenning.Status.Value
        );
}
