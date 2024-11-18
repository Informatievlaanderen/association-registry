namespace AssociationRegistry.Test.E2E.When_Voeg_Lidmaatschap_Toe.Publiek.Detail;

using Formats;
using Framework.AlbaHost;
using JsonLdContext;
using KellermanSoftware.CompareNetObjects;
using Public.Api.Verenigingen.Detail.ResponseModels;
using Public.ProjectionHost.Infrastructure.Extensions;
using Xunit;
using Lidmaatschap = Public.Api.Verenigingen.Detail.ResponseModels.Lidmaatschap;

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
            id = JsonLdType.Lidmaatschap.CreateWithIdValues(_context.Request.AndereVereniging),
            type = JsonLdType.Lidmaatschap.Type,
            //LidmaatschapId = 1,
            AndereVereniging = _context.Request.AndereVereniging,
            Beschrijving = _context.Request.Beschrijving,
            Van = _context.Request.Van.FormatAsBelgianDate(),
            Tot = _context.Request.Tot.FormatAsBelgianDate(),
            Identificatie = _context.Request.Identificatie,
            Naam = _context.Scenario.AndereFeitelijkeVerenigingWerdGeregistreerd.Naam,
        };

        Response.Vereniging.Lidmaatschappen.Single(x => x.id == JsonLdType.Lidmaatschap.CreateWithIdValues(_context.Request.AndereVereniging))
                .ShouldCompare(expected, compareConfig: comparisonConfig);
    }

    public PubliekVerenigingDetailResponse Response { get; set; }

    public async Task InitializeAsync()
    {
        Response = _context.ApiSetup.PublicApiHost.GetPubliekDetail(_context.VCode);
    }

    public async Task DisposeAsync()
    {
    }
}
