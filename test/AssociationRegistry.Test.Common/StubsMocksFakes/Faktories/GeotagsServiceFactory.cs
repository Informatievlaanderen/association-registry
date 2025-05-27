namespace AssociationRegistry.Test.Common.StubsMocksFakes.Faktories;

using AssociationRegistry.Vereniging;
using AssociationRegistry.Vereniging.Geotags;
using global::AutoFixture;
using Moq;

public class GeotagsServiceFactory(Fixture fixture)
{
    public (IGeotagsService Object, Geotag[] geotags) ReturnsRandomGeotags(
        IEnumerable<Locatie> givenLocaties,
        IEnumerable<Werkingsgebied> givenWerkingsgebieden)
    {
        var geotagsService = new Mock<IGeotagsService>();

        var geotags = fixture.CreateMany<Geotag>().ToArray();

        geotagsService.Setup(x => x.CalculateGeotags(givenLocaties.ToArray(), givenWerkingsgebieden))
                      .ReturnsAsync(geotags);

        return (geotagsService.Object, geotags);
    }
}
