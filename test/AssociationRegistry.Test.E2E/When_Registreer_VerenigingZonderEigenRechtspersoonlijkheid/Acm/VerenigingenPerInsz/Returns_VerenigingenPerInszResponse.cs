namespace AssociationRegistry.Test.E2E.When_Registreer_VerenigingZonderEigenRechtspersoonlijkheid.Acm.VerenigingenPerInsz;

using AssociationRegistry.Acm.Api.WebApi.VerenigingenPerInsz;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Mappers;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Vereniging;
using Xunit;
using VerenigingStatus = AssociationRegistry.Acm.Schema.Constants.VerenigingStatus;
using Verenigingstype = DecentraalBeheer.Vereniging.Verenigingstype;

[Collection(nameof(RegistreerVerenigingZonderEigenRechtspersoonlijkheidCollection))]
public class Returns_VerenigingenPerInszResponse : End2EndTest<VerenigingenPerInszResponse>
{
    private readonly RegistreerVerenigingZonderEigenRechtspersoonlijkheidContext _testContext;

    public Returns_VerenigingenPerInszResponse(RegistreerVerenigingZonderEigenRechtspersoonlijkheidContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override VerenigingenPerInszResponse GetResponse(FullBlownApiSetup setup)
        => setup.AcmApiHost.GetVerenigingenPerInsz(new VerenigingenPerInszRequest()
                 {
                     Insz = _testContext.CommandRequest.Vertegenwoordigers[0].Insz,
                     KboNummers = [],
                 }, _testContext.CommandResult.Sequence)
                .GetAwaiter().GetResult();

    [Fact]
    public void With_Verenigingen()
    {
        Response.ShouldCompare(new VerenigingenPerInszResponse()
        {
            Insz = _testContext.CommandRequest.Vertegenwoordigers[0].Insz,
            Verenigingen =
            [
                new VerenigingenPerInszResponse.Vereniging()
                {
                    VCode = _testContext.VCode,
                    CorresponderendeVCodes = [],
                    VertegenwoordigerId = 1,
                    Naam = _testContext.CommandRequest.Naam,
                    Status = VerenigingStatus.Actief,
                    KboNummer = string.Empty,
                    Verenigingstype = new VerenigingenPerInszResponse.Verenigingstype(
                        Verenigingstype.VZER.Code,
                        Verenigingstype.VZER.Naam),
                    Verenigingssubtype =  VerenigingssubtypeCode.Default.Map<VerenigingenPerInszResponse.Verenigingssubtype>(),
                    IsHoofdvertegenwoordigerVan = true,
                },
            ],
            KboNummers = [],
        });
    }
}
