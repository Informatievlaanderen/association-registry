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
}
