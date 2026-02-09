namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Bankrekeningen.Kbo;

using AssociationRegistry.Admin.Schema.Historiek;
using AssociationRegistry.Events;
using AssociationRegistry.Test.Projections.Scenario.Bankrekeningnummers.Vzer;
using Scenario.Bankrekeningnummers.Kbo;

[Collection(nameof(ProjectionContext))]
public class Given_BankrekeningnummerWerdOvergenomenVanuitKBO(
    BeheerHistoriekScenarioFixture<BankrekeningnummerWerdOvergenomenVanuitKBOScenario> fixture
) : BeheerHistoriekScenarioClassFixture<BankrekeningnummerWerdOvergenomenVanuitKBOScenario>
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
                    Beschrijving: $"Bankrekeningnummer met IBAN '{fixture.Scenario.BankrekeningnummerWerdOvergenomenVanuitKBO.Iban}' werd overgenomen vanuit KBO.",
                    nameof(BankrekeningnummerWerdOvergenomenVanuitKBO),
                    fixture.Scenario.BankrekeningnummerWerdOvergenomenVanuitKBO,
                    fixture.MetadataInitiator,
                    fixture.MetadataTijdstip
                )
            );
}
