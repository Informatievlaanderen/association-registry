﻿namespace AssociationRegistry.Test.E2E.When_SubtypeWerdTerugGezetNaarNogNietBepaald.Beheer.Detail.Without_Header;

using Admin.Api.Verenigingen.Detail.ResponseModels;
using Admin.Api.Verenigingen.Subtype;
using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Xunit;

[Collection(FullBlownApiCollection.Name)]
public class Returns_Detail : End2EndTest<ZetSubtypeNaarNogNietBepaaldContext, WijzigSubtypeRequest, DetailVerenigingResponse>
{
    private readonly ZetSubtypeNaarNogNietBepaaldContext _context;

    public Returns_Detail(ZetSubtypeNaarNogNietBepaaldContext context): base(context)
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
