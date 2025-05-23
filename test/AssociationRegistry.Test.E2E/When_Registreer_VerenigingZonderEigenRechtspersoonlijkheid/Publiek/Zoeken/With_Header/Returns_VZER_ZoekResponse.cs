﻿namespace AssociationRegistry.Test.E2E.When_Registreer_VerenigingZonderEigenRechtspersoonlijkheid.Publiek.Zoeken.With_Header;

using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.Comparison;
using Framework.Mappers;
using Framework.TestClasses;
using JsonLdContext;
using KellermanSoftware.CompareNetObjects;
using Public.Api.Verenigingen.Search.ResponseModels;
using Xunit;
using DoelgroepResponse = Public.Api.Verenigingen.Search.ResponseModels.DoelgroepResponse;
using Vereniging = Public.Api.Verenigingen.Search.ResponseModels.Vereniging;
using Verenigingssubtype = Public.Api.Verenigingen.Search.ResponseModels.Verenigingssubtype;
using Verenigingstype = Vereniging.Verenigingstype;

[Collection(nameof(RegistreerVerenigingZonderEigenRechtspersoonlijkheidCollection))]
public class Returns_VZER_ZoekResponse : End2EndTest<SearchVerenigingenResponse>
{
    private readonly RegistreerVerenigingZonderEigenRechtspersoonlijkheidContext _testContext;

    public Returns_VZER_ZoekResponse(RegistreerVerenigingZonderEigenRechtspersoonlijkheidContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override SearchVerenigingenResponse GetResponse(FullBlownApiSetup setup)
        => setup.PublicApiHost.GetPubliekZoekenWithHeader(setup.SuperAdminHttpClient, $"vCode:{_testContext.VCode}", _testContext.CommandResult.Sequence).GetAwaiter().GetResult();

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/publiek/zoek-verenigingen-context.json");
    }

    [Fact]
    public async ValueTask WithFeitelijkeVereniging()
        => Response.Verenigingen.Single().ShouldCompare(new Vereniging
        {
            type = JsonLdType.FeitelijkeVereniging.Type,
            Doelgroep = new DoelgroepResponse
            {
                type = JsonLdType.Doelgroep.Type,
                id = JsonLdType.Doelgroep.CreateWithIdValues(_testContext.VCode),
                Minimumleeftijd = 1,
                Maximumleeftijd = 149,
            },
            VCode = _testContext.VCode,
            KorteBeschrijving = _testContext.CommandRequest.KorteBeschrijving,
            KorteNaam = _testContext.CommandRequest.KorteNaam,
            Verenigingstype = new VerenigingsType
            {
                Code = Verenigingstype.VZER.Code,
                Naam = Verenigingstype.VZER.Naam,
            },
            Verenigingssubtype = new Verenigingssubtype
            {
                Code = string.Empty,
                Naam = string.Empty,
            },
            Naam = _testContext.CommandRequest.Naam,
            HoofdactiviteitenVerenigingsloket =
                PubliekZoekResponseMapper.MapHoofdactiviteitenVerenigingsloket(
                    _testContext.CommandRequest.HoofdactiviteitenVerenigingsloket),
            Werkingsgebieden = PubliekZoekResponseMapper.MapWerkingsgebieden(_testContext.CommandRequest.Werkingsgebieden),
            Locaties = PubliekZoekResponseMapper.MapLocaties(_testContext.CommandRequest.Locaties, _testContext.VCode),
            Relaties = [],
            Lidmaatschappen = [],
            Sleutels = PubliekZoekResponseMapper.MapSleutels(_testContext.VCode),
            Links = new VerenigingLinks()
            {
                Detail = new Uri($"{_testContext.PublicApiAppSettings.BaseUrl}/v1/verenigingen/{_testContext.VCode}"),
            },
        }, compareConfig: PubliekZoekenComparisonConfig.Instance);
}
