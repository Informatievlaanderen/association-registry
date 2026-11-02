namespace AssociationRegistry.Test.Projections.Beheer.Detail.Dubbels;

using Admin.Schema.Detail;
using DecentraalBeheer.Vereniging.Mappers;
using Events;
using Scenario.Dubbels;
using static ExpectedBeheerDetail;

/// <summary>
///     Elk overgenomen item heeft hetzelfde id (of dezelfde code) als een bestaand item op de authentieke vereniging.
///     Er mag dus niets bijkomen: het bestaande item wordt vervangen door de nieuwe waarde.
/// </summary>
#pragma warning disable CS0618
[Collection(nameof(ProjectionContext))]
public class Given_DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVereniging_With_Existing_Items(
    BeheerDetailScenarioFixture<DubbeleVerenigingWerdSamengevoegdWithExistingItemsScenario> fixture
) : BeheerDetailScenarioClassFixture<DubbeleVerenigingWerdSamengevoegdWithExistingItemsScenario>
{
    private string VCode => fixture.Scenario.AggregateId;
    private SamenvoegingDubbeleVereniging.OvergenomenGegevensPerGroep<
        Registratiedata.Vertegenwoordiger,
        SamenvoegingDubbeleVereniging.Bankrekeningnummer
    > Overgenomen => fixture.Scenario.Samengevoegd.OvergenomenGegevens;

    [Fact]
    public void Metadata_Is_Updated() => fixture.Result.Metadata.Version.Should().Be(6);

    [Fact]
    public void Hoofdactiviteit_Is_Replaced() =>
        fixture
            .Result.HoofdactiviteitenVerenigingsloket.Should()
            .BeEquivalentTo([Hoofdactiviteit(Overgenomen.Hoofdactiviteiten.Single().NieuweWaarde)]);

    [Fact]
    public void Werkingsgebied_Is_Replaced() =>
        fixture
            .Result.Werkingsgebieden.Should()
            .BeEquivalentTo([Werkingsgebied(Overgenomen.Werkingsgebieden.Single().NieuweWaarde)]);

    [Fact]
    public void Contactgegeven_Is_Replaced() =>
        fixture
            .Result.Contactgegevens.Should()
            .BeEquivalentTo([Contactgegeven(Overgenomen.Contactgegevens.Single().NieuweWaarde, VCode)]);

    [Fact]
    public void Locatie_Is_Replaced() =>
        fixture.Result.Locaties.Should().BeEquivalentTo([Locatie(Overgenomen.Locaties.Single().NieuweWaarde, VCode)]);

    [Fact]
    public void Vertegenwoordiger_Is_Replaced() =>
        fixture
            .Result.Vertegenwoordigers.Should()
            .BeEquivalentTo([Vertegenwoordiger(Overgenomen.Vertegenwoordigers.Single().NieuweWaarde, VCode)]);

    [Fact]
    public void Bankrekeningnummer_Is_Replaced() =>
        fixture
            .Result.Bankrekeningnummers.Should()
            .BeEquivalentTo([Bankrekeningnummer(Overgenomen.Bankrekeningnummers.Single().NieuweWaarde, VCode)]);

    [Fact]
    public void Erkenning_Is_Replaced() =>
        fixture
            .Result.Erkenningen.Should()
            .BeEquivalentTo([Erkenning(Overgenomen.Erkenningen.Single().NieuweWaarde, VCode)]);

    [Fact]
    public void Lidmaatschap_Is_Replaced() =>
        fixture
            .Result.Lidmaatschappen.Should()
            .BeEquivalentTo([Lidmaatschap(Overgenomen.Lidmaatschappen.Single().NieuweWaarde, VCode)]);

    [Fact]
    public void SubverenigingVan_Is_Replaced()
    {
        fixture
            .Result.Verenigingssubtype.Should()
            .BeEquivalentTo(DecentraalBeheer.Vereniging.VerenigingssubtypeCode.Subvereniging.Map<Verenigingssubtype>());
        fixture
            .Result.SubverenigingVan.Should()
            .BeEquivalentTo(SubverenigingVan(Overgenomen.SubverenigingVan!.NieuweWaarde));
    }
}
#pragma warning restore CS0618
