namespace AssociationRegistry.Test.E2E.When_Markeer_Als_Dubbel_Van.Acm;

using Admin.Api.Verenigingen.Dubbelbeheer.FeitelijkeVereniging.MarkeerAlsDubbelVan.RequestModels;
using AssociationRegistry.Acm.Api.VerenigingenPerInsz;
using AssociationRegistry.Acm.Schema.Constants;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Xunit;

[Collection(FullBlownApiCollection.Name)]
public class Returns_VerenigingenPerInszResponse :
    End2EndTest<MarkeerAlsDubbelVanContext, MarkeerAlsDubbelVanRequest, VerenigingenPerInszResponse>
{
    private readonly string _inszToCompare;
    private readonly VerenigingenPerInszRequest _request;

    public Returns_VerenigingenPerInszResponse(MarkeerAlsDubbelVanContext testContext) : base(testContext)
    {
        _inszToCompare = TestContext.Scenario.FeitelijkeVerenigingWerdGeregistreerd.Vertegenwoordigers[0].Insz;
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
                // Dubbele vereniging wordt niet getoond

                // Authentieke vereniging wordt getoond met corresponderende verenigingen
                new VerenigingenPerInszResponse.Vereniging()
                {
                    VCode = TestContext.Scenario.AndereFeitelijkeVerenigingWerdGeregistreerd.VCode,
                    CorresponderendeVCodes = [TestContext.Scenario.FeitelijkeVerenigingWerdGeregistreerd.VCode],
                    VertegenwoordigerId = TestContext.Scenario.AndereFeitelijkeVerenigingWerdGeregistreerd.Vertegenwoordigers[0].VertegenwoordigerId,
                    Naam = TestContext.Scenario.AndereFeitelijkeVerenigingWerdGeregistreerd.Naam,
                    Status = VerenigingStatus.Actief,
                    KboNummer = string.Empty,
                    Verenigingstype = new VerenigingenPerInszResponse.Verenigingstype(
                        Vereniging.Verenigingstype.FeitelijkeVereniging.Code,
                        Vereniging.Verenigingstype.FeitelijkeVereniging.Naam),
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
