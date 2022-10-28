namespace AssociationRegistry.Test.Admin.Api.UnitTests.CommandHandlerTests.When_a_RegistreerVerenigingCommand_is_received;

using AssociationRegistry.Admin.Api.Verenigingen;
using Events;
using FluentAssertions;
using Xunit;

public class VerenigingRepositoryMock : IVerenigingsRepository
{
    public record Invocation(Vereniging Vereniging);

    public readonly List<Invocation> Invocations = new();

    public async Task Save(Vereniging vereniging)
    {
        Invocations.Add(new Invocation(vereniging));
        await Task.CompletedTask;
    }
}

public class Given_A_RegistreerVerenigingCommandHandler
{
    [Fact]
    public async Task Then_a_new_vereniging_is_saved_in_the_repository()
    {
        var vNummerService = new InMemorySequentialVCodeService();
        var verenigingsRepository = new VerenigingRepositoryMock();

        var handler = new RegistreerVerenigingCommandHandler(verenigingsRepository, vNummerService);
        var registreerVerenigingCommand = new CommandEnvelope<RegistreerVerenigingCommand>(new RegistreerVerenigingCommand("naam1"));

        await handler.Handle(registreerVerenigingCommand, CancellationToken.None);

        var invocation = verenigingsRepository.Invocations.Single();
        invocation.Vereniging.VCode.Should().Be(InMemorySequentialVCodeService.GetLast());
        var events = invocation.Vereniging.Events;
        events.Single().Should().BeEquivalentTo(new VerenigingWerdGeregistreerd(InMemorySequentialVCodeService.GetLast(), "naam1"));
    }
}
