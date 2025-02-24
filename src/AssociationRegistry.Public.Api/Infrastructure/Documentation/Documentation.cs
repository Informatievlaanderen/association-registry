namespace AssociationRegistry.Public.Api.Infrastructure.Documentation;

using ConfigurationBindings;
using System;
using System.Text;

public static class Documentation
{
    private static Func<AppSettings, string> IntroductieText
        => appSettings => @$"
---
Het Verenigingsregister verstrekt informatie over verenigingen die in interactie treden met een overheid (in het kader van dienstverlening).
<br />Het betreft verenigingen zonder rechtspersoonlijkheid (zoals feitelijke verenigingen) en verenigingen met rechtspersoonlijkheid (zoals vzw’s).

Deze API geeft *enkel leesrechten* tot de informatie uit het Verenigingsregister.

Schrijf je in op <a href=""{appSettings.NewsletterUrl}"">onze nieuwsbrief</a> om op de hoogte te blijven met informatie en nieuwigheden over het verenigingsregister.";

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
Header    | `vr-api-key` | `curl --request GET --url '{appSettings.BaseUrl}/v1/hoofdactiviteitenVerenigingsloket' --header 'vr-api-key: api-key'`|
Query parameter | `vr-api-key` | {appSettings.BaseUrl}/v1/hoofdactiviteitenVerenigingsloket?vr-api-key=api-key |

## Gebruik API Versies

Om gebruik te kunnen maken van een andere API versie, is het noodzakelijk een API versie mee te geven.

Deze dienen meegestuurd te worden als header, of via de query parameters.

Mogelijke waarden zijn:

* {WellknownVersions.V2} - in deze versie wordt het verenigingstype omgezet van FV-feitelijke vereniging naar VZER-vereniging zonder eigen rechtspersoonlijkheid

Type | Naam | Voorbeeld                                                    |
----------- | ----------------------------------------------------------------- |----|
Header    | `vr-api-version` | `curl --request GET --url '{appSettings.BaseUrl}/v1/hoofdactiviteitenVerenigingsloket' --header 'vr-api-version: {WellknownVersions.V2}'`|
Query parameter | `vr-api-version` | {appSettings.BaseUrl}/v1/hoofdactiviteitenVerenigingsloket?vr-api-version={WellknownVersions.V2} |

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

        text.Append(IntroductieText(appSettings));
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
