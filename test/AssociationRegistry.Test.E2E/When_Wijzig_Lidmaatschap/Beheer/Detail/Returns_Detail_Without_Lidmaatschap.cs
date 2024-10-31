namespace AssociationRegistry.Test.E2E.When_Wijzig_Lidmaatschap.Beheer.Detail;

using Admin.Api.Verenigingen.Detail.ResponseModels;
using Admin.ProjectionHost.Infrastructure.Extensions;
using Framework.AlbaHost;
using KellermanSoftware.CompareNetObjects;
using Xunit;

[Collection(FullBlownApiCollection.Name)]
public class Returns_Detail_Without_Lidmaatschap : IClassFixture<WijzigLidmaatschapContext>, IAsyncLifetime
{
    private readonly WijzigLidmaatschapContext _context;

    public Returns_Detail_Without_Lidmaatschap(WijzigLidmaatschapContext context)
    {
        _context = context;
    }

    [Fact]
    public void JsonContentMatches()
    {
        var comparisonConfig = new ComparisonConfig();
        comparisonConfig.MaxDifferences = 10;
        comparisonConfig.MaxMillisecondsDateDifference = (int)TimeSpan.FromSeconds(10).TotalMilliseconds;

        var expected = new Lidmaatschap
        {
            LidmaatschapId = _context.Scenario.LidmaatschapWerdToegevoegd.Lidmaatschap.LidmaatschapId,
            AndereVereniging = _context.Scenario.LidmaatschapWerdToegevoegd.Lidmaatschap.AndereVereniging,
            Beschrijving = _context.Request.Beschrijving,
            Van = _context.Request.Van.ToBelgianDate(),
            Tot = _context.Request.Tot.ToBelgianDate(),
            Identificatie = _context.Request.Identificatie,
            Naam = _context.Scenario.NaamVereniging,
        };

        Response.Vereniging.Lidmaatschappen.Single(x => x.LidmaatschapId == _context.Scenario.LidmaatschapWerdToegevoegd.Lidmaatschap.LidmaatschapId)
                .ShouldCompare(expected, compareConfig: comparisonConfig);
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
