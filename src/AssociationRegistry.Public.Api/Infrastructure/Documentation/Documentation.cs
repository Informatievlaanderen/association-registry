namespace AssociationRegistry.Public.Api.Infrastructure.Documentation;

using System.Text;
using ConfigurationBindings;

public static class Documentation
{
    public static string GetApiLeadingText(AppSettings appSettings)
    {
        var text = new StringBuilder(capacity: 1000);

        text.Append(
            $@"
---
Momenteel leest u de documentatie voor versie v1 van de Basisregisters Vlaanderen Verenigingsregister Publieke API.

Voor meer algemene informatie over het gebruik van deze API, raadpleeg onze <a href=""https://vlaamseoverheid.atlassian.net/wiki/spaces/AGB/pages/6285361348/API+documentatie"">publieke confluence pagina</a>.

# Introductie
Het Verenigingsregister stelt u in staat om informatie te verkrijgen over:
* verenigingen die in interactie treden met een overheid (in het kader van digitale dienstverlening)
* verenigingen zonder winstoogmerk
* verenigingen zonder rechtspersoon, met name:
* feitelijke verenigingen
* afdelingen van koepelorganisaties (sommige vzw’s vertakken hun werking in afdelingen omwille van interne operationele werking)

Deze API geeft *enkel leesrechten* tot de informatie uit het Verenigingsregister.

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


## Foutmeldingen

De Basisregisters Vlaanderen API gebruikt [Problem Details for HTTP APIs (RFC7807)](https://tools.ietf.org/html/rfc7807) om foutmeldingen te ontsluiten. Een foutmelding zal resulteren in volgende datastructuur:

```
{{
  ""type"": ""string"",
  ""title"": ""string"",
  ""detail"": ""string"",
  ""status"": number,
  ""instance"": ""string""
}}
```
");
        return text.ToString();
    }
}
