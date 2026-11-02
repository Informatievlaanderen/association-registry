namespace AssociationRegistry.DecentraalBeheer.Vereniging.Samenvoeging;

using Events;
using Events.Factories;
using Gegevens = Events.SamenvoegingDubbeleVereniging;

/// <summary>
///     Vertegenwoordigers die enkel op de dubbele vereniging bestaan, worden toegevoegd aan de authentieke vereniging.
///     Vertegenwoordigers met hetzelfde INSZ als een vertegenwoordiger van de authentieke vereniging worden niet dubbel
///     toegevoegd; de gegevens op de authentieke vereniging worden nooit overschreven.
///     Heeft de authentieke vereniging al een primaire vertegenwoordiger, dan wordt een primaire vertegenwoordiger van de
///     dubbele vereniging overgenomen als niet-primair.
/// </summary>
/// <remarks>De beschrijvingen bevatten geen persoonsgegevens, want ze worden mee bewaard in het event.</remarks>
public static class VertegenwoordigersSamenvoeging
{
    public static (
        Gegevens.OvergenomenGegeven<Registratiedata.Vertegenwoordiger>[] Overgenomen,
        Gegevens.NietOvergenomenGegeven<Registratiedata.Vertegenwoordiger>[] NietOvergenomen
    ) Merge(Vertegenwoordigers authentiekeVereniging, Vertegenwoordigers dubbeleVereniging)
    {
        var vertegenwoordigers = authentiekeVereniging;

        var overgenomen = new List<Gegevens.OvergenomenGegeven<Registratiedata.Vertegenwoordiger>>();
        var nietOvergenomen = new List<Gegevens.NietOvergenomenGegeven<Registratiedata.Vertegenwoordiger>>();

        foreach (var vertegenwoordiger in dubbeleVereniging.OrderBy(x => x.VertegenwoordigerId))
        {
            var description = $"Vertegenwoordiger met id {vertegenwoordiger.VertegenwoordigerId}";
            var original = EventFactory.Vertegenwoordiger(vertegenwoordiger);

            if (vertegenwoordigers.Any(x => x.Insz == vertegenwoordiger.Insz))
            {
                nietOvergenomen.Add(Gegevens.Gededupliceerd(description, original));

                continue;
            }

            var mustBecomeNietPrimair = vertegenwoordiger.IsPrimair && vertegenwoordigers.Any(x => x.IsPrimair);

            var added = vertegenwoordigers.VoegToe(
                mustBecomeNietPrimair ? vertegenwoordiger with { IsPrimair = false } : vertegenwoordiger
            );
            vertegenwoordigers = vertegenwoordigers.Hydrate(vertegenwoordigers.Append(added));

            overgenomen.Add(
                mustBecomeNietPrimair
                    ? Gegevens.ToegevoegdMetAanpassing(
                        $"{description} werd toegevoegd als niet-primair, omdat de authentieke vereniging al een primaire vertegenwoordiger heeft.",
                        original,
                        EventFactory.Vertegenwoordiger(added)
                    )
                    : new Gegevens.OvergenomenGegeven<Registratiedata.Vertegenwoordiger>(
                        Gegevens.Acties.Toegevoegd,
                        $"{description} werd toegevoegd vanuit de dubbele vereniging.",
                        original,
                        EventFactory.Vertegenwoordiger(added)
                    )
            );
        }

        return ([.. overgenomen], [.. nietOvergenomen]);
    }
}
