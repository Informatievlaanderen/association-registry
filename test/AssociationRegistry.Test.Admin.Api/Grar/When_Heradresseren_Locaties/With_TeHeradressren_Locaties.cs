namespace AssociationRegistry.Test.Admin.Api.Grar.When_Heradresseren_Locaties;

using AssociationRegistry.Grar;
using AssociationRegistry.Grar.HeradresseerLocaties;
using AssociationRegistry.Grar.Models;
using AutoFixture;
using Events;
using Fakes;
using Fixtures.Scenarios.CommandHandling;
using Framework;
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

        grarClientMock.Setup(x => x.GetAddress("123"))
                      .ReturnsAsync(mockedAdresDetail);

        var message = fixture.Create<TeHeradresserenLocatiesMessage>() with
        {
            LocatiesMetAdres = new List<LocatieIdWithAdresId>() { new(1, "123") },
            VCode = "V001",
            idempotencyKey = "123456789"
        };

        var messageHandler = new HeradresseerLocatiesMessageHandler(verenigingRepositoryMock, grarClientMock.Object);

        await messageHandler.Handle(message);

        verenigingRepositoryMock.ShouldHaveSaved(
            new AdresWerdGeheradresseerdInAdressenregister(scenario.VCode.Value, 1, AdresDetailUitAdressenregister.FromResponse(mockedAdresDetail), message.idempotencyKey)
        );
    }
}

public class With_Multiple_TeHeradressren_Locaties
{
    [Fact]
    public async Task Then_A_LocatieWerdToegevoegd_Event_Is_Saved()
    {
        var scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario().GetVerenigingState();

        var verenigingRepositoryMock = new VerenigingRepositoryMock(scenario);

        var fixture = new Fixture().CustomizeAdminApi();

        var mockedAdresDetail1 = fixture.Create<AddressDetailResponse>();
        var mockedAdresDetail2 = fixture.Create<AddressDetailResponse>();

        var grarClientMock = new Mock<IGrarClient>();

        grarClientMock.Setup(x => x.GetAddress("123"))
                      .ReturnsAsync(mockedAdresDetail1);

        grarClientMock.Setup(x => x.GetAddress("456"))
                      .ReturnsAsync(mockedAdresDetail2);

        var message = fixture.Create<TeHeradresserenLocatiesMessage>() with
        {
            LocatiesMetAdres = new List<LocatieIdWithAdresId>() { new (1, "123"), new (2, "456") },
            VCode = "V001",
            idempotencyKey = "123456789"
        };

        var messageHandler = new HeradresseerLocatiesMessageHandler(verenigingRepositoryMock, grarClientMock.Object);

        await messageHandler.Handle(message);

        verenigingRepositoryMock.ShouldHaveSaved(
            new AdresWerdGeheradresseerdInAdressenregister(scenario.VCode.Value, 1, AdresDetailUitAdressenregister.FromResponse(mockedAdresDetail1), message.idempotencyKey),
            new AdresWerdGeheradresseerdInAdressenregister(scenario.VCode.Value, 2, AdresDetailUitAdressenregister.FromResponse(mockedAdresDetail2), message.idempotencyKey)
        );
    }
}
