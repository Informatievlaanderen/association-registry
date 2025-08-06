namespace AssociationRegistry.Test.E2E.When_Wijzig_Basisgegevens_Kbo.Beheer.Detail;

using Admin.Api.WebApi.Verenigingen.Detail.ResponseModels;
using AssociationRegistry.Formats;
using AssociationRegistry.JsonLdContext;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.Comparison;
using AssociationRegistry.Test.E2E.Framework.Mappers;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using AssociationRegistry.Vereniging.Bronnen;
using KellermanSoftware.CompareNetObjects;
using NodaTime;
using Xunit;
using VerenigingStatus = Admin.Schema.Constants.VerenigingStatus;
using Verenigingstype = Admin.Api.WebApi.Verenigingen.Detail.ResponseModels.Verenigingstype;

[Collection(nameof(WijzigBasisgegevensKbocollection))]
public class Returns_DetailResponse : End2EndTest<DetailVerenigingResponse>
{
    private readonly WijzigBasisgegevensKboContext _testContext;

    public Returns_DetailResponse(WijzigBasisgegevensKboContext context) : base(context.ApiSetup)
    {
        _testContext = context;
    }

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/beheer/detail-vereniging-context.json");
    }

    [Fact]
    public void With_Metadata_DatumLaatsteAanpassing()
    {
        Response.Metadata.DatumLaatsteAanpassing.ShouldCompare(
            Instant.FromDateTimeOffset(DateTimeOffset.Now).FormatAsBelgianDate(),
            compareConfig: new ComparisonConfig { MaxMillisecondsDateDifference = 5000 });
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
                id = JsonLdType.Doelgroep.CreateWithIdValues(_testContext.VCode),
                type = JsonLdType.Doelgroep.Type,
                Minimumleeftijd = _testContext.CommandRequest.Doelgroep.Minimumleeftijd.Value,
                Maximumleeftijd = _testContext.CommandRequest.Doelgroep.Maximumleeftijd.Value,
            },
            Roepnaam = _testContext.CommandRequest.Roepnaam,
            VCode = _testContext.VCode,
            KorteBeschrijving = _testContext.CommandRequest.KorteBeschrijving,
            KorteNaam = _testContext.RegistratieData.KorteNaam,
            Verenigingstype = new Verenigingstype
            {
                Code = AssociationRegistry.Vereniging.Verenigingstype.VZW.Code,
                Naam = AssociationRegistry.Vereniging.Verenigingstype.VZW.Naam,
            },
            Naam = _testContext.RegistratieData.Naam,
            Startdatum = Instant.FromDateTimeOffset(
                new DateTimeOffset(_testContext.RegistratieData.Startdatum.Value.ToDateTime(new TimeOnly(12, 0, 0)))
            ).FormatAsBelgianDate(),
            Einddatum = null,
            Status = VerenigingStatus.Actief,
            IsUitgeschrevenUitPubliekeDatastroom = false,
            Contactgegevens = [],
            HoofdactiviteitenVerenigingsloket = BeheerDetailResponseMapper.MapHoofdactiviteitenVerenigingsloket(_testContext.CommandRequest.HoofdactiviteitenVerenigingsloket),
            Werkingsgebieden = BeheerDetailResponseMapper.MapWerkingsgebieden(_testContext.CommandRequest.Werkingsgebieden),
            Locaties = [],
            Vertegenwoordigers = [],
            Relaties = [],
            Lidmaatschappen = [],
            Sleutels = BeheerDetailResponseMapper.MapSleutels(_testContext.VCode, _testContext.RegistratieData.KboNummer),
            IsDubbelVan = string.Empty,
        }, compareConfig: AdminDetailComparisonConfig.Instance);

    public override DetailVerenigingResponse GetResponse(FullBlownApiSetup setup)
        => setup.AdminApiHost.GetBeheerDetail(setup.AdminHttpClient, _testContext.VCode,new RequestParameters().WithExpectedSequence(_testContext.CommandResult.Sequence)).GetAwaiter().GetResult();
}
