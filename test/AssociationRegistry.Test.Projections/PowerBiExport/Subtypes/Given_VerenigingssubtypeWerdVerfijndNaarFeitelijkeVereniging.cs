namespace AssociationRegistry.Test.Projections.PowerBiExport.Subtypes;

using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Test.Projections.Scenario.Subtypes;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging(
    PowerBiScenarioFixture<SubverenigingWerdVerfijndNaarFeitelijkeVerenigingScenario> fixture)
    : PowerBiScenarioClassFixture<SubverenigingWerdVerfijndNaarFeitelijkeVerenigingScenario>
{
    [Fact]
    public void Verenigingssubtype_Is_FeitelijkeVereniging()
    {
        fixture.Result.Verenigingssubtype.Should().BeEquivalentTo(new Verenigingssubtype()
        {
            Code = VerenigingssubtypeCode.FeitelijkeVereniging.Code,
            Naam = VerenigingssubtypeCode.FeitelijkeVereniging.Naam
        });
    }

    [Fact]
    public void SubverenigingVan_Is_Cleared()
    {
        fixture.Result.SubverenigingVan.Should().BeNull();
    }
}
