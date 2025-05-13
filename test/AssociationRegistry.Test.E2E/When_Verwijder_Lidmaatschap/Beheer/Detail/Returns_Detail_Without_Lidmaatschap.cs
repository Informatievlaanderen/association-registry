namespace AssociationRegistry.Test.E2E.When_Verwijder_Lidmaatschap.Detail;

using Admin.Api.Verenigingen.Detail.ResponseModels;
using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
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

    public override DetailVerenigingResponse GetResponse(FullBlownApiSetup setup)
        => setup.AdminApiHost.GetBeheerDetail(_testContext.VCode);
}
