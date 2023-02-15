namespace AssociationRegistry.Acm.Api.Projections;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Events;
using JasperFx.Core;
using Marten;
using Marten.Events;
using Marten.Events.Aggregation;
using Marten.Events.Projections;
using Marten.Internal.Operations;
using Marten.Storage;
using Schema.VerenigingenPerInsz;
using Vertegenwoordigers;

public class VerenigingenPerInszIProjection : IProjection
{
    public VerenigingenPerInszIProjection()
    {
        // Identities<VerenigingWerdGeregistreerd>(geregistreerd => geregistreerd.Vertegenwoordigers.Select(vertegenwoordiger => vertegenwoordiger.Insz).ToArray());
        // FanOut<VerenigingWerdGeregistreerd, VerenigingWerdGeregistreerd.Vertegenwoordiger>(x => x.Vertegenwoordigers);
        // CustomGrouping(new CustomSlicer());
    }

    public void Apply(IDocumentOperations ops, IReadOnlyList<StreamAction> streams)
    {
        var questEvents = streams.SelectMany(x => x.Events).OrderBy(s => s.Sequence).Select(s => s.Data);

        foreach (var @event in questEvents)
        {
            var newDocs = new List<VerenigingenPerInszDocument>();
            if (@event is VerenigingWerdGeregistreerd werdGeregistreerd)
            {
                foreach (var vertegenwoordiger in werdGeregistreerd.Vertegenwoordigers)
                {
                    var verenigingenPerInszDocument =
                        ops.Query<VerenigingenPerInszDocument>()
                            .SingleOrDefault(x => x.Insz == vertegenwoordiger.Insz);
                    if (verenigingenPerInszDocument is null)
                    {
                        verenigingenPerInszDocument = new VerenigingenPerInszDocument
                        {
                            Insz = vertegenwoordiger.Insz,
                            Verenigingen = new List<Vereniging>()
                        };
                        newDocs.Add(verenigingenPerInszDocument);
                    }

                    verenigingenPerInszDocument.Verenigingen.Add(new Vereniging(werdGeregistreerd.VCode, werdGeregistreerd.Naam));
                    ops.Insert(verenigingenPerInszDocument);
                }
            }
            else if (@event is NaamWerdGewijzigd e)
            {
                var allDocs = ops.Query<VerenigingenPerInszDocument>().ToList();
                var documents = ops.Query<VerenigingenPerInszDocument>()
                    .Where(document => document.Verenigingen.Any(vereniging => vereniging.VCode == e.VCode))
                    .ToList()
                    .Union(newDocs)
                    .ToList();

                foreach (var verenigingenPerInszDocument in documents)
                {
                    verenigingenPerInszDocument.Verenigingen.Single(vereniging => vereniging.VCode == e.VCode).Naam = e.Naam;
                    ops.Update(verenigingenPerInszDocument);
                }
            }
        }
    }

    public Task ApplyAsync(IDocumentOperations operations, IReadOnlyList<StreamAction> streams,
        CancellationToken cancellation)
    {
        Apply(operations, streams);
        return Task.CompletedTask;
    }

    public void Project(VerenigingWerdGeregistreerd werdGeregistreerd, IDocumentOperations ops)
    {
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
            ops.Insert(verenigingenPerInszDocument);
        }
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
            ops.Update(verenigingenPerInszDocument);
        }
    }
}
