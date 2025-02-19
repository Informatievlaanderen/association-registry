namespace AssociationRegistry.Test.E2E.When_Registreer_FeitelijkeVereniging.Publiek.Detail_All;

using Admin.Api.Verenigingen.Common;
using Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
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
using Contactgegeven = Public.Api.Verenigingen.Detail.ResponseModels.Contactgegeven;
using DoelgroepResponse = Public.Api.Verenigingen.Detail.ResponseModels.DoelgroepResponse;
using GestructureerdeIdentificator = Public.Api.Verenigingen.Detail.ResponseModels.GestructureerdeIdentificator;
using HoofdactiviteitVerenigingsloket = Public.Api.Verenigingen.Detail.ResponseModels.HoofdactiviteitVerenigingsloket;
using Locatie = Public.Api.Verenigingen.Detail.ResponseModels.Locatie;
using Relatie = Public.Api.Verenigingen.Detail.ResponseModels.Relatie;
using Sleutel = Public.Api.Verenigingen.Detail.ResponseModels.Sleutel;
using Vereniging = Public.Api.Verenigingen.Detail.ResponseModels.Vereniging;
using VerenigingStatus = Admin.Schema.Constants.VerenigingStatus;
using VerenigingsType = Public.Api.Verenigingen.Detail.ResponseModels.VerenigingsType;
using Werkingsgebied = Public.Api.Verenigingen.Detail.ResponseModels.Werkingsgebied;

[Collection(FullBlownApiCollection.Name)]
public class Returns_ArrayOfDetailResponses : End2EndTest<RegistreerFeitelijkeVerenigingTestContext, RegistreerFeitelijkeVerenigingRequest, PubliekVerenigingDetailResponse>
{
    public override Func<IApiSetup, PubliekVerenigingDetailResponse> GetResponse =>
        setup => setup.PublicApiHost
                      .GetPubliekDetailAll()
                      .FindVereniging(TestContext.VCode);

    public Returns_ArrayOfDetailResponses(RegistreerFeitelijkeVerenigingTestContext testContext) : base(testContext)
    {
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
    public void WithFeitelijkeVereniging()
        => Response.Vereniging.ShouldCompare(new Vereniging
        {
            type = JsonLdType.FeitelijkeVereniging.Type,
            Doelgroep = new DoelgroepResponse
            {
                type = JsonLdType.Doelgroep.Type,
                id = JsonLdType.Doelgroep.CreateWithIdValues(TestContext.VCode),
                Minimumleeftijd = 1,
                Maximumleeftijd = 149,
            },
            VCode = TestContext.VCode,
            KorteBeschrijving = Request.KorteBeschrijving,
            KorteNaam = Request.KorteNaam,
            Verenigingstype = new VerenigingsType
            {
                Code = Verenigingstype.FeitelijkeVereniging.Code,
                Naam = Verenigingstype.FeitelijkeVereniging.Naam,
            },
            Naam = Request.Naam,
            Startdatum = DateOnly.FromDateTime(DateTime.Now),
            Status = VerenigingStatus.Actief,
            Contactgegevens = PubliekDetailResponseMapper.MapContactgegevens(Request.Contactgegevens, TestContext.VCode),
            HoofdactiviteitenVerenigingsloket = PubliekDetailResponseMapper.MapHoofdactiviteitenVerenigingsloket(Request.HoofdactiviteitenVerenigingsloket),
            Werkingsgebieden = PubliekDetailResponseMapper.MapWerkingsgebieden(Request.Werkingsgebieden),
            Locaties = PubliekDetailResponseMapper.MapLocaties(Request.Locaties, TestContext.VCode),
            Relaties = [],
            Sleutels = PubliekDetailResponseMapper.MapSleutels(TestContext.VCode),
        }, compareConfig: AdminDetailComparisonConfig.Instance);
}
