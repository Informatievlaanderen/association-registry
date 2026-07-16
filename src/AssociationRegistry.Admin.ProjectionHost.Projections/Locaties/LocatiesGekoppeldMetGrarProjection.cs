namespace AssociationRegistry.Admin.ProjectionHost.Projections.Locaties;

using Events;
using JasperFx.Events;
using JasperFx.Events.Grouping;
using JasperFx.Events.Projections;
using Marten;
using Marten.Events.Aggregation;
using Marten.Events.Projections;
using Microsoft.Extensions.Logging;
using Schema.Detail;
using Schema.Locaties;
using IEvent = JasperFx.Events.IEvent;

public class LocatiesGekoppeldMetGrarProjection : MultiStreamProjection<LocatieLookupDocument, string>
{
    public static readonly ShardName ShardName = new("beheer.postgres.locatiesgekoppeldmetgrar");

    public LocatiesGekoppeldMetGrarProjection(ILogger<LocatiesGekoppeldMetGrarProjection> logger)
    {
        Name = ShardName.Name;

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

        ProjectEvent<AdresWerdOvergenomenUitAdressenregister>(
            (doc, e) =>
            {
                doc.AdresPuri = e.AdresId.Bronwaarde;
                doc.AdresId = e.AdresId.ToId();
            }
        );

        Identity<AdresWerdGewijzigdInAdressenregister>(x => $"{x.VCode}-{x.LocatieId}");
        ProjectEvent<AdresWerdGewijzigdInAdressenregister>(
            (doc, @event) =>
            {
                doc.AdresPuri = @event.AdresId.Bronwaarde;
                doc.AdresId = new Uri(@event.AdresId.Bronwaarde).Segments[^1].TrimEnd('/');
            }
        );

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
        DeleteEvent<Archived>();
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
        var eventList = events.ToList();

        var verwijderdEvents = eventList.OfType<IEvent<VerenigingWerdVerwijderd>>().Cast<IEvent>().ToList();

        var archivedEvents = eventList.OfType<IEvent<Archived>>().Cast<IEvent>().ToList();

        var deletionEvents = verwijderdEvents.Concat(archivedEvents).ToList();

        if (!deletionEvents.Any())
            return;

        foreach (var deletion in deletionEvents)
        {
            _logger.LogInformation(
                $"[{deletion.StreamKey}]\tFound deletion event {deletion.EventTypeName} with id {deletion.Sequence}"
            );
        }

        var vCodes = deletionEvents.Select(e => e.StreamKey!).Distinct().ToList();

        var result = await session.Query<LocatieLookupDocument>().Where(x => vCodes.Contains(x.VCode)).ToListAsync();

        foreach (var deletion in deletionEvents)
        {
            _logger.LogInformation($"[{deletion.StreamKey}]\tFound {result.Count} related lookup documents");
        }

        foreach (var locatieLookupDocument in result)
        {
            var deletion = deletionEvents.Single(x => x.StreamKey == locatieLookupDocument.VCode);
            grouping.AddEvent(locatieLookupDocument.Id, deletion);

            _logger.LogInformation(
                $"[{deletion.StreamKey}]\tAdding event {deletion.Sequence} to lookup document {locatieLookupDocument.Id}"
            );
        }
    }
}
