namespace AssociationRegistry.Test.GrarUpdates.TeOntkoppelenLocatiesHandlerTests;

using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using Common.Scenarios.CommandHandling;
using Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using Grar.GrarConsumer.Messaging.OntkoppelAdres;
using Grar.GrarUpdates.Fusies.TeOntkoppelenLocaties;
using Xunit;

public class Given_No_Existing_Locatie
{
    [Fact]
    public async Task Then_The_Locaties_Are_Ontkoppeld()
    {
        var fixture = new Fixture().CustomizeDomain();
        var scenario = new FeitelijkeVerenigingWerdGeregistreerdWithMinimalFields();

        var verenigingRepositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState(), expectedLoadingDubbel: true);

        var message = fixture.Create<OntkoppelLocatiesMessage>() with
        {
            VCode = scenario.VCode,
        };

        var sut = new OntkoppelLocatiesMessageHandler(verenigingRepositoryMock);

        await sut.Handle(message, CancellationToken.None);

        verenigingRepositoryMock.ShouldNotHaveAnySaves();
    }
}
