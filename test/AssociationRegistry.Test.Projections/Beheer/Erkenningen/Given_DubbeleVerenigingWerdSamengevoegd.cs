namespace AssociationRegistry.Test.Projections.Beheer.Erkenningen;

using AssociationRegistry.Admin.Schema.Erkenningen;
using AssociationRegistry.Test.Projections.Scenario.Dubbels;

/// <summary>
///     Overgenomen erkenningen worden erkenningen van de authentieke vereniging met hun nieuwe id, zodat ze ook
///     automatisch geactiveerd en verlopen worden. Niet-overgenomen erkenningen komen er niet bij.
/// </summary>
[Collection(nameof(ProjectionContext))]
public class Given_DubbeleVerenigingWerdSamengevoegd(
    ErkenningenActivatieScenarioFixture<DubbeleVerenigingWerdSamengevoegdWithNewItemsScenario> fixture
) : ErkenningenActivatieScenarioClassFixture<DubbeleVerenigingWerdSamengevoegdWithNewItemsScenario>
{
    [Fact]
    public void Then_The_Overgenomen_Erkenningen_Are_Added_To_The_Authentieke_Vereniging()
    {
#pragma warning disable CS0618
        var overgenomen = fixture.Scenario.Samengevoegd.OvergenomenGegevens.Erkenningen.Select(x => x.NieuweWaarde);
#pragma warning restore CS0618

        fixture
            .Result.Should()
            .BeEquivalentTo(
                overgenomen.Select(erkenning => new ErkenningDocument
                {
                    Id = $"{fixture.Scenario.AggregateId}-{erkenning.ErkenningId}",
                    VCode = fixture.Scenario.AggregateId,
                    ErkenningId = erkenning.ErkenningId,
                    Startdatum = erkenning.Startdatum,
                    Einddatum = erkenning.Einddatum,
                    Status = erkenning.Status,
                })
            );
    }
}
