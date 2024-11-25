namespace AssociationRegistry.Test.Projections.Beheer.Historiek;

using Admin.Schema.Historiek;
using Events;
using FluentAssertions;
using Framework;
using Marten;
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
    public async Task Metadata_Is_Updated()
    {
        var document =
            await _context
                 .Session
                 .Query<BeheerVerenigingHistoriekDocument>()
                 .Where(w => w.VCode == _scenario.WerkingsgebiedenWerdenNietVanToepassing.VCode)
                 .SingleAsync();

        document.Metadata.Version.Should().Be(3);
    }

    [Fact]
    public async Task Document_Is_Updated()
    {
        var document =
            await _context
                 .Session
                 .Query<BeheerVerenigingHistoriekDocument>()
                 .Where(w => w.VCode == _scenario.WerkingsgebiedenWerdenNietVanToepassing.VCode)
                 .SingleAsync();

        document.Gebeurtenissen.Last()
                .Should().BeEquivalentTo(new BeheerVerenigingHistoriekGebeurtenis(
                                             Beschrijving: "Werkingsgebieden werden niet van toepassing.",
                                             nameof(WerkingsgebiedenWerdenNietVanToepassing),
                                             _scenario.WerkingsgebiedenWerdenNietVanToepassing,
                                             _context.MetadataInitiator,
                                             _context.MetadataTijdstip));
    }
}
