namespace AssociationRegistry.Test.Projections.Beheer.Historiek;

using Admin.Schema.Historiek;
using Events;
using FluentAssertions;
using Framework;
using Marten;
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
    public async Task Metadata_Is_Updated()
    {
        var document =
            await _context
                 .Session
                 .Query<BeheerVerenigingHistoriekDocument>()
                 .Where(w => w.VCode == _scenario.WerkingsgebiedenWerdenBepaald.VCode)
                 .SingleAsync();

        document.Metadata.Version.Should().Be(2);
    }

    [Fact]
    public async Task Document_Is_Updated()
    {
        var document =
            await _context
                 .Session
                 .Query<BeheerVerenigingHistoriekDocument>()
                 .Where(w => w.VCode == _scenario.WerkingsgebiedenWerdenBepaald.VCode)
                 .SingleAsync();

        document.Gebeurtenissen.Last()
                .Should().BeEquivalentTo(new BeheerVerenigingHistoriekGebeurtenis(
                                             Beschrijving: "Werkingsgebieden werden bepaald.",
                                             nameof(WerkingsgebiedenWerdenBepaald),
                                             _scenario.WerkingsgebiedenWerdenBepaald,
                                             _context.MetadataInitiator,
                                             _context.MetadataTijdstip));
    }
}
