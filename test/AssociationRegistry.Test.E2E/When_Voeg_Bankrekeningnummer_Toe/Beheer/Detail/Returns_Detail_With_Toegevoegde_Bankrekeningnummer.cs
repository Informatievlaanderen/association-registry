namespace AssociationRegistry.Test.E2E.When_Voeg_Bankrekeningnummer_Toe.Beheer.Detail;

using Admin.Api.WebApi.Verenigingen.Detail.ResponseModels;
using Be.Vlaanderen.Basisregisters.Utilities;
using Contracts.JsonLdContext;
using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using Xunit;

[Collection(nameof(VoegBankrekeningnummerToeCollection))]
public class Returns_Detail_With_Toegevoegde_Bankrekeningnummer : End2EndTest<DetailVerenigingResponse>
{
    private readonly VoegBankrekeningnummerToeContext _testContext;

    public Returns_Detail_With_Toegevoegde_Bankrekeningnummer(VoegBankrekeningnummerToeContext testContext)
        : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override async Task<DetailVerenigingResponse> GetResponse(FullBlownApiSetup setup)
        => await setup.AdminApiHost.GetBeheerDetail(setup.AdminHttpClient, _testContext.VCode,new RequestParameters().WithExpectedSequence(_testContext.CommandResult.Sequence));

  [Fact]
    public void JsonContentMatches()
    {
        Response.Vereniging.Bankrekeningnummers.Should().BeEquivalentTo([
            new Bankrekeningnummer()
            {
                type = JsonLdType.Bankrekeningnummer.Type,
                id = JsonLdType.Bankrekeningnummer.CreateWithIdValues(_testContext.VCode, 1.ToString()),
                BankrekeningnummerId = 1,
                Iban = _testContext.CommandRequest.Bankrekeningnummer.Iban,
                Doel = _testContext.CommandRequest.Bankrekeningnummer.Doel,
                Titularis = _testContext.CommandRequest.Bankrekeningnummer.Titularis,
            },
        ]);
    }
}
