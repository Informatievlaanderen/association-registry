namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Bankrekeningen.Vzer;

using Admin.Schema.Historiek;
using Events;
using Scenario.Bankrekeningnummers.Vzer;

[Collection(nameof(ProjectionContext))]
public class Given_BankrekeningWerdGewijzigd(
    BeheerHistoriekScenarioFixture<BankrekeningnummerWerdGewijzigdScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<BankrekeningnummerWerdGewijzigdScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(3);

    [Fact]
    public void Historiek_Saved_Bankrekenummer_Werd_Gewijzigd()
        => fixture.Result
                  .Gebeurtenissen.Last()
                  .Should().BeEquivalentTo(new BeheerVerenigingHistoriekGebeurtenis(
                                               Beschrijving: $"Bankrekeningnummer werd gewijzigd.",
                                               nameof(BankrekeningnummerWerdGewijzigd),
                                               fixture.Scenario.BankrekeningnummerWerdGewijzigd,
                                               fixture.MetadataInitiator,
                                               fixture.MetadataTijdstip));
}
