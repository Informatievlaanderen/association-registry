namespace AssociationRegistry.Test.E2E.When_Corrigeer_Markeer_Als_Dubbel_Van.Acm;

using AssociationRegistry.Acm.Api.VerenigingenPerInsz;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Scenarios.Requests;
using Vereniging;
using Xunit;
using VerenigingStatus = AssociationRegistry.Acm.Schema.Constants.VerenigingStatus;
using Verenigingstype = Vereniging.Verenigingstype;

[Collection(FullBlownApiCollection.Name)]
public class Returns_VerenigingenPerInszResponse :
    End2EndTest<CorrigeerMarkeringAlsDubbelVanContext, NullRequest, VerenigingenPerInszResponse>
{
    private readonly CorrigeerMarkeringAlsDubbelVanContext _context;
    private readonly string _inszToCompare;
    private readonly VerenigingenPerInszRequest _request;

    public Returns_VerenigingenPerInszResponse(CorrigeerMarkeringAlsDubbelVanContext context): base(context)
    {
        _context = context;
        _inszToCompare = context.Scenario.DubbeleVerenging.Vertegenwoordigers[0].Insz;

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
                // Dubbele vereniging wordt getoond
                new VerenigingenPerInszResponse.Vereniging()
                {
                    VCode = _context.Scenario.DubbeleVerenging.VCode,
                    CorresponderendeVCodes = [],
                    VertegenwoordigerId = _context.Scenario.DubbeleVerenging.Vertegenwoordigers[0].VertegenwoordigerId,
                    Naam = _context.Scenario.DubbeleVerenging.Naam,
                    Status = VerenigingStatus.Actief,
                    KboNummer = string.Empty,
                    Verenigingstype = new AssociationRegistry.Acm.Api.VerenigingenPerInsz.VerenigingenPerInszResponse.Verenigingstype(
                        Verenigingstype.VZER.Code,
                        Verenigingstype.VZER.Naam),
                    Verenigingssubtype =  new VerenigingenPerInszResponse.Verenigingssubtype()
                    {
                        Code = Verenigingssubtype.NogNietBepaald.Code,
                        Naam = Verenigingssubtype.NogNietBepaald.Naam,
                    },
                    IsHoofdvertegenwoordigerVan = true,
                },

                // Authentieke vereniging wordt getoond zonder corresponderende verenigingen dubbele verenigingen
                new VerenigingenPerInszResponse.Vereniging()
                {
                    VCode = _context.Scenario.AuthentiekeVereniging.VCode,
                    CorresponderendeVCodes = [],
                    VertegenwoordigerId = _context.Scenario.AuthentiekeVereniging.Vertegenwoordigers[0].VertegenwoordigerId,
                    Naam = _context.Scenario.AuthentiekeVereniging.Naam,
                    Status = VerenigingStatus.Actief,
                    KboNummer = string.Empty,
                    Verenigingstype = new AssociationRegistry.Acm.Api.VerenigingenPerInsz.VerenigingenPerInszResponse.Verenigingstype(
                        Verenigingstype.VZER.Code,
                        Verenigingstype.VZER.Naam),
                    Verenigingssubtype =  new VerenigingenPerInszResponse.Verenigingssubtype()
                    {
                        Code = Verenigingssubtype.NogNietBepaald.Code,
                        Naam = Verenigingssubtype.NogNietBepaald.Naam,
                    },
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
