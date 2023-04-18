namespace AssociationRegistry.Test.Admin.Api.When_Wijzig_Contactgegeven.CommandHandling;

using Acties.WijzigContactgegeven;
using AssociationRegistry.Framework;
using AutoFixture;
using Fakes;
using Fixtures.Scenarios;
using FluentAssertions;
using Framework;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_No_Modifications_To_The_Contactgegeven: IAsyncLifetime
{
    private readonly WijzigContactgegevenCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly VerenigingWerdGeregistreerd_WithAPrimairEmailContactgegeven_Commandhandler_Scenario _scenario;
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private CommandResult _commandResult = null!;

    public Given_No_Modifications_To_The_Contactgegeven()
    {
        _scenario = new VerenigingWerdGeregistreerd_WithAPrimairEmailContactgegeven_Commandhandler_Scenario();
        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVereniging());

        _fixture = new Fixture().CustomizeAll();

        _commandHandler = new WijzigContactgegevenCommandHandler(_verenigingRepositoryMock);
    }

    public async Task InitializeAsync()
    {
        var command = new WijzigContactgegevenCommand(
            _scenario.VCode,
            new WijzigContactgegevenCommand.CommandContactgegeven(
                VerenigingWerdGeregistreerd_WithAPrimairEmailContactgegeven_Commandhandler_Scenario.ContactgegevenId,
                VerenigingWerdGeregistreerd_WithAPrimairEmailContactgegeven_Commandhandler_Scenario.Waarde,
                VerenigingWerdGeregistreerd_WithAPrimairEmailContactgegeven_Commandhandler_Scenario.Beschrijving,
                VerenigingWerdGeregistreerd_WithAPrimairEmailContactgegeven_Commandhandler_Scenario.IsPrimair));

        _commandResult = await _commandHandler.Handle(new CommandEnvelope<WijzigContactgegevenCommand>(command, _fixture.Create<CommandMetadata>()));
    }

    [Fact]
    public void Then_No_ContactgegevenWerdToegevoegd_Event_Is_Saved()
    {
        _verenigingRepositoryMock.ShouldNotHaveAnySaves();
    }

    [Fact]
    public void Then_CommandResult_Has_No_Changes()
    {
        _commandResult.HasChanges().Should().BeFalse();
    }

    public Task DisposeAsync()
        => Task.CompletedTask;
}
