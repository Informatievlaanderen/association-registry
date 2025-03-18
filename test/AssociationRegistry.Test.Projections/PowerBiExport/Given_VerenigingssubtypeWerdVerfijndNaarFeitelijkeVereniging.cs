namespace AssociationRegistry.Test.Projections.PowerBiExport;

using Admin.Schema.Detail;
using Scenario.Subtypes;

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
            Code = Vereniging.Verenigingssubtype.FeitelijkeVereniging.Code,
            Naam = Vereniging.Verenigingssubtype.FeitelijkeVereniging.Naam
        });
    }

    [Fact]
    public void SubverenigingVan_Is_Cleared()
    {
        fixture.Result.SubverenigingVan.Should().BeNull();
    }
}
