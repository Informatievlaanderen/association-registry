namespace AssociationRegistry.Test.Projections.Beheer.Detail.Bankrekeningnummers;

using Admin.ProjectionHost.Projections.Detail;
using Admin.Schema.Detail;
using Contracts.JsonLdContext;
using Scenario.Bankrekeningnummers;

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
                    fixture.Scenario.BankrekeningnummerWerdToegevoegd.IBAN),
                IBAN = fixture.Scenario.BankrekeningnummerWerdToegevoegd.IBAN,
                GebruiktVoor = fixture.Scenario.BankrekeningnummerWerdToegevoegd.GebruiktVoor,
                Titularis = fixture.Scenario.BankrekeningnummerWerdToegevoegd.Titularis,
            },
        ]);
    }
}
