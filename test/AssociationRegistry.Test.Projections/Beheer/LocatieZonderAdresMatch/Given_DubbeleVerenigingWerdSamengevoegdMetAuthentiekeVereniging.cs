namespace AssociationRegistry.Test.Projections.Beheer.LocatieZonderAdresMatch;

using Scenario.Dubbels;

/// <summary>Overgenomen locaties worden net als toegevoegde locaties nog gematcht met het adressenregister.</summary>
[Collection(nameof(ProjectionContext))]
public class Given_DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVereniging(
    LocatiesZonderAdresMatchScenarioFixture<DubbeleVerenigingWerdSamengevoegdWithNewItemsScenario> fixture
) : LocatiesZonderAdresMatchScenarioClassFixture<DubbeleVerenigingWerdSamengevoegdWithNewItemsScenario>
{
    [Fact]
    public void Then_The_Overgenomen_Locatie_Should_Be_Matched()
    {
#pragma warning disable CS0618
        var overgenomenLocatieIds = fixture.Scenario.Samengevoegd.OvergenomenGegevens.Locaties.Select(x =>
            x.NieuweWaarde.LocatieId
        );
#pragma warning restore CS0618

        fixture
            .Result.Single()
            .LocatieIds.Should()
            .BeEquivalentTo(
                fixture
                    .Scenario.AuthentiekeVerenigingWerdGeregistreerd.Locaties.Select(x => x.LocatieId)
                    .Concat(overgenomenLocatieIds)
            );
    }
}
