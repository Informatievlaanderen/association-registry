namespace AssociationRegistry.Test.Projections.Beheer.Historiek.VertegenwoordigerPersoonsgegevens.When_VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd.Samenvoeging;

using AssociationRegistry.Admin.Schema.Historiek;
using AssociationRegistry.Admin.Schema.Historiek.EventData;
using AssociationRegistry.Events;
using Scenario.VertegenwoordigerPersoonsgegevens.Samenvoeging;

#pragma warning disable CS0618
public static class SamenvoegingGebeurtenis
{
    public static DubbeleVerenigingWerdSamengevoegdData DataOf(BeheerVerenigingHistoriekDocument document) =>
        (DubbeleVerenigingWerdSamengevoegdData)
            document
                .Gebeurtenissen.Single(x =>
                    x.Gebeurtenis == nameof(DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVereniging)
                )
                .Data!;

    public static VertegenwoordigerData Origineel(DubbeleVerenigingWerdSamengevoegdData data) =>
        data.OvergenomenGegevens.Vertegenwoordigers.Single().OrigineleWaarde;

    public static VertegenwoordigerData Nieuw(DubbeleVerenigingWerdSamengevoegdData data) =>
        data.OvergenomenGegevens.Vertegenwoordigers.Single().NieuweWaarde;
}

[Collection(nameof(ProjectionContext))]
public class Given_The_Nieuwe_Waarde_Of_An_Overgenomen_Vertegenwoordiger(
    BeheerHistoriekScenarioFixture<VertegenwoordigerPersoonsgegevensVanDeNieuweWaardeWerdenGeanonimiseerdScenario> fixture
) : BeheerHistoriekScenarioClassFixture<VertegenwoordigerPersoonsgegevensVanDeNieuweWaardeWerdenGeanonimiseerdScenario>
{
    private DubbeleVerenigingWerdSamengevoegdData Expected =>
        DubbeleVerenigingWerdSamengevoegdData.Create(fixture.Scenario.Samengevoegd);

    private DubbeleVerenigingWerdSamengevoegdData Actual => SamenvoegingGebeurtenis.DataOf(fixture.Result);

    [Fact]
    public void Only_The_Nieuwe_Waarde_Is_Anonymized()
    {
        SamenvoegingGebeurtenis
            .Nieuw(Actual)
            .Should()
            .BeEquivalentTo(SamenvoegingGebeurtenis.Nieuw(Expected).MakeAnonymous());
        SamenvoegingGebeurtenis.Origineel(Actual).Should().BeEquivalentTo(SamenvoegingGebeurtenis.Origineel(Expected));
        Actual.NietOvergenomenGegevens.Should().BeEquivalentTo(Expected.NietOvergenomenGegevens);
    }

    [Fact]
    public void The_Rest_Of_The_Samenvoeging_Is_Unchanged() =>
        Actual
            .Should()
            .BeEquivalentTo(Expected.MakeAnonymous(fixture.Scenario.VertegenwoordigerIdDieGeanonimiseerdWerd));
}

[Collection(nameof(ProjectionContext))]
public class Given_The_Originele_Waarde_Of_An_Overgenomen_Vertegenwoordiger(
    BeheerHistoriekScenarioFixture<VertegenwoordigerPersoonsgegevensVanDeOrigineleWaardeWerdenGeanonimiseerdScenario> fixture
)
    : BeheerHistoriekScenarioClassFixture<VertegenwoordigerPersoonsgegevensVanDeOrigineleWaardeWerdenGeanonimiseerdScenario>
{
    private DubbeleVerenigingWerdSamengevoegdData Expected =>
        DubbeleVerenigingWerdSamengevoegdData.Create(fixture.Scenario.Samengevoegd);

    private DubbeleVerenigingWerdSamengevoegdData Actual => SamenvoegingGebeurtenis.DataOf(fixture.Result);

    [Fact]
    public void Only_The_Originele_Waarde_Is_Anonymized()
    {
        SamenvoegingGebeurtenis
            .Origineel(Actual)
            .Should()
            .BeEquivalentTo(SamenvoegingGebeurtenis.Origineel(Expected).MakeAnonymous());
        SamenvoegingGebeurtenis.Nieuw(Actual).Should().BeEquivalentTo(SamenvoegingGebeurtenis.Nieuw(Expected));
        Actual.NietOvergenomenGegevens.Should().BeEquivalentTo(Expected.NietOvergenomenGegevens);
    }

    [Fact]
    public void The_Rest_Of_The_Samenvoeging_Is_Unchanged() =>
        Actual
            .Should()
            .BeEquivalentTo(Expected.MakeAnonymous(fixture.Scenario.VertegenwoordigerIdDieGeanonimiseerdWerd));
}

[Collection(nameof(ProjectionContext))]
public class Given_A_Vertegenwoordiger_Outside_The_Samenvoeging(
    BeheerHistoriekScenarioFixture<VertegenwoordigerPersoonsgegevensBuitenDeSamenvoegingWerdenGeanonimiseerdScenario> fixture
)
    : BeheerHistoriekScenarioClassFixture<VertegenwoordigerPersoonsgegevensBuitenDeSamenvoegingWerdenGeanonimiseerdScenario>
{
    [Fact]
    public void The_Samenvoeging_Is_Unchanged() =>
        SamenvoegingGebeurtenis
            .DataOf(fixture.Result)
            .Should()
            .BeEquivalentTo(DubbeleVerenigingWerdSamengevoegdData.Create(fixture.Scenario.Samengevoegd));

    [Fact]
    public void The_Beschrijving_Is_Unchanged() =>
        fixture
            .Result.Gebeurtenissen.Single(x =>
                x.Gebeurtenis == nameof(DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVereniging)
            )
            .Beschrijving.Should()
            .Be(
                $"Gegevens van dubbele vereniging {fixture.Scenario.Samengevoegd.VCodeDubbeleVereniging} werden samengevoegd."
            );
}
#pragma warning restore CS0618
