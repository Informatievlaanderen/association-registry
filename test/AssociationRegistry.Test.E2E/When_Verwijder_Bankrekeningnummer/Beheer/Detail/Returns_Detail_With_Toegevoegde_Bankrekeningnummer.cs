namespace AssociationRegistry.Test.E2E.When_Verwijder_Bankrekeningnummer.Beheer.Detail;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Detail.ResponseModels;
using AssociationRegistry.Contracts.JsonLdContext;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using FluentAssertions;
using Xunit;

[Collection(nameof(VerwijderBankrekeningnummerCollection))]
public class Returns_Detail_Without_Verwijderd_Bankrekeningnummer : End2EndTest<DetailVerenigingResponse>
{
    private readonly VerwijderBankrekeningnummerContext _testContext;

    public Returns_Detail_Without_Verwijderd_Bankrekeningnummer(VerwijderBankrekeningnummerContext testContext)
        : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override async Task<DetailVerenigingResponse> GetResponse(FullBlownApiSetup setup)
        => await setup.AdminApiHost.GetBeheerDetail(setup.AdminHttpClient, _testContext.VCode,new RequestParameters().WithExpectedSequence(_testContext.CommandResult.Sequence));

  [Fact]
    public void JsonContentMatches()
    {
        Response.Vereniging.Bankrekeningnummers.Should().BeEmpty();
    }
}
