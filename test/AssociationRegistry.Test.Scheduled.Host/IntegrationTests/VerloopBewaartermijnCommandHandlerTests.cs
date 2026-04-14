namespace AssociationRegistry.Test.Scheduled.Host.IntegrationTests;

using Admin.Schema.Persoonsgegevens;
using AssociationRegistry.Framework;
using AutoFixture;
using CommandHandling.Bewaartermijnen.Acties.Verlopen;
using Common.AutoFixture;
using Common.Framework;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Bewaartermijnen;
using Events;
using EventStore.ConflictResolution;
using FluentAssertions;
using Marten;
using MartenDb.BankrekeningnummerPersoonsgegevens;
using MartenDb.Store;
using MartenDb.Transformers;
using MartenDb.VertegenwoordigerPersoonsgegevens;
using Microsoft.Extensions.Logging.Abstractions;
using NodaTime;

public class VerloopBewaartermijnCommandHandlerTestsClassFixture : IAsyncLifetime
{
    public IDocumentSession Session;
    public string VCode { get; set; }

    public Instant Vervaldag { get; set; }
    public int VertegenwoordigerId { get; set; }

    public string Reden { get; set; }
    private EventStore _eventStore;

    public async ValueTask InitializeAsync()
    {
        await SetupStore();
        await InsertScenario();
        await Handle();
    }

    private async Task Handle()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var handler = new VerloopBewaartermijnCommandHandler(new VertegenwoordigerPersoonsgegevensRepository(Session, new VertegenwoordigerPersoonsgegevensQuery(Session)), Session);

        var commandEnvelope = fixture.Create<CommandEnvelope<VerloopBewaartermijnCommand>>() with
        {
            Command = new VerloopBewaartermijnCommand(VCode, VertegenwoordigerId, Reden, Vervaldag),
        };

        await handler.Handle(commandEnvelope);
    }

    private async Task SetupStore()
    {
        var store = await TestDocumentStoreFactory.CreateAsync(nameof(VerloopBewaartermijnCommandHandlerTests));

        Session = store.LightweightSession();

        var eventConflictResolver = new EventConflictResolver(
            Array.Empty<IEventPreConflictResolutionStrategy>(),
            Array.Empty<IEventPostConflictResolutionStrategy>()
        );

        _eventStore = new EventStore(
            session: Session,
            conflictResolver: eventConflictResolver,
            new PersoonsgegevensProcessor(
                new PersoonsgegevensEventTransformers(),
                new VertegenwoordigerPersoonsgegevensRepository(
                    session: Session,
                    new VertegenwoordigerPersoonsgegevensQuery(Session)
                ),
                new BankrekeningnummerPersoonsgegevensRepository(
                    session: Session,
                    new BankrekeningnummerPersoonsgegevensQuery(Session)
                ),
                NullLogger<PersoonsgegevensProcessor>.Instance
            ),
            NullLogger<EventStore>.Instance
        );
    }

    public async ValueTask DisposeAsync()
    {
        await Session.DisposeAsync();
    }

    private async Task InsertScenario()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        Reden = BewaartermijnReden.VertegenwoordigerWerdVerwijderd;
        Vervaldag = Instant.MinValue;

        var verenigingWerdGeregistreerd = fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        var vertergenwoordigerWerdGewijzigd = fixture.Create<VertegenwoordigerWerdGewijzigd>() with
        {
            VertegenwoordigerId = verenigingWerdGeregistreerd.Vertegenwoordigers.First().VertegenwoordigerId,
        };

        var vertegenwoordigerWerdVerwijderd = fixture.Create<VertegenwoordigerWerdVerwijderd>() with
        {
            VertegenwoordigerId = vertergenwoordigerWerdGewijzigd.VertegenwoordigerId,
        };

        var bewaartermijnWerdGestartV2 = fixture.Create<BewaartermijnWerdGestartV2>() with
        {
            EntityId = vertegenwoordigerWerdVerwijderd.VertegenwoordigerId,
            Vervaldag = Vervaldag,
            Reden = Reden,
            BewaartermijnId = BewaartermijnId.CreateId(
                DecentraalBeheer.Vereniging.VCode.Create(verenigingWerdGeregistreerd.VCode),
                PersoonsgegevensType.Vertegenwoordigers,
                vertegenwoordigerWerdVerwijderd.VertegenwoordigerId
            ),
        };

        await _eventStore.SaveNew(
            verenigingWerdGeregistreerd.VCode,
            CommandMetadata.ForDigitaalVlaanderenProcess,
            CancellationToken.None,
            [verenigingWerdGeregistreerd, vertergenwoordigerWerdGewijzigd, vertegenwoordigerWerdVerwijderd]
        );

        await _eventStore.SaveNew(
            bewaartermijnWerdGestartV2.BewaartermijnId,
            CommandMetadata.ForDigitaalVlaanderenProcess,
            CancellationToken.None,
            [bewaartermijnWerdGestartV2]
        );

        VCode = verenigingWerdGeregistreerd.VCode;
        VertegenwoordigerId = vertergenwoordigerWerdGewijzigd.VertegenwoordigerId;
    }
}

public class VerloopBewaartermijnCommandHandlerTests
    : IClassFixture<VerloopBewaartermijnCommandHandlerTestsClassFixture>
{
    private readonly IDocumentSession _session;
    private readonly string _vCode;
    private readonly int _vertegenwoordigerId;
    private readonly Instant _vervaldag;
    private readonly string _reden;

    public VerloopBewaartermijnCommandHandlerTests(VerloopBewaartermijnCommandHandlerTestsClassFixture fixture)
    {
        _session = fixture.Session;
        _vCode = fixture.VCode;
        _vertegenwoordigerId = fixture.VertegenwoordigerId;
        _reden = fixture.Reden;
        _vervaldag = fixture.Vervaldag;
    }

    [Fact]
    public async Task Then_It_Deletes_Persoonsgegevens()
    {
        var persoonsgegevens = await _session.Query<VertegenwoordigerPersoonsgegevensDocument>().ToListAsync();

        persoonsgegevens.Should().NotBeEmpty();
        persoonsgegevens.Should().NotContain(x => _vCode == x.VCode && x.VertegenwoordigerId == _vertegenwoordigerId);
        persoonsgegevens.Should().Contain(x => _vCode == x.VCode);
    }

    [Fact]
    public async Task Then_It_Saves_BewaartermijnWerdVerlopen_Event_On_Bewaartermijn()
    {
        var bewaartermijnWerdVerlopen = await _session
            .Events.QueryRawEventDataOnly<BewaartermijnWerdVerlopen>()
            .SingleAsync();

        bewaartermijnWerdVerlopen
            .Should()
            .BeEquivalentTo(
                new BewaartermijnWerdVerlopen(
                    BewaartermijnId.CreateId(
                        VCode.Create(_vCode),
                        PersoonsgegevensType.Vertegenwoordigers,
                        _vertegenwoordigerId
                    ),
                    _vCode,
                    PersoonsgegevensType.Vertegenwoordigers.Value,
                    _vertegenwoordigerId,
                    _reden,
                    _vervaldag
                )
            );
    }

    [Fact]
    public async Task Then_It_Saves_VertegenwoordigerPersoonsGegevensWerdenGeanonimiseerd_Event_On_Vereniging()
    {
        var vertegenwoordigerPersoonsGegevensWerdenGeanonimiseerd = await _session
            .Events.QueryRawEventDataOnly<VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd>()
            .SingleAsync();

        vertegenwoordigerPersoonsGegevensWerdenGeanonimiseerd
            .Should()
            .BeEquivalentTo(
                new VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd(
                    _vCode,
                    _vertegenwoordigerId,
                    _reden,
                    _vervaldag
                )
            );
    }
}
