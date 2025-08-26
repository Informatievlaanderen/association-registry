namespace AssociationRegistry.Test.E2E.When_Wijzig_Basisgegevens_Kbo.Publiek.Detail;

using AssociationRegistry.Admin.Schema.Constants;
using AssociationRegistry.Formats;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.Comparison;
using AssociationRegistry.Test.E2E.Framework.Mappers;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using Contracts.JsonLdContext;
using KellermanSoftware.CompareNetObjects;
using NodaTime;
using Public.Api.WebApi.Verenigingen.Detail.ResponseModels;
using Xunit;

[Collection(nameof(WijzigBasisgegevensKbocollection))]
public class Returns_DetailResponse : End2EndTest<PubliekVerenigingDetailResponse>
{
    private readonly WijzigBasisgegevensKboContext _testContext;

    public Returns_DetailResponse(WijzigBasisgegevensKboContext testContext)
        : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/publiek/detail-vereniging-context.json");
    }

    [Fact]
    public void With_Metadata_DatumLaatsteAanpassing()
    {
        Response.Metadata.DatumLaatsteAanpassing.ShouldCompare(Instant.FromDateTimeOffset(DateTimeOffset.Now).FormatAsBelgianDate(),
                                                               compareConfig: new ComparisonConfig
                                                                   { MaxMillisecondsDateDifference = 5000 });
    }

    [Fact]
    public async ValueTask WithVerenigingMetRechtspersoonlijkheid()
        => Response.Vereniging.ShouldCompare(new Vereniging
        {
            type = JsonLdType.VerenigingMetRechtspersoonlijkheid.Type,
            Doelgroep = new DoelgroepResponse
            {
                type = JsonLdType.Doelgroep.Type,
                id = JsonLdType.Doelgroep.CreateWithIdValues(_testContext.VCode),
                Minimumleeftijd = 1,
                Maximumleeftijd = 149,
            },
            VCode = _testContext.VCode,
            KorteBeschrijving = _testContext.CommandRequest.KorteBeschrijving,
            KorteNaam = _testContext.RegistratieData.KorteNaam,
            Verenigingstype = new Verenigingstype
            {
                Code = DecentraalBeheer.Vereniging.Verenigingstype.VZW.Code,
                Naam = DecentraalBeheer.Vereniging.Verenigingstype.VZW.Naam,
            },
            Naam = _testContext.RegistratieData.Naam,
            Roepnaam = _testContext.CommandRequest.Roepnaam,
            Startdatum = DateOnly.FromDateTime(DateTime.Now),
            Status = VerenigingStatus.Actief,
            Contactgegevens = [],
            HoofdactiviteitenVerenigingsloket = PubliekDetailResponseMapper.MapHoofdactiviteitenVerenigingsloket(_testContext.CommandRequest.HoofdactiviteitenVerenigingsloket),
            Werkingsgebieden = PubliekDetailResponseMapper.MapWerkingsgebieden(_testContext.CommandRequest.Werkingsgebieden),
            Locaties = [],
            Relaties = [],
            Sleutels = PubliekDetailResponseMapper.MapSleutels(_testContext.VCode, _testContext.RegistratieData.KboNummer),
        }, compareConfig: AdminDetailComparisonConfig.Instance);

    public override async Task<PubliekVerenigingDetailResponse> GetResponse(FullBlownApiSetup setup)
        => await setup.PublicApiHost.GetPubliekDetail(_testContext.VCode);
}
