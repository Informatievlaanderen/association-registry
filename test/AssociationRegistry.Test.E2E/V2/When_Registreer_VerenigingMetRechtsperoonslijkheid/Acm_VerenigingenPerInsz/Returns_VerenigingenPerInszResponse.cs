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
    private readonly VerenigingenPerInszRequest _request;

    public Returns_VerenigingenPerInszResponse(RegistreerVerenigingMetRechtsperoonlijkheidTestContext context) : base(context)
    {
        _request = new VerenigingenPerInszRequest()
        {
            Insz = "0123456789",
            KboNummers =
            [
                new VerenigingenPerInszRequest.KboRequest()
                {
                    KboNummer = context.Request.KboNummer,
                    Rechtsvorm = "NVT",
                },
            ],
        };
    }

    [Fact]
    public void With_Context()
    {
        Response.ShouldCompare(new VerenigingenPerInszResponse()
        {
            Insz = _request.Insz,
            Verenigingen = [],
            KboNummers = _request.KboNummers.Select(s => new VerenigingenPerInszResponse.KboResponse()
                {
                    KboNummer = _request.KboNummers.First().KboNummer,
                    VCode = TestContext.RequestResult.VCode,
                    IsHoofdVertegenwoordiger = true,
                }).ToArray(),
        });
    }

    public override Func<IApiSetup, VerenigingenPerInszResponse> GetResponse
        => setup => setup.AcmApiHost.GetVerenigingenPerInsz(_request)
                         .GetAwaiter().GetResult();
}
