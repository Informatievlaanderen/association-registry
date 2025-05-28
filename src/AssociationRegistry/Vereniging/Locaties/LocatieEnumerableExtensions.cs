namespace AssociationRegistry.Vereniging;

using Events;

public static class LocatieEnumerableExtensions
{
    public static IEnumerable<Locatie> Without(this IEnumerable<Locatie> locaties, Locatie locatie)
        => locaties.Without(locatie.LocatieId);

    public static IEnumerable<Locatie> Without(this IEnumerable<Locatie> locaties, int locatieId)
        => locaties.Where(l => l.LocatieId != locatieId);

    public static IEnumerable<Locatie> AppendFromEventData(this IEnumerable<Locatie> locaties, Registratiedata.Locatie eventData)
        => locaties.Append(
            Locatie.Hydrate(
                eventData.LocatieId,
                eventData.Naam,
                eventData.IsPrimair,
                eventData.Locatietype,
                eventData.Adres is null
                    ? null
                    : Adres.Hydrate(
                        eventData.Adres.Straatnaam,
                        eventData.Adres.Huisnummer,
                        eventData.Adres.Busnummer,
                        eventData.Adres.Postcode,
                        eventData.Adres.Gemeente,
                        eventData.Adres.Land),
                eventData.AdresId is null ? null : AdresId.Hydrate(eventData.AdresId.Broncode, eventData.AdresId.Bronwaarde))
        );

    public static bool HasPrimaireLocatie(this IEnumerable<Locatie> locaties)
        => locaties.Any(l => l.IsPrimair);

    public static bool HasCorrespondentieLocatie(this IEnumerable<Locatie> locaties)
        => locaties.Any(l => l.Locatietype == Locatietype.Correspondentie);

    public static (T[] Matched, T[] Unmatched) Partition<T>(
        this IEnumerable<T> source,
        Func<T,bool> predicate)
    {
        var yes = new List<T>();
        var no  = new List<T>();
        foreach (var item in source)
        {
            if (predicate(item))
                yes.Add(item);
            else
                no.Add(item);
        }
        return (yes.ToArray(), no.ToArray());
    }
}
