namespace AssociationRegistry.Test.Projections.Acm.Dubbels;

using Scenario.Dubbels;

[Collection(nameof(ProjectionContext))]
public class Given_WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt(
    VerenigingenPerInszScenarioFixture<WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerktScenario> fixture)
    : VerenigingenPerInszScenarioClassFixture<WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerktScenario>
{
    [Fact]
    public void Then_Dubbele_Vereniging_Is_Gemarkeerd_Als_Dubbel()
    {
        var dubbeleVereniging =
            fixture.Result.Verenigingen.Single(x => x.VCode == fixture.Scenario.DubbeleVerenigingWerdGeregistreerd.VCode);

        dubbeleVereniging.IsDubbel.Should().BeFalse();
    }
}
