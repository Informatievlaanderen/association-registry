# ADR 0001: Geotags herberekenen via event subscription in plaats van in de KBO-sync command handlers

- **Status:** Accepted
- **Datum:** 2026-10-06

## Context

Geotags van een vereniging worden afgeleid van haar locaties en werkingsgebieden. Wanneer de maatschappelijke zetel
van een vereniging met rechtspersoonlijkheid verandert via de KBO-sync, moeten de geotags opnieuw berekend worden.
Dat gebeurt door `VerenigingOfAnyKind.HerberekenGeotags(IGeotagsService)`, die een `GeotagsWerdenBepaald` event
toevoegt wanneer de berekende geotags verschillen van de huidige state (idempotent).

De relevante KBO-events zijn:

- `MaatschappelijkeZetelWerdOvergenomenUitKbo`
- `MaatschappelijkeZetelWerdGewijzigdInKbo`
- `MaatschappelijkeZetelWerdVerwijderdUitKbo` (resulteert in een lege `GeotagsWerdenBepaald`)

Er waren twee mogelijke plaatsen om de herberekening te triggeren:

1. **In de command handlers van de KBO-sync**: de `IGeotagsService` injecteren en `HerberekenGeotags` aanroepen na het
   verwerken van de KBO-data.
2. **Via een event subscription**: een Marten/Wolverine subscription (`HerberekenGeotagsEventHandler`) luistert naar
   de bovenstaande events en stuurt een `HerberekenGeotagsMessage` naar een Postgresql queue. Die wordt verwerkt door
   `HerberekenGeotagsMessageHandler`, die een `CommandEnvelope<HerberekenGeotagsCommand>` doorgeeft aan
   `HerberekenGeotagsCommandHandler`.

## Beslissing

We herberekenen geotags via **optie 2, een event subscription**, en voegen de `IGeotagsService` **niet** toe aan de
command handlers van de KBO-sync.

## Redenen

### 1. Historische events moeten de herberekening ook triggeren

Een subscription is gekoppeld aan events en niet aan het moment van verwerken van een command. Daardoor kunnen ook
reeds bestaande (oude) events de herberekening triggeren, bijvoorbeeld via een rebuild van de subscription
(`v1/projections/eventsubscription/herberekengeotags/rebuild`). Een oplossing in de command handlers zou enkel werken
voor nieuwe sync-runs. Bestaande verenigingen zouden hun geotags dan nooit krijgen zonder opnieuw te syncen.

### 2. De KBO-sync command handler is al groot en de service zou door alle lagen moeten

De handler van de KBO-sync is nu al lang. De `IGeotagsService` toevoegen betekent dat hij doorgegeven moet worden aan
elke laag tot aan de plek waar `HerberekenGeotags` effectief wordt aangeroepen:
handler, aggregate-methode(s), en helper-methodes (`NeemDataOverUitKbo`, `WijzigMaatschappelijkeZetelUitKbo`, ...).
Dit vergroot de koppeling en de verantwoordelijkheden van een handler die al te veel doet.

### 3. Moeilijk te testen door de ketting

Door de service door meerdere lagen te moeten doorgeven, is elke test van de sync afhankelijk van een geconfigureerde
`IGeotagsService`-mock en moet de volledige keten opgezet worden. Met een aparte command handler
(`HerberekenGeotagsCommandHandler`) is het gedrag geïsoleerd en eenvoudig te testen met een scenario en context
(`HerberekenGeotagsContext`). Dit wordt gedaan voor zowel `VerenigingZonderEigenRechtspersoonlijkheid` als
`VerenigingMetRechtspersoonlijkheid`, inclusief de idempotentiecheck.

## Gevolgen

### Positief

- Oude en nieuwe events triggeren dezelfde herberekening, en een rebuild van de subscription is mogelijk.
- KBO-sync handlers blijven ongewijzigd en kennen geen geotags.
- Herberekening is geïsoleerd en eenvoudig unit testbaar. De E2E-tests dekken de volledige flow.
- De herberekening is idempotent: er wordt enkel een `GeotagsWerdenBepaald` event toegevoegd bij een wijziging.

### Negatief / aandachtspunten

- **Eventual consistency**: geotags worden asynchroon bijgewerkt. Tests moeten pollen tot het event aanwezig is.
- **Meer bewegende delen die geregistreerd moeten worden**, en die elk afzonderlijk kunnen ontbreken:
  - Marten subscription in de projection host (shard `beheer.eventsubscription.herberekengeotags`)
  - `options.Discovery.IncludeType(typeof(HerberekenGeotagsEventHandler))` in de Admin ProjectionHost
  - Publish naar de Postgresql queue `WellknownQueueNames.HerberekenGeotagsQueueName` vanuit de projection host
  - `ListenToPostgresqlQueue(...)` in de Admin Api (`PostgresWolverineSetup`)
- Wanneer een nieuw event de maatschappelijke zetel kan wijzigen, moet het toegevoegd worden aan
  `HerberekenGeotagsEventHandler`.

## Overwogen alternatieven

- **`IGeotagsService` in de command handlers** (`SyncKboCommandHandler`, `RegistreerVerenigingUitKboCommandHandler`): afgewezen om de bovenstaande redenen.

