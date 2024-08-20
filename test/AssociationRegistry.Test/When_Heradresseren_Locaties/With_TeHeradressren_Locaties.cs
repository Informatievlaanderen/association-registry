namespace AssociationRegistry.Test.When_Heradresseren_Locaties;

using AssociationRegistry.Events;
using AssociationRegistry.Grar;
using AssociationRegistry.Grar.HeradresseerLocaties;
using AssociationRegistry.Grar.Models;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling;
using AutoFixture;
using Common.Framework;
using Moq;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_TeHeradressren_Locaties
{
    [Fact]
    public async Task Then_A_LocatieWerdToegevoegd_Event_Is_Saved()
    {
        var scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario().GetVerenigingState();

        var verenigingRepositoryMock = new VerenigingRepositoryMock(scenario);

        var fixture = new Fixture().CustomizeAdminApi();

        var mockedAdresDetail = fixture.Create<AddressDetailResponse>();

        var grarClientMock = new Mock<IGrarClient>();

        grarClientMock.Setup(x => x.GetAddressById("123", CancellationToken.None))
                      .ReturnsAsync(mockedAdresDetail);

        var locatieId = scenario.Locaties.First().LocatieId;

        var message = fixture.Create<TeHeradresserenLocatiesMessage>() with
        {
            LocatiesMetAdres = new List<LocatieIdWithAdresId>() { new(locatieId, "123") },
            VCode = "V001",
            idempotencyKey = "123456789",
        };

        var messageHandler = new TeHeradresserenLocatiesMessageHandler(verenigingRepositoryMock, grarClientMock.Object);

        await messageHandler.Handle(message, CancellationToken.None);

        verenigingRepositoryMock.ShouldHaveSaved(
            new AdresWerdGewijzigdInAdressenregister(scenario.VCode.Value, locatieId,
                                                     mockedAdresDetail.AdresId,
                                                     mockedAdresDetail.ToAdresUitAdressenregister(),
                                                     message.idempotencyKey)
        );
    }
}
