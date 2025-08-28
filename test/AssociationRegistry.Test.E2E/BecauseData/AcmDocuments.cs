namespace AssociationRegistry.Test.E2E.BecauseData;

using Acm.Schema.VerenigingenPerInsz;
using DecentraalBeheer.Vereniging;
using Marten;
using Newtonsoft.Json;

public static class AcmDocuments
{
    public static string GetMissingDocuments(IDocumentSession session, string inszToCompare, VCode vCode)
    {
        var verenigingPerInszDocument = session.Query<VerenigingenPerInszDocument>()
                                               .Where(x => x.Insz == inszToCompare)
                                               .Single();

        var verenigingDocument = session.Query<VerenigingDocument>()
                                        .Where(x => x.VCode == vCode)
                                        .Single();

        return $"VerenigingenPerInszDocument: {JsonConvert.SerializeObject(verenigingPerInszDocument)}{Environment.NewLine}" +
               $"VerenigingDocument: {JsonConvert.SerializeObject(verenigingDocument)}";
    }
}
