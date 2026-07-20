namespace AssociationRegistry.Test.E2E.When_Zet_Vereniging_In_Stopzetting.Publiek.Detail;

using FluentAssertions;
using Formats;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using NodaTime;
using Public.Api.WebApi.Verenigingen.Detail.ResponseModels;
using Xunit;

[Collection(nameof(ZetVerenigingInStopzettingCollection))]
public class Returns_DetailResponse : End2EndTest<PubliekVerenigingDetailResponse>
{
    private readonly ZetVerenigingInStopzettingContext _testContext;

    public Returns_DetailResponse(ZetVerenigingInStopzettingContext testContext)
        : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override async Task<PubliekVerenigingDetailResponse> GetResponse(FullBlownApiSetup setup) =>
        await setup.PublicApiHost.GetPubliekDetail(_testContext.VCode);

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/publiek/detail-vereniging-context.json");
    }

    [Fact]
    public void With_Metadata_DatumLaatsteAanpassing()
    {
        Response.Metadata.DatumLaatsteAanpassing.ShouldCompare(
            Instant.FromDateTimeOffset(DateTimeOffset.Now).FormatAsBelgianDate(),
            compareConfig: new ComparisonConfig { MaxMillisecondsDateDifference = 5000 }
        );
    }

    [Fact]
    public async ValueTask WithFeitelijkeVereniging() => Response.Vereniging.InStopzetting.Should().BeTrue();
}
