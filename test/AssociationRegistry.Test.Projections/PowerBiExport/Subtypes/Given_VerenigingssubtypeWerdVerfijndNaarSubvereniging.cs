namespace AssociationRegistry.Test.Projections.PowerBiExport.Subtypes;

using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Test.Projections.Scenario.Verenigingssubtypes;
using SubverenigingVan = Admin.Schema.Detail.SubverenigingVan;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingssubtypeWerdVerfijndNaarSubvereniging(
    PowerBiScenarioFixture<VerenigingssubtypeWerdVerfijndNaarSubverenigingScenario> fixture)
    : PowerBiScenarioClassFixture<VerenigingssubtypeWerdVerfijndNaarSubverenigingScenario>
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
            AndereVereniging = fixture.Scenario.VerenigingssubtypeWerdVerfijndNaarSubvereniging.SubverenigingVan.AndereVereniging,
            Identificatie = fixture.Scenario.VerenigingssubtypeWerdVerfijndNaarSubvereniging.SubverenigingVan.Identificatie,
            Beschrijving = fixture.Scenario.VerenigingssubtypeWerdVerfijndNaarSubvereniging.SubverenigingVan.Beschrijving,
        });
    }
}
