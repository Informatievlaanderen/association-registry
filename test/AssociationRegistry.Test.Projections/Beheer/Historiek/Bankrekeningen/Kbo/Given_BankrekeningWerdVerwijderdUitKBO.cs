namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Bankrekeningen.Kbo;

using Admin.Schema.Historiek;
using Events;
using Scenario.Bankrekeningnummers.Kbo;

[Collection(nameof(ProjectionContext))]
public class Given_BankrekeningWerdVerwijderdUitKBO(
    BeheerHistoriekScenarioFixture<BankrekeningnummerWerdVerwijderdUitKBOScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<BankrekeningnummerWerdVerwijderdUitKBOScenario>
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
                                               Beschrijving: $"Bankrekeningnummer met IBAN '{fixture.Scenario.BankrekeningnummerWerdVerwijderdUitKBO.Iban}' werd verwijderd uit KBO.",
                                               nameof(BankrekeningnummerWerdVerwijderdUitKBO),
                                               fixture.Scenario.BankrekeningnummerWerdVerwijderdUitKBO,
                                               fixture.MetadataInitiator,
                                               fixture.MetadataTijdstip));
}
