namespace AssociationRegistry.Test.E2E.When_Markeer_Als_Dubbel_Van.Acm;

using AssociationRegistry.Acm.Api.WebApi.VerenigingenPerInsz;
using DecentraalBeheer.Vereniging;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Vereniging;
using Xunit;
using VerenigingStatus = AssociationRegistry.Acm.Schema.Constants.VerenigingStatus;

[Collection(nameof(MarkeerAlsDubbelVanCollection))]
public class Returns_Vereniging : End2EndTest<VerenigingenPerInszResponse>
{
    private readonly MarkeerAlsDubbelVanContext _testContext;
    private readonly string _inszToCompare;
    private readonly VerenigingenPerInszRequest _request;

    public Returns_Vereniging(MarkeerAlsDubbelVanContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
        _inszToCompare = testContext.Scenario.FeitelijkeVerenigingWerdGeristreerdMetPersoonsgegevens.Vertegenwoordigers[0].VertegenwoordigerPersoonsgegevens.Insz;
        _request = new VerenigingenPerInszRequest()
        {
            Insz = _inszToCompare,
            KboNummers = [],
        };
    }

    public override async Task<VerenigingenPerInszResponse> GetResponse(FullBlownApiSetup setup)
        => await setup.AcmApiHost.GetVerenigingenPerInsz(_request, _testContext.VerenigingAanvaarddeDubbeleVereniging.Sequence);
    [Fact]
    public void With_Verenigingen()
    {
        Response.ShouldCompare(new VerenigingenPerInszResponse()
        {
            Insz = _inszToCompare,
            Verenigingen =
            [
                // Dubbele vereniging wordt niet getoond

                // Authentieke vereniging wordt getoond met corresponderende verenigingen
                new VerenigingenPerInszResponse.Vereniging()
                {
                    VCode = _testContext.Scenario.AndereFeitelijkeVerenigingWerdGeristreerdMetPersoonsgegevens.VCode,
                    CorresponderendeVCodes = [_testContext.Scenario.FeitelijkeVerenigingWerdGeristreerdMetPersoonsgegevens.VCode],
                    VertegenwoordigerId = _testContext.Scenario.AndereFeitelijkeVerenigingWerdGeristreerdMetPersoonsgegevens.Vertegenwoordigers[0].VertegenwoordigerId,
                    Naam = _testContext.Scenario.AndereFeitelijkeVerenigingWerdGeristreerdMetPersoonsgegevens.Naam,
                    Status = VerenigingStatus.Actief,
                    KboNummer = string.Empty,
                    Verenigingstype = new VerenigingenPerInszResponse.Verenigingstype(
                        Verenigingstype.VZER.Code,
                        Verenigingstype.VZER.Naam),
                    Verenigingssubtype =  new VerenigingenPerInszResponse.Verenigingssubtype()
                    {
                        Code = VerenigingssubtypeCode.Default.Code,
                        Naam = VerenigingssubtypeCode.Default.Naam,
                    },
                    IsHoofdvertegenwoordigerVan = true,
                },
            ],
            KboNummers = [],
        });
    }
}
