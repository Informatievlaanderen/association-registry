namespace AssociationRegistry.Test.E2E.When_Corrigeer_Markeer_Als_Dubbel_Van.Acm;

using AssociationRegistry.Acm.Api.VerenigingenPerInsz;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Vereniging;
using Xunit;
using VerenigingStatus = AssociationRegistry.Acm.Schema.Constants.VerenigingStatus;
using Verenigingstype = Vereniging.Verenigingstype;

[Collection(nameof(CorrigeerMarkeringAlsDubbelVanCollection))]
public class Returns_VerenigingenPerInszResponse : End2EndTest<VerenigingenPerInszResponse>
{
    private readonly CorrigeerMarkeringAlsDubbelVanContext _testContext;
    private readonly VerenigingenPerInszRequest _request;
    private readonly string _inszToCompare;

    public Returns_VerenigingenPerInszResponse(CorrigeerMarkeringAlsDubbelVanContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
        _inszToCompare = testContext.Scenario.DubbeleVerenging.Vertegenwoordigers[0].Insz;

        _request = new VerenigingenPerInszRequest()
        {
            Insz = _inszToCompare,
            KboNummers = [],
        };
    }

    public override VerenigingenPerInszResponse GetResponse(FullBlownApiSetup setup)
        => setup.AcmApiHost.GetVerenigingenPerInsz(_request)
                .GetAwaiter().GetResult();

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
                    VCode = _testContext.Scenario.DubbeleVerenging.VCode,
                    CorresponderendeVCodes = [],
                    VertegenwoordigerId = _testContext.Scenario.DubbeleVerenging.Vertegenwoordigers[0].VertegenwoordigerId,
                    Naam = _testContext.Scenario.DubbeleVerenging.Naam,
                    Status = VerenigingStatus.Actief,
                    KboNummer = string.Empty,
                    Verenigingstype = new AssociationRegistry.Acm.Api.VerenigingenPerInsz.VerenigingenPerInszResponse.Verenigingstype(
                        Verenigingstype.VZER.Code,
                        Verenigingstype.VZER.Naam),
                    Verenigingssubtype =  new VerenigingenPerInszResponse.Verenigingssubtype()
                    {
                        Code = VerenigingssubtypeCode.Default.Code,
                        Naam = VerenigingssubtypeCode.Default.Naam,
                    },
                    IsHoofdvertegenwoordigerVan = true,
                },

                // Authentieke vereniging wordt getoond zonder corresponderende verenigingen dubbele verenigingen
                new VerenigingenPerInszResponse.Vereniging()
                {
                    VCode = _testContext.Scenario.AuthentiekeVereniging.VCode,
                    CorresponderendeVCodes = [],
                    VertegenwoordigerId = _testContext.Scenario.AuthentiekeVereniging.Vertegenwoordigers[0].VertegenwoordigerId,
                    Naam = _testContext.Scenario.AuthentiekeVereniging.Naam,
                    Status = VerenigingStatus.Actief,
                    KboNummer = string.Empty,
                    Verenigingstype = new AssociationRegistry.Acm.Api.VerenigingenPerInsz.VerenigingenPerInszResponse.Verenigingstype(
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
