namespace AssociationRegistry.Test.Projections.Beheer.Zoeken;

using Admin.Schema.Search;
using FluentAssertions;
using Framework;
using ScenarioClassFixtures;
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
    public async Task Document_Is_Updated()
    {
        var getResponse =
            await _context
                 .AdminElasticClient
                 .GetAsync<VerenigingZoekDocument>(_scenario.WerkingsgebiedenWerdenGewijzigd.VCode);

        getResponse.Source.Werkingsgebieden
                   .Should().BeEquivalentTo(_scenario.WerkingsgebiedenWerdenGewijzigd.Werkingsgebieden.Select(
                                                s => new VerenigingZoekDocument.Werkingsgebied
                                                {
                                                    Code = s.Code,
                                                    Naam = s.Naam,
                                                }),
                                            config: options => options.Excluding(x => x.JsonLdMetadata));
    }
}
