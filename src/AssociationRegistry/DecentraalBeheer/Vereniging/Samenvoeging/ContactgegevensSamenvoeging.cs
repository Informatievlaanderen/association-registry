namespace AssociationRegistry.DecentraalBeheer.Vereniging.Samenvoeging;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Events;
using Events.Factories;
using Gegevens = Events.SamenvoegingDubbeleVereniging;

/// <summary>
///     Contactgegevens van de dubbele vereniging worden overgenomen als ze valide en uniek zijn volgens de bestaande
///     regels; telefoonnummers worden genormaliseerd voor de vergelijking.
///     Heeft de authentieke vereniging al een primair contactgegeven van hetzelfde type, dan wordt een primair
///     contactgegeven van de dubbele vereniging overgenomen als niet-primair. Bestaande contactgegevens worden nooit
///     overschreven.
/// </summary>
public static class ContactgegevensSamenvoeging
{
    public const string RedenNietValide = "Het contactgegeven voldoet niet aan de validatieregels.";

    public static (
        Gegevens.OvergenomenGegeven<Registratiedata.Contactgegeven>[] Overgenomen,
        Gegevens.NietOvergenomenGegeven<Registratiedata.Contactgegeven>[] NietOvergenomen
    ) Merge(Contactgegevens authentiekeVereniging, Contactgegevens dubbeleVereniging)
    {
        var contactgegevens = authentiekeVereniging;

        var overgenomen = new List<Gegevens.OvergenomenGegeven<Registratiedata.Contactgegeven>>();
        var nietOvergenomen = new List<Gegevens.NietOvergenomenGegeven<Registratiedata.Contactgegeven>>();

        foreach (var contactgegeven in dubbeleVereniging.OrderBy(x => x.ContactgegevenId))
        {
            var description = $"{contactgegeven.Contactgegeventype} '{contactgegeven.Waarde}'";
            var original = EventFactory.Contactgegeven(contactgegeven);

            var candidate = TryCreate(contactgegeven);

            if (candidate is null)
            {
                nietOvergenomen.Add(Gegevens.NietOvergenomen(description, original, RedenNietValide));

                continue;
            }

            if (contactgegevens.ContainsMetZelfdeWaarden(candidate))
            {
                nietOvergenomen.Add(Gegevens.Gededupliceerd(description, original));

                continue;
            }

            var mustBecomeNietPrimair =
                candidate.IsPrimair && contactgegevens.HasPrimairForType(candidate.Contactgegeventype);

            var added = contactgegevens.VoegToe(
                mustBecomeNietPrimair ? candidate with { IsPrimair = false } : candidate
            );
            contactgegevens = contactgegevens.Hydrate(contactgegevens.Append(added));

            overgenomen.Add(
                mustBecomeNietPrimair
                    ? Gegevens.ToegevoegdMetAanpassing(
                        $"{description} werd toegevoegd als niet-primair, omdat de authentieke vereniging al een primair contactgegeven van dit type heeft.",
                        original,
                        EventFactory.Contactgegeven(added)
                    )
                    : new Gegevens.OvergenomenGegeven<Registratiedata.Contactgegeven>(
                        Gegevens.Acties.Toegevoegd,
                        $"{description} werd toegevoegd vanuit de dubbele vereniging.",
                        original,
                        EventFactory.Contactgegeven(added)
                    )
            );
        }

        return ([.. overgenomen], [.. nietOvergenomen]);
    }

    /// <summary>Maakt het contactgegeven opnieuw aan, zodat de bestaande validatie- en vergelijkingsregels gelden.</summary>
    private static Contactgegeven? TryCreate(Contactgegeven contactgegeven)
    {
        try
        {
            return Contactgegeven.CreateFromInitiator(
                contactgegeven.Contactgegeventype,
                contactgegeven.Waarde,
                contactgegeven.Beschrijving,
                contactgegeven.IsPrimair
            );
        }
        catch (DomainException)
        {
            return null;
        }
    }
}
