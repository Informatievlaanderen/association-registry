namespace AssociationRegistry.Test.Projections.Publiek.Zoeken;

using Public.Schema.Search;
using FluentAssertions;
using Framework;
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
                                                    Code = Werkingsgebied.NietVanToepassing.Code,
                                                    Naam = Werkingsgebied.NietVanToepassing.Naam,
                                                },
                                            ],
                                            config: options => options.Excluding(x => x.JsonLdMetadata));
    }
}
