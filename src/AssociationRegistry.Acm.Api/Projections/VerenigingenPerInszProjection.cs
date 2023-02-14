namespace AssociationRegistry.Acm.Api.Projections;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Events;
using Marten;
using Marten.Events;
using Marten.Events.Aggregation;
using Marten.Events.Projections;
using Schema.VerenigingenPerInsz;

public class VerenigingenPerInszProjection : EventProjection
{
    public VerenigingenPerInszProjection()
    {
        Options.BatchSize = 1;
    }

    public void Project(VerenigingWerdGeregistreerd @event, IDocumentOperations ops)
    {
        foreach (var vertegenwoordiger in @event.Vertegenwoordigers)
        {
            var doc = ops.Load<VerenigingenPerInszDocument>(vertegenwoordiger.Insz) ??
                      new VerenigingenPerInszDocument
                      {
                          Insz = vertegenwoordiger.Insz,
                      };
            doc.Verenigingen.Add(new Vereniging(@event.VCode, @event.Naam));
            ops.Store(doc);
        }
    }

    public async Task Project(NaamWerdGewijzigd e, IDocumentOperations ops)
    {
        var docs = await ops.Query<VerenigingenPerInszDocument>()
            .Where(document => document.Verenigingen.Any(vereniging => vereniging.VCode == e.VCode))
            .ToListAsync();

        foreach (var doc in docs)
        {
            doc.Verenigingen.Single(vereniging => vereniging.VCode == e.VCode).Naam = e.Naam;
        }

        ops.Store(docs.ToArray());
    }

}

public class NaamWerdGewijzigdGrouper : IAggregateGrouper<string>
{
    public async Task Group(IQuerySession session, IEnumerable<IEvent> events, ITenantSliceGroup<string> grouping)
    {
        var naamWerdGewijzigdEvents = events
            .OfType<IEvent<NaamWerdGewijzigd>>()
            .ToList();

        if (!naamWerdGewijzigdEvents.Any())
        {
            return;
        }

        var vCodes = naamWerdGewijzigdEvents.Select(x => x.Data.VCode).ToList();
        var documents = await session.Query<VerenigingenPerInszDocument>()
            .Where(doc => doc.Verenigingen.Any(x => vCodes.Contains(x.VCode)))
            .ToListAsync();

        foreach (var document in documents)
        {
            var vCodesForDoc = document.Verenigingen.Select(x => x.VCode);
            var groupedEvents = naamWerdGewijzigdEvents
                .Where(@event => vCodesForDoc.Contains(@event.Data.VCode));
            grouping.AddEvents(document.Insz, groupedEvents);
        }

        // var vcodes = naamWerdGewijzigdEvents.Select(x => x.Data.VCode);
        //
        // var eventsPerInsz = new Dictionary<string, NaamWerdGewijzigd[]>();
        //
        // foreach (var vcode in vcodes)
        // {
        //     var documents = await session.Query<VerenigingenPerInszDocument>()
        //         .Where(doc =>
        //             doc.Verenigingen.Any(x => x.VCode == vcode))
        //         .ToListAsync();
        //
        //     foreach (var document in documents)
        //     {
        //         // if(eventsPerInsz.ContainsKey(document.Insz))
        //         eventsPerInsz[document.Insz] =
        //     }
        // }
        // grouping.AddEvents<NaamWerdGewijzigd>(e => insz, @events);

    }
}
