namespace AssociationRegistry.Test.Locaties.Adressen.When_SynchroniserenLocatieAdres;

using Acties.SyncAdresLocaties;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using Common.Scenarios.CommandHandling;
using Events;
using Grar;
using Grar.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_InactiveAddressFromGrar
{
    [Fact]
    public async Task Then_An_AdresWerdOntkoppeldVanAdressenregister_Was_Saved()
    {
        var scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario().GetVerenigingState();

        var verenigingRepositoryMock = new VerenigingRepositoryMock(scenario, expectedLoadingDubbel: true);

        var fixture = new Fixture().CustomizeDomain();

        var mockedAdresDetail = fixture.Create<AddressDetailResponse>() with
        {
            IsActief = false,
        };

        var grarClientMock = new Mock<IGrarClient>();

        grarClientMock.Setup(x => x.GetAddressById("123", CancellationToken.None))
                      .ReturnsAsync(mockedAdresDetail);

        var locatie = scenario.Locaties.First();

        var command = fixture.Create<SyncAdresLocatiesCommand>() with
        {
            LocatiesWithAdres = new List<LocatieWithAdres>
                { new(locatie.LocatieId, mockedAdresDetail) },
            VCode = "V001",
            IdempotenceKey = "123456789",
        };

        var commandHandler = new SyncAdresLocatiesCommandHandler(verenigingRepositoryMock, grarClientMock.Object,
                                                                 new NullLogger<SyncAdresLocatiesCommandHandler>());

        await commandHandler.Handle(command, CancellationToken.None);

        verenigingRepositoryMock.ShouldHaveSaved(
            new AdresWerdOntkoppeldVanAdressenregister(scenario.VCode.Value,
                                                       locatie.LocatieId,
                                                       Registratiedata.AdresId.With(locatie.AdresId),
                                                       Registratiedata.Adres.With(locatie.Adres)));
    }
}
