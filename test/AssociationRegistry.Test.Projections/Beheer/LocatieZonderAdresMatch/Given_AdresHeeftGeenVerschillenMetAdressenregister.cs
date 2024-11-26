namespace AssociationRegistry.Test.Projections.Beheer.LocatieZonderAdresMatch;

using Admin.Schema.Detail;
using FluentAssertions;
using Framework;
using Framework.Fixtures;
using Marten;
using Scenarios;
using Xunit;

[Collection(nameof(ProjectionContext))]
public class Given_AdresHeeftGeenVerschillenMetAdressenregister : IClassFixture<LocatieZonderAdresMatchClassFixture<AdresHeeftGeenVerschillenMetAdressenregisterScenario>>
{
    private readonly ProjectionContext _context;
    private readonly LocatieZonderAdresMatchClassFixture<AdresHeeftGeenVerschillenMetAdressenregisterScenario> _fixture;

    public Given_AdresHeeftGeenVerschillenMetAdressenregister(
        ProjectionContext context,
        LocatieZonderAdresMatchClassFixture<AdresHeeftGeenVerschillenMetAdressenregisterScenario> fixture)
    {
        _context = context;
        _fixture = fixture;
    }

    [Fact]
    public async Task The_Record_Has_Been_Removed()
    {
        var locatieZonderAdresMatchDocument =
            await _context
                 .Session
                 .Query<LocatieZonderAdresMatchDocument>()
                 .Where(w => w.VCode == _fixture.Scenario.AdresHeeftGeenVerschillenMetAdressenregister.VCode
                          && w.LocatieId == _fixture.Scenario.AdresHeeftGeenVerschillenMetAdressenregister.LocatieId)
                 .SingleOrDefaultAsync();

        locatieZonderAdresMatchDocument.Should().BeNull();
    }
}
