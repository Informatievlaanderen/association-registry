namespace AssociationRegistry.Test.E2E.When_Wijzig_Basisgegevens_Kbo.Publiek.Detail_All;

using Admin.Api.Verenigingen.WijzigBasisgegevens.MetRechtspersoonlijkheid.RequestModels;
using Formats;
using JsonLdContext;
using AssociationRegistry.Public.Api.Verenigingen.Detail.ResponseModels;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.Comparison;
using Framework.Mappers;
using Framework.TestClasses;
using Vereniging;
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
    private readonly WijzigBasisgegevensKboContext _context;

    public override PubliekVerenigingDetailResponse GetResponse(FullBlownApiSetup setup)
        => setup.PublicApiHost
                      .GetPubliekDetailAll()
                .FindVereniging(_context.VCode);

    public Returns_ArrayOfDetailResponses(WijzigBasisgegevensKboContext context)
        : base(context.ApiSetup)
    {
        _context = context;
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
                id = JsonLdType.Doelgroep.CreateWithIdValues(_context.VCode),
                Minimumleeftijd = 1,
                Maximumleeftijd = 149,
            },
            VCode = _context.VCode,
            KorteBeschrijving = _context.CommandRequest.KorteBeschrijving,
            KorteNaam = _context.RegistratieData.KorteNaam,
            Verenigingstype = new Verenigingstype
            {
                Code = AssociationRegistry.Vereniging.Verenigingstype.VZW.Code,
                Naam = AssociationRegistry.Vereniging.Verenigingstype.VZW.Naam,
            },
            Naam = _context.RegistratieData.Naam,
            Roepnaam = _context.CommandRequest.Roepnaam,
            Startdatum = DateOnly.FromDateTime(DateTime.Now),
            Status = VerenigingStatus.Actief,
            Contactgegevens = [],
            HoofdactiviteitenVerenigingsloket = PubliekDetailResponseMapper.MapHoofdactiviteitenVerenigingsloket(_context.CommandRequest.HoofdactiviteitenVerenigingsloket),
            Werkingsgebieden = PubliekDetailResponseMapper.MapWerkingsgebieden(_context.CommandRequest.Werkingsgebieden),
            Locaties = [],
            Relaties = [],
            Sleutels = PubliekDetailResponseMapper.MapSleutels(_context.VCode, _context.RegistratieData.KboNummer),
        }, compareConfig: AdminDetailComparisonConfig.Instance);
}
