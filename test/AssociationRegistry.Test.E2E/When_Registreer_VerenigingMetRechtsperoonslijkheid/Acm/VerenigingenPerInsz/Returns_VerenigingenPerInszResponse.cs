namespace AssociationRegistry.Test.E2E.When_Registreer_VerenigingMetRechtsperoonslijkheid.Acm.VerenigingenPerInsz;

using AcmBevraging;
using AssociationRegistry.Acm.Api.VerenigingenPerInsz;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Xunit;

[Collection(nameof(RegistreerVerenigingMetRechtsperoonlijkheidCollection))]
public class Returns_Vereniging : End2EndTest<VerenigingenPerInszResponse>
{
    private readonly RegistreerVerenigingMetRechtsperoonlijkheidContext _testContext;
    private readonly VerenigingenPerInszRequest _request;

    public Returns_Vereniging(RegistreerVerenigingMetRechtsperoonlijkheidContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
        _request = new VerenigingenPerInszRequest()
        {
            Insz = "0123456789",
            KboNummers =
            [
                new VerenigingenPerInszRequest.KboNummerMetRechtsvormRequest()
                {
                    KboNummer = testContext.CommandRequest.KboNummer,
                    Rechtsvorm = "ONGEKENDE RECHTSVORM", // TODO: Veranderen naar happy path
                },
            ],
        };
    }

    public override VerenigingenPerInszResponse GetResponse(FullBlownApiSetup setup)
        => setup.AcmApiHost.GetVerenigingenPerInsz(_request)
                .GetAwaiter().GetResult();

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
}
