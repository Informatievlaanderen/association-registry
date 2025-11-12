namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Vertegenwoordigers.VerenigingOfAnyKind.When_Wijzig_Vertegenwoordiger.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Vertegenwoordigers.WijzigVertegenwoordiger;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Common.StubsMocksFakes.VertegenwoordigerPersoonsgegevensRepositories;
using Xunit;

public abstract class WijzigVertegenwoordigerCommandHandlerTestBase<TScenario> : IAsyncLifetime
    where TScenario : CommandhandlerScenarioBase, new()
{
    public WijzigVertegenwoordigerCommandHandlerTestBase()
    {
        Fixture = new Fixture().CustomizeAdminApi();
        Scenario = new TScenario();

        VertegenwoordigerPersoonsgegevensRepositoryMock = Scenario.VertegenwoordigerPersoonsgegevensRepository;
        VerenigingRepositoryMock = new VerenigingRepositoryMock(VerenigingState);

        CommandHandler = new WijzigVertegenwoordigerCommandHandler(VerenigingRepositoryMock, VertegenwoordigerPersoonsgegevensRepositoryMock);
        CommandMetadata = Fixture.Create<CommandMetadata>();
    }

    private WijzigVertegenwoordigerCommand? _command;
    public WijzigVertegenwoordigerCommand Command => _command ??= CreateCommand();
    protected abstract WijzigVertegenwoordigerCommand CreateCommand();
    public CommandMetadata CommandMetadata { get; set; }
    public WijzigVertegenwoordigerCommandHandler CommandHandler { get; set; }
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
        CommandResult = await CommandHandler.Handle(new CommandEnvelope<WijzigVertegenwoordigerCommand>(Command, CommandMetadata));
    }
}
