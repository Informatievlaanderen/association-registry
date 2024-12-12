namespace AssociationRegistry.Test.Projections.Beheer.Zoeken.Dubbels;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingAanvaarddeDubbeleVereniging(BeheerZoekenScenarioFixture<VerenigingAanvaarddeDubbeleVerenigingScenario> fixture)
    : BeheerZoekenScenarioClassFixture<VerenigingAanvaarddeDubbeleVerenigingScenario>
{
    [Fact]
    public void CorresponderendeVCodes_Contains_DubbeleVereniging()
        => fixture.Result.CorresponderendeVCodes.Should().Contain(fixture.Scenario.DubbeleVerenigingWerdGeregistreerd.VCode);
}
