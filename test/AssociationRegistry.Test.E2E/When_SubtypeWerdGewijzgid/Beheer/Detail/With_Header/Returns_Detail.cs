﻿namespace AssociationRegistry.Test.E2E.When_SubtypeWerdGewijzgid.Beheer.Detail.With_Header;

using AssociationRegistry.Admin.Api;
using AssociationRegistry.Admin.Api.Verenigingen.Detail.ResponseModels;
using AssociationRegistry.Admin.Api.Verenigingen.Subtype.RequestModels;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using FluentAssertions;
using KellermanSoftware.CompareNetObjects;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Verenigingssubtype = Admin.Api.Verenigingen.Detail.ResponseModels.Verenigingssubtype;

[Collection(FullBlownApiCollection.Name)]
public class Returns_Detail : End2EndTest<WhenSubtypeWerdGewijzigdContext, WijzigSubtypeRequest, DetailVerenigingResponse>
{
    private readonly WhenSubtypeWerdGewijzigdContext _context;

    public Returns_Detail(WhenSubtypeWerdGewijzigdContext context): base(context)
    {
        _context = context;
    }

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/beheer/detail-vereniging-context.json");
    }

    [Fact]
    public void Subtype_Is_Subvereniging()
    {
        var expected = new Verenigingssubtype()
        {
            Code = Vereniging.Verenigingssubtype.Subvereniging.Code,
            Naam = Vereniging.Verenigingssubtype.Subvereniging.Naam
        };

        Response.Vereniging.Verenigingssubtype.Should().BeEquivalentTo(expected);

        Response.Vereniging.SubverenigingVan.Should().BeEquivalentTo(new SubverenigingVan()
        {
            Naam = _context.Scenario.BaseScenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.Naam,
            AndereVereniging = _context.Request.AndereVereniging!,
            Beschrijving = _context.Request.Beschrijving!,
            Identificatie = _context.Request.Identificatie!,
        });
    }

    public override Func<IApiSetup, DetailVerenigingResponse> GetResponse
    {
        get { return setup =>
        {
            var logger = setup.AdminApiHost.Services.GetRequiredService<ILogger<Program>>();

            logger.LogInformation("EXECUTING GET REQUEST");

            return setup.AdminApiHost.GetBeheerDetailWithHeader(setup.SuperAdminHttpClient, TestContext.RequestResult.VCode,
                                                                TestContext.RequestResult.Sequence)
                        .GetAwaiter().GetResult();
        }; }
    }
}
