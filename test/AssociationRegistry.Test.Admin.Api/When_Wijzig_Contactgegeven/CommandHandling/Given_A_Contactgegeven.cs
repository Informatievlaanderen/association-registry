namespace AssociationRegistry.Test.Admin.Api.When_Wijzig_Contactgegeven.CommandHandling;

using AssociationRegistry.Framework;
using AutoFixture;
using ContactGegevens;
using ContactGegevens.Emails;
using Events;
using Fakes;
using Fixtures.Scenarios;
using Framework;
using Vereniging.WijzigContactgegeven;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Contactgegeven
{
    private readonly WijzigContactgegevenCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly VerenigingWerdGeregistreerd_WithAPrimairEmailContactgegeven_Commandhandler_Scenario _scenario;
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;

    public Given_A_Contactgegeven()
    {
        _scenario = new VerenigingWerdGeregistreerd_WithAPrimairEmailContactgegeven_Commandhandler_Scenario();
        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVereniging());

        _fixture = new Fixture().CustomizeAll();

        _commandHandler = new WijzigContactgegevenCommandHandler(_verenigingRepositoryMock);
    }

    [Fact]
    public async Task Then_A_ContactgegevenWerdToegevoegd_Event_Is_Saved_With_The_Next_Id()
    {
        var command = new WijzigContactgegevenCommand(
            _scenario.VCode,
            new WijzigContactgegevenCommand.CommandContactgegeven(
                _scenario.ContactgegevenId,
                _fixture.Create<Email>().Waarde,
                _fixture.Create<string?>(),
                IsPrimair: false));

        await _commandHandler.Handle(new CommandEnvelope<WijzigContactgegevenCommand>(command, _fixture.Create<CommandMetadata>()));

        _verenigingRepositoryMock.ShouldHaveSaved(
            new ContactgegevenWerdGewijzigd(
                ContactgegevenId: _scenario.ContactgegevenId,
                ContactgegevenType.Email,
                command.Contactgegeven.Waarde!,
                command.Contactgegeven.Omschrijving ?? string.Empty,
                IsPrimair: false)
        );
    }
}
