namespace AssociationRegistry.Admin.ProjectionHost.Projections.Locaties;

using Events;
using JasperFx.Events;
using JasperFx.Events.Grouping;
using Marten;
using Marten.Events.Aggregation;
using Marten.Events.Projections;
using Schema.Detail;
using IEvent = JasperFx.Events.IEvent;

public class LocatieLookupProjection : MultiStreamProjection<LocatieLookupDocument, string>
{
    public LocatieLookupProjection(ILogger<LocatieLookupProjection> logger)
    {
        Options.BatchSize = 1;
        Options.EnableDocumentTrackingByIdentity = true;
        Options.MaximumHopperSize = 1;

        Options.DeleteViewTypeOnTeardown<LocatieLookupDocument>();

        Identity<AdresWerdOvergenomenUitAdressenregister>(x => $"{x.VCode}-{x.LocatieId}");
        CreateEvent<AdresWerdOvergenomenUitAdressenregister>(x => new LocatieLookupDocument
        {
            AdresPuri = x.AdresId.Bronwaarde,
            AdresId = x.AdresId.ToId(),
            LocatieId = x.LocatieId,
            VCode = x.VCode,
        });

        ProjectEvent<AdresWerdOvergenomenUitAdressenregister>((doc, e) =>
        {
            doc.AdresPuri = e.AdresId.Bronwaarde;
            doc.AdresId = e.AdresId.ToId();
        });

        Identity<AdresWerdGewijzigdInAdressenregister>(x => $"{x.VCode}-{x.LocatieId}");
        ProjectEvent<AdresWerdGewijzigdInAdressenregister>((doc, @event) =>
        {
            doc.AdresPuri = @event.AdresId.Bronwaarde;
            doc.AdresId = new Uri(@event.AdresId.Bronwaarde).Segments[^1].TrimEnd('/');
        });

        Identity<LocatieWerdVerwijderd>(x => $"{x.VCode}-{x.Locatie.LocatieId}");
        DeleteEvent<LocatieWerdVerwijderd>();

        Identity<AdresWerdNietGevondenInAdressenregister>(x => $"{x.VCode}-{x.LocatieId}");
        DeleteEvent<AdresWerdNietGevondenInAdressenregister>();

        Identity<AdresNietUniekInAdressenregister>(x => $"{x.VCode}-{x.LocatieId}");
        DeleteEvent<AdresNietUniekInAdressenregister>();

        Identity<AdresWerdOntkoppeldVanAdressenregister>(x => $"{x.VCode}-{x.LocatieId}");
        DeleteEvent<AdresWerdOntkoppeldVanAdressenregister>();

        Identity<LocatieDuplicaatWerdVerwijderdNaAdresMatch>(x => $"{x.VCode}-{x.VerwijderdeLocatieId}");
        DeleteEvent<LocatieDuplicaatWerdVerwijderdNaAdresMatch>();

        CustomGrouping(new LocatieLookupGrouper(logger));
        DeleteEvent<VerenigingWerdVerwijderd>();
    }
}

public class LocatieLookupGrouper : IAggregateGrouper<string>
{
    private readonly ILogger _logger;

    public LocatieLookupGrouper(ILogger logger)
    {
        _logger = logger;
    }

    public async Task Group(IQuerySession session, IEnumerable<IEvent> events, IEventGrouping<string> grouping)
    {
        var verwijderdEvents = events
                              .OfType<IEvent<VerenigingWerdVerwijderd>>()
                              .ToList();

        if (!verwijderdEvents.Any())
            return;

        foreach (var verwijderd in verwijderdEvents)
        {
            _logger.LogInformation($"[{verwijderd.StreamKey}]\tFound Verwijderd event with id {verwijderd.Sequence}");
        }

        var vCodes = verwijderdEvents
                    .Select(e => e.Data.VCode)
                    .ToList();

        var result = await session.Query<LocatieLookupDocument>()
                                  .Where(x => vCodes.Contains(x.VCode))
                                  .ToListAsync();

        foreach (var verwijderd in verwijderdEvents)
        {
            _logger.LogInformation($"[{verwijderd.StreamKey}]\tFound {result.Count} related lookup documents");
        }

        foreach (var locatieLookupDocument in result)
        {
            var verwijderd = verwijderdEvents.Single(x => x.StreamKey == locatieLookupDocument.VCode);
            grouping.AddEvent(locatieLookupDocument.Id, verwijderd);

            _logger.LogInformation(
                $"[{verwijderd.StreamKey}]\tAdding event {verwijderd.Sequence} to lookup document {locatieLookupDocument.Id}");
        }
    }
}
