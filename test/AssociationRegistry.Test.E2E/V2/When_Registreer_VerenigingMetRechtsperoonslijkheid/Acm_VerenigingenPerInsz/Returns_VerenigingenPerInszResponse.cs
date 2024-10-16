namespace AssociationRegistry.Test.E2E.V2.When_Registreer_VerenigingMetRechtsperoonslijkheid.Acm_VerenigingenPerInsz;

using Acm.Api.VerenigingenPerInsz;
using Acm.Schema.Constants;
using Admin.Api.Verenigingen.Registreer.MetRechtspersoonlijkheid.RequestModels;
using Events;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Xunit;
using Verenigingstype = Vereniging.Verenigingstype;

[Collection(FullBlownApiCollection.Name)]
public class Returns_VerenigingenPerInszResponse :
    End2EndTest<RegistreerVerenigingMetRechtsperoonlijkheidTestContext, RegistreerVerenigingUitKboRequest, VerenigingenPerInszResponse>
{
    private readonly string _inszToCompare;
    private readonly FeitelijkeVerenigingWerdGeregistreerd _geregistreerdeVereniging;

    public Returns_VerenigingenPerInszResponse(RegistreerVerenigingMetRechtsperoonlijkheidTestContext context) : base(context)
    {
        _geregistreerdeVereniging = context.RegistratieData;
        _inszToCompare = _geregistreerdeVereniging.Vertegenwoordigers[0].Insz;
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
                    Naam = _geregistreerdeVereniging.Naam,
                    Status = VerenigingStatus.Actief,
                    KboNummer = string.Empty,
                    Verenigingstype = new Acm.Api.VerenigingenPerInsz.Verenigingstype(
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
