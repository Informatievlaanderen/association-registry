namespace AssociationRegistry.Test.Projections.Acm.Vertegenwoordigers;

using Scenario.Vertegenwoordigers.Kbo;

public class Given_VertegenwoordigerWerdToegevoegdVanuitKBO
{
    [Collection(nameof(ProjectionContext))]
    public class Given_Vertegenwoordiger_Toegevoegd_Na_Aanvaarding_Dubbel(
        VerenigingenPerInszScenarioFixture<VertegenwoordigerWerdToegevoegdVanuitKBOScenario> fixture)
        : VerenigingenPerInszScenarioClassFixture<VertegenwoordigerWerdToegevoegdVanuitKBOScenario>
    {
        [Fact]
        public void Then()
        {
            fixture.Result.Verenigingen
                   .FirstOrDefault(x => x.VCode == fixture.Scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode)
                   .Should().NotBeNull();
        }
    }
}
