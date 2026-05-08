namespace AssociationRegistry.Test.E2E.Erkenningen.When_Verwijder_Erkenning.Beheer.Detail;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Detail.ResponseModels;
using AssociationRegistry.Admin.ProjectionHost.Constants;
using AssociationRegistry.Contracts.JsonLdContext;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using FluentAssertions;
using Xunit;
using Erkenning = Admin.Api.WebApi.Verenigingen.Detail.ResponseModels.Erkenning;
using IpdcProduct = Admin.Api.WebApi.Verenigingen.Detail.ResponseModels.IpdcProduct;

[Collection(nameof(VerwijderErkenningCollection))]
public class Returns_Detail_With_Geschorste_Erkenning : End2EndTest<DetailVerenigingResponse>
{
    private readonly VerwijderErkenningContext _testContext;

    public Returns_Detail_With_Geschorste_Erkenning(VerwijderErkenningContext testContext)
        : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override async Task<DetailVerenigingResponse> GetResponse(FullBlownApiSetup setup) =>
        await setup.AdminApiHost.GetBeheerDetail(
            setup.AdminHttpClient,
            _testContext.VCode,
            new RequestParameters().WithExpectedSequence(_testContext.CommandResult.Sequence)
        );

    [Fact]
    public void JsonContentMatches()
    {
        Response.Vereniging.Erkenningen.Should().BeEmpty();
    }
}
