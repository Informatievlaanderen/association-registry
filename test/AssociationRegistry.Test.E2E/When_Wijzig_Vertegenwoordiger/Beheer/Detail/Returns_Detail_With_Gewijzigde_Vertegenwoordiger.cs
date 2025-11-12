namespace AssociationRegistry.Test.E2E.When_Wijzig_Vertegenwoordiger.Beheer.Detail;

using Admin.Api.WebApi.Verenigingen.Detail.ResponseModels;
using Contracts.JsonLdContext;
using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using Vereniging.Bronnen;
using Xunit;

[Collection(nameof(WijzigVertegenwoordigerCollection))]
public class Returns_Detail_With_Gewijzigde_Vertegenwoordiger : End2EndTest<DetailVerenigingResponse>
{
    private readonly WijzigVertegenwoordigerContext _testContext;

    public Returns_Detail_With_Gewijzigde_Vertegenwoordiger(WijzigVertegenwoordigerContext testContext)
        : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override async Task<DetailVerenigingResponse> GetResponse(FullBlownApiSetup setup)
        => await setup.AdminApiHost.GetBeheerDetail(setup.AdminHttpClient, _testContext.VCode,new RequestParameters().WithExpectedSequence(_testContext.CommandResult.Sequence));

  [Fact]
    public void JsonContentMatches()
    {
        Response.Vereniging.Vertegenwoordigers.Single(x => x.VertegenwoordigerId == _testContext.Scenario.VertegenwoordigerWerdToegevoegd.VertegenwoordigerId)
                .Should().BeEquivalentTo(new Vertegenwoordiger()
                 {
                     type = JsonLdType.Vertegenwoordiger.Type,
                     id = JsonLdType.Vertegenwoordiger.CreateWithIdValues(_testContext.VCode, _testContext.Scenario.VertegenwoordigerWerdToegevoegd.VertegenwoordigerId.ToString()),
                     VertegenwoordigerId = _testContext.Scenario.VertegenwoordigerWerdToegevoegd.VertegenwoordigerId,
                     Bron = Bron.Initiator,
                     Email = _testContext.CommandRequest.Vertegenwoordiger.Email,
                     Insz = _testContext.Scenario.VertegenwoordigerPersoonsgegevensDocument.Insz,
                     Achternaam = _testContext.Scenario.VertegenwoordigerPersoonsgegevensDocument.Achternaam,
                     Mobiel = _testContext.CommandRequest.Vertegenwoordiger.Mobiel,
                     Roepnaam = _testContext.CommandRequest.Vertegenwoordiger.Roepnaam,
                     Rol = _testContext.CommandRequest.Vertegenwoordiger.Rol,
                     SocialMedia = _testContext.CommandRequest.Vertegenwoordiger.SocialMedia,
                     Telefoon = _testContext.CommandRequest.Vertegenwoordiger.Telefoon,
                     Voornaam = _testContext.Scenario.VertegenwoordigerPersoonsgegevensDocument.Voornaam,
                     PrimairContactpersoon = false,
                     VertegenwoordigerContactgegevens = new VertegenwoordigerContactgegevens()
                     {
                         Email = _testContext.CommandRequest.Vertegenwoordiger.Email,
                         Mobiel = _testContext.CommandRequest.Vertegenwoordiger.Mobiel,
                         SocialMedia = _testContext.CommandRequest.Vertegenwoordiger.SocialMedia,
                         Telefoon = _testContext.CommandRequest.Vertegenwoordiger.Telefoon,
                         id = JsonLdType.VertegenwoordigerContactgegeven.CreateWithIdValues(_testContext.VCode, _testContext.Scenario.VertegenwoordigerWerdToegevoegd.VertegenwoordigerId.ToString()),
                         type = JsonLdType.VertegenwoordigerContactgegeven.Type,
                     }
                 });
    }
}
