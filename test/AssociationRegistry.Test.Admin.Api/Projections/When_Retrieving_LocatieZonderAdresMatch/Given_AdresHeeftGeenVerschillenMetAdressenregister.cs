namespace AssociationRegistry.Test.Admin.Api.Projections.When_Retrieving_LocatieZonderAdresMatch;

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
    public async Task The_Record_Has_Been_Removed()
    {
        var locatieZonderAdresMatchDocument =
            await _context
                 .Session
                 .Query<LocatieZonderAdresMatchDocument>()
                 .Where(w => w.VCode == _scenario.AdresHeeftGeenVerschillenMetAdressenregister.VCode
                          && w.LocatieId == _scenario.AdresHeeftGeenVerschillenMetAdressenregister.LocatieId)
                 .SingleOrDefaultAsync();

        locatieZonderAdresMatchDocument.Should().BeNull();
    }
}
