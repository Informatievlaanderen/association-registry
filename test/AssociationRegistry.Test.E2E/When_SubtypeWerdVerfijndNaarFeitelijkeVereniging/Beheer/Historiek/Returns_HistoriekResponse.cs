﻿namespace AssociationRegistry.Test.E2E.When_SubtypeWerdVerfijndNaarFeitelijkeVereniging.Beheer.Historiek;

using Admin.Api.Verenigingen.Historiek.ResponseModels;
using Events;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.Comparison;
using Framework.Mappers;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Xunit;

[Collection(nameof(VerfijnSubtypeNaarFeitelijkeVerenigingCollection))]
public class Returns_Detail : End2EndTest<HistoriekResponse>
{
    private readonly VerfijnSubtypeNaarFeitelijkeVerenigingContext _testContext;

    public Returns_Detail(VerfijnSubtypeNaarFeitelijkeVerenigingContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override HistoriekResponse GetResponse(FullBlownApiSetup setup)
        => setup.AdminApiHost.GetBeheerHistoriek(setup.AdminHttpClient, _testContext.VCode,new RequestParameters().WithExpectedSequence(_testContext.CommandResult.Sequence)).GetAwaiter().GetResult();

    [Fact]
    public void With_VCode()
    {
        Response.VCode.ShouldCompare(_testContext.VCode);
    }

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/beheer/historiek-vereniging-context.json");
    }

    [Fact]
    public void With_All_Gebeurtenissen()
    {
        var werdVerfijnd =
            Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging));

        werdVerfijnd.ShouldCompare(HistoriekGebeurtenisMapper.SubTypeWerdVerfijndNaarFeitelijkeVereniging(_testContext.VCode),
                                        compareConfig: HistoriekComparisonConfig.Instance);
    }
}
