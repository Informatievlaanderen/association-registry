namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Bankrekeningen;

using Admin.Schema.Historiek;
using Events;
using Scenario.Bankrekeningnummers;

[Collection(nameof(ProjectionContext))]
public class Given_BankrekeningWerdToegevoegd(
    BeheerHistoriekScenarioFixture<BankrekeningnummerWerdToegevoegdScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<BankrekeningnummerWerdToegevoegdScenario>
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
                                               Beschrijving: $"Bankrekeningnummer met IBAN '{fixture.Scenario.BankrekeningnummerWerdToegevoegd.IBAN}' werd toegevoegd.",
                                               nameof(BankrekeningnummerWerdToegevoegd),
                                               fixture.Scenario.BankrekeningnummerWerdToegevoegd,
                                               fixture.MetadataInitiator,
                                               fixture.MetadataTijdstip));
}
