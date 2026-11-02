namespace AssociationRegistry.DecentraalBeheer.Vereniging.Samenvoeging;

using Events;
using Events.Factories;
using Gegevens = Events.SamenvoegingDubbeleVereniging;

/// <summary>
///     Werkingsgebieden van de dubbele vereniging worden toegevoegd als de code nog niet bestaat op de authentieke
///     vereniging. Bestaande codes worden niet dubbel toegevoegd.
///     'Niet van toepassing' kan niet gecombineerd worden met andere werkingsgebieden; zo'n werkingsgebied wordt niet
///     overgenomen.
/// </summary>
public static class WerkingsgebiedenSamenvoeging
{
    public const string RedenNietVanToepassing =
        "'Niet van toepassing' kan niet gecombineerd worden met andere werkingsgebieden.";

    public static (
        Gegevens.OvergenomenGegeven<Registratiedata.Werkingsgebied>[] Overgenomen,
        Gegevens.NietOvergenomenGegeven<Registratiedata.Werkingsgebied>[] NietOvergenomen
    ) Merge(Werkingsgebieden authentiekeVereniging, Werkingsgebieden dubbeleVereniging)
    {
        var existingCodes = authentiekeVereniging.Select(x => x.Code).ToHashSet();

        var overgenomen = new List<Gegevens.OvergenomenGegeven<Registratiedata.Werkingsgebied>>();
        var nietOvergenomen = new List<Gegevens.NietOvergenomenGegeven<Registratiedata.Werkingsgebied>>();

        foreach (var werkingsgebied in dubbeleVereniging)
        {
            var description = $"Werkingsgebied '{werkingsgebied.Naam}'";
            var value = EventFactory.Werkingsgebied(werkingsgebied);

            if (existingCodes.Contains(werkingsgebied.Code))
                nietOvergenomen.Add(Gegevens.Gededupliceerd(description, value));
            else if (WouldCombineNietVanToepassing(authentiekeVereniging, werkingsgebied))
                nietOvergenomen.Add(Gegevens.NietOvergenomen(description, value, RedenNietVanToepassing));
            else
                overgenomen.Add(Gegevens.Toegevoegd(description, value));
        }

        return ([.. overgenomen], [.. nietOvergenomen]);
    }

    private static bool WouldCombineNietVanToepassing(
        Werkingsgebieden authentiekeVereniging,
        Werkingsgebied werkingsgebied
    ) =>
        authentiekeVereniging.IsNietVanToepassing
        || (werkingsgebied == Werkingsgebied.NietVanToepassing && !authentiekeVereniging.IsNietBepaald);
}
