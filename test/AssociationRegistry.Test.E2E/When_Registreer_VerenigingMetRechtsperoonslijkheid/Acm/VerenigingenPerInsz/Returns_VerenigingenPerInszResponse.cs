namespace AssociationRegistry.Test.E2E.When_Registreer_VerenigingMetRechtsperoonslijkheid.Acm.VerenigingenPerInsz;

using AssociationRegistry.Acm.Api.Queries.VerenigingenPerKbo;
using AssociationRegistry.Acm.Api.WebApi.VerenigingenPerInsz;
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

    public override async Task<VerenigingenPerInszResponse> GetResponse(FullBlownApiSetup setup)
        => await setup.AcmApiHost.GetVerenigingenPerInsz(_request, _testContext.CommandResult.Sequence);

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
