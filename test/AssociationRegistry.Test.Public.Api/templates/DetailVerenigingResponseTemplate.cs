namespace AssociationRegistry.Test.Public.Api.templates;

using AssociationRegistry.Public.ProjectionHost.Infrastructure.Extensions;
using AssociationRegistry.Public.Schema.Constants;
using Common.Extensions;
using Events;
using Formats;
using JsonLdContext;
using NodaTime;
using Scriban;
using System.Dynamic;
using Vereniging;

using VerenigingStatus = AssociationRegistry.Public.Schema.Constants.VerenigingStatus;
using WellknownFormats = AssociationRegistry.Public.Api.Constants.WellknownFormats;

public class DetailVerenigingResponseTemplate
{
    private readonly dynamic _vereniging;
    private object _datumLaatsteAanpassing;

    public DetailVerenigingResponseTemplate()
    {
        _vereniging = new ExpandoObject();
        _vereniging.locaties = new List<object>();
        _vereniging.contactgegevens = new List<object>();
        _vereniging.hoofdactiviteiten = new List<object>();
        _vereniging.relaties = new List<object>();
        _vereniging.sleutels = new List<object>();
        _vereniging.werkingsgebieden = new List<object>();
        _vereniging.lidmaatschappen = new List<object>();

        WithStatus(VerenigingStatus.Actief);
        WithKorteNaam(string.Empty);
        WithKorteBeschrijving(string.Empty);
        WithStartdatum(null);
    }

    public DetailVerenigingResponseTemplate WithVCode(string vCode)
    {
        _vereniging.vcode = vCode;
        _vereniging.jsonldtype = JsonLdType.FeitelijkeVereniging.Type;

        _vereniging.sleutels.Add(new
        {
            jsonldid = JsonLdType.Sleutel.CreateWithIdValues(vCode, Sleutelbron.VR.Waarde),
            jsonldtype = JsonLdType.Sleutel.Type,
            codeersysteem = CodeerSysteem.VR.Waarde,
            identificator = new
            {
                jsonldid = JsonLdType.GestructureerdeSleutel.CreateWithIdValues(vCode, Sleutelbron.VR.Waarde),
                jsonldtype = JsonLdType.GestructureerdeSleutel.Type,
                nummer = vCode,
            },

            bron = Sleutelbron.VR.Waarde,
            waarde = vCode,
        });

        return this;
    }

    public DetailVerenigingResponseTemplate WithType(Verenigingstype type)
    {
        _vereniging.verenigingstype = new
        {
            code = type.Code,
            naam = type.Naam,
        };

        return this;
    }

    public DetailVerenigingResponseTemplate WithNaam(string naam)
    {
        _vereniging.naam = naam;

        return this;
    }

    public DetailVerenigingResponseTemplate WithRoepnaam(string roepnaam)
    {
        _vereniging.roepnaam = roepnaam;

        return this;
    }

    public DetailVerenigingResponseTemplate WithKorteNaam(string korteNaam)
    {
        _vereniging.kortenaam = korteNaam;

        return this;
    }

    public DetailVerenigingResponseTemplate WithKorteBeschrijving(string korteBeschrijving)
    {
        _vereniging.kortebeschrijving = korteBeschrijving;

        return this;
    }

    public DetailVerenigingResponseTemplate WithStartdatum(DateOnly? startdatum)
    {
        _vereniging.startdatum = startdatum?.ToString(WellknownFormats.DateOnly);

        return this;
    }

    public DetailVerenigingResponseTemplate WithStatus(string status)
    {
        _vereniging.status = status;

        return this;
    }

    public DetailVerenigingResponseTemplate WithHoofdactiviteit(string code, string naam)
    {
        var hoofdactiviteit = new
        {
            jsonldid = JsonLdType.Hoofdactiviteit.CreateWithIdValues(code),
            jsonldtype = JsonLdType.Hoofdactiviteit.Type,
            code = code,
            naam = naam,
        };

        _vereniging.hoofdactiviteiten.Add(hoofdactiviteit);

        return this;
    }

    public DetailVerenigingResponseTemplate WithWerkingsgebied(string code, string naam)
    {
        var werkingsgebied = new
        {
            jsonldid = JsonLdType.Werkingsgebied.CreateWithIdValues(code),
            jsonldtype = JsonLdType.Werkingsgebied.Type,
            code = code,
            naam = naam,
        };

        _vereniging.werkingsgebieden.Add(werkingsgebied);

        return this;
    }

    public DetailVerenigingResponseTemplate WithKboNummer(string vCode, string kboNummer)
    {
        _vereniging.sleutels.Add(new
        {
            jsonldid = JsonLdType.Sleutel.CreateWithIdValues(vCode, Sleutelbron.KBO.Waarde),
            jsonldtype = JsonLdType.Sleutel.Type,
            codeersysteem = CodeerSysteem.KBO.Waarde,
            identificator = new
            {
                jsonldid = JsonLdType.GestructureerdeSleutel.CreateWithIdValues(vCode, Sleutelbron.KBO.Waarde),
                jsonldtype = JsonLdType.GestructureerdeSleutel.Type,
                nummer = kboNummer,
            },

            bron = Sleutelbron.KBO.Waarde,
            waarde = kboNummer,
        });

        return this;
    }

    public DetailVerenigingResponseTemplate WithDoelgroep(string vcode, int minimumleeftijd = 0, int maximumleeftijd = 150)
    {
        _vereniging.doelgroep = new
        {
            jsonldid = JsonLdType.Doelgroep.CreateWithIdValues(vcode),
            jsonldtype = JsonLdType.Doelgroep.Type,
            minimumleeftijd = minimumleeftijd,
            maximumleeftijd = maximumleeftijd,
        };

        return this;
    }

    public DetailVerenigingResponseTemplate WithLocatie(
        string vCode,
        string locatieId,
        string type,
        string naam,
        string adresVoorstelling,
        string straatnaam,
        string huisnummer,
        string? busnummer,
        string postcode,
        string gemeente,
        string land,
        bool isPrimair = false)
    {
        _vereniging.locaties.Add(new
        {
            jsonldid = JsonLdType.Locatie.CreateWithIdValues(vCode, locatieId),
            jsonldtype = JsonLdType.Locatie.Type,
            type = type,
            naam = naam,
            adresvoorstelling = adresVoorstelling,
            adres = new
            {
                jsonldid = JsonLdType.Adres.CreateWithIdValues(vCode, locatieId),
                jsonldtype = JsonLdType.Adres.Type,
                straatnaam = straatnaam,
                huisnummer = huisnummer,
                busnummer = busnummer,
                postcode = postcode,
                gemeente = gemeente,
                land = land,
            },
            isprimair = isPrimair,
        });

        return this;
    }

    public DetailVerenigingResponseTemplate WithLocatie(
        string vCode,
        string locatieId,
        string type,
        string naam,
        string broncode,
        string bronwaarde,
        bool isPrimair)
    {
        _vereniging.locaties.Add(new
        {
            jsonldid = JsonLdType.Locatie.CreateWithIdValues(vCode, locatieId),
            jsonldtype = JsonLdType.Locatie.Type,
            type = type,
            naam = naam,
            adresvoorstelling = string.Empty,
            adresid = new
            {
                broncode = broncode,
                bronwaarde = bronwaarde,
            },
            verwijstnaar = new
            {
                jsonldid = JsonLdType.AdresVerwijzing.CreateWithIdValues(bronwaarde.Split('/').Last()),
                jsonldtype = JsonLdType.AdresVerwijzing.Type,
            },
            isprimair = isPrimair,
        });

        return this;
    }

    public DetailVerenigingResponseTemplate WithLocatie(
        string vCode,
        string locatieId,
        string type,
        string naam,
        string adresVoorstelling,
        string straatnaam,
        string huisnummer,
        string busnummer,
        string postcode,
        string gemeente,
        string land,
        string broncode,
        string bronwaarde,
        bool isPrimair)
    {
        _vereniging.locaties.Add(new
        {
            jsonldid = JsonLdType.Locatie.CreateWithIdValues(vCode, locatieId),
            jsonldtype = JsonLdType.Locatie.Type,
            type = type,
            naam = naam,
            adresvoorstelling = adresVoorstelling,
            adres = new
            {
                jsonldid = JsonLdType.Adres.CreateWithIdValues(vCode, locatieId),
                jsonldtype = JsonLdType.Adres.Type,
                straatnaam = straatnaam,
                huisnummer = huisnummer,
                busnummer = busnummer,
                postcode = postcode,
                gemeente = gemeente,
                land = land,
            },
            adresid = new
            {
                broncode = broncode,
                bronwaarde = bronwaarde,
            },
            verwijstnaar = new
            {
                jsonldid = JsonLdType.AdresVerwijzing.CreateWithIdValues(bronwaarde.Split('/').Last()),
                jsonldtype = JsonLdType.AdresVerwijzing.Type,
            },
            isprimair = isPrimair,
        });

        return this;
    }

    public DetailVerenigingResponseTemplate FromEvent(FeitelijkeVerenigingWerdGeregistreerd e)
    {
        var template = WithVCode(e.VCode)
                      .WithJsonLdType(JsonLdType.FeitelijkeVereniging)
                      .WithType(Verenigingstype.FeitelijkeVereniging)
                      .WithNaam(e.Naam)
                      .WithKorteNaam(e.KorteNaam)
                      .WithKorteBeschrijving(e.KorteBeschrijving)
                      .WithStartdatum(e.Startdatum)
                      .WithDoelgroep(e.VCode, e.Doelgroep.Minimumleeftijd, e.Doelgroep.Maximumleeftijd);

        foreach (var h in e.HoofdactiviteitenVerenigingsloket)
        {
            template.WithHoofdactiviteit(h.Code, h.Naam);
        }

        foreach (var c in e.Contactgegevens)
        {
            template.WithContactgegeven(e.VCode, c.ContactgegevenId.ToString(), c.Contactgegeventype, c.Waarde, c.Beschrijving,
                                        c.IsPrimair);
        }

        foreach (var l in e.Locaties)
        {
            WithLocatie(e.VCode, l);
        }

        return template;
    }

    public DetailVerenigingResponseTemplate WithJsonLdType(JsonLdType jsonLdType)
    {
        _vereniging.jsonldtype = jsonLdType.Type;

        return this;
    }

    public DetailVerenigingResponseTemplate FromEvent(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd e)
    {
        var template = WithVCode(e.VCode)
                      .WithJsonLdType(JsonLdType.VerenigingMetRechtspersoonlijkheid)
                      .WithType(Verenigingstype.Parse(e.Rechtsvorm))
                      .WithNaam(e.Naam)
                      .WithRoepnaam(string.Empty)
                      .WithKorteNaam(e.KorteNaam)
                      .WithStartdatum(e.Startdatum)
                      .WithKboNummer(e.VCode, e.KboNummer)
                      .WithDoelgroep(e.VCode);

        return template;
    }

    public DetailVerenigingResponseTemplate WithDatumLaatsteAanpassing(Instant instant)
    {
        _datumLaatsteAanpassing = instant.FormatAsBelgianDate();

        return this;
    }

    public static implicit operator string(DetailVerenigingResponseTemplate source)
        => source.Build();

    public string Build()
    {
        var json = GetType().Assembly.GetAssemblyResource(name: "templates.DetailVerenigingResponse.json");

        var responseTemplate = Template.Parse(json);

        return responseTemplate.Render(new
        {
            vereniging = _vereniging,
            datumlaatsteaanpassing = _datumLaatsteAanpassing,
        });
    }

    private DetailVerenigingResponseTemplate WithLocatie(string vCode, Registratiedata.Locatie l)
    {
        if (l.Adres is not null && l.AdresId is null)
            return WithLocatie(vCode, l.LocatieId.ToString(), l.Locatietype, l.Naam, AdresFormatter.ToAdresString(l.Adres),
                               l.Adres.Straatnaam,
                               l.Adres.Huisnummer,
                               l.Adres.Busnummer, l.Adres.Postcode, l.Adres.Gemeente, l.Adres.Land, l.IsPrimair);

        if (l.Adres is null && l.AdresId is not null)
            return WithLocatie(vCode, l.LocatieId.ToString(), l.Locatietype, l.Naam, l.AdresId.Broncode, l.AdresId.Bronwaarde, l.IsPrimair);

        return WithLocatie(vCode, l.LocatieId.ToString(), l.Locatietype, l.Naam, AdresFormatter.ToAdresString(l.Adres), l.Adres.Straatnaam,
                           l.Adres.Huisnummer,
                           l.Adres.Busnummer, l.Adres.Postcode, l.Adres.Gemeente, l.Adres.Land, l.AdresId.Broncode,
                           l.AdresId.Bronwaarde, l.IsPrimair);
    }

    public DetailVerenigingResponseTemplate WithContactgegeven(
        string vCode,
        string contactgegevenId,
        string type,
        string waarde,
        string beschrijving = "",
        bool isPrimair = false)
    {
        _vereniging.contactgegevens.Add(new
        {
            jsonldid = JsonLdType.Contactgegeven.CreateWithIdValues(vCode, contactgegevenId),
            jsonldtype = JsonLdType.Contactgegeven.Type,
            contactgegeventype = type,
            waarde = waarde,
            beschrijving = beschrijving,
            isprimair = isPrimair,
        });

        return this;
    }
}
