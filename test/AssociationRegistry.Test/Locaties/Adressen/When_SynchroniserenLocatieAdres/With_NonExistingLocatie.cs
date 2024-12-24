namespace AssociationRegistry.Test.Locaties.Adressen.When_SynchroniserenLocatieAdres;

using Acties.SyncAdresLocaties;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using Common.Scenarios.CommandHandling;
using Grar;
using Grar.Models;
using Messages;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_NonExistingLocatie
{
    [Fact]
    public async Task Then_An_AdresWerdOntkoppeldVanAdressenregister_Was_Saved()
    {
        var scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario().GetVerenigingState();

        var verenigingRepositoryMock = new VerenigingRepositoryMock(scenario);

        var fixture = new Fixture().CustomizeDomain();

        var mockedAdresDetail = fixture.Create<AddressDetailResponse>() with
        {
            IsActief = false,
        };

        var grarClientMock = new Mock<IGrarClient>();

        grarClientMock.Setup(x => x.GetAddressById("123", CancellationToken.None))
                      .ReturnsAsync(mockedAdresDetail);

        var locatieId = scenario.Locaties.First().LocatieId;

        var nonExistingLocatieId = locatieId * -1;

        var command = fixture.Create<SyncAdresLocatiesCommand>() with
        {
            LocatiesWithAdres = new List<LocatieWithAdres>
                { new(nonExistingLocatieId, mockedAdresDetail) },
            VCode = "V001",
            IdempotenceKey = "123456789",
        };

        var commandHandler = new SyncAdresLocatiesCommandHandler(verenigingRepositoryMock, grarClientMock.Object,
                                                                 new NullLogger<SyncAdresLocatiesCommandHandler>());

        await commandHandler.Handle(command, CancellationToken.None);

        verenigingRepositoryMock.ShouldNotHaveAnySaves();
    }
}
