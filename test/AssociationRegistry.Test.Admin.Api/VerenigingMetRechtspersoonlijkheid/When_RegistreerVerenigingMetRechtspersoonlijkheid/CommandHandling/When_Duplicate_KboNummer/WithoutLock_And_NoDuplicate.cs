namespace AssociationRegistry.Test.Admin.Api.VerenigingMetRechtspersoonlijkheid.When_RegistreerVerenigingMetRechtspersoonlijkheid.
    CommandHandling.
    When_Duplicate_KboNummer;

using Acties.RegistreerVerenigingUitKbo;
using AssociationRegistry.Framework;
using AutoFixture;
using EventStore;
using Fakes;
using FluentAssertions;
using Framework;
using Kbo;
using Moq;
using ResultNet;
using Test.Framework;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class WithoutLock_And_NoDuplicate : IAsyncLifetime
{
    private Result _result = null!;
    private readonly RegistreerVerenigingUitKboCommandHandler _commandHandler;
    private readonly CommandEnvelope<RegistreerVerenigingUitKboCommand> _envelope;

    public WithoutLock_And_NoDuplicate()
    {
        var fixture = new Fixture().CustomizeAdminApi();


        _envelope = new CommandEnvelope<RegistreerVerenigingUitKboCommand>(fixture.Create<RegistreerVerenigingUitKboCommand>(),
                                                                           fixture.Create<CommandMetadata>());


        ILockStore lockStoreMock = new NoLockStoreMock();
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
