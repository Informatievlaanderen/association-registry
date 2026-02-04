namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Bankrekeningen.Kbo;

using Admin.Schema.Historiek;
using Events;
using Scenario.Bankrekeningnummers.Kbo;

[Collection(nameof(ProjectionContext))]
public class Given_BankrekeningnummerWerdGewijzigd(
    BeheerHistoriekScenarioFixture<BankrekeningnummerWerdGewijzigdKBOScenario> fixture
) : BeheerHistoriekScenarioClassFixture<BankrekeningnummerWerdGewijzigdKBOScenario>
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
                    Beschrijving: $"Bankrekeningnummer werd gewijzigd.",
                    nameof(BankrekeningnummerWerdGewijzigd),
                    fixture.Scenario.BankrekeningnummerWerdGewijzigd,
                    fixture.MetadataInitiator,
                    fixture.MetadataTijdstip
                )
            );
}
