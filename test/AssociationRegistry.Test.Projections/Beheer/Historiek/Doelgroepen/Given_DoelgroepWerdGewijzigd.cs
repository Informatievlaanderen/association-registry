namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Doelgroepen;

using Admin.Schema.Historiek;
using Events;
using Scenario.Doelgroepen;

[Collection(nameof(ProjectionContext))]
public class Given_DoelgroepWerdGewijzigdd(
    BeheerHistoriekScenarioFixture<DoelgroepWerdGewijzigdScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<DoelgroepWerdGewijzigdScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(2);

    [Fact]
    public void Document_Is_Updated()
        => fixture.Result
                  .Gebeurtenissen.Last()
                  .Should().BeEquivalentTo(new BeheerVerenigingHistoriekGebeurtenis(
                                               Beschrijving: $"Doelgroep werd gewijzigd naar '{fixture.Scenario.DoelgroepWerdGewijzigd.Doelgroep.Minimumleeftijd} - {fixture.Scenario.DoelgroepWerdGewijzigd.Doelgroep.Maximumleeftijd}'.",
                                               nameof(DoelgroepWerdGewijzigd),
                                               fixture.Scenario.DoelgroepWerdGewijzigd,
                                               fixture.MetadataInitiator,
                                               fixture.MetadataTijdstip));
}
