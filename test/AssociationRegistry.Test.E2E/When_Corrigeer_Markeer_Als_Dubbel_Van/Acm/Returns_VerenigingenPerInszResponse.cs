namespace AssociationRegistry.Test.E2E.When_Corrigeer_Markeer_Als_Dubbel_Van.Acm;

using AssociationRegistry.Acm.Api.VerenigingenPerInsz;
using AssociationRegistry.Acm.Schema.Constants;
using Framework.AlbaHost;
using KellermanSoftware.CompareNetObjects;
using Xunit;
using Verenigingstype = Vereniging.Verenigingstype;

[Collection(FullBlownApiCollection.Name)]
public class Returns_VerenigingenPerInszResponse :
    IClassFixture<CorrigeerMarkeringAlsDubbelVanContext>
{
    private readonly CorrigeerMarkeringAlsDubbelVanContext _context;
    private readonly string _inszToCompare;
    private readonly VerenigingenPerInszRequest _request;

    public Returns_VerenigingenPerInszResponse(CorrigeerMarkeringAlsDubbelVanContext context)
    {
        _context = context;
        _inszToCompare = context.Scenario.DubbeleVerenging.Vertegenwoordigers[0].Insz;
        _request = new VerenigingenPerInszRequest()
        {
            Insz = _inszToCompare,
            KboNummers = [],
        };
    }

    private VerenigingenPerInszResponse Response { get; set; }

    public async Task InitializeAsync()
    {
        Response = await _context.ApiSetup.AdminApiHost.GetVerenigingenPerInsz(_request);
    }

    [Fact]
    public void With_Verenigingen()
    {
        Response.ShouldCompare(new VerenigingenPerInszResponse()
        {
            Insz = _inszToCompare,
            Verenigingen =
            [
                // Dubbele vereniging wordt getoond
                new VerenigingenPerInszResponse.Vereniging()
                {
                    VCode = _context.Scenario.DubbeleVerenging.VCode,
                    CorresponderendeVCodes = [],
                    VertegenwoordigerId = _context.Scenario.DubbeleVerenging.Vertegenwoordigers[0].VertegenwoordigerId,
                    Naam = _context.Scenario.DubbeleVerenging.Naam,
                    Status = VerenigingStatus.Actief,
                    KboNummer = string.Empty,
                    Verenigingstype = new AssociationRegistry.Acm.Api.VerenigingenPerInsz.Verenigingstype(
                        Verenigingstype.FeitelijkeVereniging.Code,
                        Verenigingstype.FeitelijkeVereniging.Naam),
                    IsHoofdvertegenwoordigerVan = true,
                },

                // Authentieke vereniging wordt getoond zonder corresponderende verenigingen dubbele verenigingen
                new VerenigingenPerInszResponse.Vereniging()
                {
                    VCode = _context.Scenario.AuthentiekeVereniging.VCode,
                    CorresponderendeVCodes = [],
                    VertegenwoordigerId = _context.Scenario.AuthentiekeVereniging.Vertegenwoordigers[0].VertegenwoordigerId,
                    Naam = _context.Scenario.AuthentiekeVereniging.Naam,
                    Status = VerenigingStatus.Actief,
                    KboNummer = string.Empty,
                    Verenigingstype = new AssociationRegistry.Acm.Api.VerenigingenPerInsz.Verenigingstype(
                        Verenigingstype.FeitelijkeVereniging.Code,
                        Verenigingstype.FeitelijkeVereniging.Naam),
                    IsHoofdvertegenwoordigerVan = true,
                },
            ],
            KboNummers = [],
        });
    }
}
