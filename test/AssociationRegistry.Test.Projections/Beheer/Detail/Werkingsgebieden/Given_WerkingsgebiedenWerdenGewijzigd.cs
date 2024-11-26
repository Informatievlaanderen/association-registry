namespace AssociationRegistry.Test.Projections.Beheer.Detail.Werkingsgebieden;

using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.Test.Projections.Framework;
using AssociationRegistry.Test.Projections.ScenarioClassFixtures;
using FluentAssertions;
using Marten;
using Xunit;

[Collection(nameof(ProjectionContext))]
public class Given_WerkingsgebiedenWerdenGewijzigd : IClassFixture<WerkingsgebiedenWerdenGewijzigdScenario>
{
    private readonly ProjectionContext _context;
    private readonly WerkingsgebiedenWerdenGewijzigdScenario _scenario;

    public Given_WerkingsgebiedenWerdenGewijzigd(
        ProjectionContext context,
        WerkingsgebiedenWerdenGewijzigdScenario scenario)
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
                 .Where(w => w.VCode == _scenario.WerkingsgebiedenWerdenGewijzigd.VCode)
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
                 .Where(w => w.VCode == _scenario.WerkingsgebiedenWerdenGewijzigd.VCode)
                 .SingleAsync();

        document.Werkingsgebieden
                .Should()
                .BeEquivalentTo(_scenario
                               .WerkingsgebiedenWerdenGewijzigd
                               .Werkingsgebieden
                               .Select(wg => new Werkingsgebied
                                {
                                    Code = wg.Code,
                                    Naam = wg.Naam,
                                }),
                                config: options => options.Excluding(x => x.JsonLdMetadata));

        ;
    }
}
