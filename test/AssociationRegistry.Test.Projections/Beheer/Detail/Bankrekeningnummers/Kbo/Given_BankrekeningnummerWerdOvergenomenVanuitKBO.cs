namespace AssociationRegistry.Test.Projections.Beheer.Detail.Bankrekeningnummers.Kbo;

using Admin.ProjectionHost.Projections.Detail;
using Contracts.JsonLdContext;
using DecentraalBeheer.Vereniging.Bankrekeningen;
using Scenario.Bankrekeningnummers.Kbo;
using Vereniging.Bronnen;
using Bankrekeningnummer = Admin.Schema.Detail.Bankrekeningnummer;

[Collection(nameof(ProjectionContext))]
public class Given_BankrekeningnummerWerdOvergenomenVanuitKBO(
    BeheerDetailScenarioFixture<BankrekeningnummerWerdOvergenomenVanuitKBOScenario> fixture
) : BeheerDetailScenarioClassFixture<BankrekeningnummerWerdOvergenomenVanuitKBOScenario>
{
    [Fact]
    public void Metadata_Is_Updated() => fixture.Result.Metadata.Version.Should().Be(3);

    [Fact]
    public void Bankrekeningnummer_Is_Toegevoegd()
    {
        fixture
            .Result.Bankrekeningnummers.Should()
            .BeEquivalentTo([
                new Bankrekeningnummer()
                {
                    JsonLdMetadata = BeheerVerenigingDetailMapper.CreateJsonLdMetadata(
                        JsonLdType.Bankrekeningnummer,
                        fixture.Scenario.AggregateId,
                        fixture.Scenario.BankrekeningnummerWerdToegevoegd.BankrekeningnummerId.ToString()
                    ),

                    BankrekeningnummerId = fixture.Scenario.BankrekeningnummerWerdToegevoegd.BankrekeningnummerId,
                    Iban = fixture.Scenario.BankrekeningnummerWerdToegevoegd.Iban,
                    Doel = fixture.Scenario.BankrekeningnummerWerdToegevoegd.Doel,
                    Titularis = fixture.Scenario.BankrekeningnummerWerdToegevoegd.Titularis,
                    BevestigdDoor = [],
                    Bron = Bron.KBO,
                },
            ]);
    }
}
