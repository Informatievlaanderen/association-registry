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
public class Given_WerkingsgebiedenWerdenBepaald : IClassFixture<BeheerDetailClassFixture<WerkingsgebiedenWerdenBepaaldScenario>>
{
    private readonly BeheerDetailClassFixture<WerkingsgebiedenWerdenBepaaldScenario> _fixture;

    public Given_WerkingsgebiedenWerdenBepaald(
        BeheerDetailClassFixture<WerkingsgebiedenWerdenBepaaldScenario> fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Metadata_Is_Updated()
        => _fixture.Document.Metadata.Version.Should().Be(2);

    [Fact]
    public void Document_Is_Updated()
        => _fixture.Document.Werkingsgebieden
                   .Should()
                   .BeEquivalentTo(_fixture
                                  .Scenario.WerkingsgebiedenWerdenBepaald
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
