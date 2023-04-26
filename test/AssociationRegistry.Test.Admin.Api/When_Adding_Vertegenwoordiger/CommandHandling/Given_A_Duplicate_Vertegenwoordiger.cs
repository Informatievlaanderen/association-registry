namespace AssociationRegistry.Test.Admin.Api.When_Adding_Vertegenwoordiger.CommandHandling;

using Acties.VoegVertegenwoordigerToe;
using AssociationRegistry.Framework;
using AutoFixture;
using Fakes;
using Fixtures.Scenarios;
using FluentAssertions;
using Framework;
using Framework.MagdaMocks;
using Vereniging.Exceptions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Duplicate_Vertegenwoordiger
{
    private readonly VoegVertegenwoordigerToeCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly VerenigingWerdGeregistreerdScenario _scenario;

    public Given_A_Duplicate_Vertegenwoordiger()
    {
        _scenario = new VerenigingWerdGeregistreerdScenario();
        var verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVereniging());

        _fixture = new Fixture().CustomizeAll();

        _commandHandler = new VoegVertegenwoordigerToeCommandHandler(verenigingRepositoryMock, new MagdaFacadeEchoMock());
    }

    [Fact]
    public async Task Then_A_DuplicateVertegenwoordiger_Is_Thrown()
    {
        var command = _fixture.Create<VoegVertegenwoordigerToeCommand>() with { VCode = _scenario.VCode };

        var commandEnvelope = new CommandEnvelope<VoegVertegenwoordigerToeCommand>(command, _fixture.Create<CommandMetadata>());

        await _commandHandler.Handle(commandEnvelope);
        var handleCall = async () => await _commandHandler.Handle(commandEnvelope);

        await handleCall.Should()
            .ThrowAsync<DuplicateInszProvided>()
            .WithMessage(new DuplicateInszProvided().Message);
    }
}
