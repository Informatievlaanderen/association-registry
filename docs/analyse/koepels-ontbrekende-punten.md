# Koepels — wat ontbreekt er nog om te starten

> Status: werkdocument · 2026-11-19 (bijgewerkt: header, GI/KBO, aggregate, koepelstatus, stories 1+25 samengevoegd, story 2 met de nieuwe storytekst)
> Bron: analyse "Koepelvereniging als GI" + user stories 1–25.
> Doel: per story oplijsten wat nog beslist of verduidelijkt moet worden vóór development, en een **voorstel** van request/response geven in de stijl van de bestaande Beheer-API.

**Legende**

| Symbool | Betekenis |
|---|---|
| 🟢 | Kan starten: de open punten blokkeren niet. |
| 🟡 | Kan deels starten: domein/events wel, endpoint of autorisatie nog niet. |
| 🔴 | Geblokkeerd: eerst een beslissing nodig. |

---

## 0. Overkoepelende punten (blokkeren meerdere stories)

Deze punten komen in bijna elke story terug. Het is efficiënter ze één keer te beslissen.

### 0.1 Wie handelt er, en namens wie? — ✅ beslist

Vandaag kent de Beheer-API alleen de header `VR-Initiator`, de OVO-code van de afnemer. Voor de koepelstories moet VR ook weten **namens welke vereniging/koepel** er gehandeld wordt.

**Beslist**
- **Nieuwe header `vr-namens`** met de handelende vCode. De schrijfwijze volgt `vr-api-key`: kleine letters.
- **Alles verloopt via MAGDA**, ook de CRM-kant.
- **Flow:** de gebruiker logt in bij een BFF. Daar wordt bepaald voor welke vCode(s) de gebruiker aanpassingen mag doen, en de BFF stuurt `vr-namens` mee. De gebruiker kan de headers zelf niet aanpassen.
- **Actieve vertegenwoordiger:** dit wordt bij het inloggen gecontroleerd. VR doet zelf geen extra INSZ-controle per request.
- **`VR-Initiator`:** elke initiator mag de acties uitvoeren.
- **Audit:** de handelende vCode volstaat. De vertegenwoordiger (INSZ) hoeft niet in de metadata.
- **Header ontbreekt:**
  - bij normale aanpassingen: de gewone flow, zoals vandaag;
  - bij de relatieacties (aanvragen, bevestigen, weigeren, intrekken): `400` met dezelfde soort boodschap als bij een ontbrekende initiator: **"vr-namens is verplicht."**
- **Header is ingevuld:** elk beheerendpoint controleert of de `vr-namens`-vCode acties mag doen op de vereniging in de route.
  - Toegelaten: dezelfde vCode, of een koepel met beheerrecht (story 9/18).
  - Niet toegelaten: **`403 Forbidden`**.
  - Dit vraagt een generieke check op alle endpoints, bv. middleware zoals de bestaande `InitiatorHeaderMiddleware`.

### 0.2 Wie kan koepel zijn? — ✅ beslist

- **De koepel (parent) is altijd een KBO-vereniging**, dus een vereniging met rechtspersoonlijkheid.
- **Een VZER kan zich niet als koepel identificeren.** `Koepel` zetten op een VZER geeft `400`.
- **Kinderen** (leden of subverenigingen) kunnen zowel KBO als VZER zijn. Subverenigingen blijven wel beperkt tot VZER (story 12).
- **Enkel een koepel** (status `Koepel`) kan de privacyverklaring accepteren.

### 0.2b Ketens van relaties — ✅ beslist

Notatie: `X --Lid--> Y` betekent dat X lid is van Y, en `X --Sub--> Y` dat X subvereniging is van Y. Pijlen wijzen van laag naar hoog. A, B en D zijn koepels (KBO); C en E zijn gewone verenigingen.

**Regels**

| # | Regel | Gevolg |
|---|---|---|
| K1 | **Een koepel kan zelf lid zijn van een of meerdere koepels.** | `B --Lid--> A` en `B --Lid--> D` mogen samen. |
| K2 | **Meerdere lidmaatschappen zijn mogelijk, ook op verschillende niveaus van dezelfde keten.** | `E --Lid--> B` en `E --Lid--> A` mogen samen, ook al is B lid van A. |
| K3 | **Een sub is altijd het laagste niveau.** Niemand kan lid of sub zijn van een subvereniging. | Een sub kan nooit koepel zijn. Dat volgt ook uit K5, want een sub is een VZER. |
| K4 | **Een koepel kan meerdere subverenigingen hebben.** | `C --Sub--> B` en `E --Sub--> B` mogen samen. |
| K5 | **Een koepel moet een KBO-vereniging zijn.** | Een VZER kan nooit parent zijn. |
| K6 | **Geen sub én lid tegelijk.** Een vereniging die sub is, kan nergens lid zijn, en omgekeerd. | `E --Sub--> B` + `E --Lid--> A`: ❌ |
| K7 | **Maximaal één sub-relatie per vereniging.** | `E --Sub--> B` + `E --Sub--> A`: ❌ |
| K8 | **Subs onder verschillende koepels in dezelfde keten zijn onafhankelijk.** | `C --Sub--> B` (B is lid van A) en `E --Sub--> A` mogen samen. |

**Voorbeelden (alle drie geldig)**
```text
Diagram 1 — leden over meerdere niveaus
  C --Sub--> B --Lid--> A
                 --Lid--> D
  E --Lid--> B
  E --Lid--> A

Diagram 2 — meerdere subs onder één koepel
  C --Sub--> B --Lid--> A
  E --Sub--> B --Lid--> D

Diagram 3 — subs op verschillende niveaus
  C --Sub--> B --Lid--> A
                 --Lid--> D
  E --Sub--> A
```

**Ongeldig**
```text
  E --Sub--> B  +  E --Lid--> A     (K6: geen sub én lid)
  E --Sub--> B  +  E --Sub--> A     (K7: max. één sub)
  X --Lid--> C  waarbij C --Sub--> B (K3: sub is altijd laagste)
  X --Lid--> E  waarbij E een VZER is (K5: koepel moet KBO zijn)
```

**Ook beslist** (K9 en K10 bevestigd; K11 volgt daaruit, nog te bevestigen)
| # | Regel | Gevolg |
|---|---|---|
| K9 | **Ketens en cycli van Lid-relaties tussen koepels zijn toegestaan** (gewijzigd op 2026-11-19). Een keten die terug naar een sub loopt, kan niet: een sub is geen KBO-vereniging en dus nooit een koepel. | A ↔ B en A ↔ B ↔ C (allemaal koepels): ✅. `C --Sub--> B` + `B --Lid--> C`: ❌. |
| K10 | **Beheer enkel via rechtstreekse relaties**, en enkel met geaccepteerde privacyverklaring. | Bij `C → B → A` mag A B beheren en mag B C beheren, maar A mag C niet beheren. In diagram 1 mag A E wel beheren, via de rechtstreekse relatie `E --Lid--> A`. |
| K11 | **`leden`/`subverenigingen` tonen enkel rechtstreekse relaties.** | `leden` van A toont B en E (diagram 1), niet C. |

**Toegestaan (ketens tussen koepels, K9)**
```text
  A --Lid--> B  +  B --Lid--> A                                    (A en B zijn koepels)
  A --Lid--> B  +  B --Lid--> A  +  B --Lid--> C  +  C --Lid--> B  (A, B en C zijn koepels)
```
Er is dus geen cyclusdetectie nodig: een Lid- of Sub-relatie kan enkel naar een koepel (KBO) wijzen.

### 0.3 Acceptatie toelatingsvoorwaarden — ✅ beslist

- `privacyVerklaringGeaccepteerd = true` dekt ook de toelatingsvoorwaarden. Er komt geen tweede veld.
- De versie of datum van de geaccepteerde verklaring wordt niet apart bewaard. Het eventtijdstip volstaat.

### 0.4 Technisch: relatieaanvraag als apart aggregate — ✅ beslist

- **Eigen aggregate/stream** `Relatieaanvraag`, los van de verenigingen.
- **Bij bevestiging** wordt de feitelijke relatie (lidmaatschap, of SUB + subtype) op de vereniging(en) gezet via commands of de outbox, **in één transactie** met de statuswijziging van de aanvraag.
  - Aandachtspunt: de analyse eist dat "bevestigd" enkel bestaat als de relatie ook geslaagd is.
  - Marten kan in één `SaveChanges` naar meerdere streams schrijven. Zo zitten aanvraag én vereniging in dezelfde databasetransactie.
  - Bij asynchrone outbox-verwerking is een compenserende stap nodig als het plaatsen van de relatie faalt.
- **Uniciteitsregels:** enkel aanvragen met status `Aangevraagd` tellen mee. Bevestigd, geweigerd, ingetrokken en vervallen tellen niet meer.
  - Max. **1 open aanvraag per koepel–verenigingpaar**.
  - Max. **1 open SUB-aanvraag per vereniging**, over alle koepels heen.
  - **Lidmaatschapsaanvragen** naar verschillende koepels mogen er **meerdere** tegelijk zijn.
  - Deze regels lopen over aggregates heen. Er is een projectie/query nodig met een concurrency-guard, bv. een unieke index op (paar, open) en op (vereniging, open SUB).

### 0.5 `aanvraagId` — ✅ beslist

- Een **leesbare sequentie zoals de vCodes**, bv. `RA0000001`, via een eigen Marten-sequence.
- Het id is uniek over de tijd. De vCodes staan als aparte velden in de aanvraag.
- [ ] Prefix bevestigen: `RA`?

### 0.6 Koepelstatus — ✅ beslist

- Het veld heet **`koepelstatus`** en vervangt `isKoepelvereniging` uit de analyse.
- De waarden zijn **`Koepel` / `GeenKoepel` / `NietBepaald`**, **overal**: intern, in de documents, in requests en in responses. Er wordt nergens `null`/`true`/`false` gebruikt.
- Standaardwaarde: `NietBepaald`.

### 0.7 Notificaties (story 14)

Het VLO-endpointcontract bestaat nog niet. Voorstel: VR publiceert per trigger een intern bericht (outbox/Wolverine). De VLO-call komt daarachter. Zo blokkeren stories 2–24 niet op het VLO-contract.

### 0.8 Publiek of enkel Beheer? — ✅ beslist

**Voorlopig enkel Beheer.** Koepelgegevens en relatieaanvragen komen niet in de Publiek API.

### 0.9 Tegenstrijdigheden in de bronteksten — ✅ beslist

| Onderwerp | Beslissing |
|---|---|
| Beëindigen SUB-relatie | **De koepel** kan beheren en beëindigen (story 24). Het is geen exclusieve VR-actie. |
| Subtype na SUB-verwijdering | SUB → **NB** |
| Leeg of NB + bevestigd lidmaatschap | Leeg of NB → **FV** (leeg bevestigd op 2026-11-23) |
| Subvereniging beëindigt zelf | **Niet toegelaten.** De sub kan haar eigen SUB-koppeling niet laten beslissen; enkel de koepel kan dat. Het bestaande `PATCH /v1/verenigingen/{vCode}/subtype` krijgt dus een regel: met `vr-namens` = de sub zelf → `403`. |

- [ ] Gevolg om te bevestigen: calls **zonder** `vr-namens`, dus andere GI's in de gewone flow, behouden hun huidige rechten op `PATCH /subtype`.

---

## Conventies voor alle voorstellen hieronder

De voorstellen volgen de bestaande Beheer-API:
- **Schrijfacties** → `202 Accepted`, met headers `ETag`, `VR-Sequence` en `Location` (bij aanmaak).
- **`If-Match`** optioneel voor optimistic concurrency → `412` bij een verouderde versie.
- **Validatiefout** → `400` met `ValidationProblemDetails`. **Businessregelfout** → `400` met `ProblemDetails`.
- **`vr-namens` hoort niet bij de vereniging** → `403`.
- **Header `VR-Initiator`** verplicht. Elke initiator mag de acties uitvoeren.
- **Header `vr-namens`**: de handelende vCode (0.1).
- Prefix `/v1/`.

---

## 1. Koepel worden

### Stories 1 + 25 — Koepelstatus en privacyverklaring vastleggen · M · 🟢

Stories 1 en 25 worden **samengenomen** in één `PATCH`-endpoint.

**Scope van deze story:** enkel het request en het tonen in de detail. Je zegt "ik ben koepel of niet" en of de privacyverklaring geaccepteerd is. Meer niet.

| In scope | Later |
|---|---|
| `PATCH /v1/verenigingen/{vCode}/koepelgegevens` | `leden`/`subverenigingen` in de detail (story 10/17) |
| `koepelstatus` + `privacyVerklaringGeaccepteerd` | Relaties verwijderen bij wissel naar GeenKoepel/NietBepaald (story N1) |
| Validaties op het request zelf (KBO, privacy enkel bij Koepel) | `vr-namens`-check (story 18) |
| Tonen in de Beheer-detail | Effect op aanvragen en beheer (stories 2–5, 9) |

**Al beslist**
- Eén endpoint **`PATCH /v1/verenigingen/{vCode}/koepelgegevens`**.
- Er kunnen enkel **twee velden** meegegeven worden, allebei optioneel (PATCH):
  - `koepelstatus`: `Koepel` / `GeenKoepel` / `NietBepaald` (0.6). Hoofdletterongevoelig, zoals `contactgegeventype`: `koepel` of `KOEPEL` wordt als `Koepel` bewaard;
  - `privacyVerklaringGeaccepteerd`.
- Standaard: `NietBepaald` en `false`.
- Enkel een **KBO-vereniging** kan `Koepel` worden (0.2).
- `privacyVerklaringGeaccepteerd = true` kan enkel bij `Koepel`.
  - Bij `GeenKoepel` of `NietBepaald` moet de waarde `false` zijn, anders `400`.
  - Wordt de status `GeenKoepel` of `NietBepaald`, dan wordt de privacyverklaring automatisch `false`.
- Verder is niets verplicht.
- Enkel Beheer (0.8).
- De versie of datum van de privacyverklaring wordt niet bewaard (0.3).

**Ontbreekt nog** (zo geïmplementeerd, nog te bevestigen)
- [ ] Mag de status terug naar `NietBepaald`, of enkel `Koepel` ↔ `GeenKoepel`? **Geïmplementeerd:** alle overgangen zijn toegelaten, ook terug naar `NietBepaald`.
- [ ] Dezelfde waarde opnieuw zetten: idempotent, zonder event? **Geïmplementeerd:** ja. Er komt geen event en het antwoord is `200 OK` zonder sequence-header, zoals bij de andere PATCH-endpoints.

**Voorstel request**
```http
PATCH /v1/verenigingen/V0000001/koepelgegevens
VR-Initiator: OVO000000
If-Match: W/"12"
Content-Type: application/json

{
  "koepelstatus": "Koepel",
  "privacyVerklaringGeaccepteerd": true
}
```
Beide velden zijn optioneel. Een request met enkel `{ "privacyVerklaringGeaccepteerd": true }` kan dus ook, als de vereniging al `Koepel` is.

**Voorstel response**
```http
202 Accepted
ETag: W/"13"
VR-Sequence: 4711
```

**Fouten**
| Situatie | Response |
|---|---|
| `Koepel` op een VZER | `400` "Enkel een vereniging met rechtspersoonlijkheid kan als koepel geïdentificeerd worden." |
| `privacyVerklaringGeaccepteerd = true` bij `GeenKoepel`/`NietBepaald` (in de request of in de huidige toestand) | `400` "De privacyverklaring kan enkel door een koepelvereniging geaccepteerd worden." |
| Ongeldige statuswaarde | `400` (validatie) |
| Leeg request (geen van beide velden) | `400` (validatie, zoals bij andere PATCH-endpoints) |

**Gevolg in Beheer-detail (`GET /v1/verenigingen/V0000001`)**
```json
{
  "vereniging": {
    "vCode": "V0000001",
    "koepelgegevens": {
      "koepelstatus": "Koepel",
      "privacyVerklaringGeaccepteerd": true
    }
  }
}
```

**Standaard, nog niet geïdentificeerd** (ook voor bestaande verenigingen zonder event)
```json
"koepelgegevens": {
  "koepelstatus": "NietBepaald",
  "privacyVerklaringGeaccepteerd": false
}
```

> **Later (story 10/17):** `koepelgegevens` krijgt `leden` en `subverenigingen`, als lege arrays wanneer er geen zijn. Dat is de omgekeerde leesrichting van `lidmaatschappen` en `subverenigingVan` op de kinderen, en toont enkel rechtstreekse relaties (K11).

**Events** (één event per overgang, naar analogie met de subtype- en werkingsgebied-events)

| Overgang | Event | Historiek |
|---|---|---|
| → Koepel | `VerenigingWerdGeidentificeerdAlsKoepel` | "Vereniging werd geïdentificeerd als koepel." |
| → GeenKoepel | `VerenigingWerdGeidentificeerdAlsGeenKoepel` | "Vereniging werd geïdentificeerd als niet-koepel." |
| → NietBepaald | `KoepelstatusWerdTerugGezetNaarNietBepaald` | "Koepelstatus werd teruggezet naar niet bepaald." |
| Privacy aanvaard | `KoepelPrivacyverklaringWerdGeaccepteerd` | "Privacyverklaring voor koepels werd geaccepteerd." |
| Privacy ingetrokken | `KoepelPrivacyverklaringWerdIngetrokken` | "Privacyverklaring voor koepels werd ingetrokken." |

- Elk event bevat enkel de `VCode`.
- Het intrekken van de privacyverklaring wordt ook expliciet als event opgeslagen bij een wissel naar `GeenKoepel`/`NietBepaald`.
- Eventnamen zonder trema (`Geidentificeerd`), omdat de typenaam in de event store en de JSON terechtkomt.

**Te doen (development)**
- [x] Domein: `Koepelstatus` + `Koepelgegevens` (`DecentraalBeheer/Vereniging/Koepels`) op `VerenigingState`, met de events en `Apply`. Methode `VerenigingOfAnyKind.WijzigKoepelgegevens`.
- [x] Command + handler `WijzigKoepelgegevens`, met de KBO- en privacy-validaties.
- [x] Controller (`PATCH /v1/verenigingen/{vCode}/koepelgegevens`), request, validator en Swagger-examples.
- [x] Beheer-detailprojectie + response-model: `koepelstatus` als string, geen `bool?` (AGENTS.md). Historiek, PowerBI-historiek en de publieke sequence verwerken de events ook.
- [x] Bestaande documents zonder event: standaard `NietBepaald`/`false` via de property-initializer. Er is geen migratie of rebuild nodig.
- [x] Tests: command handling, validatie, request-mapping, projecties (detail, historiek, PowerBI) en E2E (detail + historiek). De bestaande E2E-detailverwachtingen zijn aangevuld met de standaard-koepelgegevens.
- [x] Postman-scenario's (`postman/features/koepels`, 22 scenario's).
- [ ] **Later** (afgesproken op 2026-11-20): `Koepelstatus` gelijktrekken met de value-object-regels uit AGENTS.md, zoals `RelatieType`.
  - `Labels`-consts op het value object.
  - `Parse` gooit een `DomainException` met de toegelaten waarden. De `IsValid`-regel verdwijnt uit `WijzigKoepelgegevensRequestValidator`.
  - De hardcoded boodschappen van die validator gaan naar de resx.
  - De Postman-scenario's in `postman/features/koepels/scenarios.js` (`leegRequest`, `ongeldigeKoepelstatus`) mee aanpassen.

---

## 2. Relatieaanvragen en registratie

### Story 2 — Koepel dient relatieaanvraag in voor bestaande vereniging · M · 🟢

> Bijgewerkt op 2026-11-19 met de nieuwe storytekst.

**Al beslist**
- Kanalen: koepelloket, CRM/MAGDA, bevoegde VR-acties en de initiële Excel-upload (story 8). Ze gebruiken allemaal hetzelfde endpoint. De upload roept het per rij aan.
- De aanvragende koepel heeft `koepelstatus = Koepel` (dat vervangt `isKoepelvereniging: true`, 0.6) en is een KBO-vereniging (0.2).
- VR controleert de vCodes, het relatietype en de combinatieregels van story 12 (K1–K9) voordat er een aanvraag ontstaat.
- Er ontstaat enkel een aanvraag met status `Aangevraagd`: geen feitelijke relatie, geen subtypewijziging en geen beheerrecht.
- De termijn is 90 dagen vanaf `aanvraagDatum`.
- Max. 1 open aanvraag per koepel–verenigingpaar, **ongeacht wie ze indiende**: de koepel (story 2) of de vereniging (story 4) (0.4).
  - Hetzelfde type opnieuw: VR geeft de bestaande open aanvraag terug.
  - Een ander type: validatiefout.
- Bestaande relaties worden niet gedupliceerd en niet automatisch omgezet (bv. lid → sub).
- Afgesloten aanvragen worden niet heropend: een nieuwe aanvraag krijgt een nieuw id.
- Meerdere open aanvragen of conflicterende gegevens geven een fout, nooit een willekeurige keuze.
- Een aanvraag mag bestaan zonder actieve vertegenwoordiger bij de ontvanger.
- De koepel mag haar eigen aanvraag niet namens de vereniging bevestigen (story 3: `403`).
- Komt de feitelijke relatie intussen rechtstreeks tot stand, dan geeft bevestiging een validatiefout.
- Eigen aggregate `Relatieaanvraag` (0.4). Het id is een sequentie zoals de vCode (0.5).
- `aangevraagdDoor` komt uit `vr-namens`, niet uit de body.
- `vr-namens` moet gelijk zijn aan `koepelVCode` of `verenigingVCode`, anders `400` "De aanvrager moet een partij van de relatieaanvraag zijn." (2026-11-19: `400` in plaats van `403`, zoals de spec). Ontbreekt `vr-namens`: `400` "vr-namens is verplicht."
- VR triggert VLO volgens story 14.

**Beslist op 2026-11-19**
- **Story 4 wordt meegenomen.** Het is hetzelfde endpoint; wie de aanvraag indient, maakt niet uit. De aanvrager komt uit `vr-namens` en moet `koepelVCode` of `verenigingVCode` uit de request zijn, anders `400`.
- **`vr-namens` is altijd nodig**, ook bij VR-acties en de Excel-upload. Zonder header: `400`.
- **De privacyverklaring is niet nodig om aan te vragen.** Ze is enkel nodig voor het latere beheer.
- **Wat een aanvraag blokkeert:**
  - een open aanvraag voor hetzelfde paar;
  - een huidig lidmaatschap van de vereniging bij de koepel;
  - een SUB-koppeling van de vereniging met de koepel.

  Een paar kan nooit tegelijk lid en sub zijn.
- **Hetzelfde type opnieuw geeft `400`**, geen `200`: een POST geeft nooit `200`. De boodschap noemt de open aanvraag.
- **`aanvraagDatum` en `vervalDatum` zijn datums** (`DateOnly`). `vervalDatum` = `aanvraagDatum` + 90 dagen.
- **Deze story is enkel het indienen**: een nieuw aggregate met de aanvraag en de controles in de handler. Bevestigen is een andere story met een ander endpoint, die de controles wellicht opnieuw doet. Er is nog geen VLO-trigger.
- **Prefix `RA`.**
- **Geen historiek.** De aanvraag verandert niets aan de verenigingen en leeft apart. Ze moet wel opvraagbaar zijn per vCode, als koepel of als vereniging, met een statusfilter. Daarvoor komt een eigen projectie.
- **Aanvraag-events horen niet in `For_Any_Event`**: het is een ander aggregate.

**Zelf ingevuld** (tenzij je iets anders wil)
- `beschrijving`: zoals bij lidmaatschap. Geen maximale lengte, wel geen HTML.
- `relatieType`: `Lidmaatschap` / `Subvereniging`, hoofdletterongevoelig zoals `koepelstatus`.
- Twee gelijktijdige aanvragen voor hetzelfde paar: de database laat er maar één toe (zie hieronder). De tweede krijgt een `400`.

**Technisch: kan starten**
- Bestaat al:
  - `EventStore` werkt met string-ids en heeft `Load<T>`, `SaveNew` en `SaveTransactional` over meerdere streams. Een eigen `Relatieaanvraag`-stream kan daarop draaien. Enkel `IAggregateSession` is vereniging-specifiek; er komt een kleine repository voor de aanvraag.
  - De Marten-sequence voor vCodes (`VCodeSequence`) dient als voorbeeld voor `RA`.
  - Koepelstatus, lidmaatschappen en subtype zitten op `VerenigingState`. De combinatieregels werken op de twee geladen verenigingen, zoals dubbelbeheer al twee verenigingen laadt.
  - Wolverine heeft een durable outbox (SQS) voor de latere VLO-trigger.
- Nieuw nodig:
  - **`vr-namens`** lezen, zoals `InitiatorHeaderMiddleware` dat doet voor `VR-Initiator`, en in `CommandMetadata` zetten (story 15). Voor dit endpoint: `400` als de header ontbreekt en `403` als hij niet de koepel of de vereniging is. De generieke `403`-check op alle endpoints (story 18) is niet nodig om te starten.
  - **Uniciteitsguard.** Alle projecties zijn vandaag async, dus "max. 1 open per paar" kan niet betrouwbaar via een projectie.
    - Voorstel: per paar, en per vereniging voor een open SUB-aanvraag, een klein lock-document met die sleutel als id, in dezelfde transactie als het event. Een tweede gelijktijdige aanvraag faalt dan in de database.
    - De handler probeert dan opnieuw, zoals `DubbelbeheerRetryPolicy`, en ziet de bestaande aanvraag.
    - Bij het afsluiten (stories 3, 13 en 23) verdwijnt het lock-document.
  - **Sanity-test:** `For_Any_Event` eist dat elk event in de vereniging-projecties verwerkt wordt. Aanvraag-events horen daar niet, want ze zitten in een eigen stream. Ze moeten uitgesloten worden, bv. via een eigen marker-interface.
  - Na de nieuwe handler: `./regen-marten.sh`, voor de statische Wolverine-code van de E2E-hosts.

**Voorstel request**
```http
POST /v1/relatieaanvragen
VR-Initiator: OVO000000
vr-namens: V0000001
Content-Type: application/json

{
  "verenigingVCode": "V0000002",
  "koepelVCode": "V0000001",
  "relatieType": "Lidmaatschap",
  "beschrijving": "Aanvraag lidmaatschap bij de koepel"
}
```
`beschrijving` is optioneel.

**Voorstel response (nieuw)**
```http
202 Accepted
Location: /v1/relatieaanvragen/RA0000001
ETag: W/"1"
VR-Sequence: 4800
```

**Voorstel response (zelfde type staat al open)**
```http
200 OK
Location: /v1/relatieaanvragen/RA0000001
```
```json
{
  "aanvraagId": "RA0000001",
  "verenigingVCode": "V0000002",
  "koepelVCode": "V0000001",
  "aangevraagdDoor": "V0000001",
  "relatieType": "Lidmaatschap",
  "status": "Aangevraagd",
  "aanvraagDatum": "2026-11-18T10:15:00+01:00",
  "afhandelDatum": null,
  "vervalDatum": "2027-02-16T10:15:00+01:00",
  "beschrijving": "Aanvraag lidmaatschap bij de koepel"
}
```

**Voorbeeld fouten**
| Situatie | Response |
|---|---|
| `vr-namens` ontbreekt | `400` "vr-namens is verplicht." |
| `vr-namens` is geen `koepelVCode` of `verenigingVCode` | `403` |
| `verenigingVCode` = `koepelVCode` | `400` |
| Koepel is geen KBO-vereniging | `400` "De beoogde koepel moet een KBO-vereniging zijn." |
| Koepel heeft status ≠ `Koepel` | `400` "De beoogde koepel moet als koepel geïdentificeerd zijn." (ongeacht wie indient, 2026-11-20) |
| Ander type staat al open voor dit paar | `400` "Er staat al een relatieaanvraag van een ander type open." |
| Actieve feitelijke relatie bestaat al | `400` "Er bestaat al een relatie tussen deze verenigingen." |
| Combinatieregel story 12 geschonden | `400` met specifieke boodschap |
| Onbekende vCode | `400` / `404` (zoals bij lidmaatschap toevoegen) |

**Event:** `RelatieaanvraagWerdIngediend` (eigen stream per `aanvraagId`, geen historiek).

**Geïmplementeerd (2026-11-19), samen met story 4**
- `POST /v1/relatieaanvragen` met `vr-namens`: `202` met `Location: /v1/relatieaanvragen/RA0000001`, `ETag` en `VR-Sequence`.
- `GET /v1/relatieaanvragen/{aanvraagId}` (doel van de `Location`) en `GET /v1/verenigingen/{vCode}/relatieaanvragen?status=…` (als koepel of als vereniging, `status` hoofdletterongevoelig).
- Volgorde van de controles, in de volgorde van de spec, allemaal `400`:
  1. `vr-namens` ontbreekt of is geen partij.
  2. Dezelfde vereniging als koepel en als vereniging.
  3. De koepel is geen KBO-vereniging: "De beoogde koepel moet een KBO-vereniging zijn."
  4. Koepelhoedanigheid: de koepel moet `Koepel` zijn: "De beoogde koepel moet als koepel geïdentificeerd zijn." Sinds 2026-11-20 is dat dezelfde boodschap, ongeacht wie indient (zie A2).
  5. Actieve feitelijke relatie: een lidmaatschap bij de koepel dat vandaag loopt, of een SUB-koppeling met de koepel. Geldt voor elk type.
  6. Lidmaatschapsaanvraag: een actueel of toekomstig lidmaatschap bij dezelfde koepel (einddatum niet voorbij).
  7. Er staat al een aanvraag open voor hetzelfde paar: aparte boodschap voor hetzelfde type (met het id) en voor een ander type.
  8. Subverenigingsaanvraag:
     - enkel een vereniging zonder rechtspersoonlijkheid met subtype leeg of `NB`;
     - geen lidmaatschap waarvan de einddatum niet voorbij is (K6);
     - geen open lidmaatschapsaanvraag bij een andere koepel (K6, 2026-11-20);
     - geen open SUB-aanvraag bij een andere koepel.
  9. Lidmaatschapsaanvraag:
     - een sub kan geen lid worden (K6);
     - geen open SUB-aanvraag bij een andere koepel (K6, 2026-11-20).
- Waar de controles zitten (2026-11-20):
  - 1 en 2 kan de request alleen al beantwoorden. Ze zitten in het value object `Partijen` (in `ToCommand`), dus er wordt niets geladen.
  - 3 en 4 zitten in `BeoogdeKoepel.Create`.
  - 5 tot 9 zitten in `Lidmaatschapsaanvraag.Create` en `Subverenigingsaanvraag.Create`.
- Een `RA`-nummer wordt pas na de controles opgevraagd: een geweigerde aanvraag verbruikt geen nummer.
- Uniciteit:
  - In dezelfde transactie als het event wordt per paar (en per vereniging voor SUB) een `OpenRelatieaanvraagDocument` bewaard. Een gelijktijdige tweede aanvraag faalt in de database en krijgt `400`.
  - **Bevestigen, weigeren, intrekken en verval (stories 3, 5, 23, 13) moeten dat document weer verwijderen.**
- Migratie `migrations/production/admin.api/scripts/up/00009_add_relatieaanvragen.sql`: sequence, lock-tabel en de tabel van de projectie, met indexen op `KoepelVCode` en `VerenigingVCode`.

**Bevestigd (2026-11-19)**
- [x] Ook als de **vereniging** indient (story 4), moet de koepel al `Koepel` zijn.
- [x] Een lidmaatschap blokkeert zolang de **einddatum** niet voorbij is: zonder einddatum, met een einddatum vanaf vandaag, en ook als het pas in de toekomst start. Bij bevestiging kan er dus geen overlap ontstaan.
- [x] De GET-endpoints vragen (nog) geen `vr-namens` (story 10).

**Beslist op 2026-11-20** (uitwerking in `relatieaanvragen-onduidelijkheden.md`)
- [x] Het request blijft zoals het is. De gegevens van de relatie (`van`, `tot`, `identificatie`, `beschrijving`) zitten niet in de aanvraag.
- [x] Een actueel of toekomstig lidmaatschap bij dezelfde koepel blijft een lidmaatschapsaanvraag blokkeren.
- [x] K6 geldt ook tussen open aanvragen: een open SUB-aanvraag blokkeert een lidmaatschapsaanvraag bij een andere koepel. Geïmplementeerd.
- [x] Omgekeerd blokkeert een open lidmaatschapsaanvraag een SUB-aanvraag. Geïmplementeerd.
- [x] Meerdere open lidmaatschapsaanvragen bij verschillende koepels blijven toegelaten.
- [x] De foutboodschap noemt het id van de open aanvraag, ook bij een andere koepel: je hebt het nodig om de aanvraag op te vragen. Vorm: "(relatieaanvraag: 'RA0000001')", of "(relatieaanvragen: …)" bij meerdere.
- [x] Twee gelijktijdige aanvragen van een ander type kunnen er nog door. Aanvaard: bevestigen controleert de regels opnieuw.
- [x] **Value objects valideren de request** (zie AGENTS.md). Een ongeldige `koepelVCode`/`verenigingVCode` geeft de gewone VCode-fout uit `VCode.Create`, zoals bij de andere endpoints. Er is geen eigen boodschap "moeten geldige vCodes zijn" meer. `relatieType` en de `status`-filter geven een fout van hun value object, met de toegelaten waarden uit dat value object: "RelatieType moet Lidmaatschap of Subvereniging zijn."

**Vergelijking met de uitgewerkte story "Registreer een relatieaanvraag" (2026-11-19)**

Grotendeels gelijk: hetzelfde endpoint, dezelfde velden, dezelfde payload van het event, 90 dagen en `vr-namens` als aanvrager. Hieronder de verschillen.

*Spec botst met een eerdere beslissing (beslist op 2026-11-19)*

| # | Onderwerp | Spec | Beslist en geïmplementeerd |
|---|---|---|---|
| A1 | Zelfde open aanvraag opnieuw | Scope en documentatie: "eenzelfde open aanvraag teruggeven zonder duplicatie" (niet in de validatietabel) | `400` met het id van de open aanvraag ("POST geeft nooit `200`"). De spec mag die twee zinnen schrappen. |
| A2 | Koepelhoedanigheid | Koepel dient in: `Koepel`. Vereniging dient in: `Koepel` of `NietBepaald`, niet `GeenKoepel`. | De koepel moet altijd `Koepel` zijn. De regeltekst in de spec laat `NietBepaald` nog toe. Sinds 2026-11-20 is het één boodschap, ongeacht wie indient en of de status `GeenKoepel` of `NietBepaald` is: "De beoogde koepel moet als koepel geïdentificeerd zijn." De KBO-check heeft een eigen boodschap: "De beoogde koepel moet een KBO-vereniging zijn." |
| A3 | `VR-Namens` is geen partij | `400` "De aanvrager moet een partij van de relatieaanvraag zijn." | `400` met die boodschap (niet meer `403`) |
| A4 | Historiek en detail | "Beheer historiek relatieaanvraag" met `namens`; detail met `afhandelDatum` | Niet: dat loopt via de GET-endpoints van de aanvragen |
| A5 | `aanvraagId` | `AV000001` (6 cijfers; elders ook `AV-001`) | `RA0000001` (prefix `RA` bevestigd), nog niet opnieuw bekeken |

*Gelijkgetrokken met de spec (2026-11-19)*
- Foutboodschappen uit de spec voor de request (verplichte velden, relatieType) en voor de regels:
  - geen partij;
  - actieve feitelijke relatie;
  - actueel of toekomstig lidmaatschap;
  - ander open type;
  - SUB-toelating.
- Aparte KBO-check op de koepel, vóór de koepelhoedanigheid. De koepelhoedanigheid heeft sinds 2026-11-20 één boodschap, ongeacht wie indient (zie A2).
- Actieve feitelijke relatie (lidmaatschap dat vandaag loopt, of een SUB-koppeling) blokkeert elk type. Een toekomstig lidmaatschap bij dezelfde koepel blokkeert een lidmaatschapsaanvraag met een eigen boodschap.
- SUB-toelating: enkel een vereniging zonder rechtspersoonlijkheid met subtype leeg of `NB`. `FV`, `SUB` en KBO krijgen de boodschap uit de spec.

*Nog open*

| # | Onderwerp | Spec | Nu |
|---|---|---|---|
| B1 | Eventnaam en titel | `RelatieaanvraagWerdGeregistreerd`, "Registreer een relatieaanvraag" | `RelatieaanvraagWerdIngediend`, "Dien een relatieaanvraag in". Nog niet uitgerold, dus hernoemen is gratis. |
| B2 | Waarden | `lidmaatschap`, `subvereniging`, `aangevraagd` (kleine letters, "exact") | `Lidmaatschap`, `Subvereniging`, `Aangevraagd`, zoals `koepelstatus` (`Koepel`); invoer is hoofdletterongevoelig |
| B8 | Header | `VR-Namens` | `vr-namens`. HTTP-headers zijn hoofdletterongevoelig, dus enkel de schrijfwijze in Swagger en de foutboodschap verschilt. |
| B9 | `412` | In de responses van de POST | Kan bij een nieuwe aanvraag niet (geen `If-Match`); wel bij de GET's met `expectedSequence` |
| B10 | "Beschrijving moet een string zijn." | Eigen boodschap | Een niet-string in de JSON geeft de algemene fout van de API bij een ongeldige body |

*Extra in de implementatie, niet in de spec (beslist in 0.2b/0.4)*
- Max. 1 open SUB-aanvraag per vereniging, over alle koepels heen.
- K6: een sub kan geen lid worden, en een vereniging met een lidmaatschap kan geen sub worden. Dat laatste valt deels samen met B6.
- K6 ook tussen open aanvragen bij verschillende koepels (2026-11-20).
- Koepel en vereniging mogen niet dezelfde zijn.
- `beschrijving` mag geen HTML bevatten.

---

### Story 3 — Vereniging bevestigt of weigert ontvangen aanvraag · M · 🟢

> **Bevestigen geïmplementeerd (2026-11-23)**, voor beide ontvangers (ook story 5): `POST /v1/relatieaanvragen/{aanvraagId}/bevestiging`. Weigeren volgt apart.
> - `vr-namens` is niet de ontvanger (de aanvrager zelf of een derde) → `400` "Enkel de ontvanger van de relatieaanvraag kan ze bevestigen." Zoals bij indienen, niet `403`.
> - Optionele body `{ identificatie, beschrijving }`: die van het lidmaatschap of de subvereniging.
> - De aanvraag moet `Aangevraagd` zijn en de vervaldatum niet voorbij; op de vervaldatum zelf kan het nog. Al bevestigd → "De relatieaanvraag werd al bevestigd."; na de vervaldatum → "De relatieaanvraag is vervallen."
> - VR controleert opnieuw de koepel (KBO, `Koepel`) en dezelfde regels als bij indienen, met de andere open aanvragen: de aanvraag zelf telt niet mee.
> - Bevestiging, relatie en subtypewijziging in één transactie, samen met het vrijgeven van de lock-documenten van de aanvraag.
> - Lidmaatschap: `van` = bevestigingsdatum, geen `tot`. Subtype **leeg of NB → FV** (0.9).
> - De aanvraag toont `bevestiging: { datum }`.
> - Response: `202`, `Location` en `ETag` van de aanvraag, `VR-Sequence` = de hoogste van aanvraag en vereniging.
> - Nog niet: de notificatie via VLO (story 14).

**Al beslist**
- `vr-namens` moet de tegenpartij van `aangevraagdDoor` zijn, anders `403` (0.1).
- De actieve vertegenwoordiger wordt bij het inloggen gecontroleerd (0.1).
- Leeg of NB + bevestigd lidmaatschap → FV (0.9).

**Ontbreekt nog**
- [ ] Gedrag bij **gelijktijdig** bevestigen en intrekken: het eerste wint, het tweede krijgt `400`/`412`?
- [ ] Wordt bij bevestiging `identificatie`/`beschrijving` van het nieuwe lidmaatschap gevuld, bv. met de aanvraagbeschrijving? Of blijven die leeg?
- [ ] Optionele reden bij weigeren?

**Aandachtspunten**
- Het nieuwe lidmaatschap krijgt `van` = de bevestigingsdatum, **niet leeg**. Een lege `van` betekent "oneindig in het verleden" en overlapt met een vroeger, beëindigd lidmaatschap bij dezelfde koepel (`LidmaatschapIsOverlappend`).
- De regels opnieuw controleren op de bestaande relaties. Twee gelijktijdige aanvragen van een ander type, of de rechtstreekse endpoints (`POST /lidmaatschappen`, `PATCH /subtype`), kunnen K6 nog breken (zie `relatieaanvragen-onduidelijkheden.md`, punt 3).

**Voorstel request**
```http
POST /v1/relatieaanvragen/RA0000001/bevestiging
VR-Initiator: OVO000000
vr-namens: V0000002
```
```http
POST /v1/relatieaanvragen/RA0000001/weigering
VR-Initiator: OVO000000
vr-namens: V0000002
Content-Type: application/json

{ "reden": "Wij zijn geen lid van deze koepel." }
```

**Voorstel response**
```http
202 Accepted
ETag: W/"2"
VR-Sequence: 4810
Location: /v1/verenigingen/V0000002/lidmaatschappen/3
```
> `Location` alleen bij bevestiging: die verwijst naar het aangemaakte lidmaatschap.

**Fouten**
| Situatie | Response |
|---|---|
| `vr-namens` ontbreekt | `400` "vr-namens is verplicht." |
| Al bevestigd (herhaling) | `400` "De relatieaanvraag werd al bevestigd." Geen events, geen notificatie. |
| Vervallen / ingetrokken / geweigerd | `400` "De relatieaanvraag is afgesloten." |
| `vr-namens` is de aanvrager of geen betrokken partij | `403` |
| Herhaalde controle faalt (subtype, andere relatie, …) | `400` met specifieke boodschap |

**Voorstel events:** `RelatieaanvraagWerdBevestigd` + bestaande `LidmaatschapWerdToegevoegd` / `VerenigingssubtypeWerdVerfijndNaarSubvereniging` (+ subtype FV bij null/NB). `RelatieaanvraagWerdGeweigerd`.

---

### Story 4 — Vereniging vraagt zelf aan om lid/sub te worden · M · 🟢

> Geïmplementeerd samen met story 2 (2026-11-19): hetzelfde endpoint, met de vereniging in `vr-namens`. De beoogde koepel moet `Koepel` zijn; `NietBepaald` of `GeenKoepel` geeft `400`.

**Al beslist**
- Wordt de beoogde koepel intussen `GeenKoepel`, dan blijft de aanvraag open en vervalt ze na 90 dagen.
- Een vereniging mag meerdere open lidmaatschapsaanvragen hebben naar verschillende koepels, maar max. 1 open SUB-aanvraag (0.4).
- De beoogde koepel moet een KBO-vereniging zijn (0.2).
- Een koepel kan zelf lid worden van een of meerdere koepels (K1, 0.2b).
- Een vereniging die al sub is, kan geen lidmaatschap aanvragen. Een vereniging die al lid is, kan geen sub-aanvraag indienen (K6).
- Een aanvraag naar een subvereniging als "koepel" is onmogelijk, want een sub is altijd het laagste niveau (K3).

**Ontbreekt nog**
- [ ] Na een wijziging naar `GeenKoepel` kan de koepel niet meer bevestigen, want daarvoor is `Koepel` vereist. Is weigeren of verval dan de enige uitweg? Dat lijkt zo; graag bevestigen.

**Voorstel request:** hetzelfde endpoint als story 2, met een andere `vr-namens`:
```http
POST /v1/relatieaanvragen
VR-Initiator: OVO000000
vr-namens: V0000002

{
  "verenigingVCode": "V0000002",
  "koepelVCode": "V0000001",
  "relatieType": "Subvereniging",
  "beschrijving": "Wij zijn de afdeling Bree"
}
```

**Response:** hetzelfde als story 2, met dezelfde boodschappen:
- `400` "De beoogde koepel moet een KBO-vereniging zijn." als de beoogde koepel een VZER is;
- `400` "De beoogde koepel moet als koepel geïdentificeerd zijn." bij status `GeenKoepel` of `NietBepaald`.

---

### Story 5 — Beoogde koepel bevestigt of weigert · M · 🟢

**Ontbreekt nog**
- [ ] Zelfde open punten als story 3 (concurrency, reden bij weigeren).
- [ ] Bij bevestiging met status `NietBepaald`: geeft VR een fout met een duidelijke boodschap ("identificeer je eerst als koepel")? Voorstel: ja. Het loket roept eerst `PATCH /koepelgegevens` aan.

**Voorstel request/response:** hetzelfde als story 3, met `vr-namens: V0000001` (de koepel).
Extra fout: `400` "De vereniging moet als koepel geïdentificeerd zijn om te kunnen bevestigen." Weigeren is altijd toegestaan.

---

### Story 6 — Koepel registreert een nieuwe vereniging · S · 🔴

**Ontbreekt nog**
- [ ] Bestaande registratie-endpoints uitbreiden (`POST /v1/verenigingen/vzer`, `POST /v1/verenigingen` voor KBO), of een apart endpoint?
- [ ] **Dubbeldetectie**: de bestaande flow geeft `409` + `VR-BevestigingsToken`. Werkt dat zo ook via MAGDA/BFF?
- [ ] KBO-registratie met `Subvereniging` is niet toegestaan (eigen rechtspersoonlijkheid). Geeft dat een fout bij de request?
- [ ] Is registratie + aanvraag (bij `Lidmaatschap`) atomisch? Er zijn twee streams: de nieuwe vereniging en de aanvraag (0.4). Voorstel: één Marten-transactie.
- [ ] Wie is `bron` van de nieuwe vereniging: de koepel (vCode) of de initiator?
- [ ] "Uitnodiging vertegenwoordigers" is een notificatie (0.7). Moet VR iets extra bijhouden?

**Voorstel request**
```http
POST /v1/verenigingen/vzer
VR-Initiator: OVO000000
vr-namens: V0000001

{
  "naam": "Schuttersgilde Hobbitstee",
  "...": "bestaande registratievelden",
  "koepel": {
    "koepelVCode": "V0000001",
    "relatieType": "Subvereniging"
  }
}
```

**Voorstel response**
```http
202 Accepted
Location: /v1/verenigingen/V0000123
ETag: W/"1"
VR-Sequence: 4900
```
Bij `Lidmaatschap`: een extra header of linkveld naar de aangemaakte aanvraag, bv. `VR-Relatieaanvraag: /v1/relatieaanvragen/RA0000002`. Te beslissen.

---

### Story 7 — Nieuwe vereniging ziet welke aanvraag/koppeling bestaat · S · 🟢 (na story 10)

**Ontbreekt nog**
- [ ] Niets functioneels. Dit volgt uit story 10 + bestaande detail (`subverenigingVan`).

---

## 3. Beheer en opvolging

### Story 17 — Overzicht beheerbare leden/subverenigingen · M · 🟡

**Al beslist:** GI-voorwaarden = KBO-vereniging + status `Koepel` + `privacyVerklaringGeaccepteerd` (0.2, 0.3).

**Ontbreekt nog**
- [ ] "Actueel lidmaatschap": `van` ≤ vandaag ≤ `tot`? Telt een lidmaatschap met `van` in de toekomst?
- [ ] Volstaat de lijst `koepelgegevens.leden` / `subverenigingen` uit de detail (stories 1+25)? Of is een apart endpoint met paginering nodig voor grote koepels?
- [ ] Een lege lijst als de koepel niet aan de GI-voorwaarden voldoet, of een fout?

**Voorstel request (indien apart endpoint)**
```http
GET /v1/koepels/V0000001/beheerbare-verenigingen?offset=0&limit=50
VR-Initiator: OVO000000
vr-namens: V0000001
```

**Voorstel response**
```json
{
  "koepelVCode": "V0000001",
  "verenigingen": [
    { "vCode": "V0000002", "naam": "Hobbitstede", "relatieType": "Lidmaatschap", "lidmaatschapId": 3 },
    { "vCode": "V0000123", "naam": "Schuttersgilde Hobbitstee", "relatieType": "Subvereniging" }
  ],
  "metadata": { "pagination": { "totalCount": 2, "offset": 0, "limit": 50 } }
}
```

---

### Story 9 — Koepel beheert gegevens van leden/subverenigingen · M · 🟡

**Al beslist**
- `vr-namens` ≠ route-vCode betekent handelen als koepel (0.1).
- Calls zonder header behouden hun huidige gedrag (0.1).
- Is de header ingevuld maar hoort de vCode niet bij de vereniging (zelf of beheerde): `403` (0.1).
- GI = KBO-verenigingen (0.2).

**Ontbreekt nog**
- [ ] **Autorisatiemodel** (story 18). Vandaag heeft de Beheer-API geen beheerscope per vCode. Dat moet geverifieerd worden.
- [ ] Welke bestaande endpoints vallen onder de scope? De lijst noemt basisgegevens, contacten, locaties, vertegenwoordigers, activiteiten, werkingsgebieden, bankrekeningnummers en stopzetten. Wat met lidmaatschappen van het lid zelf, subtype en dubbelbeheer?
- [x] ~~Beheer via ketens~~: enkel rechtstreekse relaties, en enkel met geaccepteerde privacyverklaring. Bij `C → B → A` mag A B beheren en mag B C beheren, maar A mag C niet beheren (K10).

**Voorstel:** geen nieuwe endpoints. De bestaande endpoints krijgen een extra autorisatiestap wanneer `vr-namens` ≠ de `vCode` in de route:
```http
PATCH /v1/verenigingen/V0000002
VR-Initiator: OVO000000
vr-namens: V0000001

{ "korteNaam": "Hobbit" }
```
| Situatie | Response |
|---|---|
| Geldige relatie + GI-voorwaarden | `202 Accepted` (zoals vandaag) |
| Geen actuele relatie / GI-voorwaarden niet voldaan | `403 Forbidden` |

---

### Story 10 — Aanvragen en feitelijke relaties raadplegen · M · 🟡

> Deels geïmplementeerd met story 2 (2026-11-19): `GET /v1/verenigingen/{vCode}/relatieaanvragen?status=…` en `GET /v1/relatieaanvragen/{aanvraagId}`, met een eigen projectie (`RelatieaanvraagProjection`). Nog zonder `vr-namens`-check, en zonder `afhandelDatum` (er bestaat nog geen afhandeling).

**Al beslist:** de omgekeerde leesrichting (koepel → leden/subverenigingen) staat in `koepelgegevens` in de detail (stories 1+25).

**Ontbreekt nog**
- [ ] Een apart endpoint voor aanvragen, of ook opnemen in de detail? Voorstel: een apart endpoint, en de detail enkel voor feitelijke relaties.
- [ ] Filteren op status of richting?
- [ ] Wie mag de aanvragen van een vCode raadplegen? Voorstel: `vr-namens` moet een betrokken partij zijn.

**Voorstel request**
```http
GET /v1/verenigingen/V0000001/relatieaanvragen?status=Aangevraagd
VR-Initiator: OVO000000
vr-namens: V0000001
```

**Voorstel response**
```json
{
  "relatieaanvragen": [
    {
      "aanvraagId": "RA0000001",
      "verenigingVCode": "V0000002",
      "koepelVCode": "V0000001",
      "aangevraagdDoor": "V0000001",
      "relatieType": "Lidmaatschap",
      "status": "Aangevraagd",
      "aanvraagDatum": "2026-11-18T10:15:00+01:00",
      "afhandelDatum": null,
      "vervalDatum": "2027-02-16T10:15:00+01:00",
      "beschrijving": "Aanvraag lidmaatschap bij de koepel"
    }
  ]
}
```
En `GET /v1/relatieaanvragen/RA0000001` voor één aanvraag, met dezelfde vorm.

---

### Story 22 — Gezamenlijk wijzigingenoverzicht · S · 🔴

**Ontbreekt nog**
- [ ] Vorm: een feed (zoals de bestaande historiek, per event) of een samenvatting per vereniging?
- [ ] Periode, paginering, sinds-parameter?
- [ ] Wat is de "actuele scope" bij wijzigingen uit het verleden? Bv. een lid sinds vorige maand: zie je de wijzigingen van daarvoor ook?
- [ ] Kan de bestaande historiekprojectie hergebruikt worden (`WebApi/.../Historiek`)?

**Voorstel request**
```http
GET /v1/koepels/V0000001/wijzigingen?sinds=2026-11-01&offset=0&limit=100
VR-Initiator: OVO000000
vr-namens: V0000001
```

**Voorstel response**
```json
{
  "wijzigingen": [
    {
      "vCode": "V0000002",
      "naam": "Hobbitstede",
      "tijdstip": "2026-11-17T14:02:11+01:00",
      "gebeurtenis": "LocatieWerdToegevoegd",
      "beschrijving": "Locatie 'Zaal Groene Draak' werd toegevoegd.",
      "initiator": "OVO000000"
    }
  ]
}
```

---

## 4. Organisatorische structuur

### Story 16 — Clusters · C · 🔴

**Ontbreekt nog** (al gemarkeerd als open in de story)
- [ ] Mag een vereniging in meerdere clusters van dezelfde koepel zitten?
- [ ] Mag een cluster meerdere bovenliggende clusters hebben? Hoe voorkomen we cycli?
- [ ] Wat gebeurt er met clusterdeelname als de feitelijke relatie wegvalt: blijft ze, of wordt ze verborgen?
- [ ] Wat gebeurt er bij het verwijderen van een cluster met kinderclusters?
- [ ] Woont dit in VR of in het mediatorplatform? Er zijn geen beheerrechten aan gekoppeld, dus het kan ook bij VLO.

**Voorstel request/response:** pas uitwerken na deze beslissingen. Een indicatieve vorm:
```http
POST /v1/koepels/V0000001/clusters
{ "naam": "Regio Noord", "coordinator": "V0000123", "bovenliggendeCluster": null }
```
```http
202 Accepted
Location: /v1/koepels/V0000001/clusters/1
```

---

## 5. Intrekken, beëindigen en corrigeren

### Story N1 (nieuw) — Relaties verwijderen als een koepel geen koepel meer is · ? · 🔴

> Nieuwe story, voortgekomen uit stories 1+25. Nog geen nummer of MoSCoW.

Als een vereniging van `Koepel` naar `GeenKoepel` of `NietBepaald` gaat, moeten de feitelijke relaties die naar haar verwijzen verwijderd worden. Daarna zijn `leden` en `subverenigingen` in haar koepelgegevens leeg.

**Ontbreekt nog**
- [ ] **Verwijderen of stopzetten?**
  - Worden lidmaatschappen echt verwijderd (`LidmaatschapWerdVerwijderd`)?
  - Of stopgezet met `tot` = vandaag (story 11)? Bij stopzetten blijft de historiek zichtbaar.
- [ ] **Subtype van de subs:** SUB → NB, zoals in 0.9?
- [ ] **Synchroon of asynchroon?** Er moeten mogelijk tientallen verenigingsstreams aangepast worden.
  - Voorstel: de statuswissel publiceert een bericht.
  - Per kind volgt dan een command via de outbox (Wolverine).
- [ ] **Open aanvragen** naar deze koepel: die blijven open en vervallen (beslist bij story 1). Of worden ze ook automatisch afgesloten?
- [ ] **De koepel zelf als lid:** als B geen koepel meer is, blijft haar eigen lidmaatschap bij A wel bestaan? Voorstel: ja, want dat is een relatie naar boven.
- [ ] **Notificaties** naar de betrokken leden en subs (0.7)?
- [ ] **Beheerrechten** vervallen al bij de statuswissel, nog vóór de verwijdering, want de privacyverklaring wordt `false`. Graag bevestigen.

**Geen eigen endpoint:** dit is een gevolg van `PATCH /koepelgegevens`.

Voorstel events:
- bestaande `LidmaatschapWerdVerwijderd` (of `LidmaatschapWerdGewijzigd`);
- de subtype-events per kind;
- metadata met een verwijzing naar de statuswissel als oorzaak.

---

### Story 23 — Eigen open aanvraag intrekken · M · 🟢

**Al beslist**
- `vr-namens` moet `aangevraagdDoor` zijn, anders `403` (0.1).
- Concurrency met bevestiging: optimistic concurrency op de aanvraagstream (eigen aggregate, 0.4).

**Voorstel request**
```http
POST /v1/relatieaanvragen/RA0000001/intrekking
VR-Initiator: OVO000000
vr-namens: V0000001
```

**Voorstel response:** `202 Accepted`. Fouten:
- `400` als de aanvraag niet meer `Aangevraagd` is;
- `400` "vr-namens is verplicht." als de header ontbreekt;
- `403` als `vr-namens` niet de aanvrager is.

**Voorstel event:** `RelatieaanvraagWerdIngetrokken`.

---

### Story 11 — Feitelijk lidmaatschap beheren of stopzetten · S · 🟡

**Ontbreekt nog**
- [ ] Is "stopzetten" een **nieuwe actie** (`tot` = vandaag), of gewoon de bestaande `PATCH` met `tot`? Voorstel: een nieuwe expliciete actie, zodat de werkafspraak "tot = datum van de actie" door VR wordt afgedwongen.
- [ ] Autorisatie voor de koepel op de stream van het lid (story 18).
- [ ] Notificatie bij stopzetting (0.7).

**Voorstel request (nieuw)**
```http
POST /v1/verenigingen/V0000002/lidmaatschappen/3/stopzetting
VR-Initiator: OVO000000
vr-namens: V0000001
```

**Voorstel response:** `202 Accepted`. Gevolg: `LidmaatschapWerdGewijzigd` met `tot` = vandaag. De bestaande `PATCH /v1/verenigingen/{vCode}/lidmaatschappen/{id}` blijft ongewijzigd.

---

### Story 13 — Automatisch verval na 90 dagen · S · 🟢

**Ontbreekt nog**
- [ ] Exact moment: vervalDatum op dagbasis (einde van de dag, Brusselse tijd) of op tijdstip?
- [ ] Draait dit als job in `AssociationRegistry.Scheduled.Host`? Welke frequentie (dagelijks)?
- [ ] Bevestigen na de vervaldatum, maar vóór de job draaide: VR moet dit ook bij bevestiging zelf controleren (de story vraagt dit al).

**Geen publiek endpoint.** Voorstel event: `RelatieaanvraagWerdVervallen`.

---

### Story 24 — Koepel beheert en beëindigt SUB-relatie · M · 🟡

**Al beslist (0.9)**
- De **koepel** kan de SUB-relatie beheren en beëindigen. Het is geen exclusieve VR-actie.
- Subtype na beëindiging: **NB**.
- De **subvereniging zelf** kan de relatie niet beëindigen. `PATCH /subtype` met `vr-namens` = de sub → `403`.

**Ontbreekt nog**
- [ ] Wat betekent "beheren" van een SUB-relatie: enkel `identificatie`/`beschrijving` wijzigen (bestaat al), of ook naar een andere koepel verhangen?
- [ ] Een nieuw endpoint, of het bestaande `PATCH /subtype` hergebruiken met `vr-namens` = de koepel?

**Voorstel request**
```http
DELETE /v1/verenigingen/V0000123/subverenigingVan
VR-Initiator: OVO000000
vr-namens: V0000001
```

**Voorstel response:** `202 Accepted`. Gevolg: subtype → NB + een event dat de koppeling verwijdert. `403` als `vr-namens` niet de koepel van deze sub is.

---

### Story 21 — Interne correctie van een aanvraag · ? · 🔴

**Ontbreekt nog**
- [ ] De story vraagt zelf "Is dit nodig?" Eerst beslissen of dit in de POC/MVP zit.
- [ ] Welke velden en statussen zijn corrigeerbaar?
- [ ] Wat gebeurt er met een al aangemaakte feitelijke relatie?

**Voorstel:** parkeren tot er een beslissing is. Indicatief: `PATCH /v1/admin/relatieaanvragen/{aanvraagId}` (intern), met een eigen event `RelatieaanvraagWerdGecorrigeerd`.

---

## 6. Initiële upload via Excel

### Story 8 — Initiële Excel-upload · M · 🔴

**Ontbreekt nog**
- [ ] Gebeurt dit via de bestaande migratietool? Dat is een aparte applicatie. In welke repo staat ze, en wie past ze aan?
- [ ] Import en export via koepelloket of via VR?
- [x] ~~Hoe komt de koepel uit de uploadcontext?~~ Via `vr-namens`, door de BFF meegestuurd (0.1).
- [ ] Rapportering per rij: hoe krijgt de koepel de resultaten per rij te zien (aangemaakt, al bestaand, fout)?
- [ ] Is een rij atomisch? Is de hele upload atomisch, of per rij?
- [ ] Hangt af van stories 2, 6 en 12.

**Voorstel:** VR voorziet geen apart Excel-endpoint. De migratietool roept per rij de endpoints van story 2 en 6 aan. Dan gelden dezelfde regels automatisch.

---

## 7. Gedeelde regels

### Story 12 — Onverenigbare relaties blokkeren · M · 🟢

**Al beslist (0.2, 0.9)**
- Leeg of NB + bevestigd lidmaatschap → FV.
- Subtype na SUB-beëindiging → NB.
- De koepel is altijd een KBO-vereniging; kinderen zijn KBO of VZER.
- Ketenregels K1–K8 (0.2b):
  - meerdere lidmaatschappen, ook op verschillende niveaus van dezelfde keten;
  - een koepel kan meerdere subs hebben;
  - een sub is altijd het laagste niveau;
  - geen sub én lid tegelijk;
  - max. één sub-relatie per vereniging.

**Ontbreekt nog**
- [ ] Moet de **bestaande** `POST /lidmaatschappen` (rechtstreeks door een GI) dezelfde combinatieregels krijgen? Bv. geen lidmaatschap bij een bestaande SUB-koppeling. Dat is te verifiëren in `VoegLidmaatschapToeCommandHandler`. Er bestaat al `VerenigingKanGeenLidWordenWaarvanZijAlReedsSubverenigingIs`, maar enkel voor dezelfde koepel.
- [ ] Zet de bestaande lidmaatschapsflow subtype null → FV vandaag al? Te verifiëren.
- [ ] Moet de bestaande lidmaatschapsflow ook afdwingen dat de "andere vereniging" (de koepel) een KBO-vereniging is?
- [x] Cycli van Lid-relaties tussen koepels: toegestaan (K9, gewijzigd op 2026-11-19).
- [ ] **TBD:** gelden K3, K6 en K9 ook op de **bestaande** endpoints `POST /lidmaatschappen` en `PATCH /subtype`?
- [ ] "Actueel lidmaatschap" eenduidig definiëren (zie story 17).

**Geen eigen endpoint:** dit zijn validaties binnen stories 2–6 en 24.

---

### Story 18 — Autorisatie op elke beheeractie · M · 🟢

**Al beslist (0.1)**
- De handelende vCode komt via `vr-namens`, door de BFF na het inloggen via MAGDA meegestuurd. VR vertrouwt die header.
- Zonder header: normale flow, behalve `400` "vr-namens is verplicht." bij relatieacties.
- Met header: een generieke check op alle beheerendpoints, en `403` bij geen bevoegdheid.

**Ontbreekt nog**
- [ ] Performance: de beheerscope berekenen bij elke request. Via een projectie "koepel → beheerbare vCodes"?

---

### Story 14 — Notificaties · C · 🟡

**Ontbreekt nog**
- [ ] Het VLO-endpointcontract: URL, auth, payload, idempotency-key.
- [ ] Retry- en foutbeleid. Wie garandeert dat er geen dubbele verzending is?

**Voorstel payload naar VLO (indicatief)**
```json
{
  "notificatieId": "c1d2...",
  "type": "RelatieaanvraagWerdIngediend",
  "aanvraagId": "RA0000001",
  "ontvanger": "V0000002",
  "betrokkenen": ["V0000001", "V0000002"],
  "tijdstip": "2026-11-18T10:15:00+01:00"
}
```
`notificatieId` dient als idempotency-key, zodat VLO dubbele verzending kan vermijden.

---

### Story 15 — Audit en historiek · S · 🟢

**Al beschikbaar:** elk event bevat `CommandMetadata` (`Initiator`, `Tijdstip`, `CorrelationId`).

**Ontbreekt nog**
- [ ] De handelende vCode (`vr-namens`) moet in `CommandMetadata` komen, zodat hij in elk event staat.
- [x] ~~Uitvoerende actor~~: de vCode volstaat (0.1).
- [ ] "Kanaal" kan deels uit `Initiator` afgeleid worden: is dat voldoende? Alles loopt via MAGDA.
- [ ] Moeten de nieuwe events in de bestaande **historiek**-endpoint verschijnen? Voorstel: ja.

---

## Samenvatting: wat nog beslissen

| # | Beslissing | Blokkeert stories |
|---|---|---|
| 1 | Story N1: relaties verwijderen bij Koepel → GeenKoepel/NietBepaald (open punten in N1) | N1 |
| 2 | Koepelgegevens bij Koepel → GeenKoepel; betekenis "velden van verwijzigde vereniging" | 1+25 |
| 3 | `leden` in de detail: enkel actuele lidmaatschappen? | 1+25, 10, 17 |
| 3b | Ketenregels (K3/K6/K9) ook op de bestaande endpoints? (TBD) | 12 |
| 4 | Prefix `aanvraagId` (`RA`?) (0.5) | 2, 3, 4, 5, 10, 23 (klein) |
| 5 | Story 21 nodig? | 21 |
| 6 | VLO-contract (0.7), niet blokkerend via outbox | 14 (+ triggers overal) |
| 7 | Clusters: in VR of in het mediatorplatform? | 16 |
| 8 | Registratie nieuwe vereniging: bestaande endpoints uitbreiden of nieuw? | 6, 8 |
| 9 | Story 2: `vr-namens` bij VR-acties en upload, privacyverklaring vereist, welke bestaande relatie blokkeert, response bij hetzelfde type | 2, 4, 8 |

**Kan nu starten**
- stories 1 + 25 (`PATCH /koepelgegevens` met `koepelstatus` + `privacyVerklaringGeaccepteerd`);
- `vr-namens`-middleware + `403`-check (story 18), als basis voor alles daarna;
- stories 2, 3, 4, 5 en 23 (aanvraagflow als eigen aggregate);
- story 12 (regels zijn nu beslist);
- story 13 (job + event);
- story 15 (meeliften);
- story 24 (koepel beëindigt SUB → NB; sub zelf `403`).
