# Import erkenningen

Interne C# scripts om erkenningen uit een Excel-bestand te valideren, lokaal te testen en daarna via de Admin API op te laden.

De flow is bedoeld voor gecontroleerde imports:

1. Dry-run op het Excel-bestand.
2. Ontbrekende v-codes seeden in een lokale of tijdelijke database.
3. Kleine uploadbatch naar de Admin API.
4. Volledige upload als de batch slaagt.
5. Alleen failure data bewaren: invalid rows en failed uploads.

Gebruik altijd expliciete pad- en rapportnamen. De bestandsnaam, sheetnaam en outputnaam zijn geen onderdeel van de logica.

```bash
FILE="/pad/naar/erkenningen.xlsx"
SHEET="Import"
RUN_ID="erkenningen-$(date +%Y%m%d-%H%M%S)"
```

Als de Excel een andere sheetnaam gebruikt, bijvoorbeeld `Blad1`, zet dan:

```bash
SHEET="Blad1"
```

## Verwachte kolommen

De importscript ondersteunt deze canonical kolommen:

- `vCode`
- `geregistreerdDoor`
- `ipdcProduct`
- `startdatum`
- `einddatum`

Daarnaast ondersteunt ze deze alias-kolommen:

- `V-code` voor `vCode`
- `Erkend door OVO-code` voor `geregistreerdDoor`
- `IPDC-code` voor `ipdcProduct`
- `Ingangsdatum` voor `startdatum`
- `Einddatum` voor `einddatum`

Datums mogen als `yyyy-MM-dd` of als Excel serial date in het bestand staan.

## Dry-run

De dry-run valideert het bestand en doet geen API-calls.

```bash
dotnet-script tools/import-erkenningen/import-erkenningen.csx --no-cache -- \
  --file "$FILE" \
  --sheet "$SHEET" \
  --report "./${RUN_ID}-dryrun-failed.csv"
```

Exitcode `0` betekent dat alle rijen valide zijn. Exitcode `2` betekent dat er invalid rows of upload failures zijn.

Het CSV-bestand is een failure report. Het bevat alleen invalid rows of failed uploads. Bij een volledig succesvolle run bevat het alleen de header.

Als er failed rows zijn die bewaard moeten blijven, commit dan alleen die `*-failed.csv`. Verwijder gewone dry-run, seed en uploadrapporten met succesvolle rijen.

## v-codes seeden

De Admin API kan alleen erkenningen toevoegen aan bestaande verenigingen. In lokale of tijdelijke omgevingen ontbreken v-codes uit externe exports vaak. Gebruik daarom eerst de bestaande seed tool tegen dezelfde database als de Admin API.

Seed lokaal:

```bash
dotnet run --project tools/SeedTestData/SeedTestData.csproj -- \
  --import-erkenningen-vcodes-file "$FILE" \
  --import-erkenningen-vcodes-sheet "$SHEET"
```

Voor een andere database:

```bash
ConnectionString="<connection-string>" \
dotnet run --project tools/SeedTestData/SeedTestData.csproj -- \
  --import-erkenningen-vcodes-file "$FILE" \
  --import-erkenningen-vcodes-sheet "$SHEET"
```

De import-vcode mode in `SeedTestData` is idempotent: bestaande streams worden geskipt, ontbrekende v-codes worden als minimale VZER streams aangemaakt.

Je kan dezelfde input ook via environment variables meegeven:

```bash
IMPORT_ERKENNINGEN_VCODES_FILE="$FILE" \
IMPORT_ERKENNINGEN_VCODES_SHEET="$SHEET" \
dotnet run --project tools/SeedTestData/SeedTestData.csproj
```

Let op: deze mode maakt minimale VZER streams aan. Dat is bedoeld voor lokaal en gecontroleerde testdata. Gebruik dit niet op productie zonder expliciete datamigratiebeslissing.

## Lokale services

Start de lokale dependencies:

```bash
docker compose up -d
docker wait vr_create_verenigingsregister_db
```

Start daarna de Admin API lokaal op poort `11004`. Voor `dotnet run` zijn de lokale admin introspection settings nodig, anders worden lokale beheer-tokens geweigerd. Zet die settings in je shell of in een lokale env-file die niet gecommit wordt.

```bash
set -a
source .env.local-admin-api
set +a

ASPNETCORE_ENVIRONMENT=Development \
dotnet run --project ./src/AssociationRegistry.Admin.Api/AssociationRegistry.Admin.Api.csproj
```

Haal een lokale token op:

```bash
export IMPORT_ERKENNINGEN_BEARER_TOKEN="$(
  curl -s -X POST "http://127.0.0.1:5051/connect/token" \
    -u "$ACM_BASIC_AUTH" \
    -H "Content-Type: application/x-www-form-urlencoded" \
    -d "grant_type=client_credentials&scope=dv_verenigingsregister_beheer" \
  | python3 -c 'import json,sys; print(json.load(sys.stdin)["access_token"])'
)"
```

`ACM_BASIC_AUTH` hoort ook uit je lokale env-file te komen en heeft het formaat `<client-id>:<client-credential>`.

## Lokale upload

Upload eerst een kleine batch:

```bash
dotnet-script tools/import-erkenningen/import-erkenningen.csx --no-cache -- \
  --file "$FILE" \
  --sheet "$SHEET" \
  --base-url "http://localhost:11004" \
  --upload \
  --limit 25 \
  --report "./${RUN_ID}-upload-25-failed.csv"
```

Als de batch slaagt, upload de rest:

```bash
dotnet-script tools/import-erkenningen/import-erkenningen.csx --no-cache -- \
  --file "$FILE" \
  --sheet "$SHEET" \
  --base-url "http://localhost:11004" \
  --upload \
  --skip 25 \
  --report "./${RUN_ID}-upload-rest-failed.csv"
```

## Upload naar test of productie

Gebruik dezelfde script met de juiste Admin API base-url en token.

```bash
IMPORT_ERKENNINGEN_BEARER_TOKEN="<token>" \
dotnet-script tools/import-erkenningen/import-erkenningen.csx --no-cache -- \
  --file "$FILE" \
  --sheet "$SHEET" \
  --base-url "https://<admin-api-host>" \
  --upload \
  --report "./${RUN_ID}-upload-failed.csv"
```

Voor productie: run eerst dry-run en testupload, bewaar alleen de failure reports, en gebruik geen lokale seeddata als basis voor productie zonder expliciete akkoord over de data-aanpak.

## API-call per rij

Per geldige rij roept de tool:

```http
POST /v1/verenigingen/{vCode}/erkenningen
X-Correlation-Id: <guid>
VR-Initiator: {geregistreerdDoor}
```

met body:

```json
{
  "Erkenning": {
    "ipdcProductNummer": "<ipdcProduct>",
    "Startdatum": "yyyy-MM-dd",
    "Einddatum": "yyyy-MM-dd",
    "Hernieuwingsdatum": null,
    "HernieuwingsUrl": ""
  }
}
```

## WireMock en IPDC

De Admin API verrijkt en valideert het `ipdcProduct` via de IPDC-integratie. Lokaal loopt die integratie via WireMock. Als WireMock het productnummer uit de import niet kent, faalt de upload met een melding zoals:

```text
Voor ipdc product nummer '<productnummer>' werd geen ipdc product gevonden.
```

Daarom is er lokaal een WireMock-stub toegevoegd voor product `38738`, omdat de huidige imports dat product gebruiken:

- `wiremock/mappings/IpdcProduct.38738.json`
- `wiremock/__files/Ipdc/Ipdc.38738.json`

Die files zijn noodzakelijk voor lokale uploads zolang de lokale WireMock geen echte of bestaande response voor `38738` heeft. Ze zijn niet nodig omdat de importscript zelf dat eist, maar omdat de Admin API dezelfde IPDC-validatie uitvoert als in de gewone flow.

Voor test en productie hoort dit via de echte omgeving of bestaande configuratie/data te komen. Als een toekomstig bestand een ander IPDC-product bevat, moet lokaal ofwel WireMock dat product kennen, ofwel de lokale IPDC-configuratie naar een bron wijzen die het product kent.
