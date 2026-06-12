namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Bankrekeningen.Kbo;

using Admin.Schema.Historiek;
using Events;
using Scenario.Bankrekeningnummers.Kbo;

[Collection(nameof(ProjectionContext))]
public class Given_BankrekeningnummerWerdOvergenomenVanuitKBO(
    BeheerHistoriekScenarioFixture<BankrekeningnummerWerdOvergenomenVanuitKBOScenario> fixture
) : BeheerHistoriekScenarioClassFixture<BankrekeningnummerWerdOvergenomenVanuitKBOScenario>
{
    [Fact]
    public void Metadata_Is_Updated() => fixture.Result.Metadata.Version.Should().Be(3);

    [Fact]
    public void Historiek_Saved_Bankrekenummer_Overgenomen_() =>
        fixture
           .Result.Gebeurtenissen.Last()
           .Should()
           .BeEquivalentTo(
                new BeheerVerenigingHistoriekGebeurtenis(
                    Beschrijving:
                    $"Bankrekeningnummer met IBAN '{fixture.Scenario.BankrekeningnummerWerdOvergenomenVanuitKBO.Iban}' werd overgenomen vanuit KBO.",
                    nameof(BankrekeningnummerWerdOvergenomenVanuitKBO),
                    fixture.Scenario.BankrekeningnummerWerdOvergenomenVanuitKBO,
                    fixture.MetadataInitiator,
                    fixture.MetadataTijdstip
                )
            );
}
