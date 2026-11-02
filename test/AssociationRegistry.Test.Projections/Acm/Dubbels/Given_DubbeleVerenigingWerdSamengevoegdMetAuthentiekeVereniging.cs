namespace AssociationRegistry.Test.Projections.Acm.Dubbels;

using DecentraalBeheer.Vereniging;
using Scenario.Dubbels;

/// <summary>De overgenomen vertegenwoordiger (Samwise) wordt vertegenwoordiger van de authentieke vereniging.</summary>
[Collection(nameof(ProjectionContext))]
public class Given_DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVereniging(
    VerenigingenPerInszScenarioFixture<DubbeleVerenigingWerdSamengevoegdForInszScenario> fixture
) : VerenigingenPerInszScenarioClassFixture<DubbeleVerenigingWerdSamengevoegdForInszScenario>
{
    [Fact]
    public void Then_The_Vertegenwoordiger_Represents_The_Authentieke_Vereniging()
    {
#pragma warning disable CS0618
        var vertegenwoordiger = fixture
            .Scenario.Samengevoegd.OvergenomenGegevens.Vertegenwoordigers.Single()
            .NieuweWaarde;
#pragma warning restore CS0618
        var registratie = fixture.Scenario.AuthentiekeVerenigingWerdGeregistreerd;

        fixture
            .Result.Verenigingen.Should()
            .ContainSingle()
            .Which.Should()
            .BeEquivalentTo(
                new
                {
                    vertegenwoordiger.VertegenwoordigerId,
                    registratie.VCode,
                    registratie.Naam,
                    IsHoofdvertegenwoordigerVan = true,
                    Verenigingssubtype = new
                    {
                        VerenigingssubtypeCode.Subvereniging.Code,
                        VerenigingssubtypeCode.Subvereniging.Naam,
                    },
                }
            );
    }
}
