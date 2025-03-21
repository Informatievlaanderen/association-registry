﻿namespace AssociationRegistry.Test.E2E.When_Wijzig_Basisgegevens.Beheer.Detail;

using Admin.Api.Verenigingen.Detail.ResponseModels;
using Admin.Api.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.RequestModels;
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
using Verenigingstype = Admin.Api.Verenigingen.Detail.ResponseModels.Verenigingstype;

[Collection(FullBlownApiCollection.Name)]
public class Returns_DetailResponse : End2EndTest<WijzigBasisgegevensTestContext, WijzigBasisgegevensRequest, DetailVerenigingResponse>,
                                      IAsyncLifetime
{
    public override Func<IApiSetup, DetailVerenigingResponse> GetResponse
        => setup => setup.AdminApiHost.GetBeheerDetail(TestContext.VCode);

    public Returns_DetailResponse(WijzigBasisgegevensTestContext testContext) : base(testContext)
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
                Minimumleeftijd = Request.Doelgroep.Minimumleeftijd.Value,
                Maximumleeftijd = Request.Doelgroep.Maximumleeftijd.Value,
            },
            VCode = TestContext.VCode,
            KorteBeschrijving = Request.KorteBeschrijving,
            KorteNaam = Request.KorteNaam,
            Verenigingstype = new Verenigingstype
            {
                Code = AssociationRegistry.Vereniging.Verenigingstype.FeitelijkeVereniging.Code,
                Naam = AssociationRegistry.Vereniging.Verenigingstype.FeitelijkeVereniging.Naam,
            },
            Naam = Request.Naam,
            Startdatum = Instant.FromDateTimeOffset(
                new DateTimeOffset(Request.Startdatum.Value.ToDateTime(new TimeOnly(12, 0, 0)))
            ).FormatAsBelgianDate(),
            Einddatum = null,
            Status = VerenigingStatus.Actief,
            IsUitgeschrevenUitPubliekeDatastroom = Request.IsUitgeschrevenUitPubliekeDatastroom.Value,
            Contactgegevens =
                BeheerDetailResponseMapper.MapContactgegevens(TestContext.RegistratieData.Contactgegevens,
                                                              TestContext.VCode),
            HoofdactiviteitenVerenigingsloket =
                BeheerDetailResponseMapper.MapHoofdactiviteitenVerenigingsloket(Request.HoofdactiviteitenVerenigingsloket),
            Werkingsgebieden = BeheerDetailResponseMapper.MapWerkingsgebieden(Request.Werkingsgebieden),
            Locaties = BeheerDetailResponseMapper.MapLocaties(TestContext.RegistratieData.Locaties, TestContext.VCode),
            Vertegenwoordigers =
                BeheerDetailResponseMapper.MapVertegenwoordigers(TestContext.RegistratieData.Vertegenwoordigers, TestContext.VCode),
            Relaties = [],
            Lidmaatschappen = [],
            Sleutels = BeheerDetailResponseMapper.MapSleutels( TestContext.VCode),
            IsDubbelVan = string.Empty,
        }, compareConfig: AdminDetailComparisonConfig.Instance);
}
