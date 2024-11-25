namespace AssociationRegistry.Test.Projections.Beheer.Zoeken;

using Admin.Schema.Search;
using FluentAssertions;
using Framework;
using ScenarioClassFixtures;
using Xunit;

[Collection(nameof(ProjectionContext))]
public class Given_WerkingsgebiedenWerdenNietBepaald : IClassFixture<WerkingsgebiedenWerdenNietBepaaldScenario>
{
    private readonly ProjectionContext _context;
    private readonly WerkingsgebiedenWerdenNietBepaaldScenario _scenario;

    public Given_WerkingsgebiedenWerdenNietBepaald(
        ProjectionContext context,
        WerkingsgebiedenWerdenNietBepaaldScenario scenario)
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
                 .GetAsync<VerenigingZoekDocument>(_scenario.WerkingsgebiedenWerdenNietBepaald.VCode);

        getResponse.Source.Werkingsgebieden
                   .Should().BeEmpty();
    }
}
