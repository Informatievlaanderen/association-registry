﻿namespace AssociationRegistry.Test.E2E.When_SubtypeWerdVerfijndNaarSubvereniging.Publiek.Detail.With_Header;

using AssociationRegistry.Admin.Api.Verenigingen.Subtype.RequestModels;
using AssociationRegistry.Public.Api;
using AssociationRegistry.Public.Api.Verenigingen.Detail.ResponseModels;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using FluentAssertions;
using KellermanSoftware.CompareNetObjects;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Verenigingssubtype = Public.Api.Verenigingen.Detail.ResponseModels.Verenigingssubtype;

[Collection(FullBlownApiCollection.Name)]
public class Returns_Detail : End2EndTest<VerfijnSubtypeNaarFeitelijkeVerenigingContext, WijzigSubtypeRequest, PubliekVerenigingDetailResponse>
{
    private readonly VerfijnSubtypeNaarFeitelijkeVerenigingContext _context;

    public Returns_Detail(VerfijnSubtypeNaarFeitelijkeVerenigingContext context): base(context)
    {
        _context = context;
    }

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/publiek/detail-vereniging-context.json");
    }

    [Fact]
    public void JsonContentMatches()
    {
        var expected = new Verenigingssubtype()
        {
            Code = AssociationRegistry.Vereniging.Verenigingssubtype.FeitelijkeVereniging.Code,
            Naam = AssociationRegistry.Vereniging.Verenigingssubtype.FeitelijkeVereniging.Naam
        };

        Response.Vereniging.Verenigingssubtype.Should().BeEquivalentTo(expected);
    }

    public override Func<IApiSetup, PubliekVerenigingDetailResponse> GetResponse
    {
        get { return setup =>
        {
            var logger = setup.PublicApiHost.Services.GetRequiredService<ILogger<Program>>();

            logger.LogInformation("EXECUTING GET REQUEST");

            return setup.PublicApiHost.GetPubliekDetailWithHeader(setup.SuperAdminHttpClient, TestContext.RequestResult.VCode,
                                                                  TestContext.RequestResult.Sequence).GetAwaiter().GetResult();
        }; }
    }
}
