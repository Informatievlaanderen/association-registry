namespace AssociationRegistry.Acm.Api.Projections;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Events;
using Marten;
using Marten.Events.Projections;
using Schema.VerenigingenPerInsz;

public class VerenigingenPerInszProjection : EventProjection
{
    public VerenigingenPerInszProjection()
    {
        // Needs a batch size of 1, because otherwise if Registered and NameChanged arrive in 1 batch/slice,
        // the newly persisted VerenigingenPerInszDocument from VerenigingWerdGeregistreerd is not in the
        // Query yet when we handle NaamWerdGewijzigd
        Options.BatchSize = 1;
    }


    public void Project(VerenigingWerdGeregistreerd werdGeregistreerd, IDocumentOperations ops)
    {
        var docs = new List<VerenigingenPerInszDocument>();
        foreach (var vertegenwoordiger in werdGeregistreerd.Vertegenwoordigers)
        {
            var verenigingenPerInszDocument =
                ops.Query<VerenigingenPerInszDocument>()
                    .SingleOrDefault(x => x.Insz == vertegenwoordiger.Insz) ??
                new VerenigingenPerInszDocument
                {
                    Insz = vertegenwoordiger.Insz,
                    Verenigingen = new List<Vereniging>(),
                };
            verenigingenPerInszDocument.Verenigingen.Add(new Vereniging(werdGeregistreerd.VCode, werdGeregistreerd.Naam));
            docs.Add(verenigingenPerInszDocument);
        }
        ops.StoreObjects(docs);
    }

    public async Task Project(NaamWerdGewijzigd e, IDocumentOperations ops)
    {
        var documents = await ops.Query<VerenigingenPerInszDocument>()
            .Where(document => document.Verenigingen.Any(vereniging => vereniging.VCode == e.VCode))
            .ToListAsync();

        foreach (var verenigingenPerInszDocument in documents)
        {
            verenigingenPerInszDocument.Verenigingen.Single(vereniging => vereniging.VCode == e.VCode).Naam = e.Naam;
            ops.Store(verenigingenPerInszDocument);
        }
    }
}
