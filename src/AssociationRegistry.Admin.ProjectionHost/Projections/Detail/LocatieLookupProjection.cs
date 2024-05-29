﻿namespace AssociationRegistry.Admin.ProjectionHost.Projections.Detail;

using Events;
using Framework;
using Infrastructure.Extensions;
using Marten;
using Marten.Events;
using Marten.Events.Aggregation;
using Marten.Events.Projections;
using Schema.Detail;
using IEvent = Marten.Events.IEvent;

public class LocatieLookupProjection : MultiStreamProjection<LocatieLookupDocument, string>
{
    public LocatieLookupProjection()
    {
        Options.BatchSize = 1;
        Options.MaximumHopperSize = 1;

        Options.DeleteViewTypeOnTeardown<LocatieLookupDocument>();

        Options.EnableDocumentTrackingByIdentity = false;

        Identity<AdresWerdOvergenomenUitAdressenregister>(x => $"{x.VCode}-{x.LocatieId}");
        Identity<LocatieWerdVerwijderd>(x => $"{x.VCode}-{x.Locatie.LocatieId}");
        Identity<AdresWerdNietGevondenInAdressenregister>(x => $"{x.VCode}-{x.LocatieId}");
        Identity<AdresNietUniekInAdressenregister>(x => $"{x.VCode}-{x.LocatieId}");

        CustomGrouping(new LocatieLookupGrouper());

        CreateEvent<AdresWerdOvergenomenUitAdressenregister>(x => new LocatieLookupDocument
        {
            AdresPuri = x.OvergenomenAdresUitAdressenregister.AdresId.Bronwaarde,
            AdresId = new Uri(x.OvergenomenAdresUitAdressenregister.AdresId.Bronwaarde).Segments[^1].TrimEnd('/'),
            LocatieId = x.LocatieId,
            VCode = x.VCode,
        });

        DeleteEvent<LocatieWerdVerwijderd>();
        DeleteEvent<AdresWerdNietGevondenInAdressenregister>();
        DeleteEvent<AdresNietUniekInAdressenregister>();

        DeleteEvent<VerenigingWerdVerwijderd>();
    }
}

public class LocatieLookupGrouper : IAggregateGrouper<string>
{

    public LocatieLookupGrouper()
    {
    }

    public async Task Group(IQuerySession session, IEnumerable<IEvent> events, ITenantSliceGroup<string> grouping)
    {
        var verwijderdEvents = events
                              .OfType<IEvent<VerenigingWerdVerwijderd>>()
                              .ToList();

        if (!verwijderdEvents.Any())
            return;

        var vCodes = verwijderdEvents
                    .Select(e => e.Data.VCode)
                    .ToList();

        var result = await session.Query<LocatieLookupDocument>()
                                  .Where(x => vCodes.Contains(x.VCode))
                                  .ToListAsync();

        foreach (var locatieLookupDocument in result)
        {
            var verwijderd = verwijderdEvents.Single(x => x.StreamKey == locatieLookupDocument.VCode);
            grouping.AddEvent(locatieLookupDocument.Id, verwijderd);
        }
    }
}
