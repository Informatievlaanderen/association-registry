namespace AssociationRegistry.Test.Grar.GrarUpdates.TeOntkoppelenLocatiesHandlerTests;

using AssociationRegistry.Events;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling;
using AssociationRegistry.Vereniging;
using CommandHandling.Grar.GrarConsumer.Messaging.OntkoppelAdres;
using Common.StubsMocksFakes.VerenigingsRepositories;
using DecentraalBeheer.Vereniging.Adressen;
using Xunit;

public class Given_Multiple_Existing_Locatie
{
    [Fact]
    public async ValueTask Then_The_Locaties_Are_Ontkoppeld()
    {
        var scenario = new MultipleAdresWerdOvergenomenUitAdressenregisterScenario();

        var verenigingRepositoryMock = new AggregateSessionMock(
            scenario.GetVerenigingState(),
            expectedLoadingDubbel: true
        );

        var message = new OntkoppelLocatiesMessage(
            scenario.VCode,
            [scenario.LocatieWerdToegevoegd.Locatie.LocatieId, scenario.LocatieWerdToegevoegd2.Locatie.LocatieId]
        );

        var sut = new OntkoppelLocatiesMessageHandler(verenigingRepositoryMock);

        await sut.Handle(message, CancellationToken.None);

        var adres = scenario.AdresWerdOvergenomenUitAdressenregister.Adres;
        var adres2 = scenario.AdresWerdOvergenomenUitAdressenregister2.Adres;

        verenigingRepositoryMock.ShouldHaveSavedExact(
            new AdresWerdOntkoppeldVanAdressenregister(
                scenario.VCode,
                scenario.LocatieWerdToegevoegd.Locatie.LocatieId,
                scenario.AdresWerdOvergenomenUitAdressenregister.AdresId,
                new Registratiedata.Adres(
                    adres.Straatnaam,
                    adres.Huisnummer,
                    adres.Busnummer,
                    adres.Postcode,
                    adres.Gemeente,
                    Adres.België
                )
            ),
            new AdresWerdOntkoppeldVanAdressenregister(
                scenario.VCode,
                scenario.LocatieWerdToegevoegd2.Locatie.LocatieId,
                scenario.AdresWerdOvergenomenUitAdressenregister2.AdresId,
                new Registratiedata.Adres(
                    adres2.Straatnaam,
                    adres2.Huisnummer,
                    adres2.Busnummer,
                    adres2.Postcode,
                    adres2.Gemeente,
                    Adres.België
                )
            )
        );
    }
}
