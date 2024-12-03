namespace AssociationRegistry.Test.GrarUpdates.TeOntkoppelenLocatiesHandlerTests;

using Acties.OntkoppelAdres;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using Common.Scenarios.CommandHandling;
using Grar.GrarUpdates.Fusies.TeOntkoppelenLocaties;
using Xunit;

public class Given_No_Existing_Locatie
{
    [Fact]
    public async Task Then_The_Locaties_Are_Ontkoppeld()
    {
        var fixture = new Fixture().CustomizeDomain();
        var scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario();

        var verenigingRepositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState());

        var message = fixture.Create<OntkoppelLocatiesMessage>() with
        {
            VCode = scenario.VCode,
        };

        var sut = new OntkoppelLocatiesHandler(verenigingRepositoryMock);

        await sut.Handle(message, CancellationToken.None);

        verenigingRepositoryMock.ShouldNotHaveAnySaves();
    }
}
