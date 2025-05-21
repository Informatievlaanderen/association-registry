namespace AssociationRegistry.Test.Common.StubsMocksFakes;

using AutoFixture;
using Clocks;
using DecentraalBeheer.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid;
using EventStore;
using global::AutoFixture;
using Moq;
using Stubs.VCodeServices;
using Vereniging;
using Vereniging.Geotags;
using VerenigingsRepositories;

public class Faktory(Fixture fixture)
{
    public static Faktory New(Fixture? fixture = null)
        => new(fixture ?? new Fixture().CustomizeDomain());

    public VCodeServiceFactory VCodeService { get; } = new VCodeServiceFactory();
    public VerenigingsRepositoryFactory VerenigingsRepository { get; } = new VerenigingsRepositoryFactory();
    public ClockFactory Clock { get; } = new ClockFactory();
    public GeotagsServiceFactory GeotagsService { get; } = new GeotagsServiceFactory(fixture);
}

public class VCodeServiceFactory
{
    public StubVCodeService Stub(VCode vCode)
        => new StubVCodeService(vCode);
}

public class VerenigingsRepositoryFactory
{
    public VerenigingRepositoryMock Mock()
        => new();
}

public class ClockFactory
{
    public ClockStub Stub(DateOnly date)
        => new(date);
}

public class GeotagsServiceFactory(Fixture fixture)
{
    public (IGeotagsService Object, GeoTag[] geotags) MockWithRandomGeotags(
        IEnumerable<Locatie> locaties,
        Werkingsgebied[] werkingsgebieden)
    {
        var geotagsService = new Mock<IGeotagsService>();

        var geotags = fixture.CreateMany<GeoTag>().ToArray();

        geotagsService.Setup(x => x.CalculateGeotags(locaties.ToArray(), werkingsgebieden))
                       .ReturnsAsync(geotags);

        return (geotagsService.Object, geotags);
    }

}
