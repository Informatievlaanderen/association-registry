namespace AssociationRegistry.Test.Admin.Api.VerenigingMetRechtspersoonlijkheid.When_RegistreerVerenigingMetRechtspersoonlijkheid.
    CommandHandling.
    When_Duplicate_KboNummer;

using Acties.RegistreerVerenigingUitKbo;
using AssociationRegistry.Framework;
using AutoFixture;
using DuplicateVerenigingDetection;
using EventStore;
using Fakes;
using FluentAssertions;
using Framework;
using Kbo;
using Moq;
using ResultNet;
using Test.Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class WithLock_And_LockRemovedWhileWaiting_And_Duplicate : IAsyncLifetime
{
    private Result _result = null!;
    private readonly RegistreerVerenigingUitKboCommandHandler _commandHandler;
    private readonly CommandEnvelope<RegistreerVerenigingUitKboCommand> _envelope;
    private readonly VerenigingsRepository.VCodeAndNaam _moederVCodeAndNaam;
    private readonly Mock<IMagdaGeefVerenigingService> _magdaGeefVerenigingService;

    public WithLock_And_LockRemovedWhileWaiting_And_Duplicate()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        _moederVCodeAndNaam = fixture.Create<VerenigingsRepository.VCodeAndNaam>();

        _envelope = new CommandEnvelope<RegistreerVerenigingUitKboCommand>(fixture.Create<RegistreerVerenigingUitKboCommand>(),
                                                                           fixture.Create<CommandMetadata>());

        _magdaGeefVerenigingService = new Mock<IMagdaGeefVerenigingService>();

        ILockStore lockStoreMock = new CountedLockStoreMock(2);
        var repositoryMock = new VerenigingRepositoryMock(moederVCodeAndNaam: _moederVCodeAndNaam, lockStore: lockStoreMock);

        _commandHandler = new RegistreerVerenigingUitKboCommandHandler(
            repositoryMock,
            new InMemorySequentialVCodeService(),
            _magdaGeefVerenigingService.Object);
    }

    public async Task InitializeAsync()
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

    public Task DisposeAsync()
        => Task.CompletedTask;
}
