# Afwijkend gedrag PATCH

## Hoe wijkt het af?

In tegenstelling tot onze andere PATCH endpoints, is het opgeven van `subtype` altijd verplicht.

Afhankelijk van het huidige subtype, en het gewenste subtype, worden andere velden toegelaten en verwacht.
Dit terwijl andere PATCH endpoints eerder uitgaan van het principe dat niet-meegestuurde waarden betekenen dat ze niet gewijzigd worden.

Voorbeeld: Bij het wijzigen van een vereniging met subtype `Subvereniging` naar `FV`, worden de `identificatie` en `beschrijving` velden niet verwacht.

Deze worden genegeerd indien toch meegegeven, en worden verwijderd op de vereniging.

## Waarom?

Een centraal endpoint voor dit soort wijzigingen leek ons eenvoudiger voor de afnemers. 

Ook gezien de migratie van `FV` naar `VZER`, waarbij we `POST v1/feitelijkevereniging` nog tijdelijk blijven ondersteunen,
leek ons dit beter om verwarring te vermijden.