namespace AssociationRegistry.Test.E2E.When_Registreer_VerenigingMetRechtsperoonslijkheid.Acm_VerenigingenPerInsz;

using Acm.Api.VerenigingenPerInsz;
using AcmBevraging;
using Admin.Api.Verenigingen.Registreer.MetRechtspersoonlijkheid.RequestModels;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Xunit;

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
                new VerenigingenPerInszRequest.KboNummerMetRechtsvormRequest()
                {
                    KboNummer = context.Request.KboNummer,
                    Rechtsvorm = "ONGEKENDE RECHTSVORM", // TODO: Veranderen naar happy path
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
            KboNummers = _request.KboNummers.Select(s => new VerenigingenPerInszResponse.VerenigingenPerKbo()
                {
                    KboNummer = _request.KboNummers.First().KboNummer,
                    VCode = VerenigingenPerKbo.VCodeUitzonderingen.NietVanToepassing,
                    IsHoofdVertegenwoordiger = false,
                }).ToArray(),
        });
    }

    public override Func<IApiSetup, VerenigingenPerInszResponse> GetResponse
        => setup => setup.AcmApiHost.GetVerenigingenPerInsz(_request)
                         .GetAwaiter().GetResult();
}
