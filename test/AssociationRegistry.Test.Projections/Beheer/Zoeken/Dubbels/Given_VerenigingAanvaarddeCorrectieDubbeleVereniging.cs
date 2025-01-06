namespace AssociationRegistry.Test.Projections.Beheer.Zoeken.Dubbels;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingAanvaarddeCorrectieDubbeleVereniging(BeheerZoekenScenarioFixture<VerenigingAanvaarddeCorrectieDubbeleVerenigingScenario> fixture)
    : BeheerZoekenScenarioClassFixture<VerenigingAanvaarddeCorrectieDubbeleVerenigingScenario>
{
    [Fact]
    public void CorresponderendeVCodes_Contains_DubbeleVereniging()
        => fixture.Result.CorresponderendeVCodes.Should().NotContain(fixture.Scenario.DubbeleVerenigingWerdGeregistreerd.VCode);
}
