namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_Adding_Vertegenwoordiger.CommandHandling;

using Acties.VoegVertegenwoordigerToe;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.Framework;
using Common.Scenarios.CommandHandling;
using FluentAssertions;
using Framework;
using Vereniging;
using Vereniging.Exceptions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Second_Primair_Vertegenwoordiger
{
    private readonly VoegVertegenwoordigerToeCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly FeitelijkeVerenigingWerdGeregistreerdScenario _scenario;

    public Given_A_Second_Primair_Vertegenwoordiger()
    {
        _scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario();
        var verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());

        _fixture = new Fixture().CustomizeAdminApi();

        _commandHandler = new VoegVertegenwoordigerToeCommandHandler(verenigingRepositoryMock);
    }

    [Fact]
    public async Task Then_A_DuplicateVertegenwoordiger_Is_Thrown()
    {
        var command = new VoegVertegenwoordigerToeCommand(
            _scenario.VCode,
            _fixture.Create<Vertegenwoordiger>()
                with
                {
                    IsPrimair = true,
                });

        var commandEnvelope = new CommandEnvelope<VoegVertegenwoordigerToeCommand>(command, _fixture.Create<CommandMetadata>());

        var secondCommand = new VoegVertegenwoordigerToeCommand(
            _scenario.VCode,
            _fixture.Create<Vertegenwoordiger>()
                with
                {
                    IsPrimair = true,
                });

        var secondCommandEnvelope = new CommandEnvelope<VoegVertegenwoordigerToeCommand>(secondCommand, _fixture.Create<CommandMetadata>());

        await _commandHandler.Handle(commandEnvelope);
        var handleCall = async () => await _commandHandler.Handle(secondCommandEnvelope);

        await handleCall.Should()
                        .ThrowAsync<MeerderePrimaireVertegenwoordigers>()
                        .WithMessage(new MeerderePrimaireVertegenwoordigers().Message);
    }
}
