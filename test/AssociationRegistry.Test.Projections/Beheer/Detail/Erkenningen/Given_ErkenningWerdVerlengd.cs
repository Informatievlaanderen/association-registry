namespace AssociationRegistry.Test.Projections.Beheer.Detail.Erkenningen;

using Admin.ProjectionHost.Constants;
using Admin.ProjectionHost.Projections.Detail;
using Contracts.JsonLdContext;
using DecentraalBeheer.Vereniging.Erkenningen;
using Scenario.Erkenningen;
using Erkenning = Admin.Schema.Detail.Erkenning;
using GegevensInitiator = Admin.Schema.Detail.GegevensInitiator;
using IpdcProduct = Admin.Schema.Detail.IpdcProduct;

[Collection(nameof(ProjectionContext))]
public class Given_ErkenningWerdVerlengd(BeheerDetailScenarioFixture<ErkenningWerdVerlengdScenario> fixture)
    : BeheerDetailScenarioClassFixture<ErkenningWerdVerlengdScenario>
{
    [Fact]
    public void Metadata_Is_Updated() => fixture.Result.Metadata.Version.Should().Be(3);

    [Fact]
    public void Then_Einddatum_And_Hernieuwingsdatum_Is_Updated()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        var erkenningsPeriode = ErkenningsPeriode.Create(
            fixture.Scenario.ErkenningWerdGeregistreerd.Startdatum,
            fixture.Scenario.ErkenningWerdVerlengd.Einddatum
        );

        var status = ErkenningStatus.Bepaal(
            erkenningsPeriode,
            today
        );
        fixture
            .Result.Erkenningen.Should()
            .BeEquivalentTo([
                new Erkenning
                {
                    JsonLdMetadata = BeheerVerenigingDetailMapper.CreateJsonLdMetadata(
                        JsonLdType.Erkenning,
                        fixture.Scenario.AggregateId,
                        fixture.Scenario.ErkenningWerdGeregistreerd.ErkenningId.ToString()
                    ),
                    ErkenningId = fixture.Scenario.ErkenningWerdGeregistreerd.ErkenningId,
                    GeregistreerdDoor = new GegevensInitiator
                    {
                        OvoCode = fixture.Scenario.ErkenningWerdGeregistreerd.GeregistreerdDoor.OvoCode,
                        Naam = fixture.Scenario.ErkenningWerdGeregistreerd.GeregistreerdDoor.Naam,
                    },
                    IpdcProduct = new IpdcProduct
                    {
                        Nummer = fixture.Scenario.ErkenningWerdGeregistreerd.IpdcProduct.Nummer,
                        Naam = fixture.Scenario.ErkenningWerdGeregistreerd.IpdcProduct.Naam,
                    },
                    Startdatum = fixture.Scenario.ErkenningWerdGeregistreerd.Startdatum?.ToString(
                        WellknownFormats.DateOnly
                    ),
                    Einddatum = fixture.Scenario.ErkenningWerdVerlengd.Einddatum.ToString(
                        WellknownFormats.DateOnly
                    ),
                    Hernieuwingsdatum = fixture.Scenario.ErkenningWerdVerlengd.Hernieuwingsdatum?.ToString(
                        WellknownFormats.DateOnly
                    ),
                    HernieuwingsUrl = fixture.Scenario.ErkenningWerdGeregistreerd.HernieuwingsUrl,
                    RedenSchorsing = string.Empty,
                    Status = status.Value,
                },
            ]);
    }
}
