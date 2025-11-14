namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid;
using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid.DuplicateVerenigingDetection;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Geotags;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Stubs.VCodeServices;
using Common.StubsMocksFakes.Clocks;
using Common.StubsMocksFakes.Faktories;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Common.StubsMocksFakes.VertegenwoordigerPersoonsgegevensRepositories;
using Marten;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using ResultNet;
using Wolverine.Marten;
using Xunit;

public abstract class RegistreerVZERCommandHandlerTestBase : IAsyncLifetime
{
    protected RegistreerVZERCommandHandlerTestBase()
    {
        Fixture = new Fixture().CustomizeAdminApi();

        var faktory = Faktory.New(Fixture);

        Today = Fixture.Create<DateOnly>();
        Clock = faktory.Clock.Stub(Today);

        VerenigingRepositoryMock = faktory.VerenigingsRepository.Mock();
        VertegenwoordigerPersoonsgegevensRepositoryMock = new VertegenwoordigerPersoonsgegevensRepositoryMock();
        VCodeService = faktory.VCodeService.Stub(Fixture.Create<VCode>());

        Command = CreateCommand();

        (GeotagsServiceMock, Geotags) = faktory.GeotagsService.ReturnsRandomGeotags(
            Command.Locaties,
            Command.Werkingsgebieden);

        CommandHandler =
            new RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler(
                VerenigingRepositoryMock,
                VertegenwoordigerPersoonsgegevensRepositoryMock,
                VCodeService,
                Mock.Of<IMartenOutbox>(),
                Mock.Of<IDocumentSession>(),
                Clock,
                GeotagsServiceMock.Object,
                NullLogger<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler>.Instance);

        CommandMetadata = Fixture.Create<CommandMetadata>();
    }

    /// <summary>
    /// Let each test define its own command (so you can tweak fields like Startdatum easily).
    /// </summary>
    protected abstract RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand CreateCommand();

    public RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler CommandHandler { get; }
    public RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand Command { get; }
    public CommandMetadata CommandMetadata { get; }
    public Result CommandResult { get; private set; } = null!;
    public VerenigingRepositoryMock VerenigingRepositoryMock { get; }
    public VertegenwoordigerPersoonsgegevensRepositoryMock VertegenwoordigerPersoonsgegevensRepositoryMock { get; }
    public StubVCodeService VCodeService { get; }
    public ClockStub Clock { get; }
    public Mock<IGeotagsService> GeotagsServiceMock { get; }
    public GeotagsCollection Geotags { get; }
    public IFixture Fixture { get; }
    public DateOnly Today { get; }

    public virtual async Task ExecuteCommand()
    {
        CommandResult = await CommandHandler.Handle(
            new CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>(Command, CommandMetadata),
            VerrijkteAdressenUitGrar.Empty,
            PotentialDuplicatesFound.None,
            CancellationToken.None);
    }

    public async ValueTask InitializeAsync()
        => await ExecuteCommand();

    public ValueTask DisposeAsync()
        => ValueTask.CompletedTask;
}

