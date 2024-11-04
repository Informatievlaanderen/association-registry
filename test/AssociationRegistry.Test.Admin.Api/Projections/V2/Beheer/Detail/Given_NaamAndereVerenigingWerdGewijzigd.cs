namespace AssociationRegistry.Test.Admin.Api.Projections.V2.Beheer.Detail;

using AssociationRegistry.Admin.Schema.Detail;
using FluentAssertions;
using Marten;
using ScenarioClassFixtures;
using Xunit;

[Collection(nameof(ProjectionContext))]
public class Given_NaamAndereVerenigingWerdGewijzigd : IClassFixture<NaamAndereVerenigingWerdGewijzigdScenario>
{
    private readonly ProjectionContext _context;
    private readonly NaamAndereVerenigingWerdGewijzigdScenario _scenario;

    public Given_NaamAndereVerenigingWerdGewijzigd(
        ProjectionContext context,
        NaamAndereVerenigingWerdGewijzigdScenario scenario)
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
                 .Query<BeheerVerenigingDetailDocument>()
                 .Where(w => w.VCode == _scenario.NaamWerdGewijzigd.VCode)
                 .SingleAsync();

        document.Metadata.Version.Should().Be(3);
    }

    [Fact]
    public async Task Document_Is_Updated()
    {
        var document =
            await _context
                 .Session
                 .Query<BeheerVerenigingDetailDocument>()
                 .Where(w => w.VCode == _scenario.LidmaatschapWerdToegevoegdScenario.VerenigingWerdGeregistreerd.VCode)
                 .SingleAsync();

        document.Lidmaatschappen[0].AndereVereniging.Should().BeEquivalentTo(
            new AndereVereniging(
                _scenario.LidmaatschapWerdToegevoegdScenario.AndereVerenigingWerdGeregistreerd.VCode,
                _scenario.LidmaatschapWerdToegevoegdScenario.AndereVerenigingWerdGeregistreerd.Naam));
    }
}
