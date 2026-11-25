namespace AssociationRegistry.MartenDb.Relatieaanvragen;

using AssociationRegistry.Events;
using AssociationRegistry.EventStore;
using DecentraalBeheer.Relatieaanvragen;
using DecentraalBeheer.Relatieaanvragen.Exceptions;
using DecentraalBeheer.Vereniging;
using Framework;
using JasperFx;
using Marten;

public class RelatieaanvraagRepository : IRelatieaanvraagRepository
{
    private readonly IDocumentSession _session;
    private readonly IEventStore _eventStore;

    public RelatieaanvraagRepository(IDocumentSession session, IEventStore eventStore)
    {
        _session = session;
        _eventStore = eventStore;
    }

    public async Task<OpenRelatieaanvragen> GetOpenRelatieaanvragen(
        VCode koepelVCode,
        VCode verenigingVCode,
        CancellationToken cancellationToken
    )
    {
        var sleutelVoorKoepelEnVereniging = OpenRelatieaanvraagDocument.SleutelVoorKoepelEnVereniging(
            koepelVCode,
            verenigingVCode
        );

        var sleutelVoorSubverenigingsaanvraag = OpenRelatieaanvraagDocument.SleutelVoorSubverenigingsaanvraag(
            verenigingVCode
        );

        var documenten = await _session.LoadManyAsync<OpenRelatieaanvraagDocument>(
            cancellationToken,
            sleutelVoorKoepelEnVereniging,
            sleutelVoorSubverenigingsaanvraag
        );

        var lidmaatschapsaanvragen = await GetOpenLidmaatschapsaanvragen(verenigingVCode, cancellationToken);

        return new OpenRelatieaanvragen(
            documenten.SingleOrDefault(x => x.Sleutel == sleutelVoorKoepelEnVereniging)?.ToOpenRelatieaanvraag(),
            documenten.SingleOrDefault(x => x.Sleutel == sleutelVoorSubverenigingsaanvraag)?.ToOpenRelatieaanvraag(),
            lidmaatschapsaanvragen.Select(x => x.ToOpenRelatieaanvraag()).ToArray()
        );
    }

    /// <summary>
    ///     A query, not a guard: two concurrent relatieaanvragen of another type for the same vereniging (a lidmaatschap
    ///     and a subvereniging) can both pass. Accepted (2026-11-20): confirming a relatieaanvraag checks the rules again.
    /// </summary>
    private async Task<IReadOnlyList<OpenRelatieaanvraagDocument>> GetOpenLidmaatschapsaanvragen(
        VCode verenigingVCode,
        CancellationToken cancellationToken
    )
    {
        var vereniging = verenigingVCode.Value;
        var lidmaatschap = RelatieType.Lidmaatschap.Value;

        return await _session
            .Query<OpenRelatieaanvraagDocument>()
            .Where(x => x.VerenigingVCode == vereniging && x.RelatieType == lidmaatschap)
            .OrderBy(x => x.AanvraagId)
            .ToListAsync(cancellationToken);
    }

    public async Task<StreamActionResult> SaveNew(
        Relatieaanvraag relatieaanvraag,
        CommandMetadata metadata,
        CancellationToken cancellationToken
    )
    {
        // Same session as the event store: the guards are committed in the same transaction as the events.
        foreach (var openRelatieaanvraag in OpenRelatieaanvraagDocument.VoorNieuweAanvraag(relatieaanvraag))
            _session.Insert(openRelatieaanvraag);

        try
        {
            return await _eventStore.SaveNew(
                relatieaanvraag.AanvraagId.Value,
                metadata,
                cancellationToken,
                relatieaanvraag.UncommittedEvents.ToArray()
            );
        }
        catch (DocumentAlreadyExistsException)
        {
            // A concurrent relatieaanvraag for the same koepel and vereniging was saved first.
            _session.EjectAllPendingChanges();

            throw new RelatieaanvraagStaatAlOpen();
        }
    }

    public Task<RelatieaanvraagState> Load(RelatieaanvraagId aanvraagId, long? expectedVersion) =>
        _eventStore.Load<RelatieaanvraagState>(aanvraagId.Value, expectedVersion);

    public async Task<StreamActionResult> SaveBevestiging(
        RelatieaanvraagState relatieaanvraag,
        RelatieaanvraagWerdBevestigd bevestigd,
        VerenigingsBase vereniging,
        CommandMetadata metadata,
        CancellationToken cancellationToken
    )
    {
        ReleaseGuards(relatieaanvraag);

        var results = await _eventStore.SaveTransactional(
            metadata,
            cancellationToken,
            new AggregateEvents(relatieaanvraag.AanvraagId.Value, relatieaanvraag.Version, [bevestigd]),
            new AggregateEvents(vereniging.VCode, vereniging.Version, vereniging.UncommittedEvents.ToArray())
        );

        return new StreamActionResult(Sequence: results.Max(result => result.Sequence), Version: results[0].Version);
    }

    public async Task<StreamActionResult> SaveAfsluiting(
        RelatieaanvraagState relatieaanvraag,
        IRelatieaanvraagEvent afsluiting,
        CommandMetadata metadata,
        CancellationToken cancellationToken
    )
    {
        ReleaseGuards(relatieaanvraag);

        var results = await _eventStore.SaveTransactional(
            metadata,
            cancellationToken,
            new AggregateEvents(relatieaanvraag.AanvraagId.Value, relatieaanvraag.Version, [afsluiting])
        );

        return results[0];
    }

    /// <summary>
    ///     A closed relatieaanvraag no longer counts as open: every guard it holds is deleted, by aanvraagId. In the same
    ///     session as the event store, so in the same transaction as the events.
    /// </summary>
    private void ReleaseGuards(RelatieaanvraagState relatieaanvraag)
    {
        var aanvraagId = relatieaanvraag.AanvraagId.Value;

        _session.DeleteWhere<OpenRelatieaanvraagDocument>(document => document.AanvraagId == aanvraagId);
    }
}
