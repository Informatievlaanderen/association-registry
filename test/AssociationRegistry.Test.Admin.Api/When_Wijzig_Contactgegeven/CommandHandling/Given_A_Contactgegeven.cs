namespace AssociationRegistry.Test.Admin.Api.When_Wijzig_Contactgegeven.CommandHandling;

using Acties.WijzigContactgegeven;
using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using Fakes;
using Fixtures.Scenarios;
using Fixtures.Scenarios.CommandHandling;
using Framework;
using Vereniging;
using Vereniging.Emails;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Contactgegeven
{
    private readonly WijzigContactgegevenCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly FeitelijkeVerenigingWerdGeregistreerdWithAPrimairEmailContactgegevenScenario _scenario;
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;

    public Given_A_Contactgegeven()
    {
        _scenario = new FeitelijkeVerenigingWerdGeregistreerdWithAPrimairEmailContactgegevenScenario();
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
                FeitelijkeVerenigingWerdGeregistreerdWithAPrimairEmailContactgegevenScenario.ContactgegevenId,
                _fixture.Create<Email>().Waarde,
                _fixture.Create<string?>(),
                IsPrimair: false));

        await _commandHandler.Handle(new CommandEnvelope<WijzigContactgegevenCommand>(command, _fixture.Create<CommandMetadata>()));

        _verenigingRepositoryMock.ShouldHaveSaved(
            new ContactgegevenWerdGewijzigd(
                ContactgegevenId: FeitelijkeVerenigingWerdGeregistreerdWithAPrimairEmailContactgegevenScenario.ContactgegevenId,
                ContactgegevenType.Email,
                command.Contactgegeven.Waarde!,
                command.Contactgegeven.Beschrijving ?? string.Empty,
                IsPrimair: false)
        );
    }
}
