namespace AssociationRegistry.Test.E2E.When_Wijzig_Basisgegevens_Kbo.Publiek.Detail_All;

using Formats;
using JsonLdContext;
using AssociationRegistry.Public.Api.Verenigingen.Detail.ResponseModels;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.Comparison;
using Framework.Mappers;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using NodaTime;

using Xunit;
using DoelgroepResponse = Public.Api.Verenigingen.Detail.ResponseModels.DoelgroepResponse;
using Vereniging = Public.Api.Verenigingen.Detail.ResponseModels.Vereniging;
using VerenigingStatus = Admin.Schema.Constants.VerenigingStatus;
using Verenigingstype = Public.Api.Verenigingen.Detail.ResponseModels.Verenigingstype;

[Collection(nameof(WijzigBasisgegevensKbocollection))]
public class Returns_ArrayOfDetailResponses : End2EndTest<PubliekVerenigingDetailResponse>
{
    private readonly WijzigBasisgegevensKboContext _testContext;

    public override PubliekVerenigingDetailResponse GetResponse(FullBlownApiSetup setup)
        => setup.PublicApiHost
                      .GetPubliekDetailAll(_testContext.CommandResult.Sequence)
                .FindVereniging(_testContext.VCode);

    public Returns_ArrayOfDetailResponses(WijzigBasisgegevensKboContext testContext)
        : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/publiek/detail-all-vereniging-context.json");
    }

    [Fact]
    public void With_Metadata_DatumLaatsteAanpassing()
    {
        Response.Metadata.DatumLaatsteAanpassing.ShouldCompare(Instant.FromDateTimeOffset(DateTimeOffset.Now).FormatAsBelgianDate(),
                                                                        compareConfig: new ComparisonConfig
                                                                            { MaxMillisecondsDateDifference = 5000 });
    }

    [Fact]
    public void WithVerenigingMetRechtspersoonlijkheid()
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
                Code = AssociationRegistry.Vereniging.Verenigingstype.VZW.Code,
                Naam = AssociationRegistry.Vereniging.Verenigingstype.VZW.Naam,
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
}
