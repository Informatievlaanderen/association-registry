namespace AssociationRegistry.Test.Public.Api.templates;

using AssociationRegistry.Public.Api.Constants;
using AssociationRegistry.Public.ProjectionHost.Infrastructure.Extensions;
using AssociationRegistry.Public.Schema.Constants;
using Events;
using Formatters;
using NodaTime;
using Scriban;
using System.Dynamic;
using Test.Framework;
using Vereniging;

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

        WithStatus(VerenigingStatus.Actief);
        WithKorteNaam(string.Empty);
        WithKorteBeschrijving(string.Empty);
        WithStartdatum(null);
        WithDoelgroep();
    }

    public DetailVerenigingResponseTemplate WithVCode(string vCode)
    {
        _vereniging.vcode = vCode;

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
        _vereniging.hoofdactiviteiten.Add(new
        {
            code = code,
            naam = naam,
        });

        return this;
    }

    public DetailVerenigingResponseTemplate WithKboNummer(string kboNummer)
    {
        _vereniging.sleutels.Add(new
        {
            bron = Sleutelbron.Kbo.Waarde,
            waarde = kboNummer,
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
            type = type,
            naam = naam,
            adresvoorstelling = adresVoorstelling,
            adres = new
            {
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

    public DetailVerenigingResponseTemplate WithLocatie(string type, string naam, string broncode, string bronwaarde, bool isPrimair)
    {
        _vereniging.locaties.Add(new
        {
            type = type,
            naam = naam,
            adresvoorstelling = string.Empty,
            adresid = new
            {
                broncode = broncode,
                bronwaarde = bronwaarde,
            },
            isprimair = isPrimair,
        });

        return this;
    }

    public DetailVerenigingResponseTemplate WithLocatie(
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
            type = type,
            naam = naam,
            adresvoorstelling = adresVoorstelling,
            adres = new
            {
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
        });

        return this;
    }

    public DetailVerenigingResponseTemplate IsAfdelingVan(string kboNummer, string vCode, string naam)
    {
        _vereniging.relaties.Add(new
        {
            relatietype = Relatietype.IsAfdelingVan.Beschrijving,
            anderevereniging = new
            {
                kbonummer = kboNummer,
                vcode = vCode,
                naam = naam,
            },
        });

        return this;
    }

    public DetailVerenigingResponseTemplate HeeftAfdeling(string vCode, string naam)
    {
        _vereniging.relaties.Add(new
        {
            relatietype = Relatietype.IsAfdelingVan.InverseBeschrijving,
            anderevereniging = new
            {
                kbonummer = string.Empty,
                vcode = vCode,
                naam = naam,
            },
        });

        return this;
    }

    public DetailVerenigingResponseTemplate FromEvent(FeitelijkeVerenigingWerdGeregistreerd e)
    {
        var template = WithVCode(e.VCode)
                      .WithType(Verenigingstype.FeitelijkeVereniging)
                      .WithNaam(e.Naam)
                      .WithKorteNaam(e.KorteNaam)
                      .WithKorteBeschrijving(e.KorteBeschrijving)
                      .WithStartdatum(e.Startdatum)
                      .WithDoelgroep(e.Doelgroep.Minimumleeftijd, e.Doelgroep.Maximumleeftijd);

        foreach (var h in e.HoofdactiviteitenVerenigingsloket)
        {
            template.WithHoofdactiviteit(h.Code, h.Naam);
        }

        foreach (var c in e.Contactgegevens)
        {
            template.WithContactgegeven(c.Contactgegeventype, c.Waarde, c.Beschrijving, c.IsPrimair);
        }

        foreach (var l in e.Locaties)
        {
            WithLocatie(l);
        }

        return template;
    }

    public DetailVerenigingResponseTemplate FromEvent(AfdelingWerdGeregistreerd e)
    {
        var template = WithVCode(e.VCode)
                      .WithType(Verenigingstype.Afdeling)
                      .WithNaam(e.Naam)
                      .WithKorteNaam(e.KorteNaam)
                      .WithKorteBeschrijving(e.KorteBeschrijving)
                      .WithStartdatum(e.Startdatum)
                      .WithDoelgroep(e.Doelgroep.Minimumleeftijd, e.Doelgroep.Maximumleeftijd)
                      .IsAfdelingVan(e.Moedervereniging.KboNummer, e.Moedervereniging.VCode, e.Moedervereniging.Naam);

        foreach (var h in e.HoofdactiviteitenVerenigingsloket)
        {
            template.WithHoofdactiviteit(h.Code, h.Naam);
        }

        foreach (var c in e.Contactgegevens)
        {
            template.WithContactgegeven(c.Contactgegeventype, c.Waarde, c.Beschrijving, c.IsPrimair);
        }

        foreach (var l in e.Locaties)
        {
            WithLocatie(l);
        }

        return template;
    }

    public DetailVerenigingResponseTemplate FromEvent(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd e)
    {
        var template = WithVCode(e.VCode)
                      .WithType(Verenigingstype.Parse(e.Rechtsvorm))
                      .WithNaam(e.Naam)
                      .WithRoepnaam(string.Empty)
                      .WithKorteNaam(e.KorteNaam)
                      .WithStartdatum(e.Startdatum)
                      .WithKboNummer(e.KboNummer);

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

    private DetailVerenigingResponseTemplate WithLocatie(Registratiedata.Locatie l)
    {
        if (l.Adres is not null && l.AdresId is null)
            return WithLocatie(l.Locatietype, l.Naam, l.Adres.ToAdresString(), l.Adres.Straatnaam, l.Adres.Huisnummer,
                               l.Adres.Busnummer, l.Adres.Postcode, l.Adres.Gemeente, l.Adres.Land, l.IsPrimair);

        if (l.Adres is null && l.AdresId is not null)
            return WithLocatie(l.Locatietype, l.Naam, l.AdresId.Broncode, l.AdresId.Bronwaarde, l.IsPrimair);

        return WithLocatie(l.Locatietype, l.Naam, l.Adres.ToAdresString(), l.Adres.Straatnaam, l.Adres.Huisnummer,
                           l.Adres.Busnummer, l.Adres.Postcode, l.Adres.Gemeente, l.Adres.Land, l.AdresId.Broncode,
                           l.AdresId.Bronwaarde, l.IsPrimair);
    }

    public DetailVerenigingResponseTemplate WithContactgegeven(string type, string waarde, string beschrijving = "", bool isPrimair = false)
    {
        _vereniging.contactgegevens.Add(new
        {
            contactgegeventype = type,
            waarde = waarde,
            beschrijving = beschrijving,
            isprimair = isPrimair,
        });

        return this;
    }
}
