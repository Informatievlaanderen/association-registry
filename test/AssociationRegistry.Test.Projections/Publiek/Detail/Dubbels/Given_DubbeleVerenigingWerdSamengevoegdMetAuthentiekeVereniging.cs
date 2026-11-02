namespace AssociationRegistry.Test.Projections.Publiek.Detail.Dubbels;

using Contracts.JsonLdContext;
using Events;
using Formats;
using Public.Schema.Detail;
using Scenario.Dubbels;

/// <summary>Vertegenwoordigers en bankrekeningnummers staan niet in het publieke detail.</summary>
#pragma warning disable CS0618
[Collection(nameof(ProjectionContext))]
public class Given_DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVereniging(
    PubliekDetailScenarioFixture<DubbeleVerenigingWerdSamengevoegdWithNewItemsScenario> fixture
) : PubliekDetailScenarioClassFixture<DubbeleVerenigingWerdSamengevoegdWithNewItemsScenario>
{
    private string VCode => fixture.Scenario.AggregateId;
    private SamenvoegingDubbeleVereniging.OvergenomenGegevensPerGroep<
        Registratiedata.Vertegenwoordiger,
        Registratiedata.Bankrekeningnummer
    > Overgenomen => fixture.Scenario.Samengevoegd.OvergenomenGegevens;

    [Fact]
    public void Basisgegevens_Are_Overgenomen()
    {
        var basisgegevens = Overgenomen.Basisgegevens;

        fixture.Result.KorteNaam.Should().Be(basisgegevens.KorteNaam!.NieuweWaarde);
        fixture.Result.KorteBeschrijving.Should().Be(basisgegevens.KorteBeschrijving!.NieuweWaarde);
        fixture.Result.Startdatum.Should().Be(basisgegevens.Startdatum!.NieuweWaarde);
        fixture
            .Result.Doelgroep.Should()
            .BeEquivalentTo(
                new Doelgroep
                {
                    JsonLdMetadata = new JsonLdMetadata(
                        JsonLdType.Doelgroep.CreateWithIdValues(VCode),
                        JsonLdType.Doelgroep.Type
                    ),
                    Minimumleeftijd = basisgegevens.Doelgroep!.NieuweWaarde.Minimumleeftijd,
                    Maximumleeftijd = basisgegevens.Doelgroep.NieuweWaarde.Maximumleeftijd,
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
                    Code = Overgenomen.Hoofdactiviteiten.Single().NieuweWaarde.Code,
                    Naam = Overgenomen.Hoofdactiviteiten.Single().NieuweWaarde.Naam,
                }
            );

    [Fact]
    public void Werkingsgebied_Is_Added() =>
        fixture
            .Result.Werkingsgebieden.Should()
            .BeEquivalentTo([
                new
                {
                    Code = Overgenomen.Werkingsgebieden.Single().NieuweWaarde.Code,
                    Naam = Overgenomen.Werkingsgebieden.Single().NieuweWaarde.Naam,
                },
            ]);

    [Fact]
    public void Contactgegeven_Is_Added()
    {
        var contactgegeven = Overgenomen.Contactgegevens.Single().NieuweWaarde;

        fixture
            .Result.Contactgegevens.Should()
            .HaveCount(2)
            .And.BeInAscendingOrder(x => x.ContactgegevenId)
            .And.ContainEquivalentOf(
                new PubliekVerenigingDetailDocument.Types.Contactgegeven
                {
                    JsonLdMetadata = new JsonLdMetadata(
                        JsonLdType.Contactgegeven.CreateWithIdValues(VCode, contactgegeven.ContactgegevenId.ToString()),
                        JsonLdType.Contactgegeven.Type
                    ),
                    ContactgegevenId = contactgegeven.ContactgegevenId,
                    Contactgegeventype = contactgegeven.Contactgegeventype,
                    Waarde = contactgegeven.Waarde,
                    Beschrijving = contactgegeven.Beschrijving,
                    IsPrimair = contactgegeven.IsPrimair,
                }
            );
    }

    [Fact]
    public void Locatie_Is_Added()
    {
        var locatie = Overgenomen.Locaties.Single().NieuweWaarde;

        fixture
            .Result.Locaties.Should()
            .HaveCount(2)
            .And.BeInAscendingOrder(x => x.LocatieId)
            .And.ContainEquivalentOf(
                new
                {
                    locatie.LocatieId,
                    locatie.Locatietype,
                    locatie.IsPrimair,
                    locatie.Naam,
                    Adresvoorstelling = locatie.Adres.ToAdresString(),
                }
            );
    }

    [Fact]
    public void Erkenning_Is_Added()
    {
        var erkenning = Overgenomen.Erkenningen.Single().NieuweWaarde;

        fixture
            .Result.Erkenningen.Should()
            .BeEquivalentTo([
                new PubliekVerenigingDetailDocument.Types.Erkenning
                {
                    JsonLdMetadata = new JsonLdMetadata(
                        JsonLdType.Erkenning.CreateWithIdValues(VCode, erkenning.ErkenningId.ToString()),
                        JsonLdType.Erkenning.Type
                    ),
                    ErkenningId = erkenning.ErkenningId,
                    GeregistreerdDoor = new PubliekVerenigingDetailDocument.Types.GegevensInitiator
                    {
                        OvoCode = erkenning.GeregistreerdDoor.OvoCode,
                        Naam = erkenning.GeregistreerdDoor.Naam,
                    },
                    IpdcProduct = new PubliekVerenigingDetailDocument.Types.IpdcProduct
                    {
                        Nummer = erkenning.IpdcProduct.Nummer,
                        Naam = erkenning.IpdcProduct.Naam,
                    },
                    Startdatum = erkenning.Startdatum,
                    Einddatum = erkenning.Einddatum,
                    Hernieuwingsdatum = erkenning.Hernieuwingsdatum,
                    HernieuwingsUrl = erkenning.HernieuwingsUrl,
                    RedenSchorsing = erkenning.RedenSchorsing,
                    Status = erkenning.Status,
                },
            ]);
    }

    [Fact]
    public void Lidmaatschap_Is_Added()
    {
        var lidmaatschap = Overgenomen.Lidmaatschappen.Single().NieuweWaarde;

        fixture
            .Result.Lidmaatschappen.Should()
            .BeEquivalentTo([
                new PubliekVerenigingDetailDocument.Types.Lidmaatschap(
                    new JsonLdMetadata(
                        JsonLdType.Lidmaatschap.CreateWithIdValues(VCode, lidmaatschap.LidmaatschapId.ToString()),
                        JsonLdType.Lidmaatschap.Type
                    ),
                    lidmaatschap.LidmaatschapId,
                    lidmaatschap.AndereVereniging,
                    lidmaatschap.DatumVan,
                    lidmaatschap.DatumTot,
                    lidmaatschap.Identificatie,
                    lidmaatschap.Beschrijving
                ),
            ]);
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
                new PubliekVerenigingDetailDocument.Types.SubverenigingVan
                {
                    AndereVereniging = subverenigingVan.AndereVereniging,
                    AndereVerenigingNaam = subverenigingVan.AndereVerenigingNaam,
                    Identificatie = subverenigingVan.Identificatie,
                    Beschrijving = subverenigingVan.Beschrijving,
                }
            );
    }
}
#pragma warning restore CS0618
