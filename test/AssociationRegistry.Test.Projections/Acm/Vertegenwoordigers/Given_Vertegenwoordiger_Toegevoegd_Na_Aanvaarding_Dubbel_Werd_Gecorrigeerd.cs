namespace AssociationRegistry.Test.Projections.Acm.Vertegenwoordigers;

[Collection(nameof(ProjectionContext))]
public class Given_Vertegenwoordiger_Toegevoegd_Na_Aanvaarding_Dubbel_Werd_Gecorrigeerd(
    VerenigingenPerInszScenarioFixture<VertegenwoordigerWerdToegevoegdNaVerenigingAanvaarddeCorrectieDubbeleVerenigingScenario> fixture)
    : VerenigingenPerInszScenarioClassFixture<VertegenwoordigerWerdToegevoegdNaVerenigingAanvaarddeCorrectieDubbeleVerenigingScenario>
{

    [Fact]
    public void Then_Authentieke_Vereniging_Has_Not_DubbeleVereniging_As_CorresponderendeVCodes()
    {
        var authentiekeVereniging =
            fixture.Result.Verenigingen.Single(x => x.VCode == fixture.Scenario.AuthentiekeVereniging.VCode);

        authentiekeVereniging.CorresponderendeVCodes.Should().BeEquivalentTo(fixture.Scenario.DubbeleVerengingOmTeHouden.VCode);
    }
}
