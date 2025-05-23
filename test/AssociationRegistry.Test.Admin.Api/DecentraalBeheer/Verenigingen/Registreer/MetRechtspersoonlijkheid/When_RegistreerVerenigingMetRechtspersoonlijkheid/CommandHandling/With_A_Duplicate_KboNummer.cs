namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.MetRechtspersoonlijkheid.When_RegistreerVerenigingMetRechtspersoonlijkheid.
    CommandHandling;

using AssociationRegistry.DecentraalBeheer.Registratie.RegistreerVerenigingUitKbo;
using AssociationRegistry.DuplicateVerenigingDetection;
using AssociationRegistry.Framework;
using AssociationRegistry.Kbo;
using AssociationRegistry.Test.Admin.Api.Framework.Fakes;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Common.Stubs.VCodeServices;
using Common.StubsMocksFakes.VerenigingsRepositories;
using FluentAssertions;
using Marten;
using Microsoft.Extensions.Logging;
using Moq;
using ResultNet;

using Xunit;

public class With_A_Duplicate_KboNummer : IAsyncLifetime
{
    private Result _result = null!;
    private readonly RegistreerVerenigingUitKboCommandHandler _commandHandler;
    private readonly CommandEnvelope<RegistreerVerenigingUitKboCommand> _envelope;
    private readonly VerenigingState _moederVCodeAndNaam;
    private readonly Mock<IMagdaGeefVerenigingService> _magdaGeefVerenigingService;
    private readonly LoggerFactory _loggerFactory;
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly InMemorySequentialVCodeService _vCodeService;

    public With_A_Duplicate_KboNummer()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        _moederVCodeAndNaam = new VerenigingState
        {
            Identity = fixture.Create<VCode>(),
            Verenigingstype = Verenigingstype.VZW,
        };

        _loggerFactory = new LoggerFactory();
        _verenigingRepositoryMock = new VerenigingRepositoryMock(_moederVCodeAndNaam);
        _vCodeService = new InMemorySequentialVCodeService();

        _envelope = new CommandEnvelope<RegistreerVerenigingUitKboCommand>(fixture.Create<RegistreerVerenigingUitKboCommand>(),
                                                                           fixture.Create<CommandMetadata>());

        _magdaGeefVerenigingService = new Mock<IMagdaGeefVerenigingService>();

        var commandHandlerLogger = _loggerFactory.CreateLogger<RegistreerVerenigingUitKboCommandHandler>();

        _commandHandler = new RegistreerVerenigingUitKboCommandHandler(
            new VerenigingRepositoryMock(_moederVCodeAndNaam),
            new InMemorySequentialVCodeService(),
            _magdaGeefVerenigingService.Object,
            new MagdaRegistreerInschrijvingServiceMock(Result.Success()),
            Mock.Of<IDocumentSession>(),
            commandHandlerLogger);
    }

    public async ValueTask InitializeAsync()
    {
        _result = await _commandHandler
           .Handle(_envelope, CancellationToken.None);
    }

    [Fact]
    public void Then_The_Result_Is_A_Failure()
    {
        _result.IsFailure().Should().BeTrue();
    }

    [Fact]
    public void Then_The_Result_Contains_The_Duplicate_VCode()
    {
        ((Result<DuplicateKboFound>)_result).Data.VCode.Should().BeEquivalentTo(_moederVCodeAndNaam.VCode);
    }

    [Fact]
    public void Then_The_MagdaService_Is_Not_Invoked()
    {
        _magdaGeefVerenigingService.Invocations.Should().BeEmpty();
    }

    public ValueTask DisposeAsync()
        => ValueTask.CompletedTask;
}
