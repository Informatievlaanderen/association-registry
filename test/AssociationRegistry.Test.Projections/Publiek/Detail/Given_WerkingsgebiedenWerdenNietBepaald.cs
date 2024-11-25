namespace AssociationRegistry.Test.Projections.Publiek.Detail;

using FluentAssertions;
using Framework;
using Marten;
using Public.Schema.Detail;
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
        var document =
            await _context
                 .Session
                 .Query<PubliekVerenigingDetailDocument>()
                 .Where(w => w.VCode == _scenario.WerkingsgebiedenWerdenNietBepaald.VCode)
                 .SingleAsync();

        document.Werkingsgebieden
                .Should()
                .BeEmpty();
    }
}
