namespace AssociationRegistry.Acm.Api.Projections;

using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Events;
using Marten;
using Marten.Events.Projections;
using Schema.VerenigingenPerInsz;
using Vertegenwoordigers;

public class VerenigingenPerInszProjection : EventProjection
{
    public VerenigingenPerInszProjection()
    {
        // Identities<VerenigingWerdGeregistreerd>(geregistreerd => geregistreerd.Vertegenwoordigers.Select(vertegenwoordiger => vertegenwoordiger.Insz).ToArray());
        // FanOut<VerenigingWerdGeregistreerd, VerenigingWerdGeregistreerd.Vertegenwoordiger>(x => x.Vertegenwoordigers);
        // CustomGrouping(new CustomSlicer());
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
                    Verenigingen = new List<Vereniging>()
                };
            verenigingenPerInszDocument.Verenigingen.Add(new Vereniging(werdGeregistreerd.VCode, werdGeregistreerd.Naam));
            docs.Add(verenigingenPerInszDocument);
        }
        ops.StoreObjects(docs);
    }

    public async Task Project(NaamWerdGewijzigd e, IDocumentOperations ops)
    {
        var allDocuments = ops.Query<VerenigingenPerInszDocument>().ToList();

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
