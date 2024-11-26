namespace AssociationRegistry.Test.Projections.Beheer.Detail.Lidmaatschappen;

using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.Test.Projections.Framework;
using AssociationRegistry.Test.Projections.ScenarioClassFixtures;
using FluentAssertions;
using Marten;
using Xunit;

[Collection(nameof(ProjectionContext))]
public class Given_LidmaatschapWerdGewijzigd : IClassFixture<LidmaatschapWerdGewijzigdScenario>
{
    private readonly ProjectionContext _context;
    private readonly LidmaatschapWerdGewijzigdScenario _scenario;

    public Given_LidmaatschapWerdGewijzigd(
        ProjectionContext context,
        LidmaatschapWerdGewijzigdScenario scenario)
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
                 .Where(w => w.VCode == _scenario.LidmaatschapWerdGewijzigd.VCode)
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
                 .Where(w => w.VCode == _scenario.LidmaatschapWerdGewijzigd.VCode)
                 .SingleAsync();

        document.Lidmaatschappen[0]
                .Should()
                .BeEquivalentTo(
                     new Lidmaatschap(
                         JsonLdMetadata: null,
                         _scenario.LidmaatschapWerdGewijzigd.Lidmaatschap.LidmaatschapId,
                         _scenario.LidmaatschapWerdGewijzigd.Lidmaatschap.AndereVereniging,
                         _scenario.LidmaatschapWerdGewijzigd.Lidmaatschap.DatumVan,
                         _scenario.LidmaatschapWerdGewijzigd.Lidmaatschap.DatumTot,
                         _scenario.LidmaatschapWerdGewijzigd.Lidmaatschap.Identificatie,
                         _scenario.LidmaatschapWerdGewijzigd.Lidmaatschap.Beschrijving
                     ),
                     config: options => options.Excluding(x => x.JsonLdMetadata));
    }
}
