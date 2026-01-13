namespace AssociationRegistry.Test.E2E.When_Verwijder_Bankrekeningnummer_Uit_KBO.Beheer.Detail;

using Admin.Api.WebApi.Verenigingen.Detail.ResponseModels;
using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using Xunit;

[Collection(nameof(BankrekeningnummerWerdVerwijderdUitKBOCollection))]
public class Returns_Detail_With_Verwijderd_Bankrekeningnummer : End2EndTest<DetailVerenigingResponse>
{
    private readonly VerwijderBankrekeningnummerUitKBOContext _testContext;

    public Returns_Detail_With_Verwijderd_Bankrekeningnummer(VerwijderBankrekeningnummerUitKBOContext testContext)
        : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override async Task<DetailVerenigingResponse> GetResponse(FullBlownApiSetup setup)
        => await setup.AdminApiHost.GetBeheerDetail(setup.AdminHttpClient, _testContext.VCode,new RequestParameters().WithExpectedSequence(_testContext.CommandResult.Sequence));

  [Fact]
    public void JsonContentMatches()
    {
        var bankrekeningnummer = _testContext.Scenario.BankrekeningnummerWerdVerwijderdUitKBO;

        Response.Vereniging.Bankrekeningnummers.FirstOrDefault(x => x.Iban == bankrekeningnummer.Iban)
                .Should().BeNull();
    }
}
