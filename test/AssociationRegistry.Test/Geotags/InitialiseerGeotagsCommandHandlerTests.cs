namespace AssociationRegistry.Test.Geotags;

using AssociationRegistry.Framework;
using CommandHandling.DecentraalBeheer.Acties.Geotags.InitialiseerGeotags;
using Common.Scenarios.CommandHandling;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Common.StubsMocksFakes;
using Common.StubsMocksFakes.Faktories;
using Common.StubsMocksFakes.VerenigingsRepositories;
using DecentraalBeheer.Vereniging;
using Events.Factories;
using Vereniging;
using Xunit;

public class InitialiseerGeotagsCommandHandlerTests
{
    [Fact]
    public async Task InitialiseGeotagsCommandHandler()
    {
        var scenario = new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdZonderGeotagsInitialisatieScenario();

        var locaties = scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Locaties;
        var werkingsgebieden = scenario.WerkingsgebiedenWerdenBepaald.Werkingsgebieden;

        var verenigingRepository = new VerenigingRepositoryMock(scenario.GetVerenigingState(), expectedLoadingDubbel: true, expectedLoadingVerwijderd: true);

        var (geotagsService, geotags) = Faktory.New()
                                                                        .GeotagsService
                                                                        .ReturnsRandomGeotags(locaties.Select(Locatie.Hydrate),
                                                                             werkingsgebieden.Select(x => Werkingsgebied.Hydrate(x.Code, x.Naam))
                                                                                .ToArray());

        var sut = new InitialiseerGeotagsCommandHandler(verenigingRepository, geotagsService.Object);

        await sut.Handle(new CommandEnvelope<InitialiseerGeotagsCommand>(new InitialiseerGeotagsCommand(scenario.VCode),
                                                                         CommandMetadata.ForDigitaalVlaanderenProcess));

        verenigingRepository.ShouldHaveSavedExact(EventFactory.GeotagsWerdenBepaald(scenario.VCode, geotags));
    }
}
