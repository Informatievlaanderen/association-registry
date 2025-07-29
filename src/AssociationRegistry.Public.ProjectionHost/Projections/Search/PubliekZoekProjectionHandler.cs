namespace AssociationRegistry.Public.ProjectionHost.Projections.Search;

using Events;
using Formats;
using JasperFx.Core.Reflection;
using JsonLdContext;
using Schema.Detail;
using Schema.Search;
using Vereniging;
using Doelgroep = Schema.Search.VerenigingZoekDocument.Types.Doelgroep;
using VerenigingStatus = Schema.Constants.VerenigingStatus;

public class PubliekZoekProjectionHandler
{
    public PubliekZoekProjectionHandler()
    {
    }

    public void Handle(EventEnvelope<FeitelijkeVerenigingWerdGeregistreerd> message, VerenigingZoekDocument document)
    {
        CreateVerenigingZonderEigenRechtspersoonDocument(message, document);
    }

    public void Handle(
        EventEnvelope<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd> message,
        VerenigingZoekDocument document)
    {
        CreateVerenigingZonderEigenRechtspersoonDocument(message, document);
    }

    private void CreateVerenigingZonderEigenRechtspersoonDocument<TEvent>(
        EventEnvelope<TEvent> message,
        VerenigingZoekDocument document)
        where TEvent : IVerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd
    {
        document.JsonLdMetadataType = JsonLdType.FeitelijkeVereniging.Type;
        document.VCode = message.Data.VCode;
        document.Verenigingstype = MapVerenigingstype(message.Data);
        document.Verenigingssubtype = MapVerenigingssubtype(message.Data);
        document.Naam = message.Data.Naam;
        document.KorteNaam = message.Data.KorteNaam;
        document.KorteBeschrijving = message.Data.KorteBeschrijving;
        document.Status = VerenigingStatus.Actief;
        document.IsUitgeschrevenUitPubliekeDatastroom = message.Data.IsUitgeschrevenUitPubliekeDatastroom;
        document.Doelgroep = Map(message.Data.Doelgroep, message.Data.VCode);
        document.Locaties = message.Data.Locaties.Select(locatie => Map(locatie, message.VCode)).ToArray();

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

        document.Sleutels =
            new[]
            {
                new VerenigingZoekDocument.Types.Sleutel
                {
                    JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.Sleutel, message.Data.VCode, Sleutelbron.VR),
                    Bron = Sleutelbron.VR,
                    Waarde = message.Data.VCode,
                    CodeerSysteem = CodeerSysteem.VR,
                    GestructureerdeIdentificator = new VerenigingZoekDocument.Types.GestructureerdeIdentificator
                    {
                        JsonLdMetadata =
                            CreateJsonLdMetadata(JsonLdType.GestructureerdeSleutel, message.Data.VCode, Sleutelbron.VR),
                        Nummer = message.Data.VCode,
                    },
                },
            };

        document.Relaties = [];
    }

    private static VerenigingZoekDocument.Types.Verenigingstype MapVerenigingstype(
        IVerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd @event)
    {
        return @event switch
        {
            FeitelijkeVerenigingWerdGeregistreerd => new VerenigingZoekDocument.Types.Verenigingstype()
            {
                Code = Verenigingstype.FeitelijkeVereniging.Code,
                Naam = Verenigingstype.FeitelijkeVereniging.Naam,
            },
            VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd => new VerenigingZoekDocument.Types.Verenigingstype()
            {
                Code = Verenigingstype.VZER.Code,
                Naam = Verenigingstype.VZER.Naam,
            },
            _ => throw new ArgumentOutOfRangeException(nameof(@event))
        };
    }

    private static VerenigingZoekDocument.Types.Verenigingssubtype MapVerenigingssubtype(
        IVerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd @event)
    {
        return @event switch
        {
            FeitelijkeVerenigingWerdGeregistreerd => null,
            VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd => new VerenigingZoekDocument.Types.Verenigingssubtype()
            {
                Code = string.Empty,
                Naam = string.Empty,
            },
            _ => throw new ArgumentOutOfRangeException(nameof(@event))
        };
    }

    public void Handle(EventEnvelope<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd> message, VerenigingZoekDocument document)
    {

                document.JsonLdMetadataType = JsonLdType.VerenigingMetRechtspersoonlijkheid.Type;
                document.VCode = message.Data.VCode;

                document.Verenigingstype = new VerenigingZoekDocument.Types.Verenigingstype
                {
                    Code = Verenigingstype.Parse(message.Data.Rechtsvorm).Code,
                    Naam = Verenigingstype.Parse(message.Data.Rechtsvorm).Naam,
                };
                document.Verenigingssubtype = null;
                document.Naam = message.Data.Naam;
                document.Roepnaam = string.Empty;
                document.KorteNaam = message.Data.KorteNaam;
                document.KorteBeschrijving = string.Empty;
                document.Status = VerenigingStatus.Actief;

                document.Doelgroep = new Doelgroep
                {
                    JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.Doelgroep, message.VCode),
                    Minimumleeftijd = AssociationRegistry.Vereniging.Doelgroep.StandaardMinimumleeftijd,
                    Maximumleeftijd = AssociationRegistry.Vereniging.Doelgroep.StandaardMaximumleeftijd,
                };
                document.Locaties = Array.Empty<VerenigingZoekDocument.Types.Locatie>();
                document.HoofdactiviteitenVerenigingsloket = Array.Empty<VerenigingZoekDocument.Types.HoofdactiviteitVerenigingsloket>();
                document.Werkingsgebieden = Array.Empty<VerenigingZoekDocument.Types.Werkingsgebied>();
                document.Lidmaatschappen = [];

                document.Sleutels =
                [
                    new VerenigingZoekDocument.Types.Sleutel
                    {
                        JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.Sleutel, message.Data.VCode, Sleutelbron.VR),
                        Bron = Sleutelbron.VR,
                        Waarde = message.Data.VCode,
                        CodeerSysteem = CodeerSysteem.VR,
                        GestructureerdeIdentificator = new VerenigingZoekDocument.Types.GestructureerdeIdentificator
                        {
                            JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.GestructureerdeSleutel, message.Data.VCode, Sleutelbron.VR),
                            Nummer = message.Data.VCode,
                        },
                    },
                    new VerenigingZoekDocument.Types.Sleutel
                    {
                        JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.Sleutel, message.Data.VCode, Sleutelbron.KBO),
                        Bron = Sleutelbron.KBO,
                        Waarde = message.Data.KboNummer,
                        CodeerSysteem = CodeerSysteem.KBO,
                        GestructureerdeIdentificator = new VerenigingZoekDocument.Types.GestructureerdeIdentificator
                        {
                            JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.GestructureerdeSleutel, message.Data.VCode, Sleutelbron.KBO),
                            Nummer = message.Data.KboNummer,
                        },
                    },
                ];

                document.Relaties = [];
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

    public void Handle(EventEnvelope<KorteBeschrijvingWerdGewijzigd> message, VerenigingZoekDocument document)
    {
        document.KorteBeschrijving = message.Data.KorteBeschrijving;
    }

    public void Handle(EventEnvelope<DoelgroepWerdGewijzigd> message, VerenigingZoekDocument document)
    {
        document.Doelgroep = new Doelgroep()
        {
            JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.Doelgroep, message.VCode),
            Minimumleeftijd = message.Data.Doelgroep.Minimumleeftijd,
            Maximumleeftijd = message.Data.Doelgroep.Maximumleeftijd,
        };
    }

    public void Handle(EventEnvelope<HoofdactiviteitenVerenigingsloketWerdenGewijzigd> message, VerenigingZoekDocument document)
    {
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
    }

    public void Handle(EventEnvelope<WerkingsgebiedenWerdenNietBepaald> message, VerenigingZoekDocument document)
    {
        document.Werkingsgebieden = [];
    }

    public void Handle(EventEnvelope<WerkingsgebiedenWerdenBepaald> message, VerenigingZoekDocument document)
    {
        document.Werkingsgebieden = message.Data.Werkingsgebieden
                                           .Select(
                                                werkingsgebied =>
                                                    new VerenigingZoekDocument.Types.Werkingsgebied()
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
        document.Werkingsgebieden = message.Data.Werkingsgebieden
                                           .Select(
                                                werkingsgebied =>
                                                    new VerenigingZoekDocument.Types.Werkingsgebied()
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

    public void Handle(EventEnvelope<WerkingsgebiedenWerdenNietVanToepassing> message, VerenigingZoekDocument document)
    {
        document.Werkingsgebieden =
        [
            new VerenigingZoekDocument.Types.Werkingsgebied()
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

    public void Handle(EventEnvelope<VerenigingWerdUitgeschrevenUitPubliekeDatastroom> message, VerenigingZoekDocument document)
    {
        document.IsUitgeschrevenUitPubliekeDatastroom = true;
    }

    public void Handle(EventEnvelope<VerenigingWerdIngeschrevenInPubliekeDatastroom> message, VerenigingZoekDocument document)
    {
        document.IsUitgeschrevenUitPubliekeDatastroom = false;
    }

    public void Handle(EventEnvelope<LocatieWerdToegevoegd> message, VerenigingZoekDocument document)
    {
        document.Locaties = document.Locaties.Append(Map(message.Data.Locatie, message.VCode)).ToArray();
    }

    public void Handle(EventEnvelope<LocatieWerdGewijzigd> message, VerenigingZoekDocument document)
    {
        document.Locaties = document.Locaties
                                    .Where(x => x.LocatieId != message.Data.Locatie.LocatieId)
                                    .Append(Map(message.Data.Locatie, message.VCode))
                                    .ToArray();
    }

    public void Handle(EventEnvelope<LocatieWerdVerwijderd> message, VerenigingZoekDocument document)
    {
        document.Locaties = document.Locaties
                                    .Where(x => x.LocatieId != message.Data.Locatie.LocatieId)
                                    .ToArray();
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

    private static VerenigingZoekDocument.Types.Locatie Map(
        VerenigingZoekDocument.Types.Locatie locatie,
        Registratiedata.AdresUitAdressenregister adresUitAdressenregister,
        string vCode)
        => new()
        {
            JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.Locatie, vCode, locatie.LocatieId.ToString()),
            LocatieId = locatie.LocatieId,
            Locatietype = locatie.Locatietype,
            Naam = locatie.Naam,
            Adresvoorstelling = adresUitAdressenregister.ToAdresString(),
            IsPrimair = locatie.IsPrimair,
            Postcode = adresUitAdressenregister.Postcode ?? string.Empty,
            Gemeente = adresUitAdressenregister.Gemeente ?? string.Empty,
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
        document.Locaties = document.Locaties.Append(Map(message.Data.Locatie, message.VCode)).ToArray();
    }

    public void Handle(EventEnvelope<MaatschappelijkeZetelVolgensKBOWerdGewijzigd> message, VerenigingZoekDocument document)
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

    public void Handle(EventEnvelope<MaatschappelijkeZetelWerdGewijzigdInKbo> message, VerenigingZoekDocument document)
    {
        document.Locaties = document.Locaties
                                    .Where(x => x.LocatieId != message.Data.Locatie.LocatieId)
                                    .Append(Map(message.Data.Locatie, message.VCode))
                                    .ToArray();
    }

    public void Handle(EventEnvelope<MaatschappelijkeZetelWerdVerwijderdUitKbo> message, VerenigingZoekDocument document)
    {
        document.Locaties = document.Locaties
                                    .Where(x => x.LocatieId != message.Data.Locatie.LocatieId)
                                    .ToArray();
    }

    public void Handle(EventEnvelope<VerenigingWerdGestopt> message, VerenigingZoekDocument document)
    {
        document.Status = VerenigingStatus.Gestopt;
    }

    public void Handle(EventEnvelope<VerenigingWerdGestoptInKBO> message, VerenigingZoekDocument document)
    {
        document.Status = VerenigingStatus.Gestopt;
    }

    public void Handle(EventEnvelope<VerenigingWerdVerwijderd> message, VerenigingZoekDocument document)
    {
        document.IsVerwijderd = true;
    }

    public void Handle(EventEnvelope<NaamWerdGewijzigdInKbo> message, VerenigingZoekDocument document)
    {
        document.Naam = message.Data.Naam;
    }

    public void Handle(EventEnvelope<KorteNaamWerdGewijzigdInKbo> message, VerenigingZoekDocument document)
    {
        document.KorteNaam = message.Data.KorteNaam;
    }

    public void Handle(EventEnvelope<RechtsvormWerdGewijzigdInKBO> message, VerenigingZoekDocument document)
    {
        document.Verenigingstype = new VerenigingZoekDocument.Types.Verenigingstype
        {
            Code = Verenigingstype.Parse(message.Data.Rechtsvorm).Code,
            Naam = Verenigingstype.Parse(message.Data.Rechtsvorm).Naam,
        };
    }

    public void Handle(EventEnvelope<AdresWerdOvergenomenUitAdressenregister> message, VerenigingZoekDocument document)
    {
        var locatie = document.Locaties.Single(x => x.LocatieId == message.Data.LocatieId);

        locatie.JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.Locatie, message.VCode, message.Data.LocatieId.ToString());
        locatie.LocatieId = message.Data.LocatieId;
        locatie.Adresvoorstelling = message.Data.Adres.ToAdresString();
        locatie.Gemeente = message.Data.Adres.Gemeente;
        locatie.Postcode = message.Data.Adres.Postcode;

        document.Locaties = document.Locaties
                                    .Where(x => x.LocatieId != message.Data.LocatieId)
                                    .Append(locatie)
                                    .ToArray();
    }

    public void Handle(EventEnvelope<AdresWerdGewijzigdInAdressenregister> message, VerenigingZoekDocument document)
    {
        var locatie = document.Locaties.Single(x => x.LocatieId == message.Data.LocatieId);

        locatie.JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.Locatie, message.VCode, message.Data.LocatieId.ToString());
        locatie.LocatieId = message.Data.LocatieId;
        locatie.Adresvoorstelling = message.Data.Adres.ToAdresString();
        locatie.Gemeente = message.Data.Adres.Gemeente;
        locatie.Postcode = message.Data.Adres.Postcode;

        document.Locaties = document.Locaties
                                    .Where(x => x.LocatieId != message.Data.LocatieId)
                                    .Append(locatie)
                                    .ToArray();
    }

    public void Handle(EventEnvelope<LocatieDuplicaatWerdVerwijderdNaAdresMatch> message, VerenigingZoekDocument document)
    {
        document.Locaties = document.Locaties
                                    .Where(x => x.LocatieId != message.Data.VerwijderdeLocatieId)
                                    .ToArray();
    }

    public void Handle(EventEnvelope<LidmaatschapWerdToegevoegd> message, VerenigingZoekDocument document)
    {
        document.Lidmaatschappen = document.Lidmaatschappen.Append(Map(message.Data.Lidmaatschap, message.VCode)).ToArray();
    }

    public void Handle(EventEnvelope<LidmaatschapWerdGewijzigd> message, VerenigingZoekDocument document)
    {
        document.Lidmaatschappen = document.Lidmaatschappen
                                           .Where(x => x.LidmaatschapId != message.Data.Lidmaatschap.LidmaatschapId)
                                           .Append(Map(message.Data.Lidmaatschap, message.VCode))
                                           .ToArray();
    }

    public void Handle(EventEnvelope<LidmaatschapWerdVerwijderd> message, VerenigingZoekDocument document)
    {
        document.Lidmaatschappen = document.Lidmaatschappen
                                           .Where(x => x.LidmaatschapId != message.Data.Lidmaatschap.LidmaatschapId)
                                           .ToArray();
    }

    public void Handle(EventEnvelope<VerenigingWerdGemarkeerdAlsDubbelVan> message, VerenigingZoekDocument document)
    {
        document.IsDubbel = true;
    }

    public void Handle(EventEnvelope<WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt> message, VerenigingZoekDocument document)
    {
        document.IsDubbel = false;
    }

    public void Handle(EventEnvelope<MarkeringDubbeleVerengingWerdGecorrigeerd> message, VerenigingZoekDocument document)
    {
        document.IsDubbel = false;
    }

    public void Handle(EventEnvelope<VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging> message, VerenigingZoekDocument document)
    {
        document.Verenigingssubtype = new VerenigingZoekDocument.Types.Verenigingssubtype{
            Code = VerenigingssubtypeCode.FeitelijkeVereniging.Code,
            Naam = VerenigingssubtypeCode.FeitelijkeVereniging.Naam,
        };

        document.SubverenigingVan = null;
    }

    public void Handle(EventEnvelope<VerenigingssubtypeWerdTerugGezetNaarNietBepaald> message, VerenigingZoekDocument document)
    {
        document.Verenigingssubtype = new VerenigingZoekDocument.Types.Verenigingssubtype
        {
            Code = VerenigingssubtypeCode.NietBepaald.Code,
            Naam = VerenigingssubtypeCode.NietBepaald.Naam,
        };

        document.SubverenigingVan = null;
    }

    public void Handle(EventEnvelope<VerenigingssubtypeWerdVerfijndNaarSubvereniging> message, VerenigingZoekDocument document)
    {
        document.Verenigingssubtype = new VerenigingZoekDocument.Types.Verenigingssubtype
        {
            Code = VerenigingssubtypeCode.Subvereniging.Code,
            Naam = VerenigingssubtypeCode.Subvereniging.Naam,
        };

        document.SubverenigingVan = new VerenigingZoekDocument.Types.SubverenigingVan
        {
            AndereVereniging = message.Data.SubverenigingVan.AndereVereniging,
            Identificatie = message.Data.SubverenigingVan.Identificatie,
            Beschrijving = message.Data.SubverenigingVan.Beschrijving,
        };
    }

    public void Handle(EventEnvelope<SubverenigingRelatieWerdGewijzigd> message, VerenigingZoekDocument document)
    {
        document.SubverenigingVan!.AndereVereniging = message.Data.AndereVereniging;
    }

    public void Handle(EventEnvelope<SubverenigingDetailsWerdenGewijzigd> message, VerenigingZoekDocument document)
    {
        document.SubverenigingVan!.Identificatie = message.Data.Identificatie;
        document.SubverenigingVan.Beschrijving = message.Data.Beschrijving;
    }

    public void Handle(EventEnvelope<FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid> message, VerenigingZoekDocument document)
    {
        document.Verenigingstype = new VerenigingZoekDocument.Types.Verenigingstype
        {
            Code = Verenigingstype.VZER.Code,
            Naam = Verenigingstype.VZER.Naam,
        };

        document.Verenigingssubtype = new VerenigingZoekDocument.Types.Verenigingssubtype
        {
            Code = string.Empty,
            Naam = string.Empty,
        };
    }

    public void Handle(EventEnvelope<GeotagsWerdenBepaald> message, VerenigingZoekDocument document)
    {
        document.Geotags = message.Data.Geotags.Select(x => new VerenigingZoekDocument.Types.Geotag(x.Identificiatie)).ToArray();
    }

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

    private static JsonLdMetadata CreateJsonLdMetadata(JsonLdType jsonLdType, params string[] values)
        => new()
        {
            Id = jsonLdType.CreateWithIdValues(values),
            Type = jsonLdType.Type,
        };
}
