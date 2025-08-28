namespace AssociationRegistry.Test.E2E.When_SubtypeWerdVerfijndNaarSubvereniging.Acm.VerenigingenPerInsz;

using AssociationRegistry.Acm.Api.WebApi.VerenigingenPerInsz;
using AssociationRegistry.Acm.Schema.VerenigingenPerInsz;
using DecentraalBeheer.Vereniging;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Marten;
using Newtonsoft.Json;
using Xunit;
using VerenigingStatus = AssociationRegistry.Acm.Schema.Constants.VerenigingStatus;
using Verenigingstype = DecentraalBeheer.Vereniging.Verenigingstype;

[Collection(nameof(VerfijnSubtypeNaarSubverenigingCollection))]
public class Returns_Vereniging : End2EndTest<VerenigingenPerInszResponse>
{
    private readonly VerfijnSubtypeNaarSubverenigingContext _testContext;
    private readonly string _inszToCompare;
    private readonly VerenigingenPerInszRequest _request;

    public Returns_Vereniging(VerfijnSubtypeNaarSubverenigingContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
        _inszToCompare = testContext.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Vertegenwoordigers[0].Insz;
        _request = new VerenigingenPerInszRequest()
        {
            Insz = _inszToCompare,
            KboNummers = [],
        };
    }

    public override async Task<VerenigingenPerInszResponse> GetResponse(FullBlownApiSetup setup)
        => await setup.AcmApiHost.GetVerenigingenPerInsz(_request, _testContext.CommandResult.Sequence);
    [Fact]
    public async ValueTask With_Verenigingen()
    {
        await using var session = _testContext.ApiSetup.AcmApiHost.DocumentStore().LightweightSession();

        var missingDocs = string.Empty;
        if (Response.Verenigingen.Length == 0)
        {
            missingDocs = LogMissingDocuments(session);
        }

        Response.ShouldCompare(new VerenigingenPerInszResponse()
        {
            Insz = _inszToCompare,
            Verenigingen =
            [
                new VerenigingenPerInszResponse.Vereniging()
                {
                    VCode = _testContext.VCode,
                    CorresponderendeVCodes = [],
                    VertegenwoordigerId = _testContext.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Vertegenwoordigers[0].VertegenwoordigerId,
                    Naam = _testContext.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Naam,
                    Status = VerenigingStatus.Actief,
                    KboNummer = string.Empty,
                    Verenigingstype = new VerenigingenPerInszResponse.Verenigingstype(
                        Verenigingstype.VZER.Code,
                        Verenigingstype.VZER.Naam),
                    Verenigingssubtype =  new VerenigingenPerInszResponse.Verenigingssubtype
                    {
                        Code = VerenigingssubtypeCode.Subvereniging.Code,
                        Naam = VerenigingssubtypeCode.Subvereniging.Naam,
                    },

                    IsHoofdvertegenwoordigerVan = true,
                },
            ],
            KboNummers = [],
        }, missingDocs);
    }

    private string LogMissingDocuments(IDocumentSession session)
    {
        var verenigingPerInszDocument = session.Query<VerenigingenPerInszDocument>()
                                               .Where(x => x.Insz == _inszToCompare)
                                               .Single();

        var verenigingDocument = session.Query<VerenigingDocument>()
                                        .Where(x => x.VCode == _testContext.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode)
                                        .Single();

        return $"VerenigingenPerInszDocument: {JsonConvert.SerializeObject(verenigingPerInszDocument)}{Environment.NewLine}" +
               $"VerenigingDocument: {JsonConvert.SerializeObject(verenigingDocument)}";

    }
}
