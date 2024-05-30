namespace AssociationRegistry.Test.Admin.Api.VerenigingMetRechtspersoonlijkheid.When_RegistreerVerenigingMetRechtspersoonlijkheid.
    CommandHandling;

using Acties.RegistreerVerenigingUitKbo;
using AssociationRegistry.Framework;
using AutoFixture;
using DuplicateVerenigingDetection;
using Fakes;
using FluentAssertions;
using Framework;
using Kbo;
using Marten;
using Microsoft.Extensions.Logging;
using Moq;
using ResultNet;
using Vereniging;
using Wolverine.Marten;
using Xunit;
using Xunit.Categories;

[UnitTest]
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

        _commandHandler = new RegistreerVerenigingUitKboCommandHandler();
    }

    public async Task InitializeAsync()
    {
        var commandHandlerLogger = _loggerFactory.CreateLogger<RegistreerVerenigingUitKboCommandHandler>();
        _result = await _commandHandler
           .Handle(_envelope, new VerenigingRepositoryMock(_moederVCodeAndNaam),
                                          new InMemorySequentialVCodeService(),
                                          _magdaGeefVerenigingService.Object,
                                          new MagdaRegistreerInschrijvingServiceMock(Result.Success()),
                                          commandHandlerLogger, CancellationToken.None);
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

    public Task DisposeAsync()
        => Task.CompletedTask;
}
