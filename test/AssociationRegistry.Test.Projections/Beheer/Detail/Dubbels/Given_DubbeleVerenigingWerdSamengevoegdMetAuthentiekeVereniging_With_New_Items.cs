namespace AssociationRegistry.Test.Projections.Beheer.Detail.Dubbels;

using Admin.Schema.Detail;
using DecentraalBeheer.Vereniging.Mappers;
using Events;
using Formats;
using Scenario.Dubbels;
using static AssociationRegistry.Test.Common.DubbelMerge.TestBankrekeningnummers;
using static AssociationRegistry.Test.Common.DubbelMerge.TestVertegenwoordigers;
using static ExpectedBeheerDetail;

#pragma warning disable CS0618
[Collection(nameof(ProjectionContext))]
public class Given_DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVereniging_With_New_Items(
    BeheerDetailScenarioFixture<DubbeleVerenigingWerdSamengevoegdWithNewItemsScenario> fixture
) : BeheerDetailScenarioClassFixture<DubbeleVerenigingWerdSamengevoegdWithNewItemsScenario>
{
    private string VCode => fixture.Scenario.AggregateId;
    private VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd Registratie =>
        fixture.Scenario.AuthentiekeVerenigingWerdGeregistreerd;
    private SamenvoegingDubbeleVereniging.OvergenomenGegevensPerGroep<
        Registratiedata.Vertegenwoordiger,
        SamenvoegingDubbeleVereniging.Bankrekeningnummer
    > Overgenomen => fixture.Scenario.Samengevoegd.OvergenomenGegevens;
    private SamenvoegingDubbeleVereniging.NietOvergenomenGegevensPerGroep NietOvergenomen =>
        fixture.Scenario.Samengevoegd.NietOvergenomenGegevens;

    [Fact]
    public void Metadata_Is_Updated() => fixture.Result.Metadata.Version.Should().Be(2);

    [Fact]
    public void KorteNaam_Is_Overgenomen() =>
        fixture.Result.KorteNaam.Should().Be(Overgenomen.Basisgegevens.KorteNaam!.NieuweWaarde);

    [Fact]
    public void KorteBeschrijving_Is_Overgenomen() =>
        fixture.Result.KorteBeschrijving.Should().Be(Overgenomen.Basisgegevens.KorteBeschrijving!.NieuweWaarde);

    [Fact]
    public void Startdatum_Is_Overgenomen() =>
        fixture
            .Result.Startdatum.Should()
            .Be(Overgenomen.Basisgegevens.Startdatum!.NieuweWaarde.ToString(WellknownFormats.DateOnly));

    [Fact]
    public void Doelgroep_Is_Overgenomen() =>
        fixture
            .Result.Doelgroep.Should()
            .BeEquivalentTo(Doelgroep(Overgenomen.Basisgegevens.Doelgroep!.NieuweWaarde, VCode));

    [Fact]
    public void Hoofdactiviteit_Is_Added() =>
        fixture
            .Result.HoofdactiviteitenVerenigingsloket.Should()
            .BeEquivalentTo([
                Hoofdactiviteit(Registratie.HoofdactiviteitenVerenigingsloket.Single()),
                Hoofdactiviteit(Overgenomen.Hoofdactiviteiten.Single().NieuweWaarde),
            ]);

    [Fact]
    public void Werkingsgebied_Is_Added() =>
        fixture
            .Result.Werkingsgebieden.Should()
            .BeEquivalentTo([Werkingsgebied(Overgenomen.Werkingsgebieden.Single().NieuweWaarde)]);

    [Fact]
    public void Contactgegeven_Is_Added() =>
        fixture
            .Result.Contactgegevens.Should()
            .HaveCount(2)
            .And.BeInAscendingOrder(x => x.ContactgegevenId)
            .And.Contain(x => x.ContactgegevenId == Registratie.Contactgegevens.Single().ContactgegevenId)
            .And.ContainEquivalentOf(Contactgegeven(Overgenomen.Contactgegevens.Single().NieuweWaarde, VCode));

    [Fact]
    public void Locatie_Is_Added() =>
        fixture
            .Result.Locaties.Should()
            .HaveCount(2)
            .And.BeInAscendingOrder(x => x.LocatieId)
            .And.Contain(x => x.LocatieId == Registratie.Locaties.Single().LocatieId)
            .And.ContainEquivalentOf(Locatie(Overgenomen.Locaties.Single().NieuweWaarde, VCode));

    [Fact]
    public void Vertegenwoordiger_Is_Added_With_The_Nieuwe_Waarde() =>
        fixture
            .Result.Vertegenwoordigers.Should()
            .HaveCount(2)
            .And.BeInAscendingOrder(x => x.VertegenwoordigerId)
            .And.Contain(x => x.VertegenwoordigerId == Registratie.Vertegenwoordigers.Single().VertegenwoordigerId)
            .And.ContainEquivalentOf(Vertegenwoordiger(Overgenomen.Vertegenwoordigers.Single().NieuweWaarde, VCode));

    [Fact]
    public void Bankrekeningnummer_Is_Added_With_The_Nieuwe_Waarde() =>
        fixture
            .Result.Bankrekeningnummers.Should()
            .HaveCount(2)
            .And.BeInAscendingOrder(x => x.BankrekeningnummerId)
            .And.Contain(x => x.BankrekeningnummerId == Registratie.Bankrekeningnummers.Single().BankrekeningnummerId)
            .And.ContainEquivalentOf(Bankrekeningnummer(Overgenomen.Bankrekeningnummers.Single().NieuweWaarde, VCode));

    [Fact]
    public void Erkenning_Is_Added() =>
        fixture
            .Result.Erkenningen.Should()
            .BeEquivalentTo([Erkenning(Overgenomen.Erkenningen.Single().NieuweWaarde, VCode)]);

    [Fact]
    public void Lidmaatschap_Is_Added() =>
        fixture
            .Result.Lidmaatschappen.Should()
            .BeEquivalentTo([Lidmaatschap(Overgenomen.Lidmaatschappen.Single().NieuweWaarde, VCode)]);

    [Fact]
    public void SubverenigingVan_Makes_It_A_Subvereniging()
    {
        fixture
            .Result.Verenigingssubtype.Should()
            .BeEquivalentTo(DecentraalBeheer.Vereniging.VerenigingssubtypeCode.Subvereniging.Map<Verenigingssubtype>());
        fixture
            .Result.SubverenigingVan.Should()
            .BeEquivalentTo(SubverenigingVan(Overgenomen.SubverenigingVan!.NieuweWaarde));
    }

    [Fact]
    public void The_Originele_Waarden_Are_Not_Added()
    {
        fixture.Result.Vertegenwoordigers.Should().NotContain(x => x.Insz == Frodo.Insz);
        fixture.Result.Bankrekeningnummers.Should().NotContain(x => x.Iban == BankrekeningnummerFrodo.Iban);
    }

    [Fact]
    public void The_NietOvergenomen_Gegevens_Are_Not_Added()
    {
        fixture.Result.Vertegenwoordigers.Should().NotContain(x => x.Insz == Aragorn.Insz);
        fixture.Result.Bankrekeningnummers.Should().NotContain(x => x.Iban == BankrekeningnummerBilbo.Iban);
        fixture
            .Result.HoofdactiviteitenVerenigingsloket.Should()
            .NotContain(x => x.Code == NietOvergenomen.Hoofdactiviteiten.Single().Waarde.Code);
        fixture
            .Result.Werkingsgebieden.Should()
            .NotContain(x => x.Code == NietOvergenomen.Werkingsgebieden.Single().Waarde.Code);
        fixture
            .Result.Contactgegevens.Should()
            .NotContain(x => x.Waarde == NietOvergenomen.Contactgegevens.Single().Waarde.Waarde);
        fixture.Result.Locaties.Should().NotContain(x => x.Naam == NietOvergenomen.Locaties.Single().Waarde.Naam);
        fixture
            .Result.Erkenningen.Should()
            .NotContain(x => x.ErkenningId == NietOvergenomen.Erkenningen.Single().Waarde.ErkenningId);
        fixture
            .Result.Lidmaatschappen.Should()
            .NotContain(x => x.LidmaatschapId == NietOvergenomen.Lidmaatschappen.Single().Waarde.LidmaatschapId);
    }
}
#pragma warning restore CS0618
