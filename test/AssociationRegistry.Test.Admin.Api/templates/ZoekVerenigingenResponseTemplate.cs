namespace AssociationRegistry.Test.Admin.Api.templates;

using AssociationRegistry.Admin.Schema.Constants;
using Events;
using Formatters;
using Scriban;
using System.Dynamic;
using Test.Framework;
using Vereniging;

public class ZoekVerenigingenResponseTemplate
{
    private readonly List<object> _verenigingen = new();
    private object _pagination = new { };
    private string _query = string.Empty;

    public ZoekVerenigingenResponseTemplate()
    {
        WithPagination();
    }

    public ZoekVerenigingenResponseTemplate FromQuery(string query)
    {
        _query = query;

        return this;
    }

    public ZoekVerenigingenResponseTemplate WithPagination(int offset = 0, int limit = 50)
    {
        _pagination = new
        {
            offset = offset,
            limit = limit,
        };

        return this;
    }

    public ZoekVerenigingenResponseTemplate WithVereniging(Func<VerenigingTemplate, VerenigingTemplate> verenigingTemplateAction)
    {
        _verenigingen.Add(verenigingTemplateAction(new VerenigingTemplate()).ToObject());

        return this;
    }

    public string Build()
    {
        var json = GetType().Assembly.GetAssemblyResource(name: "templates.ZoekVerenigingenResponse.json");

        var responseTemplate = Template.Parse(json);

        return responseTemplate.Render(new
        {
            verenigingen = _verenigingen,
            pagination = _pagination,
            query = _query,
        });
    }

    public static implicit operator string(ZoekVerenigingenResponseTemplate source)
        => source.Build();

    public class VerenigingTemplate
    {
        private readonly dynamic _vereniging;

        public VerenigingTemplate()
        {
            _vereniging = new ExpandoObject();
            _vereniging.locaties = new List<object>();
            _vereniging.hoofdactiviteiten = new List<object>();
            _vereniging.sleutels = new List<object>();

            WithStatus(VerenigingStatus.Actief);
            WithKorteNaam(string.Empty);
            WithDoelgroep();
        }

        public VerenigingTemplate WithVCode(string vCode)
        {
            _vereniging.vcode = vCode;

            return this;
        }

        public VerenigingTemplate WithType(Verenigingstype type)
        {
            _vereniging.verenigingstype = new
            {
                code = type.Code,
                naam = type.Naam,
            };

            return this;
        }

        public VerenigingTemplate WithNaam(string naam)
        {
            _vereniging.naam = naam;

            return this;
        }

        public VerenigingTemplate WithRoepnaam(string roepnaam)
        {
            _vereniging.roepnaam = roepnaam;

            return this;
        }

        public VerenigingTemplate WithKorteNaam(string korteNaam)
        {
            _vereniging.kortenaam = korteNaam;

            return this;
        }

        public VerenigingTemplate WithStatus(string status)
        {
            _vereniging.status = status;

            return this;
        }

        public VerenigingTemplate WithHoofdactiviteit(string code, string naam)
        {
            _vereniging.hoofdactiviteiten.Add(new
            {
                code = code,
                naam = naam,
            });

            return this;
        }

        public VerenigingTemplate WithKboNummer(string kboNummer)
        {
            _vereniging.sleutels.Add(new
            {
                bron = Sleutelbron.Kbo.Waarde,
                waarde = kboNummer,
            });

            return this;
        }

        public VerenigingTemplate WithDoelgroep(int minimumleeftijd = 0, int maximumleeftijd = 150)
        {
            _vereniging.doelgroep = new
            {
                minimumleeftijd = minimumleeftijd,
                maximumleeftijd = maximumleeftijd,
            };

            return this;
        }

        public VerenigingTemplate WithLocatie(
            string type,
            string naam,
            string? adresVoorstelling,
            string? postcode,
            string? gemeente,
            bool isPrimair = false)
        {
            _vereniging.locaties.Add(new
            {
                type = type,
                naam = naam,
                adresvoorstelling = adresVoorstelling ?? string.Empty,
                postcode = postcode ?? string.Empty,
                gemeente = gemeente ?? string.Empty,
                isprimair = isPrimair,
            });

            return this;
        }

        public VerenigingTemplate FromEvent(FeitelijkeVerenigingWerdGeregistreerd e)
        {
            var template = WithVCode(e.VCode)
                          .WithType(Verenigingstype.FeitelijkeVereniging)
                          .WithNaam(e.Naam)
                          .WithKorteNaam(e.KorteNaam)
                          .WithDoelgroep(e.Doelgroep.Minimumleeftijd, e.Doelgroep.Maximumleeftijd);

            foreach (var h in e.HoofdactiviteitenVerenigingsloket)
            {
                template.WithHoofdactiviteit(h.Code, h.Naam);
            }

            foreach (var l in e.Locaties)
            {
                template.WithLocatie(l.Locatietype, l.Naam, l.Adres.ToAdresString(), l.Adres?.Postcode, l.Adres?.Gemeente, l.IsPrimair);
            }

            return template;
        }

        public VerenigingTemplate FromEvent(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd e)
        {
            var template = WithVCode(e.VCode)
                          .WithType(Verenigingstype.Parse(e.Rechtsvorm))
                          .WithNaam(e.Naam)
                          .WithRoepnaam(string.Empty)
                          .WithKorteNaam(e.KorteNaam)
                          .WithKboNummer(e.KboNummer);

            return template;
        }

        internal object ToObject()
            => _vereniging;
    }
}
