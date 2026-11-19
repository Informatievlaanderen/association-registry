# Koepels — wat ontbreekt er nog om te starten

> Status: werkdocument · 2026-11-18 (bijgewerkt: header, GI/KBO, aggregate, koepelstatus, stories 1+25 samengevoegd)
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

**Nog open**
| # | Regel | Gevolg |
|---|---|---|
| K9 | **Geen cycli** in lidmaatschappen. | A lid van B + B lid van A: ❌. Ook langere cirkels zoals A → B → D → A: ❌. |
| K10 | **Beheer enkel via rechtstreekse relaties**, en enkel met geaccepteerde privacyverklaring. | Bij `C → B → A` mag A B beheren en mag B C beheren, maar A mag C niet beheren. In diagram 1 mag A E wel beheren, via de rechtstreekse relatie `E --Lid--> A`. |
| K11 | **`leden`/`subverenigingen` tonen enkel rechtstreekse relaties.** | `leden` van A toont B en E (diagram 1), niet C. |

**Ongeldig (cycli)**
```text
  A --Lid--> B  +  B --Lid--> A                  (K9)
  A --Lid--> B --Lid--> D --Lid--> A             (K9)
```
- [ ] Technisch: de cyclusdetectie loopt over meerdere aggregates. Voorstel: bij het aanvragen én bij het bevestigen de keten naar boven volgen via de projectie (BFS op `lidmaatschappen`).

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
| NB + bevestigd lidmaatschap | NB → **FV** |
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
- [ ] Postman-scenario.

---

## 2. Relatieaanvragen en registratie

### Story 2 — Koepel dient relatieaanvraag in voor bestaande vereniging · M · 🟢

**Al beslist**
- Komt de feitelijke relatie intussen rechtstreeks tot stand, dan geeft bevestiging een validatiefout.
- Eigen aggregate `Relatieaanvraag` (0.4). Het id is een sequentie zoals de vCode (0.5).
- `aangevraagdDoor` komt uit `vr-namens`, niet uit de body.
- `vr-namens` moet gelijk zijn aan `koepelVCode` of `verenigingVCode`, anders `403`.
- Ontbreekt `vr-namens`: `400` "vr-namens is verplicht."
- De koepel moet een KBO-vereniging zijn (0.2).

**Ontbreekt nog**
- [ ] Response bij "zelfde type staat al open": `200 OK` met de bestaande aanvraag, of `202` + `Location` naar de bestaande? Voorstel: `200` + body + `Location`.
- [ ] Wat geldt als **aanvraagDatum**: commandtijdstip of eventtijdstip? Wordt `vervalDatum` op dag- of tijdstipniveau berekend, en in welke tijdzone?
- [ ] Maximale lengte van `beschrijving`?
- [ ] Notificatietrigger (0.7).

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
| Koepel heeft status ≠ `Koepel` | `400` "De aanvragende vereniging is niet als koepel geïdentificeerd." |
| Koepel is geen KBO-vereniging | `400` |
| Ander type staat al open voor dit paar | `400` "Er staat al een relatieaanvraag van een ander type open." |
| Actieve feitelijke relatie bestaat al | `400` "Er bestaat al een relatie tussen deze verenigingen." |
| Combinatieregel story 12 geschonden | `400` met specifieke boodschap |
| Onbekende vCode | `400` / `404` (zoals bij lidmaatschap toevoegen) |

**Voorstel event:** `RelatieaanvraagWerdIngediend`.

---

### Story 3 — Vereniging bevestigt of weigert ontvangen aanvraag · M · 🟢

**Al beslist**
- `vr-namens` moet de tegenpartij van `aangevraagdDoor` zijn, anders `403` (0.1).
- De actieve vertegenwoordiger wordt bij het inloggen gecontroleerd (0.1).
- NB + bevestigd lidmaatschap → FV (0.9).

**Ontbreekt nog**
- [ ] Gedrag bij **gelijktijdig** bevestigen en intrekken: het eerste wint, het tweede krijgt `400`/`412`?
- [ ] Wordt bij bevestiging `identificatie`/`beschrijving` van het nieuwe lidmaatschap gevuld, bv. met de aanvraagbeschrijving? Of blijven die leeg?
- [ ] Optionele reden bij weigeren?

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

**Response:** hetzelfde als story 2. Extra fouten:
- `400` "De beoogde koepel is expliciet als niet-koepel geïdentificeerd." bij status `GeenKoepel`;
- `400` als de beoogde koepel een VZER is.

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
- NB + bevestigd lidmaatschap → FV.
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
- [x] ~~Cycli~~: geblokkeerd (K9).
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

**Kan nu starten**
- stories 1 + 25 (`PATCH /koepelgegevens` met `koepelstatus` + `privacyVerklaringGeaccepteerd`);
- `vr-namens`-middleware + `403`-check (story 18), als basis voor alles daarna;
- stories 2, 3, 4, 5 en 23 (aanvraagflow als eigen aggregate);
- story 12 (regels zijn nu beslist);
- story 13 (job + event);
- story 15 (meeliften);
- story 24 (koepel beëindigt SUB → NB; sub zelf `403`).
