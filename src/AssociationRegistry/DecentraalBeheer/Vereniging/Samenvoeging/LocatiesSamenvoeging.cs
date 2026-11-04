namespace AssociationRegistry.DecentraalBeheer.Vereniging.Samenvoeging;

using Adressen;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Events;
using Events.Factories;
using Formats;
using Gegevens = Events.SamenvoegingDubbeleVereniging;

/// <summary>
///     Locaties van de dubbele vereniging worden overgenomen als ze valide en uniek zijn volgens de bestaande regels;
///     adrescomponenten worden genormaliseerd voor de vergelijking.
///     Een correspondentieadres wordt enkel overgenomen als de authentieke vereniging er nog geen heeft.
///     Heeft de authentieke vereniging al een primaire locatie, dan wordt een primaire locatie van de dubbele vereniging
///     overgenomen als niet-primair. Bestaande locaties worden nooit overschreven.
/// </summary>
public static class LocatiesSamenvoeging
{
    public const string RedenNietValide = "De locatie voldoet niet aan de validatieregels.";

    public const string RedenCorrespondentie = "De authentieke vereniging heeft al een correspondentieadres.";

    public static (
        Gegevens.OvergenomenGegeven<Registratiedata.Locatie>[] Overgenomen,
        Gegevens.NietOvergenomenGegeven<Registratiedata.Locatie>[] NietOvergenomen
    ) Merge(Locaties authentiekeVereniging, Locaties dubbeleVereniging)
    {
        var locaties = authentiekeVereniging;
        var resultaat = new SamenvoegingResultaat<Registratiedata.Locatie>();

        foreach (var locatie in dubbeleVereniging.OrderBy(x => x.LocatieId))
        {
            var original = EventFactory.Locatie(locatie);
            var description = Describe(original);

            var candidate = TryCreate(locatie);

            if (candidate is null)
            {
                resultaat.NietOvergenomen(description, original, RedenNietValide);

                continue;
            }

            if (locaties.HasDuplicateOf(candidate))
            {
                resultaat.Gededupliceerd(description, original);

                continue;
            }

            if (candidate.Locatietype == Locatietype.Correspondentie && locaties.HasCorrespondentieLocatie())
            {
                resultaat.NietOvergenomen(description, original, RedenCorrespondentie);

                continue;
            }

            var mustBecomeNietPrimair = candidate.IsPrimair && locaties.HasPrimaireLocatie();

            var added = locaties.VoegToe(mustBecomeNietPrimair ? candidate with { IsPrimair = false } : candidate);
            locaties = locaties.Hydrate(locaties.Append(added));

            if (mustBecomeNietPrimair)
                resultaat.ToegevoegdAlsNietPrimair(
                    description,
                    "een primaire locatie",
                    original,
                    EventFactory.Locatie(added)
                );
            else
                resultaat.Toegevoegd(description, original, EventFactory.Locatie(added));
        }

        return resultaat.Build();
    }

    private static string Describe(Registratiedata.Locatie locatie)
    {
        var label =
            !string.IsNullOrWhiteSpace(locatie.Naam) ? locatie.Naam
            : locatie.Adres is not null ? locatie.Adres.ToAdresString()
            : locatie.AdresId?.Bronwaarde;

        return $"Locatie '{label}'";
    }

    /// <summary>Maakt de locatie opnieuw aan, zodat de bestaande validatieregels gelden.</summary>
    private static Locatie? TryCreate(Locatie locatie)
    {
        try
        {
            var adres = locatie.Adres is null
                ? null
                : Adres.Create(
                    locatie.Adres.Straatnaam,
                    locatie.Adres.Huisnummer,
                    locatie.Adres.Busnummer,
                    locatie.Adres.Postcode,
                    locatie.Adres.Gemeente.Naam,
                    locatie.Adres.Land
                );

            if (locatie.AdresId is not null && !Adresbron.CanParse(locatie.AdresId.Adresbron.Code))
                return null;

            var adresId = locatie.AdresId is null
                ? null
                : AdresId.Create(Adresbron.Parse(locatie.AdresId.Adresbron.Code), locatie.AdresId.Bronwaarde);

            return Locatie.Create(
                Locatienaam.Create(locatie.Naam),
                locatie.IsPrimair,
                locatie.Locatietype.Waarde,
                adresId,
                adres
            );
        }
        catch (DomainException)
        {
            return null;
        }
    }
}
