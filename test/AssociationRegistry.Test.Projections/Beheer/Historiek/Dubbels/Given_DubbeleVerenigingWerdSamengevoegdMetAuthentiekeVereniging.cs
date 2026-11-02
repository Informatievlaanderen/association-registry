namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Dubbels;

using Admin.Schema.Historiek;
using Admin.Schema.Historiek.EventData;
using Events;
using Scenario.Dubbels;

#pragma warning disable CS0618
[Collection(nameof(ProjectionContext))]
public class Given_DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVereniging(
    BeheerHistoriekScenarioFixture<DubbeleVerenigingWerdSamengevoegdWithNewItemsScenario> fixture
) : BeheerHistoriekScenarioClassFixture<DubbeleVerenigingWerdSamengevoegdWithNewItemsScenario>
{
    [Fact]
    public void Metadata_Is_Updated() => fixture.Result.Metadata.Version.Should().Be(2);

    [Fact]
    public void Document_Is_Updated() =>
        fixture
            .Result.Gebeurtenissen.Last()
            .Should()
            .BeEquivalentTo(
                new BeheerVerenigingHistoriekGebeurtenis(
                    Beschrijving: $"Gegevens van dubbele vereniging {fixture.Scenario.Samengevoegd.VCodeDubbeleVereniging} werden samengevoegd.",
                    nameof(DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVereniging),
                    DubbeleVerenigingWerdSamengevoegdData.Create(fixture.Scenario.Samengevoegd),
                    fixture.MetadataInitiator,
                    fixture.MetadataTijdstip
                )
            );
}
#pragma warning restore CS0618
