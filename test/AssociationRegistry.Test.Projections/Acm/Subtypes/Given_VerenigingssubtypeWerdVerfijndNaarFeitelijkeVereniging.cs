﻿namespace AssociationRegistry.Test.Projections.Acm.Subtypes;

using AssociationRegistry.Acm.Schema.VerenigingenPerInsz;
using AssociationRegistry.Test.Projections.Scenario.Verenigingssubtypes;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging(
    VerenigingenPerInszScenarioFixture<VerenigingssubtypeWerdVerfijndNaarFeitelijkeVerenigingScenario> fixture)
    : VerenigingenPerInszScenarioClassFixture<VerenigingssubtypeWerdVerfijndNaarFeitelijkeVerenigingScenario>
{
    [Fact]
    public void Then_Verenigingssubtype_Is_NogNietBepaald()
    {
        var vzer =
            fixture.Result.Verenigingen.Single(x => x.VCode == fixture.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode);

        vzer.Verenigingssubtype.Should().BeEquivalentTo(new Verenigingstype(AssociationRegistry.Vereniging.Verenigingssubtype.FeitelijkeVereniging.Code, AssociationRegistry.Vereniging.Verenigingssubtype.FeitelijkeVereniging.Naam));
    }
}
