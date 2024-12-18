namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingMetRechtspersoonlijkheid.When_Removing_Vertegenwoordiger.CommandHandling;

using Acties.VerwijderVertegenwoordiger;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using Common.Scenarios.CommandHandling;
using FluentAssertions;
using Vereniging.Exceptions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Then_It_Throws
{
    private readonly VerwijderVertegenwoordigerCommandHandler _commandHandler;
    private readonly CommandEnvelope<VerwijderVertegenwoordigerCommand> _envelope;

    public Then_It_Throws()
    {
        var scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario();
        var verenigingRepositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState());

        var fixture = new Fixture().CustomizeAdminApi();
        var command = fixture.Create<VerwijderVertegenwoordigerCommand>() with { VCode = scenario.VCode };
        var commandMetadata = fixture.Create<CommandMetadata>();

        _commandHandler = new VerwijderVertegenwoordigerCommandHandler(verenigingRepositoryMock);
        _envelope = new CommandEnvelope<VerwijderVertegenwoordigerCommand>(command, commandMetadata);
    }

    [Fact]
    public async Task Then_A_UnsupportedOperationException_Is_Thrown()
    {
        var method = () => _commandHandler.Handle(_envelope);
        await method.Should().ThrowAsync<VerenigingMetRechtspersoonlijkheidKanGeenVertegenwoordigersVerwijderen>();
    }
}
