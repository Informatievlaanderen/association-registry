namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Vertegenwoordigers.FeitelijkeVereniging.When_Adding_Vertegenwoordiger.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Vertegenwoordigers.VoegVertegenwoordigerToe;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using AssociationRegistry.Framework;
using AssociationRegistry.Magda.Persoon;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using AutoFixture;
using Common.StubsMocksFakes.VerenigingsRepositories;
using FluentAssertions;
using Xunit;

public class Given_A_Duplicate_Vertegenwoordiger
{
    private readonly VoegVertegenwoordigerToeCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly FeitelijkeVerenigingWerdGeregistreerdScenario _scenario;

    public Given_A_Duplicate_Vertegenwoordiger()
    {
        _scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario();
        var verenigingRepositoryMock = new AggregateSessionMock(_scenario.GetVerenigingState());

        _fixture = new Fixture().CustomizeAdminApi();

        _commandHandler = new VoegVertegenwoordigerToeCommandHandler(verenigingRepositoryMock);
    }

    [Fact]
    public async ValueTask Then_A_DuplicateVertegenwoordiger_Is_Thrown()
    {
        var command = _fixture.Create<VoegVertegenwoordigerToeCommand>() with { VCode = _scenario.VCode };

        var commandEnvelope = new CommandEnvelope<VoegVertegenwoordigerToeCommand>(
            command,
            _fixture.Create<CommandMetadata>()
        );

        await _commandHandler.Handle(commandEnvelope, _fixture.Create<PersoonUitKsz>());
        var handleCall = async () => await _commandHandler.Handle(commandEnvelope, _fixture.Create<PersoonUitKsz>());

        await handleCall.Should().ThrowAsync<InszMoetUniekZijn>().WithMessage(new InszMoetUniekZijn().Message);
    }
}
