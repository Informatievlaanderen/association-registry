namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingOfAnyKind.When_Adding_Contactgegeven.CommandHandling;

using Acties.VoegContactgegevenToe;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using Common.Scenarios.CommandHandling;
using Events;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_NietPrimair_Contactgegeven
{
    [Theory]
    [InlineData("E-mail", "email@example.org")]
    [InlineData("Website", "https://www.example.org")]
    [InlineData("SocialMedia", "https://www.example.org")]
    [InlineData("Telefoon", "0000112233")]
    public async Task Then_A_ContactgegevenWerdToegevoegd_Event_Is_Saved(string type, string waarde)
    {
        var scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario();
        var verenigingRepositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState());

        var fixture = new Fixture().CustomizeAdminApi();

        var commandHandler = new VoegContactgegevenToeCommandHandler(verenigingRepositoryMock);

        var command = new VoegContactgegevenToeCommand(
            scenario.VCode,
            Contactgegeven.Create(
                type,
                waarde,
                fixture.Create<string>(),
                isPrimair: false));

        await commandHandler.Handle(new CommandEnvelope<VoegContactgegevenToeCommand>(command, fixture.Create<CommandMetadata>()));

        verenigingRepositoryMock.ShouldHaveSaved(
            new ContactgegevenWerdToegevoegd(
                scenario.FeitelijkeVerenigingWerdGeregistreerd.Contactgegevens.Max(c => c.ContactgegevenId) + 1,
                command.Contactgegeven.Contactgegeventype,
                command.Contactgegeven.Waarde,
                command.Contactgegeven.Beschrijving,
                IsPrimair: false)
        );
    }
}
