namespace AssociationRegistry.Test.Admin.ExpiredBewaartermijn.E2E;

using AssociationRegistry.Admin.Schema.Persoonsgegevens;
using AssociationRegistry.Bewaartermijnen.PurgeRunner;
using AssociationRegistry.Framework;
using AutoFixture;
using CommandHandling.Bewaartermijnen.Acties.Verlopen;
using Common.AutoFixture;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Bewaartermijnen;
using Events;
using FluentAssertions;
using Marten;
using MartenDb.Store;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NodaTime;

public class VerwijderVertegenwoordigerPersoonsgegevensCommandFixture : IAsyncLifetime
{
    private IHost _host = null!;
    private IEventStore _store;
    public IDocumentSession Session { get; set; }
    public string VCode { get; set; }
    public int VertegenwoordigerId { get; set; }
    public string Reden { get; set; }
    public Instant Vervaldag { get; set; }

    public async ValueTask InitializeAsync()
    {
        var fixture = new Fixture().CustomizeDomain();

        _host = Program.BuildHost();

        await using var scope = _host.Services.CreateAsyncScope();
        var sp = scope.ServiceProvider;

        var documentStore = sp.GetRequiredService<IDocumentStore>();
        await documentStore.Advanced.ResetAllData();

        _store = sp.GetRequiredService<IEventStore>();
        Session = sp.GetRequiredService<IDocumentSession>();

        var handler = sp.GetRequiredService<VerloopBewaartermijnCommandHandler>();

        await InsertScenario(fixture);

        var commandEnvelope = fixture.Create<CommandEnvelope<VerloopBewaartermijnCommand>>() with
        {
            Command = new VerloopBewaartermijnCommand(VCode, VertegenwoordigerId, Reden, Vervaldag),
        };

        await handler.Handle(commandEnvelope);
    }

    public async ValueTask DisposeAsync()
    {
        await _host.StopAsync();
        _host.Dispose();
    }

    private async Task InsertScenario(Fixture fixture)
    {
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
            BewaartermijnId =
            BewaartermijnId.CreateId(DecentraalBeheer.Vereniging.VCode.Create(verenigingWerdGeregistreerd.VCode),
                                     PersoonsgegevensType.Vertegenwoordigers,
                                     vertegenwoordigerWerdVerwijderd.VertegenwoordigerId),
        };

        await _store.SaveNew(verenigingWerdGeregistreerd.VCode,
                             CommandMetadata.ForDigitaalVlaanderenProcess,
                             CancellationToken.None,
                             [
                                 verenigingWerdGeregistreerd, vertergenwoordigerWerdGewijzigd,
                                 vertegenwoordigerWerdVerwijderd,
                             ]);

        await _store.SaveNew(bewaartermijnWerdGestartV2.BewaartermijnId,
                             CommandMetadata.ForDigitaalVlaanderenProcess,
                             CancellationToken.None,
                             [bewaartermijnWerdGestartV2]);

        VCode = verenigingWerdGeregistreerd.VCode;
        VertegenwoordigerId = vertergenwoordigerWerdGewijzigd.VertegenwoordigerId;
    }
}

public class
    VerloopBewaartermijnCommandHandlerTests : IClassFixture<
    VerwijderVertegenwoordigerPersoonsgegevensCommandFixture>
{
    private readonly IDocumentSession _session;
    private readonly string _vCode;
    private readonly int _vertegenwoordigerId;
    private readonly Instant _vervaldag;
    private readonly string _reden;

    public VerloopBewaartermijnCommandHandlerTests(
        VerwijderVertegenwoordigerPersoonsgegevensCommandFixture fixture)
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
        var bewaartermijnWerdVerlopen =
            await _session.Events.QueryRawEventDataOnly<BewaartermijnWerdVerlopen>().FirstAsync();

        bewaartermijnWerdVerlopen.Should()
                                 .BeEquivalentTo(new BewaartermijnWerdVerlopen(
                                                     BewaartermijnId.CreateId(
                                                         VCode.Create(_vCode),
                                                         PersoonsgegevensType.Vertegenwoordigers,
                                                         _vertegenwoordigerId),
                                                     _vCode,
                                                     PersoonsgegevensType.Vertegenwoordigers.Value,
                                                     _vertegenwoordigerId,
                                                     _reden,
                                                     _vervaldag));
    }

    [Fact]
    public async Task Then_It_Saves_VertegenwoordigerPersoonsGegevensWerdenGeanonimiseerd_Event_On_Vereniging()
    {
        var vertegenwoordigerPersoonsGegevensWerdenGeanonimiseerd =
            await _session.Events.QueryRawEventDataOnly<VertegenwoordigerPersoonsGegevensWerdenGeanonimiseerd>()
                          .FirstAsync();

        vertegenwoordigerPersoonsGegevensWerdenGeanonimiseerd.Should()
                                                             .BeEquivalentTo(
                                                                  new
                                                                      VertegenwoordigerPersoonsGegevensWerdenGeanonimiseerd(
                                                                          _vCode,
                                                                          _vertegenwoordigerId,
                                                                          _reden,
                                                                          _vervaldag));
    }
}
