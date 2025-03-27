---
title: Lokaal testen
---
# Duplicate Detection lokaal testen

## Applicatie opstarten

```
docker compose -f docker-compose.yml -f docker-compose.services.yml up --build -d
```

## Authenticatie token verkrijgen

```
docker compose -f docker-compose.yml -f docker-compose.services.yml up token-fetcher
```

Neem het `access_token` over.

## Api calls

Geef bij elke call naar het verenigingsregister volgende headers mee:

- `--header 'X-Correlation-Id: <random string>'`
- `--header 'VR-Initiator: <random string>'`
- `--header 'Authorization: Bearer <access_token>'`
- `--header 'Content-Type: application/json'`



### Registratie voorbeeld

```
curl --location --globoff 'http://127.0.0.1:11004/v1/verenigingen/vzer' -v \
--data '{
"naam": "VerenigingNaam",
"korteNaam": "korteNaam",
"korteBeschrijving": "korteBeschrijving",
"startdatum": "2022-01-01",
"isUitgeschrevenUitPubliekeDatastroom": "false",     "locaties":[
{
    "naam": "VerenigingNaam egon",
    "korteNaam": "korteNaam",
    "korteBeschrijving": "korteBeschrijving",
    "startdatum": "2022-01-01",
    "isUitgeschrevenUitPubliekeDatastroom": "false",
    "locaties": [
        {
            "locatietype": "Activiteiten",
            "adres": {
                "straatnaam": "taartstraat",
                "huisnummer": "121",
                "postcode": "8448",
                "gemeente": "Mellem",
                "land": "België"
            }
        }
    ]
}'
````

### Minimumscore

Voor meer informatie over hoe minimumscore werkt, klik [hier](Basisconcepten.md#minimumscore).

#### Opvragen

```
curl --location 'http://localhost:11004/v1/admin/config/minimumScoreDuplicateDetection' 
```

#### Wijzigen

```
curl --location 'http://localhost:11004/v1/admin/config/minimumScoreDuplicateDetection' \
--data '{
    "waarde": 5
}'
```

#### Controleren duplicate score

Om te controleren welke score een vereniging zou krijgen bij registratie, kan je gebruik maken van het `dubbelControle` endpoint.

Je kan de `minimumScore` voor deze call overschrijven aan de hand van de query parameter `minimumScoreOverride`.

```
curl --location 'http://localhost:11004/v1/admin/dubbelcontrole?minimumScoreOverride=0' \
--data '{
    "naam": "klj hooglede",
    "locaties": [
        {
            "locatietype": "activiteiten",
            "adres": {
                "straatnaam": "x",
                "huisnummer": "x",
                "postcode": "1243",
                "gemeente": "hooglede",
                "land": "x"
            }
        }
    ]
}'
```