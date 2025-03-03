namespace AssociationRegistry.Test.Admin.Api.Framework.templates;

using Events;
using Formats;
using JsonLdContext;
using System.Dynamic;
using Vereniging;
using Vereniging.Verenigingstype;
using VerenigingStatus = AssociationRegistry.Admin.Schema.Constants.VerenigingStatus;

public class ZoekVerenigingenResponseTemplate : ResponseTemplate
{
    private readonly List<object> _verenigingen = new();
    private object _pagination = new { };
    private string _query = string.Empty;

    public ZoekVerenigingenResponseTemplate()
        : base("Framework.templates.ZoekVerenigingenResponse.json")
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

    protected override dynamic BuildModel()
        => new
        {
            verenigingen = _verenigingen,
            pagination = _pagination,
            query = _query,
        };

    public class VerenigingTemplate
    {
        private readonly dynamic _vereniging;

        public VerenigingTemplate()
        {
            _vereniging = new ExpandoObject();
            _vereniging.corresponderendevcodes = Array.Empty<string>();
            _vereniging.locaties = new List<object>();
            _vereniging.hoofdactiviteiten = new List<object>();
            _vereniging.werkingsgebieden = new List<object>();
            _vereniging.sleutels = new List<object>();
            _vereniging.lidmaatschappen = new List<object>();

            WithStatus(VerenigingStatus.Actief);
            WithKorteNaam(string.Empty);
        }

        public VerenigingTemplate WithVCode(string vCode)
        {
            _vereniging.vcode = vCode;

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

        public VerenigingTemplate WithWerkingsgebied(string code, string naam)
        {
            _vereniging.werkingsgebieden.Add(new
            {
                jsonldid = JsonLdType.Werkingsgebied.CreateWithIdValues(code),
                jsonldtype = JsonLdType.Werkingsgebied.Type,
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

        public VerenigingTemplate WithStartdatum(DateOnly? startdatum)
        {
            _vereniging.startdatum = startdatum?.ToString(WellknownFormats.DateOnly);

            return this;
        }

        public VerenigingTemplate WithEinddatum(DateOnly? einddatum)
        {
            _vereniging.einddatum = einddatum?.ToString(WellknownFormats.DateOnly);

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

        public VerenigingTemplate WithJsonLdType(JsonLdType jsonLdType)
        {
            _vereniging.jsonldtype = jsonLdType.Type;

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
                          .WithJsonLdType(JsonLdType.FeitelijkeVereniging)
                          .WithType(Verenigingstype.FeitelijkeVereniging)
                          .WithNaam(e.Naam)
                          .WithKorteNaam(e.KorteNaam)
                          .WithStartdatum(e.Startdatum)
                          .WithDoelgroep(e.VCode, e.Doelgroep.Minimumleeftijd,
                                         e.Doelgroep.Maximumleeftijd);

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
                          .WithJsonLdType(JsonLdType.VerenigingMetRechtspersoonlijkheid)
                          .WithType(Verenigingstype.Parse(e.Rechtsvorm))
                          .WithNaam(e.Naam)
                          .WithRoepnaam(string.Empty)
                          .WithKorteNaam(e.KorteNaam)
                          .WithKboNummer(e.KboNummer, e.VCode)
                          .WithStartdatum(e.Startdatum)
                          .WithEinddatum(null)
                          .WithDoelgroep(e.VCode);

            return template;
        }

        internal object ToObject()
            => _vereniging;
    }
}
