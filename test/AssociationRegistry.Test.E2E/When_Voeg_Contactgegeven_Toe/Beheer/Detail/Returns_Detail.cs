namespace AssociationRegistry.Test.E2E.When_Voeg_Contactgegeven_Toe.Beheer.Detail;

using Admin.Api.WebApi.Verenigingen.Detail.ResponseModels;
using Contracts.JsonLdContext;
using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using Vereniging.Bronnen;
using Xunit;

[Collection(nameof(VoegContactgegevenToeCollection))]
public class Returns_Detail_With_Lidmaatschap : End2EndTest<DetailVerenigingResponse>
{
    private readonly VoegContactgegevenToeContext _testContext;

    public Returns_Detail_With_Lidmaatschap(VoegContactgegevenToeContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override async Task<DetailVerenigingResponse> GetResponse(FullBlownApiSetup setup)
        => await setup.AdminApiHost.GetBeheerDetail(setup.AdminHttpClient, _testContext.VCode,new RequestParameters().WithExpectedSequence(_testContext.CommandResult.Sequence));

    [Fact]
    public void JsonContentMatches()
    {
        var nextContactgegevenId = _testContext.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdMetPersoonsgegevens
                                           .Contactgegevens.Max(x => x.ContactgegevenId) + 1;
        Response.Vereniging.Contactgegevens.Single(x => x.ContactgegevenId == nextContactgegevenId)
                .Should().BeEquivalentTo(new Contactgegeven()
                 {
                     type = JsonLdType.Contactgegeven.Type,
                     id = JsonLdType.Contactgegeven.CreateWithIdValues(_testContext.VCode, nextContactgegevenId.ToString()),
                     ContactgegevenId = nextContactgegevenId,
                     Beschrijving = _testContext.CommandRequest.Contactgegeven.Beschrijving,
                     Bron = Bron.Initiator,
                     Contactgegeventype = _testContext.CommandRequest.Contactgegeven.Contactgegeventype,
                     IsPrimair = _testContext.CommandRequest.Contactgegeven.IsPrimair,
                     Waarde = _testContext.CommandRequest.Contactgegeven.Waarde
                 });
    }
}
