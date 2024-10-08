namespace AssociationRegistry.Test.E2E.Framework.Mappers;

using Admin.Api.Verenigingen.Common;
using Admin.Api.Verenigingen.Historiek.ResponseModels;
using Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using Admin.ProjectionHost.Constants;
using Admin.Schema.Historiek.EventData;
using AlbaHost;
using Events;
using EventStore;
using Vereniging;

public static class HistoriekGebeurtenisMapper
{
    public static HistoriekGebeurtenisResponse FeitelijkeVerenigingWerdGeregistreerd(
        RegistreerFeitelijkeVerenigingRequest request,
        string vCode)
    {
        return new HistoriekGebeurtenisResponse
        {
            Beschrijving = $"Feitelijke vereniging werd geregistreerd met naam '{request.Naam}'.",
            Gebeurtenis = nameof(Events.FeitelijkeVerenigingWerdGeregistreerd),
            Data = new FeitelijkeVerenigingWerdGeregistreerdData(
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
                HoofdactiviteitenVerenigingsloket: null,
                Werkingsgebieden: MapWerkingsgebieden(request.Werkingsgebieden)),
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
            Data = new FeitelijkeVerenigingWerdGeregistreerdData(
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
                HoofdactiviteitenVerenigingsloket: null,
                Werkingsgebieden: feitelijkeVerenigingWerdGeregistreerd.Werkingsgebieden),
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

    public static HistoriekGebeurtenisResponse WerkingsgebiedenWerdenGewijzigd(
        string[] werkingsgebieden)
    {
        var @event = new WerkingsgebiedenWerdenGewijzigd(werkingsgebieden
                                                        .Select(Werkingsgebied.Create)
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

    private static Registratiedata.Werkingsgebied[] MapWerkingsgebieden(string[] werkingsgebiedenCodes)
    {
        return werkingsgebiedenCodes
              .Select(code => Werkingsgebied.Create(code))
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
            Initiator = EventStore.DigitaalVlaanderenOvoNumber,
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
}
