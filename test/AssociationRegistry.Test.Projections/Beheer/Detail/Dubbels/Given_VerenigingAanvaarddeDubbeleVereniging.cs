namespace AssociationRegistry.Test.Projections.Beheer.Detail.Dubbels;

using Admin.Schema.Constants;
using Scenario.Dubbels;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingWerdToegevoegdAlsDubbel(BeheerDetailScenarioFixture<VerenigingAanvaarddeDubbeleVerenigingScenario> fixture)
    : BeheerDetailScenarioClassFixture<VerenigingAanvaarddeDubbeleVerenigingScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(2);

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
