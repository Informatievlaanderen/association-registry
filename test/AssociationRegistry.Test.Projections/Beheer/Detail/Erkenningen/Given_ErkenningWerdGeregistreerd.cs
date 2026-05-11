namespace AssociationRegistry.Test.Projections.Beheer.Detail.Erkenningen;

using Admin.ProjectionHost.Constants;
using Admin.ProjectionHost.Projections.Detail;
using Admin.Schema.Detail;
using Contracts.JsonLdContext;
using Scenario.Erkenningen;

[Collection(nameof(ProjectionContext))]
public class Given_ErkenningWerdGeregistreerd(BeheerDetailScenarioFixture<ErkenningWerdGeregistreerdScenario> fixture)
    : BeheerDetailScenarioClassFixture<ErkenningWerdGeregistreerdScenario>
{
    [Fact]
    public void Metadata_Is_Updated() => fixture.Result.Metadata.Version.Should().Be(2);

    [Fact]
    public void Then_Erkenning_Is_Added()
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
                    Startdatum = fixture.Scenario.ErkenningWerdGeregistreerd.Startdatum?.ToString(
                        WellknownFormats.DateOnly
                    ),
                    Einddatum = fixture.Scenario.ErkenningWerdGeregistreerd.Einddatum?.ToString(
                        WellknownFormats.DateOnly
                    ),
                    Hernieuwingsdatum = fixture.Scenario.ErkenningWerdGeregistreerd.Hernieuwingsdatum?.ToString(
                        WellknownFormats.DateOnly
                    ),
                    HernieuwingsUrl = fixture.Scenario.ErkenningWerdGeregistreerd.HernieuwingsUrl,
                    RedenSchorsing = string.Empty,
                    Status = fixture.Scenario.ErkenningWerdGeregistreerd.Status,
                },
            ]);
    }
}
