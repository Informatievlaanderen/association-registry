namespace AssociationRegistry.Test.E2E.When_SubtypeWerdVerfijndNaarFeitelijkeVereniging.Beheer.Detail.With_Header;

using Admin.Api;
using Admin.Api.Verenigingen.Detail.ResponseModels;
using Admin.Api.Verenigingen.Subtype.RequestModels;
using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Verenigingssubtype = Admin.Api.Verenigingen.Detail.ResponseModels.Verenigingssubtype;

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
        var expected = new Verenigingssubtype()
        {
            Code = Vereniging.Verenigingssubtype.FeitelijkeVereniging.Code,
            Naam = Vereniging.Verenigingssubtype.FeitelijkeVereniging.Naam
        };

        Response.Vereniging.Verenigingssubtype.Should().BeEquivalentTo(expected);
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
