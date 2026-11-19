// Koepels
//
// Startversie: de collectie wordt verder onderhouden in de Postman workspace (de bron van waarheid).
// Dit bestand wordt niet bijgewerkt. Opnieuw genereren overschrijft wat er sindsdien in Postman is aangepast.
//
// Rules: WijzigKoepelgegevensRequestValidator (request niet leeg, geldige koepelstatus, hoofdletterongevoelig),
// VerenigingOfAnyKind.WijzigKoepelgegevens (enkel een vereniging met rechtspersoonlijkheid kan koepel zijn, enkel een
// koepel kan de privacyverklaring accepteren, GeenKoepel of NietBepaald trekt een geaccepteerde privacyverklaring in,
// zelfde waarden -> 200 zonder gebeurtenis). Koepelgegevens zijn enkel zichtbaar in beheer detail en historiek.
//
// Op Test wordt een bestaande vereniging met rechtspersoonlijkheid hergebruikt (kboModus 'bestaand'): die scenario's
// zetten de koepelstatus eerst terug op NietBepaald (200 of 202) en controleren de historiek enkel met bevat (een
// gebeurtenis kan ook van een vorige run zijn). Een vereniging zonder rechtspersoonlijkheid is altijd nieuw: daar
// worden de gebeurtenissen exact geteld.

// ================================================================ test data

const KOEPEL = 'Koepel';
const GEEN_KOEPEL = 'GeenKoepel';
const NIET_BEPAALD = 'NietBepaald';
const ONBEKENDE_VCODE = 'V9999999';

const BERICHT = {
  enkelRechtspersoonlijkheid: 'Enkel een vereniging met rechtspersoonlijkheid kan als koepel geïdentificeerd worden.',
  privacyEnkelKoepel: 'De privacyverklaring kan enkel door een koepelvereniging geaccepteerd worden.',
  leegRequest: 'Een request mag niet leeg zijn.',
  ongeldigeKoepelstatus: 'De koepelstatus moet een geldige waarde hebben.',
  verenigingNietGevonden: `De resource met id '${ONBEKENDE_VCODE}' werd niet gevonden.`,
};

// ================================================================ building blocks

const ok = (status = 202) => ({ status });
const fout = (detail) => ({ status: 400, detail });
const validatie = (veld, reden) => ({ status: 400, validatie: { veld, reden } });

// actie op /v1/verenigingen/{vCode}/koepelgegevens
const wijzig = (naam, body, verwacht = ok(), extra = {}) => ({ naam, actie: 'PATCH', pad: 'koepelgegevens', body, verwacht, ...extra });

const ALS_KOEPEL = { koepelstatus: KOEPEL };
const ALS_GEEN_KOEPEL = { koepelstatus: GEEN_KOEPEL };
const ALS_NIET_BEPAALD = { koepelstatus: NIET_BEPAALD };
const KOEPEL_MET_PRIVACY = { koepelstatus: KOEPEL, privacyVerklaringGeaccepteerd: true };
const PRIVACY_ACCEPTEREN = { privacyVerklaringGeaccepteerd: true };
const PRIVACY_INTREKKEN = { privacyVerklaringGeaccepteerd: false };

// Op Test is de vereniging met rechtspersoonlijkheid hergebruikt: eerst naar een gekende begintoestand.
const beginToestand = () =>
  wijzig('Begintoestand: koepelstatus NietBepaald (hergebruikte vereniging op Test)', ALS_NIET_BEPAALD, { status: [200, 202] });
const koepelMetPrivacy = () => wijzig('Identificeer als koepel en accepteer privacyverklaring', KOEPEL_MET_PRIVACY);

// beheer detail
const detail = (koepelstatus, privacyVerklaringGeaccepteerd) => ({
  beheerDetail: { props: { koepelgegevens: { koepelstatus, privacyVerklaringGeaccepteerd } } },
});
const NIET_GEWIJZIGD = detail(NIET_BEPAALD, false);

// historiek (aantal: exact aantal; zonder aantal: minstens één)
const gebeurtenis = (naam, beschrijving) => (aantal) => ({ gebeurtenis: naam, beschrijving, ...(aantal === undefined ? {} : { aantal }) });
const alsKoepel = gebeurtenis('VerenigingWerdGeidentificeerdAlsKoepel', 'Vereniging werd geïdentificeerd als koepel.');
const alsGeenKoepel = gebeurtenis('VerenigingWerdGeidentificeerdAlsGeenKoepel', 'Vereniging werd geïdentificeerd als niet-koepel.');
const teruggezet = gebeurtenis('KoepelstatusWerdTerugGezetNaarNietBepaald', 'Koepelstatus werd teruggezet naar niet bepaald.');
const privacyGeaccepteerd = gebeurtenis('KoepelPrivacyverklaringWerdGeaccepteerd', 'Privacyverklaring voor koepels werd geaccepteerd.');
const privacyIngetrokken = gebeurtenis('KoepelPrivacyverklaringWerdIngetrokken', 'Privacyverklaring voor koepels werd ingetrokken.');
const GEEN_KOEPELGEBEURTENISSEN = { geen: ['VerenigingWerdGeidentificeerdAls', 'KoepelstatusWerd', 'KoepelPrivacyverklaring'] };

// ================================================================ scenarios

const nieuweVereniging = [
  {
    id: 'KP1',
    name: 'Nieuwe vereniging -> koepelstatus NietBepaald, privacyverklaring niet geaccepteerd',
    final: NIET_GEWIJZIGD,
    historiek: GEEN_KOEPELGEBEURTENISSEN,
  },
];

const metRechtspersoonlijkheid = [
  {
    id: 'KP10',
    name: 'Koepel -> geïdentificeerd als koepel, privacyverklaring nog niet geaccepteerd',
    kbo: true,
    stappen: [beginToestand(), wijzig('Identificeer als koepel', ALS_KOEPEL)],
    final: detail(KOEPEL, false),
    historiek: { bevat: [alsKoepel()] },
  },
  {
    id: 'KP11',
    name: 'Koepel en privacyverklaring in één request -> beide vastgelegd',
    kbo: true,
    stappen: [beginToestand(), koepelMetPrivacy()],
    final: detail(KOEPEL, true),
    historiek: { bevat: [alsKoepel(), privacyGeaccepteerd()] },
  },
  {
    id: 'KP12',
    name: 'Koepel accepteert privacyverklaring later -> geaccepteerd, blijft koepel',
    kbo: true,
    stappen: [beginToestand(), wijzig('Identificeer als koepel', ALS_KOEPEL), wijzig('Accepteer privacyverklaring', PRIVACY_ACCEPTEREN)],
    final: detail(KOEPEL, true),
    historiek: { bevat: [alsKoepel(), privacyGeaccepteerd()] },
  },
  {
    id: 'KP13',
    name: 'Koepel trekt privacyverklaring in -> ingetrokken, blijft koepel',
    kbo: true,
    stappen: [beginToestand(), koepelMetPrivacy(), wijzig('Trek privacyverklaring in', PRIVACY_INTREKKEN)],
    final: detail(KOEPEL, false),
    historiek: { bevat: [privacyGeaccepteerd(), privacyIngetrokken()] },
  },
  {
    id: 'KP14',
    name: 'Koepel met privacyverklaring wordt GeenKoepel -> privacyverklaring ingetrokken',
    kbo: true,
    stappen: [beginToestand(), koepelMetPrivacy(), wijzig('Identificeer als geen koepel', ALS_GEEN_KOEPEL)],
    final: detail(GEEN_KOEPEL, false),
    historiek: { bevat: [alsGeenKoepel(), privacyIngetrokken()] },
  },
  {
    id: 'KP15',
    name: 'Koepel met privacyverklaring terug naar NietBepaald -> teruggezet, privacyverklaring ingetrokken',
    kbo: true,
    stappen: [beginToestand(), koepelMetPrivacy(), wijzig('Zet koepelstatus terug op NietBepaald', ALS_NIET_BEPAALD)],
    final: NIET_GEWIJZIGD,
    historiek: { bevat: [teruggezet(), privacyIngetrokken()] },
  },
  {
    id: 'KP16',
    name: 'GeenKoepel -> geïdentificeerd als niet-koepel',
    kbo: true,
    stappen: [beginToestand(), wijzig('Identificeer als geen koepel', ALS_GEEN_KOEPEL)],
    final: detail(GEEN_KOEPEL, false),
    historiek: { bevat: [alsGeenKoepel()] },
  },
  {
    id: 'KP17',
    name: 'Zelfde waarden opnieuw -> 200, geen wijziging',
    kbo: true,
    stappen: [
      beginToestand(),
      koepelMetPrivacy(),
      wijzig('Zelfde koepelstatus en privacyverklaring opnieuw', KOEPEL_MET_PRIVACY, ok(200)),
      wijzig('Zelfde koepelstatus opnieuw', ALS_KOEPEL, ok(200)),
      wijzig('Privacyverklaring opnieuw accepteren', PRIVACY_ACCEPTEREN, ok(200)),
    ],
    final: detail(KOEPEL, true),
  },
  {
    id: 'KP18',
    name: 'Koepelstatus in een andere schrijfwijze (koepel, KOEPEL) -> aanvaard, bewaard als Koepel',
    kbo: true,
    stappen: [
      beginToestand(),
      wijzig('Identificeer als koepel in kleine letters', { koepelstatus: 'koepel' }),
      wijzig('Identificeer opnieuw als KOEPEL in hoofdletters', { koepelstatus: 'KOEPEL' }, ok(200)),
    ],
    final: detail(KOEPEL, false),
    historiek: { bevat: [alsKoepel()] },
  },
];

// Een vereniging zonder rechtspersoonlijkheid is altijd nieuw: gebeurtenissen exact geteld.
const zonderRechtspersoonlijkheid = [
  {
    id: 'KP30',
    name: 'Vereniging zonder rechtspersoonlijkheid als GeenKoepel -> toegestaan, opnieuw -> 200',
    stappen: [wijzig('Identificeer als geen koepel', ALS_GEEN_KOEPEL), wijzig('Identificeer opnieuw als geen koepel', ALS_GEEN_KOEPEL, ok(200))],
    final: detail(GEEN_KOEPEL, false),
    historiek: { bevat: [alsGeenKoepel(1)], geen: ['VerenigingWerdGeidentificeerdAlsKoepel', 'KoepelstatusWerd', 'KoepelPrivacyverklaring'] },
  },
  {
    id: 'KP31',
    name: 'GeenKoepel terug naar NietBepaald -> teruggezet, geen privacyverklaring in te trekken',
    stappen: [wijzig('Identificeer als geen koepel', ALS_GEEN_KOEPEL), wijzig('Zet koepelstatus terug op NietBepaald', ALS_NIET_BEPAALD)],
    final: NIET_GEWIJZIGD,
    historiek: { bevat: [alsGeenKoepel(1), teruggezet(1)], geen: ['KoepelPrivacyverklaring'] },
  },
  {
    id: 'KP32',
    name: 'Niets te wijzigen (NietBepaald, privacyverklaring intrekken) -> 200, geen gebeurtenis',
    stappen: [
      wijzig('Zet koepelstatus op NietBepaald', ALS_NIET_BEPAALD, ok(200)),
      wijzig('Trek privacyverklaring in', PRIVACY_INTREKKEN, ok(200)),
      wijzig('NietBepaald zonder privacyverklaring', { koepelstatus: NIET_BEPAALD, privacyVerklaringGeaccepteerd: false }, ok(200)),
    ],
    final: NIET_GEWIJZIGD,
    historiek: GEEN_KOEPELGEBEURTENISSEN,
  },
];

const weigeringen = [
  {
    id: 'KP20',
    name: 'Vereniging zonder rechtspersoonlijkheid als koepel -> 400, niets gewijzigd',
    stappen: [
      wijzig('Identificeer als koepel', ALS_KOEPEL, fout(BERICHT.enkelRechtspersoonlijkheid)),
      wijzig('Identificeer als koepel en accepteer privacyverklaring', KOEPEL_MET_PRIVACY, fout(BERICHT.enkelRechtspersoonlijkheid)),
    ],
    final: NIET_GEWIJZIGD,
    historiek: GEEN_KOEPELGEBEURTENISSEN,
  },
  {
    id: 'KP21',
    name: 'Vereniging zonder rechtspersoonlijkheid accepteert privacyverklaring -> 400, niets gewijzigd',
    stappen: [wijzig('Accepteer privacyverklaring', PRIVACY_ACCEPTEREN, fout(BERICHT.privacyEnkelKoepel))],
    final: NIET_GEWIJZIGD,
    historiek: GEEN_KOEPELGEBEURTENISSEN,
  },
  {
    id: 'KP22',
    name: 'Privacyverklaring accepteren als NietBepaald of GeenKoepel -> 400',
    kbo: true,
    stappen: [
      beginToestand(),
      wijzig('Accepteer privacyverklaring als NietBepaald', PRIVACY_ACCEPTEREN, fout(BERICHT.privacyEnkelKoepel)),
      wijzig('Identificeer als geen koepel', ALS_GEEN_KOEPEL),
      wijzig('Accepteer privacyverklaring als GeenKoepel', PRIVACY_ACCEPTEREN, fout(BERICHT.privacyEnkelKoepel)),
    ],
    final: detail(GEEN_KOEPEL, false),
  },
  {
    id: 'KP23',
    name: 'Koepel naar GeenKoepel of NietBepaald met privacyverklaring in één request -> 400, blijft koepel',
    kbo: true,
    stappen: [
      beginToestand(),
      wijzig('Identificeer als koepel', ALS_KOEPEL),
      wijzig(
        'Identificeer als geen koepel en accepteer privacyverklaring',
        { koepelstatus: GEEN_KOEPEL, privacyVerklaringGeaccepteerd: true },
        fout(BERICHT.privacyEnkelKoepel)
      ),
      wijzig(
        'Zet terug op NietBepaald en accepteer privacyverklaring',
        { koepelstatus: NIET_BEPAALD, privacyVerklaringGeaccepteerd: true },
        fout(BERICHT.privacyEnkelKoepel)
      ),
    ],
    final: detail(KOEPEL, false),
  },
  {
    id: 'KP24',
    name: 'Onbekende vereniging -> 400',
    geenRegistratie: true,
    stappen: [wijzig('Identificeer onbekende vereniging als koepel', ALS_KOEPEL, fout(BERICHT.verenigingNietGevonden), { vCode: ONBEKENDE_VCODE })],
  },
];

const validatieFouten = [
  {
    id: 'KP40',
    name: 'Request zonder waarden -> 400',
    stappen: [
      wijzig('Wijzig met lege request', {}, validatie('request', BERICHT.leegRequest)),
      wijzig('Wijzig met enkel null-waarden', { koepelstatus: null, privacyVerklaringGeaccepteerd: null }, validatie('request', BERICHT.leegRequest)),
    ],
    final: NIET_GEWIJZIGD,
    historiek: GEEN_KOEPELGEBEURTENISSEN,
  },
  {
    id: 'KP41',
    name: 'Ongeldige koepelstatus -> 400',
    stappen: [
      wijzig('Koepelstatus Onbekend', { koepelstatus: 'Onbekend' }, validatie('koepelstatus', BERICHT.ongeldigeKoepelstatus)),
      wijzig('Koepelstatus Niet bepaald met spatie', { koepelstatus: 'Niet bepaald' }, validatie('koepelstatus', BERICHT.ongeldigeKoepelstatus)),
      wijzig('Lege koepelstatus', { koepelstatus: '' }, validatie('koepelstatus', BERICHT.ongeldigeKoepelstatus)),
    ],
    final: NIET_GEWIJZIGD,
    historiek: GEEN_KOEPELGEBEURTENISSEN,
  },
];

const ifMatch = [
  {
    id: 'KP50',
    name: 'Verouderde If-Match -> 412, niets gewijzigd',
    stappen: [
      wijzig('Identificeer als geen koepel', ALS_GEEN_KOEPEL, ok(), { bewaarHeader: { ETag: 'etagKP50' } }),
      wijzig('Zet terug op NietBepaald met actuele If-Match', ALS_NIET_BEPAALD, ok(), { headers: { 'If-Match': '{{etagKP50}}' } }),
      wijzig('Identificeer als geen koepel met verouderde If-Match', ALS_GEEN_KOEPEL, { status: 412 }, { headers: { 'If-Match': '{{etagKP50}}' } }),
    ],
    final: NIET_GEWIJZIGD,
    historiek: { bevat: [alsGeenKoepel(1), teruggezet(1)] },
  },
];

const autorisatie = [
  {
    id: 'KP60',
    name: 'Zonder token -> 401, niets gewijzigd',
    stappen: [wijzig('Identificeer als geen koepel zonder token', ALS_GEEN_KOEPEL, { status: 401, title: 'Unauthorized' }, { zonderToken: true })],
    final: NIET_GEWIJZIGD,
    historiek: GEEN_KOEPELGEBEURTENISSEN,
  },
];

module.exports = {
  name: 'Koepels',
  description:
    'Koepelgegevens van een vereniging (PATCH /v1/verenigingen/{vCode}/koepelgegevens): de koepelstatus (Koepel, ' +
    'GeenKoepel, NietBepaald) en de acceptatie van de privacyverklaring. Enkel zichtbaar in beheer detail en historiek.',
  groepen: [
    { name: 'Nieuwe vereniging', scenarios: nieuweVereniging },
    { name: 'Vereniging met rechtspersoonlijkheid', scenarios: metRechtspersoonlijkheid },
    { name: 'Weigeringen', scenarios: weigeringen },
    { name: 'Vereniging zonder rechtspersoonlijkheid', scenarios: zonderRechtspersoonlijkheid },
    { name: 'Validatie', scenarios: validatieFouten },
    { name: 'If-Match', scenarios: ifMatch },
    { name: 'Autorisatie', scenarios: autorisatie },
  ],
};
