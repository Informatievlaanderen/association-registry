namespace AssociationRegistry.Test.Locaties.When_Heradresseren_Locaties;

using AssociationRegistry.Grar.Clients;
using AssociationRegistry.Grar.GrarConsumer.Messaging.HeradresseerLocaties;
using AssociationRegistry.Grar.GrarUpdates.Hernummering;
using AssociationRegistry.Grar.Models;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AutoFixture;
using Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using Moq;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_NonExistingLocatie
{
    [Fact]
    public async ValueTask Then_A_LocatieWerdToegevoegd_Event_Is_Saved()
    {
        var scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario().GetVerenigingState();

        var verenigingRepositoryMock = new VerenigingRepositoryMock(scenario, expectedLoadingDubbel: true);

        var fixture = new Fixture().CustomizeAdminApi();

        var mockedAdresDetail = fixture.Create<AddressDetailResponse>();

        var grarClientMock = new Mock<IGrarClient>();

        grarClientMock.Setup(x => x.GetAddressById("123", CancellationToken.None))
                      .ReturnsAsync(mockedAdresDetail);

        var locatieId = scenario.Locaties.First().LocatieId;
        var nonExistingLocatieId = locatieId * -1;

        var message = fixture.Create<HeradresseerLocatiesMessage>() with
        {
            TeHeradresserenLocaties = new List<TeHeradresserenLocatie>
                { new(nonExistingLocatieId, NaarAdresId: "123") },
            VCode = "V001",
            idempotencyKey = "123456789",
        };

        var messageHandler = new HeradresseerLocatiesMessageHandler(verenigingRepositoryMock, grarClientMock.Object);

        await messageHandler.Handle(message, CancellationToken.None);

        verenigingRepositoryMock.ShouldNotHaveAnySaves();
    }
}
