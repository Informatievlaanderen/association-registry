namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.InStopzetting.When_Update_Vereniging_InStopzetting.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.InStopzetting.UpdateInStopzetting;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling;
using AssociationRegistry.Test.Common.StubsMocksFakes.VerenigingsRepositories;
using AutoFixture;
using Builders;
using Erkenningen;

public class UpdateVerenigingInStopzettingContext<TScenario>
    where TScenario : CommandhandlerScenarioBase
{
    public Fixture Fixture { get; }
    private readonly UpdateInStopzettingCommandHandler _commandHandler;

    public TScenario Scenario { get; }
    public AggregateSessionMock AggregateSessionMock { get; }
    public CommandMetadata Metadata { get; private set; }
    public UpdateInStopzettingCommand Command { get; private set; } = null!;

    public UpdateVerenigingInStopzettingContext(TScenario scenario)
    {
        Fixture = new Fixture().CustomizeAdminApi();
        Scenario = scenario;
        AggregateSessionMock = new AggregateSessionMock(Scenario.GetVerenigingState(), expectedLoadingDubbel: true);
        _commandHandler = new UpdateInStopzettingCommandHandler(AggregateSessionMock);
        Metadata = Fixture.Create<CommandMetadata>();
    }

    public static UpdateVerenigingInStopzettingContext<TScenario> Given(TScenario scenario) => new(scenario);

    public UpdateVerenigingInStopzettingContext<TScenario> WithCommand(
        Func<UpdateInStopzettingCommand, UpdateInStopzettingCommand> configure
    )
    {
        Command = CreateCommand();
        Command = configure(Command);

        return this;
    }

    public UpdateVerenigingInStopzettingContext<TScenario> WithInitiator(string? ovoCode = null)
    {
        Metadata = Metadata with { Initiator = ovoCode ?? Fixture.Create<string>() };

        return this;
    }

    public async ValueTask<UpdateVerenigingInStopzettingContext<TScenario>> WhenHandled()
    {
        await _commandHandler.Handle(new CommandEnvelope<UpdateInStopzettingCommand>(Command, Metadata));

        return this;
    }

    public void ShouldHaveSaved(params IEvent[] events) => AggregateSessionMock.ShouldHaveSavedExact(events);

    public CommandMetadata CreateMetadata(string? initiator = null) =>
        Fixture.Create<CommandMetadata>() with
        {
            Initiator = initiator ?? Fixture.Create<string>(),
        };

    public async ValueTask Handle(UpdateInStopzettingCommand command, CommandMetadata? metadata = null) =>
        await _commandHandler.Handle(new CommandEnvelope<UpdateInStopzettingCommand>(command, metadata ?? Metadata));

    private UpdateInStopzettingCommand CreateCommand() =>
        Fixture.Create<UpdateInStopzettingCommand>() with
        {
            VCode = Scenario.VCode,
        };
}
