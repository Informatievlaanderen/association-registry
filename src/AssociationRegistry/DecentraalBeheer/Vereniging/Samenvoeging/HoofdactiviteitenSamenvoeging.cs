namespace AssociationRegistry.DecentraalBeheer.Vereniging.Samenvoeging;

using Events;
using Events.Factories;
using Gegevens = Events.SamenvoegingDubbeleVereniging;

/// <summary>
///     Hoofdactiviteiten van de dubbele vereniging worden toegevoegd als de code nog niet bestaat op de authentieke
///     vereniging. Bestaande codes worden niet dubbel toegevoegd.
/// </summary>
public static class HoofdactiviteitenSamenvoeging
{
    public static (
        Gegevens.OvergenomenGegeven<Registratiedata.HoofdactiviteitVerenigingsloket>[] Overgenomen,
        Gegevens.NietOvergenomenGegeven<Registratiedata.HoofdactiviteitVerenigingsloket>[] NietOvergenomen
    ) Merge(
        HoofdactiviteitenVerenigingsloket authentiekeVereniging,
        HoofdactiviteitenVerenigingsloket dubbeleVereniging
    )
    {
        var existingCodes = authentiekeVereniging.Select(x => x.Code).ToHashSet();

        var overgenomen = new List<Gegevens.OvergenomenGegeven<Registratiedata.HoofdactiviteitVerenigingsloket>>();
        var nietOvergenomen =
            new List<Gegevens.NietOvergenomenGegeven<Registratiedata.HoofdactiviteitVerenigingsloket>>();

        foreach (var hoofdactiviteit in dubbeleVereniging)
        {
            var description = $"Hoofdactiviteit '{hoofdactiviteit.Naam}'";
            var value = EventFactory.HoofdactiviteitVerenigingsloket(hoofdactiviteit);

            if (existingCodes.Contains(hoofdactiviteit.Code))
                nietOvergenomen.Add(Gegevens.Gededupliceerd(description, value));
            else
                overgenomen.Add(Gegevens.Toegevoegd(description, value));
        }

        return ([.. overgenomen], [.. nietOvergenomen]);
    }
}
