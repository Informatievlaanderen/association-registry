namespace AssociationRegistry.Test.E2E.When_SubtypeWerdVerfijndNaarFeitelijkeVereniging.Acm.VerenigingenPerInsz;

using Admin.Api.Verenigingen.Subtype.RequestModels;
using AssociationRegistry.Acm.Api.VerenigingenPerInsz;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Vereniging;
using Xunit;
using VerenigingStatus = AssociationRegistry.Acm.Schema.Constants.VerenigingStatus;
using Verenigingstype = Vereniging.Verenigingstype;

[Collection(FullBlownApiCollection.Name)]
public class Returns_VerenigingenPerInszResponse :
    End2EndTest<VerfijnSubtypeNaarFeitelijkeVerenigingContext, WijzigSubtypeRequest, VerenigingenPerInszResponse>
{
    private readonly string _inszToCompare;
    private readonly VerenigingenPerInszRequest _request;

    public Returns_VerenigingenPerInszResponse(VerfijnSubtypeNaarFeitelijkeVerenigingContext testContext)
    {
        _inszToCompare = TestContext.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Vertegenwoordigers[0].Insz;
        _request = new VerenigingenPerInszRequest()
        {
            Insz = _inszToCompare,
            KboNummers = [],
        };
    }

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
                    VCode = TestContext.VCode,
                    CorresponderendeVCodes = [],
                    VertegenwoordigerId = TestContext.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Vertegenwoordigers[0].VertegenwoordigerId,
                    Naam = TestContext.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Naam,
                    Status = VerenigingStatus.Actief,
                    KboNummer = string.Empty,
                    Verenigingstype = new AssociationRegistry.Acm.Api.VerenigingenPerInsz.VerenigingenPerInszResponse.Verenigingstype(
                        Verenigingstype.VZER.Code,
                        Verenigingstype.VZER.Naam),
                    Verenigingssubtype =  new VerenigingenPerInszResponse.Verenigingssubtype()
                    {
                        Code = VerenigingssubtypeCode.FeitelijkeVereniging.Code,
                        Naam = VerenigingssubtypeCode.FeitelijkeVereniging.Naam,
                    },
                    IsHoofdvertegenwoordigerVan = true,
                },
            ],
            KboNummers = [],
        });
    }

    public override Func<IApiSetup, VerenigingenPerInszResponse> GetResponse
        => setup => setup.AcmApiHost.GetVerenigingenPerInsz(_request)
                         .GetAwaiter().GetResult();
}
