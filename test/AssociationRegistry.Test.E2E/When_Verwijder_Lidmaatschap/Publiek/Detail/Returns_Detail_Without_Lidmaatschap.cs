namespace AssociationRegistry.Test.E2E.When_Verwijder_Lidmaatschap.Publiek.Detail;

using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using Public.Api.Verenigingen.Detail.ResponseModels;
using Xunit;

[Collection(nameof(VerwijderLidmaatschapCollection))]
public class Returns_Detail_Without_Lidmaatschap : End2EndTest<PubliekVerenigingDetailResponse>
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

    public override PubliekVerenigingDetailResponse GetResponse(FullBlownApiSetup setup)
        => setup.PublicApiHost.GetPubliekDetail(_testContext.VCode);
}
