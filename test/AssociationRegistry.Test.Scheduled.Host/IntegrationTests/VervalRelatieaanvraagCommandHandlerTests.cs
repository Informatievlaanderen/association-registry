namespace AssociationRegistry.Test.Scheduled.Host.IntegrationTests;

using AssociationRegistry.Framework;
using CommandHandling.DecentraalBeheer.Acties.Relatieaanvragen.VervalRelatieaanvraag;
using Common.Framework;
using Common.StubsMocksFakes.Clocks;
using DecentraalBeheer.Relatieaanvragen;
using DecentraalBeheer.Vereniging;
using Events;
using EventStore.ConflictResolution;
using FluentAssertions;
using Marten;
using MartenDb.BankrekeningnummerPersoonsgegevens;
using MartenDb.Relatieaanvragen;
using MartenDb.Store;
using MartenDb.Transformers;
using MartenDb.VertegenwoordigerPersoonsgegevens;
using Microsoft.Extensions.Logging.Abstractions;

/// <summary>
///     Two open relatieaanvragen at the koepel, with their guards: a subverenigingsaanvraag whose vervaldatum is over, and
///     a lidmaatschapsaanvraag of another vereniging that is still open. The command for the first comes twice, like a
///     message that is delivered again.
/// </summary>
public class VervalRelatieaanvraagCommandHandlerTestsFixture : IAsyncLifetime
{
    public static readonly VCode Koepel = VCode.Create("V0001001");
    public static readonly VCode Subvereniging = VCode.Create("V0001003");
    public static readonly VCode Lid = VCode.Create("V0001005");
    public static readonly RelatieaanvraagId TeVervallen = RelatieaanvraagId.Create(1);
    public static readonly RelatieaanvraagId NogOpen = RelatieaanvraagId.Create(2);
    public static readonly DateOnly VervalDatum = new(2026, 11, 18);
    public static readonly DateOnly Today = VervalDatum.AddDays(1);

    public IDocumentSession Session { get; private set; } = null!;

    public async ValueTask InitializeAsync()
    {
        var store = await TestDocumentStoreFactory.CreateAsync(
            nameof(VervalRelatieaanvraagCommandHandlerTests),
            options => options.RegisterOpenRelatieaanvraagDocument()
        );

        Session = store.LightweightSession();
        var eventStore = CreateEventStore(Session);

        await DienIn(eventStore, TeVervallen, Subvereniging, RelatieType.Subvereniging, VervalDatum);
        await DienIn(eventStore, NogOpen, Lid, RelatieType.Lidmaatschap, VervalDatum.AddDays(30));

        var handler = new VervalRelatieaanvraagCommandHandler(
            new RelatieaanvraagRepository(Session, eventStore),
            new ClockStub(Today),
            NullLogger<VervalRelatieaanvraagCommandHandler>.Instance
        );

        await handler.Handle(Envelope(TeVervallen));
        await handler.Handle(Envelope(TeVervallen));
    }

    public async ValueTask DisposeAsync()
    {
        await Session.DisposeAsync();
    }

    /// <summary>The events and the guards that the repository saves for a new relatieaanvraag.</summary>
    private async Task DienIn(
        EventStore eventStore,
        RelatieaanvraagId aanvraagId,
        VCode vereniging,
        RelatieType relatieType,
        DateOnly vervalDatum
    )
    {
        Session.Insert(
            Guard(
                OpenRelatieaanvraagDocument.SleutelVoorKoepelEnVereniging(Koepel, vereniging),
                aanvraagId,
                relatieType,
                vereniging
            )
        );

        if (relatieType.IsSubvereniging)
            Session.Insert(
                Guard(
                    OpenRelatieaanvraagDocument.SleutelVoorSubverenigingsaanvraag(vereniging),
                    aanvraagId,
                    relatieType,
                    vereniging
                )
            );

        await eventStore.SaveNew(
            aanvraagId.Value,
            CommandMetadata.ForDigitaalVlaanderenProcess,
            CancellationToken.None,
            [
                new RelatieaanvraagWerdIngediend(
                    aanvraagId.Value,
                    Koepel.Value,
                    vereniging.Value,
                    vereniging.Value,
                    relatieType.Value,
                    "Aanvraag",
                    vervalDatum.AddDays(-90),
                    vervalDatum
                ),
            ]
        );
    }

    private static OpenRelatieaanvraagDocument Guard(
        string sleutel,
        RelatieaanvraagId aanvraagId,
        RelatieType relatieType,
        VCode vereniging
    ) =>
        new()
        {
            Sleutel = sleutel,
            AanvraagId = aanvraagId.Value,
            RelatieType = relatieType.Value,
            VerenigingVCode = vereniging.Value,
        };

    private static CommandEnvelope<VervalRelatieaanvraagCommand> Envelope(RelatieaanvraagId aanvraagId) =>
        new(new VervalRelatieaanvraagCommand(aanvraagId.Value), CommandMetadata.ForDigitaalVlaanderenProcess);

    private static EventStore CreateEventStore(IDocumentSession session) =>
        new(
            session: session,
            conflictResolver: new EventConflictResolver([], []),
            new PersoonsgegevensProcessor(
                new PersoonsgegevensEventTransformers(),
                new VertegenwoordigerPersoonsgegevensRepository(
                    session: session,
                    new VertegenwoordigerPersoonsgegevensQuery(session)
                ),
                new BankrekeningnummerPersoonsgegevensRepository(
                    session: session,
                    new BankrekeningnummerPersoonsgegevensQuery(session)
                ),
                NullLogger<PersoonsgegevensProcessor>.Instance
            ),
            NullLogger<EventStore>.Instance
        );
}

public class VervalRelatieaanvraagCommandHandlerTests(VervalRelatieaanvraagCommandHandlerTestsFixture fixture)
    : IClassFixture<VervalRelatieaanvraagCommandHandlerTestsFixture>
{
    [Fact]
    public async Task Then_The_Relatieaanvraag_Is_Vervallen_Once_Today()
    {
        var events = await fixture.Session.Events.FetchStreamAsync(
            VervalRelatieaanvraagCommandHandlerTestsFixture.TeVervallen.Value
        );

        events.Should().HaveCount(2);

        events[1]
            .Data.Should()
            .Be(
                new RelatieaanvraagWerdVervallen(
                    VervalRelatieaanvraagCommandHandlerTestsFixture.TeVervallen.Value,
                    VervalRelatieaanvraagCommandHandlerTestsFixture.Today
                )
            );
    }

    /// <summary>Both its guards are deleted: a new relatieaanvraag for the same pair is possible again.</summary>
    [Fact]
    public async Task Then_Its_Guards_Are_Deleted()
    {
        var guards = await fixture.Session.Query<OpenRelatieaanvraagDocument>().ToListAsync();

        guards
            .Should()
            .NotContain(guard => guard.AanvraagId == VervalRelatieaanvraagCommandHandlerTestsFixture.TeVervallen.Value);
    }

    [Fact]
    public async Task Then_The_Other_Relatieaanvraag_Keeps_Its_Guard()
    {
        var guards = await fixture.Session.Query<OpenRelatieaanvraagDocument>().ToListAsync();

        guards
            .Should()
            .ContainSingle(guard => guard.AanvraagId == VervalRelatieaanvraagCommandHandlerTestsFixture.NogOpen.Value);
    }
}
