namespace AssociationRegistry.Test.Projections.Beheer.Detail;

using Admin.Schema.Detail;
using FluentAssertions;
using Framework;
using Marten;
using ScenarioClassFixtures;
using Xunit;

[Collection(nameof(ProjectionContext))]
public class Given_LidmaatschapWerdToegevoegd : IClassFixture<LidmaatschapWerdToegevoegdScenario>
{
    private readonly ProjectionContext _context;
    private readonly LidmaatschapWerdToegevoegdScenario _scenario;

    public Given_LidmaatschapWerdToegevoegd(
        ProjectionContext context,
        LidmaatschapWerdToegevoegdScenario scenario)
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
                 .Where(w => w.VCode == _scenario.VerenigingWerdGeregistreerd.VCode)
                 .SingleAsync();

        document.Metadata.Version.Should().Be(2);
    }

    [Fact]
    public async Task Document_Is_Updated()
    {
        var document =
            await _context
                 .Session
                 .Query<BeheerVerenigingDetailDocument>()
                 .Where(w => w.VCode == _scenario.VerenigingWerdGeregistreerd.VCode)
                 .SingleAsync();

        document.Lidmaatschappen[0]
                .Should()
                .BeEquivalentTo(
                     new Lidmaatschap(
                         JsonLdMetadata: null,
                         _scenario.LidmaatschapWerdToegevoegd.Lidmaatschap.LidmaatschapId,
                         _scenario.LidmaatschapWerdToegevoegd.Lidmaatschap.AndereVereniging,
                         _scenario.LidmaatschapWerdToegevoegd.Lidmaatschap.DatumVan,
                         _scenario.LidmaatschapWerdToegevoegd.Lidmaatschap.DatumTot,
                         _scenario.LidmaatschapWerdToegevoegd.Lidmaatschap.Identificatie,
                         _scenario.LidmaatschapWerdToegevoegd.Lidmaatschap.Beschrijving
                     ),
                     config: options => options.Excluding(x => x.JsonLdMetadata));
    }
}
