namespace AssociationRegistry.Test.Projections.Beheer.Detail.Dubbels;

using Scenario.Dubbels;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingAanvaarddeCorrectieDubbeleVereniging(BeheerDetailScenarioFixture<VerenigingAanvaarddeCorrectieDubbeleVerenigingScenario> fixture)
    : BeheerDetailScenarioClassFixture<VerenigingAanvaarddeCorrectieDubbeleVerenigingScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(3);

    [Fact]
    public void Document_Does_Not_Have_DubbeleVereniging_In_CorresponderendeVCodes()
        => fixture.Result.CorresponderendeVCodes.Should().NotContain(fixture.Scenario.DubbeleVerenigingWerdGeregistreerd.VCode);
}
