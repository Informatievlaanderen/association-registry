namespace AssociationRegistry.Test.E2E.When_Verwijder_Lidmaatschap.Beheer.Detail;

using Admin.Api.Verenigingen.Detail.ResponseModels;
using FluentAssertions;
using Framework.AlbaHost;
using KellermanSoftware.CompareNetObjects;
using Xunit;

[Collection(VerwijderLidmaatschapContext.Name)]
public class Returns_Detail_Without_Lidmaatschap : IAsyncLifetime
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

    public DetailVerenigingResponse Response { get; set; }

    public async ValueTask InitializeAsync()
    {
        Response = _context.ApiSetup.AdminApiHost.GetBeheerDetail(_context.VCode);
    }

    public async ValueTask DisposeAsync()
    {
    }
}
