namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Bankrekeningen.Vzer;

using Admin.Schema.Historiek;
using Events;
using Scenario.Bankrekeningnummers.Vzer;

[Collection(nameof(ProjectionContext))]
public class Given_BankrekeningWerdVerwijderd(
    BeheerHistoriekScenarioFixture<BankrekeningnummerWerdVerwijderdScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<BankrekeningnummerWerdVerwijderdScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(3);

    [Fact]
    public void Historiek_Saved_Bankrekenummer_Werd_Verwijderd()
        => fixture.Result
                  .Gebeurtenissen.Last()
                  .Should().BeEquivalentTo(new BeheerVerenigingHistoriekGebeurtenis(
                                               Beschrijving:
                                               $"Bankrekeningnummer met IBAN '{fixture.Scenario.BankrekeningnummerWerdVerwijderd.Iban}' werd verwijderd.",
                                               nameof(BankrekeningnummerWerdVerwijderd),
                                               fixture.Scenario.BankrekeningnummerWerdVerwijderd,
                                               fixture.MetadataInitiator,
                                               fixture.MetadataTijdstip));
}
