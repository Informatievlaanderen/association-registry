namespace AssociationRegistry.Test.E2E.Voeg_Vertegenwoordiger_Toe_Vanuit_KBO.Beheer.Detail;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Detail.ResponseModels;
using AssociationRegistry.Contracts.JsonLdContext;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using AssociationRegistry.Vereniging.Bronnen;
using FluentAssertions;
using Xunit;

[Collection(nameof(VoegVertegenwoordigerToeVanuitKBOCollection))]
public class Returns_Detail_With_Toegevoegde_Vertegenwoordiger : End2EndTest<DetailVerenigingResponse>
{
    private readonly VoegVertegenwoordigerToeVanuitKBOContext _testContext;

    public Returns_Detail_With_Toegevoegde_Vertegenwoordiger(VoegVertegenwoordigerToeVanuitKBOContext testContext)
        : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override async Task<DetailVerenigingResponse> GetResponse(FullBlownApiSetup setup)
        => await setup.AdminApiHost.GetBeheerDetail(setup.AdminHttpClient, _testContext.VCode,new RequestParameters().WithExpectedSequence(_testContext.CommandResult.Sequence));

  [Fact]
    public void JsonContentMatches()
    {
       var vertegenwoordiger = _testContext.Scenario.VertegenwoordigerWerdToegevoegdVanuitKBO;
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
