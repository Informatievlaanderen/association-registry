namespace AssociationRegistry.Test.Projections.Beheer.Zoeken.Dubbels;

using Events;
using Formats;
using Scenario.Dubbels;

/// <summary>
///     Korte beschrijving, contactgegevens, vertegenwoordigers, bankrekeningnummers en erkenningen staan niet in het
///     beheer zoekdocument.
/// </summary>
#pragma warning disable CS0618
[Collection(nameof(ProjectionContext))]
public class Given_DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVereniging(
    BeheerZoekenScenarioFixture<DubbeleVerenigingWerdSamengevoegdWithNewItemsScenario> fixture
) : BeheerZoekenScenarioClassFixture<DubbeleVerenigingWerdSamengevoegdWithNewItemsScenario>
{
    private SamenvoegingDubbeleVereniging.OvergenomenGegevensPerGroep<
        Registratiedata.Vertegenwoordiger,
        SamenvoegingDubbeleVereniging.Bankrekeningnummer
    > Overgenomen => fixture.Scenario.Samengevoegd.OvergenomenGegevens;

    [Fact]
    public void Basisgegevens_Are_Overgenomen()
    {
        var basisgegevens = Overgenomen.Basisgegevens;

        fixture.Result.KorteNaam.Should().Be(basisgegevens.KorteNaam!.NieuweWaarde);
        fixture
            .Result.Startdatum.Should()
            .Be(basisgegevens.Startdatum!.NieuweWaarde.ToString(WellknownFormats.DateOnly));
        fixture
            .Result.Doelgroep.Should()
            .BeEquivalentTo(
                new
                {
                    basisgegevens.Doelgroep!.NieuweWaarde.Minimumleeftijd,
                    basisgegevens.Doelgroep.NieuweWaarde.Maximumleeftijd,
                }
            );
    }

    [Fact]
    public void Hoofdactiviteit_Is_Added() =>
        fixture
            .Result.HoofdactiviteitenVerenigingsloket.Should()
            .HaveCount(2)
            .And.ContainEquivalentOf(
                new
                {
                    Overgenomen.Hoofdactiviteiten.Single().NieuweWaarde.Code,
                    Overgenomen.Hoofdactiviteiten.Single().NieuweWaarde.Naam,
                }
            );

    [Fact]
    public void Werkingsgebied_Is_Added() =>
        fixture
            .Result.Werkingsgebieden.Should()
            .ContainSingle()
            .Which.Should()
            .BeEquivalentTo(
                new
                {
                    Overgenomen.Werkingsgebieden.Single().NieuweWaarde.Code,
                    Overgenomen.Werkingsgebieden.Single().NieuweWaarde.Naam,
                }
            );

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
    public void Lidmaatschap_Is_Added()
    {
        var lidmaatschap = Overgenomen.Lidmaatschappen.Single().NieuweWaarde;

        fixture
            .Result.Lidmaatschappen.Should()
            .ContainSingle()
            .Which.Should()
            .BeEquivalentTo(
                new
                {
                    lidmaatschap.LidmaatschapId,
                    lidmaatschap.AndereVereniging,
                    DatumVan = lidmaatschap.DatumVan.FormatAsBelgianDate(),
                    DatumTot = lidmaatschap.DatumTot.FormatAsBelgianDate(),
                    lidmaatschap.Identificatie,
                    lidmaatschap.Beschrijving,
                }
            );
    }

    [Fact]
    public void SubverenigingVan_Makes_It_A_Subvereniging()
    {
        var subverenigingVan = Overgenomen.SubverenigingVan!.NieuweWaarde;

        fixture
            .Result.Verenigingssubtype!.Code.Should()
            .Be(DecentraalBeheer.Vereniging.VerenigingssubtypeCode.Subvereniging.Code);
        fixture
            .Result.SubverenigingVan.Should()
            .BeEquivalentTo(
                new
                {
                    subverenigingVan.AndereVereniging,
                    subverenigingVan.Identificatie,
                    subverenigingVan.Beschrijving,
                }
            );
    }
}
#pragma warning restore CS0618
