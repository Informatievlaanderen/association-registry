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
using Test.Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class WithLock_And_LockNotRemovedWhileWaiting
{
    private readonly RegistreerVerenigingUitKboCommandHandler _commandHandler;
    private readonly CommandEnvelope<RegistreerVerenigingUitKboCommand> _envelope;

    public WithLock_And_LockNotRemovedWhileWaiting()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var moederVCodeAndNaam = fixture.Create<VerenigingsRepository.VCodeAndNaam>();

        _envelope = new CommandEnvelope<RegistreerVerenigingUitKboCommand>(fixture.Create<RegistreerVerenigingUitKboCommand>(),
                                                                           fixture.Create<CommandMetadata>());

        ILockStore lockStoreMock = new AlwaysLockStoreMock();
        var repositoryMock = new VerenigingRepositoryMock(moederVCodeAndNaam: moederVCodeAndNaam, lockStore: lockStoreMock);

        _commandHandler = new RegistreerVerenigingUitKboCommandHandler(
            repositoryMock,
            new InMemorySequentialVCodeService(),
            Mock.Of<IMagdaGeefVerenigingService>());
    }

    [Fact]
    public void Then_It_Throws_An_Exception()
    {
        var handle = async () => await _commandHandler
           .Handle(_envelope, CancellationToken.None);

        handle.Should().ThrowAsync<ApplicationException>()
              .WithMessage($"Kan niet langer wachten op lock voor KBO nummer {_envelope.Command.KboNummer}");
    }
}
