namespace AssociationRegistry.Test.Admin.Api.When_Adding_Vertegenwoordiger.CommandHandling;

using Acties.VoegVertegenwoordigerToe;
using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using Fakes;
using Fixtures.Scenarios;
using Framework;
using Framework.MagdaMocks;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_NietPrimair_Vertegenwoordiger
{
    [Fact]
    public async Task Then_A_VertegenwoordigerWerdToegevoegd_Event_Is_Saved()
    {
        var scenario = new VerenigingWerdGeregistreerdScenario();
        var verenigingRepositoryMock = new VerenigingRepositoryMock(scenario.GetVereniging());

        var fixture = new Fixture().CustomizeAll();

        var commandHandler = new VoegVertegenwoordigerToeCommandHandler(verenigingRepositoryMock, new MagdaFacadeEchoMock());
        var command = new VoegVertegenwoordigerToeCommand(
            scenario.VCode,
            fixture.Create<Vertegenwoordiger>());

        await commandHandler.Handle(new CommandEnvelope<VoegVertegenwoordigerToeCommand>(command, fixture.Create<CommandMetadata>()));

        verenigingRepositoryMock.ShouldHaveSaved(
            new VertegenwoordigerWerdToegevoegd(
                scenario.VerenigingWerdGeregistreerd.Vertegenwoordigers.Max(v => v.VertegenwoordigerId) + 1,
                command.Vertegenwoordiger.Insz,
                command.Vertegenwoordiger.IsPrimair,
                command.Vertegenwoordiger.Roepnaam ?? string.Empty,
                command.Vertegenwoordiger.Rol ?? string.Empty,
                command.Vertegenwoordiger.Insz,
                command.Vertegenwoordiger.Insz,
                command.Vertegenwoordiger.Email.Waarde,
                command.Vertegenwoordiger.Telefoon.Waarde,
                command.Vertegenwoordiger.Mobiel.Waarde,
                command.Vertegenwoordiger.SocialMedia.Waarde)
        );
    }
}
