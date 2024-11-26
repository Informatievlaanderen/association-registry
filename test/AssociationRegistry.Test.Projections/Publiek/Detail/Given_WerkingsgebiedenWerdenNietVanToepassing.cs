namespace AssociationRegistry.Test.Projections.Publiek.Detail;

using FluentAssertions;
using Framework;
using JsonLdContext;
using Marten;
using Public.Schema.Detail;
using ScenarioClassFixtures;
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
        var document =
            await _context
                 .Session
                 .Query<PubliekVerenigingDetailDocument>()
                 .Where(w => w.VCode == _scenario.WerkingsgebiedenWerdenNietVanToepassing.VCode)
                 .SingleAsync();

        document.Werkingsgebieden
                .Should()
                .BeEquivalentTo(
                 [
                     new PubliekVerenigingDetailDocument.Werkingsgebied
                     {
                         JsonLdMetadata = new JsonLdMetadata(
                             JsonLdType.Werkingsgebied.CreateWithIdValues(Vereniging.Werkingsgebied.NietVanToepassing.Code),
                             JsonLdType.Werkingsgebied.Type),

                         Code = Vereniging.Werkingsgebied.NietVanToepassing.Code,
                         Naam = Vereniging.Werkingsgebied.NietVanToepassing.Naam,
                     },
                 ]);
    }
}
