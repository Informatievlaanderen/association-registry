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
public class Given_ErkenningWerdVerwijderd(BeheerDetailScenarioFixture<ErkenningWerdVerwijderdScenario> fixture)
    : BeheerDetailScenarioClassFixture<ErkenningWerdVerwijderdScenario>
{
    [Fact]
    public void Metadata_Is_Updated() => fixture.Result.Metadata.Version.Should().Be(3);

    [Fact]
    public void Erkenning_Werd_Ge()
    {
        fixture.Result.Erkenningen.Should().BeEmpty();
    }
}
