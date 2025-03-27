# Wanneer de verwerking van een KBO Sync naar de DLQ sturen

## Beslissing

We willen enkel zaken in de DLQ plaatsen wanneer de vereniging gekend is in het Verenigingsregister.

## Motivatie

Wanneer iets niet gekend is in het Verenigingsregister, gaat het waarschijnlijk over een niet-geslaagde registratie van een vereniging,
waarbij de RegistreerInschrijving call wel is geslaagd.

We willen altijd eerst RegistreerInschrijving laten slagen, omdat we geen KBO verenigingen willen hebben in het Verenigingsregister
zonder inschrijving bij Magda.