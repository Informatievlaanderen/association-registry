namespace AssociationRegistry.Admin.Api.GrarConsumer.Handlers.StraatHernummering.Groupers;

using Acties.HeradresseerLocaties;
using AssociationRegistry.Admin.Api.GrarConsumer.Finders;
using AssociationRegistry.Grar.Models;
using Grar.GrarUpdates.Fusies.TeHeradresserenLocaties;

public static class LocatiesVolgensVCodeGrouper
{
    public static HeradresseerLocatiesMessage[] Group(IEnumerable<LocatieMetVCode> lookupDocuments, int destinationAdresId)
    {
        if (!lookupDocuments.Any())
            return [];

        return lookupDocuments
              .GroupBy(v => v.VCode)
              .Select(g =>
                          new HeradresseerLocatiesMessage(
                              g.Key,
                              g.Select(doc => new TeHeradresserenLocatie(doc.LocatieId, destinationAdresId.ToString())).ToList(),
                              idempotencyKey: ""))
              .ToArray();
    }
}
