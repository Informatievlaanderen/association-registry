namespace AssociationRegistry.Test.Admin.Api.VerenigingOfAnyKind.When_Adding_Contactgegeven.CommandHandling;

using Acties.VoegContactgegevenToe;
using Events;
using AssociationRegistry.Framework;
using Fakes;
using AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios.CommandHandling;
using Framework;
using Vereniging;
using AutoFixture;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Second_NietPrimair_Contactgegeven
{
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly VoegContactgegevenToeCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly FeitelijkeVerenigingWerdGeregistreerdWithAPrimairEmailContactgegevenScenario _scenario;

    public Given_A_Second_NietPrimair_Contactgegeven()
    {
        _scenario = new FeitelijkeVerenigingWerdGeregistreerdWithAPrimairEmailContactgegevenScenario();
        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());

        _fixture = new Fixture().CustomizeAdminApi();

        _commandHandler = new VoegContactgegevenToeCommandHandler(_verenigingRepositoryMock);
    }

    [Theory]
    [InlineData("E-mail", "email2@example.org")]
    [InlineData("Website", "https://www.example.org")]
    [InlineData("SocialMedia", "https://www.example.org")]
    [InlineData("Telefoon", "0000112233")]
    public async Task Then_A_ContactgegevenWerdToegevoegd_Event_Is_Saved(string type, string waarde)
    {
        var command = new VoegContactgegevenToeCommand(
            _scenario.VCode,
            Contactgegeven.CreateFromInitiator(
                ContactgegevenType.Parse(type),
                waarde,
                _fixture.Create<string?>(),
                false));

        await _commandHandler.Handle(new CommandEnvelope<VoegContactgegevenToeCommand>(command, _fixture.Create<CommandMetadata>()));

        _verenigingRepositoryMock.ShouldHaveSaved(
            new ContactgegevenWerdToegevoegd(2, command.Contactgegeven.Type, command.Contactgegeven.Waarde, command.Contactgegeven.Beschrijving, false)
        );
    }
}
