namespace AssociationRegistry.Test.E2E.When_Verwijder_Lidmaatschap.Publiek.Detail;

using FluentAssertions;
using Framework.AlbaHost;
using KellermanSoftware.CompareNetObjects;
using Public.Api.Verenigingen.Detail.ResponseModels;
using System;
using System.Threading.Tasks;
using Xunit;

[Collection(WellKnownCollections.VerwijderLidmaatschap)]
public class Returns_Detail_Without_Lidmaatschap : IClassFixture<VerwijderLidmaatschapContext>, IAsyncLifetime
{
    private readonly VerwijderLidmaatschapContext _context;

    public Returns_Detail_Without_Lidmaatschap(VerwijderLidmaatschapContext context)
    {
        _context = context;
    }

    [Fact]
    public void JsonContentMatches()
    {
        var comparisonConfig = new ComparisonConfig();
        comparisonConfig.MaxDifferences = 10;
        comparisonConfig.MaxMillisecondsDateDifference = (int)TimeSpan.FromSeconds(10).TotalMilliseconds;

        Response.Vereniging.Lidmaatschappen.Should().BeEmpty();
    }

    public PubliekVerenigingDetailResponse Response { get; set; }

    public async ValueTask InitializeAsync()
    {
        Response = _context.ApiSetup.PublicApiHost.GetPubliekDetail(_context.VCode);
    }

    public async ValueTask DisposeAsync()
    {
    }
}
