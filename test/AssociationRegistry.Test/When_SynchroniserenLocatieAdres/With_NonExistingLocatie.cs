﻿namespace AssociationRegistry.Test.When_SynchroniserenLocatieAdres;

using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using Common.Scenarios.CommandHandling;
using Framework.Customizations;
using Grar;
using Grar.AddressSync;
using Grar.Models;
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

        var message = fixture.Create<TeSynchroniserenLocatieAdresMessage>() with
        {
            LocatiesWithAdres = new List<LocatieWithAdres>
                { new(nonExistingLocatieId, mockedAdresDetail) },
            VCode = "V001",
            IdempotenceKey = "123456789",
        };

        var messageHandler = new TeSynchroniserenLocatieAdresMessageHandler(verenigingRepositoryMock, grarClientMock.Object,
                                                                            new NullLogger<TeSynchroniserenLocatieAdresMessageHandler>());

        await messageHandler.Handle(message, CancellationToken.None);

        verenigingRepositoryMock.ShouldNotHaveAnySaves();
    }
}
