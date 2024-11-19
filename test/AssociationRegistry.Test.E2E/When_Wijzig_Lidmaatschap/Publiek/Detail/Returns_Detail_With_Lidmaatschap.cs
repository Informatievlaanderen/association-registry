namespace AssociationRegistry.Test.E2E.When_Wijzig_Lidmaatschap.Publiek.Detail;

using Admin.ProjectionHost.Infrastructure.Extensions;
using AssociationRegistry.Public.Api.Verenigingen.Detail.ResponseModels;
using AssociationRegistry.Public.ProjectionHost.Infrastructure.Extensions;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using Formats;
using JsonLdContext;
using KellermanSoftware.CompareNetObjects;
using Xunit;

[Collection(FullBlownApiCollection.Name)]
public class Returns_Detail_With_Lidmaatschap : IClassFixture<WijzigLidmaatschapContext>, IAsyncLifetime
{
    private readonly WijzigLidmaatschapContext _context;

    public Returns_Detail_With_Lidmaatschap(WijzigLidmaatschapContext context)
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
            id = JsonLdType.Lidmaatschap.CreateWithIdValues(_context.VCode, _context.Scenario.LidmaatschapWerdToegevoegd.Lidmaatschap.LidmaatschapId.ToString()),
            type = JsonLdType.Lidmaatschap.Type,
            AndereVereniging = _context.Scenario.LidmaatschapWerdToegevoegd.Lidmaatschap.AndereVereniging,
            Beschrijving = _context.Request.Beschrijving,
            Van = _context.Request.Van.Value.FormatAsBelgianDate(),
            Tot = _context.Request.Tot.Value.FormatAsBelgianDate(),
            Identificatie = _context.Request.Identificatie,
            Naam = _context.Scenario.BaseScenario.AndereFeitelijkeVerenigingWerdGeregistreerd.Naam,
        };

        Response.Vereniging.Lidmaatschappen.Single(x => x.id == JsonLdType.Lidmaatschap.CreateWithIdValues(_context.VCode, _context.Scenario.LidmaatschapWerdToegevoegd.Lidmaatschap.LidmaatschapId.ToString()))
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
