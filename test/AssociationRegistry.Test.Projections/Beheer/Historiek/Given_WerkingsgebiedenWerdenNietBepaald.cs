namespace AssociationRegistry.Test.Projections.Beheer.Historiek;

using Admin.Schema.Historiek;
using Events;
using FluentAssertions;
using Framework;
using Marten;
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
    public async Task Metadata_Is_Updated()
    {
        var document =
            await _context
                 .Session
                 .Query<BeheerVerenigingHistoriekDocument>()
                 .Where(w => w.VCode == _scenario.WerkingsgebiedenWerdenNietBepaald.VCode)
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
                 .Where(w => w.VCode == _scenario.WerkingsgebiedenWerdenNietBepaald.VCode)
                 .SingleAsync();

        document.Gebeurtenissen.Last()
                .Should().BeEquivalentTo(new BeheerVerenigingHistoriekGebeurtenis(
                                             Beschrijving: "Werkingsgebieden werden niet bepaald.",
                                             nameof(WerkingsgebiedenWerdenNietBepaald),
                                             _scenario.WerkingsgebiedenWerdenNietBepaald,
                                             _context.MetadataInitiator,
                                             _context.MetadataTijdstip));
    }
}
