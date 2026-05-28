namespace AssociationRegistry.Test.Projections.Beheer.Detail.Erkenningen;

using Admin.ProjectionHost.Constants;
using Admin.ProjectionHost.Projections.Detail;
using Contracts.JsonLdContext;
using Scenario.Erkenningen;
using Erkenning = Admin.Schema.Detail.Erkenning;
using GegevensInitiator = Admin.Schema.Detail.GegevensInitiator;
using IpdcProduct = Admin.Schema.Detail.IpdcProduct;

[Collection(nameof(ProjectionContext))]
public class Given_ErkenningWerdGewijzigd(
    BeheerDetailScenarioFixture<ErkenningWerdGewijzigdScenario> fixture
) : BeheerDetailScenarioClassFixture<ErkenningWerdGewijzigdScenario>
{
    [Fact]
    public void Metadata_Is_Updated() => fixture.Result.Metadata.Version.Should().Be(3);

    [Fact]
    public void Erkenning_Werd_Gewijzigd()
    {
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
                    Startdatum = fixture.Scenario.ErkenningWerdGewijzigd.Startdatum?.ToString(
                        WellknownFormats.DateOnly
                    ),
                    Einddatum = fixture.Scenario.ErkenningWerdGewijzigd.Einddatum?.ToString(
                        WellknownFormats.DateOnly
                    ),
                    Hernieuwingsdatum = fixture.Scenario.ErkenningWerdGewijzigd.Hernieuwingsdatum?.ToString(
                        WellknownFormats.DateOnly
                    ),
                    HernieuwingsUrl = fixture.Scenario.ErkenningWerdGewijzigd.HernieuwingsUrl,
                    RedenSchorsing = string.Empty,
                    Status = fixture.Scenario.ErkenningWerdGewijzigd.Status,
                },
            ]);
    }
}
