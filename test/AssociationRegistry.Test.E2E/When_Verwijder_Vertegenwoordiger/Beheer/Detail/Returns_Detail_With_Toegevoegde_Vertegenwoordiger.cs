namespace AssociationRegistry.Test.E2E.When_Verwijder_Vertegenwoordiger.Beheer.Detail;

using Admin.Api.WebApi.Verenigingen.Detail.ResponseModels;
using Contracts.JsonLdContext;
using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using Vereniging.Bronnen;
using Xunit;

[Collection(nameof(VerwijderVertegenwoordigerCollection))]
public class Returns_Detail_With_Toegevoegde_Vertegenwoordiger : End2EndTest<DetailVerenigingResponse>
{
    private readonly VerwijderVertegenwoordigerContext _testContext;

    public Returns_Detail_With_Toegevoegde_Vertegenwoordiger(VerwijderVertegenwoordigerContext testContext)
        : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override async Task<DetailVerenigingResponse> GetResponse(FullBlownApiSetup setup)
        => await setup.AdminApiHost.GetBeheerDetail(setup.AdminHttpClient, _testContext.VCode,new RequestParameters().WithExpectedSequence(_testContext.CommandResult.Sequence));

  [Fact]
    public void JsonContentMatches()
    {
        var vertegenwoordigerId = _testContext.Scenario.VertegenwoordigerWerdToegevoegd.VertegenwoordigerId;
        Response.Vereniging.Vertegenwoordigers.SingleOrDefault(x => x.VertegenwoordigerId == vertegenwoordigerId)
                .Should().BeNull();
    }
}
