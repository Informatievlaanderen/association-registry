namespace AssociationRegistry.Test.E2E.When_Markeer_Als_Dubbel_Van.Acm;

using AssociationRegistry.Acm.Api.WebApi.VerenigingenPerInsz;
using AssociationRegistry.Test.Common.DubbelMerge;
using DecentraalBeheer.Vereniging;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Xunit;
using VerenigingStatus = AssociationRegistry.Acm.Schema.Constants.VerenigingStatus;

/// <summary>
///     Samwise was enkel vertegenwoordiger van de dubbele vereniging en werd overgenomen door de authentieke vereniging.
/// </summary>
[Collection(nameof(MarkeerAlsDubbelVanCollection))]
public class Returns_Vereniging_For_An_Overgenomen_Vertegenwoordiger : End2EndTest<VerenigingenPerInszResponse>
{
    private readonly MarkeerAlsDubbelVanContext _testContext;

    private readonly VerenigingenPerInszRequest _request = new()
    {
        Insz = TestVertegenwoordigers.Samwise.Insz,
        KboNummers = [],
    };

    public Returns_Vereniging_For_An_Overgenomen_Vertegenwoordiger(MarkeerAlsDubbelVanContext testContext)
        : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override async Task<VerenigingenPerInszResponse> GetResponse(FullBlownApiSetup setup) =>
        await setup.AcmApiHost.GetVerenigingenPerInsz(
            _request,
            _testContext.DubbeleVerenigingWerdSamengevoegd!.Sequence
        );

    [Fact]
    public void With_The_Authentieke_Vereniging()
    {
        Response.ShouldCompare(
            new VerenigingenPerInszResponse
            {
                Insz = TestVertegenwoordigers.Samwise.Insz,
                Verenigingen =
                [
                    // Dubbele vereniging wordt niet getoond
                    new VerenigingenPerInszResponse.Vereniging
                    {
                        VCode = _testContext.Scenario.AndereFeitelijkeVerenigingWerdGeregistreerd.VCode,
                        CorresponderendeVCodes = [_testContext.Scenario.FeitelijkeVerenigingWerdGeregistreerd.VCode],
                        VertegenwoordigerId = 3,
                        Naam = _testContext.Scenario.AndereFeitelijkeVerenigingWerdGeregistreerd.Naam,
                        Status = VerenigingStatus.Actief,
                        KboNummer = string.Empty,
                        Verenigingstype = new VerenigingenPerInszResponse.Verenigingstype(
                            Verenigingstype.VZER.Code,
                            Verenigingstype.VZER.Naam
                        ),
                        Verenigingssubtype = new VerenigingenPerInszResponse.Verenigingssubtype
                        {
                            Code = VerenigingssubtypeCode.Default.Code,
                            Naam = VerenigingssubtypeCode.Default.Naam,
                        },
                        IsHoofdvertegenwoordigerVan = true,
                    },
                ],
                KboNummers = [],
            }
        );
    }
}
