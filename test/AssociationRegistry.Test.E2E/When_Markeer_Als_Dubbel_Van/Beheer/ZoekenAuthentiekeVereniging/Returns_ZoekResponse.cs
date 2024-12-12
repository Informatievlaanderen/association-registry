﻿namespace AssociationRegistry.Test.E2E.When_Markeer_Als_Dubbel_Van.Beheer.ZoekenAuthentiekeVereniging;

using AssociationRegistry.Admin.Api.Verenigingen.Dubbels.FeitelijkeVereniging.MarkeerAlsDubbelVan.RequestModels;
using AssociationRegistry.Admin.Api.Verenigingen.Search.ResponseModels;
using AssociationRegistry.Events;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using FluentAssertions;
using KellermanSoftware.CompareNetObjects;
using Xunit;

[Collection(FullBlownApiCollection.Name)]
public class Returns_SearchVerenigingenResponse : End2EndTest<MarkeerAlsDubbelVanContext, MarkeerAlsDubbelVanRequest, SearchVerenigingenResponse>
{
    private readonly MarkeerAlsDubbelVanContext _testContext;

    public Returns_SearchVerenigingenResponse(MarkeerAlsDubbelVanContext testContext) : base(testContext)
    {
        _testContext = testContext;
    }

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/beheer/zoek-verenigingen-context.json");
    }

    [Fact]
    public async Task WithFeitelijkeVereniging()
    {
        Response.Verenigingen.Single(x => x.VCode == _testContext.Scenario.AndereFeitelijkeVerenigingWerdGeregistreerd.VCode)
                .CorresponderendeVCodes.Should().Contain(_testContext.Scenario.FeitelijkeVerenigingWerdGeregistreerd.VCode);
    }

    public override Func<IApiSetup, SearchVerenigingenResponse> GetResponse
        => setup => setup.AdminApiHost.GetBeheerZoeken($"vCode:{_testContext.Scenario.AndereFeitelijkeVerenigingWerdGeregistreerd.VCode}");
}