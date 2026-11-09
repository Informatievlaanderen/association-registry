// Turns scenario data (postman/features/<feature>/scenarios.js) into Postman requests with generated test scripts.
// See .opencode/skills/postman-regressie/SKILL.md for the rules and the scenario format.

const fs = require('fs');
const path = require('path');
const { RETRY, HELPERS } = require('./helpers');

const BEHEER = '{{beheer_url}}/v1/verenigingen';
const PUBLIEK = '{{publiek_url}}/v1/verenigingen';
const JSON_HEADERS = [{ key: 'Content-Type', value: 'application/json' }];
// The publiek API requires an API key; publiekApiKey is read from the vault by root-prerequest.js.
const PUBLIEK_HEADERS = [{ key: 'vr-api-key', value: '{{publiekApiKey}}' }];

// ---------------------------------------------------------------- small helpers

const script = (listen, code) => ({ listen, script: { type: 'text/javascript', exec: code.split('\n') } });
const test = (code) => script('test', code);
const q = (v) => JSON.stringify(v);
const title = (t) => t.replace(/\\/g, '\\\\').replace(/'/g, "\\'");
const kaal = (v) => (typeof v === 'string' ? v : JSON.stringify(v)).replace(/\{\{|\}\}/g, '');
const indent = (text, n) => text.split('\n').map((l) => (l ? ' '.repeat(n) + l : l)).join('\n');
const jsonBody = (body) => ({
  mode: 'raw',
  raw: typeof body === 'string' ? body : JSON.stringify(body, null, 2),
  options: { raw: { language: 'json' } },
});

const lijstOp = (veld, items) => ({ lijst: { veld, items } });
const bevatOp = (veld, items) => ({ bevat: { veld, items } });
const zonderOp = (veld, items) => ({ zonder: { veld, items } });
const zonderVelden = (items, ...keys) =>
  items.map((i) => Object.fromEntries(Object.entries(i).filter(([k]) => !keys.includes(k))));

const STORE_SEQUENCE = `if (pm.response.headers.get('VR-Sequence')) pm.collectionVariables.set('sequence', pm.response.headers.get('VR-Sequence'));`;

// ---------------------------------------------------------------- expected response of a write

// verwacht = { status, detail?, title?, validatie?: { veld, reden, code? } | [...] }
function verwachtChecks(label, verwacht) {
  const v = verwacht || { status: 202 };
  const lines = [
    `pm.test('${title(`${label} geeft ${v.status}`)}', () => pm.expect(pm.response.code, pm.response.text()).to.eql(${v.status}));`,
  ];
  if (v.detail)
    lines.push(
      `pm.test('${title(`${label}: detail '${kaal(v.detail)}'`)}', () => pm.expect(pm.response.json().detail).to.eql(pm.variables.replaceIn(${q(v.detail)})));`
    );
  if (v.title)
    lines.push(`pm.test('${title(`${label}: title '${v.title}'`)}', () => pm.expect(pm.response.json().title).to.eql(${q(v.title)}));`);
  [].concat(v.validatie || []).forEach((val) =>
    lines.push(`pm.test('${title(`${label}: ${val.veld}: ${val.reden}`)}', () => {
    const errors = pm.response.json().validationErrors || {};
    const key = Object.keys(errors).find(k => k.toLowerCase() === ${q(val.veld.toLowerCase())});
    pm.expect(key, JSON.stringify(errors)).to.exist;
    const fout = errors[key].find(e => e.reason === ${q(val.reden)});
    pm.expect(fout, JSON.stringify(errors[key])).to.exist;${val.code ? `\n    pm.expect(fout.code).to.eql(${q(val.code)});` : ''}
});`)
  );
  if (v.status >= 200 && v.status < 300) lines.push(STORE_SEQUENCE);
  return lines.join('\n');
}

// ---------------------------------------------------------------- registration

// The bevestigingstoken is a hash of the request body, so a retry after 409 must send the same body:
// the naam and the setup values (iban, insz, ...) are only generated on the first attempt.
function registreer(id, rol, gegevens = {}, { setup = '', verwacht } = {}) {
  const tokenKey = `bevestigingsToken_${id}_${rol}`;
  const naamKey = `naam_${id}_${rol}`;
  const succes = !verwacht || verwacht.status === 202;
  const resultaat = succes
    ? `pm.test('${id}: registratie ${rol} geeft 202', () => pm.expect(pm.response.code, pm.response.text()).to.eql(202));
pm.collectionVariables.set('vCode${rol}', (pm.response.headers.get('Location') || '').split('/').pop());
${STORE_SEQUENCE}`
    : verwachtChecks(`${id}: registratie ${rol}`, verwacht);
  return {
    name: `${id} - Registreer vereniging ${rol}`,
    event: [
      script(
        'prerequest',
        `const token = pm.collectionVariables.get('${tokenKey}');
if (token && pm.collectionVariables.get('${naamKey}')) {
    // retry na dubbeldetectie: zelfde body, met het bevestigingstoken
    pm.request.headers.upsert({ key: 'VR-BevestigingsToken', value: token });
} else {
    pm.collectionVariables.unset('${tokenKey}');
${indent(setup.trimEnd(), 4)}
    pm.collectionVariables.set('${naamKey}', 'Regressie ${id} ${rol} ' + pm.variables.replaceIn('{{$randomUUID}}'));
}`
      ),
      test(`if (pm.response.code === 409) {
    // dubbeldetectie: opnieuw met bevestigingstoken
    pm.collectionVariables.set('${tokenKey}', pm.response.json().bevestigingsToken);
    pm.execution.setNextRequest(pm.info.requestName);
} else {
    pm.collectionVariables.unset('${tokenKey}');
    pm.collectionVariables.unset('${naamKey}');
${indent(resultaat, 4)}
}`),
    ],
    request: { method: 'POST', header: JSON_HEADERS, body: jsonBody({ naam: `{{${naamKey}}}`, ...gegevens }), url: `${BEHEER}/{{registreerVereniging}}` },
  };
}

// Vereniging met rechtspersoonlijkheid. kboModus 'random' (local): a new one per run. 'bestaand' (Test): an existing
// vereniging is reused (200), so a scenario may only assert what it adds itself (bevat / zonder, not lijst).
function registreerKbo(id, rol = 'A', setup = '') {
  return {
    name: `${id} - Registreer vereniging met rechtspersoonlijkheid ${rol}`,
    event: [
      script('prerequest', `${setup.trimEnd()}\npm.collectionVariables.set('kboNummer', funcs.kboNummer());`),
      test(`pm.test('${id}: registratie uit KBO geeft 202 (nieuw) of 200 (bestaand)', () => {
    pm.expect(pm.response.code, pm.response.text()).to.be.oneOf([200, 202]);
});
pm.collectionVariables.set('vCode${rol}', (pm.response.headers.get('Location') || '').split('/').pop());
${STORE_SEQUENCE}`),
    ],
    request: { method: 'POST', header: JSON_HEADERS, body: jsonBody({ kboNummer: '{{kboNummer}}' }), url: `${BEHEER}/kbo` },
  };
}

// ---------------------------------------------------------------- actions

// stap = { naam, actie: 'POST'|'PATCH'|'DELETE', pad?, op? ('A'), vCode? (literal), body?, zonderToken?, verwacht?,
//          bewaarId?, initiator? }
// bewaarId: store the id from the Location header (last path segment) in this collection variable.
// initiator: VR-Initiator (OVO code) for this request; the root script sets the default, so it is set in a prerequest.
function stapRequest(id, stap) {
  const vCode = stap.vCode || `{{vCode${stap.op || 'A'}}}`;
  let checks = verwachtChecks(`${id} ${stap.naam}`, stap.verwacht);
  if (stap.bewaarId)
    checks += `\nif (pm.response.code === 202) pm.collectionVariables.set('${stap.bewaarId}', (pm.response.headers.get('Location') || '').split('/').pop());`;
  const request = { method: stap.actie, header: stap.body === undefined ? [] : JSON_HEADERS, url: `${BEHEER}/${vCode}${stap.pad ? '/' + stap.pad : ''}` };
  if (stap.body !== undefined) request.body = jsonBody(stap.body);
  if (stap.zonderToken) request.auth = { type: 'noauth' };
  const event = [test(checks)];
  if (stap.initiator)
    event.unshift(script('prerequest', `pm.request.headers.upsert({ key: 'VR-Initiator', value: pm.variables.replaceIn(${q(stap.initiator)}) });`));
  return { name: `${id} - ${stap.naam}`, event, request };
}

// ---------------------------------------------------------------- end state

// spec = { lijst | bevat | zonder: { veld, items: [subset] }, props | propsNiet: { veld: waarde }, nietInResponse: [waarde] }
function specChecks(id, label, spec) {
  const lines = [`const spec = resolve(${q(spec)});`];
  if (spec.props)
    Object.keys(spec.props).forEach((k) =>
      lines.push(
        `pm.test('${title(`${id} ${label}: ${k} = ${kaal(spec.props[k])}`)}', () => pm.expect(matches(vereniging, { ${q(k)}: spec.props[${q(k)}] }), JSON.stringify(getCI(vereniging, ${q(k)}))).to.be.true);`
      )
    );
  if (spec.propsNiet)
    Object.keys(spec.propsNiet).forEach((k) =>
      lines.push(
        `pm.test('${title(`${id} ${label}: ${k} is niet ${kaal(spec.propsNiet[k])}`)}', () => pm.expect(matches(vereniging, { ${q(k)}: spec.propsNiet[${q(k)}] }), JSON.stringify(getCI(vereniging, ${q(k)}))).to.be.false);`
      )
    );
  if (spec.lijst) {
    const { veld, items } = spec.lijst;
    lines.push(
      `pm.test('${id} ${label}: ${items.length} ${veld}', () => pm.expect((getCI(vereniging, '${veld}') || []).length, JSON.stringify(getCI(vereniging, '${veld}'))).to.eql(${items.length}));`,
      ...items.map(
        (item, i) =>
          `pm.test('${title(`${id} ${label}: ${veld} ${kaal(item)}`)}', () => pm.expect((getCI(vereniging, '${veld}') || []).some(x => matches(x, spec.lijst.items[${i}])), JSON.stringify(getCI(vereniging, '${veld}'))).to.be.true);`
      )
    );
  }
  if (spec.bevat)
    spec.bevat.items.forEach((item, i) =>
      lines.push(
        `pm.test('${title(`${id} ${label}: ${spec.bevat.veld} bevat ${kaal(item)}`)}', () => pm.expect((getCI(vereniging, '${spec.bevat.veld}') || []).some(x => matches(x, spec.bevat.items[${i}])), JSON.stringify(getCI(vereniging, '${spec.bevat.veld}'))).to.be.true);`
      )
    );
  if (spec.zonder)
    spec.zonder.items.forEach((item, i) =>
      lines.push(
        `pm.test('${title(`${id} ${label}: ${spec.zonder.veld} zonder ${kaal(item)}`)}', () => pm.expect((getCI(vereniging, '${spec.zonder.veld}') || []).some(x => matches(x, spec.zonder.items[${i}])), JSON.stringify(getCI(vereniging, '${spec.zonder.veld}'))).to.be.false);`
      )
    );
  if (spec.nietInResponse)
    spec.nietInResponse.forEach((v, i) =>
      lines.push(`pm.test('${title(`${id} ${label}: bevat geen ${kaal(v)}`)}', () => {
    const tekst = pm.response.text();
    pm.expect(spec.nietInResponse[${i}], 'variabele niet ingevuld').to.not.include('{{');
    vormen(spec.nietInResponse[${i}]).forEach(vorm => pm.expect(tekst.includes(vorm), vorm).to.be.false);
});`)
    );
  return lines.join('\n');
}

function beheerDetail(s) {
  return {
    name: `${s.id} - Beheer detail vereniging A`,
    event: [
      test(`${RETRY}
${HELPERS}
if (!retryIf(pm.response.code === 412)) {
    pm.test('${s.id} beheer detail: 200', () => pm.response.to.have.status(200));
    const vereniging = pm.response.json().vereniging || {};
${indent(specChecks(s.id, 'beheer detail', s.final.beheerDetail), 4)}
}`),
    ],
    request: { method: 'GET', header: [], url: `${BEHEER}/{{vCodeA}}?expectedSequence={{sequence}}` },
  };
}

// historiek = { bevat: [{ gebeurtenis, data?, beschrijving?, aantal? }], geen: ['prefix van gebeurtenis'] }
// data is a subset match; aantal = exact number of matching gebeurtenissen (default: at least one).
function historiek(s) {
  const h = s.historiek;
  const checks = [
    ...(h.bevat || []).map(
      (e, i) => `pm.test('${title(`${s.id} historiek: ${e.aantal !== undefined ? e.aantal + 'x ' : ''}${e.gebeurtenis}${e.data ? ' ' + kaal(e.data) : ''}`)}', () => {
    const e = H.bevat[${i}];
    const gevonden = gebeurtenissen.filter(g => g.gebeurtenis === e.gebeurtenis && (!e.data || matches(g.data, e.data)));
    const context = JSON.stringify(gebeurtenissen.filter(g => g.gebeurtenis === e.gebeurtenis));
    if (e.aantal !== undefined) pm.expect(gevonden.length, context).to.eql(e.aantal);
    else pm.expect(gevonden.length, context).to.be.above(0);
    if (e.beschrijving) pm.expect(gevonden.map(g => g.beschrijving), context).to.include(e.beschrijving);
});`
    ),
    ...(h.geen || []).map(
      (prefix) => `pm.test('${title(`${s.id} historiek: geen ${prefix}*`)}', () =>
    pm.expect(gebeurtenissen.filter(g => g.gebeurtenis.startsWith(${q(prefix)})).map(g => g.gebeurtenis)).to.eql([]));`
    ),
  ];
  return {
    name: `${s.id} - Beheer historiek vereniging A`,
    event: [
      test(`${RETRY}
${HELPERS}
if (!retryIf(pm.response.code === 412)) {
    pm.test('${s.id} historiek: 200', () => pm.response.to.have.status(200));
    const gebeurtenissen = pm.response.json().gebeurtenissen || [];
    const H = resolve(${q(h)});
${indent(checks.join('\n'), 4)}
}`),
    ],
    request: { method: 'GET', header: [], url: `${BEHEER}/{{vCodeA}}/historiek?expectedSequence={{sequence}}` },
  };
}

// Search and publiek endpoints have no sequence guard: retry until the vereniging is found and satisfies the spec.
function eventueel(s, endpoint, naam, label, url, pick, header = []) {
  return {
    name: `${s.id} - ${naam}`,
    event: [
      test(`${RETRY}
${HELPERS}
const vCodeA = pm.collectionVariables.get('vCodeA');
const json = pm.response.code === 200 ? pm.response.json() : {};
const vereniging = (${pick}) || null;
const upToDate = !!vereniging && voldoet(vereniging, resolve(${q(s.final[endpoint])}));
if (!retryIf(!upToDate)) {
    pm.test('${s.id} ${label}: 200', () => pm.response.to.have.status(200));
    pm.test('${s.id} ${label}: vereniging gevonden', () => pm.expect(vereniging, pm.response.text()).to.exist);
    if (vereniging) {
${indent(specChecks(s.id, label, s.final[endpoint]), 8)}
    }
}`),
    ],
    request: { method: 'GET', header, url },
  };
}

const FIND_IN_ZOEKEN = '(json.verenigingen || []).find(v => v.vCode === vCodeA)';
const ENDPOINTS = {
  beheerDetail,
  beheerZoeken: (s) => eventueel(s, 'beheerZoeken', 'Beheer zoeken vereniging A', 'beheer zoeken', `${BEHEER}/zoeken?q=vCode:{{vCodeA}}`, FIND_IN_ZOEKEN),
  publiekDetail: (s) => eventueel(s, 'publiekDetail', 'Publiek detail vereniging A', 'publiek detail', `${PUBLIEK}/{{vCodeA}}`, 'json.vereniging', PUBLIEK_HEADERS),
  publiekZoeken: (s) => eventueel(s, 'publiekZoeken', 'Publiek zoeken vereniging A', 'publiek zoeken', `${PUBLIEK}/zoeken?q=vCode:{{vCodeA}}`, FIND_IN_ZOEKEN, PUBLIEK_HEADERS),
};

// ---------------------------------------------------------------- folders and collection

// scenario = { id, name, kbo?, setup?, a?, registratie? (verwacht), stappen?, historiek?, final?: { <endpoint>: spec } }
function scenarioFolder(feature, s) {
  const setup = s.setup !== undefined ? s.setup : feature.setup || '';
  const items = [s.kbo ? registreerKbo(s.id, 'A', setup) : registreer(s.id, 'A', s.a, { setup, verwacht: s.registratie })];
  (s.stappen || []).forEach((stap) => items.push(stapRequest(s.id, stap)));
  const final = s.final || {};
  if (final.beheerDetail) items.push(beheerDetail(s));
  if (s.historiek) items.push(historiek(s));
  ['beheerZoeken', 'publiekDetail', 'publiekZoeken'].forEach((e) => final[e] && items.push(ENDPOINTS[e](s)));
  return { name: `${s.id} - ${s.name}`, item: items };
}

function controleerIds(feature) {
  const ids = feature.groepen.flatMap((g) => g.scenarios.map((s) => s.id));
  const dubbel = ids.filter((id, i) => ids.indexOf(id) !== i);
  if (dubbel.length) throw new Error(`Dubbele scenario-ids in ${feature.name}: ${dubbel.join(', ')}`);
}

function maakCollectie(feature) {
  controleerIds(feature);
  const rootScript = fs.readFileSync(path.join(__dirname, 'root-prerequest.js'), 'utf8').replace(/\r\n/g, '\n');
  return {
    info: {
      name: feature.name,
      description:
        `${feature.description || ''}\n\nEerste versie gegenereerd met postman/generate.js uit postman/features; wordt verder onderhouden in de Postman workspace. ` +
        'Configuratie via de environment (Local / Test in de Postman workspace), runtime-waarden in collectievariabelen. ' +
        'Elke folder is een zelfstandig scenario.',
      schema: 'https://schema.getpostman.com/json/collection/v2.1.0/collection.json',
    },
    auth: { type: 'bearer', bearer: [{ key: 'token', value: '{{token}}', type: 'string' }] },
    event: [script('prerequest', rootScript)],
    item: feature.groepen.map((g) => ({ name: g.name, item: g.scenarios.map((s) => scenarioFolder(feature, s)) })),
  };
}

module.exports = { maakCollectie, lijstOp, bevatOp, zonderOp, zonderVelden };
