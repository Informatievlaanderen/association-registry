namespace AssociationRegistry.Test.E2E.When_Wijzig_Basisgegevens.Beheer.Detail;

using Admin.Api.WebApi.Verenigingen.Detail.ResponseModels;
using Contracts.JsonLdContext;
using Formats;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.Comparison;
using Framework.Mappers;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using NodaTime;
using Vereniging.Bronnen;
using Xunit;
using VerenigingStatus = Admin.Schema.Constants.VerenigingStatus;
using Verenigingstype = Admin.Api.WebApi.Verenigingen.Detail.ResponseModels.Verenigingstype;

[Collection(nameof(WijzigBasisGegevensCollection))]
public class Returns_DetailResponse : End2EndTest<DetailVerenigingResponse>
{
    private readonly WijzigBasisgegevensContext _testContext;

    public Returns_DetailResponse(WijzigBasisgegevensContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override async Task<DetailVerenigingResponse> GetResponse(FullBlownApiSetup setup)
        => await setup.AdminApiHost.GetBeheerDetail(setup.AdminHttpClient, _testContext.VCode,new RequestParameters().WithExpectedSequence(_testContext.CommandResult.Sequence));

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/beheer/detail-vereniging-context.json");
    }

    [Fact]
    public void With_Metadata_DatumLaatsteAanpassing()
    {
        Response.Metadata.DatumLaatsteAanpassing.ShouldCompare(Instant.FromDateTimeOffset(DateTimeOffset.Now).FormatAsBelgianDate(),
                                                               compareConfig: new ComparisonConfig
                                                                   { MaxMillisecondsDateDifference = 5000 });
    }

    [Fact]
    public async ValueTask WithFeitelijkeVereniging()
        => Response.Vereniging.ShouldCompare(new VerenigingDetail
        {
            Bron = Bron.Initiator,
            type = JsonLdType.FeitelijkeVereniging.Type,
            CorresponderendeVCodes = [],
            Doelgroep = new DoelgroepResponse
            {
                type = JsonLdType.Doelgroep.Type,
                id = JsonLdType.Doelgroep.CreateWithIdValues(_testContext.VCode),
                Minimumleeftijd = _testContext.CommandRequest.Doelgroep.Minimumleeftijd.Value,
                Maximumleeftijd = _testContext.CommandRequest.Doelgroep.Maximumleeftijd.Value,
            },
            VCode = _testContext.VCode,
            KorteBeschrijving = _testContext.CommandRequest.KorteBeschrijving,
            KorteNaam = _testContext.CommandRequest.KorteNaam,
            Verenigingstype = new Verenigingstype
            {
                Code = DecentraalBeheer.Vereniging.Verenigingstype.FeitelijkeVereniging.Code,
                Naam = DecentraalBeheer.Vereniging.Verenigingstype.FeitelijkeVereniging.Naam,
            },
            Naam = _testContext.CommandRequest.Naam,
            Startdatum = Instant.FromDateTimeOffset(
                new DateTimeOffset(_testContext.CommandRequest.Startdatum.Value.ToDateTime(new TimeOnly(12, 0, 0)))
            ).FormatAsBelgianDate(),
            Einddatum = null,
            Status = VerenigingStatus.Actief,
            IsUitgeschrevenUitPubliekeDatastroom = _testContext.CommandRequest.IsUitgeschrevenUitPubliekeDatastroom.Value,
            Contactgegevens =
                BeheerDetailResponseMapper.MapContactgegevens(_testContext.Scenario.FeitelijkeVerenigingWerdGeregistreerd.Contactgegevens,
                                                              _testContext.VCode),
            HoofdactiviteitenVerenigingsloket =
                BeheerDetailResponseMapper.MapHoofdactiviteitenVerenigingsloket(_testContext.CommandRequest.HoofdactiviteitenVerenigingsloket),
            Werkingsgebieden = BeheerDetailResponseMapper.MapWerkingsgebieden(_testContext.CommandRequest.Werkingsgebieden),
            Locaties = BeheerDetailResponseMapper.MapLocaties(_testContext.Scenario.FeitelijkeVerenigingWerdGeregistreerd.Locaties, _testContext.VCode),
            Vertegenwoordigers =
                BeheerDetailResponseMapper.MapVertegenwoordigers(_testContext.Scenario.FeitelijkeVerenigingWerdGeregistreerd.Vertegenwoordigers, _testContext.VCode),
            Relaties = [],
            Lidmaatschappen = [],
            Sleutels = BeheerDetailResponseMapper.MapSleutels( _testContext.VCode),
            IsDubbelVan = string.Empty,
        }, compareConfig: AdminDetailComparisonConfig.Instance);
}
