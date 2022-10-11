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
        var vNummerService = new SequentialVNummerService();
        var verenigingsRepository = new VerenigingRepositotyMock();

        var handler = new CreateVerenigingCommandHandler(verenigingsRepository, vNummerService);
        var createVerenigingCommand = new CreateVerenigingCommand("naam1");

        await handler.Handle(createVerenigingCommand);

        var invocation = verenigingsRepository.Invocations.Single();
        invocation.vereniging.VNummer.Should().Be(vNummerService.GetLast());
        var events = invocation.vereniging.Events;
        events.Single().Should().BeEquivalentTo(new VerenigingCreated(vNummerService.GetLast(), "naam1"));
    }
}
