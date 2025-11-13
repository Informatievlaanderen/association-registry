namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Vertegenwoordigers.FeitelijkeVereniging.When_Removing_Vertegenwoordiger.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Vertegenwoordigers.VerwijderVertegenwoordiger;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Common.StubsMocksFakes.VertegenwoordigerPersoonsgegevensRepositories;
using AutoFixture;
using Xunit;

public abstract class VerwijderVertegenwoordigerCommandHandlerTestBase<TScenario> : IAsyncLifetime
    where TScenario : CommandhandlerScenarioBase, new()
{
    public VerwijderVertegenwoordigerCommandHandlerTestBase()
    {
        Fixture = new Fixture().CustomizeAdminApi();
        Scenario = new TScenario();

        VertegenwoordigerPersoonsgegevensRepositoryMock = Scenario.VertegenwoordigerPersoonsgegevensRepository;
        VerenigingRepositoryMock = new VerenigingRepositoryMock(VerenigingState);

        CommandHandler = new VerwijderVertegenwoordigerCommandHandler(VerenigingRepositoryMock, VertegenwoordigerPersoonsgegevensRepositoryMock);
        CommandMetadata = Fixture.Create<CommandMetadata>();
    }

    private VerwijderVertegenwoordigerCommand? _command;
    public VerwijderVertegenwoordigerCommand Command => _command ??= CreateCommand();
    protected abstract VerwijderVertegenwoordigerCommand CreateCommand();
    public CommandMetadata CommandMetadata { get; set; }
    public VerwijderVertegenwoordigerCommandHandler CommandHandler { get; set; }
    public CommandResult CommandResult { get; set; }
    public VertegenwoordigerPersoonsgegevensRepositoryMock VertegenwoordigerPersoonsgegevensRepositoryMock { get; set; }
    public VerenigingRepositoryMock VerenigingRepositoryMock { get; set; }
    public IFixture Fixture { get; set; }
    public TScenario Scenario { get; set; }
    public VerenigingState VerenigingState => Scenario.GetVerenigingState();

    public async ValueTask DisposeAsync()
    {
    }

    public async ValueTask InitializeAsync()
        => await ExecuteCommand();

    public virtual async Task ExecuteCommand()
    {
        CommandResult = await CommandHandler.Handle(new CommandEnvelope<VerwijderVertegenwoordigerCommand>(Command, CommandMetadata));
    }
}
