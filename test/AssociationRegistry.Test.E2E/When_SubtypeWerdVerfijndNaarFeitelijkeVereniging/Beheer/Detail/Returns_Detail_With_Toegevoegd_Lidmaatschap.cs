namespace AssociationRegistry.Test.E2E.When_SubtypeWerdVerfijndNaarFeitelijkeVereniging.Beheer.Detail;

using Admin.Api.Verenigingen.Detail.ResponseModels;
using Admin.Api.Verenigingen.Subtype;
using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Xunit;
using Subtype = Vereniging.Subtype;

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
        var expected = new SubtypeData
        {
            //id = JsonLdType.Subtype.CreateWithIdValues(_context.VCode, "1"),
            //type = JsonLdType.Subtype.Type,
            Subtype = new AssociationRegistry.Admin.Api.Verenigingen.Detail.ResponseModels.Subtype()
            {
                Code = Subtype.FeitelijkeVereniging.Code,
                Naam = Subtype.FeitelijkeVereniging.Naam
            },
        };

        Response.Vereniging.Subtype.Should().BeEquivalentTo(expected);
    }

    public override Func<IApiSetup, DetailVerenigingResponse> GetResponse
        => setup => setup.AdminApiHost.GetBeheerDetail(TestContext.VCode);
}
