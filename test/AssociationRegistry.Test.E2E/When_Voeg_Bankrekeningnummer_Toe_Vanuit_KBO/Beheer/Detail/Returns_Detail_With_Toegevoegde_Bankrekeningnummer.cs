namespace AssociationRegistry.Test.E2E.When_Voeg_Bankrekeningnummer_Toe_Vanuit_KBO.Beheer.Detail;

using Admin.Api.WebApi.Verenigingen.Detail.ResponseModels;
using Contracts.JsonLdContext;
using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using Xunit;

[Collection(nameof(VoegBankrekeningnummerToeVanuitKBOCollection))]
public class Returns_Detail_With_Toegevoegde_Bankrekeningnummer : End2EndTest<DetailVerenigingResponse>
{
    private readonly VoegBankrekeningnummerToeVanuitKBOContext _testContext;

    public Returns_Detail_With_Toegevoegde_Bankrekeningnummer(VoegBankrekeningnummerToeVanuitKBOContext testContext)
        : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override async Task<DetailVerenigingResponse> GetResponse(FullBlownApiSetup setup)
        => await setup.AdminApiHost.GetBeheerDetail(setup.AdminHttpClient, _testContext.VCode,new RequestParameters().WithExpectedSequence(_testContext.CommandResult.Sequence));

  [Fact]
    public void JsonContentMatches()
    {
        var bankrekeningnummer = _testContext.Scenario.BankrekeningnummerWerdToegevoegdVanuitKBO;
        Response.Vereniging.Bankrekeningnummers.Single(x => x.Iban == bankrekeningnummer.Iban)
                .Should().BeEquivalentTo(new Bankrekeningnummer()
                 {
                     type = JsonLdType.Bankrekeningnummer.Type,
                     id = JsonLdType.Bankrekeningnummer.CreateWithIdValues(_testContext.VCode, bankrekeningnummer.BankrekeningnummerId.ToString()),
                     Iban = bankrekeningnummer.Iban,
                     Doel = string.Empty,
                     Titularis = string.Empty,
                 });
    }
}
