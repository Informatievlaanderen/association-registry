namespace AssociationRegistry.Test.Projections.Beheer.Detail.Bankrekeningnummers.Vzer;

using AssociationRegistry.Admin.ProjectionHost.Projections.Detail;
using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.Contracts.JsonLdContext;
using AssociationRegistry.Test.Projections.Scenario.Bankrekeningnummers.Vzer;

[Collection(nameof(ProjectionContext))]
public class Given_BankrekeningnummerWerdGevalideerd(
    BeheerDetailScenarioFixture<BankrekeningnummerWerdGevalideerdScenario> fixture)
    : BeheerDetailScenarioClassFixture<BankrekeningnummerWerdGevalideerdScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(3);

    [Fact]
    public void Vertegenwoordiger_Is_Toegevoegd()
    {
        fixture.Result.Bankrekeningnummers.Should().BeEquivalentTo([
            new Bankrekeningnummer()
            {
                JsonLdMetadata = BeheerVerenigingDetailMapper.CreateJsonLdMetadata(
                    JsonLdType.Bankrekeningnummer, fixture.Scenario.AggregateId,
                    fixture.Scenario.BankrekeningnummerWerdToegevoegd.BankrekeningnummerId.ToString()),
                BankrekeningnummerId = fixture.Scenario.BankrekeningnummerWerdToegevoegd.BankrekeningnummerId,
                Iban = fixture.Scenario.BankrekeningnummerWerdToegevoegd.Iban,
                Doel = fixture.Scenario.BankrekeningnummerWerdToegevoegd.Doel,
                Titularis = fixture.Scenario.BankrekeningnummerWerdToegevoegd.Titularis,
                IsGevalideerd = true,
            },
        ]);
    }
}
