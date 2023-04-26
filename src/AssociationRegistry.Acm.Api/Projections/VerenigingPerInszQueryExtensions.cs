namespace AssociationRegistry.Acm.Api.Projections;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marten;
using Schema.VerenigingenPerInsz;

public static class VerenigingPerInszQueryExtensions
{
    public static async Task<VerenigingenPerInszDocument> GetVerenigingenPerInszDocumentOrNew(this IDocumentOperations source, string insz)
        => await source.Query<VerenigingenPerInszDocument>().SingleOrDefaultAsync(document => document.Insz.Equals(insz)) ??
           new VerenigingenPerInszDocument
           {
               Insz = insz,
               Verenigingen = new List<Vereniging>(),
           };

    public static async Task<VerenigingDocument> GetVerenigingDocument(this IDocumentOperations source, string vCode)
        => await source.Query<VerenigingDocument>()
            .Where(v => v.VCode == vCode)
            .SingleAsync();

    public static async Task<IReadOnlyList<VerenigingenPerInszDocument>> GetVerenigingenPerInszDocuments(this IDocumentOperations source, string vCode)
    {
        return await source.Query<VerenigingenPerInszDocument>()
            .Where(document => document.Verenigingen.Any(vereniging => vereniging.VCode == vCode))
            .ToListAsync();
    }
}
