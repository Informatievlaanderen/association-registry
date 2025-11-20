namespace AssociationRegistry.Test.E2E.When_Wijzig_Vertegenwoordiger_Voor_Rechtspersoon.Beheer.Detail;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Detail.ResponseModels;
using AssociationRegistry.Contracts.JsonLdContext;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using AssociationRegistry.Vereniging.Bronnen;
using FluentAssertions;
using Xunit;

[Collection(nameof(WijzigVertegenwoordigerVoorRechtspersoonCollection))]
public class Returns_Detail_With_Toegevoegde_Vertegenwoordiger : End2EndTest<DetailVerenigingResponse>
{
    private readonly WijzigVertegenwoordigerContextVoorRechtspersoon _testContext;

    public Returns_Detail_With_Toegevoegde_Vertegenwoordiger(WijzigVertegenwoordigerContextVoorRechtspersoon testContext)
        : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override async Task<DetailVerenigingResponse> GetResponse(FullBlownApiSetup setup)
        => await setup.AdminApiHost.GetBeheerDetail(setup.AdminHttpClient, _testContext.VCode,new RequestParameters().WithExpectedSequence(_testContext.CommandResult.Sequence));

  [Fact]
    public void JsonContentMatches()
    {
        var vertegenwoordigerId = _testContext.Scenario.VertegenwoordigerWerdOvergenomenUitKBO.VertegenwoordigerId;
        Response.Vereniging.Vertegenwoordigers.Single(x => x.VertegenwoordigerId == vertegenwoordigerId)
                .Should().BeEquivalentTo(new Vertegenwoordiger()
                 {
                     type = JsonLdType.Vertegenwoordiger.Type,
                     id = JsonLdType.Vertegenwoordiger.CreateWithIdValues(_testContext.VCode, vertegenwoordigerId.ToString()),
                     VertegenwoordigerId = vertegenwoordigerId,
                     Bron = Bron.KBO,
                     Email = _testContext.CommandRequest.Vertegenwoordiger.Email,
                     Insz = _testContext.Scenario.VertegenwoordigerWerdOvergenomenUitKBO.Insz,
                     Achternaam = _testContext.Scenario.VertegenwoordigerWerdOvergenomenUitKBO.Achternaam,
                     Mobiel = _testContext.CommandRequest.Vertegenwoordiger.Mobiel,
                     Roepnaam = _testContext.CommandRequest.Vertegenwoordiger.Roepnaam,
                     Rol = _testContext.CommandRequest.Vertegenwoordiger.Rol,
                     SocialMedia = _testContext.CommandRequest.Vertegenwoordiger.SocialMedia,
                     Telefoon = _testContext.CommandRequest.Vertegenwoordiger.Telefoon,
                     Voornaam = _testContext.Scenario.VertegenwoordigerWerdOvergenomenUitKBO.Voornaam,
                     PrimairContactpersoon = false,
                     VertegenwoordigerContactgegevens = new VertegenwoordigerContactgegevens()
                     {
                         Email = _testContext.CommandRequest.Vertegenwoordiger.Email,
                         Mobiel = _testContext.CommandRequest.Vertegenwoordiger.Mobiel,
                         SocialMedia = _testContext.CommandRequest.Vertegenwoordiger.SocialMedia,
                         Telefoon = _testContext.CommandRequest.Vertegenwoordiger.Telefoon,
                         id = JsonLdType.VertegenwoordigerContactgegeven.CreateWithIdValues(_testContext.VCode, vertegenwoordigerId.ToString()),
                         type = JsonLdType.VertegenwoordigerContactgegeven.Type,
                     }
                 });
    }
}
