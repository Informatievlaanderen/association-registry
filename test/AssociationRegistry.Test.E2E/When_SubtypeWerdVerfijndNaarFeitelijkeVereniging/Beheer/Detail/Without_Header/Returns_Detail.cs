﻿namespace AssociationRegistry.Test.E2E.When_SubtypeWerdVerfijndNaarFeitelijkeVereniging.Beheer.Detail.Without_Header;

using AssociationRegistry.Admin.Api.Verenigingen.Detail.ResponseModels;
using Admin.Api.Verenigingen.Subtype.RequestModels;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using FluentAssertions;
using KellermanSoftware.CompareNetObjects;
using Xunit;

[Collection(FullBlownApiCollection.Name)]
public class Returns_Detail : End2EndTest<VerfijnSubtypeNaarFeitelijkeVerenigingContext, WijzigSubtypeRequest, DetailVerenigingResponse>
{
    private readonly VerfijnSubtypeNaarFeitelijkeVerenigingContext _context;

    public Returns_Detail(VerfijnSubtypeNaarFeitelijkeVerenigingContext context): base(context)
    {
        _context = context;
    }

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/beheer/detail-vereniging-context.json");
    }

    [Fact]
    public void JsonContentMatches()
    {
        Response.Vereniging.Subtype.Should().BeNull();
    }

    public override Func<IApiSetup, DetailVerenigingResponse> GetResponse
        => setup => setup.AdminApiHost.GetBeheerDetail(TestContext.VCode);
}
