namespace AssociationRegistry.Test.GrarUpdates.TeOntkoppelenLocatiesHandlerTests;

using Acties.OntkoppelAdres;
using Common.Framework;
using Common.Scenarios.CommandHandling;
using Events;
using Grar.GrarUpdates.Fusies.TeOntkoppelenLocaties;
using Vereniging;
using Xunit;

public class Given_Multiple_Existing_Locatie
{
    [Fact]
    public async Task Then_The_Locaties_Are_Ontkoppeld()
    {
        var scenario = new MultipleAdresWerdOvergenomenUitAdressenregisterScenario();

        var verenigingRepositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState());

        var message = new OntkoppelLocatiesMessage(scenario.VCode,
        [
            scenario.LocatieWerdToegevoegd.Locatie.LocatieId,
            scenario.LocatieWerdToegevoegd2.Locatie.LocatieId,
        ]);

        var sut = new OntkoppelLocatiesHandler(verenigingRepositoryMock);

        await sut.Handle(message, CancellationToken.None);

        var adres = scenario.AdresWerdOvergenomenUitAdressenregister.Adres;
        var adres2 = scenario.AdresWerdOvergenomenUitAdressenregister2.Adres;

        verenigingRepositoryMock.ShouldHaveSaved(
            new AdresWerdOntkoppeldVanAdressenregister(
                scenario.VCode,
                scenario.LocatieWerdToegevoegd.Locatie.LocatieId,
                scenario.AdresWerdOvergenomenUitAdressenregister.AdresId,
                new Registratiedata.Adres(adres.Straatnaam, adres.Huisnummer, adres.Busnummer, adres.Postcode, adres.Gemeente,
                                          Adres.België)),
            new AdresWerdOntkoppeldVanAdressenregister(
                scenario.VCode,
                scenario.LocatieWerdToegevoegd2.Locatie.LocatieId,
                scenario.AdresWerdOvergenomenUitAdressenregister2.AdresId,
                new Registratiedata.Adres(adres2.Straatnaam, adres2.Huisnummer, adres2.Busnummer, adres2.Postcode, adres2.Gemeente,
                                          Adres.België)));
    }
}
