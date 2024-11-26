namespace AssociationRegistry.Test.Projections.Beheer.Zoeken;

using Admin.Schema;
using Admin.Schema.Search;
using FluentAssertions;
using Framework;
using Framework.Fixtures;
using JsonLdContext;
using Scenarios;
using Xunit;

[Collection(nameof(ProjectionContext))]
public class Given_WerkingsgebiedenWerdenGewijzigd : IClassFixture<BeheerZoekClassFixture<WerkingsgebiedenWerdenGewijzigdScenario>>
{
    private readonly ProjectionContext _context;
    private readonly BeheerZoekClassFixture<WerkingsgebiedenWerdenGewijzigdScenario> _fixture;

    public Given_WerkingsgebiedenWerdenGewijzigd(
        ProjectionContext context,
        BeheerZoekClassFixture<WerkingsgebiedenWerdenGewijzigdScenario> fixture)
    {
        _context = context;
        _fixture = fixture;
    }

    [Fact]
    public void Document_Is_Updated()
    {
        _fixture.Document.Werkingsgebieden
                   .Should().BeEquivalentTo(_fixture.Scenario.WerkingsgebiedenWerdenGewijzigd.Werkingsgebieden.Select(
                                                s => new VerenigingZoekDocument.Werkingsgebied
                                                {
                                                    JsonLdMetadata = new JsonLdMetadata(
                                                        JsonLdType.Werkingsgebied.CreateWithIdValues(s.Code),
                                                        JsonLdType.Werkingsgebied.Type),
                                                    Code = s.Code,
                                                    Naam = s.Naam,
                                                }));
    }
}
