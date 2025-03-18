namespace AssociationRegistry.Test.Projections.PowerBiExport;

using Admin.Schema.Detail;
using Scenario.Verenigingssubtypes;

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
            Code = Vereniging.Verenigingssubtype.Subvereniging.Code,
            Naam = Vereniging.Verenigingssubtype.Subvereniging.Naam,
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
