namespace AssociationRegistry.Test.E2E.V2.When_Registreer_FeitelijkeVereniging.Acm_VerenigingenPerInsz;

using Acm.Api.VerenigingenPerInsz;
using Acm.Schema.Constants;
using Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Xunit;
using Verenigingstype = Vereniging.Verenigingstype;

[Collection(FullBlownApiCollection.Name)]
public class Returns_VerenigingenPerInszResponse :
    End2EndTest<RegistreerFeitelijkeVerenigingTestContext, RegistreerFeitelijkeVerenigingRequest, VerenigingenPerInszResponse>
{
    private readonly string _inszToCompare;

    public Returns_VerenigingenPerInszResponse(RegistreerFeitelijkeVerenigingTestContext testContext) : base(testContext)
    {
        _inszToCompare = TestContext.Request.Vertegenwoordigers[0].Insz;
    }

    [Fact]
    public void With_Context()
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
                    VertegenwoordigerId = 1,
                    Naam = TestContext.Request.Naam,
                    Status = VerenigingStatus.Actief,
                    KboNummer = string.Empty,
                    Verenigingstype = new AssociationRegistry.Acm.Api.VerenigingenPerInsz.Verenigingstype(
                        Verenigingstype.FeitelijkeVereniging.Code,
                        Verenigingstype.FeitelijkeVereniging.Naam),
                    IsHoofdvertegenwoordigerVan = true,
                },
            ],
        });
    }

    public override Func<IApiSetup, VerenigingenPerInszResponse> GetResponse
        => setup => setup.AcmApiHost.GetVerenigingenPerInsz(_inszToCompare)
                         .GetAwaiter().GetResult();
}
