# Relatieaanvragen: verval na 90 dagen (story 13)

> Status: geïmplementeerd · design 2026-11-25, beslissingen en implementatie 2026-11-26
> Basis: branch `or-3805` (indienen, bevestigen, weigeren, intrekken) en het bestaande patroon in `AssociationRegistry.Scheduled.Host` (erkenningen verlopen).

## In het kort

Elke nacht zoekt een job in de scheduled host de aanvragen met status `Aangevraagd` waarvan de vervaldatum voorbij is. Per aanvraag stuurt de job een command naar een Postgres-queue van Wolverine. Een handler in dezelfde host laadt de aanvraag, controleert opnieuw of ze vervallen is, en bewaart `RelatieaanvraagWerdVervallen`. In dezelfde transactie verdwijnen de lock-documenten, zodat het paar weer vrij is. De projectie zet de status op `Vervallen`.

```
Scheduled host
  Quartz-cron (01:00 UTC)
    └─ VervalRelatieaanvragenJob
         └─ VervalRelatieaanvragenProcessor
              ├─ TeVervallenRelatieaanvragenQuery
              │    RelatieaanvraagDocument: status Aangevraagd, vervalDatum vóór vandaag
              └─ per aanvraag: SendAsync(CommandEnvelope<VervalRelatieaanvraagCommand>)
                   └─ Postgres-queue verval-relatieaanvragen-queue (Durable)
                        └─ VervalRelatieaanvraagCommandHandler
                             ├─ laadt de stream van de aanvraag
                             ├─ KanVervallenOp(vandaag)
                             │    nog Aangevraagd en vervaldatum voorbij? Anders overslaan.
                             └─ SaveAfsluiting: RelatieaanvraagWerdVervallen
                                  + lock-documenten weg, in 1 transactie

Admin projection host
  └─ RelatieaanvraagProjection: status Vervallen
```

## Wat er al is

- `RelatieaanvraagState.ThrowIfNotOpen` weigert bevestigen, weigeren en intrekken zodra vandaag na de vervaldatum ligt. Die controle blijft de echte grens, en de job sluit de aanvraag daarna enkel af (beslist: B, status én datum).
- `RelatieaanvraagRepository.SaveAfsluiting` bewaart een afsluitend event en verwijdert de lock-documenten in één transactie. De verval-handler hergebruikt die.
- `RelatieaanvraagDocument` heeft `Status` en `VervalDatum`.
- De scheduled host heeft het patroon al voor erkenningen: Quartz-job, query, een command per item, handler in dezelfde host.

## Bevinding: de bestaande jobs gaan niet via de database

| Job | Hoe het command vertrekt | Wat er echt gebeurt |
|---|---|---|
| Erkenningen activeren en verlopen | `SendAsync(CommandEnvelope<…>)`, met routing op het kale command: `PublishMessage<VerloopErkenningCommand>().ToPostgresqlQueue(…)` | De regel matcht de envelope niet. Wolverine valt terug op een lokale queue in het geheugen (`BufferedInMemory`). Er komt niets in Postgres. |
| Bewaartermijnen | `InvokeAsync` | Inline in de job. De geconfigureerde Postgres-queue wordt niet gebruikt. |

Nagegaan met een proef op Wolverine 6.17.0, dezelfde versie als VR:

- `PublishMessage<Cmd>()` en dan `SendAsync(Envelope<Cmd>)` → `local://envelopecmd/`, mode `BufferedInMemory`.
- `PublishMessage<Envelope<Cmd>>().ToPostgresqlQueue(…)` → de Postgres-queue, mode `Durable`.
- `ToPostgresqlQueue` en `ListenToPostgresqlQueue` zijn standaard `Durable`, zonder `UseDurableInbox` of `UseDurableOutbox`.

Gevolg:

- Erkenningen: herstart de host tijdens de verwerking, dan gaan de commands in het geheugen verloren.
- Bewaartermijnen: faalt er één, dan stopt de lus. De job meldt het in Slack.
- In beide gevallen vindt de query ze de volgende nacht opnieuw, dus er gaat niets definitief mis. Maar het is niet wat de configuratie belooft.

Voor verval routeren we het type dat echt verstuurd wordt, en een test bewaakt dat. De bestaande jobs rechtzetten gebeurt later, in een aparte commit.

## Ontwerpkeuzes

### 1. De query leest de projectie

`RelatieaanvraagDocument` met `Status == Aangevraagd` in de database, daarna `VervalDatum` vóór vandaag in het geheugen, zoals bij de erkenningen. Het gaat om weinig documenten.

- De lock-documenten kennen de vervaldatum niet, dus daar kan de query niet op werken.
- Loopt de projectie achter, dan is dat geen probleem: de handler beslist op de stream. In het slechtste geval komt een aanvraag een dag later aan de beurt.

### 2. De regel staat één keer in het domein

Vervallen betekent: vandaag ligt na de vervaldatum. Op de vervaldatum zelf is de aanvraag nog open. Dat staat nu in `ThrowIfNotOpen`. Het staat in één functie, `Relatieaanvraag.IsTermijnVerstreken`, die `ThrowIfNotOpen`, de handler en de query gebruiken.

De job rekent niet met `aanvraagDatum + 90`. Hij gebruikt de bewaarde `vervalDatum`. De vervaltermijn is 90 dagen, per environment instelbaar met `RelatieaanvraagOptions:VervaltermijnInDagen`. Een andere termijn geldt enkel voor nieuwe aanvragen: bestaande houden hun vervaldatum.

### 3. Durable via een Postgres-queue

- Routing op `CommandEnvelope<VervalRelatieaanvraagCommand>`, het type dat de processor verstuurt.
- Queue `verval-relatieaanvragen-queue`. Zender en luisteraar zitten in de scheduled host.
- Het command staat in de database zodra `SendAsync` terugkomt. De handler verwerkt het ook na een herstart.
- Een test controleert dat de envelope naar de Postgres-queue gaat, in mode `Durable`.

### 4. De handler is idempotent

Een command kan dubbel komen: Wolverine levert minstens één keer, de job kan twee keer draaien (cron en manuele trigger), en de projectie kan achterlopen. Daarom:

- De handler laadt de aanvraag zelf en kijkt opnieuw: is ze nog `Aangevraagd` en is de vervaldatum voorbij?
- Zo niet, dan slaat hij de aanvraag over met een log, zonder fout. Een fout zou retries en dead letters geven voor iets wat normaal is.
- Zoals bij de erkenningen (tester-doer): `RelatieaanvraagState.KanVervallenOp(vandaag)` en `Verval(vandaag)`, naar `KanVerlopenWordenOp` en `VerloopErkenning`. De handler vraagt eerst `KanVervallenOp`. `Verval` gooit `RelatieaanvraagKanNietVervallen` als iemand die vraag overslaat: een `ApplicationException`, zoals `ErkenningKanNietVerlopenWorden`, want er is geen partij die een foutmelding moet krijgen.
- De handler laadt de koepel en de vereniging niet. Verval hangt niet af van hun toestand. Zo sluit verval ook aanvragen af van een vereniging die intussen verwijderd of dubbel is.

### 5. Gelijktijdigheid

- `SaveAfsluiting` bewaart met de versie van de stream. Schrijft iets anders tussen laden en bewaren, dan volgt `UnexpectedAggregateVersionException`.
- Na de vervaldatum kan geen gebruiker nog schrijven (`ThrowIfNotOpen`). Dit gebeurt dus enkel bij een dubbel command.
- De handler krijgt een retry op die exception (1 s, 5 s, 15 s). Bij de nieuwe poging ziet hij `Vervallen` en slaat hij over.
- Andere fouten gaan na de retries naar de dead letters van Wolverine, zonder Slack-melding. De aanvraag blijft `Aangevraagd`, dus de job neemt ze de volgende nacht opnieuw mee.

### 6. Lock-documenten en het venster na middernacht

- `SaveAfsluiting` verwijdert de locks. Daarna is het paar vrij voor een nieuwe aanvraag, en vallen de K6- en K7-blokkades weg.
- Het venster: `ThrowIfNotOpen` kijkt naar de datum, maar de locks verdwijnen pas met het event van de job. Tussen middernacht (UTC) en de job:
  - geven bevestigen, weigeren en intrekken al "De relatieaanvraag is vervallen.";
  - botst een nieuwe aanvraag voor hetzelfde paar nog op de locks;
  - toont de GET nog `Aangevraagd`.
- Daarom draait de job vroeg: 01:00 UTC. Het venster duurt dan een uur, 's nachts.
- Faalt de job, dan blijft het paar zo staan tot de volgende run of een manuele trigger. De job meldt de fout in Slack, zoals de bewaartermijnen.
- Locks die zelf hun vervaldatum kennen, zouden het venster sluiten, maar vragen een migratie van de bestaande locks. Dat is te veel voor een uur 's nachts.
- Zonder datumcheck was er geen venster, maar dan wijkt VR af van story 13 (optie A, niet gekozen).

### 7. Event

`RelatieaanvraagWerdVervallen(string AanvraagId, DateOnly VervalDatum)`, een `IRelatieaanvraagEvent` in de stream van de aanvraag.

- De naam volgt de andere afsluitingen (`WerdBevestigd`, `WerdGeweigerd`, `WerdIngetrokken`) en de status `Vervallen`. Story 13 stelt ze al voor.
- Een eventnaam ligt vast zodra hij in productie staat.
- `VervalDatum` is het zakelijke feit: de aanvraag verviel na die dag. De datum waarop de job draaide hoort er niet in. Die hangt af van de job, en het tijdstip staat al in de metadata van het event.

### 8. Projectie en response

- `RelatieaanvraagProjection` zet de status op `Vervallen` en de `afhandelDatum` op de dag na de vervalDatum: de eerste dag dat de aanvraag vervallen is. Elke afsluiting vult `afhandelDatum` (2026-11-26, in plaats van de objecten `bevestiging`, `weigering` en `intrekking`).
- `GET …/relatieaanvragen?status=Vervallen` werkt daarmee vanzelf.
- Geen SQL-migratie: het document is JSON. Waar al afgesloten aanvragen in de oude vorm bewaard zijn, moet de projectie opnieuw opgebouwd worden.

### 9. Status ook in `ThrowIfNotOpen`

`ThrowIfNotOpen` gooit `RelatieaanvraagIsVervallen` ook op de status `Vervallen`, niet enkel op de datum. De boodschap blijft "De relatieaanvraag is vervallen."

### 10. Command met het aanvraagId als string

`VervalRelatieaanvraagCommand(string AanvraagId)`, zoals `VerloopErkenningCommand(string VCode, int ErkenningId)`. Het command gaat door de queue en wordt dus geserialiseerd: met een string is er geen JSON-converter nodig. De handler maakt er een `RelatieaanvraagId` van, zoals de erkenningen-handler `VCode.Create` doet.

- Een test doet een round-trip van de envelope door de serializer van de queue. De metadata heeft een `Instant`, en dit is de eerste envelope die echt door een queue van de scheduled host gaat.

### 11. Tijdzone

Alle datums in VR zijn UTC-dagen (`Clock.Today` in de containers), ook de aanvraag- en de vervaldatum. De job gebruikt dezelfde klok, dus alles blijft consistent. Story 13 vraagt "einde van de dag, Brusselse tijd": dat is een keuze voor heel VR, niet voor deze story.

### 12. Initiator

`CommandMetadata.ForDigitaalVlaanderenProcess`, zoals bij de erkenningen.

## Wijzigingen per laag

| Laag | Wijziging |
|---|---|
| Events | `RelatieaanvraagWerdVervallen` |
| Domein | `RelatieaanvraagState.KanVervallenOp` en `Verval`, met `RelatieaanvraagKanNietVervallen`. Eén functie voor "vandaag na de vervaldatum": `Relatieaanvraag.IsTermijnVerstreken`. `RelatieaanvraagState.Apply`. `ThrowIfNotOpen` ook op de status. |
| Command | `VervalRelatieaanvraagCommand` en `VervalRelatieaanvraagCommandHandler` met retry-policy, in CommandHandling |
| Opslag | `Load` en `SaveAfsluiting` bestaan al. Nieuw: een gedeelde registratie van `OpenRelatieaanvraagDocument` voor de Admin API en de scheduled host. |
| Projectie | `RelatieaanvraagProjection.Apply(RelatieaanvraagWerdVervallen)` |
| Scheduled host | Job, processor, query, `VervalRelatieaanvragenOptions.Cron`, trigger `POST v1/trigger/verval-relatieaanvragen`, Wolverine-routing en listener, Marten-registratie van beide documenten, de repository in DI, Slack-melding bij een fout |
| Config | `appsettings.json` van de scheduled host: `VervalRelatieaanvragenOptions:Cron` = `0 0 1 * * ?` |
| Admin API | `RelatieaanvraagOptions` uit de appsettings in DI: de vervaltermijn bij het indienen. Een voorbeeld met status `Vervallen` in de response-examples. Manuele trigger `POST /v1/admin/scheduledhost/trigger/verval-relatieaanvragen` (superadmin, 202), zoals de andere jobs. Weigert de scheduled host de trigger, dan geeft de Admin API een fout in plaats van 202. |
| Docs | Story 13 in `koepels-ontbrekende-punten.md` bijwerken. Confluence OR-3805, validatie 10 (niet vervallen): ook de status `Vervallen`. |

De scheduled host doet `AssertDatabaseMatchesConfigurationOnStartup`. Hij moet de tabellen dus exact zo registreren als de Admin API en de projection host. Daarom hergebruikt hij `RegisterRelatieaanvraagDocument()`, en komt er een gedeelde methode voor `OpenRelatieaanvraagDocument`.

## Tests

- Domein: vervalt de dag na de vervaldatum, niet op de vervaldatum zelf, en niet als ze al bevestigd, geweigerd, ingetrokken of vervallen is.
- `ThrowIfNotOpen` op status `Vervallen`, voor bevestigen, weigeren en intrekken.
- Handler: bewaart het event en geeft het paar vrij, zodat een nieuwe aanvraag voor hetzelfde paar lukt. Een afgesloten aanvraag of een tweede command doet niets en gooit niet.
- Projectie: `Given_RelatieaanvraagWerdVervallen` geeft status `Vervallen` en afhandelDatum = de dag na de vervalDatum.
- Query: enkel `Aangevraagd` met een vervaldatum vóór vandaag, zoals `TeVerlopenErkenningenQueryTests`.
- Processor: één `SendAsync` per aanvraag.
- Routing: `CommandEnvelope<VervalRelatieaanvraagCommand>` gaat naar de Postgres-queue, mode `Durable`.
- Serialisatie: round-trip van de envelope, met het `Instant` in de metadata.
- Postman: RA140 start het verval via de Admin API (superadmin) en controleert dat een aanvraag binnen de termijn open blijft. RA141: zonder token 401. Het verval zelf niet: de API zet de aanvraagdatum altijd op vandaag, dus een vervallen aanvraag kan je niet maken.

## Niet in deze story

- Aanvragen die niet meer kunnen slagen vroeger afsluiten (punt 4 in `relatieaanvragen-onduidelijkheden.md`). Verval ruimt ze wel op na 90 dagen.
- De routing van de erkenningen en de bewaartermijnen rechtzetten.
- Brusselse tijd.

## Beslist (2026-11-26)

1. Event `RelatieaanvraagWerdVervallen(AanvraagId, VervalDatum)`, met `VervalDatum` als enige datum.
2. De job draait om 01:00 UTC.
3. `ThrowIfNotOpen` kijkt naar de status én de datum (B hieronder), zoals story 13 vraagt.
4. De routing van de bestaande jobs komt later, in een aparte commit. Bij de bewaartermijnen verandert dan ook de foutafhandeling: nu komt een fout via `InvokeAsync` in de job en dus in Slack; via een queue komt ze na de retries in de dead letters.
5. De vervaltermijn is per environment instelbaar (`RelatieaanvraagOptions:VervaltermijnInDagen`, standaard 90), om verval op Test manueel te testen: met 1 dag vervalt een aanvraag twee dagen na het indienen, met 0 dagen de dag erna. Binnen één Postman-run kan het dus nooit.

### Waarom B: status én datum

| | A. Enkel status | B. Status en datum (gekozen) |
|---|---|---|
| Na middernacht, vóór de job | Bevestigen, weigeren en intrekken lukken nog. Wie eerst komt, wint. | "De relatieaanvraag is vervallen." |
| Venster (sectie 6) | Geen | Een uur 's nachts |
| Faalt de job | De aanvraag blijft open, ook na de `vervalDatum` die de API toont | Het paar zit vast tot de volgende run of een manuele trigger |
| Story 13 | Wijkt af | Volgt de story |
| Code | `today` uit `ThrowIfNotOpen` en de twee `Create`s. De datumtests van OR-3805 worden statustests. | Enkel de statuscheck erbij |

- Wie eerst komt, bij A: de versie van de stream laat maar één afsluiting toe. Wie als tweede bewaart, krijgt `UnexpectedAggregateVersionException`. De job slaat de aanvraag bij de retry over, een gebruiker krijgt 412.
- Story 13, volgens onze analyse: bevestigen na de vervaldatum, maar vóór de job draaide, moet falen (`koepels-ontbrekende-punten.md` bij story 13, `relatieaanvragen-onduidelijkheden.md`).
- B volgt de story, en `vervalDatum` in de API klopt dan altijd.
