namespace AssociationRegistry.Test.Projections.Beheer.Detail.PubliekeDatastroom;

using Scenario.PubliekeDatastroom;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingWerdIngeschrevenInPubliekeDatastroom(
    BeheerDetailScenarioFixture<VerenigingWerdIngeschrevenInPubliekeDatastroomScenario> fixture)
    : BeheerDetailScenarioClassFixture<VerenigingWerdIngeschrevenInPubliekeDatastroomScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(3);

    [Fact]
    public void Document_Is_Updated()
    {
        fixture.Result.IsUitgeschrevenUitPubliekeDatastroom.Should().BeFalse();
    }
}
