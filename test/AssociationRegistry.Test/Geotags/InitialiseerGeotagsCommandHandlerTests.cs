namespace AssociationRegistry.Test.Geotags;

using AssociationRegistry.Framework;
using Common.Scenarios.CommandHandling;
using Common.StubsMocksFakes;
using Common.StubsMocksFakes.Faktories;
using Common.StubsMocksFakes.VerenigingsRepositories;
using DecentraalBeheer.Geotags.InitialiseerGeotags;
using EventFactories;
using Vereniging;
using Xunit;

public class InitialiseerGeotagsCommandHandlerTests
{
    [Fact]
    public async Task InitialiseGeotagsCommandHandler()
    {
        var scenario = new WerkingsgebiedenWerdenBepaaldScenario();

        var locaties = scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Locaties;
        var werkingsgebieden = scenario.WerkingsgebiedenWerdenBepaald.Werkingsgebieden;

        var verenigingRepository = new VerenigingRepositoryMock(scenario.GetVerenigingState(), expectedLoadingDubbel: true, expectedLoadingVerwijderd: true);

        var geotagsService =
            Faktory.New()
                   .GeotagsService
                   .ReturnsRandomGeotags(locaties.Select(Locatie.Hydrate),
                                          werkingsgebieden.Select(x => Werkingsgebied.Hydrate(x.Code, x.Naam))
                                                          .ToArray());

        var sut = new InitialiseerGeotagsCommandHandler(verenigingRepository, geotagsService.Object);

        await sut.Handle(new CommandEnvelope<InitialiseerGeotagsCommand>(new InitialiseerGeotagsCommand(scenario.VCode),
                                                                         CommandMetadata.ForDigitaalVlaanderenProcess));

        verenigingRepository.ShouldHaveSaved(EventFactory.GeotagsWerdenBepaald(scenario.VCode, geotagsService.geotags));
    }
}
