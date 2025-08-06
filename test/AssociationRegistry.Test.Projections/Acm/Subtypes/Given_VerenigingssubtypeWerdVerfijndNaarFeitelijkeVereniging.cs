namespace AssociationRegistry.Test.Projections.Acm.Subtypes;

using DecentraalBeheer.Vereniging;
using Scenario.Verenigingssubtypes;
using Verenigingstype = AssociationRegistry.Acm.Schema.VerenigingenPerInsz.Verenigingstype;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging(
    VerenigingenPerInszScenarioFixture<VerenigingssubtypeWerdVerfijndNaarFeitelijkeVerenigingScenario> fixture)
    : VerenigingenPerInszScenarioClassFixture<VerenigingssubtypeWerdVerfijndNaarFeitelijkeVerenigingScenario>
{
    [Fact]
    public void Then_Verenigingssubtype_Is_NietBepaald()
    {
        var vzer =
            fixture.Result.Verenigingen.Single(x => x.VCode == fixture.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode);

        vzer.Verenigingssubtype.Should().BeEquivalentTo(new Verenigingstype(VerenigingssubtypeCode.FeitelijkeVereniging.Code, VerenigingssubtypeCode.FeitelijkeVereniging.Naam));
    }
}
