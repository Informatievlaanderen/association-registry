namespace AssociationRegistry.Test.Projections.Beheer.Zoeken;

using Admin.Schema;
using Admin.Schema.Search;
using FluentAssertions;
using Framework;
using JsonLdContext;
using ScenarioClassFixtures;
using Xunit;

[Collection(nameof(ProjectionContext))]
public class Given_WerkingsgebiedenWerdenBepaald : IClassFixture<WerkingsgebiedenWerdenBepaaldScenario>
{
    private readonly ProjectionContext _context;
    private readonly WerkingsgebiedenWerdenBepaaldScenario _scenario;

    public Given_WerkingsgebiedenWerdenBepaald(
        ProjectionContext context,
        WerkingsgebiedenWerdenBepaaldScenario scenario)
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
                 .GetAsync<VerenigingZoekDocument>(_scenario.WerkingsgebiedenWerdenBepaald.VCode);

        getResponse.Source.Werkingsgebieden
                   .Should().BeEquivalentTo(_scenario.WerkingsgebiedenWerdenBepaald.Werkingsgebieden.Select(
                                                s => new VerenigingZoekDocument.Werkingsgebied
                                                {
                                                    JsonLdMetadata = new JsonLdMetadata(
                                                        JsonLdType.Werkingsgebied.CreateWithIdValues(s.Code),
                                                        JsonLdType.Werkingsgebied.Type),
                                                    Code = s.Code,
                                                    Naam = s.Naam,
                                                }));
    }
}
