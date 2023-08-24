namespace AssociationRegistry.Test.Admin.Api.VerenigingOfAnyKind.When_Adding_Locatie.CommandHandling;

using Acties.VoegLocatieToe;
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
public class Given_A_Locatie
{
    [Theory]
    [MemberData(nameof(Data))]
    public async Task Then_A_LocatieWerdToegevoegd_Event_Is_Saved(CommandhandlerScenarioBase scenario, int expectedLocatieId)
    {
        var verenigingRepositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState());

        var fixture = new Fixture().CustomizeAdminApi();

        var commandHandler = new VoegLocatieToeCommandHandler(verenigingRepositoryMock);
        var command = new VoegLocatieToeCommand(scenario.VCode, fixture.Create<Locatie>());

        await commandHandler.Handle(new CommandEnvelope<VoegLocatieToeCommand>(command, fixture.Create<CommandMetadata>()));

        verenigingRepositoryMock.ShouldHaveSaved(
            new LocatieWerdToegevoegd(
                Registratiedata.Locatie.With(command.Locatie) with
                {
                    LocatieId = expectedLocatieId,
                })
        );
    }

    public static IEnumerable<object[]> Data
    {
        get
        {
            var feitelijkeVerenigingWerdGeregistreerdScenario = new FeitelijkeVerenigingWerdGeregistreerdScenario();
            yield return new object[]
            {
                feitelijkeVerenigingWerdGeregistreerdScenario,
                feitelijkeVerenigingWerdGeregistreerdScenario.FeitelijkeVerenigingWerdGeregistreerd.Locaties.Max(l => l.LocatieId) + 1,
            };
            var verenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario();
            yield return new object[]
            {
                verenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario,
                1,
            };
        }
    }
}
