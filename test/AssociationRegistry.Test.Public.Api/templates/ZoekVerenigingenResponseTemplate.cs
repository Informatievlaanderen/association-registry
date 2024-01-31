namespace AssociationRegistry.Test.Public.Api.templates;

using Events;
using Formatters;
using JsonLdContext;
using Scriban;
using System.Dynamic;
using Test.Framework;
using Vereniging;

public class ZoekVerenigingenResponseTemplate
{
    private readonly List<dynamic> _facets = new();
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
        _verenigingen.Add(verenigingTemplateAction(new VerenigingTemplate(this)).ToObject());

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
            facets = _facets,
            query = _query,
        });
    }

    public static implicit operator string(ZoekVerenigingenResponseTemplate source)
        => source.Build();

    private void UpdateFacet(string code)
    {
        var maybeFacet = _facets.SingleOrDefault(f => f.code == code);

        if (maybeFacet is not null) _facets.Remove(maybeFacet);

        _facets.Add(new
        {
            code = code,
            count = (maybeFacet?.count ?? 0) + 1,
        });
    }

    public class VerenigingTemplate
    {
        private readonly dynamic _vereniging;
        private readonly ZoekVerenigingenResponseTemplate _zoekVerenigingenResponseTemplate;

        public VerenigingTemplate(ZoekVerenigingenResponseTemplate zoekVerenigingenResponseTemplate)
        {
            _zoekVerenigingenResponseTemplate = zoekVerenigingenResponseTemplate;
            _vereniging = new ExpandoObject();
            _vereniging.locaties = new List<object>();
            _vereniging.hoofdactiviteiten = new List<object>();
            _vereniging.relaties = new List<object>();
            _vereniging.sleutels = new List<object>();

            WithKorteNaam(string.Empty);
            WithDoelgroep();
        }

        public VerenigingTemplate WithVCode(string vCode)
        {
            _vereniging.vcode = vCode;
            _vereniging.jsonldid = JsonLdType.Vereniging.CreateWithIdValues(vCode);
            _vereniging.jsonldtype = JsonLdType.Vereniging.Type;
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

        public VerenigingTemplate WithKorteBeschrijving(string korteBeschrijving)
        {
            _vereniging.kortebeschrijving = korteBeschrijving;

            return this;
        }

        public VerenigingTemplate WithHoofdactiviteit(string code, string beschrijving)
        {
            _vereniging.hoofdactiviteiten.Add(new
            {
                jsonldid = JsonLdType.Hoofdactiviteit.CreateWithIdValues(code),
                jsonldtype = JsonLdType.Hoofdactiviteit.Type,
                code = code,
                beschrijving = beschrijving,
            });

            _zoekVerenigingenResponseTemplate.UpdateFacet(code);

            return this;
        }

        public VerenigingTemplate WithKboNummer(string kboNummer, string vCode)
        {
            _vereniging.sleutels.Add(new
            {
                jsonldid = JsonLdType.Sleutel.CreateWithIdValues(vCode, Sleutelbron.Kbo.Waarde),
                jsonldtype = JsonLdType.Sleutel.Type,

                identificator = new
                {
                    jsonldid = JsonLdType.GestructureerdeSleutel.CreateWithIdValues(vCode, Sleutelbron.Kbo.Waarde),
                    jsonldtype = JsonLdType.GestructureerdeSleutel.Type,
                    nummer = kboNummer,
                },

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
            string vCode,
            int locatieId,
            bool isPrimair = false)
        {
            _vereniging.locaties.Add(new
            {
                jsonldid = JsonLdType.Locatie.CreateWithIdValues(vCode, locatieId.ToString()),
                jsonldtype = JsonLdType.Locatie.Type,
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
                          .WithKorteBeschrijving(e.KorteBeschrijving)
                          .WithDoelgroep(e.Doelgroep.Minimumleeftijd, e.Doelgroep.Maximumleeftijd);

            foreach (var h in e.HoofdactiviteitenVerenigingsloket)
            {
                template.WithHoofdactiviteit(h.Code, h.Naam);
            }

            foreach (var l in e.Locaties)
            {
                template.WithLocatie(l.Locatietype, l.Naam, l.Adres.ToAdresString(), l.Adres?.Postcode, l.Adres?.Gemeente, e.VCode, l.LocatieId, l.IsPrimair);
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
                          .WithKorteBeschrijving(string.Empty)
                          .WithKboNummer(e.KboNummer, e.VCode);

            return template;
        }

        internal object ToObject()
            => _vereniging;
    }
}
