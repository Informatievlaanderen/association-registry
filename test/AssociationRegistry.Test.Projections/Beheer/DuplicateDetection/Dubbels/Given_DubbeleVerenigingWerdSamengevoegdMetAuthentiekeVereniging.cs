namespace AssociationRegistry.Test.Projections.Beheer.DuplicateDetection.Dubbels;

using Events;
using Formats;
using Scenario.Dubbels;

/// <summary>Enkel de gegevens die meetellen voor de detectie van dubbels staan in dit document.</summary>
#pragma warning disable CS0618
[Collection(nameof(ProjectionContext))]
public class Given_DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVereniging(
    DuplicateDetectionScenarioFixture<DubbeleVerenigingWerdSamengevoegdWithNewItemsScenario> fixture
) : DuplicateDetectionClassFixture<DubbeleVerenigingWerdSamengevoegdWithNewItemsScenario>
{
    private SamenvoegingDubbeleVereniging.OvergenomenGegevensPerGroep<
        Registratiedata.Vertegenwoordiger,
        SamenvoegingDubbeleVereniging.Bankrekeningnummer
    > Overgenomen => fixture.Scenario.Samengevoegd.OvergenomenGegevens;

    [Fact]
    public void KorteNaam_Is_Overgenomen() =>
        fixture.Result.KorteNaam.Should().Be(Overgenomen.Basisgegevens.KorteNaam!.NieuweWaarde);

    [Fact]
    public void Hoofdactiviteit_Is_Added() =>
        fixture
            .Result.HoofdactiviteitVerenigingsloket.Should()
            .BeEquivalentTo([
                fixture.Scenario.AuthentiekeVerenigingWerdGeregistreerd.HoofdactiviteitenVerenigingsloket.Single().Code,
                Overgenomen.Hoofdactiviteiten.Single().NieuweWaarde.Code,
            ]);

    [Fact]
    public void Locatie_Is_Added()
    {
        var locatie = Overgenomen.Locaties.Single().NieuweWaarde;

        fixture
            .Result.Locaties.Should()
            .HaveCount(2)
            .And.ContainEquivalentOf(
                new
                {
                    locatie.LocatieId,
                    locatie.Locatietype,
                    locatie.IsPrimair,
                    locatie.Naam,
                    Adresvoorstelling = locatie.Adres.ToAdresString(),
                    locatie.Adres!.Postcode,
                    locatie.Adres.Gemeente,
                }
            );
    }

    [Fact]
    public void SubverenigingVan_Makes_It_A_Subvereniging() =>
        fixture
            .Result.VerenigingssubtypeCode.Should()
            .Be(DecentraalBeheer.Vereniging.VerenigingssubtypeCode.Subvereniging.Code);
}
#pragma warning restore CS0618
