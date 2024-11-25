namespace AssociationRegistry.Test.Projections.Publiek.Detail;

using Admin.Schema.Detail;
using FluentAssertions;
using Framework;
using Marten;
using Public.Schema.Detail;
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

    public async Task Document_Is_Updated()
    {
        var document =
            await _context
                 .Session
                 .Query<PubliekVerenigingDetailDocument>()
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
