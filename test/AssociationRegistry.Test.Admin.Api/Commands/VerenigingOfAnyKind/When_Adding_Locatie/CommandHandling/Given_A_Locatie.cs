namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingOfAnyKind.When_Adding_Locatie.CommandHandling;

using Acties.VoegLocatieToe;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.Framework;
using Common.Scenarios.CommandHandling;
using Events;
using Framework;
using Grar;
using Marten;
using Moq;
using Vereniging;
using Wolverine.Marten;
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

        var commandHandler = new VoegLocatieToeCommandHandler(verenigingRepositoryMock,
                                                              Mock.Of<IMartenOutbox>(),
                                                              Mock.Of<IDocumentSession>(),
                                                              Mock.Of<IGrarClient>()
        );

        var command = new VoegLocatieToeCommand(scenario.VCode, fixture.Create<Locatie>() with
        {
            AdresId = null,
        });

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

            var verenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario =
                new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario();

            yield return new object[]
            {
                verenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario,
                1,
            };
        }
    }
}
