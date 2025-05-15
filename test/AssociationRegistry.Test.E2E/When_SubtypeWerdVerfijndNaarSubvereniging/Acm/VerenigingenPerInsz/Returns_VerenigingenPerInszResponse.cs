namespace AssociationRegistry.Test.E2E.When_SubtypeWerdVerfijndNaarSubvereniging.Acm.VerenigingenPerInsz;

using AssociationRegistry.Acm.Api.VerenigingenPerInsz;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using AssociationRegistry.Vereniging;
using KellermanSoftware.CompareNetObjects;
using Xunit;
using VerenigingStatus = AssociationRegistry.Acm.Schema.Constants.VerenigingStatus;
using Verenigingstype = Vereniging.Verenigingstype;

[Collection(nameof(VerfijnSubtypeNaarSubverenigingCollection))]
public class Returns_Vereniging : End2EndTest<VerenigingenPerInszResponse>
{
    private readonly VerfijnSubtypeNaarSubverenigingContext _testContext;
    private readonly string _inszToCompare;
    private readonly VerenigingenPerInszRequest _request;

    public Returns_Vereniging(VerfijnSubtypeNaarSubverenigingContext testContext) : base(testContext.ApiSetup)
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
        => setup.AcmApiHost.GetVerenigingenPerInsz(_request, _testContext.CommandResult.Sequence)
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
                    Verenigingstype = new AssociationRegistry.Acm.Api.VerenigingenPerInsz.VerenigingenPerInszResponse.Verenigingstype(
                        Verenigingstype.VZER.Code,
                        Verenigingstype.VZER.Naam),
                    Verenigingssubtype =  new VerenigingenPerInszResponse.Verenigingssubtype
                    {
                        Code = VerenigingssubtypeCode.Subvereniging.Code,
                        Naam = VerenigingssubtypeCode.Subvereniging.Naam,
                    },

                    IsHoofdvertegenwoordigerVan = true,
                },
            ],
            KboNummers = [],
        });
    }
}
