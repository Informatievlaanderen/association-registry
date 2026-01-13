namespace AssociationRegistry.Test.Projections.Beheer.Detail.Bankrekeningnummers;

using Admin.ProjectionHost.Projections.Detail;
using Admin.Schema.Detail;
using Contracts.JsonLdContext;
using Scenario.Bankrekeningnummers;
using Scenario.Bankrekeningnummers.Vzer;

[Collection(nameof(ProjectionContext))]
public class Given_BankrekeningnummerWerdToegevoegd(
    BeheerDetailScenarioFixture<BankrekeningnummerWerdToegevoegdScenario> fixture)
    : BeheerDetailScenarioClassFixture<BankrekeningnummerWerdToegevoegdScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(2);

    [Fact]
    public void Vertegenwoordiger_Is_Toegevoegd()
    {
        fixture.Result.Bankrekeningnummers.Should().BeEquivalentTo([
            new Bankrekeningnummer()
            {
                JsonLdMetadata = BeheerVerenigingDetailMapper.CreateJsonLdMetadata(
                    JsonLdType.Bankrekeningnummer, fixture.Scenario.AggregateId,
                    fixture.Scenario.BankrekeningnummerWerdToegevoegd.BankrekeningnummerId.ToString()),
                Iban = fixture.Scenario.BankrekeningnummerWerdToegevoegd.Iban,
                Doel = fixture.Scenario.BankrekeningnummerWerdToegevoegd.Doel,
                Titularis = fixture.Scenario.BankrekeningnummerWerdToegevoegd.Titularis,
            },
        ]);
    }
}
