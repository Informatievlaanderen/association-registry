namespace AssociationRegistry.Test.E2E.VertegenwoordigerPersoonsgegevensTests.When_Wijzig_Naam_On_Vzer.Publiek.Detail;

using AssociationRegistry.Formats;
using AssociationRegistry.Public.Api.WebApi.Verenigingen.Detail.ResponseModels;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using FluentAssertions;
using KellermanSoftware.CompareNetObjects;
using NodaTime;
using When_Wijzig_Naam_On_FV;
using Xunit;

[Collection(nameof(WijzigNaamOnVZERTestCollection))]
public class Returns_DetailResponse
    : End2EndTest<PubliekVerenigingDetailResponse>
{
    private readonly WijzigNaamOnVZERTestContext _testContext;

    public Returns_DetailResponse(WijzigNaamOnVZERTestContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override async Task<PubliekVerenigingDetailResponse> GetResponse(FullBlownApiSetup setup)
        => await setup.PublicApiHost.GetPubliekDetail(_testContext.VCode);

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/publiek/detail-vereniging-context.json");
    }

    [Fact]
    public void With_Metadata_DatumLaatsteAanpassing()
    {
        Response.Metadata.DatumLaatsteAanpassing.ShouldCompare(Instant.FromDateTimeOffset(DateTimeOffset.Now).FormatAsBelgianDate(),
                                                               compareConfig: new ComparisonConfig
                                                                   { MaxMillisecondsDateDifference = 5000 });
    }

    [Fact]
    public async ValueTask WithFeitelijkeVereniging()
        => Response.Vereniging.Naam.Should().Be(_testContext.CommandRequest.Naam);
}
