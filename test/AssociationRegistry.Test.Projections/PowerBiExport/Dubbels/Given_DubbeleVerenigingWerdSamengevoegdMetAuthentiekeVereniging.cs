namespace AssociationRegistry.Test.Projections.PowerBiExport.Dubbels;

using Admin.Schema.PowerBiExport;
using Events;
using Formats;
using Scenario.Dubbels;
using Vereniging.Bronnen;

/// <summary>Van vertegenwoordigers wordt enkel het aantal bijgehouden; bankrekeningnummers zonder persoonsgegevens.</summary>
#pragma warning disable CS0618
[Collection(nameof(ProjectionContext))]
public class Given_DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVereniging(
    PowerBiScenarioFixture<DubbeleVerenigingWerdSamengevoegdWithNewItemsScenario> fixture
) : PowerBiScenarioClassFixture<DubbeleVerenigingWerdSamengevoegdWithNewItemsScenario>
{
    private SamenvoegingDubbeleVereniging.OvergenomenGegevensPerGroep<
        Registratiedata.Vertegenwoordiger,
        SamenvoegingDubbeleVereniging.Bankrekeningnummer
    > Overgenomen => fixture.Scenario.Samengevoegd.OvergenomenGegevens;

    [Fact]
    public void Historiek_Contains_The_Event() =>
        fixture
            .Result.Historiek.Last()
            .EventType.Should()
            .Be(nameof(DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVereniging));

    [Fact]
    public void Basisgegevens_Are_Overgenomen()
    {
        var basisgegevens = Overgenomen.Basisgegevens;

        fixture.Result.KorteNaam.Should().Be(basisgegevens.KorteNaam!.NieuweWaarde);
        fixture.Result.KorteBeschrijving.Should().Be(basisgegevens.KorteBeschrijving!.NieuweWaarde);
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
            .BeEquivalentTo([
                new Werkingsgebied
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
                new
                {
                    contactgegeven.ContactgegevenId,
                    contactgegeven.Contactgegeventype,
                    contactgegeven.Waarde,
                    contactgegeven.Beschrijving,
                    contactgegeven.IsPrimair,
                    Bron = (string)Bron.Initiator,
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
                    Bron = (string)Bron.Initiator,
                }
            );
    }

    [Fact]
    public void Only_The_Added_Vertegenwoordiger_Is_Counted() =>
        fixture
            .Result.AantalVertegenwoordigers.Should()
            .Be(fixture.Scenario.AuthentiekeVerenigingWerdGeregistreerd.Vertegenwoordigers.Length + 1);

    [Fact]
    public void Bankrekeningnummer_Is_Added_Without_Persoonsgegevens()
    {
        var bankrekeningnummer = Overgenomen.Bankrekeningnummers.Single().NieuweWaarde;

        fixture
            .Result.Bankrekeningnummers.Should()
            .HaveCount(2)
            .And.ContainEquivalentOf(
                new Bankrekeningnummer(
                    bankrekeningnummer.BankrekeningnummerId,
                    bankrekeningnummer.Doel,
                    bankrekeningnummer
                        .BevestigdDoor.Select(x => new GegevensInitiator { OvoCode = x.OvoCode, Naam = x.Naam })
                        .ToArray(),
                    Bron.Initiator
                )
            );
    }

    [Fact]
    public void Erkenning_Is_Added()
    {
        var erkenning = Overgenomen.Erkenningen.Single().NieuweWaarde;

        fixture
            .Result.Erkenningen.Should()
            .BeEquivalentTo([
                new Erkenning
                {
                    ErkenningId = erkenning.ErkenningId,
                    GeregistreerdDoor = new GegevensInitiator
                    {
                        OvoCode = erkenning.GeregistreerdDoor.OvoCode,
                        Naam = erkenning.GeregistreerdDoor.Naam,
                    },
                    IpdcProduct = new IpdcProduct
                    {
                        Nummer = erkenning.IpdcProduct.Nummer,
                        Naam = erkenning.IpdcProduct.Naam,
                    },
                    Startdatum = erkenning.Startdatum.FormatAsBelgianDate(),
                    Einddatum = erkenning.Einddatum.FormatAsBelgianDate(),
                    Hernieuwingsdatum = erkenning.Hernieuwingsdatum.FormatAsBelgianDate(),
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
                new Lidmaatschap(
                    lidmaatschap.LidmaatschapId,
                    lidmaatschap.AndereVereniging,
                    lidmaatschap.DatumVan.FormatAsBelgianDate(),
                    lidmaatschap.DatumTot.FormatAsBelgianDate(),
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
