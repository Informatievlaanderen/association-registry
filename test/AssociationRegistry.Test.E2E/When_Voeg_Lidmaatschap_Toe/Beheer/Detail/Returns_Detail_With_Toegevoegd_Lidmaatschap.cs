namespace AssociationRegistry.Test.E2E.When_Voeg_Lidmaatschap_Toe.Beheer.Detail;

using Admin.Api.DecentraalBeheer.Verenigingen.Detail.ResponseModels;
using Formats;
using Framework.AlbaHost;
using JsonLdContext;
using KellermanSoftware.CompareNetObjects;
using Xunit;
using Lidmaatschap = Admin.Api.DecentraalBeheer.Verenigingen.Detail.ResponseModels.Lidmaatschap;

[Collection(FullBlownApiCollection.Name)]
public class Returns_Detail_With_Toegevoegd_Lidmaatschap : IClassFixture<VoegLidmaatschapToeContext>, IAsyncLifetime
{
    private readonly VoegLidmaatschapToeContext _context;

    public Returns_Detail_With_Toegevoegd_Lidmaatschap(VoegLidmaatschapToeContext context)
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
            id = JsonLdType.Lidmaatschap.CreateWithIdValues(_context.VCode, "1"),
            type = JsonLdType.Lidmaatschap.Type,
            LidmaatschapId = 1,
            AndereVereniging = _context.Request.AndereVereniging,
            Beschrijving = _context.Request.Beschrijving,
            Van = _context.Request.Van.FormatAsBelgianDate(),
            Tot = _context.Request.Tot.FormatAsBelgianDate(),
            Identificatie = _context.Request.Identificatie,
            Naam = _context.Scenario.AndereFeitelijkeVerenigingWerdGeregistreerd.Naam,
        };

        Response.Vereniging.Lidmaatschappen.Single(x => x.LidmaatschapId == 1)
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
