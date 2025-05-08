namespace AssociationRegistry.Test.E2E.When_Stop_Vereniging.Beheer.Detail;

using Admin.Api.Verenigingen.Detail.ResponseModels;
using FluentAssertions;
using Formats;
using Framework.AlbaHost;
using Vereniging;
using Xunit;

[Collection(FullBlownApiCollection.Name)]
public class Returns_Detail : IClassFixture<StopVerenigingContext>, IAsyncLifetime
{
    private readonly StopVerenigingContext _context;

    public Returns_Detail(StopVerenigingContext context)
    {
        _context = context;
    }

    [Fact]
    public void JsonContentMatches()
    {
       Response.Vereniging.Einddatum.Should().BeEquivalentTo(_context.Request.Einddatum.FormatAsBelgianDate());
       Response.Vereniging.Status.Should().BeEquivalentTo(VerenigingStatus.Gestopt.StatusNaam);
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
