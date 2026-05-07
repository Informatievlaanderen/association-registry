namespace AssociationRegistry.Test.Projections.PowerBiExport.Subtypes;

using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Test.Projections.Scenario.Subtypes;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingssubtypeWerdTerugGezetNaarNietBepaald(
    PowerBiScenarioFixture<SubverenigingWerdTerugGezetNaarNietBepaaldScenario> fixture)
    : PowerBiScenarioClassFixture<SubverenigingWerdTerugGezetNaarNietBepaaldScenario>
{
    [Fact]
    public void Verenigingssubtype_Is_FeitelijkeVereniging()
    {
        fixture.Result.Verenigingssubtype.Should().BeEquivalentTo(new Verenigingssubtype()
        {
            Code = VerenigingssubtypeCode.NietBepaald.Code,
            Naam = VerenigingssubtypeCode.NietBepaald.Naam
        });
    }

    [Fact]
    public void SubverenigingVan_Is_Cleared()
    {
        fixture.Result.SubverenigingVan.Should().BeNull();
    }
}
