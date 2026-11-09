// Bankrekeningnummers
//
// Startversie: de collectie wordt verder onderhouden in de Postman workspace (de bron van waarheid).
// Dit bestand wordt niet bijgewerkt en dient als voorbeeld voor nieuwe features. Opnieuw genereren overschrijft
// wat er sindsdien in Postman is aangepast.
//
// Rules: VoegBankrekeningnummerToeValidator, WijzigBankrekeningnummerValidator, registratie-validators,
// Bankrekeningnummers (IBAN uniek, ids niet hergebruikt), VerenigingOfAnyKind.Wijzig/VerwijderBankrekeningnummer
// (BankrekeningnummerIsNietGekend, KBO-bron niet verwijderen), IbanNummer (Belgisch, formaat met spaties).
// Bankrekeningnummers zijn persoonsgegevens: enkel zichtbaar in beheer detail en historiek, nooit publiek.

const { lijstOp, bevatOp, zonderOp } = require('../../shared/builders');

// ================================================================ test data

// Per scenario nieuwe, geldige Belgische IBANs: ibanA/B/C, met varianten _spaties en _punten.
const SETUP_IBAN = `function belgischeIban() {
    const basis = String(Math.floor(Math.random() * 1e10)).padStart(10, '0');
    let controle = Number(BigInt(basis) % 97n);
    if (controle === 0) controle = 97;
    const bban = basis + String(controle).padStart(2, '0');
    const check = String(98n - (BigInt(bban + '111400') % 97n)).padStart(2, '0');
    return 'BE' + check + bban;
}
['A', 'B', 'C'].forEach(x => {
    const iban = belgischeIban();
    pm.collectionVariables.set('iban' + x, iban);
    pm.collectionVariables.set('iban' + x + '_spaties', iban.replace(/(.{4})(?=.)/g, '$1 '));
    pm.collectionVariables.set('iban' + x + '_punten', iban.replace(/(.{4})(?=.)/g, '$1.'));
});`;

const FRODO = 'Frodo Baggins';
const SAM = 'Samwise Gamgee';
const PIPPIN = 'Peregrin Took';
const tekst = (n) => 'Mellon vriend '.repeat(20).slice(0, n); // geen spatie op positie 128 of 129
const T128 = tekst(128);
const T129 = tekst(129);
const ONBEKENDE_VCODE = 'V9999999';

const BERICHT = {
  ibanOngeldig: "Het opgegeven 'IBAN' is geen geldig Belgisch IBAN.",
  ibanUniek: 'Het IBAN van een bankrekening moet uniek zijn binnen de vereniging.',
  titularissenUniek: 'De titularissen van een bankrekening moeten uniek zijn.',
  doelTeLang: 'Doel mag niet langer dan 128 karakters zijn.',
  titularisTeLang: 'Titularis mag niet langer dan 128 karakters zijn.',
  doelOfTitularissen: 'Doel of Titularissen moet ingevuld zijn.',
  nietGekend: (id) => `Bankrekeningnummer met bankrekeningnummerId '${id}' is niet gekend.`,
  verenigingNietGevonden: `De resource met id '${ONBEKENDE_VCODE}' werd niet gevonden.`,
};

// ================================================================ building blocks

const br = (iban, doel, titularissen) => {
  const b = { iban };
  if (doel !== undefined) b.doel = doel;
  if (titularissen !== undefined) b.titularissen = titularissen;
  return b;
};
const IBAN = (x) => `{{iban${x}}}`;

// acties op /v1/verenigingen/{vCode}/bankrekeningnummers
const voegToe = (naam, bankrekeningnummer, verwacht, extra = {}) => ({
  naam,
  actie: 'POST',
  pad: 'bankrekeningnummers',
  body: bankrekeningnummer === undefined ? {} : { bankrekeningnummer },
  verwacht,
  ...extra,
});
const wijzig = (naam, id, body, verwacht, extra = {}) => ({ naam, actie: 'PATCH', pad: `bankrekeningnummers/${id}`, body, verwacht, ...extra });
const verwijder = (naam, id, verwacht, extra = {}) => ({ naam, actie: 'DELETE', pad: `bankrekeningnummers/${id}`, verwacht, ...extra });
const ok = (status = 202) => ({ status });
const validatie = (veld, reden, code) => ({ status: 400, validatie: { veld, reden, ...(code ? { code } : {}) } });
const fout = (detail) => ({ status: 400, detail });

// standaard voorbereiding: één of drie bankrekeningnummers (gewoon, met puntjes, met spaties)
const rekeningA = () => voegToe('Voeg bankrekeningnummer A toe', br(IBAN('A'), 'Lidgeld', [FRODO]));
const drieRekeningen = () => [
  rekeningA(),
  voegToe('Voeg bankrekeningnummer B toe (puntjes)', br('{{ibanB_punten}}', 'Drinkgeld', [SAM])),
  voegToe('Voeg bankrekeningnummer C toe (spaties)', br('{{ibanC_spaties}}', 'Uitstapjes', [PIPPIN])),
];

// beheer detail
const rek = (bankrekeningnummerId, x, doel, titularissen) => ({ bankrekeningnummerId, iban: IBAN(x), doel, titularissen });
const REK_A = rek(1, 'A', 'Lidgeld', [FRODO]);
const REK_B = rek(2, 'B', 'Drinkgeld', [SAM]);
const REK_C = rek(3, 'C', 'Uitstapjes', [PIPPIN]);
const lijst = (...items) => ({ beheerDetail: lijstOp('bankrekeningnummers', items) });
const GEEN_REKENINGEN = lijst();

// historiek
const toegevoegd = (id, x, doel, titularissen, aantal = 1) => ({
  gebeurtenis: 'BankrekeningnummerWerdToegevoegd',
  data: { bankrekeningnummerId: id, iban: IBAN(x), doel, titularissen },
  beschrijving: `Bankrekeningnummer met IBAN '{{iban${x}_spaties}}' werd toegevoegd.`,
  aantal,
});
const gewijzigd = (id, doel, titularissen) => ({
  gebeurtenis: 'BankrekeningnummerWerdGewijzigd',
  data: { bankrekeningnummerId: id, doel, titularissen },
  beschrijving: 'Bankrekeningnummer werd gewijzigd.',
  aantal: 1,
});
const verwijderd = (id, x) => ({
  gebeurtenis: 'BankrekeningnummerWerdVerwijderd',
  data: { bankrekeningnummerId: id, iban: IBAN(x) },
  beschrijving: `Bankrekeningnummer met IBAN '{{iban${x}_spaties}}' werd verwijderd.`,
  aantal: 1,
});
const GEEN_EVENTS = { geen: ['Bankrekeningnummer'] };
const GEEN_WIJZIGING = { bevat: [toegevoegd(1, 'A', 'Lidgeld', [FRODO])], geen: ['BankrekeningnummerWerdGewijzigd'] };

// ================================================================ scenarios

const toevoegen = [
  {
    id: 'BR1',
    name: 'Eén bankrekeningnummer -> toegevoegd, enkel zichtbaar in beheer (persoonsgegevens)',
    stappen: [rekeningA()],
    final: {
      beheerDetail: lijstOp('bankrekeningnummers', [{ ...REK_A, bron: 'Initiator' }]),
      publiekDetail: { nietInResponse: [IBAN('A'), FRODO] },
    },
    historiek: { bevat: [toegevoegd(1, 'A', 'Lidgeld', [FRODO])] },
  },
  {
    id: 'BR2',
    name: 'Meerdere titularissen -> allemaal bewaard, in volgorde',
    stappen: [voegToe('Voeg bankrekeningnummer toe met drie titularissen', br(IBAN('A'), 'Lidgeld', [FRODO, SAM, PIPPIN]))],
    final: lijst(rek(1, 'A', 'Lidgeld', [FRODO, SAM, PIPPIN])),
    historiek: { bevat: [toegevoegd(1, 'A', 'Lidgeld', [FRODO, SAM, PIPPIN])] },
  },
  {
    id: 'BR3',
    name: 'Zonder doel -> doel is leeg',
    stappen: [voegToe('Voeg bankrekeningnummer toe zonder doel', br(IBAN('A'), undefined, [FRODO]))],
    final: lijst(rek(1, 'A', '', [FRODO])),
    historiek: { bevat: [toegevoegd(1, 'A', '', [FRODO])] },
  },
  {
    id: 'BR4',
    name: 'Doel lege string -> toegevoegd met leeg doel',
    stappen: [voegToe('Voeg bankrekeningnummer toe met leeg doel', br(IBAN('A'), '', [FRODO]))],
    final: lijst(rek(1, 'A', '', [FRODO])),
  },
  {
    id: 'BR5',
    name: 'Doel en titularis van 128 karakters -> toegevoegd',
    stappen: [voegToe('Voeg bankrekeningnummer toe met doel en titularis van 128 karakters', br(IBAN('A'), T128, [T128]))],
    final: lijst(rek(1, 'A', T128, [T128])),
  },
  {
    id: 'BR6',
    name: 'Drie bankrekeningnummers (gewoon, met puntjes, met spaties) -> ids 1, 2 en 3',
    stappen: drieRekeningen(),
    final: lijst(REK_A, REK_B, REK_C),
    historiek: {
      bevat: [toegevoegd(1, 'A', 'Lidgeld', [FRODO]), toegevoegd(2, 'B', 'Drinkgeld', [SAM]), toegevoegd(3, 'C', 'Uitstapjes', [PIPPIN])],
    },
  },
];

const toevoegenWeigeringen = [
  {
    id: 'BR10',
    name: 'Zonder IBAN -> 400',
    stappen: [voegToe('Voeg bankrekeningnummer toe zonder IBAN', { doel: 'Lidgeld', titularissen: [FRODO] }, validatie('bankrekeningnummer.Iban', "'iban' is verplicht."))],
    final: GEEN_REKENINGEN,
    historiek: GEEN_EVENTS,
  },
  {
    id: 'BR11',
    name: 'Lege IBAN -> 400',
    stappen: [voegToe('Voeg bankrekeningnummer toe met lege IBAN', br('', 'Lidgeld', [FRODO]), validatie('bankrekeningnummer.Iban', "'iban' mag niet leeg zijn."))],
    final: GEEN_REKENINGEN,
  },
  {
    id: 'BR12',
    name: 'Ongeldige IBANs -> 400',
    stappen: [
      ['buitenlandse IBAN', 'NL91ABNA0417164300'],
      ['te lang', 'BE685390075470341'],
      ['te kort', 'BE6853900754703'],
      ['vreemde tekens', 'BE68@39007547#34'],
      ['liggende streepjes', 'BE68_3900_7547_034'],
      ['verkeerd controlegetal', 'BE00539007547034'],
      ['letters in het nummer', 'BE68ABC87547034'],
      ['landcode in kleine letters', 'be68539007547034'],
    ].map(([soort, iban]) =>
      voegToe(`Voeg bankrekeningnummer toe met IBAN: ${soort}`, br(iban, 'Lidgeld', [FRODO]), validatie('bankrekeningnummer.Iban', BERICHT.ibanOngeldig))
    ),
    final: GEEN_REKENINGEN,
    historiek: GEEN_EVENTS,
  },
  {
    id: 'BR13',
    name: 'Zonder titularissen -> 400',
    stappen: [voegToe('Voeg bankrekeningnummer toe zonder titularissen', br(IBAN('A'), 'Lidgeld'), validatie('bankrekeningnummer.Titularissen', "'Titularissen' is verplicht."))],
    final: GEEN_REKENINGEN,
  },
  {
    id: 'BR14',
    name: 'Lege lijst titularissen -> 400',
    stappen: [voegToe('Voeg bankrekeningnummer toe met lege lijst titularissen', br(IBAN('A'), 'Lidgeld', []), validatie('bankrekeningnummer.Titularissen', "'Titularissen' mag niet leeg zijn."))],
    final: GEEN_REKENINGEN,
  },
  {
    id: 'BR15',
    name: 'Titularis null of lege string -> 400',
    stappen: [
      voegToe('Voeg bankrekeningnummer toe met titularis null', br(IBAN('A'), 'Lidgeld', [null]), validatie('bankrekeningnummer.Titularissen[0]', "'Titularissen' is verplicht.", 'NotNullValidator')),
      voegToe('Voeg bankrekeningnummer toe met lege titularis', br(IBAN('A'), 'Lidgeld', ['']), validatie('bankrekeningnummer.Titularissen[0]', "'Titularissen' mag niet leeg zijn.", 'NotEmptyValidator')),
    ],
    final: GEEN_REKENINGEN,
  },
  {
    id: 'BR16',
    name: 'Dezelfde titularis tweemaal (andere hoofdletters) -> 400',
    stappen: [voegToe('Voeg bankrekeningnummer toe met dubbele titularis', br(IBAN('A'), 'Lidgeld', [FRODO, SAM, FRODO.toLowerCase()]), validatie('bankrekeningnummer.Titularissen', BERICHT.titularissenUniek))],
    final: GEEN_REKENINGEN,
  },
  {
    id: 'BR17',
    name: 'Doel of titularis van 129 karakters -> 400',
    stappen: [
      voegToe('Voeg bankrekeningnummer toe met doel van 129 karakters', br(IBAN('A'), T129, [FRODO]), validatie('bankrekeningnummer.Doel', BERICHT.doelTeLang)),
      voegToe('Voeg bankrekeningnummer toe met titularis van 129 karakters', br(IBAN('A'), 'Lidgeld', [T129]), validatie('bankrekeningnummer.Titularissen[0]', BERICHT.titularisTeLang)),
    ],
    final: GEEN_REKENINGEN,
    historiek: GEEN_EVENTS,
  },
  {
    id: 'BR18',
    name: 'Body zonder bankrekeningnummer -> 400',
    stappen: [voegToe('Voeg bankrekeningnummer toe met lege body', undefined, validatie('bankrekeningnummer', "'Bankrekeningnummer' is verplicht."))],
    final: GEEN_REKENINGEN,
  },
  {
    id: 'BR19',
    name: 'Zelfde IBAN opnieuw, ook met puntjes of spaties -> 400, IBAN is uniek',
    stappen: [
      rekeningA(),
      voegToe('Voeg zelfde IBAN opnieuw toe', br(IBAN('A'), 'Drinkgeld', [SAM]), fout(BERICHT.ibanUniek)),
      voegToe('Voeg zelfde IBAN met puntjes toe', br('{{ibanA_punten}}', 'Drinkgeld', [SAM]), fout(BERICHT.ibanUniek)),
      voegToe('Voeg zelfde IBAN met spaties toe', br('{{ibanA_spaties}}', 'Drinkgeld', [SAM]), fout(BERICHT.ibanUniek)),
    ],
    final: lijst(REK_A),
    historiek: { bevat: [toegevoegd(1, 'A', 'Lidgeld', [FRODO])] },
  },
  {
    id: 'BR20',
    name: 'Onbekende vereniging -> 400',
    stappen: [voegToe('Voeg bankrekeningnummer toe aan onbekende vereniging', br(IBAN('A'), 'Lidgeld', [FRODO]), fout(BERICHT.verenigingNietGevonden), { vCode: ONBEKENDE_VCODE })],
  },
];

const registratie = [
  {
    id: 'BR30',
    name: 'Registratie met één bankrekeningnummer -> id 1',
    a: { bankrekeningnummers: [br(IBAN('A'), 'Lidgeld', [FRODO])] },
    final: lijst(REK_A),
    historiek: {
      bevat: [
        {
          gebeurtenis: 'VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd',
          data: { bankrekeningnummers: [{ bankrekeningnummerId: 1, iban: IBAN('A'), doel: 'Lidgeld', titularissen: [FRODO] }] },
          aantal: 1,
        },
      ],
    },
  },
  {
    id: 'BR31',
    name: 'Registratie met drie bankrekeningnummers (gewoon, puntjes, spaties) -> ids 1, 2 en 3',
    a: { bankrekeningnummers: [br(IBAN('A'), 'Lidgeld', [FRODO]), br('{{ibanB_punten}}', 'Drinkgeld', [SAM]), br('{{ibanC_spaties}}', 'Uitstapjes', [PIPPIN])] },
    final: lijst(REK_A, REK_B, REK_C),
  },
  {
    id: 'BR32',
    name: 'Registratie met tweemaal hetzelfde IBAN -> 400',
    a: { bankrekeningnummers: [br(IBAN('A'), 'Lidgeld', [FRODO]), br(IBAN('A'), 'Drinkgeld', [SAM])] },
    registratie: validatie('bankrekeningnummers', BERICHT.ibanUniek),
  },
  {
    id: 'BR33',
    name: 'Registratie met hetzelfde IBAN in een ander formaat -> 400',
    a: { bankrekeningnummers: [br(IBAN('A'), 'Lidgeld', [FRODO]), br('{{ibanA_spaties}}', 'Drinkgeld', [SAM])] },
    registratie: fout(BERICHT.ibanUniek),
  },
  {
    id: 'BR34',
    name: 'Registratie met ongeldig IBAN -> 400',
    a: { bankrekeningnummers: [br('NL91ABNA0417164300', 'Lidgeld', [FRODO])] },
    registratie: validatie('bankrekeningnummers[0].Iban', BERICHT.ibanOngeldig),
  },
];

const wijzigen = [
  {
    id: 'BR40',
    name: 'Enkel doel wijzigen -> titularissen blijven',
    stappen: [rekeningA(), wijzig('Wijzig doel', 1, { bankrekeningnummer: { doel: 'Varia' } }, ok())],
    final: lijst(rek(1, 'A', 'Varia', [FRODO])),
    historiek: { bevat: [gewijzigd(1, 'Varia', [FRODO])] },
  },
  {
    id: 'BR41',
    name: 'Enkel titularissen wijzigen -> de lijst wordt volledig vervangen',
    stappen: [rekeningA(), wijzig('Wijzig titularissen', 1, { bankrekeningnummer: { titularissen: [SAM, PIPPIN] } }, ok())],
    final: lijst(rek(1, 'A', 'Lidgeld', [SAM, PIPPIN])),
    historiek: { bevat: [gewijzigd(1, 'Lidgeld', [SAM, PIPPIN])] },
  },
  {
    id: 'BR42',
    name: 'Doel en titularissen van één van drie wijzigen -> de andere blijven ongewijzigd',
    stappen: [...drieRekeningen(), wijzig('Wijzig doel en titularissen van C', 3, { bankrekeningnummer: { doel: 'Feest in de Gouw', titularissen: [FRODO] } }, ok())],
    final: lijst(REK_A, REK_B, rek(3, 'C', 'Feest in de Gouw', [FRODO])),
    historiek: { bevat: [gewijzigd(3, 'Feest in de Gouw', [FRODO])] },
  },
  {
    id: 'BR43',
    name: 'Doel leegmaken -> doel is leeg',
    stappen: [rekeningA(), wijzig('Wijzig doel naar lege string', 1, { bankrekeningnummer: { doel: '' } }, ok())],
    final: lijst(rek(1, 'A', '', [FRODO])),
    historiek: { bevat: [gewijzigd(1, '', [FRODO])] },
  },
  {
    id: 'BR44',
    name: 'Doel null met nieuwe titularissen -> doel blijft',
    stappen: [rekeningA(), wijzig('Wijzig titularissen met doel null', 1, { bankrekeningnummer: { doel: null, titularissen: [SAM] } }, ok())],
    final: lijst(rek(1, 'A', 'Lidgeld', [SAM])),
  },
  {
    id: 'BR45',
    name: 'Doel en titularis van 128 karakters -> gewijzigd',
    stappen: [rekeningA(), wijzig('Wijzig naar doel en titularis van 128 karakters', 1, { bankrekeningnummer: { doel: T128, titularissen: [T128] } }, ok())],
    final: lijst(rek(1, 'A', T128, [T128])),
  },
  {
    id: 'BR46',
    name: 'Zelfde waarden -> 200, geen wijziging',
    stappen: [
      rekeningA(),
      wijzig('Wijzig doel naar dezelfde waarde', 1, { bankrekeningnummer: { doel: 'Lidgeld' } }, ok(200)),
      wijzig('Wijzig titularissen naar dezelfde waarde', 1, { bankrekeningnummer: { titularissen: [FRODO] } }, ok(200)),
    ],
    final: lijst(REK_A),
    historiek: GEEN_WIJZIGING,
  },
];

const wijzigenWeigeringen = [
  {
    id: 'BR50',
    name: 'Lege body of geen doel en titularissen -> 400',
    stappen: [
      rekeningA(),
      wijzig('Wijzig met lege body', 1, {}, validatie('bankrekeningnummer', "'Bankrekeningnummer' is verplicht.")),
      wijzig('Wijzig zonder doel en titularissen', 1, { bankrekeningnummer: {} }, validatie('bankrekeningnummer', BERICHT.doelOfTitularissen)),
      wijzig('Wijzig met enkel een lege lijst titularissen', 1, { bankrekeningnummer: { titularissen: [] } }, validatie('bankrekeningnummer', BERICHT.doelOfTitularissen)),
    ],
    final: lijst(REK_A),
    historiek: GEEN_WIJZIGING,
  },
  {
    id: 'BR51',
    name: 'IBAN wijzigen kan niet -> 400',
    stappen: [rekeningA(), wijzig('Wijzig enkel het IBAN', 1, { bankrekeningnummer: { iban: IBAN('B') } }, validatie('bankrekeningnummer', BERICHT.doelOfTitularissen))],
    final: lijst(REK_A),
    historiek: GEEN_WIJZIGING,
  },
  {
    id: 'BR52',
    name: 'Ongeldige titularissen -> 400',
    stappen: [
      rekeningA(),
      wijzig('Wijzig naar lege titularis', 1, { bankrekeningnummer: { titularissen: [''] } }, validatie('bankrekeningnummer.Titularissen[0]', 'Titularissen mag niet leeg zijn.')),
      wijzig('Wijzig doel met lege lijst titularissen', 1, { bankrekeningnummer: { doel: 'Varia', titularissen: [] } }, validatie('bankrekeningnummer.Titularissen', "'Titularissen' mag niet leeg zijn.")),
      wijzig('Wijzig naar dubbele titularis', 1, { bankrekeningnummer: { titularissen: [SAM, SAM.toUpperCase()] } }, validatie('bankrekeningnummer.Titularissen', BERICHT.titularissenUniek)),
      wijzig('Wijzig naar titularis van 129 karakters', 1, { bankrekeningnummer: { titularissen: [T129] } }, validatie('bankrekeningnummer.Titularissen[0]', BERICHT.titularisTeLang)),
    ],
    final: lijst(REK_A),
    historiek: GEEN_WIJZIGING,
  },
  {
    id: 'BR53',
    name: 'Doel van 129 karakters -> 400',
    stappen: [rekeningA(), wijzig('Wijzig naar doel van 129 karakters', 1, { bankrekeningnummer: { doel: T129 } }, validatie('bankrekeningnummer.Doel', BERICHT.doelTeLang))],
    final: lijst(REK_A),
  },
  {
    id: 'BR54',
    name: 'Onbekend bankrekeningnummer of onbekende vereniging -> 400',
    stappen: [
      rekeningA(),
      wijzig('Wijzig onbekend bankrekeningnummer', 9, { bankrekeningnummer: { doel: 'Varia' } }, fout(BERICHT.nietGekend(9))),
      wijzig('Wijzig bankrekeningnummer van onbekende vereniging', 1, { bankrekeningnummer: { doel: 'Varia' } }, fout(BERICHT.verenigingNietGevonden), { vCode: ONBEKENDE_VCODE }),
    ],
    final: lijst(REK_A),
    historiek: GEEN_WIJZIGING,
  },
];

const verwijderen = [
  {
    id: 'BR60',
    name: 'Verwijderen -> weg, de andere blijven ongewijzigd',
    stappen: [...drieRekeningen(), verwijder('Verwijder bankrekeningnummer 1', 1, ok())],
    final: lijst(REK_B, REK_C),
    historiek: { bevat: [verwijderd(1, 'A')] },
  },
  {
    id: 'BR61',
    name: 'Zelfde IBAN opnieuw toevoegen na verwijderen -> nieuw id, ids worden niet hergebruikt',
    stappen: [...drieRekeningen(), verwijder('Verwijder bankrekeningnummer 1', 1, ok()), voegToe('Voeg IBAN A opnieuw toe', br(IBAN('A'), 'Lidgeld', [FRODO]))],
    final: lijst(REK_B, REK_C, rek(4, 'A', 'Lidgeld', [FRODO])),
    historiek: { bevat: [verwijderd(1, 'A'), toegevoegd(4, 'A', 'Lidgeld', [FRODO])] },
  },
  {
    id: 'BR62',
    name: 'Tweemaal verwijderen -> tweede keer 400',
    stappen: [rekeningA(), verwijder('Verwijder bankrekeningnummer 1', 1, ok()), verwijder('Verwijder bankrekeningnummer 1 opnieuw', 1, fout(BERICHT.nietGekend(1)))],
    final: GEEN_REKENINGEN,
    historiek: { bevat: [verwijderd(1, 'A')] },
  },
  {
    id: 'BR63',
    name: 'Onbekend id, niet-numeriek id of onbekende vereniging -> 400 / 404',
    stappen: [
      rekeningA(),
      verwijder('Verwijder onbekend bankrekeningnummer 0', 0, fout(BERICHT.nietGekend(0))),
      verwijder('Verwijder bankrekeningnummer ABC', 'ABC', { status: 404, title: 'Not Found' }),
      verwijder('Verwijder bankrekeningnummer van onbekende vereniging', 1, fout(BERICHT.verenigingNietGevonden), { vCode: ONBEKENDE_VCODE }),
    ],
    final: lijst(REK_A),
    historiek: { geen: ['BankrekeningnummerWerdVerwijderd'] },
  },
  {
    id: 'BR64',
    name: 'Vereniging zonder bankrekeningnummers -> 400',
    stappen: [verwijder('Verwijder bankrekeningnummer 1', 1, fout(BERICHT.nietGekend(1)))],
    final: GEEN_REKENINGEN,
    historiek: GEEN_EVENTS,
  },
];

// Op Test wordt een bestaande vereniging met rechtspersoonlijkheid hergebruikt (kboModus 'bestaand'): de scenario's
// werken met een eigen, uniek IBAN en controleren enkel dat (bevat / zonder), nooit de volledige lijst of vaste ids.
const KBO_REKENING = { iban: IBAN('A'), doel: 'Lidgeld', titularissen: [FRODO], bron: 'Initiator' };
const kboToevoegen = () => voegToe('Voeg bankrekeningnummer A toe', br(IBAN('A'), 'Lidgeld', [FRODO]), ok(), { bewaarId: 'bankrekeningnummerIdA' });
const KBO_TOEGEVOEGD = (aantal = 1) => ({
  gebeurtenis: 'BankrekeningnummerWerdToegevoegd',
  data: { iban: IBAN('A'), doel: 'Lidgeld', titularissen: [FRODO] },
  beschrijving: "Bankrekeningnummer met IBAN '{{ibanA_spaties}}' werd toegevoegd.",
  aantal,
});
const KBO_VERWIJDERD = {
  gebeurtenis: 'BankrekeningnummerWerdVerwijderd',
  data: { iban: IBAN('A') },
  beschrijving: "Bankrekeningnummer met IBAN '{{ibanA_spaties}}' werd verwijderd.",
  aantal: 1,
};

const kbo = [
  {
    id: 'BR70',
    name: 'Toevoegen aan vereniging met rechtspersoonlijkheid -> bron Initiator',
    kbo: true,
    stappen: [kboToevoegen()],
    final: { beheerDetail: bevatOp('bankrekeningnummers', [KBO_REKENING]) },
    historiek: { bevat: [KBO_TOEGEVOEGD()] },
  },
  {
    id: 'BR71',
    name: 'Eigen bankrekeningnummer verwijderen van vereniging met rechtspersoonlijkheid -> weg',
    kbo: true,
    stappen: [kboToevoegen(), verwijder('Verwijder bankrekeningnummer A', '{{bankrekeningnummerIdA}}', ok())],
    final: { beheerDetail: zonderOp('bankrekeningnummers', [{ iban: IBAN('A') }]) },
    historiek: { bevat: [KBO_TOEGEVOEGD(), KBO_VERWIJDERD] },
  },
  {
    id: 'BR72',
    name: 'Zelfde IBAN opnieuw toevoegen na verwijderen op vereniging met rechtspersoonlijkheid -> toegevoegd',
    kbo: true,
    stappen: [
      kboToevoegen(),
      verwijder('Verwijder bankrekeningnummer A', '{{bankrekeningnummerIdA}}', ok()),
      voegToe('Voeg IBAN A opnieuw toe', br(IBAN('A'), 'Lidgeld', [FRODO])),
    ],
    final: { beheerDetail: bevatOp('bankrekeningnummers', [KBO_REKENING]) },
    historiek: { bevat: [KBO_TOEGEVOEGD(2), KBO_VERWIJDERD] },
  },
];

// Validatie (aanwezigheid validatiedocument): de organisatie is de VR-Initiator. VLO mag dit niet.
// Geldige OVO-codes: OVO000001, OVO002949, OVO002956, OVO003168.
const OVO_1 = 'OVO000001';
const OVO_2 = 'OVO002956';
const VLO = 'OVO000067';
const valideer = (naam, id, ovo, verwacht) => ({ naam, actie: 'POST', pad: `bankrekeningnummers/${id}/validaties`, initiator: ovo, verwacht });
const maakOngedaan = (naam, id, ovo, verwacht) => ({ naam, actie: 'DELETE', pad: `bankrekeningnummers/${id}/validaties`, initiator: ovo, verwacht });
const bevestigdDoor = (...ovos) => ({ ...REK_A, bevestigdDoor: ovos.map((ovoCode) => ({ ovoCode })) });
const bevestigd = (ovo) => ({
  gebeurtenis: 'AanwezigheidBankrekeningnummerValidatieDocumentWerdBevestigd',
  data: { bankrekeningnummerId: 1, bevestigdDoor: { ovoCode: ovo } },
  beschrijving: `Bankrekeningnummervalidatiedocument werd bevestigd door '${ovo}'.`,
  aantal: 1,
});
const ongedaanGemaakt = (ovo) => ({
  gebeurtenis: 'AanwezigheidBankrekeningnummerValidatieDocumentWerdOngedaanGemaakt',
  data: { bankrekeningnummerId: 1, ongedaanGemaaktDoor: ovo },
  beschrijving: `Bankrekeningnummervalidatiedocument werd ongedaan gemaakt door '${ovo}'.`,
  aantal: 1,
});

const validatieDocument = [
  {
    id: 'BR90',
    name: 'Valideren door twee organisaties -> beide in bevestigdDoor',
    stappen: [rekeningA(), valideer(`Valideer door ${OVO_1}`, 1, OVO_1, ok()), valideer(`Valideer door ${OVO_2}`, 1, OVO_2, ok())],
    final: lijst(bevestigdDoor(OVO_1, OVO_2)),
    historiek: { bevat: [bevestigd(OVO_1), bevestigd(OVO_2)] },
  },
  {
    id: 'BR91',
    name: 'Tweemaal valideren door dezelfde organisatie -> 400',
    stappen: [
      rekeningA(),
      valideer(`Valideer door ${OVO_1}`, 1, OVO_1, ok()),
      valideer(`Valideer opnieuw door ${OVO_1}`, 1, OVO_1, fout(`Bankrekening validatie met OvoCode '${OVO_1}' is al reeds toegevoegd.`)),
    ],
    final: lijst(bevestigdDoor(OVO_1)),
    historiek: { bevat: [bevestigd(OVO_1)] },
  },
  {
    id: 'BR92',
    name: 'Validatie ongedaan maken door dezelfde organisatie -> bevestigdDoor leeg',
    stappen: [rekeningA(), valideer(`Valideer door ${OVO_1}`, 1, OVO_1, ok()), maakOngedaan(`Maak validatie ongedaan door ${OVO_1}`, 1, OVO_1, ok())],
    final: lijst(bevestigdDoor()),
    historiek: { bevat: [bevestigd(OVO_1), ongedaanGemaakt(OVO_1)] },
  },
  {
    id: 'BR93',
    name: 'Validatie ongedaan maken door een andere organisatie -> 400, validatie blijft',
    stappen: [
      rekeningA(),
      valideer(`Valideer door ${OVO_1}`, 1, OVO_1, ok()),
      maakOngedaan(
        `Maak validatie ongedaan door ${OVO_2}`,
        1,
        OVO_2,
        fout(`Validatie kan niet ongedaan gemaakt worden omdat er geen validatie is met OvoCode '${OVO_2}'.`)
      ),
    ],
    final: lijst(bevestigdDoor(OVO_1)),
    historiek: { bevat: [bevestigd(OVO_1)], geen: ['AanwezigheidBankrekeningnummerValidatieDocumentWerdOngedaanGemaakt'] },
  },
  {
    id: 'BR94',
    name: 'Onbekend bankrekeningnummer -> 400',
    stappen: [
      rekeningA(),
      valideer('Valideer onbekend bankrekeningnummer', 9, OVO_1, fout(BERICHT.nietGekend(9))),
      maakOngedaan('Maak validatie van onbekend bankrekeningnummer ongedaan', 9, OVO_1, fout(BERICHT.nietGekend(9))),
    ],
    final: lijst(bevestigdDoor()),
    historiek: { geen: ['AanwezigheidBankrekeningnummerValidatieDocument'] },
  },
  {
    id: 'BR95',
    name: 'VLO mag niet valideren of ongedaan maken -> 400',
    stappen: [
      rekeningA(),
      valideer('Valideer door VLO', 1, VLO, fout(`OvoCode '${VLO}' is niet gemachtigd om deze actie uit te voeren.`)),
      maakOngedaan('Maak validatie ongedaan door VLO', 1, VLO, fout(`OvoCode '${VLO}' is niet gemachtigd om deze actie uit te voeren.`)),
    ],
    final: lijst(bevestigdDoor()),
    historiek: { geen: ['AanwezigheidBankrekeningnummerValidatieDocument'] },
  },
];

const zonderToken = { status: 401, title: 'Unauthorized' };
const autorisatie = [
  {
    id: 'BR80',
    name: 'Zonder token -> 401, niets gewijzigd',
    stappen: [
      rekeningA(),
      voegToe('Voeg bankrekeningnummer toe zonder token', br(IBAN('B'), 'Drinkgeld', [SAM]), zonderToken, { zonderToken: true }),
      wijzig('Wijzig doel zonder token', 1, { bankrekeningnummer: { doel: 'Varia' } }, zonderToken, { zonderToken: true }),
      verwijder('Verwijder zonder token', 1, zonderToken, { zonderToken: true }),
    ],
    final: lijst(REK_A),
    historiek: { bevat: [toegevoegd(1, 'A', 'Lidgeld', [FRODO])], geen: ['BankrekeningnummerWerdGewijzigd', 'BankrekeningnummerWerdVerwijderd'] },
  },
];

module.exports = {
  name: 'Bankrekeningnummers',
  description:
    'Toevoegen, wijzigen en verwijderen van bankrekeningnummers, ook bij registratie en op een vereniging met ' +
    'rechtspersoonlijkheid. Bankrekeningnummers zijn persoonsgegevens: enkel zichtbaar in beheer.',
  setup: SETUP_IBAN,
  groepen: [
    { name: 'Toevoegen', scenarios: toevoegen },
    { name: 'Toevoegen - weigeringen', scenarios: toevoegenWeigeringen },
    { name: 'Registratie met bankrekeningnummers', scenarios: registratie },
    { name: 'Wijzigen', scenarios: wijzigen },
    { name: 'Wijzigen - weigeringen', scenarios: wijzigenWeigeringen },
    { name: 'Verwijderen', scenarios: verwijderen },
    { name: 'Vereniging met rechtspersoonlijkheid', scenarios: kbo },
    { name: 'Validatiedocument', scenarios: validatieDocument },
    { name: 'Autorisatie', scenarios: autorisatie },
  ],
};
