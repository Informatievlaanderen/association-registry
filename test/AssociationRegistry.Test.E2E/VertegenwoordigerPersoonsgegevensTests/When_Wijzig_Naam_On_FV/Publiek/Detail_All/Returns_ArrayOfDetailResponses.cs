namespace AssociationRegistry.Test.E2E.VertegenwoordigerPersoonsgegevensTests.When_Wijzig_Naam_On_FV.Publiek.Detail_All;

using AssociationRegistry.Contracts.JsonLdContext;
using AssociationRegistry.Formats;
using AssociationRegistry.Public.Api.WebApi.Verenigingen.Detail.ResponseModels;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.Comparison;
using AssociationRegistry.Test.E2E.Framework.Mappers;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using FluentAssertions;
using KellermanSoftware.CompareNetObjects;
using NodaTime;
using Xunit;
using DoelgroepResponse = Public.Api.WebApi.Verenigingen.Detail.ResponseModels.DoelgroepResponse;
using Vereniging = Public.Api.WebApi.Verenigingen.Detail.ResponseModels.Vereniging;
using Verenigingssubtype = Public.Api.WebApi.Verenigingen.Detail.ResponseModels.Verenigingssubtype;
using VerenigingStatus = Admin.Schema.Constants.VerenigingStatus;
using Verenigingstype = Public.Api.WebApi.Verenigingen.Detail.ResponseModels.Verenigingstype;

[Collection(nameof(WijzigNaamOnFVTestCollection))]
public class Returns_ArrayOfDetailResponses
    : End2EndTest<PubliekVerenigingDetailResponse>
{
    private readonly WijzigNaamOnFVTestContext _testContext;

    public Returns_ArrayOfDetailResponses(WijzigNaamOnFVTestContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override async Task<PubliekVerenigingDetailResponse> GetResponse(FullBlownApiSetup setup)
    {
        var details = await setup.PublicApiHost.GetPubliekDetailAll(_testContext.CommandResult.Sequence);
        return details.FindVereniging(_testContext.VCode);
    }

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/publiek/detail-all-vereniging-context.json");
    }

    [Fact]

    public void With_Metadata_DatumLaatsteAanpassing()
    {
        Response.Metadata.DatumLaatsteAanpassing.ShouldCompare(Instant.FromDateTimeOffset(DateTimeOffset.Now).FormatAsBelgianDate(),
                                                                        compareConfig: new ComparisonConfig
                                                                            { MaxMillisecondsDateDifference = 5000 });
    }

    [Fact]
    public void WithFeitelijkeVereniging()
        => Response.Vereniging.Naam.Should().Be(_testContext.CommandRequest.Naam);
}
