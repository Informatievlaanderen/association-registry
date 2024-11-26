namespace AssociationRegistry.Test.Projections.Beheer.Detail.Werkingsgebieden;

using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.Test.Projections.Framework;
using AssociationRegistry.Test.Projections.ScenarioClassFixtures;
using FluentAssertions;
using Marten;
using Xunit;

[Collection(nameof(ProjectionContext))]
public class Given_WerkingsgebiedenWerdenNietVanToepassing : IClassFixture<WerkingsgebiedenWerdenNietVanToepassingScenario>
{
    private readonly ProjectionContext _context;
    private readonly WerkingsgebiedenWerdenNietVanToepassingScenario _scenario;

    public Given_WerkingsgebiedenWerdenNietVanToepassing(
        ProjectionContext context,
        WerkingsgebiedenWerdenNietVanToepassingScenario scenario)
    {
        _context = context;
        _scenario = scenario;
    }

    [Fact]
    public async Task Metadata_Is_Updated()
    {
        var document =
            await _context
                 .Session
                 .Query<BeheerVerenigingDetailDocument>()
                 .Where(w => w.VCode == _scenario.WerkingsgebiedenWerdenNietVanToepassing.VCode)
                 .SingleAsync();

        document.Metadata.Version.Should().Be(3);
    }

    [Fact]
    public async Task Document_Is_Updated()
    {
        var document =
            await _context
                 .Session
                 .Query<BeheerVerenigingDetailDocument>()
                 .Where(w => w.VCode == _scenario.WerkingsgebiedenWerdenNietVanToepassing.VCode)
                 .SingleAsync();

        document.Werkingsgebieden
                .Should()
                .BeEquivalentTo(
                     [
                         new Werkingsgebied
                         {
                             Code = Vereniging.Werkingsgebied.NietVanToepassing.Code,
                             Naam = Vereniging.Werkingsgebied.NietVanToepassing.Naam,
                         },
                     ],
                     config: options => options.Excluding(x => x.JsonLdMetadata));
    }
}
