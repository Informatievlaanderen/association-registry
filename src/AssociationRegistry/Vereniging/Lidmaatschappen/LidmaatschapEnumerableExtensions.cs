namespace AssociationRegistry.Vereniging;

using Events;

public static class LidmaatschapEnumerableExtensions
{
    public static IEnumerable<Lidmaatschap> Without(this IEnumerable<Lidmaatschap> lidmaatschappen, Lidmaatschap lidmaatschap)
        => lidmaatschappen.Without(lidmaatschap.LidmaatschapId);

    public static IEnumerable<Lidmaatschap> Without(this IEnumerable<Lidmaatschap> lidmaatschappen, int lidmaatschapId)
        => lidmaatschappen.Where(l => l.LidmaatschapId != lidmaatschapId);

    public static IEnumerable<Lidmaatschap> AppendFromEventData(this IEnumerable<Lidmaatschap> lidmaatschappen, Registratiedata.Lidmaatschap eventData)
        => lidmaatschappen.Append(
            Lidmaatschap.Hydrate(
                eventData.LidmaatschapId,
                VCode.Create(eventData.AndereVereniging),
                eventData.AndereVerenigingNaam,
                new Geldigheidsperiode(new GeldigVan(eventData.DatumVan), new GeldigTot(eventData.DatumTot)),
               eventData.Identificatie,
                eventData.Beschrijving)
        );
}
