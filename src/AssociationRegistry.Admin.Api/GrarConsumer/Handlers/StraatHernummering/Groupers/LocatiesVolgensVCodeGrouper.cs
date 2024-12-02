namespace AssociationRegistry.Admin.Api.GrarConsumer.Handlers.StraatHernummering.Groupers;

using AssociationRegistry.Admin.Api.GrarConsumer.Finders;
using AssociationRegistry.Grar.GrarUpdates.TeHeradresserenLocaties;
using AssociationRegistry.Grar.Models;

public static class LocatiesVolgensVCodeGrouper
{
    public static TeHeradresserenLocatiesMessage[] Group(IEnumerable<LocatieMetVCode> lookupDocuments, int destinationAdresId)
    {
        if (!lookupDocuments.Any())
            return [];

        return lookupDocuments
              .GroupBy(v => v.VCode)
              .Select(g =>
                          new TeHeradresserenLocatiesMessage(
                              g.Key,
                              g.Select(doc => new TeHeradresserenLocatie(doc.LocatieId, destinationAdresId.ToString())).ToList(),
                              idempotencyKey: ""))
              .ToArray();
    }
}
