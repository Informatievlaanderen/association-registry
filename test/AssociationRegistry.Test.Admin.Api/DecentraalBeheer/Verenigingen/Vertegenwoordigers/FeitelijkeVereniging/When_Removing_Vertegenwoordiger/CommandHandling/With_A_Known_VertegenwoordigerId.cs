namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Vertegenwoordigers.FeitelijkeVereniging.When_Removing_Vertegenwoordiger.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Vertegenwoordigers.VerwijderVertegenwoordiger;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Events;
using Xunit;

public class With_A_Known_VertegenwoordigerId
{
    private readonly AggregateSessionMock _aggregateSessionMock;
    private readonly FeitelijkeVerenigingWerdGeregistreerdWithAPrimairVertegenwoordigerScenario _scenario;
    private VerwijderVertegenwoordigerCommand _command;
    private CommandMetadata? _commandMetadata;

    public With_A_Known_VertegenwoordigerId()
    {
        _scenario = new FeitelijkeVerenigingWerdGeregistreerdWithAPrimairVertegenwoordigerScenario();

        _aggregateSessionMock = new AggregateSessionMock(_scenario.GetVerenigingState());

        var fixture = new Fixture().CustomizeAdminApi();
        _command = new VerwijderVertegenwoordigerCommand(
            _scenario.VCode,
            _scenario.VertegenwoordigerWerdToegevoegd.VertegenwoordigerId
        );
        _commandMetadata = fixture.Create<CommandMetadata>();
        var commandHandler = new VerwijderVertegenwoordigerCommandHandler(_aggregateSessionMock);

        commandHandler
            .Handle(new CommandEnvelope<VerwijderVertegenwoordigerCommand>(_command, _commandMetadata))
            .GetAwaiter()
            .GetResult();
    }

    [Fact]
    public void Then_The_Correct_Vereniging_Is_Loaded_Once()
    {
        _aggregateSessionMock.ShouldHaveLoaded<Vereniging>(_scenario.VCode);
    }

    [Fact]
    public void Then_A_VertegenwoordigerWerdVerwijderd_Event_Is_Saved()
    {
        _aggregateSessionMock.ShouldHaveSavedExact(
            new VertegenwoordigerWerdVerwijderd(
                _scenario.VertegenwoordigerWerdToegevoegd.VertegenwoordigerId,
                _scenario.VertegenwoordigerWerdToegevoegd.Insz,
                _scenario.VertegenwoordigerWerdToegevoegd.Voornaam,
                _scenario.VertegenwoordigerWerdToegevoegd.Achternaam
            )
        );
    }
}
