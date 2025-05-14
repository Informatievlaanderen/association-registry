namespace AssociationRegistry.Test.Projections.Beheer.Detail.Subtypes;

using Scenario.Subtypes;

[Collection(nameof(ProjectionContext))]
public class Given_SubverengingVerfijndNaarNietBepaald(
    BeheerDetailScenarioFixture<SubverenigingWerdTerugGezetNaarNietBepaaldScenario> fixture)
    : BeheerDetailScenarioClassFixture<SubverenigingWerdTerugGezetNaarNietBepaaldScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(3);

    [Fact]
    public void SubverenigingVan_Is_Cleared()
    {
        fixture.Result.SubverenigingVan.Should().BeNull();
    }
}
