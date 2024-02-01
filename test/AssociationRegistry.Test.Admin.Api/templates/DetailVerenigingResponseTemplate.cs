﻿namespace AssociationRegistry.Test.Admin.Api.templates;

using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.ProjectionHost.Infrastructure.Extensions;
using AssociationRegistry.Admin.Schema.Constants;
using Events;
using Formatters;
using JsonLdContext;
using NodaTime;
using Scriban;
using System.Dynamic;
using Test.Framework;
using Vereniging;
using Vereniging.Bronnen;

public class DetailVerenigingResponseTemplate
{
    private object _datumLaatsteAanpassing;
    private readonly dynamic _vereniging;

    public DetailVerenigingResponseTemplate()
    {
        _vereniging = new ExpandoObject();
        _vereniging.locaties = new List<object>();
        _vereniging.contactgegevens = new List<dynamic>();
        _vereniging.hoofdactiviteiten = new List<object>();
        _vereniging.relaties = new List<object>();
        _vereniging.sleutels = new List<object>();
        _vereniging.vertegenwoordigers = new List<object>();

        WithStatus(VerenigingStatus.Actief);
        WithKorteNaam(string.Empty);
        WithKorteBeschrijving(string.Empty);
        WithStartdatum(null);
        WithEinddatum(null);
        WithDoelgroep();
        WithIsUitgeschrevenUitPubliekeDatastroom(false);
    }

    public DetailVerenigingResponseTemplate WithVCode(string vCode)
    {
        _vereniging.vcode = vCode;
        _vereniging.jsonldid = JsonLdType.Vereniging.CreateWithIdValues(vCode);
        _vereniging.jsonldtype = JsonLdType.Vereniging.Type;

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

    public DetailVerenigingResponseTemplate WithEinddatum(DateOnly? einddatum)
    {
        _vereniging.einddatum = einddatum?.ToString(WellknownFormats.DateOnly);

        return this;
    }

    public DetailVerenigingResponseTemplate WithStatus(string status)
    {
        _vereniging.status = status;

        return this;
    }

    public DetailVerenigingResponseTemplate WithIsUitgeschrevenUitPubliekeDatastroom(bool isUitgeschreven)
    {
        _vereniging.isuitgeschreven = isUitgeschreven;

        return this;
    }

    public DetailVerenigingResponseTemplate WithBron(Bron bron)
    {
        _vereniging.bron = bron.Waarde;

        return this;
    }

    public DetailVerenigingResponseTemplate WithHoofdactiviteit(string code, string beschrijving, string vCode)
    {
        _vereniging.hoofdactiviteiten.Add(new
        {
            jsonldid = JsonLdType.Hoofdactiviteit.CreateWithIdValues(vCode, code),
            jsonldtype = JsonLdType.Hoofdactiviteit.Type,
            code = code,
            naam = beschrijving,
        });

        return this;
    }

    public DetailVerenigingResponseTemplate WithKboNummer(string kboNummer, string vCode)
    {
        _vereniging.sleutels.Add(new
        {
            jsonldid = JsonLdType.Sleutel.CreateWithIdValues(vCode, Sleutelbron.Kbo.Waarde),
            jsonldtype = JsonLdType.Sleutel.Type,
            bron = Sleutelbron.Kbo.Waarde,
            waarde = kboNummer,
            identificator = new
            {
                jsonldid = JsonLdType.GestructureerdeSleutel.CreateWithIdValues(vCode, Sleutelbron.Kbo.Waarde),
                jsonldtype = JsonLdType.GestructureerdeSleutel.Type,
                nummer = kboNummer,
            },
        });

        return this;
    }

    public DetailVerenigingResponseTemplate WithDoelgroep(int minimumleeftijd = 0, int maximumleeftijd = 150)
    {
        _vereniging.doelgroep = new
        {
            minimumleeftijd = minimumleeftijd,
            maximumleeftijd = maximumleeftijd,
        };

        return this;
    }

    public DetailVerenigingResponseTemplate WithLocatie(
        int locatieId,
        string type,
        string naam,
        string adresVoorstelling,
        string straatnaam,
        string huisnummer,
        string? busnummer,
        string postcode,
        string gemeente,
        string land,
        bool isPrimair,
        Bron bron,
        string vCode)
    {
        _vereniging.locaties.Add(new
        {
            jsonldid = JsonLdType.Locatie.CreateWithIdValues(vCode, locatieId.ToString()),
            jsonldtype = JsonLdType.Locatie.Type,
            id = locatieId,
            type = new
            {
                jsonldid = JsonLdType.LocatieType.CreateWithIdValues(type),
                jsonldtype = JsonLdType.LocatieType.Type,
                naam = type,
            },
            naam = naam,
            adresvoorstelling = adresVoorstelling,
            adres = new
            {
                jsonldid = JsonLdType.Adres.CreateWithIdValues(vCode, locatieId.ToString()),
                jsonldtype = JsonLdType.Adres.Type,
                straatnaam = straatnaam,
                huisnummer = huisnummer,
                busnummer = busnummer,
                postcode = postcode,
                gemeente = gemeente,
                land = land,
            },
            isprimair = isPrimair,
            bron = bron.Waarde,
        });

        return this;
    }

    public DetailVerenigingResponseTemplate WithLocatie(
        int locatieId,
        string type,
        string naam,
        string broncode,
        string bronwaarde,
        bool isPrimair,
        Bron bron,
        string vCode)
    {
        _vereniging.locaties.Add(new
        {
            jsonldid = JsonLdType.Locatie.CreateWithIdValues(vCode, locatieId.ToString()),
            jsonldtype = JsonLdType.Locatie.Type, id = locatieId,
            type = new
            {
                jsonldid = JsonLdType.LocatieType.CreateWithIdValues(type),
                jsonldtype = JsonLdType.LocatieType.Type,
                naam = type,
            },
            naam = naam,
            adresvoorstelling = string.Empty,
            adresid = new
            {
                broncode = broncode,
                bronwaarde = bronwaarde,
            },
            isprimair = isPrimair,
            bron = bron.Waarde,
        });

        return this;
    }

    public DetailVerenigingResponseTemplate WithLocatie(
        int locatieId,
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
        bool isPrimair,
        Bron bron,
        string vCode)
    {
        _vereniging.locaties.Add(new
        {
            jsonldid = JsonLdType.Locatie.CreateWithIdValues(vCode, locatieId.ToString()),
            jsonldtype = JsonLdType.Locatie.Type, id = locatieId,
            type = new
            {
                jsonldid = JsonLdType.LocatieType.CreateWithIdValues(type),
                jsonldtype = JsonLdType.LocatieType.Type,
                naam = type,
            },
            naam = naam,
            adresvoorstelling = adresVoorstelling,
            adres = new
            {
                jsonldid = JsonLdType.Adres.CreateWithIdValues(vCode, locatieId.ToString()),
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
            isprimair = isPrimair,
            bron = bron.Waarde,
        });

        return this;
    }

    public DetailVerenigingResponseTemplate FromEvent(FeitelijkeVerenigingWerdGeregistreerd e)
    {
        WithVCode(e.VCode)
           .WithType(Verenigingstype.FeitelijkeVereniging)
           .WithNaam(e.Naam)
           .WithKorteNaam(e.KorteNaam)
           .WithKorteBeschrijving(e.KorteBeschrijving)
           .WithStartdatum(e.Startdatum)
           .WithDoelgroep(e.Doelgroep.Minimumleeftijd, e.Doelgroep.Maximumleeftijd)
           .WithBron(e.Bron);

        foreach (var h in e.HoofdactiviteitenVerenigingsloket)
        {
            WithHoofdactiviteit(h.Code, h.Naam, e.VCode);
        }

        foreach (var c in e.Contactgegevens)
        {
            WithContactgegeven(c.ContactgegevenId, Bron.Initiator, c.Contactgegeventype, c.Waarde, e.VCode, c.Beschrijving, c.IsPrimair);
        }

        foreach (var l in e.Locaties)
        {
            WithLocatie(l, e.Bron, e.VCode);
        }

        foreach (var v in e.Vertegenwoordigers)
        {
            WithVertegenwoordiger(v.VertegenwoordigerId, v.Voornaam, v.Achternaam, v.Roepnaam, v.Rol, v.Insz, v.Email, v.Telefoon, v.Mobiel,
                                  v.SocialMedia, v.IsPrimair, e.Bron, e.VCode);
        }

        return this;
    }

    public DetailVerenigingResponseTemplate WithVertegenwoordiger(
        int id,
        string voornaam,
        string achternaam,
        string roepnaam,
        string rol,
        string insz,
        string email,
        string telefoon,
        string mobiel,
        string socialMedia,
        bool isPrimair,
        Bron bron,
        string vCode)
    {
        _vereniging.vertegenwoordigers.Add(new
        {
            jsonldid = JsonLdType.Vertegenwoordiger.CreateWithIdValues(vCode, id.ToString()),
            jsonldtype = JsonLdType.Vertegenwoordiger.Type,
            id = id,
            voornaam = voornaam,
            achternaam = achternaam,
            roepnaam = roepnaam,
            rol = rol,
            insz = insz,
            email = email,
            telefoon = telefoon,
            mobiel = mobiel,
            socialmedia = socialMedia,
            isprimair = isPrimair,
            vertegenwoordigercontactgegevens = new
            {
                jsonldid = JsonLdType.VertegenwoordigerContactgegeven.CreateWithIdValues(vCode, id.ToString()),
                jsonldtype = JsonLdType.VertegenwoordigerContactgegeven.Type,
                email = email,
                telefoon = telefoon,
                mobiel = mobiel,
                socialmedia = socialMedia,
                isprimair = isPrimair,
            },
            bron = bron.Waarde,
        });

        return this;
    }

    public DetailVerenigingResponseTemplate FromEvent(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd e)
    {
        var template = WithVCode(e.VCode)
                      .WithType(Verenigingstype.Parse(e.Rechtsvorm))
                      .WithNaam(e.Naam)
                      .WithRoepnaam(string.Empty)
                      .WithKorteNaam(e.KorteNaam)
                      .WithStartdatum(e.Startdatum)
                      .WithKboNummer(e.KboNummer, e.VCode)
                      .WithBron(e.Bron);

        return template;
    }

    public DetailVerenigingResponseTemplate WithDatumLaatsteAanpassing(Instant instant)
    {
        _datumLaatsteAanpassing = instant.ToBelgianDate();

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

    private DetailVerenigingResponseTemplate WithLocatie(Registratiedata.Locatie l, Bron bron, string vCode)
    {
        if (l.Adres is not null && l.AdresId is null)
            return WithLocatie(l.LocatieId, l.Locatietype, l.Naam, l.Adres.ToAdresString(), l.Adres.Straatnaam, l.Adres.Huisnummer,
                               l.Adres.Busnummer, l.Adres.Postcode, l.Adres.Gemeente, l.Adres.Land, l.IsPrimair, bron, vCode);

        if (l.Adres is null && l.AdresId is not null)
            return WithLocatie(l.LocatieId, l.Locatietype, l.Naam, l.AdresId.Broncode, l.AdresId.Bronwaarde, l.IsPrimair, bron, vCode);

        return WithLocatie(l.LocatieId, l.Locatietype, l.Naam, l.Adres.ToAdresString(), l.Adres.Straatnaam, l.Adres.Huisnummer,
                           l.Adres.Busnummer, l.Adres.Postcode, l.Adres.Gemeente, l.Adres.Land, l.AdresId.Broncode,
                           l.AdresId.Bronwaarde, l.IsPrimair, bron, vCode);
    }

    public DetailVerenigingResponseTemplate WithContactgegeven(
        int id,
        Bron bron,
        string contactgegeventype,
        string waarde,
        string vCode,
        string beschrijving = "",
        bool isPrimair = false)
    {
        _vereniging.contactgegevens.Add(new
        {
            jsonldid = JsonLdType.Contactgegeven.CreateWithIdValues(vCode, id.ToString()),
            jsonldtype = JsonLdType.Contactgegeven.Type,
            id = id,
            contactgegeventype = contactgegeventype,
            waarde = waarde,
            beschrijving = beschrijving,
            isprimair = isPrimair,
            bron = bron.Waarde,
        });

        _vereniging.contactgegevens = ((List<dynamic>)_vereniging.contactgegevens).OrderBy(c => c.id).ToList();

        return this;
    }
}
