namespace AssociationRegistry.Test.Admin.Api.Projections.V2.Detail;

using AssociationRegistry.Admin.Schema.Detail;
using FluentAssertions;
using Marten;
using When_Retrieving_LocatieZonderAdresMatch.ScenarioClassFixtures;
using Xunit;

[Collection("detailcollection")]
public class Given_AdresHeeftGeenVerschillenMetAdressenregister : IClassFixture<AdresHeeftGeenVerschillenMetAdressenregisterScenario>
{
    private readonly DetailContext _context;
    private readonly AdresHeeftGeenVerschillenMetAdressenregisterScenario _scenario;

    public Given_AdresHeeftGeenVerschillenMetAdressenregister(
        DetailContext context,
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
                 .Where(w => w.VCode == _scenario.FeitelijkeVerenigingWerdGeregistreerd.VCode)
                 .SingleAsync();

        document.Metadata.Sequence.Should().Be(2);
        document.Metadata.Version.Should().Be(2);
    }
}
