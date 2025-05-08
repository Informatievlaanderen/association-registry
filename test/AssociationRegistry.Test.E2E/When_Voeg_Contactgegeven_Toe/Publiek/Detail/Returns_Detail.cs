namespace AssociationRegistry.Test.E2E.When_Voeg_Contactgegeven_Toe.Publiek.Detail;

using FluentAssertions;
using Framework.AlbaHost;
using JsonLdContext;
using Public.Api.Verenigingen.Detail.ResponseModels;
using Xunit;

[Collection(VoegContactgegevenToeContext.Name)]
public class Returns_Detail : IAsyncLifetime
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
        Response.Vereniging.Contactgegevens.Last()
                .Should().BeEquivalentTo(new Contactgegeven()
                 {
                     type = JsonLdType.Contactgegeven.Type,
                     id = JsonLdType.Contactgegeven.CreateWithIdValues(_context.VCode, nextContactgegevenId.ToString()),
                     Beschrijving = _context.Request.Contactgegeven.Beschrijving,
                     Contactgegeventype = _context.Request.Contactgegeven.Contactgegeventype,
                     IsPrimair = _context.Request.Contactgegeven.IsPrimair,
                     Waarde = _context.Request.Contactgegeven.Waarde
                 });
    }

    public PubliekVerenigingDetailResponse Response { get; set; }

    public async ValueTask InitializeAsync()
    {
        Response = _context.ApiSetup.PublicApiHost.GetPubliekDetail(_context.VCode);
    }

    public async ValueTask DisposeAsync()
    {
    }
}
