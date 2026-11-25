// Relatieaanvragen
//
// Startversie: de collectie wordt verder onderhouden in de Postman workspace (de bron van waarheid).
// Dit bestand wordt niet bijgewerkt. Opnieuw genereren overschrijft wat er sindsdien in Postman is aangepast.
//
// Rules:
// - DienRelatieaanvraagInRequestValidator: verplichte velden (koepelVCode, verenigingVCode, relatieType), geen HTML in
//   de beschrijving.
// - DienRelatieaanvraagInRequest.ToCommand: vr-namens verplicht, geldige vCodes, Partijen (de aanvrager is de koepel of
//   de vereniging, koepel en vereniging verschillen), RelatieType (hoofdletterongevoelig).
// - BeoogdeKoepel: een KBO-vereniging die als koepel geïdentificeerd is.
// - Lidmaatschapsaanvraag: geen actieve relatie, geen actueel of toekomstig lidmaatschap bij dezelfde koepel, max. 1
//   open aanvraag per paar, een subvereniging kan geen lid worden, ook niet met een open subverenigingsaanvraag (K6).
// - Subverenigingsaanvraag: geen actieve relatie, max. 1 open aanvraag per paar, enkel een VZER met subtype leeg of
//   NB, geen lopend lidmaatschap en geen open lidmaatschapsaanvraag (K6), max. 1 open subverenigingsaanvraag (K7).
// - Opvragen: GET /v1/relatieaanvragen/{aanvraagId} en GET /v1/verenigingen/{vCode}/relatieaanvragen[?status=].
// - Bevestigen: POST /v1/relatieaanvragen/{aanvraagId}/bevestiging met vr-namens = de ontvanger (de andere partij dan
//   de aanvrager). BevestigRelatieaanvraagRequestValidator: geen HTML in identificatie en beschrijving.
//   OntvangenRelatieaanvraag: enkel de ontvanger, niet al bevestigd, geweigerd of ingetrokken, niet vervallen (90
//   dagen: niet te testen). Daarna de koepel en de regels van indienen opnieuw, zonder de aanvraag zelf. Lidmaatschap:
//   vanaf vandaag zonder einddatum, subtype leeg of NB wordt FV. Subvereniging: subtype SUB van de koepel.
// - Weigeren: POST /v1/relatieaanvragen/{aanvraagId}/weigering met vr-namens = de ontvanger en een optionele reden.
//   WeigerRelatieaanvraagRequestValidator: geen HTML in de reden. Dezelfde OntvangenRelatieaanvraag, maar verder geen
//   controles: ook een koepel die niet meer als koepel geïdentificeerd is, kan weigeren. Er ontstaat geen relatie, en
//   het paar is weer vrij voor een nieuwe aanvraag.
// - Intrekken: POST /v1/relatieaanvragen/{aanvraagId}/intrekking met vr-namens = de aanvrager en een optionele reden.
//   TrekRelatieaanvraagInRequestValidator: geen HTML in de reden. IngediendeRelatieaanvraag: enkel de aanvrager, en
//   dezelfde controles op de status en de vervaldatum als hierboven. Verder geen controles. Er ontstaat geen relatie,
//   en het paar is weer vrij voor een nieuwe aanvraag.
// Een relatieaanvraag heeft een eigen stream: ze verschijnt niet in de historiek of de detail van de verenigingen. Enkel
// de relatie die bij de bevestiging ontstaat, komt op de vereniging.
//
// Rollen: A is altijd een nieuwe VZER (de vereniging), K en L zijn koepels (KBO; op Test hergebruikt), X is een
// tweede VZER. Op Test zijn K en L bestaande verenigingen: ze worden eerst als koepel geïdentificeerd (200 of 202), en
// hun lijst van relatieaanvragen wordt enkel met bevat gecontroleerd. Een paar met A is altijd nieuw, dus de open
// aanvragen van een vorige run blokkeren niets.

// ================================================================ test data

const LIDMAATSCHAP = 'Lidmaatschap';
const SUBVERENIGING = 'Subvereniging';
const RELATIETYPES = [LIDMAATSCHAP, SUBVERENIGING];
const STATUSSEN = ['Aangevraagd', 'Bevestigd', 'Geweigerd', 'Ingetrokken', 'Vervallen'];
const AANGEVRAAGD = 'Aangevraagd';
const ONBEKENDE_VCODE = 'V9999999';
const ONBEKENDE_AANVRAAG = 'RA9999999';
const AANVRAAG_ID_FORMAAT = 'RA0000000';
const BEVESTIGD = 'Bevestigd';
const GEWEIGERD = 'Geweigerd';
const INGETROKKEN = 'Ingetrokken';

// "A, B of C", zoals AllowedValues.Describe
const opsomming = (waarden) => (waarden.length === 1 ? waarden[0] : `${waarden.slice(0, -1).join(', ')} of ${waarden[waarden.length - 1]}`);

const BERICHT = {
  verplichteVelden: 'Geef een geldig JSON-object met verenigingVCode, koepelVCode en relatieType op.',
  html: 'Deze waarde bevat niet toegestane tekens.',
  vrNamensVerplicht: 'vr-namens is verplicht.',
  vCodeFormaat: "Formaat van de VCode moet 'V0000000' zijn",
  ongeldigRelatieType: `RelatieType moet ${opsomming(RELATIETYPES)} zijn.`,
  ongeldigeStatus: `De status moet ${opsomming(STATUSSEN)} zijn.`,
  geenPartij: 'De aanvrager moet een partij van de relatieaanvraag zijn.',
  zichzelf: 'Een vereniging kan geen relatie met zichzelf aanvragen.',
  koepelGeenKbo: 'De beoogde koepel moet een KBO-vereniging zijn.',
  geenKoepel: 'De beoogde koepel moet als koepel geïdentificeerd zijn.',
  nietGevonden: `De resource met id '${ONBEKENDE_VCODE}' werd niet gevonden.`,
  aanvraagNietGevonden: 'De gevraagde relatieaanvraag werd niet gevonden.',
  actieveRelatie: 'Er bestaat al een actieve feitelijke relatie tussen deze verenigingen.',
  toekomstigLidmaatschap: 'Een actueel of toekomstig lidmaatschap bij dezelfde koepel blokkeert de lidmaatschapsaanvraag.',
  subNietToegestaan: 'Een subverenigingsaanvraag is alleen toegestaan voor VZER met subtype null of NB.',
  subKanGeenLid: 'Een subvereniging kan geen lid worden van een koepel.',
  lidKanGeenSub: 'Een vereniging met een lidmaatschap kan geen subvereniging worden.',
  openSubKanGeenLid: (id) => `Een vereniging met een open subverenigingsaanvraag kan geen lid worden van een koepel (relatieaanvraag: '${id}').`,
  openLidKanGeenSub: (id) => `Een vereniging met een open lidmaatschapsaanvraag kan geen subvereniging worden (relatieaanvraag: '${id}').`,
  zelfdeTypeOpen: (id) => `Er staat al een relatieaanvraag van hetzelfde type open voor deze verenigingen: '${id}'.`,
  anderTypeOpen: 'Er bestaat al een open aanvraag van een ander relatietype voor dit paar.',
  subAlOpen: (id) => `Er staat al een aanvraag open om subvereniging te worden: '${id}'.`,
  nietDeOntvanger: 'Enkel de ontvanger van de relatieaanvraag kan ze bevestigen of weigeren.',
  alBevestigd: 'De relatieaanvraag werd al bevestigd.',
  alGeweigerd: 'De relatieaanvraag werd al geweigerd.',
  nietDeAanvrager: 'Enkel de aanvrager van de relatieaanvraag kan ze intrekken.',
  alIngetrokken: 'De relatieaanvraag werd al ingetrokken.',
  aanvraagIdFormaat: `Formaat van de aanvraagId moet '${AANVRAAG_ID_FORMAAT}' zijn.`,
  aanvraagOnbekend: `De resource met id '${ONBEKENDE_AANVRAAG}' werd niet gevonden.`,
};

// Datums (yyyy-MM-dd, UTC zoals de API-containers): aanvraag- en vervaldatum, begin en einde van een lidmaatschap.
const SETUP_DATUMS = `const dag = (n) => { const d = new Date(); d.setUTCDate(d.getUTCDate() + n); return d.toISOString().slice(0, 10); };
pm.collectionVariables.set('vandaag', dag(0));
pm.collectionVariables.set('vervaldatum', dag(90));
pm.collectionVariables.set('gisteren', dag(-1));
pm.collectionVariables.set('morgen', dag(1));`;

// ================================================================ building blocks

const ok = (status = 202) => ({ status });
const fout = (detail) => ({ status: 400, detail });
const validatie = (veld, reden) => ({ status: 400, validatie: { veld, reden } });

// a rol (A, K, L, X) or a literal value (an unknown or invalid vCode)
const vCode = (x) => (/^[A-Z]$/.test(x) ? `{{vCode${x}}}` : x);
const aanvraagVar = (id, n = 1) => `aanvraagId_${id}_${n}`;
const aanvraag = (id, n = 1) => `{{${aanvraagVar(id, n)}}}`;

// POST /v1/relatieaanvragen. namens: rol of literal, null = zonder vr-namens.
// bewaar: het aanvraagId komt in aanvraagId_<id>_<n> (uit de Location header).
function dienIn(naam, { koepel = 'K', vereniging = 'A', type = LIDMAATSCHAP, namens = 'A', beschrijving, body } = {}, verwacht = ok(), extra = {}) {
  return {
    naam,
    actie: 'POST',
    beheerPad: 'v1/relatieaanvragen',
    headers: namens === null ? {} : { 'vr-namens': vCode(namens) },
    body: body !== undefined ? body : { koepelVCode: vCode(koepel), verenigingVCode: vCode(vereniging), relatieType: type, ...(beschrijving === undefined ? {} : { beschrijving }) },
    verwacht,
    ...extra,
  };
}
const ingediend = (id, n, naam, opties) => dienIn(naam, opties, ok(), { bewaarId: aanvraagVar(id, n) });

// POST /v1/relatieaanvragen/{aanvraagId}/bevestiging, /weigering of /intrekking. Default: K handelt aanvraag 1 van het
// scenario af, met een lege body (geen relatiegegevens, geen reden). aanvraagId: een literal (onbekend of ongeldig).
// namens: rol of literal, null = zonder vr-namens.
const handelAf =
  (afhandeling) =>
  (naam, { id, n = 1, aanvraagId, namens = 'K', body = {} } = {}, verwacht = ok(), extra = {}) => ({
    naam,
    actie: 'POST',
    beheerPad: `v1/relatieaanvragen/${aanvraagId || aanvraag(id, n)}/${afhandeling}`,
    headers: namens === null ? {} : { 'vr-namens': vCode(namens) },
    body,
    verwacht,
    ...extra,
  });
const bevestig = handelAf('bevestiging');
const weiger = handelAf('weigering');
const trekIn = handelAf('intrekking');

const alsKoepel = (rol = 'K') => ({
  naam: `Identificeer ${rol} als koepel`,
  actie: 'PATCH',
  op: rol,
  pad: 'koepelgegevens',
  body: { koepelstatus: 'Koepel' },
  verwacht: { status: [200, 202] },
});
const nietAlsKoepel = (rol = 'K') => ({
  naam: `Zet koepelstatus van ${rol} op NietBepaald`,
  actie: 'PATCH',
  op: rol,
  pad: 'koepelgegevens',
  body: { koepelstatus: 'NietBepaald' },
  verwacht: { status: [200, 202] },
});
const wachtOp = (rol) => ({ wachtOp: rol });
const lidmaatschap = (rol, { van, tot } = {}) => ({
  naam: `Voeg lidmaatschap van A bij ${rol} toe${van ? ' vanaf ' + van : ''}${tot ? ' tot ' + tot : ''}`,
  actie: 'POST',
  pad: 'lidmaatschappen',
  body: { andereVereniging: vCode(rol), ...(van ? { van } : {}), ...(tot ? { tot } : {}) },
});
const subverenigingVan = (rol) => ({
  naam: `Maak A subvereniging van ${rol}`,
  actie: 'PATCH',
  pad: 'subtype',
  body: { subtype: 'SUB', andereVereniging: vCode(rol) },
});
const subtype = (code) => ({ naam: `Zet subtype van A op ${code}`, actie: 'PATCH', pad: 'subtype', body: { subtype: code } });

// controles op de opvraag-endpoints (retried until the projection holds)
const detail = (id, props, n = 1) => ({
  naam: `Detail relatieaanvraag ${n}`,
  pad: `v1/relatieaanvragen/${aanvraag(id, n)}?expectedSequence={{sequence}}`,
  spec: { props },
});
const lijstVan = (rol, items, { status, bevat = false } = {}) => ({
  naam: `Relatieaanvragen van ${rol}${status ? ' met status ' + status : ''}`,
  pad: `v1/verenigingen/${vCode(rol)}/relatieaanvragen?expectedSequence={{sequence}}${status ? '&status=' + status : ''}`,
  spec: bevat ? { bevat: { veld: 'relatieaanvragen', items } } : { lijst: { veld: 'relatieaanvragen', items } },
});
const item = (id, n = 1, extra = {}) => ({ aanvraagId: aanvraag(id, n), status: AANGEVRAAGD, ...extra });

const KOEPEL = { K: { kbo: true } };
const TWEE_KOEPELS = { K: { kbo: true }, L: { kbo: true } };
// A relatieaanvraag changes nothing on the verenigingen.
const GEEN_RELATIEAANVRAAG_IN_HISTORIEK = { geen: ['Relatieaanvraag'] };

// beheer detail van A na een bevestiging
const lidmaatschapVan = (rol, { identificatie = '', beschrijving = '' } = {}) => ({
  andereVereniging: vCode(rol),
  van: '{{vandaag}}',
  tot: '',
  identificatie,
  beschrijving,
});
const lidmaatschappen = (items) => ({ lijst: { veld: 'lidmaatschappen', items } });
const GEEN_LIDMAATSCHAP = { beheerDetail: lidmaatschappen([]) };
const gebeurtenis = (naam) => ({ gebeurtenis: naam, aantal: 1 });
const nogOpen = (id) => detail(id, { status: AANGEVRAAGD });

// ================================================================ scenarios

const indienen = [
  {
    id: 'RA1',
    name: 'Vereniging vraagt lidmaatschap aan bij koepel -> Aangevraagd, vervalt na 90 dagen',
    verenigingen: KOEPEL,
    stappen: [alsKoepel(), ingediend('RA1', 1, 'Dien lidmaatschapsaanvraag in namens A', { beschrijving: 'Lid van de Raad van Elrond' })],
    historiek: GEEN_RELATIEAANVRAAG_IN_HISTORIEK,
    controles: [
      detail('RA1', {
        aanvraagId: aanvraag('RA1'),
        koepelVCode: '{{vCodeK}}',
        verenigingVCode: '{{vCodeA}}',
        aangevraagdDoor: '{{vCodeA}}',
        relatieType: LIDMAATSCHAP,
        status: AANGEVRAAGD,
        aanvraagDatum: '{{vandaag}}',
        vervalDatum: '{{vervaldatum}}',
        beschrijving: 'Lid van de Raad van Elrond',
      }),
      lijstVan('A', [item('RA1', 1, { koepelVCode: '{{vCodeK}}', relatieType: LIDMAATSCHAP })]),
      lijstVan('K', [item('RA1', 1, { verenigingVCode: '{{vCodeA}}' })], { bevat: true }),
    ],
  },
  {
    id: 'RA2',
    name: 'Koepel vraagt vereniging om subvereniging te worden, zonder beschrijving -> Aangevraagd door de koepel',
    verenigingen: KOEPEL,
    stappen: [alsKoepel(), ingediend('RA2', 1, 'Dien subverenigingsaanvraag in namens K', { type: SUBVERENIGING, namens: 'K' })],
    historiek: GEEN_RELATIEAANVRAAG_IN_HISTORIEK,
    controles: [
      detail('RA2', {
        koepelVCode: '{{vCodeK}}',
        verenigingVCode: '{{vCodeA}}',
        aangevraagdDoor: '{{vCodeK}}',
        relatieType: SUBVERENIGING,
        status: AANGEVRAAGD,
        beschrijving: '',
      }),
      lijstVan('A', [item('RA2', 1, { relatieType: SUBVERENIGING })]),
    ],
  },
  {
    id: 'RA3',
    name: 'RelatieType in een andere schrijfwijze (lidmaatschap) -> aanvaard, bewaard als Lidmaatschap',
    verenigingen: KOEPEL,
    stappen: [alsKoepel(), ingediend('RA3', 1, 'Dien aanvraag in met relatieType in kleine letters', { type: 'lidmaatschap' })],
    controles: [detail('RA3', { relatieType: LIDMAATSCHAP })],
  },
  {
    id: 'RA4',
    name: 'Lidmaatschapsaanvragen bij twee koepels -> beide open',
    verenigingen: TWEE_KOEPELS,
    stappen: [
      alsKoepel('K'),
      alsKoepel('L'),
      ingediend('RA4', 1, 'Dien lidmaatschapsaanvraag bij K in', { koepel: 'K' }),
      ingediend('RA4', 2, 'Dien lidmaatschapsaanvraag bij L in', { koepel: 'L' }),
    ],
    controles: [
      lijstVan('A', [
        item('RA4', 1, { koepelVCode: '{{vCodeK}}' }),
        item('RA4', 2, { koepelVCode: '{{vCodeL}}' }),
      ]),
    ],
  },
  {
    id: 'RA5',
    name: 'Beëindigd lidmaatschap bij dezelfde koepel -> nieuwe lidmaatschapsaanvraag toegestaan',
    verenigingen: KOEPEL,
    stappen: [
      alsKoepel(),
      wachtOp('K'),
      lidmaatschap('K', { tot: '{{gisteren}}' }),
      ingediend('RA5', 1, 'Dien lidmaatschapsaanvraag bij K in'),
    ],
    controles: [detail('RA5', { relatieType: LIDMAATSCHAP, status: AANGEVRAAGD })],
  },
  {
    id: 'RA6',
    name: 'Vereniging met rechtspersoonlijkheid vraagt lidmaatschap aan -> Aangevraagd',
    // On Test both KBO verenigingen are reused: the pair is still open from a previous run.
    vereist: { kboModus: 'random' },
    kbo: true,
    verenigingen: KOEPEL,
    stappen: [alsKoepel(), ingediend('RA6', 1, 'Dien lidmaatschapsaanvraag in namens A')],
    controles: [detail('RA6', { verenigingVCode: '{{vCodeA}}', relatieType: LIDMAATSCHAP, status: AANGEVRAAGD })],
  },
];

const partijen = [
  {
    id: 'RA10',
    name: 'Aanvrager is geen partij -> 400',
    verenigingen: KOEPEL,
    stappen: [alsKoepel(), dienIn('Dien in namens een andere vereniging', { namens: ONBEKENDE_VCODE }, fout(BERICHT.geenPartij))],
    controles: [lijstVan('A', [])],
  },
  {
    id: 'RA11',
    name: 'Relatie met zichzelf -> 400',
    stappen: [dienIn('Dien aanvraag in met A als koepel en vereniging', { koepel: 'A' }, fout(BERICHT.zichzelf))],
  },
  {
    id: 'RA12',
    name: 'Zonder of met ongeldige vr-namens -> 400',
    verenigingen: KOEPEL,
    stappen: [
      alsKoepel(),
      dienIn('Dien in zonder vr-namens', { namens: null }, fout(BERICHT.vrNamensVerplicht)),
      dienIn('Dien in met ongeldige vr-namens', { namens: 'ongeldig' }, fout(BERICHT.vCodeFormaat)),
    ],
    controles: [lijstVan('A', [])],
  },
];

const koepel = [
  {
    id: 'RA20',
    name: 'Beoogde koepel zonder rechtspersoonlijkheid -> 400',
    verenigingen: { X: {} },
    stappen: [
      dienIn('Vraag lidmaatschap aan bij X', { koepel: 'X' }, fout(BERICHT.koepelGeenKbo)),
      dienIn('Vraag subvereniging aan bij X', { koepel: 'X', type: SUBVERENIGING }, fout(BERICHT.koepelGeenKbo)),
    ],
    controles: [lijstVan('A', [])],
  },
  {
    id: 'RA21',
    name: 'Beoogde koepel niet als koepel geïdentificeerd -> 400',
    verenigingen: KOEPEL,
    stappen: [
      nietAlsKoepel(),
      dienIn('Vraag lidmaatschap aan bij K', {}, fout(BERICHT.geenKoepel)),
      dienIn('Vraag subvereniging aan bij K namens K', { type: SUBVERENIGING, namens: 'K' }, fout(BERICHT.geenKoepel)),
    ],
    controles: [lijstVan('A', [])],
  },
  {
    id: 'RA22',
    name: 'Onbekende koepel of vereniging -> 400',
    verenigingen: KOEPEL,
    stappen: [
      alsKoepel(),
      dienIn('Vraag lidmaatschap aan bij een onbekende koepel', { koepel: ONBEKENDE_VCODE }, fout(BERICHT.nietGevonden)),
      dienIn('K vraagt een onbekende vereniging', { vereniging: ONBEKENDE_VCODE, namens: 'K' }, fout(BERICHT.nietGevonden)),
    ],
  },
];

const openAanvragen = [
  {
    id: 'RA30',
    name: 'Zelfde type opnieuw, ook door de andere partij -> 400 met de open aanvraag',
    verenigingen: KOEPEL,
    stappen: [
      alsKoepel(),
      ingediend('RA30', 1, 'Dien lidmaatschapsaanvraag in namens A'),
      dienIn('Dien dezelfde aanvraag opnieuw in namens A', {}, fout(BERICHT.zelfdeTypeOpen(aanvraag('RA30')))),
      dienIn('Dien dezelfde aanvraag in namens K', { namens: 'K' }, fout(BERICHT.zelfdeTypeOpen(aanvraag('RA30')))),
    ],
    controles: [lijstVan('A', [item('RA30')])],
  },
  {
    id: 'RA31',
    name: 'Ander type voor hetzelfde paar -> 400',
    verenigingen: KOEPEL,
    stappen: [
      alsKoepel(),
      ingediend('RA31', 1, 'Dien lidmaatschapsaanvraag in'),
      dienIn('Dien subverenigingsaanvraag bij dezelfde koepel in', { type: SUBVERENIGING }, fout(BERICHT.anderTypeOpen)),
    ],
    controles: [lijstVan('A', [item('RA31', 1, { relatieType: LIDMAATSCHAP })])],
  },
  {
    id: 'RA32',
    name: 'Open subverenigingsaanvraag -> lidmaatschapsaanvraag bij een andere koepel 400 (K6)',
    verenigingen: TWEE_KOEPELS,
    stappen: [
      alsKoepel('K'),
      alsKoepel('L'),
      ingediend('RA32', 1, 'Dien subverenigingsaanvraag bij K in', { type: SUBVERENIGING }),
      dienIn('Dien lidmaatschapsaanvraag bij L in', { koepel: 'L' }, fout(BERICHT.openSubKanGeenLid(aanvraag('RA32')))),
    ],
    controles: [lijstVan('A', [item('RA32', 1, { koepelVCode: '{{vCodeK}}', relatieType: SUBVERENIGING })])],
  },
  {
    id: 'RA33',
    name: 'Open lidmaatschapsaanvraag -> subverenigingsaanvraag bij een andere koepel 400 (K6)',
    verenigingen: TWEE_KOEPELS,
    stappen: [
      alsKoepel('K'),
      alsKoepel('L'),
      ingediend('RA33', 1, 'Dien lidmaatschapsaanvraag bij K in'),
      dienIn('Dien subverenigingsaanvraag bij L in', { koepel: 'L', type: SUBVERENIGING }, fout(BERICHT.openLidKanGeenSub(aanvraag('RA33')))),
    ],
    controles: [lijstVan('A', [item('RA33', 1, { koepelVCode: '{{vCodeK}}', relatieType: LIDMAATSCHAP })])],
  },
  {
    id: 'RA34',
    name: 'Open subverenigingsaanvraag -> tweede subverenigingsaanvraag bij een andere koepel 400 (K7)',
    verenigingen: TWEE_KOEPELS,
    stappen: [
      alsKoepel('K'),
      alsKoepel('L'),
      ingediend('RA34', 1, 'Dien subverenigingsaanvraag bij K in', { type: SUBVERENIGING }),
      dienIn('Dien subverenigingsaanvraag bij L in namens L', { koepel: 'L', type: SUBVERENIGING, namens: 'L' }, fout(BERICHT.subAlOpen(aanvraag('RA34')))),
    ],
    controles: [lijstVan('A', [item('RA34', 1, { koepelVCode: '{{vCodeK}}' })])],
  },
];

const bestaandeRelaties = [
  {
    id: 'RA40',
    name: 'Actief lidmaatschap bij de koepel -> lidmaatschaps- en subverenigingsaanvraag 400',
    verenigingen: KOEPEL,
    stappen: [
      alsKoepel(),
      wachtOp('K'),
      lidmaatschap('K'),
      dienIn('Vraag lidmaatschap aan bij K', {}, fout(BERICHT.actieveRelatie)),
      dienIn('Vraag subvereniging aan bij K', { type: SUBVERENIGING }, fout(BERICHT.actieveRelatie)),
    ],
    controles: [lijstVan('A', [])],
  },
  {
    id: 'RA41',
    name: 'Toekomstig lidmaatschap bij de koepel -> lidmaatschapsaanvraag 400',
    verenigingen: KOEPEL,
    stappen: [
      alsKoepel(),
      wachtOp('K'),
      lidmaatschap('K', { van: '{{morgen}}' }),
      dienIn('Vraag lidmaatschap aan bij K', {}, fout(BERICHT.toekomstigLidmaatschap)),
    ],
    controles: [lijstVan('A', [])],
  },
  {
    id: 'RA42',
    name: 'Lidmaatschap bij een andere koepel -> subverenigingsaanvraag 400',
    verenigingen: TWEE_KOEPELS,
    stappen: [
      alsKoepel('K'),
      wachtOp('L'),
      lidmaatschap('L'),
      dienIn('Vraag subvereniging aan bij K', { type: SUBVERENIGING }, fout(BERICHT.lidKanGeenSub)),
    ],
    controles: [lijstVan('A', [])],
  },
  {
    id: 'RA43',
    name: 'Subvereniging van de koepel -> lidmaatschaps- en subverenigingsaanvraag 400',
    verenigingen: KOEPEL,
    stappen: [
      alsKoepel(),
      wachtOp('K'),
      subverenigingVan('K'),
      dienIn('Vraag lidmaatschap aan bij K', {}, fout(BERICHT.actieveRelatie)),
      dienIn('Vraag subvereniging aan bij K', { type: SUBVERENIGING }, fout(BERICHT.actieveRelatie)),
    ],
    controles: [lijstVan('A', [])],
  },
  {
    id: 'RA44',
    name: 'Subvereniging van een andere koepel -> lidmaatschaps- en subverenigingsaanvraag 400',
    verenigingen: TWEE_KOEPELS,
    stappen: [
      alsKoepel('K'),
      wachtOp('L'),
      subverenigingVan('L'),
      dienIn('Vraag lidmaatschap aan bij K', {}, fout(BERICHT.subKanGeenLid)),
      dienIn('Vraag subvereniging aan bij K', { type: SUBVERENIGING }, fout(BERICHT.subNietToegestaan)),
    ],
    controles: [lijstVan('A', [])],
  },
  {
    id: 'RA45',
    name: 'Feitelijke vereniging (subtype FV) -> subverenigingsaanvraag 400, lidmaatschapsaanvraag toegestaan',
    verenigingen: KOEPEL,
    stappen: [
      alsKoepel(),
      subtype('FV'),
      dienIn('Vraag subvereniging aan bij K', { type: SUBVERENIGING }, fout(BERICHT.subNietToegestaan)),
      ingediend('RA45', 1, 'Vraag lidmaatschap aan bij K'),
    ],
    controles: [lijstVan('A', [item('RA45', 1, { relatieType: LIDMAATSCHAP })])],
  },
  {
    id: 'RA46',
    name: 'Vereniging met rechtspersoonlijkheid -> subverenigingsaanvraag 400',
    kbo: true,
    verenigingen: KOEPEL,
    stappen: [alsKoepel(), dienIn('Vraag subvereniging aan bij K', { type: SUBVERENIGING }, fout(BERICHT.subNietToegestaan))],
  },
];

const validatieFouten = [
  {
    id: 'RA50',
    name: 'Verplichte velden ontbreken -> 400',
    geenRegistratie: true,
    stappen: [
      dienIn('Dien lege request in', { namens: ONBEKENDE_VCODE, body: {} }, validatie('request', BERICHT.verplichteVelden)),
      dienIn(
        'Dien in zonder relatieType',
        { namens: ONBEKENDE_VCODE, body: { koepelVCode: ONBEKENDE_VCODE, verenigingVCode: 'V9999998' } },
        validatie('request', BERICHT.verplichteVelden)
      ),
    ],
  },
  {
    id: 'RA51',
    name: 'Ongeldig relatieType -> 400',
    verenigingen: KOEPEL,
    stappen: [alsKoepel(), dienIn('Dien in met relatieType Erelid', { type: 'Erelid' }, fout(BERICHT.ongeldigRelatieType))],
    controles: [lijstVan('A', [])],
  },
  {
    id: 'RA52',
    name: 'Ongeldige vCode van koepel of vereniging -> 400',
    stappen: [
      dienIn('Dien in met ongeldige koepelVCode', { koepel: 'K123' }, fout(BERICHT.vCodeFormaat)),
      dienIn('Dien in met ongeldige verenigingVCode', { koepel: 'A', vereniging: 'X123' }, fout(BERICHT.vCodeFormaat)),
    ],
  },
  {
    id: 'RA53',
    name: 'HTML in de beschrijving -> 400',
    verenigingen: KOEPEL,
    stappen: [alsKoepel(), dienIn('Dien in met HTML in de beschrijving', { beschrijving: '<b>Fellowship</b>' }, validatie('beschrijving', BERICHT.html))],
    controles: [lijstVan('A', [])],
  },
];

const opvragen = [
  {
    id: 'RA60',
    name: 'Onbekende relatieaanvraag -> 404',
    geenRegistratie: true,
    stappen: [
      {
        naam: 'Vraag onbekende relatieaanvraag op',
        actie: 'GET',
        beheerPad: `v1/relatieaanvragen/${ONBEKENDE_AANVRAAG}`,
        verwacht: { status: 404, detail: BERICHT.aanvraagNietGevonden },
      },
    ],
  },
  {
    id: 'RA61',
    name: 'Filter op status -> enkel aanvragen met die status, ongeldige status 400',
    verenigingen: KOEPEL,
    stappen: [
      alsKoepel(),
      ingediend('RA61', 1, 'Dien lidmaatschapsaanvraag in'),
      {
        naam: 'Vraag relatieaanvragen op met ongeldige status',
        actie: 'GET',
        beheerPad: 'v1/verenigingen/{{vCodeA}}/relatieaanvragen?status=Onbekend',
        verwacht: fout(BERICHT.ongeldigeStatus),
      },
    ],
    controles: [
      lijstVan('A', [item('RA61')], { status: AANGEVRAAGD }),
      lijstVan('A', [item('RA61')], { status: 'aangevraagd' }),
      lijstVan('A', [], { status: 'Bevestigd' }),
    ],
  },
  {
    id: 'RA62',
    name: 'Vereniging zonder relatieaanvragen -> lege lijst',
    controles: [lijstVan('A', [])],
  },
];

const autorisatie = [
  {
    id: 'RA70',
    name: 'Zonder token -> 401, geen aanvraag',
    verenigingen: KOEPEL,
    stappen: [alsKoepel(), dienIn('Dien in zonder token', {}, { status: 401, title: 'Unauthorized' }, { zonderToken: true })],
    controles: [lijstVan('A', [])],
  },
];

const bevestigen = [
  {
    id: 'RA80',
    name: 'Koepel bevestigt lidmaatschapsaanvraag -> lidmaatschap vanaf vandaag zonder einddatum, subtype FV, Bevestigd',
    verenigingen: KOEPEL,
    stappen: [
      alsKoepel(),
      ingediend('RA80', 1, 'Dien lidmaatschapsaanvraag in namens A'),
      bevestig(
        'Bevestig namens K',
        { id: 'RA80', body: { identificatie: 'LID-0001', beschrijving: 'Lid van de Witte Raad' } },
        { status: 202, locatie: aanvraag('RA80') }
      ),
    ],
    final: {
      beheerDetail: {
        ...lidmaatschappen([lidmaatschapVan('K', { identificatie: 'LID-0001', beschrijving: 'Lid van de Witte Raad' })]),
        props: { verenigingssubtype: { code: 'FV' } },
      },
    },
    historiek: {
      bevat: [gebeurtenis('LidmaatschapWerdToegevoegd'), gebeurtenis('VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging')],
      geen: ['Relatieaanvraag'],
    },
    controles: [
      detail('RA80', { status: BEVESTIGD, bevestiging: { datum: '{{vandaag}}' } }),
      lijstVan('A', [item('RA80', 1, { status: BEVESTIGD })], { status: BEVESTIGD }),
      lijstVan('A', [], { status: AANGEVRAAGD }),
    ],
  },
  {
    id: 'RA81',
    name: 'Vereniging bevestigt subverenigingsaanvraag van de koepel -> subtype SUB van de koepel, Bevestigd',
    verenigingen: KOEPEL,
    stappen: [
      alsKoepel(),
      ingediend('RA81', 1, 'Dien subverenigingsaanvraag in namens K', { type: SUBVERENIGING, namens: 'K' }),
      bevestig('Bevestig namens A', {
        id: 'RA81',
        namens: 'A',
        body: { identificatie: 'SUB-0001', beschrijving: 'Afdeling Rivendel' },
      }),
    ],
    final: {
      beheerDetail: {
        ...lidmaatschappen([]),
        props: {
          verenigingssubtype: { code: 'SUB' },
          subverenigingVan: { andereVereniging: '{{vCodeK}}', identificatie: 'SUB-0001', beschrijving: 'Afdeling Rivendel' },
        },
      },
    },
    historiek: { bevat: [gebeurtenis('VerenigingssubtypeWerdVerfijndNaarSubvereniging')], geen: ['Lidmaatschap'] },
    controles: [detail('RA81', { status: BEVESTIGD, bevestiging: { datum: '{{vandaag}}' } })],
  },
  {
    id: 'RA82',
    name: 'Feitelijke vereniging, zonder relatiegegevens -> lidmaatschap met lege identificatie en beschrijving, subtype blijft FV',
    verenigingen: KOEPEL,
    stappen: [
      alsKoepel(),
      subtype('FV'),
      ingediend('RA82', 1, 'Dien lidmaatschapsaanvraag in'),
      bevestig('Bevestig zonder relatiegegevens', { id: 'RA82' }),
    ],
    final: { beheerDetail: { ...lidmaatschappen([lidmaatschapVan('K')]), props: { verenigingssubtype: { code: 'FV' } } } },
    // FV komt van de subtype-stap, niet nog eens van de bevestiging
    historiek: { bevat: [gebeurtenis('LidmaatschapWerdToegevoegd'), gebeurtenis('VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging')] },
  },
  {
    id: 'RA83',
    name: 'Bevestigen door de aanvrager of door een andere vereniging -> 400, aanvraag blijft open',
    verenigingen: KOEPEL,
    stappen: [
      alsKoepel(),
      ingediend('RA83', 1, 'Dien lidmaatschapsaanvraag in namens A'),
      bevestig('Bevestig namens A (de aanvrager)', { id: 'RA83', namens: 'A' }, fout(BERICHT.nietDeOntvanger)),
      bevestig('Bevestig namens een andere vereniging', { id: 'RA83', namens: ONBEKENDE_VCODE }, fout(BERICHT.nietDeOntvanger)),
    ],
    final: GEEN_LIDMAATSCHAP,
    controles: [nogOpen('RA83')],
  },
  {
    id: 'RA84',
    name: 'Al bevestigd -> opnieuw bevestigen 400, één lidmaatschap',
    verenigingen: KOEPEL,
    stappen: [
      alsKoepel(),
      ingediend('RA84', 1, 'Dien lidmaatschapsaanvraag in'),
      bevestig('Bevestig namens K', { id: 'RA84' }),
      bevestig('Bevestig opnieuw namens K', { id: 'RA84' }, fout(BERICHT.alBevestigd)),
    ],
    final: { beheerDetail: lidmaatschappen([lidmaatschapVan('K')]) },
    historiek: { bevat: [gebeurtenis('LidmaatschapWerdToegevoegd')] },
  },
  {
    id: 'RA85',
    name: 'Na bevestiging -> nieuwe aanvraag voor hetzelfde paar 400 door de relatie, niet omdat ze nog open staat',
    verenigingen: KOEPEL,
    stappen: [
      alsKoepel(),
      ingediend('RA85', 1, 'Dien lidmaatschapsaanvraag in'),
      bevestig('Bevestig namens K', { id: 'RA85' }),
      dienIn('Dien opnieuw een lidmaatschapsaanvraag in namens K', { namens: 'K' }, fout(BERICHT.actieveRelatie)),
    ],
    controles: [lijstVan('A', [item('RA85', 1, { status: BEVESTIGD })])],
  },
  {
    id: 'RA86',
    name: 'Koepel intussen niet meer als koepel geïdentificeerd -> bevestigen 400, aanvraag blijft open',
    verenigingen: KOEPEL,
    stappen: [
      alsKoepel(),
      ingediend('RA86', 1, 'Dien lidmaatschapsaanvraag in'),
      nietAlsKoepel(),
      bevestig('Bevestig namens K', { id: 'RA86' }, fout(BERICHT.geenKoepel)),
    ],
    final: GEEN_LIDMAATSCHAP,
    controles: [nogOpen('RA86')],
  },
  {
    id: 'RA87',
    name: 'Lidmaatschap intussen rechtstreeks toegevoegd -> bevestigen 400, aanvraag blijft open',
    verenigingen: KOEPEL,
    stappen: [
      alsKoepel(),
      wachtOp('K'),
      ingediend('RA87', 1, 'Dien lidmaatschapsaanvraag in'),
      lidmaatschap('K'),
      bevestig('Bevestig namens K', { id: 'RA87' }, fout(BERICHT.actieveRelatie)),
    ],
    historiek: { bevat: [gebeurtenis('LidmaatschapWerdToegevoegd')] },
    controles: [nogOpen('RA87')],
  },
  {
    id: 'RA88',
    name: 'Subverenigingsaanvraag, intussen lid van een andere koepel -> bevestigen 400, aanvraag blijft open',
    verenigingen: TWEE_KOEPELS,
    stappen: [
      alsKoepel('K'),
      wachtOp('L'),
      ingediend('RA88', 1, 'Dien subverenigingsaanvraag bij K in', { type: SUBVERENIGING }),
      lidmaatschap('L'),
      bevestig('Bevestig namens K', { id: 'RA88' }, fout(BERICHT.lidKanGeenSub)),
    ],
    final: { beheerDetail: { propsNiet: { verenigingssubtype: { code: 'SUB' } } } },
    controles: [nogOpen('RA88')],
  },
  {
    id: 'RA89',
    name: 'Verouderde If-Match -> 412, actuele If-Match -> bevestigd',
    verenigingen: KOEPEL,
    stappen: [
      alsKoepel(),
      dienIn('Dien lidmaatschapsaanvraag in', {}, ok(), { bewaarId: aanvraagVar('RA89'), bewaarHeader: { ETag: 'etagRA89' } }),
      bevestig('Bevestig met een te hoge If-Match', { id: 'RA89' }, { status: 412 }, { headers: { 'vr-namens': '{{vCodeK}}', 'If-Match': 'W/"9"' } }),
      bevestig('Bevestig met de actuele If-Match', { id: 'RA89' }, ok(), { headers: { 'vr-namens': '{{vCodeK}}', 'If-Match': '{{etagRA89}}' } }),
    ],
    final: { beheerDetail: lidmaatschappen([lidmaatschapVan('K')]) },
    controles: [detail('RA89', { status: BEVESTIGD })],
  },
];

const bevestigenValidatie = [
  {
    id: 'RA90',
    name: 'Ongeldige aanvraagId -> 400',
    geenRegistratie: true,
    stappen: [bevestig('Bevestig met een vCode als aanvraagId', { aanvraagId: 'V0001001', namens: ONBEKENDE_VCODE }, fout(BERICHT.aanvraagIdFormaat))],
  },
  {
    id: 'RA91',
    name: 'Onbekende relatieaanvraag -> 400',
    geenRegistratie: true,
    stappen: [bevestig('Bevestig een onbekende aanvraag', { aanvraagId: ONBEKENDE_AANVRAAG, namens: ONBEKENDE_VCODE }, fout(BERICHT.aanvraagOnbekend))],
  },
  {
    id: 'RA92',
    name: 'Zonder of met ongeldige vr-namens -> 400',
    geenRegistratie: true,
    stappen: [
      bevestig('Bevestig zonder vr-namens', { aanvraagId: ONBEKENDE_AANVRAAG, namens: null }, fout(BERICHT.vrNamensVerplicht)),
      bevestig('Bevestig met ongeldige vr-namens', { aanvraagId: ONBEKENDE_AANVRAAG, namens: 'ongeldig' }, fout(BERICHT.vCodeFormaat)),
    ],
  },
  {
    id: 'RA93',
    name: 'HTML in identificatie of beschrijving -> 400',
    geenRegistratie: true,
    stappen: [
      bevestig(
        'Bevestig met HTML in de identificatie',
        { aanvraagId: ONBEKENDE_AANVRAAG, namens: ONBEKENDE_VCODE, body: { identificatie: '<b>LID</b>' } },
        validatie('identificatie', BERICHT.html)
      ),
      bevestig(
        'Bevestig met HTML in de beschrijving',
        { aanvraagId: ONBEKENDE_AANVRAAG, namens: ONBEKENDE_VCODE, body: { beschrijving: '<b>Fellowship</b>' } },
        validatie('beschrijving', BERICHT.html)
      ),
    ],
  },
  {
    id: 'RA94',
    name: 'Zonder token -> 401, aanvraag blijft open',
    verenigingen: KOEPEL,
    stappen: [
      alsKoepel(),
      ingediend('RA94', 1, 'Dien lidmaatschapsaanvraag in'),
      bevestig('Bevestig zonder token', { id: 'RA94' }, { status: 401, title: 'Unauthorized' }, { zonderToken: true }),
    ],
    final: GEEN_LIDMAATSCHAP,
    controles: [nogOpen('RA94')],
  },
];

const REDEN_WEIGERING = 'Wij blijven onafhankelijk van de Witte Raad.';

const weigeren = [
  {
    id: 'RA100',
    name: 'Vereniging weigert lidmaatschapsaanvraag van de koepel -> Geweigerd met reden, geen relatie',
    verenigingen: KOEPEL,
    stappen: [
      alsKoepel(),
      ingediend('RA100', 1, 'Dien lidmaatschapsaanvraag in namens K', { namens: 'K' }),
      weiger('Weiger namens A', { id: 'RA100', namens: 'A', body: { reden: REDEN_WEIGERING } }, { status: 202, locatie: aanvraag('RA100') }),
    ],
    final: { beheerDetail: { ...lidmaatschappen([]), propsNiet: { verenigingssubtype: { code: 'FV' } } } },
    historiek: { geen: ['Relatieaanvraag', 'Lidmaatschap'] },
    controles: [
      detail('RA100', { status: GEWEIGERD, weigering: { datum: '{{vandaag}}', reden: REDEN_WEIGERING } }),
      lijstVan('A', [item('RA100', 1, { status: GEWEIGERD })], { status: GEWEIGERD }),
      lijstVan('A', [], { status: AANGEVRAAGD }),
    ],
  },
  {
    id: 'RA101',
    name: 'Koepel weigert subverenigingsaanvraag zonder reden -> Geweigerd met lege reden, geen subvereniging',
    verenigingen: KOEPEL,
    stappen: [
      alsKoepel(),
      ingediend('RA101', 1, 'Dien subverenigingsaanvraag in namens A', { type: SUBVERENIGING }),
      weiger('Weiger namens K zonder reden', { id: 'RA101' }),
    ],
    final: { beheerDetail: { propsNiet: { verenigingssubtype: { code: 'SUB' } } } },
    controles: [detail('RA101', { status: GEWEIGERD, weigering: { datum: '{{vandaag}}', reden: '' } })],
  },
  {
    id: 'RA102',
    name: 'Na weigering -> nieuwe aanvraag voor hetzelfde paar kan opnieuw',
    verenigingen: KOEPEL,
    stappen: [
      alsKoepel(),
      ingediend('RA102', 1, 'Dien lidmaatschapsaanvraag in'),
      weiger('Weiger namens K', { id: 'RA102' }),
      ingediend('RA102', 2, 'Dien opnieuw een lidmaatschapsaanvraag in'),
    ],
    controles: [lijstVan('A', [item('RA102', 1, { status: GEWEIGERD }), item('RA102', 2)])],
  },
  {
    id: 'RA103',
    name: 'Na weigering van een subverenigingsaanvraag -> subverenigingsaanvraag bij een andere koepel kan (K7)',
    verenigingen: TWEE_KOEPELS,
    stappen: [
      alsKoepel('K'),
      alsKoepel('L'),
      ingediend('RA103', 1, 'Dien subverenigingsaanvraag bij K in', { type: SUBVERENIGING }),
      weiger('Weiger namens K', { id: 'RA103' }),
      ingediend('RA103', 2, 'Dien subverenigingsaanvraag bij L in', { koepel: 'L', type: SUBVERENIGING }),
    ],
    controles: [
      lijstVan('A', [item('RA103', 1, { koepelVCode: '{{vCodeK}}', status: GEWEIGERD }), item('RA103', 2, { koepelVCode: '{{vCodeL}}' })]),
    ],
  },
  {
    id: 'RA104',
    name: 'Weigeren door de aanvrager of door een andere vereniging -> 400, aanvraag blijft open',
    verenigingen: KOEPEL,
    stappen: [
      alsKoepel(),
      ingediend('RA104', 1, 'Dien lidmaatschapsaanvraag in namens A'),
      weiger('Weiger namens A (de aanvrager)', { id: 'RA104', namens: 'A' }, fout(BERICHT.nietDeOntvanger)),
      weiger('Weiger namens een andere vereniging', { id: 'RA104', namens: ONBEKENDE_VCODE }, fout(BERICHT.nietDeOntvanger)),
    ],
    controles: [nogOpen('RA104')],
  },
  {
    id: 'RA105',
    name: 'Al geweigerd -> opnieuw weigeren of bevestigen 400, geen relatie',
    verenigingen: KOEPEL,
    stappen: [
      alsKoepel(),
      ingediend('RA105', 1, 'Dien lidmaatschapsaanvraag in'),
      weiger('Weiger namens K', { id: 'RA105' }),
      weiger('Weiger opnieuw namens K', { id: 'RA105' }, fout(BERICHT.alGeweigerd)),
      bevestig('Bevestig na de weigering', { id: 'RA105' }, fout(BERICHT.alGeweigerd)),
    ],
    final: GEEN_LIDMAATSCHAP,
    controles: [detail('RA105', { status: GEWEIGERD })],
  },
  {
    id: 'RA106',
    name: 'Al bevestigd -> weigeren 400, lidmaatschap blijft',
    verenigingen: KOEPEL,
    stappen: [
      alsKoepel(),
      ingediend('RA106', 1, 'Dien lidmaatschapsaanvraag in'),
      bevestig('Bevestig namens K', { id: 'RA106' }),
      weiger('Weiger na de bevestiging', { id: 'RA106' }, fout(BERICHT.alBevestigd)),
    ],
    final: { beheerDetail: lidmaatschappen([lidmaatschapVan('K')]) },
    controles: [detail('RA106', { status: BEVESTIGD })],
  },
  {
    id: 'RA107',
    name: 'Koepel intussen niet meer als koepel geïdentificeerd -> weigeren kan nog',
    verenigingen: KOEPEL,
    stappen: [
      alsKoepel(),
      ingediend('RA107', 1, 'Dien lidmaatschapsaanvraag in'),
      nietAlsKoepel(),
      weiger('Weiger namens K', { id: 'RA107' }),
    ],
    controles: [detail('RA107', { status: GEWEIGERD })],
  },
  {
    id: 'RA108',
    name: 'Verouderde If-Match -> 412, actuele If-Match -> geweigerd',
    verenigingen: KOEPEL,
    stappen: [
      alsKoepel(),
      dienIn('Dien lidmaatschapsaanvraag in', {}, ok(), { bewaarId: aanvraagVar('RA108'), bewaarHeader: { ETag: 'etagRA108' } }),
      weiger('Weiger met een te hoge If-Match', { id: 'RA108' }, { status: 412 }, { headers: { 'vr-namens': '{{vCodeK}}', 'If-Match': 'W/"9"' } }),
      weiger('Weiger met de actuele If-Match', { id: 'RA108' }, ok(), { headers: { 'vr-namens': '{{vCodeK}}', 'If-Match': '{{etagRA108}}' } }),
    ],
    controles: [detail('RA108', { status: GEWEIGERD })],
  },
];

const weigerenValidatie = [
  {
    id: 'RA110',
    name: 'Ongeldige aanvraagId -> 400',
    geenRegistratie: true,
    stappen: [weiger('Weiger met een vCode als aanvraagId', { aanvraagId: 'V0001001', namens: ONBEKENDE_VCODE }, fout(BERICHT.aanvraagIdFormaat))],
  },
  {
    id: 'RA111',
    name: 'Onbekende relatieaanvraag -> 400',
    geenRegistratie: true,
    stappen: [weiger('Weiger een onbekende aanvraag', { aanvraagId: ONBEKENDE_AANVRAAG, namens: ONBEKENDE_VCODE }, fout(BERICHT.aanvraagOnbekend))],
  },
  {
    id: 'RA112',
    name: 'Zonder of met ongeldige vr-namens -> 400',
    geenRegistratie: true,
    stappen: [
      weiger('Weiger zonder vr-namens', { aanvraagId: ONBEKENDE_AANVRAAG, namens: null }, fout(BERICHT.vrNamensVerplicht)),
      weiger('Weiger met ongeldige vr-namens', { aanvraagId: ONBEKENDE_AANVRAAG, namens: 'ongeldig' }, fout(BERICHT.vCodeFormaat)),
    ],
  },
  {
    id: 'RA113',
    name: 'HTML in de reden -> 400',
    geenRegistratie: true,
    stappen: [
      weiger(
        'Weiger met HTML in de reden',
        { aanvraagId: ONBEKENDE_AANVRAAG, namens: ONBEKENDE_VCODE, body: { reden: '<b>Mordor</b>' } },
        validatie('reden', BERICHT.html)
      ),
    ],
  },
  {
    id: 'RA114',
    name: 'Zonder token -> 401, aanvraag blijft open',
    verenigingen: KOEPEL,
    stappen: [
      alsKoepel(),
      ingediend('RA114', 1, 'Dien lidmaatschapsaanvraag in'),
      weiger('Weiger zonder token', { id: 'RA114' }, { status: 401, title: 'Unauthorized' }, { zonderToken: true }),
    ],
    controles: [nogOpen('RA114')],
  },
];

const REDEN_INTREKKING = 'Per vergissing ingediend door Pippin.';

const intrekken = [
  {
    id: 'RA120',
    name: 'Koepel trekt lidmaatschapsaanvraag in -> Ingetrokken met reden, geen relatie',
    verenigingen: KOEPEL,
    stappen: [
      alsKoepel(),
      ingediend('RA120', 1, 'Dien lidmaatschapsaanvraag in namens K', { namens: 'K' }),
      trekIn('Trek in namens K', { id: 'RA120', body: { reden: REDEN_INTREKKING } }, { status: 202, locatie: aanvraag('RA120') }),
    ],
    final: { beheerDetail: { ...lidmaatschappen([]), propsNiet: { verenigingssubtype: { code: 'FV' } } } },
    historiek: { geen: ['Relatieaanvraag', 'Lidmaatschap'] },
    controles: [
      detail('RA120', { status: INGETROKKEN, intrekking: { datum: '{{vandaag}}', reden: REDEN_INTREKKING } }),
      lijstVan('A', [item('RA120', 1, { status: INGETROKKEN })], { status: INGETROKKEN }),
      lijstVan('A', [], { status: AANGEVRAAGD }),
    ],
  },
  {
    id: 'RA121',
    name: 'Vereniging trekt subverenigingsaanvraag in zonder reden -> Ingetrokken met lege reden, geen subvereniging',
    verenigingen: KOEPEL,
    stappen: [
      alsKoepel(),
      ingediend('RA121', 1, 'Dien subverenigingsaanvraag in namens A', { type: SUBVERENIGING }),
      trekIn('Trek in namens A zonder reden', { id: 'RA121', namens: 'A' }),
    ],
    final: { beheerDetail: { propsNiet: { verenigingssubtype: { code: 'SUB' } } } },
    controles: [detail('RA121', { status: INGETROKKEN, intrekking: { datum: '{{vandaag}}', reden: '' } })],
  },
  {
    id: 'RA122',
    name: 'Na intrekking -> nieuwe aanvraag voor hetzelfde paar kan opnieuw',
    verenigingen: KOEPEL,
    stappen: [
      alsKoepel(),
      ingediend('RA122', 1, 'Dien lidmaatschapsaanvraag in'),
      trekIn('Trek in namens A', { id: 'RA122', namens: 'A' }),
      ingediend('RA122', 2, 'Dien opnieuw een lidmaatschapsaanvraag in'),
    ],
    controles: [lijstVan('A', [item('RA122', 1, { status: INGETROKKEN }), item('RA122', 2)])],
  },
  {
    id: 'RA123',
    name: 'Na intrekking van een subverenigingsaanvraag -> subverenigingsaanvraag bij een andere koepel kan (K7)',
    verenigingen: TWEE_KOEPELS,
    stappen: [
      alsKoepel('K'),
      alsKoepel('L'),
      ingediend('RA123', 1, 'Dien subverenigingsaanvraag bij K in', { type: SUBVERENIGING }),
      trekIn('Trek in namens A', { id: 'RA123', namens: 'A' }),
      ingediend('RA123', 2, 'Dien subverenigingsaanvraag bij L in', { koepel: 'L', type: SUBVERENIGING }),
    ],
    controles: [
      lijstVan('A', [item('RA123', 1, { koepelVCode: '{{vCodeK}}', status: INGETROKKEN }), item('RA123', 2, { koepelVCode: '{{vCodeL}}' })]),
    ],
  },
  {
    id: 'RA124',
    name: 'Intrekken door de ontvanger of door een andere vereniging -> 400, aanvraag blijft open',
    verenigingen: KOEPEL,
    stappen: [
      alsKoepel(),
      ingediend('RA124', 1, 'Dien lidmaatschapsaanvraag in namens A'),
      trekIn('Trek in namens K (de ontvanger)', { id: 'RA124' }, fout(BERICHT.nietDeAanvrager)),
      trekIn('Trek in namens een andere vereniging', { id: 'RA124', namens: ONBEKENDE_VCODE }, fout(BERICHT.nietDeAanvrager)),
    ],
    controles: [nogOpen('RA124')],
  },
  {
    id: 'RA125',
    name: 'Al ingetrokken -> opnieuw intrekken, bevestigen of weigeren 400, geen relatie',
    verenigingen: KOEPEL,
    stappen: [
      alsKoepel(),
      ingediend('RA125', 1, 'Dien lidmaatschapsaanvraag in'),
      trekIn('Trek in namens A', { id: 'RA125', namens: 'A' }),
      trekIn('Trek opnieuw in namens A', { id: 'RA125', namens: 'A' }, fout(BERICHT.alIngetrokken)),
      bevestig('Bevestig na de intrekking', { id: 'RA125' }, fout(BERICHT.alIngetrokken)),
      weiger('Weiger na de intrekking', { id: 'RA125' }, fout(BERICHT.alIngetrokken)),
    ],
    final: GEEN_LIDMAATSCHAP,
    controles: [detail('RA125', { status: INGETROKKEN })],
  },
  {
    id: 'RA126',
    name: 'Al bevestigd -> intrekken 400, lidmaatschap blijft',
    verenigingen: KOEPEL,
    stappen: [
      alsKoepel(),
      ingediend('RA126', 1, 'Dien lidmaatschapsaanvraag in'),
      bevestig('Bevestig namens K', { id: 'RA126' }),
      trekIn('Trek in na de bevestiging', { id: 'RA126', namens: 'A' }, fout(BERICHT.alBevestigd)),
    ],
    final: { beheerDetail: lidmaatschappen([lidmaatschapVan('K')]) },
    controles: [detail('RA126', { status: BEVESTIGD })],
  },
  {
    id: 'RA127',
    name: 'Al geweigerd -> intrekken 400',
    verenigingen: KOEPEL,
    stappen: [
      alsKoepel(),
      ingediend('RA127', 1, 'Dien lidmaatschapsaanvraag in'),
      weiger('Weiger namens K', { id: 'RA127' }),
      trekIn('Trek in na de weigering', { id: 'RA127', namens: 'A' }, fout(BERICHT.alGeweigerd)),
    ],
    controles: [detail('RA127', { status: GEWEIGERD })],
  },
  {
    id: 'RA128',
    name: 'Verouderde If-Match -> 412, actuele If-Match -> ingetrokken',
    verenigingen: KOEPEL,
    stappen: [
      alsKoepel(),
      dienIn('Dien lidmaatschapsaanvraag in', {}, ok(), { bewaarId: aanvraagVar('RA128'), bewaarHeader: { ETag: 'etagRA128' } }),
      trekIn('Trek in met een te hoge If-Match', { id: 'RA128' }, { status: 412 }, { headers: { 'vr-namens': '{{vCodeA}}', 'If-Match': 'W/"9"' } }),
      trekIn('Trek in met de actuele If-Match', { id: 'RA128' }, ok(), { headers: { 'vr-namens': '{{vCodeA}}', 'If-Match': '{{etagRA128}}' } }),
    ],
    controles: [detail('RA128', { status: INGETROKKEN })],
  },
];

const intrekkenValidatie = [
  {
    id: 'RA130',
    name: 'Ongeldige aanvraagId -> 400',
    geenRegistratie: true,
    stappen: [trekIn('Trek in met een vCode als aanvraagId', { aanvraagId: 'V0001001', namens: ONBEKENDE_VCODE }, fout(BERICHT.aanvraagIdFormaat))],
  },
  {
    id: 'RA131',
    name: 'Onbekende relatieaanvraag -> 400',
    geenRegistratie: true,
    stappen: [trekIn('Trek een onbekende aanvraag in', { aanvraagId: ONBEKENDE_AANVRAAG, namens: ONBEKENDE_VCODE }, fout(BERICHT.aanvraagOnbekend))],
  },
  {
    id: 'RA132',
    name: 'Zonder of met ongeldige vr-namens -> 400',
    geenRegistratie: true,
    stappen: [
      trekIn('Trek in zonder vr-namens', { aanvraagId: ONBEKENDE_AANVRAAG, namens: null }, fout(BERICHT.vrNamensVerplicht)),
      trekIn('Trek in met ongeldige vr-namens', { aanvraagId: ONBEKENDE_AANVRAAG, namens: 'ongeldig' }, fout(BERICHT.vCodeFormaat)),
    ],
  },
  {
    id: 'RA133',
    name: 'HTML in de reden -> 400',
    geenRegistratie: true,
    stappen: [
      trekIn(
        'Trek in met HTML in de reden',
        { aanvraagId: ONBEKENDE_AANVRAAG, namens: ONBEKENDE_VCODE, body: { reden: '<b>Isengard</b>' } },
        validatie('reden', BERICHT.html)
      ),
    ],
  },
  {
    id: 'RA134',
    name: 'Zonder token -> 401, aanvraag blijft open',
    verenigingen: KOEPEL,
    stappen: [
      alsKoepel(),
      ingediend('RA134', 1, 'Dien lidmaatschapsaanvraag in'),
      trekIn('Trek in zonder token', { id: 'RA134', namens: 'A' }, { status: 401, title: 'Unauthorized' }, { zonderToken: true }),
    ],
    controles: [nogOpen('RA134')],
  },
];

module.exports = {
  name: 'Relatieaanvragen',
  description:
    'Relatieaanvragen tussen een koepel en een vereniging (POST /v1/relatieaanvragen met vr-namens): een lidmaatschap ' +
    'of een subvereniging, ingediend door de koepel of de vereniging. Opvragen via GET /v1/relatieaanvragen/{aanvraagId} ' +
    'en GET /v1/verenigingen/{vCode}/relatieaanvragen. Een aanvraag wijzigt niets aan de verenigingen. Bevestigen via ' +
    'POST /v1/relatieaanvragen/{aanvraagId}/bevestiging door de ontvanger: dan ontstaat het lidmaatschap of de ' +
    'subvereniging op de vereniging. Weigeren via POST /v1/relatieaanvragen/{aanvraagId}/weigering door de ontvanger, ' +
    'met een optionele reden: er ontstaat geen relatie. Intrekken via POST /v1/relatieaanvragen/{aanvraagId}/intrekking ' +
    'door de aanvrager, met een optionele reden: er ontstaat geen relatie.',
  setup: SETUP_DATUMS,
  groepen: [
    { name: 'Indienen', scenarios: indienen },
    { name: 'Partijen', scenarios: partijen },
    { name: 'Beoogde koepel', scenarios: koepel },
    { name: 'Open aanvragen', scenarios: openAanvragen },
    { name: 'Bestaande relaties', scenarios: bestaandeRelaties },
    { name: 'Validatie', scenarios: validatieFouten },
    { name: 'Opvragen', scenarios: opvragen },
    { name: 'Autorisatie', scenarios: autorisatie },
    { name: 'Bevestigen', scenarios: bevestigen },
    { name: 'Bevestigen - validatie', scenarios: bevestigenValidatie },
    { name: 'Weigeren', scenarios: weigeren },
    { name: 'Weigeren - validatie', scenarios: weigerenValidatie },
    { name: 'Intrekken', scenarios: intrekken },
    { name: 'Intrekken - validatie', scenarios: intrekkenValidatie },
  ],
};
