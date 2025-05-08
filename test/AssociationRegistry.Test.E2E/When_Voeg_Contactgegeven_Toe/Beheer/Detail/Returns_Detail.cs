namespace AssociationRegistry.Test.E2E.When_Voeg_Contactgegeven_Toe.Beheer.Detail;

using Admin.Api.Verenigingen.Detail.ResponseModels;
using Be.Vlaanderen.Basisregisters.Utilities;
using FluentAssertions;
using Framework.AlbaHost;
using JsonLdContext;
using Vereniging.Bronnen;
using Xunit;

[Collection(FullBlownApiCollection.Name)]
public class Returns_Detail : IClassFixture<VoegContactgegevenToeContext>, IAsyncLifetime
{
    private readonly VoegContactgegevenToeContext _context;

    public Returns_Detail(VoegContactgegevenToeContext context)
    {
        _context = context;
    }

    [Fact]
    public void JsonContentMatches()
    {
        var nextContactgegevenId = _context.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd
                                           .Contactgegevens.Max(x => x.ContactgegevenId) + 1;
        Response.Vereniging.Contactgegevens.Single(x => x.ContactgegevenId == nextContactgegevenId)
                .Should().BeEquivalentTo(new Contactgegeven()
                 {
                     type = JsonLdType.Contactgegeven.Type,
                     id = JsonLdType.Contactgegeven.CreateWithIdValues(_context.VCode, nextContactgegevenId.ToString()),
                     ContactgegevenId = nextContactgegevenId,
                     Beschrijving = _context.Request.Contactgegeven.Beschrijving,
                     Bron = Bron.Initiator,
                     Contactgegeventype = _context.Request.Contactgegeven.Contactgegeventype,
                     IsPrimair = _context.Request.Contactgegeven.IsPrimair,
                     Waarde = _context.Request.Contactgegeven.Waarde
                 });
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
