namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_Adding_Vertegenwoordiger.CommandHandling;

using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using Common.Scenarios.CommandHandling;
using DecentraalBeheer.Vertegenwoordigers.VoegVertegenwoordigerToe;
using FluentAssertions;
using Vereniging.Exceptions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Duplicate_Vertegenwoordiger
{
    private readonly VoegVertegenwoordigerToeCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly FeitelijkeVerenigingWerdGeregistreerdScenario _scenario;

    public Given_A_Duplicate_Vertegenwoordiger()
    {
        _scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario();
        var verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());

        _fixture = new Fixture().CustomizeAdminApi();

        _commandHandler = new VoegVertegenwoordigerToeCommandHandler(verenigingRepositoryMock);
    }

    [Fact]
    public async Task Then_A_DuplicateVertegenwoordiger_Is_Thrown()
    {
        var command = _fixture.Create<VoegVertegenwoordigerToeCommand>() with { VCode = _scenario.VCode };

        var commandEnvelope = new CommandEnvelope<VoegVertegenwoordigerToeCommand>(command, _fixture.Create<CommandMetadata>());

        await _commandHandler.Handle(commandEnvelope);
        var handleCall = async () => await _commandHandler.Handle(commandEnvelope);

        await handleCall.Should()
                        .ThrowAsync<InszMoetUniekZijn>()
                        .WithMessage(new InszMoetUniekZijn().Message);
    }
}
