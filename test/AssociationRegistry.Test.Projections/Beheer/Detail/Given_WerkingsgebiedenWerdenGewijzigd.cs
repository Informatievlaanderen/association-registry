namespace AssociationRegistry.Test.Projections.Beheer.Detail;

using Admin.ProjectionHost.Projections.Detail;
using Admin.Schema.Detail;
using FluentAssertions;
using Framework;
using Framework.Fixtures;
using JsonLdContext;
using Scenarios;
using Xunit;

[Collection(nameof(ProjectionContext))]
public class Given_WerkingsgebiedenWerdenGewijzigd : IClassFixture<BeheerDetailClassFixture<WerkingsgebiedenWerdenGewijzigdScenario>>
{
    private readonly BeheerDetailClassFixture<WerkingsgebiedenWerdenGewijzigdScenario> _fixture;

    public Given_WerkingsgebiedenWerdenGewijzigd(
        BeheerDetailClassFixture<WerkingsgebiedenWerdenGewijzigdScenario> fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Metadata_Is_Updated()
        => _fixture.Document.Metadata.Version.Should().Be(3);

    [Fact]
    public void Document_Is_Updated()
        => _fixture.Document.Werkingsgebieden
                   .Should()
                   .BeEquivalentTo(_fixture
                                  .Scenario.WerkingsgebiedenWerdenGewijzigd
                                  .Werkingsgebieden
                                  .Select(wg => new Werkingsgebied
                                   {
                                       JsonLdMetadata = BeheerVerenigingDetailMapper.CreateJsonLdMetadata(
                                           JsonLdType.Werkingsgebied,
                                           wg.Code),
                                       Code = wg.Code,
                                       Naam = wg.Naam,
                                   }));
}
