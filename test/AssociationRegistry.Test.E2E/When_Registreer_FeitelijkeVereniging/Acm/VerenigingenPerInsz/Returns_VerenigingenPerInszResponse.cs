namespace AssociationRegistry.Test.E2E.When_Registreer_FeitelijkeVereniging.Acm.VerenigingenPerInsz;

using AssociationRegistry.Acm.Api.VerenigingenPerInsz;
using AssociationRegistry.Acm.Schema.Constants;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Xunit;
using Verenigingstype = Vereniging.Verenigingstype;

[Collection(FullBlownApiCollection.Name)]
public class Returns_VerenigingenPerInszResponse :
    End2EndTest<RegistreerFeitelijkeVerenigingTestContext, RegistreerFeitelijkeVerenigingRequest, VerenigingenPerInszResponse>
{
    private readonly string _inszToCompare;
    private readonly VerenigingenPerInszRequest _request;

    public Returns_VerenigingenPerInszResponse(RegistreerFeitelijkeVerenigingTestContext testContext) : base(testContext)
    {
        _inszToCompare = TestContext.Request.Vertegenwoordigers[0].Insz;
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
            KboNummers = [],
        });
    }

    public override Func<IApiSetup, VerenigingenPerInszResponse> GetResponse
        => setup => setup.AcmApiHost.GetVerenigingenPerInsz(_request)
                         .GetAwaiter().GetResult();
}
