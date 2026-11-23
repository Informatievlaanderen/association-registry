# Relatieaanvragen: onduidelijkheden en wat logischer lijkt

> Status: werkdocument · 2026-11-20 (bijgewerkt: punten 1, 2 en 5 beslist, punt 6 opgelost, Excel-upload buiten beschouwing)
> Basis: de code op `or-3800` (indienen, story 2 + 4) en `koepels-ontbrekende-punten.md`.
> Doel: oplijsten wat nog onduidelijk is, of wat anders logischer lijkt, vóór bevestigen/weigeren/intrekken/verval (stories 3, 5, 23, 13) gebouwd worden.

**Snel overzicht**

| # | Punt | Status |
|---|---|---|
| 1 | De aanvraag bevat de gegevens van de relatie niet | ✅ beslist: het request blijft zoals het is |
| 2 | Een actueel of toekomstig lidmaatschap blokkeert | ✅ beslist: blijft zo |
| 3 | Welke controles gebeuren opnieuw bij bevestiging? | 🟡 nodig voor story 3/5 |
| 4 | Aanvragen die niet meer kunnen slagen, blijven open | 🟡 |
| 5 | K6 tussen open aanvragen | ✅ beslist en geïmplementeerd |
| 6 | Wie mag bevestigen namens wie? | ✅ geen probleem |
| 7 | Nieuwe verenigingen (story 6): SUB rechtstreeks, lidmaatschap via een aanvraag | 🟡 |
| 8 | Bevestigen schrijft naar twee streams: welke `ETag` en `Location`? | 🟢 klein |
| 9 | `400` of `403` voor "verkeerde partij" | 🟢 klein |
| 10 | Kleinere punten | 🟢 |
| 11 | Analysedocument niet meer in lijn met de beslissingen | 🟢 opruimen |

---

## 1. De aanvraag bevat de gegevens van de relatie niet — ✅ beslist (2026-11-20)

**Beslist:** het request blijft zoals het is: `koepelVCode`, `verenigingVCode`, `relatieType` en een optionele `beschrijving` van de aanvraag. De gegevens van de relatie (`van`, `tot`, `identificatie`, `beschrijving`) zitten er niet in.

**Gevolgen voor bevestigen (story 3/5)**
- Het nieuwe lidmaatschap start op de bevestigingsdatum en heeft geen einddatum.
- `van` moet de bevestigingsdatum zijn, **niet leeg**. Een lege `van` betekent "oneindig in het verleden" (`Geldigheidsperiode.OverlapsWith`).
  - Voorbeeld: een vereniging was lid van de koepel van 2020 tot 2024. De aanvraag mag, want dat lidmaatschap is voorbij.
  - Een lidmaatschap zonder `van` zou overlappen met 2020–2024. Dan faalt de bevestiging met `LidmaatschapIsOverlappend`.
- Nog open (story 3): worden `identificatie` en `beschrijving` van de relatie gevuld, bv. met de aanvraagbeschrijving, of blijven ze leeg? Daarna kunnen ze aangepast worden via de bestaande endpoints.

---

## 2. Een actueel of toekomstig lidmaatschap blokkeert — ✅ beslist (2026-11-20)

**Beslist:** een lidmaatschap bij dezelfde koepel dat nu loopt of nog moet starten, blokkeert een lidmaatschapsaanvraag. Dat volgt uit punt 1: het nieuwe lidmaatschap loopt vanaf de bevestiging zonder einde, en overlapt dus met elk lidmaatschap dat nog niet voorbij is.

- Dit wijkt bewust af van `POST /lidmaatschappen`, dat 2026–2027 naast 2030–2031 toelaat.
- Het deel "actueel" wordt al door controle 5 opgevangen ("actieve feitelijke relatie"). `ActueelOfToekomstigLidmaatschapBijDezelfdeKoepel` gaat in de praktijk dus enkel af bij een toekomstig lidmaatschap. Geen actie nodig.

---

## 3. Welke controles gebeuren opnieuw bij bevestiging?

Tussen indienen en bevestigen kunnen tot 90 dagen zitten. Het analysedocument zegt dat bevestigen de controles "wellicht opnieuw" doet. Dat moet concreet worden.

| Wat kan er intussen veranderd zijn | Vraag |
|---|---|
| De koepel is `GeenKoepel` of `NietBepaald` geworden | Story 4: de koepel kan dan niet meer bevestigen. **Kan de vereniging een aanvraag van de koepel nog wel bevestigen?** Dat staat nergens. |
| De privacyverklaring is (niet) geaccepteerd | Is ze nodig om te bevestigen? Nu niet nodig om in te dienen. |
| De relatie kwam intussen rechtstreeks tot stand (`POST /lidmaatschappen`, `PATCH /subtype`) | Beslist: bevestiging geeft een fout. Zie punt 4 voor wat er dan met de aanvraag gebeurt. |
| De vereniging werd intussen sub, of kreeg een lidmaatschap elders (K6) | Fout bij bevestiging? Kan via de rechtstreekse endpoints, of via twee gelijktijdige aanvragen (punt 5). |
| Het subtype werd intussen `FV` (bij een SUB-aanvraag) | Fout bij bevestiging, zoals bij indienen? |
| Een van beide werd verwijderd of als dubbel gemarkeerd | Het laden faalt nu al (`AggregateSession.cs:98-102`). Welke boodschap krijgt de gebruiker? |
| Een van beide werd **gestopt** | Wordt nergens gecontroleerd, ook niet bij indienen. Mag een gestopte vereniging een relatie aanvragen of krijgen? |
| De vervaldatum is voorbij, maar de verval-job liep nog niet | Fout bij bevestiging (story 13 vraagt dit al). Is de vervaldatum zelf nog geldig? |

**Voorstel:** bij bevestiging dezelfde controles als bij indienen, maar enkel op de bestaande relaties en niet op de open aanvragen. Daarbij komen de vervaldatum en de status van de aanvraag.

---

## 4. Aanvragen die niet meer kunnen slagen, blijven open

**Nu beslist**
- Wordt de koepel `GeenKoepel`, dan blijft de aanvraag open en vervalt ze na 90 dagen (story 4).
- Komt de relatie rechtstreeks tot stand, dan geeft bevestiging een fout (story 2), en de aanvraag blijft open.

**Wat vreemd is**
- Zo'n aanvraag kan niemand nog bevestigen, maar ze blokkeert wel het paar: het lock-document (`OpenRelatieaanvraagDocument`) blijft tot 90 dagen staan.
- Met punt 5 weegt dat zwaarder: een open SUB-aanvraag die niet meer kan slagen, blokkeert ook elke lidmaatschapsaanvraag van de vereniging, en omgekeerd.
- De verenigingen zien dan een open aanvraag die nergens meer naartoe kan.
- N1 vraagt al of open aanvragen bij `Koepel → GeenKoepel` automatisch afgesloten worden. Dat staat in tegenspraak met wat bij story 4 "al beslist" is.

**Te beslissen**
- [ ] Automatisch afsluiten (bv. status `Vervallen`, of een nieuwe status zoals `Geannuleerd`) via een eventhandler, of laten vervallen?
- [ ] Bij dubbel: verhuizen de open aanvragen van de dubbele vereniging naar de authentieke vereniging, of worden ze afgesloten?

---

## 5. K6 tussen open aanvragen — ✅ beslist en geïmplementeerd (2026-11-20)

**Beslist:** een vereniging kan niet tegelijk sub en lid zijn (K6). Ze kan dus ook niet tegelijk een open SUB-aanvraag en een open lidmaatschapsaanvraag hebben, bij welke koepels ook.

**Voorbeeld:** C vraagt aan om sub te worden van A (RA0000001), en daarna om lid te worden van B. Die tweede aanvraag wordt geweigerd: "Een vereniging met een open subverenigingsaanvraag kan geen lid worden van een koepel (relatieaanvraag: 'RA0000001')."

**Geïmplementeerd** (beide richtingen bevestigd op 2026-11-20)

| Nieuwe aanvraag | Wordt geweigerd bij | Boodschap |
|---|---|---|
| Lidmaatschap | een open SUB-aanvraag, bij welke koepel ook | "Een vereniging met een open subverenigingsaanvraag kan geen lid worden van een koepel (relatieaanvraag: 'RA0000001')." |
| Subvereniging | een open lidmaatschapsaanvraag, bij welke koepel ook | "Een vereniging met een open lidmaatschapsaanvraag kan geen subvereniging worden (relatieaanvraag: 'RA0000002')." |
| Subvereniging | meerdere open lidmaatschapsaanvragen | "Een vereniging met open lidmaatschapsaanvragen kan geen subvereniging worden (relatieaanvragen: 'RA0000002', 'RA0000003')." |

- Bij dezelfde koepel gold dit al via "1 open aanvraag per paar": "Er bestaat al een open aanvraag van een ander relatietype voor dit paar."
- Meerdere open lidmaatschapsaanvragen bij verschillende koepels blijven toegelaten (0.4). Bevestigd op 2026-11-20.
- **Id in de boodschap** (beslist op 2026-11-20): de boodschap noemt het id van de open aanvraag, ook als die bij een andere koepel hoort. Je hebt het nodig om de aanvraag op te vragen ter controle. Bij een SUB-aanvraag noemt ze elke open lidmaatschapsaanvraag. De K7-boodschap ("Er staat al een aanvraag open om subvereniging te worden: 'RA…'") doet dat al.
- **Gelijktijdige aanvragen** (aanvaard op 2026-11-20): de open SUB-aanvraag wordt via haar lock-document opgehaald, de open lidmaatschapsaanvragen via een query (er kan er een per koepel zijn). Twee aanvragen van een ander type op exact hetzelfde moment kunnen er dus allebei door. Bevestigen controleert de regels opnieuw (punt 3) en vangt dat op.

---

## 6. Wie mag bevestigen namens wie? — ✅ geen probleem

`vr-namens` is de vCode waarop de gebruiker ingelogd is (0.1). De regel "`vr-namens` = de tegenpartij van de aanvrager" volstaat:
- Een koepel met beheerrecht is ingelogd als koepel, dus `vr-namens` is de koepel. Ze kan dus niet namens haar lid bevestigen.
- Zonder actieve vertegenwoordiger kan niemand op die vereniging inloggen. Niemand bevestigt, en de aanvraag vervalt na 90 dagen. Dat is zo bedoeld (story 2).
- VR-acties sturen ook `vr-namens` mee (beslist op 2026-11-19).

---

## 7. Nieuwe verenigingen (story 6)

> De initiële Excel-upload (story 8) laten we voorlopig buiten beschouwing.

- Het voorstel zet een SUB-koppeling **rechtstreeks** bij de registratie, maar maakt voor een lidmaatschap een **aanvraag**.
- Dat is omgekeerd van wat je zou verwachten: de SUB-koppeling is de sterkere relatie (subtype, K6/K7, beheer).
- [ ] Waarom heeft SUB geen bevestiging nodig en een lidmaatschap wel? Als het antwoord is "de koepel richt de vereniging zelf op", dan geldt dat ook voor het lidmaatschap.

---

## 8. Bevestigen schrijft naar twee streams

Bevestigen schrijft naar de aanvraag én naar de vereniging, in één transactie (0.4).

- [ ] **`ETag`:** van welke stream? Wie daarna het lidmaatschap wil aanpassen met `If-Match`, heeft de versie van de **vereniging** nodig, niet die van de aanvraag.
- [ ] **`VR-Sequence`:** de hoogste van beide? Daarmee kan de klant wachten op de projecties van zowel de aanvraag als de vereniging.
- [ ] **`Location`:** het voorstel verwijst naar het nieuwe lidmaatschap (`/lidmaatschappen/3`). Voor een SUB-koppeling bestaat er geen id. Verwijst `Location` dan naar de detail van de vereniging?

---

## 9. `400` of `403` voor "verkeerde partij"

- Indienen: `vr-namens` is geen partij → `400` (zoals de spec, beslist op 2026-11-19).
- Bevestigen, weigeren, intrekken: het analysedocument zegt `403` voor dezelfde soort fout.
- [ ] Gelijktrekken? Wat zeggen de specs van stories 3, 5 en 23?

---

## 10. Kleinere punten

- **`afhandelDatum`** (stories 3, 13, 23): AGENTS.md verbiedt `DateOnly?` op een `*Document`. Dat moet anders gemodelleerd worden, bv. een aparte afhandeling met datum en status.
- **Vervaldatum:** is de aanvraag op de vervaldatum zelf nog te bevestigen? (Story 13 vraagt al "einde van de dag".)
- **Reden bij weigeren of intrekken:** optioneel? (Story 3 open.)
- **GET zonder `vr-namens`:** elke initiator ziet alle aanvragen van elke vereniging. Beslist als "nog", maar het moet nog op de lijst voor story 10.

---

## 11. Analysedocument niet meer in lijn met de beslissingen

In `koepels-ontbrekende-punten.md`:

| Waar | Staat er | Beslist |
|---|---|---|
| 0.5 | `[ ] Prefix bevestigen: RA?` | `RA` (story 2, 2026-11-19) |
| Story 2, "Technisch: kan starten" | `403` als `vr-namens` geen partij is | `400` |
| Story 2, "Voorstel response (zelfde type staat al open)" | `200 OK` met de open aanvraag | `400` met het id |
| Story 2 en 10, voorbeeld-responses | `aanvraagDatum`/`vervalDatum` met tijdstip, `afhandelDatum: null` | `DateOnly`, nog geen `afhandelDatum` |
| Story 2, "Voorbeeld fouten" | `403` voor geen partij | `400` |
| Story 4 en N1 | Story 4 "al beslist": de aanvraag blijft open. N1: open vraag of ze afgesloten wordt | Tegenstrijdig (punt 4) |
| Story 5 | Bevestiging met `NietBepaald` | Indienen eist al `Koepel`. Kan enkel nog als de status intussen wijzigde (punt 3). |
| Samenvatting, #4 en #9 | Prefix en story 2-beslissingen nog open | Beslist op 2026-11-19 |
