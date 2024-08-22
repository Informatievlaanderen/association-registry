namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_Adding_Vertegenwoordiger.CommandHandling;

using Acties.VoegVertegenwoordigerToe;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.Framework;
using Common.Scenarios.CommandHandling;
using Events;
using Framework;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_NietPrimair_Vertegenwoordiger
{
    [Fact]
    public async Task Then_A_VertegenwoordigerWerdToegevoegd_Event_Is_Saved()
    {
        var scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario();
        var verenigingRepositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState());

        var fixture = new Fixture().CustomizeAdminApi();

        var commandHandler = new VoegVertegenwoordigerToeCommandHandler(verenigingRepositoryMock);

        var command = new VoegVertegenwoordigerToeCommand(
            scenario.VCode,
            fixture.Create<Vertegenwoordiger>());

        await commandHandler.Handle(new CommandEnvelope<VoegVertegenwoordigerToeCommand>(command, fixture.Create<CommandMetadata>()));

        verenigingRepositoryMock.ShouldHaveSaved(
            new VertegenwoordigerWerdToegevoegd(
                scenario.FeitelijkeVerenigingWerdGeregistreerd.Vertegenwoordigers.Max(v => v.VertegenwoordigerId) + 1,
                command.Vertegenwoordiger.Insz,
                command.Vertegenwoordiger.IsPrimair,
                command.Vertegenwoordiger.Roepnaam ?? string.Empty,
                command.Vertegenwoordiger.Rol ?? string.Empty,
                command.Vertegenwoordiger.Voornaam,
                command.Vertegenwoordiger.Achternaam,
                command.Vertegenwoordiger.Email.Waarde,
                command.Vertegenwoordiger.Telefoon.Waarde,
                command.Vertegenwoordiger.Mobiel.Waarde,
                command.Vertegenwoordiger.SocialMedia.Waarde)
        );
    }
}
