namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Vertegenwoordigers.FeitelijkeVereniging.When_Adding_Vertegenwoordiger.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Vertegenwoordigers.VoegVertegenwoordigerToe;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using AssociationRegistry.Framework;
using AssociationRegistry.Magda.Persoon;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Common.StubsMocksFakes.VerenigingsRepositories;
using FluentAssertions;
using Xunit;

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
    public async ValueTask Then_A_DuplicateVertegenwoordiger_Is_Thrown()
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

        await _commandHandler.Handle(commandEnvelope, _fixture.Create<PersoonUitKsz>());
        var handleCall = async () => await _commandHandler.Handle(secondCommandEnvelope, _fixture.Create<PersoonUitKsz>());

        await handleCall.Should()
                        .ThrowAsync<MeerderePrimaireVertegenwoordigers>()
                        .WithMessage(new MeerderePrimaireVertegenwoordigers().Message);
    }
}
