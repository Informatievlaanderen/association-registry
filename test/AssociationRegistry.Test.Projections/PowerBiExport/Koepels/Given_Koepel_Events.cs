namespace AssociationRegistry.Test.Projections.PowerBiExport.Koepels;

using Events;
using Scenario.Koepels;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingWerdGeidentificeerdAlsGeenKoepel(
    PowerBiScenarioFixture<VerenigingWerdGeidentificeerdAlsGeenKoepelScenario> fixture
) : PowerBiScenarioClassFixture<VerenigingWerdGeidentificeerdAlsGeenKoepelScenario>
{
    [Fact]
    public void Historiek_Contains_The_Koepel_Events()
    {
        fixture
            .Result.Historiek.Skip(1)
            .Select(gebeurtenis => gebeurtenis.EventType)
            .Should()
            .Equal(
                nameof(VerenigingWerdGeidentificeerdAlsKoepel),
                nameof(KoepelPrivacyverklaringWerdGeaccepteerd),
                nameof(VerenigingWerdGeidentificeerdAlsGeenKoepel),
                nameof(KoepelPrivacyverklaringWerdIngetrokken)
            );
    }
}

[Collection(nameof(ProjectionContext))]
public class Given_KoepelstatusWerdTerugGezetNaarNietBepaald(
    PowerBiScenarioFixture<KoepelstatusWerdTerugGezetNaarNietBepaaldScenario> fixture
) : PowerBiScenarioClassFixture<KoepelstatusWerdTerugGezetNaarNietBepaaldScenario>
{
    [Fact]
    public void Historiek_Contains_The_Koepel_Events()
    {
        fixture
            .Result.Historiek.Skip(1)
            .Select(gebeurtenis => gebeurtenis.EventType)
            .Should()
            .Equal(nameof(VerenigingWerdGeidentificeerdAlsKoepel), nameof(KoepelstatusWerdTerugGezetNaarNietBepaald));
    }
}
