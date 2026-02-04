namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Bankrekeningen.Vzer;

using AssociationRegistry.Admin.Schema.Historiek;
using AssociationRegistry.Events;
using AssociationRegistry.Test.Projections.Scenario.Bankrekeningnummers.Vzer;

[Collection(nameof(ProjectionContext))]
public class Given_BankrekeningnummerValidatieWerdOngedaanGemaaktDoorWijzigingTitularis(
    BeheerHistoriekScenarioFixture<BankrekeningnummerValidatieWerdOngedaanGemaaktDoorWijzigingTitularisScenario> fixture
) : BeheerHistoriekScenarioClassFixture<BankrekeningnummerValidatieWerdOngedaanGemaaktDoorWijzigingTitularisScenario>
{
    [Fact]
    public void Metadata_Is_Updated() => fixture.Result.Metadata.Version.Should().Be(5);

    [Fact]
    public void Document_Is_Updated() =>
        fixture
            .Result.Gebeurtenissen.Last()
            .Should()
            .BeEquivalentTo(
                new BeheerVerenigingHistoriekGebeurtenis(
                    Beschrijving: $"Bankrekeningnummer validatie werd ongedaan gemaakt door wijziging titularis.",
                    nameof(BankrekeningnummerValidatieWerdOngedaanGemaaktDoorWijzigingTitularis),
                    fixture.Scenario.BankrekeningnummerValidatieWerdOngedaanGemaaktDoorWijzigingTitularis,
                    fixture.MetadataInitiator,
                    fixture.MetadataTijdstip
                )
            );
}
