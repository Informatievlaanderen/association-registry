// Relatieaanvragen - demo
//
// Startversie: de collectie wordt verder onderhouden in de Postman workspace (de bron van waarheid).
// Dit bestand wordt niet bijgewerkt. Opnieuw genereren overschrijft wat er sindsdien in Postman is aangepast.
//
// De belangrijkste scenario's voor een demo aan de klant, uit de regressiecollectie (features/relatieaanvragen): de
// afhandelingen (bevestigen, weigeren, intrekken) en vier bedrijfsregels. Validatie, autorisatie, If-Match en de
// randgevallen staan enkel in de regressiecollectie. Het verval na 90 dagen is niet live te tonen: D1 toont de
// vervaldatum.
//
// Elke folder is een zelfstandig verhaal, met de uitleg in de Overview van de folder. Stuur de requests in volgorde
// (Send), of draai de folder in de Runner: enkel de Runner herhaalt een request tot de projectie bij is.
//
// Rollen: A is altijd een nieuwe vereniging zonder rechtspersoonlijkheid, met een leesbare naam (een registratie
// zonder locaties gaat niet door de dubbeldetectie). K en L zijn koepels (KBO). Op Test zijn K en L bestaande
// verenigingen: ze worden eerst als koepel geïdentificeerd (200 of 202), en de lijst van K bevat ook aanvragen van de
// regressietesten.

const { lijstOp, bevatOp } = require('../../shared/builders');

// ================================================================ test data

const LIDMAATSCHAP = 'Lidmaatschap';
const SUBVERENIGING = 'Subvereniging';
const KOEPEL = 'Koepel';
const NIET_BEPAALD = 'NietBepaald';
const AANGEVRAAGD = 'Aangevraagd';
const BEVESTIGD = 'Bevestigd';
const GEWEIGERD = 'Geweigerd';
const INGETROKKEN = 'Ingetrokken';

const BERICHT = {
  nietDeOntvanger: 'Enkel de ontvanger van de relatieaanvraag kan ze bevestigen of weigeren.',
  nietDeAanvrager: 'Enkel de aanvrager van de relatieaanvraag kan ze intrekken.',
  zelfdeTypeOpen: (id) => `Er staat al een relatieaanvraag van hetzelfde type open voor deze verenigingen: '${id}'.`,
  geenKoepel: 'De beoogde koepel moet als koepel geïdentificeerd zijn.',
  subKanGeenLid: 'Een subvereniging kan geen lid worden van een koepel.',
};

const AANSLUITEN = 'Wij willen aansluiten bij de koepel.';

// Datums (yyyy-MM-dd, UTC zoals de API-containers): de aanvraagdatum en de vervaldatum, 90 dagen later, tenzij de
// environment vervaltermijnInDagen zet (waar de Admin API een andere termijn heeft, bv. 1 op Test).
const SETUP_DATUMS = `const dag = (n) => { const d = new Date(); d.setUTCDate(d.getUTCDate() + n); return d.toISOString().slice(0, 10); };
const vervaltermijn = String(pm.variables.get('vervaltermijnInDagen') === undefined ? '' : pm.variables.get('vervaltermijnInDagen')).trim();
pm.collectionVariables.set('vandaag', dag(0));
pm.collectionVariables.set('vervaldatum', dag(vervaltermijn === '' ? 90 : Number(vervaltermijn)));`;

// ================================================================ building blocks

const ok = (status = 202) => ({ status });
const fout = (detail) => ({ status: 400, detail });

const vCode = (rol) => `{{vCode${rol}}}`;
// het aanvraagId van het scenario (uit de Location header van de aanvraag)
const aanvraag = (id) => `{{aanvraagId_${id}}}`;

const EEN_KOEPEL = { K: { kbo: true } };
const TWEE_KOEPELS = { K: { kbo: true }, L: { kbo: true } };

// POST /v1/relatieaanvragen namens een partij (header vr-namens). A is altijd de vereniging.
function dienIn(id, naam, { koepel = 'K', type = LIDMAATSCHAP, namens = 'A', beschrijving } = {}, verwacht = ok()) {
  return {
    naam,
    actie: 'POST',
    beheerPad: 'v1/relatieaanvragen',
    headers: { 'vr-namens': vCode(namens) },
    body: { koepelVCode: vCode(koepel), verenigingVCode: vCode('A'), relatieType: type, ...(beschrijving ? { beschrijving } : {}) },
    verwacht,
    ...(verwacht.status === 202 ? { bewaarId: `aanvraagId_${id}` } : {}),
  };
}

// POST /v1/relatieaanvragen/{aanvraagId}/bevestiging, /weigering of /intrekking namens een partij
const handelAf =
  (afhandeling, standaardBody) =>
  (id, naam, namens, body = standaardBody, verwacht = { status: 202, locatie: aanvraag(id) }) => ({
    naam,
    actie: 'POST',
    beheerPad: `v1/relatieaanvragen/${aanvraag(id)}/${afhandeling}`,
    headers: { 'vr-namens': vCode(namens) },
    body,
    verwacht,
  });
// bevestigen met een (eventueel lege) body, weigeren en intrekken zonder body
const bevestig = handelAf('bevestiging', {});
const weiger = handelAf('weigering');
const trekIn = handelAf('intrekking');

// PATCH /v1/verenigingen/{vCode}/koepelgegevens: 200 als de koepelstatus al zo was (hergebruikte vereniging op Test)
const koepelstatus = (rol, status, naam) => ({
  naam,
  actie: 'PATCH',
  op: rol,
  pad: 'koepelgegevens',
  body: { koepelstatus: status },
  verwacht: { status: [200, 202] },
});
const alsKoepel = (rol = 'K') => koepelstatus(rol, KOEPEL, `Identificeer ${rol} als koepel`);

// GET /v1/relatieaanvragen/{aanvraagId}, herhaald tot de projectie bij is
const bekijkAanvraag = (id, naam, props) => ({
  naam,
  pad: `v1/relatieaanvragen/${aanvraag(id)}?expectedSequence={{sequence}}`,
  spec: { props },
});
// GET /v1/verenigingen/{vCode}/relatieaanvragen[?status=]: de aanvragen van een vereniging, als koepel of als vereniging
const aanvragenVan = (rol, naam, spec, status) => ({
  naam,
  pad: `v1/verenigingen/${vCode(rol)}/relatieaanvragen?expectedSequence={{sequence}}${status ? `&status=${status}` : ''}`,
  spec,
});
const relatieaanvragen = (items) => lijstOp('relatieaanvragen', items);
// een controle tussen twee stappen, bv. de aanvraag die nog open staat voor ze bevestigd wordt
const tussendoor = (controle) => ({ controle });

// beheer detail en historiek van A
const lidmaatschappen = (items) => lijstOp('lidmaatschappen', items);
const lidVan = (rol, identificatie = '', beschrijving = '') => ({ andereVereniging: vCode(rol), van: '{{vandaag}}', tot: '', identificatie, beschrijving });
const gebeurtenis = (naam) => ({ gebeurtenis: naam, aantal: 1 });

// ================================================================ scenarios

const afhandelen = [
  {
    id: 'D1',
    name: 'Vereniging vraagt lidmaatschap aan, koepel bevestigt -> lid van de koepel',
    description:
      'Story 4 en 5. De vereniging vraagt zelf het lidmaatschap aan: wie handelt, staat in de header vr-namens. De ' +
      'aanvraag krijgt een eigen id (RA...), staat op Aangevraagd en vervalt na 90 dagen als niemand ze afhandelt. De ' +
      'koepel ziet ze bij haar open aanvragen en bevestigt. Pas dan ontstaat het lidmaatschap op de vereniging: vanaf ' +
      'vandaag, zonder einddatum, en de vereniging wordt een feitelijke vereniging (FV). De historiek van de vereniging ' +
      'toont het lidmaatschap, niet de aanvraag: die leeft apart.',
    a: { naam: 'Wandelclub Hobbitstee' },
    verenigingen: EEN_KOEPEL,
    stappen: [
      alsKoepel(),
      dienIn('D1', 'A vraagt lidmaatschap aan bij K', { beschrijving: AANSLUITEN }),
      tussendoor(
        bekijkAanvraag('D1', 'Bekijk de aanvraag: Aangevraagd, vervalt na 90 dagen', {
          aanvraagId: aanvraag('D1'),
          koepelVCode: vCode('K'),
          verenigingVCode: vCode('A'),
          aangevraagdDoor: vCode('A'),
          relatieType: LIDMAATSCHAP,
          status: AANGEVRAAGD,
          aanvraagDatum: '{{vandaag}}',
          vervalDatum: '{{vervaldatum}}',
          beschrijving: AANSLUITEN,
        })
      ),
      tussendoor(
        aanvragenVan(
          'K',
          'K ziet de aanvraag bij haar open aanvragen',
          bevatOp('relatieaanvragen', [{ aanvraagId: aanvraag('D1'), verenigingVCode: vCode('A'), status: AANGEVRAAGD }]),
          AANGEVRAAGD
        )
      ),
      bevestig('D1', 'K bevestigt de aanvraag', 'K', { identificatie: 'LID-0042', beschrijving: 'Aangesloten wandelclub' }),
    ],
    final: {
      beheerDetail: {
        ...lidmaatschappen([lidVan('K', 'LID-0042', 'Aangesloten wandelclub')]),
        props: { verenigingssubtype: { code: 'FV' } },
      },
    },
    historiek: {
      bevat: [gebeurtenis('LidmaatschapWerdToegevoegd'), gebeurtenis('VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging')],
      geen: ['Relatieaanvraag'],
    },
    controles: [bekijkAanvraag('D1', 'Bekijk de aanvraag: Bevestigd', { status: BEVESTIGD, afhandelDatum: '{{vandaag}}' })],
  },
  {
    id: 'D2',
    name: 'Koepel vraagt vereniging als subvereniging, vereniging bevestigt -> subvereniging van de koepel',
    description:
      'Story 2 en 3. Ook de koepel kan een aanvraag indienen, hier om subvereniging te worden. De vereniging ziet de ' +
      'aanvraag in haar lijst en bevestigt. Ze wordt subvereniging (subtype SUB) van de koepel, met de identificatie en ' +
      'beschrijving uit de bevestiging.',
    a: { naam: 'Toneelkring Rivendel' },
    verenigingen: EEN_KOEPEL,
    stappen: [
      alsKoepel(),
      dienIn('D2', 'K vraagt A om subvereniging te worden', { type: SUBVERENIGING, namens: 'K', beschrijving: 'Word onze afdeling in Rivendel.' }),
      tussendoor(
        aanvragenVan(
          'A',
          'A ziet de aanvraag van K',
          relatieaanvragen([
            { aanvraagId: aanvraag('D2'), koepelVCode: vCode('K'), aangevraagdDoor: vCode('K'), relatieType: SUBVERENIGING, status: AANGEVRAAGD },
          ])
        )
      ),
      bevestig('D2', 'A bevestigt de aanvraag', 'A', { identificatie: 'AFD-007', beschrijving: 'Afdeling Rivendel' }),
    ],
    final: {
      beheerDetail: {
        props: {
          verenigingssubtype: { code: 'SUB' },
          subverenigingVan: { andereVereniging: vCode('K'), identificatie: 'AFD-007', beschrijving: 'Afdeling Rivendel' },
        },
      },
    },
    historiek: { bevat: [gebeurtenis('VerenigingssubtypeWerdVerfijndNaarSubvereniging')] },
    controles: [bekijkAanvraag('D2', 'Bekijk de aanvraag: Bevestigd', { status: BEVESTIGD, afhandelDatum: '{{vandaag}}' })],
  },
  {
    id: 'D3',
    name: 'Koepel weigert de lidmaatschapsaanvraag -> Geweigerd, geen lidmaatschap',
    description:
      'Story 5. De ontvanger kan een aanvraag weigeren. Er ontstaat geen relatie, en koepel en vereniging kunnen meteen ' +
      'een nieuwe aanvraag indienen.',
    a: { naam: 'Harmonie Minas Tirith' },
    verenigingen: EEN_KOEPEL,
    stappen: [
      alsKoepel(),
      dienIn('D3', 'A vraagt lidmaatschap aan bij K', { beschrijving: AANSLUITEN }),
      weiger('D3', 'K weigert de aanvraag', 'K'),
    ],
    final: { beheerDetail: lidmaatschappen([]) },
    controles: [bekijkAanvraag('D3', 'Bekijk de aanvraag: Geweigerd', { status: GEWEIGERD, afhandelDatum: '{{vandaag}}' })],
  },
  {
    id: 'D4',
    name: 'Vereniging trekt haar aanvraag in -> Ingetrokken, geen lidmaatschap',
    description: 'Story 23. De aanvrager kan zijn open aanvraag intrekken. Er ontstaat geen relatie.',
    a: { naam: 'Schaakclub Edoras' },
    verenigingen: EEN_KOEPEL,
    stappen: [
      alsKoepel(),
      dienIn('D4', 'A vraagt lidmaatschap aan bij K', { beschrijving: AANSLUITEN }),
      trekIn('D4', 'A trekt de aanvraag in', 'A'),
    ],
    final: { beheerDetail: lidmaatschappen([]) },
    controles: [
      bekijkAanvraag('D4', 'Bekijk de aanvraag: Ingetrokken', { status: INGETROKKEN, afhandelDatum: '{{vandaag}}' }),
    ],
  },
];

const bedrijfsregels = [
  {
    id: 'D5',
    name: 'Aanvrager kan niet zelf bevestigen, ontvanger kan niet intrekken -> 400, aanvraag blijft open',
    description:
      'Wie mag wat. Enkel de ontvanger (de andere partij dan de aanvrager) kan bevestigen of weigeren, enkel de ' +
      'aanvrager kan intrekken. De koepel kan haar eigen aanvraag dus niet namens de vereniging bevestigen.',
    a: { naam: 'Zangkoor Lothlorien' },
    verenigingen: EEN_KOEPEL,
    stappen: [
      alsKoepel(),
      dienIn('D5', 'K vraagt A om lid te worden', { namens: 'K', beschrijving: 'Sluit aan bij onze koepel.' }),
      bevestig('D5', 'K bevestigt haar eigen aanvraag', 'K', {}, fout(BERICHT.nietDeOntvanger)),
      trekIn('D5', 'A trekt de aanvraag van K in', 'A', {}, fout(BERICHT.nietDeAanvrager)),
    ],
    controles: [bekijkAanvraag('D5', 'Bekijk de aanvraag: nog Aangevraagd', { status: AANGEVRAAGD })],
  },
  {
    id: 'D6',
    name: 'Eén open aanvraag per koepel en vereniging, ongeacht wie ze indient -> 400 met de open aanvraag',
    description:
      'Story 2. Per koepel en vereniging staat er hoogstens één aanvraag open, ongeacht wie ze indiende. De foutmelding ' +
      'noemt de open aanvraag.',
    a: { naam: 'Fanfare Hobbitstee' },
    verenigingen: EEN_KOEPEL,
    stappen: [
      alsKoepel(),
      dienIn('D6', 'A vraagt lidmaatschap aan bij K', { beschrijving: AANSLUITEN }),
      dienIn('D6', 'K dient dezelfde aanvraag in voor A', { namens: 'K' }, fout(BERICHT.zelfdeTypeOpen(aanvraag('D6')))),
    ],
    controles: [
      aanvragenVan('A', 'A heeft één open aanvraag', relatieaanvragen([{ aanvraagId: aanvraag('D6'), aangevraagdDoor: vCode('A'), status: AANGEVRAAGD }])),
    ],
  },
  {
    id: 'D7',
    name: 'Beoogde koepel niet als koepel geïdentificeerd -> 400, na de identificatie lukt het',
    description:
      'Story 4. Een aanvraag kan enkel naar een vereniging met rechtspersoonlijkheid die als koepel geïdentificeerd is ' +
      '(koepelstatus Koepel). Na de identificatie lukt dezelfde aanvraag wel. Op Test zet de eerste stap een bestaande ' +
      'vereniging terug op NietBepaald; op Local is K nieuw en al NietBepaald (200).',
    a: { naam: 'Volksdansgroep Rohan' },
    verenigingen: EEN_KOEPEL,
    stappen: [
      koepelstatus('K', NIET_BEPAALD, 'Zet de koepelstatus van K op NietBepaald'),
      dienIn('D7', 'A vraagt lidmaatschap aan bij K', { beschrijving: AANSLUITEN }, fout(BERICHT.geenKoepel)),
      alsKoepel(),
      dienIn('D7', 'A vraagt opnieuw lidmaatschap aan bij K', { beschrijving: AANSLUITEN }),
    ],
    controles: [aanvragenVan('A', 'A heeft één open aanvraag', relatieaanvragen([{ aanvraagId: aanvraag('D7'), status: AANGEVRAAGD }]))],
  },
  {
    id: 'D8',
    name: 'Subvereniging kan nergens lid worden (K6) -> lidmaatschapsaanvraag 400',
    description:
      'Story 12, regel K6: een vereniging kan niet tegelijk subvereniging en lid zijn. Eens subvereniging van de ene ' +
      'koepel, kan ze bij geen enkele koepel lid worden.',
    a: { naam: 'Jeugdbeweging Fangorn' },
    verenigingen: TWEE_KOEPELS,
    stappen: [
      alsKoepel('K'),
      alsKoepel('L'),
      dienIn('D8', 'A vraagt subvereniging te worden van K', { type: SUBVERENIGING, beschrijving: 'Wij zijn de afdeling Fangorn.' }),
      bevestig('D8', 'K bevestigt: A is subvereniging van K', 'K'),
      dienIn('D8', 'A vraagt lidmaatschap aan bij L', { koepel: 'L', beschrijving: AANSLUITEN }, fout(BERICHT.subKanGeenLid)),
    ],
    final: {
      beheerDetail: {
        ...lidmaatschappen([]),
        props: { verenigingssubtype: { code: 'SUB' }, subverenigingVan: { andereVereniging: vCode('K') } },
      },
    },
  },
];

module.exports = {
  name: 'Relatieaanvragen - demo',
  description:
    'Demo van de relatieaanvragen tussen een koepel en een vereniging (Beheer API, met de header vr-namens): indienen, ' +
    'opvragen, bevestigen, weigeren en intrekken, en vier bedrijfsregels. Een selectie uit de regressiecollectie ' +
    'Relatieaanvragen; elke folder legt in zijn Overview uit wat hij toont.',
  setup: SETUP_DATUMS,
  groepen: [
    { name: 'Aanvragen afhandelen', scenarios: afhandelen },
    { name: 'Bedrijfsregels', scenarios: bedrijfsregels },
  ],
};
