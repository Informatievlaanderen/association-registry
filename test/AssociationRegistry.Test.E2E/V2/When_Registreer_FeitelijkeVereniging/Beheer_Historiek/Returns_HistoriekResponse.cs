namespace AssociationRegistry.Test.E2E.V2.When_Registreer_FeitelijkeVereniging.Beheer_Historiek;

using Alba;
using AssociationRegistry.Admin.Api.Verenigingen.Historiek.ResponseModels;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using AssociationRegistry.Events;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.Comparison;
using AssociationRegistry.Test.E2E.Framework.Mappers;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using Beheer_Detail;
using E2E.When_Registreer_FeitelijkeVereniging;
using KellermanSoftware.CompareNetObjects;
using Xunit;

[Collection(RegistreerVerenigingAdminCollection2.Name)]
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
