namespace AssociationRegistry.Test.Grar.GrarUpdates.TeOntkoppelenLocatiesHandlerTests;

using AssociationRegistry.Events;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling;
using AssociationRegistry.Vereniging;
using CommandHandling.Grar.GrarConsumer.Messaging.OntkoppelAdres;
using Common.StubsMocksFakes.VerenigingsRepositories;
using DecentraalBeheer.Vereniging.Adressen;
using Xunit;

public class Given_A_Single_Existing_Locatie
{
    [Fact]
    public async ValueTask Then_The_Locatie_Is_Ontkoppeld()
    {
        var scenario = new AdresWerdOvergenomenUitAdressenregisterScenario();

        var verenigingRepositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState(), expectedLoadingDubbel: true);

        var message = new OntkoppelLocatiesMessage(scenario.VCode,
        [
            scenario.LocatieWerdToegevoegd.Locatie.LocatieId,
        ]);

        var sut = new OntkoppelLocatiesMessageHandler(verenigingRepositoryMock);

        await sut.Handle(message, CancellationToken.None);

        var adres = scenario.AdresWerdOvergenomenUitAdressenregister.Adres;

        verenigingRepositoryMock.ShouldHaveSavedExact(
            new AdresWerdOntkoppeldVanAdressenregister(
                scenario.VCode,
                scenario.LocatieWerdToegevoegd.Locatie.LocatieId,
                scenario.AdresWerdOvergenomenUitAdressenregister.AdresId,
                new Registratiedata.Adres(adres.Straatnaam, adres.Huisnummer, adres.Busnummer, adres.Postcode, adres.Gemeente,
                                          Adres.België)));
    }
}
