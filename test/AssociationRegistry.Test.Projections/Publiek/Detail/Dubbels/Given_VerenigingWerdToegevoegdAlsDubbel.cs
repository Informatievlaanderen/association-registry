namespace AssociationRegistry.Test.Projections.Publiek.Detail.Dubbels;

using Public.Schema.Constants;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingWerdToegevoegdAlsDubbel(PubliekDetailScenarioFixture<VerenigingWerdToegevoegdAlsDubbelScenario> fixture)
    : PubliekDetailScenarioClassFixture<VerenigingWerdToegevoegdAlsDubbelScenario>
{
    [Fact]
    public void Document_IsDubbelVan_Is_Updated()
        => fixture.Result.IsDubbelVan.Should().BeEmpty();

    [Fact]
    public void Document_Status_Is_Actief()
        => fixture.Result.Status.Should().Be(VerenigingStatus.Actief);

    [Fact]
    public void Document_Has_DubbeleVereniging_In_CorresponderendeVCodes()
        => fixture.Result.CorresponderendeVCodes.Should().Contain(fixture.Scenario.DubbeleVerenigingWerdGeregistreerd.VCode);
}
