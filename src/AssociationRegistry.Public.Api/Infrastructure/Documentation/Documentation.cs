namespace AssociationRegistry.Public.Api.Infrastructure.Documentation;

using System;
using System.Text;
using ConfigurationBindings;

public static class Documentation
{
    private static string IntroductieText
        => @"
---
Voor meer algemene informatie over het gebruik van deze API, raadpleeg onze <a href=""https://vlaamseoverheid.atlassian.net/wiki/spaces/AGB/pages/6285361348/API+documentatie"">publieke confluence pagina</a>.

# Introductie
Het Verenigingsregister stelt je in staat om informatie te verkrijgen over verenigingen die in interactie treden met een overheid (in het kader van dienstverlening):
* verenigingen zonder winstoogmerk
* verenigingen zonder rechtspersoon, met name:
    * feitelijke verenigingen
    * afdelingen van koepelorganisaties (sommige vzw’s vertakken hun werking in afdelingen omwille van interne operationele werking)

Deze API geeft *enkel leesrechten* tot de informatie uit het Verenigingsregister.
";

    private static Func<AppSettings, string> ToegangTotHetRegisterText
        => appSettings => @$"
# Toegang tot het register
## Basis-URL

De REST API van Basisregisters Vlaanderen is te bereiken via volgende basis-URL.

Doelpubliek | REST basis-URL                                                    |
----------- | ----------------------------------------------------------------- |
Iedereen    | {appSettings.BaseUrl} |

## Gebruik API Keys

Om gebruik te kunnen maken van deze API, is het noodzakelijk een API Key aan te vragen via [dit formulier]({appSettings.ApiKeyRequestFormUrl})

Na het verkrijgen van de API Key, dient deze meegestuurd te worden als header, of via de query parameters.

Type | Naam | Voorbeeld                                                    |
----------- | ----------------------------------------------------------------- |----|
Header    | `VR-api-key` | `curl --request GET --url '{appSettings.BaseUrl}/v1/hoofdactiviteitenVerenigingsloket' --header 'VR-api-key: api-key'`|
Query parameter | `vr-api-key` | {appSettings.BaseUrl}/v1/hoofdactiviteitenVerenigingsloket?vr-api-key=api-key |

";

    private static string FoutmeldingenText
        => @"# Foutmeldingen

De Basisregisters Vlaanderen API gebruikt [Problem Details for HTTP APIs (RFC7807)](https://tools.ietf.org/html/rfc7807) om foutmeldingen te ontsluiten. Een foutmelding zal resulteren in volgende datastructuur:

```
{
  ""type"": ""string"",
  ""title"": ""string"",
  ""detail"": ""string"",
  ""status"": number,
  ""instance"": ""string""
}
```

## Mogelijke foutmeldingen

Binnen de aangeboden endpoints zijn er een aantal foutmeldingen die kunnen voorkomen. Het veld ‘Detail’ binnen de response body bevat meer informatie over de foutmelding.

Foutmelding | Wanneer                                                           |
----------- | ----------------------------------------------------------------- |
403    |Wanneer er geen API key wordt meegegeven. <br> Wanneer de opgegeven API key niet correct is. |
404    |Wanneer de resource niet gevonden kan worden. |
500    |Wanneer er een interne fout is gebeurd. |
";

    public static string GetApiLeadingText(AppSettings appSettings)
    {
        var text = new StringBuilder(capacity: 1000);

        text.Append(IntroductieText);
        text.AppendLine(ToegangTotHetRegisterText(appSettings));
        text.AppendLine(FoutmeldingenText);
        return text.ToString();
    }

    public static string GetHeadContent()
    {
        var borderAfterFoutmeldingenSection = @"
<style>
li[data-item-id=""section/Foutmeldingen""]
{
    border-bottom: 1px solid rgb(225, 225, 225);
}
</style>";

        var lessSpaceBetweenSections = @"
<script src=""https://cdn.redoc.ly/redoc/latest/bundles/redoc.standalone.js""> </script>
<script>
window.addEventListener('load', () => {
Redoc.init(
        '/docs/v1/docs.json',
    {
        'theme': {
            'spacing': {
                'sectionVertical': '0'
            },
        }
    },
    document.getElementById('redoc-container')
)
});
</script>";

        return $@"
{borderAfterFoutmeldingenSection}
{lessSpaceBetweenSections}
";
    }
}
