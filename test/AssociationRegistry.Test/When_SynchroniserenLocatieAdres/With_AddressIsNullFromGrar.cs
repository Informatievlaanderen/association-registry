namespace AssociationRegistry.Test.Admin.Api.Grar.When_SynchroniserenLocatieAdres;

using AssociationRegistry.Grar;
using AssociationRegistry.Grar.AddressSync;
using AssociationRegistry.Grar.Models;
using AutoFixture;
using Common.Framework;
using Common.Scenarios.CommandHandling;
using Events;
using Framework;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Test.Framework.Customizations;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_AddressIsNullFromGrar
{
    [Fact]
    public async Task Then_ShouldHaveNoSaves()
    {
        var scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario().GetVerenigingState();

        var verenigingRepositoryMock = new VerenigingRepositoryMock(scenario);

        var fixture = new Fixture().CustomizeDomain();

        var grarClientMock = new Mock<IGrarClient>();

        var locatie = scenario.Locaties.First();

        var message = fixture.Create<TeSynchroniserenLocatieAdresMessage>() with
        {
            LocatiesWithAdres = new List<LocatieWithAdres>() { new(locatie.LocatieId, null) },
            VCode = "V001",
            IdempotenceKey = "123456789",
        };

        var messageHandler = new TeSynchroniserenLocatieAdresMessageHandler(verenigingRepositoryMock, grarClientMock.Object,
                                                                            new NullLogger<TeSynchroniserenLocatieAdresMessageHandler>());

        await messageHandler.Handle(message, CancellationToken.None);

        verenigingRepositoryMock.ShouldHaveSaved(new AdresWerdOntkoppeldVanAdressenregister(
                                                     scenario.VCode.Value,
                                                     locatie.LocatieId,
                                                     Registratiedata.AdresId.With(locatie.AdresId),
                                                     Registratiedata.Adres.With(locatie.Adres)));
    }
}
