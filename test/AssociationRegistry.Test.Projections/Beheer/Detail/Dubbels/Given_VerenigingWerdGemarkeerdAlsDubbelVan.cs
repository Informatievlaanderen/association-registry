namespace AssociationRegistry.Test.Projections.Beheer.Detail.Dubbels;

using Admin.Schema.Constants;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingWerdGemarkeerdAlsDubbelVan(BeheerDetailScenarioFixture<VerenigingWerdGemarkeerdAlsDubbelVanScenario> fixture)
    : BeheerDetailScenarioClassFixture<VerenigingWerdGemarkeerdAlsDubbelVanScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(2);

    [Fact]
    public void Document_IsDubbelVan_Is_Updated()
        => fixture.Result.IsDubbelVan.Should().Be(fixture.Scenario.VerenigingWerdGemarkeerdAlsDubbelVan.VCodeAuthentiekeVereniging);

    [Fact]
    public void Document_Status_Is_Dubbel()
        => fixture.Result.Status.Should().Be(VerenigingStatus.Dubbel);
}
