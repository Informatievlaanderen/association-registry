﻿namespace AssociationRegistry.Test.Projections.Acm.Dubbels;

[Collection(nameof(ProjectionContext))]
public class Given_MarkeringDubbeleVerengingWerdGecorrigeerd(
    VerenigingenPerInszScenarioFixture<VerenigingAanvaarddeCorrectieDubbeleVerenigingScenario> fixture)
    : VerenigingenPerInszScenarioClassFixture<VerenigingAanvaarddeCorrectieDubbeleVerenigingScenario>
{
    [Fact]
    public void Then_Dubbele_Vereniging_Is_Gemarkeerd_Als_Dubbel()
    {
        var dubbeleVereniging =
            fixture.Result.Verenigingen.Single(x => x.VCode == fixture.Scenario.DubbeleVerenigingWerdGeregistreerd.VCode);

        dubbeleVereniging.IsDubbel.Should().BeFalse();
    }

    [Fact]
    public void Then_Authentieke_Vereniging_Has_CorresponderendeVCodes()
    {
        var authentiekeVereniging =
            fixture.Result.Verenigingen.Single(x => x.VCode == fixture.Scenario.AuthentiekeVerenigingWerdGeregistreerd.VCode);

        authentiekeVereniging.CorresponderendeVCodes.Should().NotContain(fixture.Scenario.DubbeleVerenigingWerdGeregistreerd.VCode);
    }
}
