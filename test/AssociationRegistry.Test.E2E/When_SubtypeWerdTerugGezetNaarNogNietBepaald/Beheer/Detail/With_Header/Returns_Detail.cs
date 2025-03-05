namespace AssociationRegistry.Test.E2E.When_SubtypeWerdTerugGezetNaarNogNietBepaald.Beheer.Detail.With_Header;

using Admin.Api;
using Admin.Api.Verenigingen.Detail.ResponseModels;
using Admin.Api.Verenigingen.Subtype.RequestModels;
using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using JsonLdContext;
using KellermanSoftware.CompareNetObjects;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Subtype = Vereniging.Subtype;

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
        var expected = new SubtypeData
        {
            id = JsonLdType.Subtype.CreateWithIdValues(Subtype.NogNietBepaald.Code),
            type = JsonLdType.Subtype.Type,
            Subtype = new Admin.Api.Verenigingen.Detail.ResponseModels.Subtype()
            {
                Code = Subtype.NogNietBepaald.Code,
                Naam = Subtype.NogNietBepaald.Naam,
            },
        };

        Response.Vereniging.Subtype.Should().BeEquivalentTo(expected);
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
