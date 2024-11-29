namespace AssociationRegistry.Admin.Api.GrarConsumer.Groupers;

using Grar.HeradresseerLocaties;
using Grar.Models;
using Finders;

internal class LocatiesVolgensVCodeGrouper
{
    public TeHeradresserenLocatiesMessage[] Group(IEnumerable<LocatieLookupData> lookupDocuments)
        => lookupDocuments
          .GroupBy(v => v.VCode)
          .Select(g =>
                      new TeHeradresserenLocatiesMessage(
                          g.Key,
                          g.Select(doc => new LocatieIdWithAdresId(doc.LocatieId, doc.AdresId)).ToList(),
                          idempotencyKey: ""))
          .ToArray();
}
