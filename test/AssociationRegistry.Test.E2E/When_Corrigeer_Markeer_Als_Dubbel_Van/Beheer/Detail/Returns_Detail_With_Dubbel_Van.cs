namespace AssociationRegistry.Test.E2E.When_Corrigeer_Markeer_Als_Dubbel_Van.Beheer.Detail;

using Admin.Api.Verenigingen.Detail.ResponseModels;
using Admin.Schema.Constants;
using Framework.AlbaHost;
using FluentAssertions;
using Framework.ApiSetup;
using Framework.TestClasses;
using Scenarios.Requests;
using Xunit;

[Collection(FullBlownApiCollection.Name)]
public class Returns_Detail_With_Dubbel_Van : End2EndTest<CorrigeerMarkeringAlsDubbelVanContext, NullRequest,
    DetailVerenigingResponse>
{
    private readonly CorrigeerMarkeringAlsDubbelVanContext _context;

    public Returns_Detail_With_Dubbel_Van(CorrigeerMarkeringAlsDubbelVanContext context) : base(context)
    {
        _context = context;
    }

    [Fact]
    public void With_IsDubbelVan_VCode_Of_AndereFeitelijkeVerenigingWerdGeregistreerd()
    {
        Response.Vereniging.IsDubbelVan.Should().BeEmpty();
    }

    [Fact]
    public void With_Status_Is_Dubbel()
    {
        Response.Vereniging.Status.Should().Be(VerenigingStatus.Actief);
    }

    public override Func<IApiSetup, DetailVerenigingResponse> GetResponse
        => setup => setup.AdminApiHost.GetBeheerDetail(_context.Scenario.DubbeleVerenging.VCode);
}
