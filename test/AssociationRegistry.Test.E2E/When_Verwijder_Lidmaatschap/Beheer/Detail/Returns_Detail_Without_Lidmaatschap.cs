namespace AssociationRegistry.Test.E2E.When_Verwijder_Lidmaatschap.Beheer.Detail;

using Admin.Api.DecentraalBeheer.Verenigingen.Detail.ResponseModels;
using FluentAssertions;
using Framework.AlbaHost;
using KellermanSoftware.CompareNetObjects;
using Xunit;

[Collection(FullBlownApiCollection.Name)]
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

    public DetailVerenigingResponse Response { get; set; }

    public async Task InitializeAsync()
    {
        Response = _context.ApiSetup.AdminApiHost.GetBeheerDetail(_context.VCode);
    }

    public async Task DisposeAsync()
    {
    }
}
