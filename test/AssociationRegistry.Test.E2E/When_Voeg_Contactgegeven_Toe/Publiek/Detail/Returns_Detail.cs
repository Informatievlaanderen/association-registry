namespace AssociationRegistry.Test.E2E.When_Voeg_Contactgegeven_Toe.Publiek.Detail;

using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using JsonLdContext;
using Public.Api.WebApi.Verenigingen.Detail.ResponseModels;
using Xunit;

[Collection(nameof(VoegContactgegevenToeCollection))]
public class Returns_Detail_With_Lidmaatschap : End2EndTest<PubliekVerenigingDetailResponse>
{
    private readonly VoegContactgegevenToeContext _testContext;

    public Returns_Detail_With_Lidmaatschap(VoegContactgegevenToeContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override PubliekVerenigingDetailResponse GetResponse(FullBlownApiSetup setup)
        => setup.PublicApiHost.GetPubliekDetail(_testContext.VCode);

    [Fact]
    public void JsonContentMatches()
    {
        var nextContactgegevenId = _testContext.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd
                                           .Contactgegevens.Max(x => x.ContactgegevenId) + 1;
        Response.Vereniging.Contactgegevens.Last()
                .Should().BeEquivalentTo(new Contactgegeven()
                 {
                     type = JsonLdType.Contactgegeven.Type,
                     id = JsonLdType.Contactgegeven.CreateWithIdValues(_testContext.VCode, nextContactgegevenId.ToString()),
                     Beschrijving = _testContext.CommandRequest.Contactgegeven.Beschrijving,
                     Contactgegeventype = _testContext.CommandRequest.Contactgegeven.Contactgegeventype,
                     IsPrimair = _testContext.CommandRequest.Contactgegeven.IsPrimair,
                     Waarde = _testContext.CommandRequest.Contactgegeven.Waarde
                 });
    }
}
