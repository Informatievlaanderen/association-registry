using AssociationRegistry.Admin.Schema.Search;

namespace AssociationRegistry.Admin.ProjectionHost.Projections.Search.Zoeken;

using Events;
using Formats;
using JsonLdContext;
using Schema;
using Schema.Search;
using Vereniging;
using Doelgroep = Schema.Search.VerenigingZoekDocumentV2.Types.Doelgroep;
using VerenigingStatus = Schema.Constants.VerenigingStatus;
using WellknownFormats = Constants.WellknownFormats;

public class BeheerZoekProjectionHandlerV2
{
    public BeheerZoekProjectionHandlerV2(
    )
    {
    }

   public void Handle(EventEnvelope<FeitelijkeVerenigingWerdGeregistreerd> message, VerenigingZoekDocumentV2 document)
    {
        document.Sequence = message.Sequence;
        document.JsonLdMetadataType = JsonLdType.FeitelijkeVereniging.Type;
        document.VCode = message.Data.VCode;

        document.Verenigingstype = new VerenigingZoekDocumentV2.Types.VerenigingsType
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
                                                                     new VerenigingZoekDocumentV2.Types.HoofdactiviteitVerenigingsloket
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
            new VerenigingZoekDocumentV2.Types.Sleutel
            {
                JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.Sleutel, message.VCode, Sleutelbron.VR.Waarde),
                Bron = Sleutelbron.VR,
                Waarde = message.Data.VCode,
                CodeerSysteem = CodeerSysteem.VR,
                GestructureerdeIdentificator = new VerenigingZoekDocumentV2.Types.GestructureerdeIdentificator
                {
                    JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.GestructureerdeSleutel, message.VCode, Sleutelbron.VR.Waarde),
                    Nummer = message.Data.VCode,
                },
            },
        ];
    }

    public void Handle(
        EventEnvelope<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd> message,
        VerenigingZoekDocumentV2 zoekDocument)
    {
        zoekDocument.Sequence = message.Sequence;
        zoekDocument.JsonLdMetadataType = JsonLdType.FeitelijkeVereniging.Type;
        zoekDocument.VCode = message.Data.VCode;

        zoekDocument.Verenigingstype = new VerenigingZoekDocumentV2.Types.VerenigingsType
        {
            Code = Verenigingstype.VZER.Code,
            Naam = Verenigingstype.VZER.Naam,
        };

        zoekDocument.Verenigingssubtype = new VerenigingZoekDocumentV2.Types.Verenigingssubtype
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
                                                                         new VerenigingZoekDocumentV2.Types.HoofdactiviteitVerenigingsloket
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
            new VerenigingZoekDocumentV2.Types.Sleutel
            {
                JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.Sleutel, message.VCode, Sleutelbron.VR.Waarde),
                Bron = Sleutelbron.VR,
                Waarde = message.Data.VCode,
                CodeerSysteem = CodeerSysteem.VR,
                GestructureerdeIdentificator = new VerenigingZoekDocumentV2.Types.GestructureerdeIdentificator
                {
                    JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.GestructureerdeSleutel, message.VCode, Sleutelbron.VR.Waarde),
                    Nummer = message.Data.VCode,
                },
            },
        ];
    }


    public void Handle(EventEnvelope<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd> message, VerenigingZoekDocumentV2 document)
    {
        document.Sequence = message.Sequence;
        document.JsonLdMetadataType = JsonLdType.VerenigingMetRechtspersoonlijkheid.Type;
        document.VCode = message.Data.VCode;

        document.Verenigingstype = new VerenigingZoekDocumentV2.Types.VerenigingsType
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
            new VerenigingZoekDocumentV2.Types.Sleutel
            {
                JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.Sleutel, message.VCode, Sleutelbron.VR.Waarde),
                Bron = Sleutelbron.VR,
                Waarde = message.Data.VCode,
                CodeerSysteem = CodeerSysteem.VR,
                GestructureerdeIdentificator = new VerenigingZoekDocumentV2.Types.GestructureerdeIdentificator
                {
                    JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.GestructureerdeSleutel, message.VCode, Sleutelbron.VR.Waarde),
                    Nummer = message.Data.VCode,
                },
            },
            new VerenigingZoekDocumentV2.Types.Sleutel
            {
                JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.Sleutel, message.VCode, Sleutelbron.KBO.Waarde),
                Bron = Sleutelbron.KBO,
                Waarde = message.Data.KboNummer,
                CodeerSysteem = CodeerSysteem.KBO,
                GestructureerdeIdentificator = new VerenigingZoekDocumentV2.Types.GestructureerdeIdentificator
                {
                    JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.GestructureerdeSleutel, message.VCode, Sleutelbron.KBO.Waarde),
                    Nummer = message.Data.KboNummer,
                },
            },
        ];
    }

    public void Handle(EventEnvelope<NaamWerdGewijzigd> message, VerenigingZoekDocumentV2 document)
    {
        document.Naam = message.Data.Naam;
    }

    public void Handle(EventEnvelope<RoepnaamWerdGewijzigd> message, VerenigingZoekDocumentV2 document)
    {
        document.Roepnaam = message.Data.Roepnaam;
    }

    public void Handle(EventEnvelope<KorteNaamWerdGewijzigd> message, VerenigingZoekDocumentV2 document)

    {
        document.KorteNaam = message.Data.KorteNaam;
    }

    public void Handle(EventEnvelope<StartdatumWerdGewijzigd> message, VerenigingZoekDocumentV2 document)
        => document.Startdatum = message.Data.Startdatum?.ToString(WellknownFormats.DateOnly);

    public void Handle(EventEnvelope<StartdatumWerdGewijzigdInKbo> message, VerenigingZoekDocumentV2 document)
        => document.Startdatum = message.Data.Startdatum?.ToString(WellknownFormats.DateOnly);

    public void Handle(EventEnvelope<EinddatumWerdGewijzigd> message, VerenigingZoekDocumentV2 document)
        => document.Einddatum = message.Data.Einddatum.ToString(WellknownFormats.DateOnly);

    public void Handle(EventEnvelope<DoelgroepWerdGewijzigd> message, VerenigingZoekDocumentV2 document)
    {
        document.Doelgroep = new Doelgroep()
        {
            Minimumleeftijd = message.Data.Doelgroep.Minimumleeftijd,
            Maximumleeftijd = message.Data.Doelgroep.Maximumleeftijd,
            JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.Doelgroep, message.VCode),
        };
    }


    public void Handle(EventEnvelope<HoofdactiviteitenVerenigingsloketWerdenGewijzigd> message, VerenigingZoekDocumentV2 document)
        => document.HoofdactiviteitenVerenigingsloket = message.Data.HoofdactiviteitenVerenigingsloket
                                                               .Select(hoofdactiviteitVerenigingsloket =>
                                                                           new VerenigingZoekDocumentV2.Types.HoofdactiviteitVerenigingsloket
                                                                           {
                                                                               JsonLdMetadata =
                                                                                   CreateJsonLdMetadata(
                                                                                       JsonLdType.Hoofdactiviteit,
                                                                                       hoofdactiviteitVerenigingsloket.Code),
                                                                               Code = hoofdactiviteitVerenigingsloket.Code,
                                                                               Naam = hoofdactiviteitVerenigingsloket.Naam,
                                                                           })
                                                               .ToArray();

    public void Handle(EventEnvelope<WerkingsgebiedenWerdenNietBepaald> message, VerenigingZoekDocumentV2 document)
    {
        document.Werkingsgebieden = [];

    }

    public void Handle(EventEnvelope<WerkingsgebiedenWerdenBepaald> message, VerenigingZoekDocumentV2 updateDocument)
    {

        updateDocument.Werkingsgebieden = message.Data.Werkingsgebieden
                                                 .Select(
                                                      werkingsgebied =>
                                                          new VerenigingZoekDocumentV2.Types.Werkingsgebied
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

    public void Handle(EventEnvelope<WerkingsgebiedenWerdenGewijzigd> message, VerenigingZoekDocumentV2 document)
    {
        document.Werkingsgebieden = message.Data.Werkingsgebieden
                                           .Select(werkingsgebied =>
                                                       new VerenigingZoekDocumentV2.Types.Werkingsgebied
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

    public void Handle(EventEnvelope<WerkingsgebiedenWerdenNietVanToepassing> message, VerenigingZoekDocumentV2 document)
    {
        document.Werkingsgebieden =
        [
            new VerenigingZoekDocumentV2.Types.Werkingsgebied
            {
                JsonLdMetadata =
                    CreateJsonLdMetadata(
                        JsonLdType.Werkingsgebied,
                        Werkingsgebied.NietVanToepassing.Code),
                Code = Werkingsgebied.NietVanToepassing.Code,
                Naam = Werkingsgebied.NietVanToepassing.Naam,
            },
        ];
    }

    public void Handle(EventEnvelope<VerenigingWerdUitgeschrevenUitPubliekeDatastroom> message, VerenigingZoekDocumentV2 document)
    {
        document.
            IsUitgeschrevenUitPubliekeDatastroom = true;
    }

    public void Handle(EventEnvelope<VerenigingWerdIngeschrevenInPubliekeDatastroom> message, VerenigingZoekDocumentV2 document)
    {
        document.
            IsUitgeschrevenUitPubliekeDatastroom = false;
    }

    public void Handle(EventEnvelope<LocatieWerdToegevoegd> message, VerenigingZoekDocumentV2 document)
    {

        document.Locaties = document.Locaties.Append(Map(message.Data.Locatie, message.VCode)).ToArray();
    }

    public void Handle(EventEnvelope<LocatieWerdGewijzigd> message, VerenigingZoekDocumentV2 document)
    {
        document.Locaties = document.Locaties
                                    .Where(x => x.LocatieId != message.Data.Locatie.LocatieId)
                                    .Append(Map(message.Data.Locatie, message.VCode))
                                    .ToArray();
    }

    public void Handle(EventEnvelope<LocatieWerdVerwijderd> message, VerenigingZoekDocumentV2 document)
    {
        document.Locaties = document.Locaties
                                    .Where(x => x.LocatieId != message.Data.Locatie.LocatieId)
                                    .ToArray();
    }

    public void Handle(EventEnvelope<LidmaatschapWerdToegevoegd> message, VerenigingZoekDocumentV2 document)
    {
        document.Lidmaatschappen = document.Lidmaatschappen.Append(Map(message.Data.Lidmaatschap, message.VCode)).ToArray();
    }

    public void Handle(EventEnvelope<LidmaatschapWerdGewijzigd> message, VerenigingZoekDocumentV2 document)
    {
        document.Lidmaatschappen = document.Lidmaatschappen
                                    .Where(x => x.LidmaatschapId != message.Data.Lidmaatschap.LidmaatschapId)
                                    .Append(Map(message.Data.Lidmaatschap, message.VCode))
                                    .ToArray();
    }

    public void Handle(EventEnvelope<LidmaatschapWerdVerwijderd> message, VerenigingZoekDocumentV2 document)
    {
        document.Lidmaatschappen = document.Lidmaatschappen
                                    .Where(x => x.LidmaatschapId != message.Data.Lidmaatschap.LidmaatschapId)
                                    .ToArray();
    }

    private static VerenigingZoekDocumentV2.Types.Locatie Map(Registratiedata.Locatie locatie, string vCode)
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

    private static VerenigingZoekDocumentV2.Types.Lidmaatschap Map(Registratiedata.Lidmaatschap lidmaatschap, string vCode)
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

    public void Handle(EventEnvelope<MaatschappelijkeZetelWerdOvergenomenUitKbo> message, VerenigingZoekDocumentV2 document)
    {
        document.Locaties = document.Locaties.Append(Map(message.Data.Locatie, message.VCode)).ToArray();

    }

    public void Handle(EventEnvelope<MaatschappelijkeZetelVolgensKBOWerdGewijzigd> message, VerenigingZoekDocumentV2 document)
    {

        var maatschappelijkeZetel = document.Locaties.Single(x => x.LocatieId == message.Data.LocatieId);

        maatschappelijkeZetel.JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.Locatie, message.VCode, message.Data.LocatieId.ToString());
        maatschappelijkeZetel.LocatieId = message.Data.LocatieId;
        maatschappelijkeZetel.Naam = message.Data.Naam;
        maatschappelijkeZetel.IsPrimair = message.Data.IsPrimair;

        document.Locaties = document.Locaties
                                           .Where(x => x.LocatieId != message.Data.LocatieId)
                                           .Append(maatschappelijkeZetel)
                                           .ToArray();
    }

    public void Handle(EventEnvelope<VerenigingWerdGestopt> message, VerenigingZoekDocumentV2 document)
    {
        document.Status = VerenigingStatus.Gestopt;
        document.Einddatum = message.Data.Einddatum.ToString(WellknownFormats.DateOnly);
    }

    public void Handle(EventEnvelope<VerenigingWerdGestoptInKBO> message, VerenigingZoekDocumentV2 document)
    {
        document.
            Status = VerenigingStatus.Gestopt;
        document.
            Einddatum = message.Data.Einddatum.ToString(WellknownFormats.DateOnly);
    }

    public void Handle(EventEnvelope<VerenigingWerdVerwijderd> message, VerenigingZoekDocumentV2 document)
    {
        document.IsVerwijderd = true;
    }

    public void Handle(EventEnvelope<NaamWerdGewijzigdInKbo> message, VerenigingZoekDocumentV2 document)
        => document.
        Naam = message.Data.Naam;


    public void Handle(EventEnvelope<KorteNaamWerdGewijzigdInKbo> message, VerenigingZoekDocumentV2 document)
        => document.
        KorteNaam = message.Data.KorteNaam;


    public void Handle(EventEnvelope<RechtsvormWerdGewijzigdInKBO> message, VerenigingZoekDocumentV2 document)
        => document.
                Verenigingstype = new VerenigingZoekDocumentV2.Types.VerenigingsType
                {
                    Code = Verenigingstype.Parse(message.Data.Rechtsvorm).Code,
                    Naam = Verenigingstype.Parse(message.Data.Rechtsvorm).Naam,
                };


    public void Handle(EventEnvelope<MaatschappelijkeZetelWerdGewijzigdInKbo> message, VerenigingZoekDocumentV2 document)
    {
        document.Locaties = document.Locaties
                                           .Where(x => x.LocatieId != message.Data.Locatie.LocatieId)
                                           .Append(Map(message.Data.Locatie, message.VCode))
                                           .ToArray();
    }

    public void Handle(EventEnvelope<MaatschappelijkeZetelWerdVerwijderdUitKbo> message, VerenigingZoekDocumentV2 document)
    {
        document.Locaties = document.Locaties
                                           .Where(x => x.LocatieId != message.Data.Locatie.LocatieId)
                                           .ToArray();
    }

    public void Handle(EventEnvelope<AdresWerdOvergenomenUitAdressenregister> message, VerenigingZoekDocumentV2 document)
    {

        var maatschappelijkeZetel = document.Locaties.Single(x => x.LocatieId == message.Data.LocatieId);

        maatschappelijkeZetel.JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.Locatie, message.VCode, message.Data.LocatieId.ToString());
        maatschappelijkeZetel.LocatieId = message.Data.LocatieId;
        maatschappelijkeZetel.Adresvoorstelling = message.Data.Adres.ToAdresString();
        maatschappelijkeZetel.Gemeente = message.Data.Adres.Gemeente;
        maatschappelijkeZetel.Postcode = message.Data.Adres.Postcode;

        document.Locaties = document.Locaties
                                    .Where(x => x.LocatieId != message.Data.LocatieId)
                                    .Append(maatschappelijkeZetel)
                                    .ToArray();
    }

    public void Handle(EventEnvelope<AdresWerdGewijzigdInAdressenregister> message, VerenigingZoekDocumentV2 document)
    {
        var maatschappelijkeZetel = document.Locaties.Single(x => x.LocatieId == message.Data.LocatieId);

        maatschappelijkeZetel.JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.Locatie, message.VCode, message.Data.LocatieId.ToString());
        maatschappelijkeZetel.LocatieId = message.Data.LocatieId;
        maatschappelijkeZetel.Adresvoorstelling = message.Data.Adres.ToAdresString();
        maatschappelijkeZetel.Gemeente = message.Data.Adres.Gemeente;
        maatschappelijkeZetel.Postcode = message.Data.Adres.Postcode;

        document.Locaties = document.Locaties
                                    .Where(x => x.LocatieId != message.Data.LocatieId)
                                    .Append(maatschappelijkeZetel)
                                    .ToArray();
    }

    public void Handle(EventEnvelope<LocatieDuplicaatWerdVerwijderdNaAdresMatch> message, VerenigingZoekDocumentV2 document)
    {
        document.Locaties = document.Locaties
                                             .Where(x => x.LocatieId != message.Data.VerwijderdeLocatieId)
                                             .ToArray();
    }

    public void Handle(EventEnvelope<VerenigingWerdGemarkeerdAlsDubbelVan> message, VerenigingZoekDocumentV2 document)
    {
        document.IsDubbel = true;
    }

    public void Handle(EventEnvelope<VerenigingAanvaarddeDubbeleVereniging> message, VerenigingZoekDocumentV2 document)
    {
        document.CorresponderendeVCodes = document.CorresponderendeVCodes.Append(message.Data.VCodeDubbeleVereniging).ToArray();
    }

    public void Handle(EventEnvelope<MarkeringDubbeleVerengingWerdGecorrigeerd> message, VerenigingZoekDocumentV2 document)
    {
        document.IsDubbel = false;
    }

    public void Handle(EventEnvelope<VerenigingAanvaarddeCorrectieDubbeleVereniging> message, VerenigingZoekDocumentV2 document)
    {
        document.CorresponderendeVCodes = document.CorresponderendeVCodes.Where(x => x != message.Data.VCodeDubbeleVereniging).ToArray();
    }

    public void Handle(EventEnvelope<WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt> message, VerenigingZoekDocumentV2 document)
    {
        document.IsDubbel = false;
    }

    public void Handle(EventEnvelope<FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid> message, VerenigingZoekDocumentV2 document)
    {
        document.Verenigingstype = new VerenigingZoekDocumentV2.Types.VerenigingsType
        {
            Code = Verenigingstype.VZER.Code,
            Naam = Verenigingstype.VZER.Naam,
        };

        document.Verenigingssubtype = new VerenigingZoekDocumentV2.Types.Verenigingssubtype
        {
            Code = string.Empty,
            Naam = string.Empty,
        };
    }

    public void Handle(EventEnvelope<VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging> message, VerenigingZoekDocumentV2 document)
    {
        document.Verenigingssubtype = new VerenigingZoekDocumentV2.Types.Verenigingssubtype{
            Code = VerenigingssubtypeCode.FeitelijkeVereniging.Code,
            Naam = VerenigingssubtypeCode.FeitelijkeVereniging.Naam,
        };

        document.SubverenigingVan = null;
    }

    public void Handle(EventEnvelope<VerenigingssubtypeWerdVerfijndNaarSubvereniging> @event, VerenigingZoekDocumentV2 document)
    {
        document.Verenigingssubtype = new VerenigingZoekDocumentV2.Types.Verenigingssubtype
        {
            Code = VerenigingssubtypeCode.Subvereniging.Code,
            Naam = VerenigingssubtypeCode.Subvereniging.Naam,
        };

        document.SubverenigingVan = new VerenigingZoekDocumentV2.Types.SubverenigingVan
        {
            AndereVereniging = @event.Data.SubverenigingVan.AndereVereniging,
            Identificatie = @event.Data.SubverenigingVan.Identificatie,
            Beschrijving = @event.Data.SubverenigingVan.Beschrijving,
        };
    }

    public void Handle(EventEnvelope<VerenigingssubtypeWerdTerugGezetNaarNietBepaald> message, VerenigingZoekDocumentV2 document)
    {

        document.Verenigingssubtype = new VerenigingZoekDocumentV2.Types.Verenigingssubtype
        {
            Code = VerenigingssubtypeCode.NietBepaald.Code,
            Naam = VerenigingssubtypeCode.NietBepaald.Naam,
        };

        document.SubverenigingVan = null;
    }

    public void Handle(EventEnvelope<SubverenigingRelatieWerdGewijzigd> @event, VerenigingZoekDocumentV2 document)
    {
        document.SubverenigingVan!.AndereVereniging = @event.Data.AndereVereniging;
    }

    public void Handle(EventEnvelope<SubverenigingDetailsWerdenGewijzigd> @event, VerenigingZoekDocumentV2 document)
    {
        document.SubverenigingVan!.Identificatie = @event.Data.Identificatie;
        document.SubverenigingVan.Beschrijving = @event.Data.Beschrijving;
    }

    private static JsonLdMetadata CreateJsonLdMetadata(JsonLdType jsonLdType, params string[] values)
        => new()
        {
            Id = jsonLdType.CreateWithIdValues(values),
            Type = jsonLdType.Type,
        };

    public void Handle(EventEnvelope<GeotagsWerdenBepaald> message, VerenigingZoekDocumentV2 updateDocument)
    {
        updateDocument.Geotags = message.Data.Geotags.Select(x => new VerenigingZoekDocumentV2.Types.Geotag(x.Identificiatie)).ToArray();
    }
}
