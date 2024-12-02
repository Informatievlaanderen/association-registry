namespace AssociationRegistry.Test.GrarUpdates.TeOntkoppelenLocatiesHandlerTests;

using Acties.OntkoppelAdres;
using Common.Framework;
using Common.Scenarios.CommandHandling;
using Events;
using Grar.GrarUpdates.Fusies.TeOntkoppelenLocaties;
using Vereniging;
using Xunit;

public class Given_A_Single_Existing_Locatie
{
    [Fact]
    public async Task Then_The_Locatie_Is_Ontkoppeld()
    {
        var scenario = new AdresWerdOvergenomenUitAdressenregisterScenario();

        var verenigingRepositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState());

        var message = new TeOntkoppelenLocatiesMessage(scenario.VCode,
        [
            scenario.LocatieWerdToegevoegd.Locatie.LocatieId,
        ]);

        var sut = new TeOntkoppelenLocatiesHandler(verenigingRepositoryMock);

        await sut.Handle(message, CancellationToken.None);

        var adres = scenario.AdresWerdOvergenomenUitAdressenregister.Adres;

        verenigingRepositoryMock.ShouldHaveSaved(
            new AdresWerdOntkoppeldVanAdressenregister(
                scenario.VCode,
                scenario.LocatieWerdToegevoegd.Locatie.LocatieId,
                scenario.AdresWerdOvergenomenUitAdressenregister.AdresId,
                new Registratiedata.Adres(adres.Straatnaam, adres.Huisnummer, adres.Busnummer, adres.Postcode, adres.Gemeente,
                                          Adres.België)));
    }
}
