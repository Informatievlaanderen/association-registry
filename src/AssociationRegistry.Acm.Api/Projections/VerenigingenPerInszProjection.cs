namespace AssociationRegistry.Acm.Api.Projections;

using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Events;
using JasperFx.Core;
using Marten;
using Marten.Events;
using Marten.Events.Aggregation;
using Marten.Events.Projections;
using Marten.Storage;
using Schema.VerenigingenPerInsz;

public class VerenigingenPerInszProjection : MultiStreamAggregation<VerenigingenPerInszDocument, string>
{
    public VerenigingenPerInszProjection()
    {
        CustomGrouping(new CustomSlicer());
    }

    public void Apply(VerenigingWerdGeregistreerd @event, VerenigingenPerInszDocument document)
    {
        document.Verenigingen.Add(new Vereniging(@event.VCode, @event.Naam));
    }

    public async Task Apply(NaamWerdGewijzigd e, VerenigingenPerInszDocument document)
    {
        document.Verenigingen.Single(vereniging => vereniging.VCode == e.VCode).Naam = e.Naam;
    }
}

public class CustomSlicer: IEventSlicer<VerenigingenPerInszDocument, string>
{
    public ValueTask<IReadOnlyList<EventSlice<VerenigingenPerInszDocument, string>>> SliceInlineActions(
        IQuerySession querySession, IEnumerable<StreamAction> streams)
    {
        var allEvents = streams.SelectMany(x => x.Events).ToList();
        var group = new TenantSliceGroup<VerenigingenPerInszDocument, string>(Tenant.ForDatabase(querySession.Database));
        group.AddEvents<VerenigingWerdGeregistreerd>(@event => @event.Vertegenwoordigers.Select(y => y.Insz).ToArray(), allEvents);

        var naamWerdGewijzigdEvents = allEvents
            .OfType<IEvent<NaamWerdGewijzigd>>()
            .ToList();

        // Take vertegenwoordigers in memory so we only do this query once
        var vCodesWithNaamGewijzigd = naamWerdGewijzigdEvents.Select(x => x.Data.VCode).ToList();
        var vertegenwoordigers = querySession.Query<VerenigingenPerInszDocument>()
            .Where(doc => doc.Verenigingen.Any(x => vCodesWithNaamGewijzigd.Contains(x.VCode)))
            .ToList();

        // Take events in memory so we only do this query once
        var verenigingGeregistreerdEvents = allEvents
            .OfType<IEvent<VerenigingWerdGeregistreerd>>()
            .ToList();

        naamWerdGewijzigdEvents.ForEach(
            naamWerdGewijzigd =>
            {
                vertegenwoordigers.Where(x => x.Verenigingen.Any(v => v.VCode == naamWerdGewijzigd.Data.VCode))
                    .ToList()
                    .ForEach(document => group.AddEvent(document.Insz, naamWerdGewijzigd));

                var geregistreerdEvent = verenigingGeregistreerdEvents.SingleOrDefault(geregistreerd => geregistreerd.Data.VCode == naamWerdGewijzigd.Data.VCode);
                if(geregistreerdEvent is not null)
                    geregistreerdEvent.Data.Vertegenwoordigers.ForEach(x => group.AddEvent(x.Insz, naamWerdGewijzigd));
            });

        return new(group.Slices.ToList());
    }

    public ValueTask<IReadOnlyList<TenantSliceGroup<VerenigingenPerInszDocument, string>>> SliceAsyncEvents(
        IQuerySession querySession, List<IEvent> events)
    {
        var group = new TenantSliceGroup<VerenigingenPerInszDocument, string>(Tenant.ForDatabase(querySession.Database));
        group.AddEvents<VerenigingWerdGeregistreerd>(@event => @event.Vertegenwoordigers.Select(y => y.Insz).ToArray(), events);

        var naamWerdGewijzigdEvents = events
            .OfType<IEvent<NaamWerdGewijzigd>>()
            .ToList();

        var verenigingGeregistreerdEvents = events
            .OfType<IEvent<VerenigingWerdGeregistreerd>>()
            .ToList();

        var vCodes = naamWerdGewijzigdEvents.Select(x => x.Data.VCode).ToList();
        var documentsThatAlreadyExist = querySession.Query<VerenigingenPerInszDocument>()
            .Where(doc => doc.Verenigingen.Any(x => vCodes.Contains(x.VCode)))
            .ToList();

        naamWerdGewijzigdEvents.ForEach(
            naamWerdGewijzigd =>
            {
                documentsThatAlreadyExist.Where(x => x.Verenigingen.Any(v => v.VCode == naamWerdGewijzigd.Data.VCode))
                    .ToList()
                    .ForEach(document => group.AddEvent(document.Insz, naamWerdGewijzigd));

                var geregistreerdEvent = verenigingGeregistreerdEvents.SingleOrDefault(geregistreerd => geregistreerd.Data.VCode == naamWerdGewijzigd.Data.VCode);
                if(geregistreerdEvent is not null)
                    geregistreerdEvent.Data.Vertegenwoordigers.ForEach(x => group.AddEvent(x.Insz, naamWerdGewijzigd));
            });


        return new(new List<TenantSliceGroup<VerenigingenPerInszDocument, string>>{group});
    }
}
