namespace AssociationRegistry.Test.E2E.When_Registreer_FeitelijkeVereniging.Beheer.Detail.With_Header;

using Admin.Api;
using Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequestModels;
using AssociationRegistry.Admin.Api.Verenigingen.Detail.ResponseModels;
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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NodaTime;

using Xunit;
using VerenigingStatus = Admin.Schema.Constants.VerenigingStatus;

[Collection(FullBlownApiCollection.Name)]
public class Returns_VZER_DetailResponse :
    End2EndTest<RegistreerFeitelijkeVerenigingTestContext, RegistreerFeitelijkeVerenigingRequest, DetailVerenigingResponse>
{
    public Returns_VZER_DetailResponse(RegistreerFeitelijkeVerenigingTestContext testContext) : base(testContext)
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
    public async Task WithFeitelijkeVereniging()
        => Response.Vereniging.ShouldCompare(new VerenigingDetail
        {
            Bron = Bron.Initiator,
            type = JsonLdType.FeitelijkeVereniging.Type,
            CorresponderendeVCodes = [],
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
                Code = Verenigingstype.VZER.Code,
                Naam = Verenigingstype.VZER.Naam,
            },
            Subtype = new SubtypeData
            {
                id = JsonLdType.Subtype.CreateWithIdValues(AssociationRegistry.Vereniging.Verenigingssubtype.NogNietBepaald.Code),
                type = JsonLdType.Subtype.Type,
                Subtype = new Admin.Api.Verenigingen.Detail.ResponseModels.Subtype()
                {
                    Code = AssociationRegistry.Vereniging.Verenigingssubtype.NogNietBepaald.Code,
                    Naam = AssociationRegistry.Vereniging.Verenigingssubtype.NogNietBepaald.Naam
                },
            },
            Naam = Request.Naam,
            Startdatum = Instant.FromDateTimeOffset(DateTimeOffset.UtcNow).FormatAsBelgianDate(),
            Einddatum = null,
            Status = VerenigingStatus.Actief,
            IsUitgeschrevenUitPubliekeDatastroom = Request.IsUitgeschrevenUitPubliekeDatastroom,
            Contactgegevens = BeheerDetailResponseMapper.MapContactgegevens(Request.Contactgegevens, TestContext.VCode),
            HoofdactiviteitenVerenigingsloket = BeheerDetailResponseMapper.MapHoofdactiviteitenVerenigingsloket(Request.HoofdactiviteitenVerenigingsloket),
            Werkingsgebieden = BeheerDetailResponseMapper.MapWerkingsgebieden(Request.Werkingsgebieden),
            Locaties = BeheerDetailResponseMapper.MapLocaties(Request.Locaties, TestContext.VCode),
            Vertegenwoordigers = BeheerDetailResponseMapper.MapVertegenwoordigers(Request.Vertegenwoordigers, TestContext.VCode),
            Relaties = [],
            Lidmaatschappen = [],
            Sleutels = BeheerDetailResponseMapper.MapSleutels(TestContext.VCode),
            IsDubbelVan = string.Empty,
        }, compareConfig: AdminDetailComparisonConfig.Instance);

    public override Func<IApiSetup, DetailVerenigingResponse> GetResponse
    {
        get { return setup =>
        {
            var logger = setup.AdminApiHost.Services.GetRequiredService<ILogger<Program>>();

            logger.LogInformation("EXECUTING GET REQUEST");

            return setup.AdminApiHost.GetBeheerDetailWithHeader(setup.SuperAdminHttpClient, TestContext.RequestResult.VCode,
                                                                TestContext.RequestResult.Sequence)
                        .GetAwaiter().GetResult();
        }; }
    }
}
