namespace AssociationRegistry.Test.Projections.Beheer.Detail.Bankrekeningnummers.Kbo;

using Admin.ProjectionHost.Projections.Detail;
using Admin.Schema.Detail;
using Contracts.JsonLdContext;
using DecentraalBeheer.Vereniging.Bankrekeningen;
using Scenario.Bankrekeningnummers.Kbo;
using Vereniging.Bronnen;
using Bankrekeningnummer = Admin.Schema.Detail.Bankrekeningnummer;

[Collection(nameof(ProjectionContext))]
public class Given_BankrekeningnummerWerdGevalideerd(
    BeheerDetailScenarioFixture<BankrekeningnummerWerdGevalideerdKBOScenario> fixture
) : BeheerDetailScenarioClassFixture<BankrekeningnummerWerdGevalideerdKBOScenario>
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
                        fixture.Scenario.BankrekeningnummerWerdToegevoegdVanuitKBO.BankrekeningnummerId.ToString()
                    ),

                    BankrekeningnummerId = fixture
                        .Scenario
                        .BankrekeningnummerWerdToegevoegdVanuitKBO
                        .BankrekeningnummerId,
                    Iban = fixture.Scenario.BankrekeningnummerWerdToegevoegdVanuitKBO.Iban,
                    Doel = string.Empty,
                    Titularissen = [],
                    BevestigdDoor =
                    [
                        new GegevensInitiator
                        {
                            OvoCode = fixture
                                .Scenario
                                .AanwezigheidBankrekeningnummerValidatieDocumentWerdBevestigd
                                .BevestigdDoor
                                .OvoCode,
                            Naam = fixture
                                .Scenario
                                .AanwezigheidBankrekeningnummerValidatieDocumentWerdBevestigd
                                .BevestigdDoor
                                .Naam,
                        },
                    ],
                    Bron = Bron.KBO,
                },
            ]);
    }
}
