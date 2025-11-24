namespace AssociationRegistry.Test.E2E.VertegenwoordigerPersoonsgegevensTests.When_Wijzig_Naam_On_KBO.Beheer.Detail;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Detail.ResponseModels;
using AssociationRegistry.Formats;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using FluentAssertions;
using KellermanSoftware.CompareNetObjects;
using NodaTime;
using Xunit;

[Collection(nameof(WijzigRoepnaamOnKBOTestCollection))]
public class Returns_Gewijzigde_Naam : End2EndTest<DetailVerenigingResponse>
{
    private readonly WijzigRoepnaamOnKBOTestContext _testContext;

    public Returns_Gewijzigde_Naam(WijzigRoepnaamOnKBOTestContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override async Task<DetailVerenigingResponse> GetResponse(FullBlownApiSetup setup)
        => await setup.AdminApiHost.GetBeheerDetail(setup.AdminHttpClient, _testContext.VCode,new RequestParameters().WithExpectedSequence(_testContext.CommandResult.Sequence));

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/beheer/detail-vereniging-context.json");
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
        => Response.Vereniging.Roepnaam.Should().Be(_testContext.CommandRequest.Roepnaam);
}
