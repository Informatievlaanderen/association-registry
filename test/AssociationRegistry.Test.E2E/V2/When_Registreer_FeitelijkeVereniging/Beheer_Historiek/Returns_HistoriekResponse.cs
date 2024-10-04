﻿namespace AssociationRegistry.Test.E2E.V2.When_Registreer_FeitelijkeVereniging.Beheer_Historiek;

using Admin.Api.Verenigingen.Historiek.ResponseModels;
using Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using Events;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.Comparison;
using Framework.Mappers;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Xunit;

[Collection(FullBlownApiCollection.Name)]
public class Returns_Historiek : End2EndTest<RegistreerFeitelijkeVerenigingContext, RegistreerFeitelijkeVerenigingRequest, HistoriekResponse>
{
    public override Func<IApiSetup, HistoriekResponse> GetResponse
        => setup => setup.AdminApiHost.GetHistoriek(Context.VCode);

    public Returns_Historiek(RegistreerFeitelijkeVerenigingContext context) : base(context)
    {
    }

    [Fact]
    public void With_VCode()
    {
        Response.VCode.ShouldCompare(Context.VCode);
    }

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/beheer/historiek-vereniging-context.json");
    }

    [Fact]
    public void With_All_Gebeurtenissen()
    {
        var werdGeregistreerd =
            Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(FeitelijkeVerenigingWerdGeregistreerd));

        werdGeregistreerd.ShouldCompare(HistoriekGebeurtenisMapper.FeitelijkeVerenigingWerdGeregistreerd(Context.Request, Context.VCode),
                                        compareConfig: HistoriekComparisonConfig.Instance);

    }
}