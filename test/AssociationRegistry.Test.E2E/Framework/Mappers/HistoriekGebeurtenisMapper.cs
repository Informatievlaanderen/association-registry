namespace AssociationRegistry.Test.E2E.Framework.Mappers;

using Admin.Api.WebApi.Verenigingen.Bankrekeningnummers.VoegBankrekeningnummerToe.RequestModels;
using Admin.Api.WebApi.Verenigingen.Common;
using Admin.Api.WebApi.Verenigingen.Contactgegevens.FeitelijkeVereniging.VoegContactGegevenToe.RequestsModels;
using Admin.Api.WebApi.Verenigingen.Dubbelbeheer.FeitelijkeVereniging.MarkeerAlsDubbelVan.RequestModels;
using Admin.Api.WebApi.Verenigingen.Historiek.ResponseModels;
using Admin.Api.WebApi.Verenigingen.Lidmaatschap.VoegLidmaatschapToe.RequestModels;
using Admin.Api.WebApi.Verenigingen.Lidmaatschap.WijzigLidmaatschap.RequestModels;
using Admin.Api.WebApi.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.RequestModels;
using Admin.Api.WebApi.Verenigingen.Vertegenwoordigers.VerenigingOfAnyKind.VoegVertegenwoordigerToe.RequestModels;
using Admin.Api.WebApi.Verenigingen.Vertegenwoordigers.VerenigingOfAnyKind.WijzigVertegenwoordiger.RequestModels;
using Admin.ProjectionHost.Constants;
using Admin.Schema.Historiek.EventData;
using AlbaHost;
using Common.Framework;
using DecentraalBeheer.Vereniging;
using Events;

public static class HistoriekGebeurtenisMapper
{
    public static HistoriekGebeurtenisResponse VerenigingWerdGeregistreerd(
        IRegistreerVereniging request,
        string vCode)
    {
        return new HistoriekGebeurtenisResponse
        {
            Beschrijving = $"Vereniging zonder eigen rechtspersoonlijkheid werd geregistreerd met naam '{request.Naam}'.",
            Gebeurtenis = nameof(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd),
            Data = new VerenigingWerdGeregistreerdData(
                vCode,
                request.Naam,
                request.KorteNaam!,
                request.KorteBeschrijving!,
                Startdatum: request.Startdatum,
                Doelgroep: new Registratiedata.Doelgroep(
                    request.Doelgroep!.Minimumleeftijd!.Value,
                    request.Doelgroep.Maximumleeftijd!.Value),
                IsUitgeschrevenUitPubliekeDatastroom: request
                   .IsUitgeschrevenUitPubliekeDatastroom,
                Contactgegevens: request.Contactgegevens.Select(
                                             (x, i) => new Registratiedata.Contactgegeven(
                                                 i + 1,
                                                 x.Contactgegeventype,
                                                 x.Waarde,
                                                 x.Beschrijving!,
                                                 x.IsPrimair))
                                        .ToArray(),
                Locaties: request.Locaties.Select(
                                      (x, i) => new Registratiedata.Locatie(
                                          i + 1,
                                          x.Locatietype,
                                          x.IsPrimair,
                                          x.Naam!,
                                          x.Adres == null
                                              ? null
                                              : new Registratiedata.Adres(
                                                  x.Adres.Straatnaam,
                                                  x.Adres.Huisnummer,
                                                  x.Adres.Busnummer!,
                                                  x.Adres.Postcode,
                                                  x.Adres.Gemeente,
                                                  x.Adres.Land
                                              ),
                                          x.AdresId == null
                                              ? null
                                              : new Registratiedata.AdresId(x.AdresId.Broncode, x.AdresId.Bronwaarde)))
                                 .ToArray(),
                Vertegenwoordigers: null,
                HoofdactiviteitenVerenigingsloket: null),
            Initiator = AuthenticationSetup.Initiator,
        };
    }

    public static HistoriekGebeurtenisResponse FeitelijkeVerenigingWerdGeregistreerd(
        FeitelijkeVerenigingWerdGeregistreerd feitelijkeVerenigingWerdGeregistreerd)
    {
        return new HistoriekGebeurtenisResponse
        {
            Beschrijving = $"Feitelijke vereniging werd geregistreerd met naam '{feitelijkeVerenigingWerdGeregistreerd.Naam}'.",
            Gebeurtenis = nameof(Events.FeitelijkeVerenigingWerdGeregistreerd),
            Data = new VerenigingWerdGeregistreerdData(
                feitelijkeVerenigingWerdGeregistreerd.VCode,
                feitelijkeVerenigingWerdGeregistreerd.Naam,
                feitelijkeVerenigingWerdGeregistreerd.KorteNaam!,
                feitelijkeVerenigingWerdGeregistreerd.KorteBeschrijving!,
                Startdatum: feitelijkeVerenigingWerdGeregistreerd.Startdatum,
                Doelgroep: new Registratiedata.Doelgroep(
                    feitelijkeVerenigingWerdGeregistreerd.Doelgroep!.Minimumleeftijd,
                    feitelijkeVerenigingWerdGeregistreerd.Doelgroep.Maximumleeftijd),
                IsUitgeschrevenUitPubliekeDatastroom: feitelijkeVerenigingWerdGeregistreerd
                   .IsUitgeschrevenUitPubliekeDatastroom,
                Contactgegevens: feitelijkeVerenigingWerdGeregistreerd.Contactgegevens.Select(
                                                                           (x, i) => new Registratiedata.Contactgegeven(
                                                                               i + 1,
                                                                               x.Contactgegeventype,
                                                                               x.Waarde,
                                                                               x.Beschrijving!,
                                                                               x.IsPrimair))
                                                                      .ToArray(),
                Locaties: feitelijkeVerenigingWerdGeregistreerd.Locaties.Select(
                                                                    (x, i) => new Registratiedata.Locatie(
                                                                        i + 1,
                                                                        x.Locatietype,
                                                                        x.IsPrimair,
                                                                        x.Naam!,
                                                                        x.Adres == null
                                                                            ? null
                                                                            : new Registratiedata.Adres(
                                                                                x.Adres.Straatnaam,
                                                                                x.Adres.Huisnummer,
                                                                                x.Adres.Busnummer!,
                                                                                x.Adres.Postcode,
                                                                                x.Adres.Gemeente,
                                                                                x.Adres.Land
                                                                            ),
                                                                        x.AdresId is null
                                                                            ? null
                                                                            : new Registratiedata.AdresId(
                                                                                x.AdresId.Broncode, x.AdresId.Bronwaarde)))
                                                               .ToArray(),
                Vertegenwoordigers: null,
                HoofdactiviteitenVerenigingsloket: null),
            Initiator = AuthenticationSetup.Initiator,
        };
    }

    public static HistoriekGebeurtenisResponse VerenigingMetRechtspersoonlijkheidWerdGeregistreerd(
        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd verenigingMetRechtspersoonlijkheidWerdGeregistreerd)
    {
        return new HistoriekGebeurtenisResponse
        {
            Beschrijving =
                $"Vereniging met rechtspersoonlijkheid werd geregistreerd met naam '{verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Naam}'.",
            Gebeurtenis = nameof(Events.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd),
            Data = verenigingMetRechtspersoonlijkheidWerdGeregistreerd,
            Initiator = AuthenticationSetup.Initiator,
        };
    }

    public static HistoriekGebeurtenisResponse NaamWerdGewijzigd(
        string vCode,
        string naam)
    {
        var @event = new NaamWerdGewijzigd(vCode, naam);

        return new HistoriekGebeurtenisResponse
        {
            Beschrijving = $"Naam werd gewijzigd naar '{@event.Naam}'.",
            Gebeurtenis = nameof(Events.NaamWerdGewijzigd),
            Data = @event,
            Initiator = AuthenticationSetup.Initiator,
        };
    }

    public static HistoriekGebeurtenisResponse NaamWerdGewijzigdInKBO(
        string naam)
    {
        var @event = new NaamWerdGewijzigdInKbo(naam);

        return new HistoriekGebeurtenisResponse
        {
            Beschrijving = $"Naam werd gewijzigd naar '{@event.Naam}'.",
            Gebeurtenis = nameof(NaamWerdGewijzigdInKbo),
            Data = @event,
            Initiator = AuthenticationSetup.Initiator,
        };
    }

    public static HistoriekGebeurtenisResponse KorteNaamWerdGewijzigd(
        string vCode,
        string naam)
    {
        var @event = new KorteNaamWerdGewijzigd(vCode, naam);

        return new HistoriekGebeurtenisResponse
        {
            Beschrijving = $"Korte naam werd gewijzigd naar '{@event.KorteNaam}'.",
            Gebeurtenis = nameof(Events.KorteNaamWerdGewijzigd),
            Data = @event,
            Initiator = AuthenticationSetup.Initiator,
        };
    }

    public static HistoriekGebeurtenisResponse KorteBeschrijvingWerdGewijzigd(
        string vCode,
        string naam)
    {
        var @event = new KorteBeschrijvingWerdGewijzigd(vCode, naam);

        return new HistoriekGebeurtenisResponse
        {
            Beschrijving = $"Korte beschrijving werd gewijzigd naar '{@event.KorteBeschrijving}'.",
            Gebeurtenis = nameof(Events.KorteBeschrijvingWerdGewijzigd),
            Data = @event,
            Initiator = AuthenticationSetup.Initiator,
        };
    }

    public static HistoriekGebeurtenisResponse VerenigingWerdGestopt(DateOnly eindDatum)
    {
        var @event = new VerenigingWerdGestopt(eindDatum);

        return new HistoriekGebeurtenisResponse
        {
            Beschrijving = $"De vereniging werd gestopt met einddatum '{eindDatum.ToString(WellknownFormats.DateOnly)}'.",
            Gebeurtenis = nameof(Events.VerenigingWerdGestopt),
            Data = @event,
            Initiator = AuthenticationSetup.Initiator,
        };
    }

    public static HistoriekGebeurtenisResponse RoepnaamWerdGewijzigd(
        string roepnaam)
    {
        var @event = new RoepnaamWerdGewijzigd(roepnaam);

        return new HistoriekGebeurtenisResponse
        {
            Beschrijving = $"Roepnaam werd gewijzigd naar '{roepnaam}'.",
            Gebeurtenis = nameof(Events.RoepnaamWerdGewijzigd),
            Data = @event,
            Initiator = AuthenticationSetup.Initiator,
        };
    }

    public static HistoriekGebeurtenisResponse StartdatumWerdGewijzigd(
        string vCode,
        DateOnly dateOnly)
    {
        var @event = new StartdatumWerdGewijzigd(vCode, dateOnly);

        return new HistoriekGebeurtenisResponse
        {
            Beschrijving = $"Startdatum werd gewijzigd naar '{dateOnly.ToString(WellknownFormats.DateOnly)}'.",
            Gebeurtenis = nameof(Events.StartdatumWerdGewijzigd),
            Data = @event,
            Initiator = AuthenticationSetup.Initiator,
        };
    }

    public static HistoriekGebeurtenisResponse DoelgroepWerdGewijzigd(
        DoelgroepRequest doelgroep)
    {
        var @event = new DoelgroepWerdGewijzigd(
            new Registratiedata.Doelgroep(doelgroep.Minimumleeftijd!.Value, doelgroep.Maximumleeftijd!.Value));

        var beschrijving = $"Doelgroep werd gewijzigd naar '{doelgroep.Minimumleeftijd} " +
                           $"- {doelgroep.Maximumleeftijd}'.";

        return new HistoriekGebeurtenisResponse
        {
            Beschrijving = beschrijving,
            Gebeurtenis = nameof(Events.DoelgroepWerdGewijzigd),
            Data = @event,
            Initiator = AuthenticationSetup.Initiator,
        };
    }

    public static HistoriekGebeurtenisResponse HoofdactiviteitenVerenigingsloketWerdenGewijzigd(
        string[] hoofdactiviteiten)
    {
        var @event = new HoofdactiviteitenVerenigingsloketWerdenGewijzigd(hoofdactiviteiten
                                                                         .Select(HoofdactiviteitVerenigingsloket.Create)
                                                                         .Select(wg => new Registratiedata.HoofdactiviteitVerenigingsloket(
                                                                                     wg.Code, wg.Naam))
                                                                         .ToArray());

        return new HistoriekGebeurtenisResponse
        {
            Beschrijving = "Hoofdactiviteiten verenigingsloket werden gewijzigd.",
            Gebeurtenis = nameof(Events.HoofdactiviteitenVerenigingsloketWerdenGewijzigd),
            Data = @event,
            Initiator = AuthenticationSetup.Initiator,
        };
    }

    public static HistoriekGebeurtenisResponse WerkingsgebiedenWerdenNietBepaald(string vCode)
    {
        var @event = new WerkingsgebiedenWerdenNietBepaald(vCode);

        return new HistoriekGebeurtenisResponse
        {
            Beschrijving = "Werkingsgebieden werden niet bepaald.",
            Gebeurtenis = nameof(Events.WerkingsgebiedenWerdenNietBepaald),
            Data = @event,
            Initiator = AuthenticationSetup.Initiator,
        };
    }

    public static HistoriekGebeurtenisResponse WerkingsgebiedenWerdenBepaald(string vCode, string[] werkingsgebieden)
    {
        var werkingsgebiedenServiceMock = new WerkingsgebiedenServiceMock();

        var @event = new WerkingsgebiedenWerdenBepaald(vCode, werkingsgebieden
                                                               .Select(werkingsgebiedenServiceMock.Create)
                                                               .Select(wg => new Registratiedata.Werkingsgebied(wg.Code, wg.Naam))
                                                               .ToArray());

        return new HistoriekGebeurtenisResponse
        {
            Beschrijving = "Werkingsgebieden werden bepaald.",
            Gebeurtenis = nameof(Events.WerkingsgebiedenWerdenBepaald),
            Data = @event,
            Initiator = AuthenticationSetup.Initiator,
        };
    }

    public static HistoriekGebeurtenisResponse WerkingsgebiedenWerdenGewijzigd(string vCode, string[] werkingsgebieden)
    {
        var werkingsgebiedenServiceMock = new WerkingsgebiedenServiceMock();

        var @event = new WerkingsgebiedenWerdenGewijzigd(vCode, werkingsgebieden
                                                        .Select(werkingsgebiedenServiceMock.Create)
                                                        .Select(wg => new Registratiedata.Werkingsgebied(wg.Code, wg.Naam))
                                                        .ToArray());

        return new HistoriekGebeurtenisResponse
        {
            Beschrijving = "Werkingsgebieden werden gewijzigd.",
            Gebeurtenis = nameof(Events.WerkingsgebiedenWerdenGewijzigd),
            Data = @event,
            Initiator = AuthenticationSetup.Initiator,
        };
    }

    public static HistoriekGebeurtenisResponse WerkingsgebiedenWerdenNietVanToepassing(string vCode)
    {
        var @event = new WerkingsgebiedenWerdenNietVanToepassing(vCode);

        return new HistoriekGebeurtenisResponse
        {
            Beschrijving = "Werkingsgebieden werden niet van toepassing.",
            Gebeurtenis = nameof(Events.WerkingsgebiedenWerdenNietVanToepassing),
            Data = @event,
            Initiator = AuthenticationSetup.Initiator,
        };
    }

    private static Registratiedata.Werkingsgebied[] MapWerkingsgebieden(string[] werkingsgebiedenCodes)
    {
        var werkingsgebiedenServiceMock = new WerkingsgebiedenServiceMock();

        return werkingsgebiedenCodes
              .Select(code => werkingsgebiedenServiceMock.Create(code))
              .Select(werkingsgebied => new Registratiedata.Werkingsgebied(werkingsgebied.Code, werkingsgebied.Naam))
              .ToArray();
    }

    public static HistoriekGebeurtenisResponse AdresWerdOvergenomen(string vCode)
        => new()
        {
            Beschrijving = "Adres werd overgenomen uit het adressenregister.",
            Gebeurtenis = nameof(AdresWerdOvergenomenUitAdressenregister),
            Data = new AdresWerdOvergenomenUitAdressenregister(
                vCode,
                LocatieId: 1,
                Adres: new Registratiedata.AdresUitAdressenregister(
                    Straatnaam: "Leopold II-laan",
                    Huisnummer: "99",
                    Busnummer: "",
                    Postcode: "9200",
                    Gemeente: "Dendermonde"),
                AdresId: new Registratiedata.AdresId(Broncode: "AR", Bronwaarde: "https://data.vlaanderen.be/id/adres/3213019")),
            Initiator = WellknownOvoNumbers.DigitaalVlaanderenOvoNumber,
        };

    public static HistoriekGebeurtenisResponse AdresNietUniekInAR(string vCode)
    {
        return new HistoriekGebeurtenisResponse
        {
            Beschrijving = "Adres niet uniek in het adressenregister.",
            Gebeurtenis = nameof(AdresNietUniekInAdressenregister),
            Data = new AdresNietUniekInAdressenregister(
                vCode,
                LocatieId: 2,
                new NietUniekeAdresMatchUitAdressenregister[]
                {
                    new()
                    {
                        AdresId =
                            new Registratiedata.AdresId(Broncode: "AR", Bronwaarde: "https://data.vlaanderen.be/id/adres/3213019"),
                        Adresvoorstelling = "Leopold II-laan 99, 9200 Dendermonde",
                        Score = 95.4942299939528,
                    },
                    new()
                    {
                        AdresId = new Registratiedata.AdresId(Broncode: "AR",
                                                              Bronwaarde: "https://data.vlaanderen.be/id/adres/20055852"),
                        Adresvoorstelling = "Leopold II-laan 99 bus 1, 9200 Dendermonde",
                        Score = 86.987355612507,
                    },
                    new()
                    {
                        AdresId =
                            new Registratiedata.AdresId(Broncode: "AR", Bronwaarde: "https://data.vlaanderen.be/id/adres/5459542"),
                        Adresvoorstelling = "Leopold II-laan 99 bus 2, 9200 Dendermonde",
                        Score = 86.987355612507,
                    },
                    new()
                    {
                        AdresId =
                            new Registratiedata.AdresId(Broncode: "AR", Bronwaarde: "https://data.vlaanderen.be/id/adres/5556720"),
                        Adresvoorstelling = "Leopold II-laan 99 bus 3, 9200 Dendermonde",
                        Score = 86.987355612507,
                    },
                    new()
                    {
                        AdresId =
                            new Registratiedata.AdresId(Broncode: "AR", Bronwaarde: "https://data.vlaanderen.be/id/adres/5435137"),
                        Adresvoorstelling = "Leopold II-laan 99 bus 6, 9200 Dendermonde",
                        Score = 86.987355612507,
                    },
                    new()
                    {
                        AdresId =
                            new Registratiedata.AdresId(Broncode: "AR", Bronwaarde: "https://data.vlaanderen.be/id/adres/5451858"),
                        Adresvoorstelling = "Leopold II-laan 99 bus 7, 9200 Dendermonde",
                        Score = 86.987355612507,
                    },
                    new()
                    {
                        AdresId = new Registratiedata.AdresId(Broncode: "AR",
                                                              Bronwaarde: "https://data.vlaanderen.be/id/adres/19038299"),
                        Adresvoorstelling = "Leopold II-laan 99 bus 8, 9200 Dendermonde",
                        Score = 86.987355612507,
                    },
                    new()
                    {
                        AdresId = new Registratiedata.AdresId(Broncode: "AR",
                                                              Bronwaarde: "https://data.vlaanderen.be/id/adres/20172048"),
                        Adresvoorstelling = "Leopold II-laan 99 bus 9, 9200 Dendermonde",
                        Score = 86.987355612507,
                    },
                    new()
                    {
                        AdresId =
                            new Registratiedata.AdresId(Broncode: "AR", Bronwaarde: "https://data.vlaanderen.be/id/adres/5656904"),
                        Adresvoorstelling = "Leopold II-laan 99 bus 10, 9200 Dendermonde",
                        Score = 86.987355612507,
                    },
                    new()
                    {
                        AdresId =
                            new Registratiedata.AdresId(Broncode: "AR", Bronwaarde: "https://data.vlaanderen.be/id/adres/5512213"),
                        Adresvoorstelling = "Leopold II-laan 99 bus 11, 9200 Dendermonde",
                        Score = 86.987355612507,
                    },
                }),
            Initiator = "OVO002949",
            Tijdstip = "2024-07-30T11:08:04Z",
        };
    }

    public static HistoriekGebeurtenisResponse AdresKonNietOvergenomenWorden(string vCode)
        => new()
        {
            Beschrijving = "Adres kon niet gevonden worden in het adressenregister.",
            Gebeurtenis = nameof(AdresWerdNietGevondenInAdressenregister),
            Data = new
            {
                vCode = vCode,
                locatieId = 3,
                straatnaam = "dorpelstraat",
                huisnummer = "169",
                busnummer = "2",
                postcode = "4567",
                gemeente = "Nothingham",
            },
            Initiator = "OVO002949",
            Tijdstip = "2024-07-30T11:08:05Z",
        };

    public static HistoriekGebeurtenisResponse IsUitgeschrevenUitPubliekeDatastroom(bool? isUitgeschrevenUitPubliekeDatastroom)
    {
        var isUitgeschreven = isUitgeschrevenUitPubliekeDatastroom!.Value;

        return new HistoriekGebeurtenisResponse
        {
            Beschrijving = isUitgeschreven
                ? "Vereniging werd uitgeschreven uit de publieke datastroom."
                : "Vereniging werd ingeschreven in de publieke datastroom.",
            Gebeurtenis = isUitgeschreven
                ? nameof(VerenigingWerdUitgeschrevenUitPubliekeDatastroom)
                : nameof(VerenigingWerdIngeschrevenInPubliekeDatastroom),
            Data = isUitgeschreven
                ? new VerenigingWerdUitgeschrevenUitPubliekeDatastroom()
                : new VerenigingWerdIngeschrevenInPubliekeDatastroom(),
            Initiator = AuthenticationSetup.Initiator,
        };
    }

    public static HistoriekGebeurtenisResponse LidmaatschapWerdToegevoegd(VoegLidmaatschapToeRequest request, string andereVerenigingNaam)
    => new()
        {
            Beschrijving = "Lidmaatschap werd toegevoegd.",
            Gebeurtenis = nameof(Events.LidmaatschapWerdToegevoegd),
            Data = new
            {
                LidmaatschapId = 1,
                AndereVereniging = request.AndereVereniging,
                Van = request.Van,
                Tot = request.Tot,
                Identificatie = request.Identificatie,
                Beschrijving = request.Beschrijving
            },
            Initiator = "OVO002949",
            Tijdstip = "2024-07-30T11:08:05Z",
        };

    public static HistoriekGebeurtenisResponse LidmaatschapWerdGewijzigd(WijzigLidmaatschapRequest request, int lidmaatschapId, string andereVereniging, string andereVerenigingNaam)
    => new()
        {
            Beschrijving = "Lidmaatschap werd gewijzigd.",
            Gebeurtenis = nameof(Events.LidmaatschapWerdGewijzigd),
            Data = new
            {
                LidmaatschapId = lidmaatschapId,
                AndereVereniging = andereVereniging,
                Van = request.Van.Value,
                Tot = request.Tot.Value,
                Identificatie = request.Identificatie,
                Beschrijving = request.Beschrijving
            },
            Initiator = "OVO002949",
            Tijdstip = "2024-07-30T11:08:05Z",
        };

    public static HistoriekGebeurtenisResponse? LidmaatschapWerdVerwijderd(LidmaatschapWerdToegevoegd lidmaatschap)
        => new()
        {
            Beschrijving = "Lidmaatschap werd verwijderd.",
            Gebeurtenis = nameof(Events.LidmaatschapWerdVerwijderd),
            Data = new
            {
                LidmaatschapId = lidmaatschap.Lidmaatschap.LidmaatschapId,
                AndereVereniging = lidmaatschap.Lidmaatschap.AndereVereniging,
                Van = lidmaatschap.Lidmaatschap.DatumVan,
                Tot = lidmaatschap.Lidmaatschap.DatumTot,
                Identificatie = lidmaatschap.Lidmaatschap.Identificatie,
                Beschrijving = lidmaatschap.Lidmaatschap.Beschrijving
            },
            Initiator = "OVO002949",
            Tijdstip = "2024-07-30T11:08:05Z",
        };

    public static HistoriekGebeurtenisResponse? VerenigingWerdGermarkeerdAlsDubbelVan(MarkeerAlsDubbelVanRequest request, string vCode)
        => new()
        {
            Beschrijving = $"Vereniging werd gemarkeerd als dubbel van {request.IsDubbelVan}.",
            Gebeurtenis = nameof(VerenigingWerdGemarkeerdAlsDubbelVan),
            Data = new
            {
                VCode = vCode,
                VCodeAuthentiekeVereniging = request.IsDubbelVan,
            },
            Initiator = "OVO002949",
            Tijdstip = "2024-07-30T11:08:05Z",
        };

    public static HistoriekGebeurtenisResponse? VerenigingAanvaarddeDubbeleVereniging(MarkeerAlsDubbelVanRequest request, string vCode)
        => new()
        {
            Beschrijving = $"Vereniging aanvaardde dubbele vereniging {vCode}.",
            Gebeurtenis = nameof(Events.VerenigingAanvaarddeDubbeleVereniging),
            Data = new
            {
                VCode = request.IsDubbelVan,
                VCodeDubbeleVereniging = vCode,
            },
            Initiator = "OVO002949",
            Tijdstip = "2024-07-30T11:08:05Z",
        };

    public static HistoriekGebeurtenisResponse MarkeringDubbeleVerengingWerdGecorrigeerd(
        VerenigingWerdGemarkeerdAlsDubbelVan verenigingWerdGemarkeerdAlsDubbelVan)
        => new()
        {
            Beschrijving = $"Markering als dubbel van vereniging {verenigingWerdGemarkeerdAlsDubbelVan.VCodeAuthentiekeVereniging} werd gecorrigeerd.",
            Gebeurtenis = nameof(Events.MarkeringDubbeleVerengingWerdGecorrigeerd),
            Data = new
            {
                VCode = verenigingWerdGemarkeerdAlsDubbelVan.VCode,
                VCodeAuthentiekeVereniging = verenigingWerdGemarkeerdAlsDubbelVan.VCodeAuthentiekeVereniging,
            },
            Initiator = "OVO002949",
            Tijdstip = "2024-07-30T11:08:05Z",
        };

    public static HistoriekGebeurtenisResponse? AanvaardingDubbeleVerenigingWerdGecorrigeerd(VerenigingAanvaarddeDubbeleVereniging scenarioVerenigingAanvaarddeDubbeleVereniging)
        => new()
        {
            Beschrijving = $"Vereniging {scenarioVerenigingAanvaarddeDubbeleVereniging.VCodeDubbeleVereniging} werd verwijderd als dubbel door correctie.",
            Gebeurtenis = nameof(VerenigingAanvaarddeCorrectieDubbeleVereniging),
            Data = new
            {
                VCode = scenarioVerenigingAanvaarddeDubbeleVereniging.VCode,
                VCodeDubbeleVereniging = scenarioVerenigingAanvaarddeDubbeleVereniging.VCodeDubbeleVereniging,
            },
            Initiator = "OVO002949",
            Tijdstip = "2024-07-30T11:08:05Z",
        };

    public static HistoriekGebeurtenisResponse? SubTypeWerdVerfijndNaarFeitelijkeVereniging(string vCode)
        => new()
        {
            Beschrijving = $"Subtype werd verfijnd naar feitelijke vereniging.",
            Gebeurtenis = nameof(VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging),
            Data = new
            {
                VCode = vCode,
            },
            Initiator = "OVO002949",
            Tijdstip = "2024-07-30T11:08:05Z",
        };

    public static HistoriekGebeurtenisResponse? SubtypeWerdTerugGezetNaarNietBepaald(string vCode)
        => new()
        {
            Beschrijving = $"Subtype werd teruggezet naar niet bepaald.",
            Gebeurtenis = nameof(VerenigingssubtypeWerdTerugGezetNaarNietBepaald),
            Data = new
            {
                VCode = vCode,
            },
            Initiator = "OVO002949",
            Tijdstip = "2024-07-30T11:08:05Z",
        };

    public static HistoriekGebeurtenisResponse? SubTypeWerdVerfijndNaarSubvereniging(VerenigingssubtypeWerdVerfijndNaarSubvereniging @event)
        => new()
        {
            Beschrijving = $"Subtype werd verfijnd naar subvereniging.",
            Gebeurtenis = nameof(VerenigingssubtypeWerdVerfijndNaarSubvereniging),
            Data = @event,
            Initiator = "OVO002949",
            Tijdstip = "2024-07-30T11:08:05Z",
        };

    public static HistoriekGebeurtenisResponse? SubverenigingRelatieWerdGewijzigd(SubverenigingRelatieWerdGewijzigd @event)
        => new()
        {
            Beschrijving = $"De relatie van het subtype werd gewijzigd.",
            Gebeurtenis = nameof(SubverenigingRelatieWerdGewijzigd),
            Data = @event,
            Initiator = "OVO002949",
            Tijdstip = "2024-07-30T11:08:05Z",
        };

    public static HistoriekGebeurtenisResponse? SubverenigingDetailsWerdenGewijzigd(SubverenigingDetailsWerdenGewijzigd @event)
        => new()
        {
            Beschrijving = $"De details van het subtype werden gewijzigd.",
            Gebeurtenis = nameof(SubverenigingDetailsWerdenGewijzigd),
            Data = @event,
            Initiator = "OVO002949",
            Tijdstip = "2024-07-30T11:08:05Z",
        };

    public static HistoriekGebeurtenisResponse? ContactgegevenWerdToegevoegd(int nextContactgegevenId, VoegContactgegevenToeRequest request)
        => new()
        {
            Beschrijving = $"{request.Contactgegeven.Contactgegeventype} '{request.Contactgegeven.Waarde}' werd toegevoegd.",
            Gebeurtenis = nameof(Events.ContactgegevenWerdToegevoegd),
            Data = new ContactgegevenWerdToegevoegd(nextContactgegevenId, request.Contactgegeven.Contactgegeventype, request.Contactgegeven.Waarde, request.Contactgegeven.Beschrijving, request.Contactgegeven.IsPrimair),
            Initiator = "OVO002949",
            Tijdstip = "2024-07-30T11:08:05Z",
        };

    public static HistoriekGebeurtenisResponse? VertegenwoordigerWerdToegevoegd(int nextVertegenwoordigerId, VoegVertegenwoordigerToeRequest request)
        => new()
        {
            Beschrijving = $"'{request.Vertegenwoordiger.Voornaam} {request.Vertegenwoordiger.Achternaam}' werd toegevoegd als vertegenwoordiger.",
            Gebeurtenis = nameof(Events.VertegenwoordigerWerdToegevoegd),
            Data = new VertegenwoordigerData(nextVertegenwoordigerId,
                                             request.Vertegenwoordiger.IsPrimair,
                                             request.Vertegenwoordiger.Roepnaam,
                                             request.Vertegenwoordiger.Rol,
                                             request.Vertegenwoordiger.Voornaam,
                                             request.Vertegenwoordiger.Achternaam,
                                             request.Vertegenwoordiger.Email,
                                             request.Vertegenwoordiger.Telefoon,
                                             request.Vertegenwoordiger.Mobiel,
                                             request.Vertegenwoordiger.SocialMedia),
            Initiator = "OVO002949",
            Tijdstip = "2024-07-30T11:08:05Z",
        };

    public static HistoriekGebeurtenisResponse? VerenigingWerdVerwijderd(string reden)
        => new()
        {
            Beschrijving = "Deze vereniging werd verwijderd.",
            Gebeurtenis = nameof(Events.VerenigingWerdVerwijderd),
            Data = new VerenigingWerdVerwijderdData(
               reden
            ),
            Initiator = "OVO002949",
            Tijdstip = "2024-07-30T11:08:05Z",
        };

    public static HistoriekGebeurtenisResponse VertegenwoordigerWerdGewijzigd(VertegenwoordigerWerdToegevoegd vertegenwoordigerWerdToegevoegd, WijzigVertegenwoordigerRequest request)
        => new()
        {
            Beschrijving = $"Vertegenwoordiger '{vertegenwoordigerWerdToegevoegd.Voornaam} {vertegenwoordigerWerdToegevoegd.Achternaam}' werd gewijzigd.",
            Gebeurtenis = nameof(Events.VertegenwoordigerWerdGewijzigd),
            Data = new VertegenwoordigerData(vertegenwoordigerWerdToegevoegd.VertegenwoordigerId,
                                             request.Vertegenwoordiger.IsPrimair.Value,
                                             request.Vertegenwoordiger.Roepnaam,
                                             request.Vertegenwoordiger.Rol,
                                             vertegenwoordigerWerdToegevoegd.Voornaam,
                                             vertegenwoordigerWerdToegevoegd.Achternaam,
                                             request.Vertegenwoordiger.Email,
                                             request.Vertegenwoordiger.Telefoon,
                                             request.Vertegenwoordiger.Mobiel,
                                             request.Vertegenwoordiger.SocialMedia),
            Initiator = "OVO002949",
            Tijdstip = "2024-07-30T11:08:05Z",
        };
    public static HistoriekGebeurtenisResponse VertegenwoordigerWerdVerwijderd(VertegenwoordigerWerdToegevoegd vertegenwoordigerWerdToegevoegd)
        => new()
        {
            Beschrijving = $"Vertegenwoordiger '{vertegenwoordigerWerdToegevoegd.Voornaam} {vertegenwoordigerWerdToegevoegd.Achternaam}' werd verwijderd.",
            Gebeurtenis = nameof(Events.VertegenwoordigerWerdVerwijderd),
            Data = new VertegenwoordigerWerdVerwijderdData(vertegenwoordigerWerdToegevoegd.VertegenwoordigerId,
                                                           vertegenwoordigerWerdToegevoegd.Voornaam,
                                                           vertegenwoordigerWerdToegevoegd.Achternaam),
            Initiator = "OVO002949",
            Tijdstip = "2024-07-30T11:08:05Z",
        };

    public static HistoriekGebeurtenisResponse? VertegenwoordigerWerdGewijzigd(VertegenwoordigerWerdOvergenomenUitKBO vertegenwoordigerWerdOvergenomenUitKbo, WijzigVertegenwoordigerRequest request)
        => new()
        {
            Beschrijving = $"Vertegenwoordiger '{vertegenwoordigerWerdOvergenomenUitKbo.Voornaam} {vertegenwoordigerWerdOvergenomenUitKbo.Achternaam}' werd gewijzigd.",
            Gebeurtenis = nameof(Events.VertegenwoordigerWerdGewijzigd),
            Data = new VertegenwoordigerData(vertegenwoordigerWerdOvergenomenUitKbo.VertegenwoordigerId,
                                             request.Vertegenwoordiger.IsPrimair.Value,
                                             request.Vertegenwoordiger.Roepnaam,
                                             request.Vertegenwoordiger.Rol,
                                             vertegenwoordigerWerdOvergenomenUitKbo.Voornaam,
                                             vertegenwoordigerWerdOvergenomenUitKbo.Achternaam,
                                             request.Vertegenwoordiger.Email,
                                             request.Vertegenwoordiger.Telefoon,
                                             request.Vertegenwoordiger.Mobiel,
                                             request.Vertegenwoordiger.SocialMedia),
            Initiator = "OVO002949",
            Tijdstip = "2024-07-30T11:08:05Z",
        };

    public static HistoriekGebeurtenisResponse? VertegenwoordigerWerdOvergenomenUitKBO(VertegenwoordigerWerdOvergenomenUitKBO vertegenwoordigerWerdOvergenomenUitKbo)
        => new()
        {
            Beschrijving = $"Vertegenwoordiger '{vertegenwoordigerWerdOvergenomenUitKbo.Voornaam} {vertegenwoordigerWerdOvergenomenUitKbo.Achternaam}' werd overgenomen uit KBO.",
            Gebeurtenis = nameof(Events.VertegenwoordigerWerdOvergenomenUitKBO),
            Data = new KBOVertegenwoordigerData(vertegenwoordigerWerdOvergenomenUitKbo.VertegenwoordigerId,
                                                vertegenwoordigerWerdOvergenomenUitKbo.Voornaam,
                                                vertegenwoordigerWerdOvergenomenUitKbo.Achternaam),
            Initiator = "OVO002949",
            Tijdstip = "2024-07-30T11:08:05Z",
        };

    public static HistoriekGebeurtenisResponse VertegenwoordigerWerdToegevoegdVanuitKBO(VertegenwoordigerWerdToegevoegdVanuitKBO vertegenwoordigerWerdToegevoegdVanuitKBO)
        => new()
        {
            Beschrijving = $"Vertegenwoordiger '{vertegenwoordigerWerdToegevoegdVanuitKBO.Voornaam} {vertegenwoordigerWerdToegevoegdVanuitKBO.Achternaam}' werd toegevoegd vanuit KBO.",
            Gebeurtenis = nameof(Events.VertegenwoordigerWerdToegevoegdVanuitKBO),
            Data = new KBOVertegenwoordigerData(vertegenwoordigerWerdToegevoegdVanuitKBO.VertegenwoordigerId,
                                                vertegenwoordigerWerdToegevoegdVanuitKBO.Voornaam,
                                                vertegenwoordigerWerdToegevoegdVanuitKBO.Achternaam),
            Initiator = "OVO002949",
            Tijdstip = "2024-07-30T11:08:05Z",
        };

    public static HistoriekGebeurtenisResponse VertegenwoordigerWerdGewijzigdInKBO(VertegenwoordigerWerdGewijzigdInKBO vertegenwoordigerWerdGewijzigdInKBO)
        => new()
        {
            Beschrijving = $"Vertegenwoordiger '{vertegenwoordigerWerdGewijzigdInKBO.Voornaam} {vertegenwoordigerWerdGewijzigdInKBO.Achternaam}' werd gewijzigd in KBO.",
            Gebeurtenis = nameof(Events.VertegenwoordigerWerdGewijzigdInKBO),
            Data = new KBOVertegenwoordigerData(vertegenwoordigerWerdGewijzigdInKBO.VertegenwoordigerId,
                                                vertegenwoordigerWerdGewijzigdInKBO.Voornaam,
                                                vertegenwoordigerWerdGewijzigdInKBO.Achternaam),
            Initiator = "OVO002949",
            Tijdstip = "2024-07-30T11:08:05Z",
        };

    public static HistoriekGebeurtenisResponse? VertegenwoordigerWerdVerwijderdUitKBO(VertegenwoordigerWerdVerwijderdUitKBO vertegenwoordigerWerdVerwijderdUitKbo)
        => new()
        {
            Beschrijving = $"Vertegenwoordiger '{vertegenwoordigerWerdVerwijderdUitKbo.Voornaam} {vertegenwoordigerWerdVerwijderdUitKbo.Achternaam}' werd verwijderd uit KBO.",
            Gebeurtenis = nameof(Events.VertegenwoordigerWerdVerwijderdUitKBO),
            Data = new KBOVertegenwoordigerData(vertegenwoordigerWerdVerwijderdUitKbo.VertegenwoordigerId,
                                                vertegenwoordigerWerdVerwijderdUitKbo.Voornaam,
                                                vertegenwoordigerWerdVerwijderdUitKbo.Achternaam),
            Initiator = "OVO002949",
            Tijdstip = "2024-07-30T11:08:05Z",
        };

    public static HistoriekGebeurtenisResponse? KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden(KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden @event)
        => new()
        {
            Beschrijving = $"Vertegenwoordiger '{@event.Voornaam} {@event.Achternaam}' is overleden volgens KSZ en werd verwijderd.",
            Gebeurtenis = nameof(Events.KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden),
            Data = new VertegenwoordigerWerdVerwijderdData(@event.VertegenwoordigerId,
                                                           @event.Voornaam,
                                                           @event.Achternaam),
            Initiator = "OVO002949",
            Tijdstip = "2024-07-30T11:08:05Z",
        };

    public static HistoriekGebeurtenisResponse? KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekend(KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekend @event)
        => new()
        {
            Beschrijving = $"Vertegenwoordiger '{@event.Voornaam} {@event.Achternaam}' werd niet teruggevonden uit KSZ en werd verwijderd.",
            Gebeurtenis = nameof(Events.KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekend),
            Data = new VertegenwoordigerWerdVerwijderdData(@event.VertegenwoordigerId,
                                                           @event.Voornaam,
                                                           @event.Achternaam),
            Initiator = "OVO002949",
            Tijdstip = "2024-07-30T11:08:05Z",
        };

    public static HistoriekGebeurtenisResponse? BankrekeningWerdToegevoegd(int nextId, VoegBankrekeningnummerToeRequest request)
        => new()
        {
            Beschrijving = $"Bankrekeningnummer met IBAN '{@request.Bankrekeningnummer.Iban}' werd toegevoegd.",
            Gebeurtenis = nameof(Events.BankrekeningnummerWerdToegevoegd),
            Data = new BankrekeningnummerWerdToegevoegd(nextId, request.Bankrekeningnummer.Iban, request.Bankrekeningnummer.GebruiktVoor, request.Bankrekeningnummer.Titularis),
            Initiator = "OVO002949",
            Tijdstip = "2024-07-30T11:08:05Z",
        };

    public static HistoriekGebeurtenisResponse BankrekeningnummerWerdToegevoegdVanuitKBO(BankrekeningnummerWerdToegevoegdVanuitKBO @event)
        => new()
        {
            Beschrijving = $"Bankrekeningnummer met IBAN '{@event.Iban}' werd toegevoegd vanuit KBO.",
            Gebeurtenis = nameof(Events.BankrekeningnummerWerdToegevoegdVanuitKBO),
            Data = new BankrekeningnummerWerdToegevoegdVanuitKBO(@event.BankrekeningnummerId, @event.Iban),
            Initiator = "OVO002949",
            Tijdstip = "2024-07-30T11:08:05Z",
        };

    public static HistoriekGebeurtenisResponse BankrekeningnummerWerdVerwijderdUitKBO(BankrekeningnummerWerdVerwijderdUitKBO @event)
        => new()
        {
            Beschrijving = $"Bankrekeningnummer met IBAN '{@event.Iban}' werd verwijderd uit KBO.",
            Gebeurtenis = nameof(Events.BankrekeningnummerWerdVerwijderdUitKBO),
            Data = new BankrekeningnummerWerdVerwijderdUitKBO(@event.BankrekeningnummerId, @event.Iban),
            Initiator = "OVO002949",
            Tijdstip = "2024-07-30T11:08:05Z",
        };
}
