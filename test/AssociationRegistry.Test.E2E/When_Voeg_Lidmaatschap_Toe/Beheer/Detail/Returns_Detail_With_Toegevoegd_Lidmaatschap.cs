namespace AssociationRegistry.Test.E2E.When_Voeg_Lidmaatschap_Toe.Beheer.Detail;

using Admin.Api.Verenigingen.Detail.ResponseModels;
using Admin.ProjectionHost.Infrastructure.Extensions;
using Framework.AlbaHost;
using KellermanSoftware.CompareNetObjects;
using Xunit;
using Lidmaatschap = Admin.Api.Verenigingen.Detail.ResponseModels.Lidmaatschap;

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
            LidmaatschapId = 1,
            AndereVereniging = _context.Request.AndereVereniging,
            Beschrijving = _context.Request.Beschrijving,
            Van = _context.Request.Van.ToBelgianDate(),
            Tot = _context.Request.Tot.ToBelgianDate(),
            Identificatie = _context.Request.Identificatie,
            Naam = string.Empty, //TODO: fetch naam van andere vereniging
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
