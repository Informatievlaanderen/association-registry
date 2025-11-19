namespace AssociationRegistry.Test.E2E.When_Voeg_Vertegenwoordiger_Toe.Beheer.Detail;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Detail.ResponseModels;
using AssociationRegistry.Contracts.JsonLdContext;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using AssociationRegistry.Vereniging.Bronnen;
using FluentAssertions;
using When_Voeg_Lidmaatschap_Toe;
using Xunit;

[Collection(nameof(VoegVertegenwoordigerToeCollection))]
public class Returns_Detail_With_Toegevoegde_Vertegenwoordiger : End2EndTest<DetailVerenigingResponse>
{
    private readonly VoegVertegenwoordigerToeContext _testContext;

    public Returns_Detail_With_Toegevoegde_Vertegenwoordiger(VoegVertegenwoordigerToeContext testContext)
        : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override async Task<DetailVerenigingResponse> GetResponse(FullBlownApiSetup setup)
        => await setup.AdminApiHost.GetBeheerDetail(setup.AdminHttpClient, _testContext.VCode,new RequestParameters().WithExpectedSequence(_testContext.CommandResult.Sequence));

  [Fact]
    public void JsonContentMatches()
    {
        var nextVertegenwoordigerId = _testContext.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd
                                           .Vertegenwoordigers.Max(x => x.VertegenwoordigerId) + 1;
        Response.Vereniging.Vertegenwoordigers.Single(x => x.VertegenwoordigerId == nextVertegenwoordigerId)
                .Should().BeEquivalentTo(new Vertegenwoordiger()
                 {
                     type = JsonLdType.Vertegenwoordiger.Type,
                     id = JsonLdType.Vertegenwoordiger.CreateWithIdValues(_testContext.VCode, nextVertegenwoordigerId.ToString()),
                     VertegenwoordigerId = nextVertegenwoordigerId,
                     Bron = Bron.Initiator,
                     Email = _testContext.CommandRequest.Vertegenwoordiger.Email,
                     Insz = _testContext.CommandRequest.Vertegenwoordiger.Insz,
                     Achternaam = _testContext.CommandRequest.Vertegenwoordiger.Achternaam,
                     Mobiel = _testContext.CommandRequest.Vertegenwoordiger.Mobiel,
                     Roepnaam = _testContext.CommandRequest.Vertegenwoordiger.Roepnaam,
                     Rol = _testContext.CommandRequest.Vertegenwoordiger.Rol,
                     SocialMedia = _testContext.CommandRequest.Vertegenwoordiger.SocialMedia,
                     Telefoon = _testContext.CommandRequest.Vertegenwoordiger.Telefoon,
                     Voornaam = _testContext.CommandRequest.Vertegenwoordiger.Voornaam,
                     PrimairContactpersoon = false,
                     VertegenwoordigerContactgegevens = new VertegenwoordigerContactgegevens()
                     {
                         Email = _testContext.CommandRequest.Vertegenwoordiger.Email,
                         Mobiel = _testContext.CommandRequest.Vertegenwoordiger.Mobiel,
                         SocialMedia = _testContext.CommandRequest.Vertegenwoordiger.SocialMedia,
                         Telefoon = _testContext.CommandRequest.Vertegenwoordiger.Telefoon,
                         id = JsonLdType.VertegenwoordigerContactgegeven.CreateWithIdValues(_testContext.VCode, nextVertegenwoordigerId.ToString()),
                         type = JsonLdType.VertegenwoordigerContactgegeven.Type,
                     }
                 });
    }
}
