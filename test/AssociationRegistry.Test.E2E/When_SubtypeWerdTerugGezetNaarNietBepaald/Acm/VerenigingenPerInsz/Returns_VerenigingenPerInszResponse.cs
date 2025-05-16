namespace AssociationRegistry.Test.E2E.When_SubtypeWerdTerugGezetNaarNietBepaald.Acm.VerenigingenPerInsz;

using AssociationRegistry.Acm.Api.VerenigingenPerInsz;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Vereniging;
using Xunit;
using VerenigingStatus = AssociationRegistry.Acm.Schema.Constants.VerenigingStatus;
using Verenigingstype = Vereniging.Verenigingstype;

[Collection(nameof(ZetSubtypeNaarNietBepaaldCollection))]
public class Returns_Detail : End2EndTest<VerenigingenPerInszResponse>
{
    private readonly ZetSubtypeNaarNietBepaaldContext _testContext;
    private readonly string _inszToCompare;
    private readonly VerenigingenPerInszRequest _request;

    public Returns_Detail(ZetSubtypeNaarNietBepaaldContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
        _inszToCompare = testContext.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Vertegenwoordigers[0].Insz;
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
                new VerenigingenPerInszResponse.Vereniging()
                {
                    VCode = _testContext.VCode,
                    CorresponderendeVCodes = [],
                    VertegenwoordigerId = _testContext.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Vertegenwoordigers[0].VertegenwoordigerId,
                    Naam = _testContext.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Naam,
                    Status = VerenigingStatus.Actief,
                    KboNummer = string.Empty,
                    Verenigingstype = new VerenigingenPerInszResponse.Verenigingstype(
                        Verenigingstype.VZER.Code,
                        Verenigingstype.VZER.Naam),
                    Verenigingssubtype =  new VerenigingenPerInszResponse.Verenigingssubtype()
                    {
                        Code = VerenigingssubtypeCode.NietBepaald.Code,
                        Naam = VerenigingssubtypeCode.NietBepaald.Naam,
                    },
                    IsHoofdvertegenwoordigerVan = true,
                },
            ],
            KboNummers = [],
        });
    }
}
