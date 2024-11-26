namespace AssociationRegistry.Test.Projections.Publiek.Zoeken;

using Public.Schema.Search;
using FluentAssertions;
using Framework;
using JsonLdContext;
using Public.Schema.Detail;
using ScenarioClassFixtures;
using Vereniging;
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
    public async Task Document_Is_Updated()
    {
        var getResponse =
            await _context
                 .PublicElasticClient
                 .GetAsync<VerenigingZoekDocument>(_scenario.WerkingsgebiedenWerdenNietVanToepassing.VCode);

        getResponse.Source.Werkingsgebieden
                   .Should().BeEquivalentTo([
                                                new VerenigingZoekDocument.Werkingsgebied
                                                {
                                                    JsonLdMetadata = new JsonLdMetadata(
                                                        JsonLdType.Werkingsgebied.CreateWithIdValues(Werkingsgebied.NietVanToepassing.Code),
                                                        JsonLdType.Werkingsgebied.Type),
                                                    Code = Werkingsgebied.NietVanToepassing.Code,
                                                    Naam = Werkingsgebied.NietVanToepassing.Naam,
                                                },
                                            ]);
    }
}
