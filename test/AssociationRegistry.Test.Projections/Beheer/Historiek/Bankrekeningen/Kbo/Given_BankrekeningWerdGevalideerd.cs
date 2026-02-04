namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Bankrekeningen.Kbo;

using AssociationRegistry.Admin.Schema.Historiek;
using AssociationRegistry.Events;
using AssociationRegistry.Test.Projections.Scenario.Bankrekeningnummers.Vzer;
using Scenario.Bankrekeningnummers.Kbo;

[Collection(nameof(ProjectionContext))]
public class Given_BankrekeningWerdGevalideerd(
    BeheerHistoriekScenarioFixture<BankrekeningnummerWerdGevalideerdKBOScenario> fixture
) : BeheerHistoriekScenarioClassFixture<BankrekeningnummerWerdGevalideerdKBOScenario>
{
    [Fact]
    public void Metadata_Is_Updated() => fixture.Result.Metadata.Version.Should().Be(3);

    [Fact]
    public void Document_Is_Updated() =>
        fixture
            .Result.Gebeurtenissen.Last()
            .Should()
            .BeEquivalentTo(
                new BeheerVerenigingHistoriekGebeurtenis(
                    Beschrijving: $"Bankrekeningnummer met IBAN '{fixture.Scenario.BankrekeningnummerWerdToegevoegdVanuitKBO.Iban}' werd gevalideerd.",
                    nameof(BankrekeningnummerWerdGevalideerd),
                    fixture.Scenario.BankrekeningnummerWerdGevalideerd,
                    fixture.MetadataInitiator,
                    fixture.MetadataTijdstip
                )
            );
}
