namespace AssociationRegistry.Admin.Api.GrarConsumer.Groupers;

using Grar.Models;
using Finders;
using Grar.GrarConsumer.TeHeradresserenLocaties;

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
