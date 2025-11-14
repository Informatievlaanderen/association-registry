namespace AssociationRegistry.Test.E2E.When_Voeg_Lidmaatschap_Toe.Publiek.Detail;

using Contracts.JsonLdContext;
using Formats;
using Framework.AlbaHost;
using KellermanSoftware.CompareNetObjects;
using Public.Api.WebApi.Verenigingen.Detail.ResponseModels;
using Xunit;
using Lidmaatschap = Public.Api.WebApi.Verenigingen.Detail.ResponseModels.Lidmaatschap;

[Collection(nameof(VoegLidmaatschapToeCollection))]
public class Returns_Detail_With_Toegevoegd_Lidmaatschap : IAsyncLifetime
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
            //LidmaatschapId = 1,
            AndereVereniging = _context.CommandRequest.AndereVereniging,
            Beschrijving = _context.CommandRequest.Beschrijving,
            Van = _context.CommandRequest.Van.FormatAsBelgianDate(),
            Tot = _context.CommandRequest.Tot.FormatAsBelgianDate(),
            Identificatie = _context.CommandRequest.Identificatie,
            Naam = _context.Scenario.AndereFeitelijkeVerenigingWerdGeristreerdMetPersoonsgegevens.Naam,
        };

        Response.Vereniging.Lidmaatschappen.Single(x => x.id == JsonLdType.Lidmaatschap.CreateWithIdValues(_context.VCode, "1"))
                .ShouldCompare(expected, compareConfig: comparisonConfig);
    }

    public PubliekVerenigingDetailResponse Response { get; set; }

    public async ValueTask InitializeAsync()
    {
        Response = await _context.ApiSetup.PublicApiHost.GetPubliekDetail(_context.VCode);
    }

    public async ValueTask DisposeAsync()
    {
    }
}
