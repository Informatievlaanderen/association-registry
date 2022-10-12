namespace AssociationRegistry.Test.Admin.Api.UnitTests.CommandHandlerTests.When_a_CreateVerenigingCommand_is_received;

using AssociationRegistry.Admin.Api.Verenigingen;
using FluentAssertions;
using Moq;
using Xunit;

public class VerenigingRepositotyMock : IVerenigingsRepository
{
    public record Invocation(Vereniging vereniging);

    public readonly List<Invocation> Invocations = new();

    public async Task Save(Vereniging vereniging)
        => Invocations.Add(new Invocation(vereniging));
}

public class Given_A_CreateVerenigingCommandHandler
{
    [Fact]
    public async Task Then_a_new_vereniging_is_saved_in_the_repository()
    {
        var vNummerService = new SequentialVCodeService();
        var verenigingsRepository = new VerenigingRepositotyMock();

        var handler = new CreateVerenigingCommandHandler(verenigingsRepository, vNummerService);
        var createVerenigingCommand = new CommandEnvelope<CreateVerenigingCommand>(new CreateVerenigingCommand("naam1"));

        await handler.Handle(createVerenigingCommand, CancellationToken.None);

        var invocation = verenigingsRepository.Invocations.Single();
        invocation.vereniging.VCode.Should().Be(SequentialVCodeService.GetLast());
        var events = invocation.vereniging.Events;
        events.Single().Should().BeEquivalentTo(new VerenigingWerdGeregistreerd(SequentialVCodeService.GetLast(), "naam1"));
    }
}
