namespace AssociationRegistry.Test.Projections.Beheer.Detail.PubliekeDatastroom;

using Scenario.PubliekeDatastroom;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingWerdUitgeschrevenUitPubliekeDatastroom(
    BeheerDetailScenarioFixture<VerenigingWerdUitgeschrevenUitPubliekeDatastroomScenario> fixture)
    : BeheerDetailScenarioClassFixture<VerenigingWerdUitgeschrevenUitPubliekeDatastroomScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(3);

    [Fact]
    public void Document_Is_Updated()
    {
        fixture.Result.IsUitgeschrevenUitPubliekeDatastroom.Should().BeTrue();
    }
}
