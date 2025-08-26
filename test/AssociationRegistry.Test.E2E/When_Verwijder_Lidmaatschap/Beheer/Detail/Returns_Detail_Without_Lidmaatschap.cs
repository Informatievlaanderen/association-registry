namespace AssociationRegistry.Test.E2E.When_Verwijder_Lidmaatschap.Beheer.Detail;

using Admin.Api.WebApi.Verenigingen.Detail.ResponseModels;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using FluentAssertions;
using Xunit;

[Collection(nameof(VerwijderLidmaatschapCollection))]
public class Returns_Detail_Without_Lidmaatschap : End2EndTest<DetailVerenigingResponse>
{
    private readonly VerwijderLidmaatschapContext _testContext;

    public Returns_Detail_Without_Lidmaatschap(VerwijderLidmaatschapContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    [Fact]
    public void JsonContentMatches()
    {
        Response.Vereniging.Lidmaatschappen.Should().BeEmpty();
    }

    public override async Task<DetailVerenigingResponse> GetResponse(FullBlownApiSetup setup)
        => await setup.AdminApiHost.GetBeheerDetail(setup.AdminHttpClient, _testContext.VCode,new RequestParameters().WithExpectedSequence(_testContext.CommandResult.Sequence));
}
