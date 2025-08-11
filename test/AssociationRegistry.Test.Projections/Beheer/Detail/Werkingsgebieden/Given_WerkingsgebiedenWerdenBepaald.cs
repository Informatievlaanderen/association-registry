namespace AssociationRegistry.Test.Projections.Beheer.Detail.Werkingsgebieden;

using Admin.ProjectionHost.Projections.Detail;
using Admin.Schema.Detail;
using Contracts.JsonLdContext;
using Scenario.Werkingsgebieden;

[Collection(nameof(ProjectionContext))]
public class Given_WerkingsgebiedenWerdenBepaald(BeheerDetailScenarioFixture<WerkingsgebiedenWerdenBepaaldScenario> fixture)
    : BeheerDetailScenarioClassFixture<WerkingsgebiedenWerdenBepaaldScenario>
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
