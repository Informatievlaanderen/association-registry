namespace AssociationRegistry.Test.Projections.Acm.Vertegenwoordigers;

using Scenario.Vertegenwoordigers.Kbo;

public class Given_VertegenwoordigerWerdGewijzigdInKBO
{
    [Collection(nameof(ProjectionContext))]
    public class Given_Vertegenwoordiger_Toegevoegd_Na_Aanvaarding_Dubbel(
        VerenigingenPerInszScenarioFixture<VertegenwoordigerWerdGewijzigdInKBOScenario> fixture)
        : VerenigingenPerInszScenarioClassFixture<VertegenwoordigerWerdGewijzigdInKBOScenario>
    {
        [Fact]
        public void Then_Vereniging_Is_Not_Changed()
        {
            fixture.Result.Verenigingen
                   .FirstOrDefault(x => x.VCode == fixture.Scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode)
                   .Should().NotBeNull();
        }
    }
}
