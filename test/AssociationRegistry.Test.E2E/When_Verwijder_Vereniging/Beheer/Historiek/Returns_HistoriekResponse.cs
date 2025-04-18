﻿namespace AssociationRegistry.Test.E2E.When_Verwijder_Vereniging.Beheer.Historiek;

using Admin.Api.Verenigingen.Historiek.ResponseModels;
using Admin.Api.Verenigingen.Verwijder.RequestModels;
using Events;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.Comparison;
using Framework.Mappers;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Xunit;

[Collection(FullBlownApiCollection.Name)]
public class Returns_Historiek : End2EndTest<VerwijderVerenigingContext, VerwijderVerenigingRequest, HistoriekResponse>
{
    private readonly VerwijderVerenigingContext _testContext;

    public override Func<IApiSetup, HistoriekResponse> GetResponse
        => setup => setup.AdminApiHost.GetBeheerHistoriek(TestContext.VCode);

    public Returns_Historiek(VerwijderVerenigingContext testContext) : base(testContext)
    {
        _testContext = testContext;
    }

    [Fact]
    public void With_VCode()
    {
        Response.VCode.ShouldCompare(TestContext.VCode);
    }

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/beheer/historiek-vereniging-context.json");
    }

    [Fact]
    public void With_VerenigingWerdGestopt_Gebeurtenis()
    {
        var gebeurtenisResponse = Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(VerenigingWerdVerwijderd));

        gebeurtenisResponse.ShouldCompare(HistoriekGebeurtenisMapper.VerenigingWerdVerwijderd(_testContext.Request.Reden),
                                                 compareConfig: HistoriekComparisonConfig.Instance);
    }
}
