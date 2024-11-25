namespace AssociationRegistry.Test.Projections.Publiek.Detail;

using Admin.Schema.Detail;
using FluentAssertions;
using Framework;
using Marten;
using Public.Schema.Detail;
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
        var document =
            await _context
                 .Session
                 .Query<PubliekVerenigingDetailDocument>()
                 .Where(w => w.VCode == _scenario.WerkingsgebiedenWerdenBepaald.VCode)
                 .SingleAsync();

        document.Werkingsgebieden
                .Should()
                .BeEquivalentTo(_scenario
                               .WerkingsgebiedenWerdenBepaald
                               .Werkingsgebieden
                               .Select(wg => new Werkingsgebied
                                {
                                    Code = wg.Code,
                                    Naam = wg.Naam,
                                }),
                                config: options => options.Excluding(x => x.JsonLdMetadata));
    }
}
