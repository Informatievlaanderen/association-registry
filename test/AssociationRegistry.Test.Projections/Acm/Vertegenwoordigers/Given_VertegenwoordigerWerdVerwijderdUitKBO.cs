namespace AssociationRegistry.Test.Projections.Acm.Vertegenwoordigers;

using Scenario.Vertegenwoordigers.Kbo;

[Collection(nameof(ProjectionContext))]
public class Given_VertegenwoordigerWerdVerwijderdUitKBO(
    VerenigingenPerInszScenarioFixture<VertegenwoordigerWerdVerwijderdUitKBOScenario> fixture)
    : VerenigingenPerInszScenarioClassFixture<VertegenwoordigerWerdVerwijderdUitKBOScenario>
{
    [Fact]
    public void Then()
    {
        fixture.Result.Verenigingen
               .FirstOrDefault(x => x.VCode == fixture.Scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode)
               .Should().BeNull();
    }
}
