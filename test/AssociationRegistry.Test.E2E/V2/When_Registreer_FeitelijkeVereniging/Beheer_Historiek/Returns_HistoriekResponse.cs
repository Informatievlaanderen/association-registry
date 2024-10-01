namespace AssociationRegistry.Test.E2E.V2.When_Registreer_FeitelijkeVereniging.Beheer_Historiek;

using Admin.Api.Verenigingen.Historiek.ResponseModels;
using Events;
using Framework.AlbaHost;
using Framework.Comparison;
using Framework.Mappers;
using KellermanSoftware.CompareNetObjects;
using Xunit;

[Collection(FullBlownApiCollection.Name)]
public class Returns_Historiek : IClassFixture<RegistreerFeitelijkeVerenigingContext>, IAsyncLifetime
{
    private readonly RegistreerFeitelijkeVerenigingContext _context;

    public Returns_Historiek(RegistreerFeitelijkeVerenigingContext context)
    {
        _context = context;
    }

    [Fact]
    public void With_VCode()
    {
        Response.VCode.ShouldCompare(_context.VCode);
    }

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/beheer/historiek-vereniging-context.json");
    }

    [Fact]
    public void With_All_Gebeurtenissen()
    {
        var werdGeregistreerd =
            Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(FeitelijkeVerenigingWerdGeregistreerd));

        werdGeregistreerd.ShouldCompare(HistoriekGebeurtenisMapper.FeitelijkeVerenigingWerdGeregistreerd(_context.Request, _context.VCode),
                                        compareConfig: HistoriekComparisonConfig.Instance);

    }

    public async Task InitializeAsync()
    {
        Response = _context.ApiSetup.AdminApiHost.GetHistoriek(_context.VCode);
    }

    public HistoriekResponse Response { get; set; }

    public async Task DisposeAsync()
    {
    }
}
