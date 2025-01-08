namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingOfAnyKind.When_Adding_Locatie.CommandHandling;

using Acties.Locaties.VoegLocatieToe;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using Common.Scenarios.CommandHandling;
using EventFactories;
using Events;
using FluentAssertions;
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
    private Fixture _fixture;

    public Given_A_Locatie()
    {
        _fixture = new Fixture().CustomizeAdminApi();
    }

    [Theory]
    [MemberData(nameof(Data))]
    public async Task Then_A_LocatieWerdToegevoegd_Event_Is_Saved(CommandhandlerScenarioBase scenario, int expectedLocatieId)
    {
        var verenigingRepositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState());

        var commandHandler = new VoegLocatieToeCommandHandler(verenigingRepositoryMock,
                                                              Mock.Of<IMartenOutbox>(),
                                                              Mock.Of<IDocumentSession>(),
                                                              Mock.Of<IGrarClient>()
        );

        var command = new VoegLocatieToeCommand(scenario.VCode, _fixture.Create<Locatie>() with
        {
            AdresId = null,
        });

        await commandHandler.Handle(new CommandEnvelope<VoegLocatieToeCommand>(command, _fixture.Create<CommandMetadata>()));

        verenigingRepositoryMock.ShouldHaveSaved(
            new LocatieWerdToegevoegd(
                EventFactory.Locatie(command.Locatie) with
                {
                    LocatieId = expectedLocatieId,
                })
        );
    }

    [Theory]
    [MemberData(nameof(Data))]
    public async Task Then_An_EntityId_Is_Returned(CommandhandlerScenarioBase scenario, int expectedLocatieId)
    {
        var verenigingRepositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState());

        var commandHandler = new VoegLocatieToeCommandHandler(verenigingRepositoryMock,
                                                              Mock.Of<IMartenOutbox>(),
                                                              Mock.Of<IDocumentSession>(),
                                                              Mock.Of<IGrarClient>()
        );

        var command = new VoegLocatieToeCommand(scenario.VCode, _fixture.Create<Locatie>() with
        {
            AdresId = null,
        });

        var result = await commandHandler.Handle(new CommandEnvelope<VoegLocatieToeCommand>(command, _fixture.Create<CommandMetadata>()));

        result.EntityId.Should().Be(expectedLocatieId);
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
