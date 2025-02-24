﻿namespace AssociationRegistry.Test.Locaties.Adressen.When_SynchroniserenLocatieAdres;

using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using Common.Scenarios.CommandHandling;
using Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using EventFactories;
using Events;
using Grar;
using Grar.Clients;
using Grar.Models;
using Grar.NightlyAdresSync.SyncAdresLocaties;
using Messages;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_AddressIsNullFromGrar
{
    [Fact]
    public async Task Then_ShouldHaveSaved()
    {
        var scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario().GetVerenigingState();

        var verenigingRepositoryMock = new VerenigingRepositoryMock(scenario, expectedLoadingDubbel: true);

        var fixture = new Fixture().CustomizeDomain();

        var grarClientMock = new Mock<IGrarClient>();

        var locatie = scenario.Locaties.First();

        var command = fixture.Create<SyncAdresLocatiesCommand>() with
        {
            LocatiesWithAdres = new List<LocatieWithAdres>
                { new(locatie.LocatieId, Adres: null) },
            VCode = "V001",
            IdempotenceKey = "123456789",
        };

        var commandHandler = new SyncAdresLocatiesCommandHandler(verenigingRepositoryMock, grarClientMock.Object,
                                                                 new NullLogger<SyncAdresLocatiesCommandHandler>());

        await commandHandler.Handle(command, CancellationToken.None);

        verenigingRepositoryMock.ShouldHaveSaved(new AdresWerdOntkoppeldVanAdressenregister(
                                                     scenario.VCode.Value,
                                                     locatie.LocatieId,
                                                     EventFactory.AdresId(locatie.AdresId),
                                                     EventFactory.Adres(locatie.Adres)));
    }
}
