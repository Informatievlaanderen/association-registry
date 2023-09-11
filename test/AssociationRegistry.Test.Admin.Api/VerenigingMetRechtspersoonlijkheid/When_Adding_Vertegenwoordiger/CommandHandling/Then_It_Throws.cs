namespace AssociationRegistry.Test.Admin.Api.VerenigingMetRechtspersoonlijkheid.When_Adding_Vertegenwoordiger.CommandHandling;

using Acties.VoegVertegenwoordigerToe;
using AssociationRegistry.Framework;
using AutoFixture;
using Fakes;
using Fixtures.Scenarios.CommandHandling;
using FluentAssertions;
using Framework;
using Vereniging.Exceptions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Then_It_Throws
{
    private readonly VoegVertegenwoordigerToeCommandHandler _commandHandler;
    private readonly CommandEnvelope<VoegVertegenwoordigerToeCommand> _envelope;

    public Then_It_Throws()
    {
        var scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario();
        var verenigingRepositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState());

        var fixture = new Fixture().CustomizeAdminApi();
        var command = fixture.Create<VoegVertegenwoordigerToeCommand>() with { VCode = scenario.VCode };
        var commandMetadata = fixture.Create<CommandMetadata>();

        _commandHandler = new VoegVertegenwoordigerToeCommandHandler(verenigingRepositoryMock);
        _envelope = new CommandEnvelope<VoegVertegenwoordigerToeCommand>(command, commandMetadata);
    }

    [Fact]
    public async Task Then_A_UnsupportedOperationException_Is_Thrown()
    {
        var method = () => _commandHandler.Handle(_envelope);
        await method.Should().ThrowAsync<UnsupportedOperationForVerenigingstype>();
    }
}
