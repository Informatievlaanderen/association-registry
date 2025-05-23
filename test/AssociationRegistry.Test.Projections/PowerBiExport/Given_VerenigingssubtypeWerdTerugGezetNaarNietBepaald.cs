﻿namespace AssociationRegistry.Test.Projections.PowerBiExport;

using Admin.Schema.Detail;
using Scenario.Subtypes;

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
            Code = Vereniging.VerenigingssubtypeCode.NietBepaald.Code,
            Naam = Vereniging.VerenigingssubtypeCode.NietBepaald.Naam
        });
    }

    [Fact]
    public void SubverenigingVan_Is_Cleared()
    {
        fixture.Result.SubverenigingVan.Should().BeNull();
    }
}
