namespace AssociationRegistry.Test.Projections.Beheer.Detail.Bankrekeningnummers.Kbo;

using Admin.ProjectionHost.Projections.Detail;
using Contracts.JsonLdContext;
using DecentraalBeheer.Vereniging.Bankrekeningen;
using Scenario.Bankrekeningnummers.Kbo;
using Vereniging.Bronnen;
using Bankrekeningnummer = Admin.Schema.Detail.Bankrekeningnummer;

[Collection(nameof(ProjectionContext))]
public class Given_ValidatieBankrekeningnummerWerdOngedaanGemaakt(
    BeheerDetailScenarioFixture<ValidatieBankrekeningnummerWerdOngedaanGemaaktKBOScenario> fixture
) : BeheerDetailScenarioClassFixture<ValidatieBankrekeningnummerWerdOngedaanGemaaktKBOScenario>
{
    [Fact]
    public void Metadata_Is_Updated() => fixture.Result.Metadata.Version.Should().Be(4);

    [Fact]
    public void Then_Validatie_Bankrekeningnummer_Werd_Ongedaan_Gemaakt()
    {
        fixture
            .Result.Bankrekeningnummers.Should()
            .BeEquivalentTo([
                new Bankrekeningnummer()
                {
                    JsonLdMetadata = BeheerVerenigingDetailMapper.CreateJsonLdMetadata(
                        JsonLdType.Bankrekeningnummer,
                        fixture.Scenario.AggregateId,
                        fixture.Scenario.BankrekeningnummerWerdToegevoegdVanuitKBO.BankrekeningnummerId.ToString()
                    ),

                    BankrekeningnummerId = fixture
                        .Scenario
                        .BankrekeningnummerWerdToegevoegdVanuitKBO
                        .BankrekeningnummerId,
                    Iban = fixture.Scenario.BankrekeningnummerWerdToegevoegdVanuitKBO.Iban,
                    Doel = string.Empty,
                    Titularis = string.Empty,
                    BevestigdDoor = [],
                    Bron = Bron.KBO,
                },
            ]);
    }
}
