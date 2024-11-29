namespace AssociationRegistry.Admin.Api.GrarSync;

using Grar.HeradresseerLocaties;
using Grar.Models;
using Schema.Detail;

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
