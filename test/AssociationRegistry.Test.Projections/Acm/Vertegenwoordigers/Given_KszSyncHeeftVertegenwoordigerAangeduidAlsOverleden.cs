namespace AssociationRegistry.Test.Projections.Acm.Vertegenwoordigers;

using Scenario.Vertegenwoordigers.Vzer;

[Collection(nameof(ProjectionContext))]
public class Given_KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden(
    VerenigingenPerInszScenarioFixture<KszSyncHeeftVertegenwoordigerAangeduidAlsOverledenScenario> fixture)
    : VerenigingenPerInszScenarioClassFixture<KszSyncHeeftVertegenwoordigerAangeduidAlsOverledenScenario>
{
    [Fact]
    public void Then()
    {
        fixture.Result.Verenigingen
               .FirstOrDefault(x => x.VCode == fixture.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode)
               .Should().BeNull();
    }
}
