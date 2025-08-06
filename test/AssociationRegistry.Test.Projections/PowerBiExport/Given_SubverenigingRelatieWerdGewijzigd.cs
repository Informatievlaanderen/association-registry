namespace AssociationRegistry.Test.Projections.PowerBiExport;

using Admin.Schema.Detail;
using DecentraalBeheer.Vereniging;
using Scenario.Verenigingssubtypes;
using Vereniging;
using SubverenigingVan = Admin.Schema.Detail.SubverenigingVan;

[Collection(nameof(ProjectionContext))]
public class Given_SubverenigingRelatieWerdGewijzigd(
    PowerBiScenarioFixture<SubverenigingWerdGewijzigdScenario> fixture)
    : PowerBiScenarioClassFixture<SubverenigingWerdGewijzigdScenario>
{
    [Fact]
    public void Verenigingssubtype_Is_FeitelijkeVereniging()
    {
        fixture.Result.Verenigingssubtype.Should().BeEquivalentTo(new Verenigingssubtype()
        {
            Code = VerenigingssubtypeCode.Subvereniging.Code,
            Naam = VerenigingssubtypeCode.Subvereniging.Naam,
        });
    }

    [Fact]
    public void SubverenigingVan_Is_Cleared()
    {
        fixture.Result.SubverenigingVan.Should().BeEquivalentTo(new SubverenigingVan()
        {
            AndereVereniging = fixture.Scenario.SubverenigingRelatieWerdGewijzigd.AndereVereniging,
            Identificatie = fixture.Scenario.SubverenigingDetailsWerdenGewijzigd.Identificatie,
            Beschrijving = fixture.Scenario.SubverenigingDetailsWerdenGewijzigd.Beschrijving,
        });
    }
}
