﻿namespace AssociationRegistry.Test.E2E.When_Registreer_FeitelijkeVereniging.Beheer.Detail.Without_Header;

using Admin.Api.Verenigingen.Detail.ResponseModels;
using Formats;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.Comparison;
using Framework.Mappers;
using Framework.TestClasses;
using JsonLdContext;
using KellermanSoftware.CompareNetObjects;
using NodaTime;
using Vereniging.Bronnen;
using Xunit;
using VerenigingStatus = Admin.Schema.Constants.VerenigingStatus;
using Verenigingstype = Admin.Api.Verenigingen.Detail.ResponseModels.Verenigingstype;

[Collection(nameof(RegistreerFeitelijkeVerenigingCollection))]
public class Returns_Vereniging : End2EndTest<DetailVerenigingResponse>
{
    private readonly RegistreerFeitelijkeVerenigingContext _testContext;

    public Returns_Vereniging(RegistreerFeitelijkeVerenigingContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override DetailVerenigingResponse GetResponse(FullBlownApiSetup setup)
        => setup.AdminApiHost.GetBeheerDetail(setup.AdminHttpClient, _testContext.VCode,new RequestParameters().WithExpectedSequence(_testContext.CommandResult.Sequence)).GetAwaiter().GetResult();


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
                Minimumleeftijd = 1,
                Maximumleeftijd = 149,
            },
            VCode = _testContext.VCode,
            KorteBeschrijving = _testContext.CommandRequest.KorteBeschrijving,
            KorteNaam = _testContext.CommandRequest.KorteNaam,
            Verenigingstype = new Verenigingstype
            {
                Code = Vereniging.Verenigingstype.FeitelijkeVereniging.Code,
                Naam = Vereniging.Verenigingstype.FeitelijkeVereniging.Naam,
            },
            Verenigingssubtype = null,
            Naam = _testContext.CommandRequest.Naam,
            Startdatum = Instant.FromDateTimeOffset(DateTimeOffset.UtcNow).FormatAsBelgianDate(),
            Einddatum = null,
            Status = VerenigingStatus.Actief,
            IsUitgeschrevenUitPubliekeDatastroom = _testContext.CommandRequest.IsUitgeschrevenUitPubliekeDatastroom,
            Contactgegevens = BeheerDetailResponseMapper.MapContactgegevens(_testContext.CommandRequest.Contactgegevens, _testContext.VCode),
            HoofdactiviteitenVerenigingsloket = BeheerDetailResponseMapper.MapHoofdactiviteitenVerenigingsloket(_testContext.CommandRequest.HoofdactiviteitenVerenigingsloket),
            Werkingsgebieden = BeheerDetailResponseMapper.MapWerkingsgebieden(_testContext.CommandRequest.Werkingsgebieden),
            Locaties = BeheerDetailResponseMapper.MapLocaties(_testContext.CommandRequest.Locaties, _testContext.VCode),
            Vertegenwoordigers = BeheerDetailResponseMapper.MapVertegenwoordigers(_testContext.CommandRequest.Vertegenwoordigers, _testContext.VCode),
            Relaties = [],
            Lidmaatschappen = [],
            Sleutels = BeheerDetailResponseMapper.MapSleutels(_testContext.VCode),
            IsDubbelVan = string.Empty,
        }, compareConfig: AdminDetailComparisonConfig.Instance);
}
