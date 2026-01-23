namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Vertegenwoordigers.VerenigingMetRechtspersoonlijkheid.When_Adding_Vertegenwoordiger.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Vertegenwoordigers.VoegVertegenwoordigerToe;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using AssociationRegistry.Framework;
using AssociationRegistry.Magda.Persoon;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using AutoFixture;
using Common.StubsMocksFakes.VerenigingsRepositories;
using FluentAssertions;
using Xunit;

public class Given_A_VerenigingMetRechtspersoonlijkheid
{
    private readonly VoegVertegenwoordigerToeCommandHandler _commandHandler;
    private readonly CommandEnvelope<VoegVertegenwoordigerToeCommand> _envelope;
    private readonly Fixture _fixture;

    public Given_A_VerenigingMetRechtspersoonlijkheid()
    {
        var scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario();
        var verenigingRepositoryMock = new AggregateSessionMock(scenario.GetVerenigingState());

        _fixture = new Fixture().CustomizeAdminApi();
        var command = _fixture.Create<VoegVertegenwoordigerToeCommand>() with { VCode = scenario.VCode };
        var commandMetadata = _fixture.Create<CommandMetadata>();

        _commandHandler = new VoegVertegenwoordigerToeCommandHandler(verenigingRepositoryMock);
        _envelope = new CommandEnvelope<VoegVertegenwoordigerToeCommand>(command, commandMetadata);
    }

    [Fact]
    public async ValueTask Then_A_UnsupportedOperationException_Is_Thrown()
    {
        var method = () => _commandHandler.Handle(_envelope, _fixture.Create<PersoonUitKsz>());
        await method.Should().ThrowAsync<VerenigingMetRechtspersoonlijkheidKanGeenVertegenwoordigersToevoegen>();
    }
}
