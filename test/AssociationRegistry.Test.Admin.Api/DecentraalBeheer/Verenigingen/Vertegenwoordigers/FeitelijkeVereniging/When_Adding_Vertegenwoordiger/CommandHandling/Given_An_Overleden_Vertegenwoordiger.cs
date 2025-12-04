namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Vertegenwoordigers.FeitelijkeVereniging.When_Adding_Vertegenwoordiger.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Vertegenwoordigers.VoegVertegenwoordigerToe;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using AssociationRegistry.Framework;
using AssociationRegistry.Magda.Persoon;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using Common.StubsMocksFakes.VerenigingsRepositories;
using FluentAssertions;
using Resources;
using Xunit;

public class Given_An_Overleden_Vertegenwoordiger
{
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly FeitelijkeVerenigingWerdGeregistreerdScenario _scenario;
    private readonly Fixture _fixture;
    private readonly VoegVertegenwoordigerToeCommandHandler _commandHandler;
    private readonly VoegVertegenwoordigerToeCommand _command;

    public Given_An_Overleden_Vertegenwoordiger()
    {
        _scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario();
        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());

        _fixture = new Fixture().CustomizeAdminApi();

        _commandHandler = new VoegVertegenwoordigerToeCommandHandler(_verenigingRepositoryMock);

        _command = new VoegVertegenwoordigerToeCommand(
            _scenario.VCode,
            _fixture.Create<Vertegenwoordiger>());
    }

    [Fact]
    public async ValueTask Then_A_VertegenwoordigerWerdToegevoegd_Event_Is_Saved()
    {
        var exception = await Assert.ThrowsAsync<OverledenVertegenwoordigerKanNietToegevoegdWorden>(() => _commandHandler
               .Handle(new CommandEnvelope<VoegVertegenwoordigerToeCommand>(_command, _fixture.Create<CommandMetadata>()),
                       _fixture.Create<PersoonUitKsz>() with { Overleden = true }));

        exception.Message.Should().Be(ExceptionMessages.OverledenVertegenwoordigerKanNietToegevoegdWorden);
    }
}
