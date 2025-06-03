namespace AssociationRegistry.Test.Common.StubsMocksFakes.Faktories;

using AssociationRegistry.Vereniging;
using AssociationRegistry.Vereniging.Geotags;
using global::AutoFixture;
using Moq;
using System.Collections;

public class GeotagsServiceFactory(IFixture fixture)
{
    public (Mock<IGeotagsService> geotagsService, GeotagsCollection geotags) ReturnsRandomGeotags()
    {
        var geotagsService = new Mock<IGeotagsService>();

        var geotags = GeotagsCollection.Hydrate(fixture.CreateMany<Geotag>().ToArray());

        geotagsService.Setup(x => x.CalculateGeotags(It.IsAny<string[]>(), It.IsAny<string[]>()))
                      .ReturnsAsync(geotags);


        geotagsService.Setup(x => x.CalculateGeotags(It.IsAny<IEnumerable<Locatie>>(), It.IsAny<IEnumerable<Werkingsgebied>>()))
                      .ReturnsAsync(geotags);

        return (geotagsService, geotags);

    }

    public Mock<IGeotagsService> ReturnsEmptyGeotags()
    {
        var geotagsService = new Mock<IGeotagsService>();

        var geotags = GeotagsCollection.Empty;

        geotagsService.Setup(x => x.CalculateGeotags(It.IsAny<string[]>(), It.IsAny<string[]>()))
                      .ReturnsAsync(geotags);


        geotagsService.Setup(x => x.CalculateGeotags(It.IsAny<IEnumerable<Locatie>>(), It.IsAny<IEnumerable<Werkingsgebied>>()))
                      .ReturnsAsync(geotags);

        return geotagsService;

    }

    public (Mock<IGeotagsService> geotagsService, GeotagsCollection geotags) ReturnsRandomGeotags(
        IEnumerable<Locatie> givenLocaties,
        IEnumerable<Werkingsgebied> givenWerkingsgebieden)
    {
        var geotagsService = new Mock<IGeotagsService>();

        var geotags = GeotagsCollection.Hydrate(fixture.CreateMany<Geotag>().ToArray());

        SetUpForObjects(geotagsService, givenLocaties, givenWerkingsgebieden, geotags);
        SetUpForStrings(geotagsService, givenLocaties.Select(x => x.Adres.Postcode).ToArray(), givenWerkingsgebieden.Select(x => x.Code).ToArray(), geotags);

        return (geotagsService, geotags);
    }

    public (Mock<IGeotagsService> geotagsService, GeotagsCollection geotags) ReturnsRandomGeotags(
        IEnumerable<string> givenLocaties,
        IEnumerable<string> givenWerkingsgebieden)
    {
        var geotagsService = new Mock<IGeotagsService>();

        var geotags = GeotagsCollection.Hydrate(fixture.CreateMany<Geotag>().ToArray());

        SetUpForStrings(geotagsService, givenLocaties, givenWerkingsgebieden, geotags);

        return (geotagsService, geotags);
    }

    public (Mock<IGeotagsService> geotagsService, GeotagsCollection geotags)  RetunsGeotags(GeotagsCollection geotags)
    {

        var geotagsService = new Mock<IGeotagsService>();

        geotagsService.Setup(x => x.CalculateGeotags(It.IsAny<string[]>(), It.IsAny<string[]>()))
                      .ReturnsAsync(geotags);


        geotagsService.Setup(x => x.CalculateGeotags(It.IsAny<IEnumerable<Locatie>>(), It.IsAny<IEnumerable<Werkingsgebied>>()))
                      .ReturnsAsync(geotags);

        return (geotagsService, geotags);
    }

    private static void SetUpForObjects(Mock<IGeotagsService> geotagsService, IEnumerable<Locatie> givenLocaties, IEnumerable<Werkingsgebied> givenWerkingsgebieden, GeotagsCollection geotags)
    {
        geotagsService.Setup(x => x.CalculateGeotags(givenLocaties.ToArray(), givenWerkingsgebieden.ToArray()))
                      .ReturnsAsync(geotags);
    }

    private static void SetUpForStrings(Mock<IGeotagsService> geotagsService, IEnumerable<string> givenLocaties, IEnumerable<string> givenWerkingsgebieden, GeotagsCollection returnsGeotags)
        => geotagsService.Setup(x => x.CalculateGeotags(givenLocaties.ToArray(), givenWerkingsgebieden.ToArray()))
                                                                                .ReturnsAsync(returnsGeotags);
}
