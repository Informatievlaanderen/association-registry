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
public class WithLock_And_LockRemovedWhileWaiting_And_NoDuplicate : IAsyncLifetime
{
    private Result _result = null!;
    private readonly RegistreerVerenigingUitKboCommandHandler _commandHandler;
    private readonly CommandEnvelope<RegistreerVerenigingUitKboCommand> _envelope;

    public WithLock_And_LockRemovedWhileWaiting_And_NoDuplicate()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        _envelope = new CommandEnvelope<RegistreerVerenigingUitKboCommand>(fixture.Create<RegistreerVerenigingUitKboCommand>(),
                                                                           fixture.Create<CommandMetadata>());

        var lockStoreMock = new CountedLockStoreMock(2);
        var repositoryMock = new VerenigingRepositoryMock(lockStore: lockStoreMock);

        _commandHandler = new RegistreerVerenigingUitKboCommandHandler(
            repositoryMock,
            new InMemorySequentialVCodeService(),
            new MagdaGeefVerenigingNumberFoundMagdaGeefVerenigingService());
    }

    public async Task InitializeAsync()
    {
        _result = await _commandHandler
           .Handle(_envelope, CancellationToken.None);
    }

    [Fact]
    public void Then_The_Result_Is_A_Success()
    {
        _result.IsSuccess().Should().BeTrue();
    }

    public Task DisposeAsync()
        => Task.CompletedTask;
}
