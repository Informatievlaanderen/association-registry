namespace AssociationRegistry.Test.E2E.When_Wijzig_Basisgegevens_Kbo.Beheer.Detail;

using Admin.Api.Verenigingen.Detail.ResponseModels;
using Admin.Api.Verenigingen.WijzigBasisgegevens.MetRechtspersoonlijkheid.RequestModels;
using Formats;
using JsonLdContext;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.Comparison;
using Framework.Mappers;
using Framework.TestClasses;
using Vereniging;
using Vereniging.Bronnen;
using KellermanSoftware.CompareNetObjects;
using NodaTime;

using Xunit;
using VerenigingStatus = Admin.Schema.Constants.VerenigingStatus;

[Collection(FullBlownApiCollection.Name)]
public class Returns_DetailResponse :
    End2EndTest<WijzigBasisgegevensKboTestContext, WijzigBasisgegevensRequest, DetailVerenigingResponse>
{
    public Returns_DetailResponse(WijzigBasisgegevensKboTestContext testContext) : base(testContext)
    {
    }

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
    public async ValueTask WithVerenigingMetRechtspersoonlijkheid()
        => Response.Vereniging.ShouldCompare(new VerenigingDetail
        {
            Bron = Bron.KBO,
            type = JsonLdType.VerenigingMetRechtspersoonlijkheid.Type,
            CorresponderendeVCodes = [],
            Doelgroep = new DoelgroepResponse
            {
                id = JsonLdType.Doelgroep.CreateWithIdValues(TestContext.VCode),
                type = JsonLdType.Doelgroep.Type,
                Minimumleeftijd = Request.Doelgroep.Minimumleeftijd.Value,
                Maximumleeftijd = Request.Doelgroep.Maximumleeftijd.Value,
            },
            Roepnaam = Request.Roepnaam,
            VCode = TestContext.VCode,
            KorteBeschrijving = Request.KorteBeschrijving,
            KorteNaam = TestContext.RegistratieData.KorteNaam,
            Verenigingstype = new VerenigingsType
            {
                Code = Verenigingstype.VZW.Code,
                Naam = Verenigingstype.VZW.Naam,
            },
            Naam = TestContext.RegistratieData.Naam,
            Startdatum = Instant.FromDateTimeOffset(
                new DateTimeOffset(TestContext.RegistratieData.Startdatum.Value.ToDateTime(new TimeOnly(12, 0, 0)))
            ).FormatAsBelgianDate(),
            Einddatum = null,
            Status = VerenigingStatus.Actief,
            IsUitgeschrevenUitPubliekeDatastroom = false,
            Contactgegevens = [],
            HoofdactiviteitenVerenigingsloket = BeheerDetailResponseMapper.MapHoofdactiviteitenVerenigingsloket(TestContext.Request.HoofdactiviteitenVerenigingsloket),
            Werkingsgebieden = BeheerDetailResponseMapper.MapWerkingsgebieden(TestContext.Request.Werkingsgebieden),
            Locaties = [],
            Vertegenwoordigers = [],
            Relaties = [],
            Lidmaatschappen = [],
            Sleutels = BeheerDetailResponseMapper.MapSleutels(TestContext.VCode, TestContext.RegistratieData.KboNummer),
            IsDubbelVan = string.Empty,
        }, compareConfig: AdminDetailComparisonConfig.Instance);

    public override Func<IApiSetup, DetailVerenigingResponse> GetResponse
        => setup => setup.AdminApiHost.GetBeheerDetail(TestContext.VCode);
}
