namespace AssociationRegistry.Admin.ProjectionHost.Projections.Locaties;

using Events;
using Grar;
using Marten;
using Marten.Events;
using Marten.Events.Aggregation;
using Marten.Events.Projections;
using Schema.Detail;
using System.Data;
using Vereniging;

public class LocatieZonderAdresMatchProjection : MultiStreamProjection<LocatieZonderAdresMatchDocument, string>
{
    public LocatieZonderAdresMatchProjection(ILogger<LocatieZonderAdresMatchProjection> logger)
    {
        Options.BatchSize = 1;
        Options.EnableDocumentTrackingByIdentity = true;
        Options.MaximumHopperSize = 1;
        Options.DeleteViewTypeOnTeardown<LocatieZonderAdresMatchDocument>();

        IncludeType<FeitelijkeVerenigingWerdGeregistreerd>();

        Identities<FeitelijkeVerenigingWerdGeregistreerd>(@event => @event.Locaties
                                                                          .OrderBy(l => l.LocatieId)
                                                                          .Select(locatie => $"{@event.VCode}-{locatie.LocatieId}")
                                                                          .ToArray());

        CreateEvent<LocatieWerdToegevoegd>(x => $"{x.StreamKey}-{x.Data.Locatie.LocatieId}", @event =>
        {
            if (@event.Data.Locatie.Locatietype == Locatietype.MaatschappelijkeZetelVolgensKbo.Waarde) return null;

            if (@event.Data.Locatie.Adres is null) return null;

            return new LocatieZonderAdresMatchDocument()
            {
                Id = $"{@event.StreamKey}-{@event.Data.Locatie.LocatieId}",
                VCode = @event.StreamKey,
                LocatieId = @event.Data.Locatie.LocatieId
            };
        });

        CreateEvent<LocatieWerdGewijzigd>(x => $"{x.StreamKey}-{x.Data.Locatie.LocatieId}", @event =>
        {
            if (@event.Data.Locatie.Locatietype == Locatietype.MaatschappelijkeZetelVolgensKbo.Waarde) return null;

            if (@event.Data.Locatie.Adres is null) return null;

            return new LocatieZonderAdresMatchDocument()
            {
                Id = $"{@event.StreamKey}-{@event.Data.Locatie.LocatieId}",
                VCode = @event.StreamKey,
                LocatieId = @event.Data.Locatie.LocatieId
            };
        });

        DeleteEvent<LocatieWerdVerwijderd>(x => $"{x.StreamKey}-{x.Data.Locatie.LocatieId}");

        DeleteEvent<AdresWerdOvergenomenUitAdressenregister>(x => $"{x.StreamKey}-{x.Data.LocatieId}");
        DeleteEvent<AdresWerdNietGevondenInAdressenregister>(x => $"{x.StreamKey}-{x.Data.LocatieId}");
        DeleteEvent<AdresNietUniekInAdressenregister>(x => $"{x.StreamKey}-{x.Data.LocatieId}");
        DeleteEvent<AdresWerdOntkoppeldVanAdressenregister>(x => $"{x.StreamKey}-{x.Data.LocatieId}");
        DeleteEvent<LocatieDuplicaatWerdVerwijderdNaAdresMatch>(x => $"{x.StreamKey}-{x.Data.VerwijderdeLocatieId}");

        IncludeType<AdresKonNietOvergenomenWordenUitAdressenregister>();
        Identity<AdresKonNietOvergenomenWordenUitAdressenregister>(x => $"{x.VCode}-{x.LocatieId}");

        DeleteEvent(
            (
                    LocatieZonderAdresMatchDocument doc,
                    AdresKonNietOvergenomenWordenUitAdressenregister adresKonNietOvergenomenWordenUitAdressenregister)
                =>
            {
                switch (adresKonNietOvergenomenWordenUitAdressenregister.Reden)
                {
                    case GrarClient.BadRequestSuccessStatusCodeMessage:
                    case AdresKonNietOvergenomenWordenUitAdressenregister.RedenLocatieWerdVerwijderd:
                        return true;

                    default:
                        return doc.Id is null;
                }
            });

        CustomGrouping(new LocatieZonderAdresMatchGrouper());
        DeleteEvent<VerenigingWerdVerwijderd>();
    }

    public LocatieZonderAdresMatchDocument Create(FeitelijkeVerenigingWerdGeregistreerd @event, IQuerySession session)
    {
        var documentSession = session.DocumentStore.IdentitySession(IsolationLevel.ReadUncommitted);

        foreach (var locatie in @event.Locaties)
        {
            if (locatie.Locatietype == Locatietype.MaatschappelijkeZetelVolgensKbo.Waarde) continue;

            if (locatie.Adres is not null)
                documentSession.Store(new LocatieZonderAdresMatchDocument
                {
                    Id = $"{@event.VCode}-{locatie.LocatieId}",
                    VCode = @event.VCode,
                    LocatieId = locatie.LocatieId,
                });
        }

        documentSession.SaveChanges();

        return null;
    }

    private void CreateEvent<TEvent>(
        Func<IEvent<TEvent>, string> identityFunc,
        Func<IEvent<TEvent>, LocatieZonderAdresMatchDocument> creator) where TEvent : class
    {
        IncludeType<TEvent>();
        Identity(identityFunc);
        CreateEvent(creator);
    }

    private void DeleteEvent<TEvent>(Func<IEvent<TEvent>, string> identityFunc) where TEvent : class
    {
        IncludeType<TEvent>();
        Identity(identityFunc);
        DeleteEvent<TEvent>();
    }
}

public class LocatieZonderAdresMatchGrouper : IAggregateGrouper<string>
{
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

        var result = await session.Query<LocatieZonderAdresMatchDocument>()
                                  .Where(x => vCodes.Contains(x.VCode))
                                  .ToListAsync();

        foreach (var locatieLookupDocument in result)
        {
            var verwijderd = verwijderdEvents.Single(x => x.StreamKey == locatieLookupDocument.VCode);
            grouping.AddEvent(locatieLookupDocument.Id, verwijderd);
        }
    }
}
