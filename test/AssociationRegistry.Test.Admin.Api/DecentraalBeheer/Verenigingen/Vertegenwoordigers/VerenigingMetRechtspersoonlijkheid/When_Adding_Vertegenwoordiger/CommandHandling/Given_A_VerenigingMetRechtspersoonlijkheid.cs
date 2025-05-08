namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Vertegenwoordigers.VerenigingMetRechtspersoonlijkheid.When_Adding_Vertegenwoordiger.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Vertegenwoordigers.VoegVertegenwoordigerToe;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using AssociationRegistry.Vereniging.Exceptions;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_VerenigingMetRechtspersoonlijkheid
{
    private readonly VoegVertegenwoordigerToeCommandHandler _commandHandler;
    private readonly CommandEnvelope<VoegVertegenwoordigerToeCommand> _envelope;

    public Given_A_VerenigingMetRechtspersoonlijkheid()
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
    public async ValueTask Then_A_UnsupportedOperationException_Is_Thrown()
    {
        var method = () => _commandHandler.Handle(_envelope);
        await method.Should().ThrowAsync<VerenigingMetRechtspersoonlijkheidKanGeenVertegenwoordigersToevoegen>();
    }
}
