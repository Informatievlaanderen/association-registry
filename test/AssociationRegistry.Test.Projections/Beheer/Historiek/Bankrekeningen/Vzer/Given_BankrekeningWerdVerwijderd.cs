namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Bankrekeningen.Vzer;

using AssociationRegistry.Admin.Schema.Historiek;
using AssociationRegistry.Events;
using AssociationRegistry.Test.Projections.Scenario.Bankrekeningnummers.Vzer;

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
    public void Document_Is_Updated()
        => fixture.Result
                  .Gebeurtenissen.Last()
                  .Should().BeEquivalentTo(new BeheerVerenigingHistoriekGebeurtenis(
                                               Beschrijving: $"Bankrekeningnummer met IBAN '{fixture.Scenario.BankrekeningnummerWerdVerwijderd.Iban}' werd verwijderd.",
                                               nameof(BankrekeningnummerWerdVerwijderd),
                                               fixture.Scenario.BankrekeningnummerWerdVerwijderd,
                                               fixture.MetadataInitiator,
                                               fixture.MetadataTijdstip));
}
