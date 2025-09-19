namespace AssociationRegistry.Test.Grar.GrarUpdates.TeOntkoppelenLocatiesHandlerTests;

using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using AutoFixture;
using AssociationRegistry.CommandHandling.Grar.GrarConsumer.Messaging.OntkoppelAdres;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Xunit;

public class Given_No_Existing_Locatie
{
    [Fact]
    public async ValueTask Then_The_Locaties_Are_Ontkoppeld()
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
