namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Bankrekeningen.Kbo;

using Admin.Schema.Historiek;
using Events;
using Scenario.Bankrekeningnummers.Kbo;

[Collection(nameof(ProjectionContext))]
public class Given_BankrekeningWerdToegevoegdVanuitKBO(
    BeheerHistoriekScenarioFixture<BankrekeningnummerWerdToegevoegdVanuitKBOScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<BankrekeningnummerWerdToegevoegdVanuitKBOScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(2);

    [Fact]
    public void Historiek_Saved_Bankrekenummer_Werd_Toegevoegd()
        => fixture.Result
                  .Gebeurtenissen.Last()
                  .Should().BeEquivalentTo(new BeheerVerenigingHistoriekGebeurtenis(
                                               Beschrijving:
                                               $"Bankrekeningnummer met IBAN '{fixture.Scenario.BankrekeningnummerWerdToegevoegdVanuitKBO.Iban}' werd toegevoegd vanuit KBO.",
                                               nameof(BankrekeningnummerWerdToegevoegdVanuitKBO),
                                               fixture.Scenario.BankrekeningnummerWerdToegevoegdVanuitKBO,
                                               fixture.MetadataInitiator,
                                               fixture.MetadataTijdstip));
}
