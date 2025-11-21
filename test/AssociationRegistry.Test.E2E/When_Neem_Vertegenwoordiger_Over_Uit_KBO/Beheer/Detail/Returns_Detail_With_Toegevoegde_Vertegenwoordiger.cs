namespace AssociationRegistry.Test.E2E.When_Neem_Vertegenwoordiger_Uit_KBO.Beheer.Detail;

using Admin.Api.WebApi.Verenigingen.Detail.ResponseModels;
using Contracts.JsonLdContext;
using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using Vereniging.Bronnen;
using Xunit;

[Collection(nameof(NeemVertegenwoordigerOverUitKBOCollection))]
public class Returns_Detail_With_Toegevoegde_Vertegenwoordiger : End2EndTest<DetailVerenigingResponse>
{
    private readonly NeemVertegenwoordigerOverUitKBOContext _testContext;

    public Returns_Detail_With_Toegevoegde_Vertegenwoordiger(NeemVertegenwoordigerOverUitKBOContext testContext)
        : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override async Task<DetailVerenigingResponse> GetResponse(FullBlownApiSetup setup)
        => await setup.AdminApiHost.GetBeheerDetail(setup.AdminHttpClient, _testContext.VCode,new RequestParameters().WithExpectedSequence(_testContext.CommandResult.Sequence));

  [Fact]
    public void JsonContentMatches()
    {
       var vertegenwoordiger = _testContext.Scenario.VertegenwoordigerWerdOvergenomenUitKBO;
        Response.Vereniging.Vertegenwoordigers.Single(x => x.VertegenwoordigerId == vertegenwoordiger.VertegenwoordigerId)
                .Should().BeEquivalentTo(new Vertegenwoordiger()
                 {
                     type = JsonLdType.Vertegenwoordiger.Type,
                     id = JsonLdType.Vertegenwoordiger.CreateWithIdValues(_testContext.VCode, vertegenwoordiger.VertegenwoordigerId.ToString()),
                     VertegenwoordigerId = vertegenwoordiger.VertegenwoordigerId,
                     Bron = Bron.KBO,
                     Email = string.Empty,
                     Insz = vertegenwoordiger.Insz,
                     Achternaam = vertegenwoordiger.Achternaam,
                     Mobiel = string.Empty,
                     Roepnaam = string.Empty,
                     Rol = string.Empty,
                     SocialMedia = string.Empty,
                     Telefoon = string.Empty,
                     Voornaam = vertegenwoordiger.Voornaam,
                     PrimairContactpersoon = false,
                     VertegenwoordigerContactgegevens = new VertegenwoordigerContactgegevens()
                     {
                         Email = string.Empty,
                         Mobiel = string.Empty,
                         SocialMedia = string.Empty,
                         Telefoon = string.Empty,
                         id = JsonLdType.VertegenwoordigerContactgegeven.CreateWithIdValues(_testContext.VCode, vertegenwoordiger.VertegenwoordigerId.ToString()),
                         type = JsonLdType.VertegenwoordigerContactgegeven.Type,
                     }
                 });
    }
}
