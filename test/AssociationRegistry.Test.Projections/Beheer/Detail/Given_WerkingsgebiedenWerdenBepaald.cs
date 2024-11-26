namespace AssociationRegistry.Test.Projections.Beheer.Detail;

using Admin.ProjectionHost.Projections.Detail;
using Admin.Schema.Detail;
using FluentAssertions;
using Framework;
using JsonLdContext;
using Xunit;

[Collection(nameof(ProjectionContext))]
public class Given_WerkingsgebiedenWerdenBepaald(WerkingsgebiedenWerdenBepaaldFixture fixture)
    : IClassFixture<WerkingsgebiedenWerdenBepaaldFixture>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(2);

    [Fact]
    public void Document_Is_Updated()
        => fixture.Result
                  .Werkingsgebieden
                  .Should()
                  .BeEquivalentTo(
                       fixture.Scenario
                              .WerkingsgebiedenWerdenBepaald
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
