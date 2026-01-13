namespace AssociationRegistry.Test.Projections.Beheer.Detail.Bankrekeningnummers.Kbo;

using Admin.ProjectionHost.Projections.Detail;
using Admin.Schema.Detail;
using Contracts.JsonLdContext;
using Scenario.Bankrekeningnummers.Kbo;

[Collection(nameof(ProjectionContext))]
public class Given_BankrekeningnummerWerdToegevoegdVanuitKbo(
    BeheerDetailScenarioFixture<BankrekeningnummerWerdToegevoegdVanuitKBOScenario> fixture)
    : BeheerDetailScenarioClassFixture<BankrekeningnummerWerdToegevoegdVanuitKBOScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(2);

    [Fact]
    public void Bankrekeningnummer_Is_Toegevoegd()
    {
        fixture.Result.Bankrekeningnummers.Should().BeEquivalentTo([
            new Bankrekeningnummer()
            {
                JsonLdMetadata = BeheerVerenigingDetailMapper.CreateJsonLdMetadata(
                    JsonLdType.Bankrekeningnummer, fixture.Scenario.AggregateId,
                    fixture.Scenario.BankrekeningnummerWerdToegevoegdVanuitKBO.BankrekeningnummerId.ToString()),

                BankrekeningnummerId = fixture.Scenario.BankrekeningnummerWerdToegevoegdVanuitKBO.BankrekeningnummerId,
                Iban = fixture.Scenario.BankrekeningnummerWerdToegevoegdVanuitKBO.Iban,
                GebruiktVoor = string.Empty,
                Titularis = string.Empty,
            },
        ]);
    }
}
