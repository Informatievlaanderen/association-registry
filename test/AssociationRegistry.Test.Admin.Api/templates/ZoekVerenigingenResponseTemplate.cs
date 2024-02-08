namespace AssociationRegistry.Test.Admin.Api.templates;

using AssociationRegistry.Admin.Schema.Constants;
using Events;
using Formatters;
using JsonLdContext;
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
        }

        public VerenigingTemplate WithVCode(string vCode)
        {
            _vereniging.vcode = vCode;
            _vereniging.jsonldid = JsonLdType.Vereniging.CreateWithIdValues(vCode);
            _vereniging.jsonldtype = JsonLdType.Vereniging.Type;

            _vereniging.sleutels.Add(new
            {
                jsonldid = JsonLdType.Sleutel.CreateWithIdValues(vCode, Sleutelbron.VR.Waarde),
                jsonldtype = JsonLdType.Sleutel.Type,
                bron = Sleutelbron.VR.Waarde,
                waarde = vCode,
                codeersysteem = CodeerSysteem.VR.Waarde,
                identificator = new
                {
                    jsonldid = JsonLdType.GestructureerdeSleutel.CreateWithIdValues(vCode, Sleutelbron.VR.Waarde),
                    jsonldtype = JsonLdType.GestructureerdeSleutel.Type,
                    nummer = vCode,
                },
            });

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
                jsonldid = JsonLdType.Hoofdactiviteit.CreateWithIdValues(code),
                jsonldtype = JsonLdType.Hoofdactiviteit.Type,
                code = code,
                naam = naam,
            });

            return this;
        }

        public VerenigingTemplate WithKboNummer(string kboNummer, string vCode)
        {
            _vereniging.sleutels.Add(new
            {
                jsonldid = JsonLdType.Sleutel.CreateWithIdValues(vCode, Sleutelbron.KBO.Waarde),
                jsonldtype = JsonLdType.Sleutel.Type,
                bron = Sleutelbron.KBO.Waarde,
                waarde = kboNummer,
                codeersysteem = CodeerSysteem.KBO.Waarde,
                identificator = new
                {
                    jsonldid = JsonLdType.GestructureerdeSleutel.CreateWithIdValues(vCode, Sleutelbron.KBO.Waarde),
                    jsonldtype = JsonLdType.GestructureerdeSleutel.Type,
                    nummer = kboNummer,
                },
            });

            return this;
        }

        public VerenigingTemplate WithDoelgroep(string vCode, int minimumleeftijd = 0, int maximumleeftijd = 150)
        {
            _vereniging.doelgroep = new
            {
                jsonldid = JsonLdType.Doelgroep.CreateWithIdValues(vCode),
                jsonldtype = JsonLdType.Doelgroep.Type,
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
            string vcode,
            int id,
            bool isPrimair = false)
        {
            _vereniging.locaties.Add(new
            {
                jsonldid = JsonLdType.Locatie.CreateWithIdValues(vcode, id.ToString()),
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
                          .WithDoelgroep(e.VCode, minimumleeftijd: e.Doelgroep.Minimumleeftijd,
                                         maximumleeftijd: e.Doelgroep.Maximumleeftijd);

            foreach (var h in e.HoofdactiviteitenVerenigingsloket)
            {
                template.WithHoofdactiviteit(h.Code, h.Naam);
            }

            foreach (var l in e.Locaties)
            {
                template.WithLocatie(l.Locatietype, l.Naam, l.Adres.ToAdresString(), l.Adres?.Postcode, l.Adres?.Gemeente, e.VCode,
                                     l.LocatieId, l.IsPrimair);
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
                          .WithKboNummer(e.KboNummer, e.VCode)
                          .WithDoelgroep(e.VCode);

            return template;
        }

        internal object ToObject()
            => _vereniging;
    }
}
