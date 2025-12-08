namespace AssociationRegistry.Test.Projections.Acm.Vertegenwoordigers;

using Scenario.Vertegenwoordigers.Vzer;

[Collection(nameof(ProjectionContext))]
public class Given_KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekend(
    VerenigingenPerInszScenarioFixture<KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekendScenario> fixture)
    : VerenigingenPerInszScenarioClassFixture<KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekendScenario>
{
    [Fact]
    public void Then()
    {
        fixture.Result.Verenigingen
               .FirstOrDefault(x => x.VCode == fixture.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode)
               .Should().BeNull();
    }
}
