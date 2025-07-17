using AssociationRegistry.Admin.Schema.Search;

namespace AssociationRegistry.Admin.ProjectionHost.Projections.Search.Zoeken;

using Events;
using Formats;
using JsonLdContext;
using Schema;
using Schema.Search;
using Vereniging;
using Doelgroep = Schema.Search.VerenigingZoekDocument.Types.Doelgroep;
using VerenigingStatus = Schema.Constants.VerenigingStatus;
using WellknownFormats = Constants.WellknownFormats;

public class BeheerZoekProjectionHandlerV2
{
    public BeheerZoekProjectionHandlerV2(
    )
    {
    }

   public void Handle(EventEnvelope<FeitelijkeVerenigingWerdGeregistreerd> message, VerenigingZoekDocument document)
    {
        document.Sequence = message.Sequence;
        document.JsonLdMetadataType = JsonLdType.FeitelijkeVereniging.Type;
        document.VCode = message.Data.VCode;

        document.Verenigingstype = new VerenigingZoekDocument.Types.VerenigingsType
        {
            Code = Verenigingstype.FeitelijkeVereniging.Code,
            Naam = Verenigingstype.FeitelijkeVereniging.Naam,
        };

        document.Verenigingssubtype = null;
        document.Naam = message.Data.Naam;
        document.KorteNaam = message.Data.KorteNaam;
        document.Status = VerenigingStatus.Actief;
        document.Locaties = message.Data.Locaties.Select(locatie => Map(locatie, message.VCode)).ToArray();
        document.Startdatum = message.Data.Startdatum?.ToString(WellknownFormats.DateOnly);
        document.Einddatum = null;
        document.Doelgroep = Map(message.Data.Doelgroep, message.VCode);
        document.IsUitgeschrevenUitPubliekeDatastroom = message.Data.IsUitgeschrevenUitPubliekeDatastroom;

        document.HoofdactiviteitenVerenigingsloket = message.Data.HoofdactiviteitenVerenigingsloket
                                                            .Select(
                                                                 hoofdactiviteitVerenigingsloket =>
                                                                     new VerenigingZoekDocument.Types.HoofdactiviteitVerenigingsloket
                                                                     {
                                                                         JsonLdMetadata =
                                                                             CreateJsonLdMetadata(
                                                                                 JsonLdType.Hoofdactiviteit,
                                                                                 hoofdactiviteitVerenigingsloket.Code),
                                                                         Code = hoofdactiviteitVerenigingsloket.Code,
                                                                         Naam = hoofdactiviteitVerenigingsloket.Naam,
                                                                     })
                                                            .ToArray();

        document.Werkingsgebieden = [];
        document.Lidmaatschappen = [];
        document.CorresponderendeVCodes = [];

        document.Sleutels = [
            new VerenigingZoekDocument.Types.Sleutel
            {
                JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.Sleutel, message.VCode, Sleutelbron.VR.Waarde),
                Bron = Sleutelbron.VR,
                Waarde = message.Data.VCode,
                CodeerSysteem = CodeerSysteem.VR,
                GestructureerdeIdentificator = new VerenigingZoekDocument.Types.GestructureerdeIdentificator
                {
                    JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.GestructureerdeSleutel, message.VCode, Sleutelbron.VR.Waarde),
                    Nummer = message.Data.VCode,
                },
            },
        ];
    }

    public void Handle(
        EventEnvelope<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd> message,
        VerenigingZoekDocument zoekDocument)
    {
        zoekDocument.Sequence = message.Sequence;
        zoekDocument.JsonLdMetadataType = JsonLdType.FeitelijkeVereniging.Type;
        zoekDocument.VCode = message.Data.VCode;

        zoekDocument.Verenigingstype = new VerenigingZoekDocument.Types.VerenigingsType
        {
            Code = Verenigingstype.VZER.Code,
            Naam = Verenigingstype.VZER.Naam,
        };

        zoekDocument.Verenigingssubtype = new VerenigingZoekDocument.Types.Verenigingssubtype
        {
            Code = string.Empty,
            Naam = string.Empty,
        };

        zoekDocument.Naam = message.Data.Naam;
        zoekDocument.KorteNaam = message.Data.KorteNaam;
        zoekDocument.Status = VerenigingStatus.Actief;
        zoekDocument.Locaties = message.Data.Locaties.Select(locatie => Map(locatie, message.VCode)).ToArray();
        zoekDocument.Startdatum = message.Data.Startdatum?.ToString(WellknownFormats.DateOnly);
        zoekDocument.Einddatum = null;
        zoekDocument.Doelgroep = Map(message.Data.Doelgroep, message.VCode);
        zoekDocument.IsUitgeschrevenUitPubliekeDatastroom = message.Data.IsUitgeschrevenUitPubliekeDatastroom;

        zoekDocument.HoofdactiviteitenVerenigingsloket = message.Data.HoofdactiviteitenVerenigingsloket
                                                                .Select(
                                                                     hoofdactiviteitVerenigingsloket =>
                                                                         new VerenigingZoekDocument.Types.HoofdactiviteitVerenigingsloket
                                                                         {
                                                                             JsonLdMetadata =
                                                                                 CreateJsonLdMetadata(
                                                                                     JsonLdType.Hoofdactiviteit,
                                                                                     hoofdactiviteitVerenigingsloket.Code),
                                                                             Code = hoofdactiviteitVerenigingsloket.Code,
                                                                             Naam = hoofdactiviteitVerenigingsloket.Naam,
                                                                         })
                                                                .ToArray();

        zoekDocument.Werkingsgebieden = [];
        zoekDocument.Lidmaatschappen = [];
        zoekDocument.CorresponderendeVCodes = [];

        zoekDocument.Sleutels = [
            new VerenigingZoekDocument.Types.Sleutel
            {
                JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.Sleutel, message.VCode, Sleutelbron.VR.Waarde),
                Bron = Sleutelbron.VR,
                Waarde = message.Data.VCode,
                CodeerSysteem = CodeerSysteem.VR,
                GestructureerdeIdentificator = new VerenigingZoekDocument.Types.GestructureerdeIdentificator
                {
                    JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.GestructureerdeSleutel, message.VCode, Sleutelbron.VR.Waarde),
                    Nummer = message.Data.VCode,
                },
            },
        ];
    }


    public void Handle(EventEnvelope<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd> message, VerenigingZoekDocument document)
    {
        document.Sequence = message.Sequence;
        document.JsonLdMetadataType = JsonLdType.VerenigingMetRechtspersoonlijkheid.Type;
        document.VCode = message.Data.VCode;

        document.Verenigingstype = new VerenigingZoekDocument.Types.VerenigingsType
        {
            Code = Verenigingstype.Parse(message.Data.Rechtsvorm).Code,
            Naam = Verenigingstype.Parse(message.Data.Rechtsvorm).Naam,
        };

        document.Verenigingssubtype = null;
        document.Naam = message.Data.Naam;
        document.Roepnaam = string.Empty;
        document.KorteNaam = message.Data.KorteNaam;
        document.Status = VerenigingStatus.Actief;
        document.Locaties = [];
        document.Startdatum = message.Data.Startdatum?.ToString(WellknownFormats.DateOnly);
        document.Einddatum = null;

        document.Doelgroep = new Doelgroep
        {
            JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.Doelgroep, message.VCode),
            Minimumleeftijd = AssociationRegistry.Vereniging.Doelgroep.StandaardMinimumleeftijd,
            Maximumleeftijd = AssociationRegistry.Vereniging.Doelgroep.StandaardMaximumleeftijd,
        };

        document.IsUitgeschrevenUitPubliekeDatastroom = false;
        document.HoofdactiviteitenVerenigingsloket = [];
        document.Werkingsgebieden = [];
        document.Lidmaatschappen = [];
        document.CorresponderendeVCodes = [];

        document.Sleutels = [
            new VerenigingZoekDocument.Types.Sleutel
            {
                JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.Sleutel, message.VCode, Sleutelbron.VR.Waarde),
                Bron = Sleutelbron.VR,
                Waarde = message.Data.VCode,
                CodeerSysteem = CodeerSysteem.VR,
                GestructureerdeIdentificator = new VerenigingZoekDocument.Types.GestructureerdeIdentificator
                {
                    JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.GestructureerdeSleutel, message.VCode, Sleutelbron.VR.Waarde),
                    Nummer = message.Data.VCode,
                },
            },
            new VerenigingZoekDocument.Types.Sleutel
            {
                JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.Sleutel, message.VCode, Sleutelbron.KBO.Waarde),
                Bron = Sleutelbron.KBO,
                Waarde = message.Data.KboNummer,
                CodeerSysteem = CodeerSysteem.KBO,
                GestructureerdeIdentificator = new VerenigingZoekDocument.Types.GestructureerdeIdentificator
                {
                    JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.GestructureerdeSleutel, message.VCode, Sleutelbron.KBO.Waarde),
                    Nummer = message.Data.KboNummer,
                },
            },
        ];
    }

    public void Handle(EventEnvelope<NaamWerdGewijzigd> message, VerenigingZoekDocument document)
    {
        document.Naam = message.Data.Naam;
    }

    public void Handle(EventEnvelope<RoepnaamWerdGewijzigd> message, VerenigingZoekDocument document)
    {
        document.Roepnaam = message.Data.Roepnaam;
    }

    public void Handle(EventEnvelope<KorteNaamWerdGewijzigd> message, VerenigingZoekDocument document)

    {
        document.KorteNaam = message.Data.KorteNaam;
    }

    public void Handle(EventEnvelope<StartdatumWerdGewijzigd> message, VerenigingZoekDocument document)
        => await _elasticRepository.UpdateStartdatum<VerenigingZoekDocument>(
            message.VCode,
            message.Data.Startdatum,
            message.Sequence
        );

    public void Handle(EventEnvelope<StartdatumWerdGewijzigdInKbo> message, VerenigingZoekDocument document)
        => await _elasticRepository.UpdateStartdatum<VerenigingZoekDocument>(
            message.VCode,
            message.Data.Startdatum,
            message.Sequence
        );

    public void Handle(EventEnvelope<EinddatumWerdGewijzigd> message, VerenigingZoekDocument document)
        => document.
                Einddatum = message.Data.Einddatum.ToString(WellknownFormats.DateOnly),
            },
            message.Sequence
        );

    public void Handle(EventEnvelope<DoelgroepWerdGewijzigd> message, VerenigingZoekDocument document)
        => document.
                Doelgroep = new Doelgroep
                {
                    JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.Doelgroep, message.VCode),
                    Minimumleeftijd = message.Data.Doelgroep.Minimumleeftijd,
                    Maximumleeftijd = message.Data.Doelgroep.Maximumleeftijd,
                },
            },
            message.Sequence
        );

    public void Handle(EventEnvelope<HoofdactiviteitenVerenigingsloketWerdenGewijzigd> message, VerenigingZoekDocument document)
    {
        document.
                HoofdactiviteitenVerenigingsloket = message.Data.HoofdactiviteitenVerenigingsloket
                                                           .Select(
                                                                hoofdactiviteitVerenigingsloket =>
                                                                    new VerenigingZoekDocument.Types.HoofdactiviteitVerenigingsloket
                                                                    {
                                                                        JsonLdMetadata =
                                                                            CreateJsonLdMetadata(
                                                                                JsonLdType.Hoofdactiviteit,
                                                                                hoofdactiviteitVerenigingsloket.Code),
                                                                        Code = hoofdactiviteitVerenigingsloket.Code,
                                                                        Naam = hoofdactiviteitVerenigingsloket.Naam,
                                                                    })
                                                           .ToArray(),
            },
            message.Sequence);
    }

    public void Handle(EventEnvelope<WerkingsgebiedenWerdenNietBepaald> message, VerenigingZoekDocument document)
    {
        document.
                Werkingsgebieden = [],
            },
            message.Sequence);
    }

    public void Handle(EventEnvelope<WerkingsgebiedenWerdenBepaald> message, VerenigingZoekDocument updateDocument)
    {

        updateDocument.Werkingsgebieden = message.Data.Werkingsgebieden
                                                 .Select(
                                                      werkingsgebied =>
                                                          new VerenigingZoekDocument.Types.Werkingsgebied
                                                          {
                                                              JsonLdMetadata =
                                                                  CreateJsonLdMetadata(
                                                                      JsonLdType.Werkingsgebied,
                                                                      werkingsgebied.Code),
                                                              Code = werkingsgebied.Code,
                                                              Naam = werkingsgebied.Naam,
                                                          })
                                                 .ToArray();
    }

    public void Handle(EventEnvelope<WerkingsgebiedenWerdenGewijzigd> message, VerenigingZoekDocument document)
    {
        document.
                Werkingsgebieden = message.Data.Werkingsgebieden
                                          .Select(
                                               werkingsgebied =>
                                                   new VerenigingZoekDocument.Types.Werkingsgebied
                                                   {
                                                       JsonLdMetadata =
                                                           CreateJsonLdMetadata(
                                                               JsonLdType.Werkingsgebied,
                                                               werkingsgebied.Code),
                                                       Code = werkingsgebied.Code,
                                                       Naam = werkingsgebied.Naam,
                                                   })
                                          .ToArray(),
            },
            message.Sequence);
    }

    public void Handle(EventEnvelope<WerkingsgebiedenWerdenNietVanToepassing> message, VerenigingZoekDocument document)
    {
        document.
                Werkingsgebieden =
                [
                    new VerenigingZoekDocument.Types.Werkingsgebied
                    {
                        JsonLdMetadata =
                            CreateJsonLdMetadata(
                                JsonLdType.Werkingsgebied,
                                Werkingsgebied.NietVanToepassing.Code),
                        Code = Werkingsgebied.NietVanToepassing.Code,
                        Naam = Werkingsgebied.NietVanToepassing.Naam,
                    },
                ],
            },
            message.Sequence);
    }

    public void Handle(EventEnvelope<VerenigingWerdUitgeschrevenUitPubliekeDatastroom> message, VerenigingZoekDocument document)
    {
        document.
                IsUitgeschrevenUitPubliekeDatastroom = true,
            },
            message.Sequence);
    }

    public void Handle(EventEnvelope<VerenigingWerdIngeschrevenInPubliekeDatastroom> message, VerenigingZoekDocument document)
    {
        document.
                IsUitgeschrevenUitPubliekeDatastroom = false,
            },
            message.Sequence);
    }

    public void Handle(EventEnvelope<LocatieWerdToegevoegd> message, VerenigingZoekDocument document)
    {
        await _elasticRepository.AppendLocatie<VerenigingZoekDocument>(
            message.VCode,
            Map(message.Data.Locatie, message.VCode),
            message.Sequence);
    }

    public void Handle(EventEnvelope<LocatieWerdGewijzigd> message, VerenigingZoekDocument document)
    {
        await _elasticRepository.UpdateLocatie<VerenigingZoekDocument>(
            message.VCode,
            Map(message.Data.Locatie, message.VCode),
            message.Sequence);
    }

    public void Handle(EventEnvelope<LocatieWerdVerwijderd> message, VerenigingZoekDocument document)
    {
        await _elasticRepository.RemoveLocatie<VerenigingZoekDocument>(
            message.VCode,
            message.Data.Locatie.LocatieId,
            message.Sequence);
    }

    public void Handle(EventEnvelope<LidmaatschapWerdToegevoegd> message, VerenigingZoekDocument document)
    {
        await _elasticRepository.AppendLidmaatschap<VerenigingZoekDocument>(
            message.VCode,
            Map(message.Data.Lidmaatschap, message.VCode),
            message.Sequence);
    }

    public void Handle(EventEnvelope<LidmaatschapWerdGewijzigd> message, VerenigingZoekDocument document)
    {
        await _elasticRepository.UpdateLidmaatschap<VerenigingZoekDocument>(
            message.VCode,
            Map(message.Data.Lidmaatschap, message.VCode),
            message.Sequence);
    }

    public void Handle(EventEnvelope<LidmaatschapWerdVerwijderd> message, VerenigingZoekDocument document)
    {
        await _elasticRepository.RemoveLidmaatschap<VerenigingZoekDocument>(
            message.VCode,
            message.Data.Lidmaatschap.LidmaatschapId,
            message.Sequence);
    }

    private static VerenigingZoekDocument.Types.Locatie Map(Registratiedata.Locatie locatie, string vCode)
        => new()
        {
            JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.Locatie, vCode, locatie.LocatieId.ToString()),

            LocatieId = locatie.LocatieId,
            Locatietype = locatie.Locatietype,
            Naam = locatie.Naam,
            Adresvoorstelling = locatie.Adres.ToAdresString(),
            IsPrimair = locatie.IsPrimair,
            Postcode = locatie.Adres?.Postcode ?? string.Empty,
            Gemeente = locatie.Adres?.Gemeente ?? string.Empty,
        };

    private static VerenigingZoekDocument.Types.Lidmaatschap Map(Registratiedata.Lidmaatschap lidmaatschap, string vCode)
        => new()
        {
            JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.Lidmaatschap, vCode, lidmaatschap.LidmaatschapId.ToString()),

            LidmaatschapId = lidmaatschap.LidmaatschapId,
            AndereVereniging = lidmaatschap.AndereVereniging,
            DatumVan = lidmaatschap.DatumVan.FormatAsBelgianDate(),
            DatumTot = lidmaatschap.DatumTot.FormatAsBelgianDate(),
            Beschrijving = lidmaatschap.Beschrijving,
            Identificatie = lidmaatschap.Identificatie,
        };

    private static Doelgroep Map(Registratiedata.Doelgroep doelgroep, string vCode)
        => new()
        {
            JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.Doelgroep, vCode),
            Minimumleeftijd = doelgroep.Minimumleeftijd,
            Maximumleeftijd = doelgroep.Maximumleeftijd,
        };

    public void Handle(EventEnvelope<MaatschappelijkeZetelWerdOvergenomenUitKbo> message, VerenigingZoekDocument document)
    {
        await _elasticRepository.AppendLocatie<VerenigingZoekDocument>(
            message.VCode,
            Map(message.Data.Locatie, message.VCode),
            message.Sequence);
    }

    public void Handle(EventEnvelope<MaatschappelijkeZetelVolgensKBOWerdGewijzigd> message, VerenigingZoekDocument document)
    {
        await _elasticRepository.UpdateLocatie<VerenigingZoekDocument>(
            message.VCode,
            new VerenigingZoekDocument.Types.Locatie
            {
                JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.Locatie, message.VCode, message.Data.LocatieId.ToString()),
                LocatieId = message.Data.LocatieId,
                Naam = message.Data.Naam,
                IsPrimair = message.Data.IsPrimair,
            },
            message.Sequence);
    }

    public void Handle(EventEnvelope<VerenigingWerdGestopt> message, VerenigingZoekDocument document)
    {
        document.
                Status = VerenigingStatus.Gestopt,
                Einddatum = message.Data.Einddatum.ToString(WellknownFormats.DateOnly),
            },
            message.Sequence);
    }

    public void Handle(EventEnvelope<VerenigingWerdGestoptInKBO> message, VerenigingZoekDocument document)
    {
        document.
                Status = VerenigingStatus.Gestopt,
                Einddatum = message.Data.Einddatum.ToString(WellknownFormats.DateOnly),
            },
            message.Sequence);
    }

    public void Handle(EventEnvelope<VerenigingWerdVerwijderd> message, VerenigingZoekDocument document)
    {
        document.
                IsVerwijderd = true,
            },
            message.Sequence);
    }

    public void Handle(EventEnvelope<NaamWerdGewijzigdInKbo> message, VerenigingZoekDocument document)
        => document.
                Naam = message.Data.Naam,
            },
            message.Sequence
        );

    public void Handle(EventEnvelope<KorteNaamWerdGewijzigdInKbo> message, VerenigingZoekDocument document)
        => document.
                KorteNaam = message.Data.KorteNaam,
            },
            message.Sequence
        );

    public void Handle(EventEnvelope<RechtsvormWerdGewijzigdInKBO> message, VerenigingZoekDocument document)
        => document.
                Verenigingstype = new VerenigingZoekDocument.Types.VerenigingsType
                {
                    Code = Verenigingstype.Parse(message.Data.Rechtsvorm).Code,
                    Naam = Verenigingstype.Parse(message.Data.Rechtsvorm).Naam,
                },
            },
            message.Sequence
        );

    public void Handle(EventEnvelope<MaatschappelijkeZetelWerdGewijzigdInKbo> message, VerenigingZoekDocument document)
    {
        await _elasticRepository.UpdateLocatie<VerenigingZoekDocument>(
            message.VCode,
            Map(message.Data.Locatie, message.VCode),
            message.Sequence);
    }

    public void Handle(EventEnvelope<MaatschappelijkeZetelWerdVerwijderdUitKbo> message, VerenigingZoekDocument document)
    {
        await _elasticRepository.RemoveLocatie<VerenigingZoekDocument>(
            message.VCode,
            message.Data.Locatie.LocatieId,
            message.Sequence);
    }

    public void Handle(EventEnvelope<AdresWerdOvergenomenUitAdressenregister> message, VerenigingZoekDocument document)
    {
        await _elasticRepository.UpdateAdres<VerenigingZoekDocument>(
            message.VCode,
            message.Data.LocatieId,
            message.Data.Adres.ToAdresString(),
            message.Data.Adres.Postcode,
            message.Data.Adres.Gemeente,
            message.Sequence);
    }

    public void Handle(EventEnvelope<AdresWerdGewijzigdInAdressenregister> message, VerenigingZoekDocument document)
    {
        await _elasticRepository.UpdateAdres<VerenigingZoekDocument>(
            message.VCode,
            message.Data.LocatieId,
            message.Data.Adres.ToAdresString(),
            message.Data.Adres.Postcode,
            message.Data.Adres.Gemeente,
            message.Sequence);
    }

    public void Handle(EventEnvelope<LocatieDuplicaatWerdVerwijderdNaAdresMatch> message, VerenigingZoekDocument document)
    {
        await _elasticRepository.RemoveLocatie<VerenigingZoekDocument>(
            message.VCode,
            message.Data.VerwijderdeLocatieId,
            message.Sequence);
    }

    public void Handle(EventEnvelope<VerenigingWerdGemarkeerdAlsDubbelVan> message, VerenigingZoekDocument document)
    {
        document.
                IsDubbel = true,
            },
            message.Sequence);
    }

    public void Handle(EventEnvelope<VerenigingAanvaarddeDubbeleVereniging> message, VerenigingZoekDocument document)
    {
        await _elasticRepository.AppendCorresponderendeVCodes<VerenigingZoekDocument>(
            message.VCode,
            message.Data.VCodeDubbeleVereniging,
            message.Sequence);
    }

    public void Handle(EventEnvelope<MarkeringDubbeleVerengingWerdGecorrigeerd> message, VerenigingZoekDocument document)
    {
        document.
                IsDubbel = false,
            },
            message.Sequence);
    }

    public void Handle(EventEnvelope<VerenigingAanvaarddeCorrectieDubbeleVereniging> message, VerenigingZoekDocument document)
    {
        await _elasticRepository.RemoveCorresponderendeVCode<VerenigingZoekDocument>(
            message.VCode,
            message.Data.VCodeDubbeleVereniging,
            message.Sequence);
    }

    public void Handle(EventEnvelope<WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt> message, VerenigingZoekDocument document)
    {
        document.
                IsDubbel = false,
            },
            message.Sequence);
    }

    public void Handle(EventEnvelope<FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid> message, VerenigingZoekDocument document)
    {
        document.
                Verenigingstype = new VerenigingZoekDocument.Types.VerenigingsType
                {
                    Code = Verenigingstype.VZER.Code,
                    Naam = Verenigingstype.VZER.Naam,
                },
                Verenigingssubtype = new VerenigingZoekDocument.Types.Verenigingssubtype
                {
                    Code = string.Empty,
                    Naam = string.Empty,
                },
            },
            message.Sequence);
    }

    public void Handle(EventEnvelope<VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging> message, VerenigingZoekDocument document)
    {
        await _elasticRepository.UpdateVerenigingsTypeAndClearSubverenigingVan<VerenigingZoekDocument>(
            message.VCode,
            VerenigingssubtypeCode.FeitelijkeVereniging.Code,
            VerenigingssubtypeCode.FeitelijkeVereniging.Naam,
            message.Sequence
        );
    }

    public void Handle(EventEnvelope<VerenigingssubtypeWerdVerfijndNaarSubvereniging> @event)
    {
        await _elasticRepository.UpdateAsync(
            @event.VCode,
            new VerenigingZoekUpdateDocument
            {
                Verenigingssubtype = new VerenigingZoekDocument.Types.Verenigingssubtype
                {
                    Code = VerenigingssubtypeCode.Subvereniging.Code,
                    Naam = VerenigingssubtypeCode.Subvereniging.Naam,
                },
                SubverenigingVan = new VerenigingZoekDocument.Types.SubverenigingVan
                {
                    AndereVereniging = @event.Data.SubverenigingVan.AndereVereniging,
                    Identificatie = @event.Data.SubverenigingVan.Identificatie,
                    Beschrijving = @event.Data.SubverenigingVan.Beschrijving,
                }
            },
            @event.Sequence);
    }

    public void Handle(EventEnvelope<VerenigingssubtypeWerdTerugGezetNaarNietBepaald> message, VerenigingZoekDocument document)
    {
        await _elasticRepository.UpdateVerenigingsTypeAndClearSubverenigingVan<VerenigingZoekDocument>(
            message.VCode,
            VerenigingssubtypeCode.NietBepaald.Code,
            VerenigingssubtypeCode.NietBepaald.Naam,
            message.Sequence
        );
    }

    public void Handle(EventEnvelope<SubverenigingRelatieWerdGewijzigd> @event)
    {
        await _elasticRepository.UpdateSubverenigingVanRelatie<VerenigingZoekDocument>(
            @event.VCode,
            @event.Data.AndereVereniging,
            @event.Sequence);
    }

    public void Handle(EventEnvelope<SubverenigingDetailsWerdenGewijzigd> @event)
    {
        await _elasticRepository.UpdateSubverenigingVanDetail<VerenigingZoekDocument>(
            @event.VCode,
            @event.Data.Identificatie,
            @event.Data.Beschrijving,
            @event.Sequence);
    }

    private static JsonLdMetadata CreateJsonLdMetadata(JsonLdType jsonLdType, params string[] values)
        => new()
        {
            Id = jsonLdType.CreateWithIdValues(values),
            Type = jsonLdType.Type,
        };

    public void Handle(EventEnvelope<GeotagsWerdenBepaald> message, VerenigingZoekDocument updateDocument)
    {
        updateDocument.Geotags = message.Data.Geotags.Select(x => new VerenigingZoekDocument.Types.Geotag(x.Identificiatie)).ToArray();
    }
}
