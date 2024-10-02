namespace AssociationRegistry.Test.Admin.Api.Projections.V2.Beheer.Detail;

using AssociationRegistry.Admin.Schema.Detail;
using FluentAssertions;
using Marten;
using ScenarioClassFixtures;
using Xunit;

[Collection(nameof(ProjectionContext))]
public class Given_AdresHeeftGeenVerschillenMetAdressenregister : IClassFixture<AdresHeeftGeenVerschillenMetAdressenregisterScenario>
{
    private readonly ProjectionContext _context;
    private readonly AdresHeeftGeenVerschillenMetAdressenregisterScenario _scenario;

    public Given_AdresHeeftGeenVerschillenMetAdressenregister(
        ProjectionContext context,
        AdresHeeftGeenVerschillenMetAdressenregisterScenario scenario)
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
}
