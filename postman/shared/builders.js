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
// A registration also keeps its own VR-Sequence in sequence<rol> (e.g. mutaties?sinds={{sequenceA}}: what came after it).
const storeSequenceVan = (rol) => `if (pm.response.headers.get('VR-Sequence')) pm.collectionVariables.set('sequence${rol}', pm.response.headers.get('VR-Sequence'));
    else pm.collectionVariables.unset('sequence${rol}');`;

// ---------------------------------------------------------------- expected response of a write

// verwacht = { status, detail?, title?, validatie?: { veld, reden, code? } | [...], locatie?, inResponse?, nietInResponse? }
// status: one code, or an array when the start state is unknown (reused vereniging: [200, 202]).
// locatie: the last segment of the Location header (e.g. '{{vCodeA}}' when a registration returns an existing vereniging).
// inResponse / nietInResponse: values that must (not) appear in the response body (e.g. a vCode in the 409 of dubbeldetectie).
function verwachtChecks(label, verwacht) {
  const v = verwacht || { status: 202 };
  const statussen = [].concat(v.status);
  const lines = [
    statussen.length === 1
      ? `pm.test('${title(`${label} geeft ${v.status}`)}', () => pm.expect(pm.response.code, pm.response.text()).to.eql(${statussen[0]}));`
      : `pm.test('${title(`${label} geeft ${statussen.join(' of ')}`)}', () => pm.expect(pm.response.code, pm.response.text()).to.be.oneOf(${q(statussen)}));`,
  ];
  if (v.locatie)
    lines.push(
      `pm.test('${title(`${label}: Location verwijst naar ${kaal(v.locatie)}`)}', () => pm.expect((pm.response.headers.get('Location') || '').split('/').pop()).to.eql(pm.variables.replaceIn(${q(v.locatie)})));`
    );
  if (v.detail)
    lines.push(
      `pm.test('${title(`${label}: detail '${kaal(v.detail)}'`)}', () => pm.expect(pm.response.json().detail).to.eql(pm.variables.replaceIn(${q(v.detail)})));`
    );
  if (v.title)
    lines.push(`pm.test('${title(`${label}: title '${v.title}'`)}', () => pm.expect(pm.response.json().title).to.eql(${q(v.title)}));`);
  (v.inResponse || []).forEach((waarde) =>
    lines.push(
      `pm.test('${title(`${label}: response bevat ${kaal(waarde)}`)}', () => pm.expect(pm.response.text()).to.include(pm.variables.replaceIn(${q(waarde)})));`
    )
  );
  (v.nietInResponse || []).forEach((waarde) =>
    lines.push(
      `pm.test('${title(`${label}: response bevat geen ${kaal(waarde)}`)}', () => {
    const waarde = pm.variables.replaceIn(${q(waarde)});
    pm.expect(waarde, 'variabele niet ingevuld').to.not.include('{{');
    pm.expect(pm.response.text()).to.not.include(waarde);
});`
    )
  );
  [].concat(v.validatie || []).forEach((val) =>
    lines.push(`pm.test('${title(`${label}: ${val.veld}: ${val.reden}`)}', () => {
    const errors = pm.response.json().validationErrors || {};
    const key = Object.keys(errors).find(k => k.toLowerCase() === ${q(val.veld.toLowerCase())});
    pm.expect(key, JSON.stringify(errors)).to.exist;
    const fout = errors[key].find(e => e.reason === ${q(val.reden)});
    pm.expect(fout, JSON.stringify(errors[key])).to.exist;${val.code ? `\n    pm.expect(fout.code).to.eql(${q(val.code)});` : ''}
});`)
  );
  if (statussen.some((s) => s >= 200 && s < 300)) lines.push(STORE_SEQUENCE);
  return lines.join('\n');
}

// ---------------------------------------------------------------- registration

// A scenario stops when its registration (or a lees step) fails: the folder script (stopScript) then skips its other
// requests, which would otherwise run against the vereniging of the previous scenario.
const stopVariabele = (id) => `stop_${id}`;
const kboGebruiktVariabele = (id) => `kboGebruikt_${id}`;
const stop = (id, reden) => `pm.collectionVariables.set('${stopVariabele(id)}', ${reden});`;

// The bevestigingstoken is a hash of the request body, so a retry after 409 must send the same body:
// the naam and the setup values (iban, insz, ...) are only generated on the first attempt.
// url: path after /v1/verenigingen/ (default {{registreerVereniging}} = 'vzer', set by the root script), e.g.
// 'feitelijkeverenigingen' for the old endpoint.
function registreer(id, rol, gegevens = {}, { setup = '', verwacht, url = '{{registreerVereniging}}' } = {}) {
  const tokenKey = `bevestigingsToken_${id}_${rol}`;
  const naamKey = `naam_${id}_${rol}`;
  const succes = !verwacht || verwacht.status === 202;
  const resultaat = succes
    ? `pm.test('${id}: registratie ${rol} geeft 202', () => pm.expect(pm.response.code, pm.response.text()).to.eql(202));
if (pm.response.code === 202) {
    pm.collectionVariables.set('vCode${rol}', (pm.response.headers.get('Location') || '').split('/').pop());
    ${STORE_SEQUENCE}
    ${storeSequenceVan(rol)}
} else {
    ${stop(id, `'registratie ${rol} gaf ' + pm.response.code`)}
}`
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
    pm.collectionVariables.unset('vCode${rol}'); // never the vereniging of a previous scenario
${indent(setup.trimEnd(), 4)}
    pm.collectionVariables.set('${naamKey}', 'Regressie ${id} ${rol} ' + pm.variables.replaceIn('{{$randomUUID}}'));
}`
      ),
      test(`if (pm.response.code === 409) {
    // dubbeldetectie: opnieuw met bevestigingstoken
    pm.collectionVariables.set('${tokenKey}', pm.response.json().bevestigingsToken);
    pm.execution.setNextRequest(pm.info.requestName);
} else {
    // the naam stays available as naam${rol} for the assertions of the scenario
    if (pm.response.code === 202) pm.collectionVariables.set('naam${rol}', pm.collectionVariables.get('${naamKey}'));
    pm.collectionVariables.unset('${tokenKey}');
    pm.collectionVariables.unset('${naamKey}');
${indent(resultaat, 4)}
}`),
    ],
    request: { method: 'POST', header: JSON_HEADERS, body: jsonBody({ naam: `{{${naamKey}}}`, ...gegevens }), url: `${BEHEER}/${url}` },
  };
}

// Vereniging met rechtspersoonlijkheid. kboModus 'random' (local): a new one per run. 'bestaand' (Test): an existing
// vereniging is reused (200), so a scenario may only assert what it adds itself (bevat / zonder, not lijst).
// lijstVariabele: pick the number from this environment variable instead (a comma separated list of KBO numbers with
// specific KBO data, e.g. an e-mail); such a vereniging is always reused, also locally.
// Missing testdata does not throw (the request would not be sent and the scenario would continue on the previous
// vereniging): the registration is sent without a number, fails, the test names the missing variable and the
// scenario stops.
// Several KBO verenigingen in one scenario (K, L, ...) get different numbers: the numbers used so far are kept in
// kboGebruikt_<id> (cleared by the first request of the scenario, see stopScript). When the list runs out the number
// is empty and the registration fails with a clear stop.
function registreerKbo(id, rol = 'A', setup = '', lijstVariabele = null) {
  const gebruikt = kboGebruiktVariabele(id);
  const kies = `const gebruikt = String(pm.collectionVariables.get('${gebruikt}') || '').split(',').filter(Boolean);
${
  lijstVariabele
    ? `const lijst = String(pm.variables.get('${lijstVariabele}') || '').split(',').map(x => x.trim()).filter(x => x && !gebruikt.includes(x));
pm.collectionVariables.set('kboNummer', lijst.length ? lijst[Math.floor(Math.random() * lijst.length)] : '');`
    : `pm.collectionVariables.set('kboNummer', funcs.kboNummer(gebruikt) || '');`
}
pm.collectionVariables.set('${gebruikt}', gebruikt.concat(pm.collectionVariables.get('kboNummer') || []).join(','));`;
  const testdataCheck = lijstVariabele
    ? `pm.test('${id}: ${lijstVariabele} is ingevuld in de environment', () =>
    pm.expect(String(pm.variables.get('${lijstVariabele}') || '').trim(), 'vul ${lijstVariabele} in de environment in').to.not.be.empty);
`
    : '';
  return {
    name: `${id} - Registreer vereniging met rechtspersoonlijkheid ${rol}`,
    event: [
      script(
        'prerequest',
        `${setup.trimEnd()}
pm.collectionVariables.unset('vCode${rol}'); // never the vereniging of a previous scenario
${kies}
// the same number in the notation 0123.456.789
const cijfers = String(pm.collectionVariables.get('kboNummer') || '').replace(/\\D/g, '');
pm.collectionVariables.set('kboNummer_punten', cijfers.slice(0, 4) + '.' + cijfers.slice(4, 7) + '.' + cijfers.slice(7));`
      ),
      test(`${testdataCheck}pm.test('${id}: registratie uit KBO geeft 202 (nieuw) of 200 (bestaand)', () => {
    pm.expect(pm.response.code, pm.response.text()).to.be.oneOf([200, 202]);
});
if (pm.response.code === 200 || pm.response.code === 202) {
    pm.collectionVariables.set('vCode${rol}', (pm.response.headers.get('Location') || '').split('/').pop());
    ${STORE_SEQUENCE}
    ${storeSequenceVan(rol)}
} else {
    ${stop(id, `'registratie uit KBO gaf ' + pm.response.code`)}
}`),
    ],
    request: { method: 'POST', header: JSON_HEADERS, body: jsonBody({ kboNummer: '{{kboNummer}}' }), url: `${BEHEER}/kbo` },
  };
}

// ---------------------------------------------------------------- actions

// stap.lees = { veld, match, bewaar: { <property>: <variabele> }, optioneel?, eerste? }
// Reads the beheer detail of A (expectedSequence, retry on 412) and stores properties of the one item in <veld> that
// matches <match> in collection variables: for ids and values you cannot know beforehand (data from KBO, a reused
// vereniging). Without optioneel exactly one item must match, retried until it does (a value set asynchronously, e.g.
// the adresId after adresmatch); with optioneel the variables are unset when none does (combine with alleenAls on the
// steps that need them); with eerste at least one must match and the first one (lowest id) is stored (e.g. one of the
// vertegenwoordigers from KBO).
function leesRequest(id, stap) {
  const { veld, match, bewaar, optioneel, eerste } = stap.lees;
  const nietKlaar = optioneel ? '' : eerste ? ' || (pm.response.code === 200 && gevonden.length === 0)' : ' || (pm.response.code === 200 && gevonden.length !== 1)';
  return {
    name: `${id} - ${stap.naam}`,
    event: [
      test(`${RETRY}
${HELPERS}
const lijst = pm.response.code === 200 ? getCI((pm.response.json() || {}).vereniging, ${q(veld)}) || [] : [];
const gevonden = lijst.filter(x => matches(x, resolve(${q(match)})));
if (!retryIf(pm.response.code === 412${nietKlaar})) {
    pm.test('${title(`${id} ${stap.naam}: 200`)}', () => pm.response.to.have.status(200));
    const bewaar = ${q(bewaar)};${
      optioneel
        ? ''
        : eerste
          ? `
    pm.test('${title(`${id} ${stap.naam}: minstens 1 ${veld} ${kaal(match)}`)}', () => pm.expect(gevonden.length, JSON.stringify(lijst)).to.be.above(0));
    if (gevonden.length === 0) ${stop(id, `${q(`${stap.naam}: niets gevonden`)}`)}`
          : `
    pm.test('${title(`${id} ${stap.naam}: 1 ${veld} ${kaal(match)}`)}', () => pm.expect(gevonden.length, JSON.stringify(lijst)).to.eql(1));
    if (gevonden.length !== 1) ${stop(id, `${q(`${stap.naam}: `)} + gevonden.length + ' gevonden'`)}`
    }
    Object.keys(bewaar).forEach(k => {
        const waarde = gevonden.length ? getCI(gevonden[0], k) : undefined;
        if (waarde === undefined || waarde === null) pm.collectionVariables.unset(bewaar[k]);
        else pm.collectionVariables.set(bewaar[k], String(waarde));
    });
}`),
    ],
    request: { method: 'GET', header: [], url: `${BEHEER}/{{vCodeA}}?expectedSequence={{sequence}}` },
  };
}

// stap.wachtOp = '<rol>': waits until that vereniging is in beheer detail (expectedSequence, retry on 412/404), e.g.
// before another vereniging refers to it (lidmaatschap), and stores its naam in naam<rol> (also for a KBO vereniging).
function wachtOpRequest(id, stap) {
  const rol = stap.wachtOp;
  return {
    name: `${id} - ${stap.naam || `Wacht op vereniging ${rol}`}`,
    event: [
      test(`${RETRY}
if (!retryIf(pm.response.code === 412 || pm.response.code === 404)) {
    pm.test('${id}: vereniging ${rol} staat in beheer detail', () => pm.expect(pm.response.code, pm.response.text()).to.eql(200));
    if (pm.response.code === 200) pm.collectionVariables.set('naam${rol}', (pm.response.json().vereniging || {}).naam);
    else ${stop(id, `'vereniging ${rol} niet gevonden: ' + pm.response.code`)}
}`),
    ],
    request: { method: 'GET', header: [], url: `${BEHEER}/{{vCode${rol}}}?expectedSequence={{sequence}}` },
  };
}

// stap.wachtOpGebeurtenis = { gebeurtenis, data? }: waits until A's historiek contains that gebeurtenis (subset match on
// data), for a result of asynchronous processing (adresmatch) that the next step depends on. Stops the scenario when it
// does not appear within maxRetries.
function wachtOpGebeurtenisRequest(id, stap) {
  const e = stap.wachtOpGebeurtenis;
  const label = `${e.gebeurtenis}${e.data ? ' ' + kaal(e.data) : ''}`;
  return {
    name: `${id} - ${stap.naam || `Wacht op ${e.gebeurtenis}`}`,
    event: [
      test(`${RETRY}
${HELPERS}
const e = resolve(${q(e)});
const gebeurtenissen = pm.response.code === 200 ? (pm.response.json().gebeurtenissen || []) : [];
const gevonden = gebeurtenissen.some(g => g.gebeurtenis === e.gebeurtenis && (!e.data || matches(g.data, e.data)));
if (!retryIf(pm.response.code === 412 || (pm.response.code === 200 && !gevonden))) {
    pm.test('${title(`${id}: historiek bevat ${label}`)}', () => pm.expect(gevonden, JSON.stringify(gebeurtenissen.map(g => g.gebeurtenis))).to.be.true);
    if (!gevonden) ${stop(id, `${q(`${e.gebeurtenis} niet gevonden`)}`)}
}`),
    ],
    request: { method: 'GET', header: [], url: `${BEHEER}/{{vCodeA}}/historiek?expectedSequence={{sequence}}` },
  };
}

// stap = { naam, actie: 'POST'|'PATCH'|'DELETE'|'GET', pad?, op? ('A'), vCode? (literal), url?, beheerPad?, body?,
//          headers?, zonderToken?, verwacht?, herhaal?, bewaarId?, bewaarHeader?, initiator?, alleenAls? }
//          or { naam, lees } (see leesRequest) or { naam?, wachtOp } (see wachtOpRequest)
//          or { naam?, wachtOpGebeurtenis } (see wachtOpGebeurtenisRequest)
// url: path after /v1/verenigingen/ instead of {vCode}/{pad} (e.g. 'kbo', '{{registreerVereniging}}').
// beheerPad: path after {{beheer_url}}/ for an endpoint outside /v1/verenigingen (e.g. 'v1/projections/...').
// api + apiPad: the request goes to another API ('beheer' | 'publiek' | 'acm', base as in controles) at apiPad, e.g.
// a GET with herhaal that waits until publiek detail shows the vereniging before a step that should remove it.
// headers: extra request headers ({ 'If-Match': '{{etag1}}' }); null removes a header ({ 'X-Correlation-Id': null }).
// bewaarId: store the id from the Location header (last path segment) in this collection variable.
// bewaarHeader: { ETag: 'etag1' } stores response headers of a successful write in collection variables.
// bewaarVeld: { bevestigingsToken: 'token_X' } stores properties of the JSON response root (any status, e.g. a 409).
// initiator: VR-Initiator (OVO code) for this request; the root script sets the default, so it is set in a prerequest.
// alleenAls: the request is skipped when this collection variable is empty (set by an optional lees step).
function stapRequest(id, stap) {
  if (stap.lees) return leesRequest(id, stap);
  if (stap.wachtOp) return wachtOpRequest(id, stap);
  if (stap.wachtOpGebeurtenis) return wachtOpGebeurtenisRequest(id, stap);
  const vCode = stap.vCode || `{{vCode${stap.op || 'A'}}}`;
  let checks = verwachtChecks(`${id} ${stap.naam}`, stap.verwacht);
  if (stap.bewaarId)
    checks += `\nif (pm.response.code === 202) pm.collectionVariables.set('${stap.bewaarId}', (pm.response.headers.get('Location') || '').split('/').pop());`;
  Object.entries(stap.bewaarHeader || {}).forEach(
    ([header, variabele]) =>
      (checks += `\nif (pm.response.code >= 200 && pm.response.code < 300) pm.collectionVariables.set('${variabele}', pm.response.headers.get('${header}'));`)
  );
  // bewaarVeld: { <property of the JSON root>: <variabele> }, whatever the status (e.g. the bevestigingsToken of a 409)
  Object.entries(stap.bewaarVeld || {}).forEach(
    ([veld, variabele]) =>
      (checks += `
{
    let json = {};
    try { json = pm.response.json() || {}; } catch (e) {}
    const k = Object.keys(json).find(x => x.toLowerCase() === ${q(veld.toLowerCase())});
    if (k !== undefined && json[k] !== null) pm.collectionVariables.set('${variabele}', String(json[k]));
    else pm.collectionVariables.unset('${variabele}');
}`)
  );
  const pad = stap.url || `${vCode}${stap.pad ? '/' + stap.pad : ''}`;
  if (stap.api && !CONTROLE_BASIS[stap.api]) throw new Error(`${id}: onbekende api '${stap.api}' in stap '${stap.naam}'`);
  if (stap.api && !stap.apiPad) throw new Error(`${id}: stap '${stap.naam}' met api zonder apiPad`);
  const header = [
    ...(stap.api === 'publiek' ? PUBLIEK_HEADERS : []),
    ...(stap.body === undefined ? [] : JSON_HEADERS),
    ...Object.entries(stap.headers || {})
      .filter(([, value]) => value !== null)
      .map(([key, value]) => ({ key, value })),
  ];
  const url = stap.api
    ? `${CONTROLE_BASIS[stap.api]}/${stap.apiPad}`
    : stap.beheerPad
      ? `{{beheer_url}}/${stap.beheerPad}`
      : `${BEHEER}/${pad}`;
  const request = { method: stap.actie, header, url };
  if (stap.body !== undefined) request.body = jsonBody(stap.body);
  if (stap.zonderToken) request.auth = { type: 'noauth' };
  const pre = [];
  if (stap.alleenAls)
    pre.push(`if (!pm.collectionVariables.get('${stap.alleenAls}')) {
    console.log('⏭️ ' + pm.info.requestName + ' overgeslagen: ${stap.alleenAls} is leeg');
    pm.execution.skipRequest();
}`);
  if (stap.initiator)
    pre.push(`pm.request.headers.upsert({ key: 'VR-Initiator', value: pm.variables.replaceIn(${q(stap.initiator)}) });`);
  // headers: { <header>: null } removes a header the root script sets (VR-Initiator, X-Correlation-Id)
  Object.entries(stap.headers || {})
    .filter(([, value]) => value === null)
    .forEach(([key]) => pre.push(`pm.request.headers.remove(${q(key)});`));
  // herhaal: an outcome that depends on an eventually consistent projection (dubbeldetectie): the request is retried
  // until status, inResponse and nietInResponse hold, then the checks run once.
  if (stap.herhaal) {
    const v = stap.verwacht || { status: 202 };
    checks = `${RETRY}
const klaar = ${q([].concat(v.status))}.includes(pm.response.code)
    && ${q(v.inResponse || [])}.every(w => pm.response.text().includes(pm.variables.replaceIn(w)))
    && !${q(v.nietInResponse || [])}.some(w => pm.response.text().includes(pm.variables.replaceIn(w)));
if (!retryIf(!klaar)) {
${indent(checks, 4)}
}`;
  } else if (
    stap.actie !== 'GET' &&
    !Object.keys(stap.headers || {}).some((k) => k.toLowerCase() === 'if-match') &&
    ![].concat((stap.verwacht || { status: 202 }).status).includes(412)
  ) {
    // A write without If-Match still appends at the version it loaded: a concurrent write on the same vereniging
    // (a reused KBO vereniging on Test, shared with other features) gives 412 and nothing is written, so retry it.
    checks = `${RETRY}
if (!retryIf(pm.response.code === 412)) {
${indent(checks, 4)}
}`;
  }
  const event = [test(checks)];
  if (pre.length) event.unshift(script('prerequest', pre.join('\n')));
  return { name: `${id} - ${stap.naam}`, event, request };
}

// ---------------------------------------------------------------- end state

// spec = { lijst | bevat | zonder: { veld, items: [subset] } (or an array of them, one per veld),
//          props | propsNiet: { veld: waarde }, nietInResponse: [waarde], telling: [{ veld, aantal, waar?, naam? }] }
function specChecks(id, label, spec) {
  // an array of lijst / bevat / zonder specs: one set of checks per veld
  const meerdere = ['lijst', 'bevat', 'zonder'].filter((k) => Array.isArray(spec[k]));
  if (meerdere.length) {
    const delen = [specChecks(id, label, Object.fromEntries(Object.entries(spec).filter(([k]) => !meerdere.includes(k))))];
    meerdere.forEach((k) => spec[k].forEach((deel) => delen.push(`{\n${indent(specChecks(id, label, { [k]: deel }), 4)}\n}`)));
    return delen.join('\n');
  }
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
    // lijst.inVolgorde: the items appear in exactly this order
    if (spec.lijst.inVolgorde)
      lines.push(
        `pm.test('${id} ${label}: ${veld} in volgorde', () => pm.expect(spec.lijst.items.every((x, i) => matches((getCI(vereniging, '${veld}') || [])[i] || {}, x)), JSON.stringify(getCI(vereniging, '${veld}'))).to.be.true);`
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
  // telling: [{ veld, aantal, waar?: { <property>: '<regex>' }, naam? }] -> exactly aantal items in veld (that match waar)
  if (spec.telling)
    spec.telling.forEach((t, i) =>
      lines.push(`pm.test('${title(`${id} ${label}: ${t.naam || t.veld} = ${t.aantal}`)}', () => {
    const t = spec.telling[${i}];
    const waar = t.waar || {};
    const lijst = (getCI(vereniging, t.veld) || []).filter(x => Object.keys(waar).every(k => new RegExp(waar[k]).test(String(getCI(x, k)))));
    pm.expect(lijst.length).to.eql(t.aantal);
});`)
    );
  // minstens: { veld, waarde } (or an array) -> every value of veld (an object or a list) is a number >= waarde
  [].concat(spec.minstens || []).forEach((m, i) =>
    lines.push(`pm.test('${title(`${id} ${label}: elke ${m.veld} minstens ${kaal(m.waarde)}`)}', () => {
    const m = [].concat(spec.minstens)[${i}];
    const w = getCI(vereniging, m.veld) || {};
    const waarden = Array.isArray(w) ? w.map((x, j) => [String(j), x]) : Object.entries(w);
    pm.expect(waarden.length, JSON.stringify(w)).to.be.above(0);
    pm.expect(waarden.filter(([, x]) => !(Number(x) >= Number(m.waarde))), 'kleiner dan ' + m.waarde).to.eql([]);
});`)
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

// Retried on 412 and until the spec holds (props / lijst / bevat): a write can change A in the same transaction as
// another vereniging (markeer D als dubbel van A), so the VR-Sequence of that write does not guarantee A is projected.
function beheerDetail(s) {
  return {
    name: `${s.id} - Beheer detail vereniging A`,
    event: [
      test(`${RETRY}
${HELPERS}
const vereniging = pm.response.code === 200 ? (pm.response.json().vereniging || {}) : {};
const klaar = voldoet(vereniging, resolve(${q(s.final.beheerDetail)}));
if (!retryIf(pm.response.code === 412 || (pm.response.code === 200 && !klaar))) {
    pm.test('${s.id} beheer detail: 200', () => pm.response.to.have.status(200));
${indent(specChecks(s.id, 'beheer detail', s.final.beheerDetail), 4)}
}`),
    ],
    request: { method: 'GET', header: [], url: `${BEHEER}/{{vCodeA}}?expectedSequence={{sequence}}` },
  };
}

// historiek = { bevat: [{ gebeurtenis, data?, beschrijving?, aantal?, naam?, nietIn? }], geen: ['prefix van gebeurtenis'],
//               totaal? }
// data is a subset match; aantal = exact number of matching gebeurtenissen (default: at least one).
// totaal: exact number of gebeurtenissen in the historiek (e.g. 1 after verwijderen, which clears the historiek).
// naam: test title instead of the data (for large data). nietIn: values that must not appear in the matching
// gebeurtenissen (persoonsgegevens; an IBAN is checked in every format).
// Retried on 412 and until every bevat holds (a gebeurtenis written in the same transaction as another vereniging).
function historiek(s) {
  const h = s.historiek;
  const label = (e) => e.naam || `${e.gebeurtenis}${e.data ? ' ' + kaal(e.data) : ''}`;
  const checks = [
    ...(h.bevat || []).flatMap((e, i) => [
      `pm.test('${title(`${s.id} historiek: ${e.aantal !== undefined ? e.aantal + 'x ' : ''}${label(e)}`)}', () => {
    const e = H.bevat[${i}];
    const gevonden = gevondenVoor(e);
    const context = JSON.stringify(gebeurtenissen.filter(g => g.gebeurtenis === e.gebeurtenis));
    if (e.aantal !== undefined) pm.expect(gevonden.length, context).to.eql(e.aantal);
    else pm.expect(gevonden.length, context).to.be.above(0);
    if (e.beschrijving) pm.expect(gevonden.map(g => g.beschrijving), context).to.include(e.beschrijving);
});`,
      ...(e.nietIn || []).map(
        (v, j) => `pm.test('${title(`${s.id} historiek: ${label(e)} bevat geen ${kaal(v)}`)}', () => {
    const waarde = H.bevat[${i}].nietIn[${j}];
    pm.expect(waarde, 'variabele niet ingevuld').to.not.include('{{');
    const tekst = JSON.stringify(gevondenVoor(H.bevat[${i}]));
    vormen(waarde).forEach(vorm => pm.expect(tekst.includes(vorm), vorm).to.be.false);
});`
      ),
    ]),
    ...(h.geen || []).map(
      (prefix) => `pm.test('${title(`${s.id} historiek: geen ${prefix}*`)}', () =>
    pm.expect(gebeurtenissen.filter(g => g.gebeurtenis.startsWith(${q(prefix)})).map(g => g.gebeurtenis)).to.eql([]));`
    ),
    ...(h.totaal === undefined
      ? []
      : [
          `pm.test('${s.id} historiek: in totaal ${h.totaal} gebeurtenis(sen)', () =>
    pm.expect(gebeurtenissen.length, JSON.stringify(gebeurtenissen.map(g => g.gebeurtenis))).to.eql(H.totaal));`,
        ]),
  ];
  return {
    name: `${s.id} - Beheer historiek vereniging A`,
    event: [
      test(`${RETRY}
${HELPERS}
const gebeurtenissen = pm.response.code === 200 ? (pm.response.json().gebeurtenissen || []) : [];
const H = resolve(${q(h)});
const gevondenVoor = (e) => gebeurtenissen.filter(g => g.gebeurtenis === e.gebeurtenis && (!e.data || matches(g.data, e.data)));
const klaar = (H.bevat || []).every(e => (e.aantal !== undefined ? gevondenVoor(e).length === e.aantal : gevondenVoor(e).length > 0))
    && (H.totaal === undefined || gebeurtenissen.length === H.totaal);
if (!retryIf(pm.response.code === 412 || (pm.response.code === 200 && !klaar))) {
    pm.test('${s.id} historiek: 200', () => pm.response.to.have.status(200));
${indent(checks.join('\n'), 4)}
}`),
    ],
    request: { method: 'GET', header: [], url: `${BEHEER}/{{vCodeA}}/historiek?expectedSequence={{sequence}}` },
  };
}

// Search and publiek endpoints have no sequence guard: retry until the vereniging is found and satisfies the spec.
// spec { afwezig: true }: retry until the vereniging is NOT found (publiek detail 404, not in the zoek results).
function eventueel(s, endpoint, naam, label, url, pick, header = []) {
  if (s.final[endpoint].afwezig)
    return {
      name: `${s.id} - ${naam}`,
      event: [
        test(`${RETRY}
const vCodeA = pm.collectionVariables.get('vCodeA');
const json = pm.response.code === 200 ? pm.response.json() : {};
const vereniging = (${pick}) || null;
const afwezig = pm.response.code === 404 || (pm.response.code === 200 && !vereniging);
if (!retryIf(!afwezig)) {
    pm.test('${s.id} ${label}: vereniging niet gevonden', () => pm.expect(afwezig, pm.response.code + ' ' + pm.response.text()).to.be.true);
}`),
      ],
      request: { method: 'GET', header, url },
    };
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

// Extra zoekopdracht: zoek = { api: 'beheer' | 'publiek', q, gevonden } -> vereniging A is (not) in the results.
// Put the 'gevonden: true' queries first: a 'gevonden: false' query is only meaningful once A is projected.
function zoekRequest(s, zoek) {
  const publiek = zoek.api === 'publiek';
  const label = `${zoek.api} zoeken ${kaal(zoek.q)}`;
  return {
    name: `${s.id} - ${publiek ? 'Publiek' : 'Beheer'} zoeken ${zoek.q}`,
    event: [
      test(`${RETRY}
const vCodeA = pm.collectionVariables.get('vCodeA');
const json = pm.response.code === 200 ? pm.response.json() : {};
const gevonden = (json.verenigingen || []).some(v => v.vCode === vCodeA);
if (!retryIf(gevonden !== ${!!zoek.gevonden})) {
    pm.test('${title(`${s.id} ${label}: 200`)}', () => pm.response.to.have.status(200));
    pm.test('${title(`${s.id} ${label}: vereniging ${zoek.gevonden ? '' : 'niet '}gevonden`)}', () => pm.expect(gevonden, pm.response.text()).to.eql(${!!zoek.gevonden}));
}`),
    ],
    request: { method: 'GET', header: publiek ? PUBLIEK_HEADERS : [], url: `${publiek ? PUBLIEK : BEHEER}/zoeken?q=${zoek.q}` },
  };
}

// Extra controle on any endpoint: controle = { naam, pad, spec, api?, body? } -> GET {{beheer_url}}/<pad> (api 'publiek':
// {{publiek_url}}, api 'acm': {{acm_url}}), retried until the response satisfies spec (the same spec as an endpoint in
// final, on the JSON root, e.g. lijstOp('data', [...]) for the locatie lookup). For projections without a sequence guard.
// publiek: true is the same as api: 'publiek'. body: POST with this JSON body instead of GET (e.g. ACM POST
// v1/verenigingen with { insz }).
// headers: { <header>: waarde | null } set in a prerequest, after the root script; null removes the header
// (e.g. { 'VR-Api-Version': null } for the response without the v2 header).
const CONTROLE_BASIS = { beheer: '{{beheer_url}}', publiek: '{{publiek_url}}', acm: '{{acm_url}}' };
function controleRequest(s, controle) {
  const label = controle.naam;
  const api = controle.api || (controle.publiek ? 'publiek' : 'beheer');
  if (!CONTROLE_BASIS[api]) throw new Error(`${s.id}: onbekende api '${api}' in controle '${label}'`);
  const headerScript = Object.entries(controle.headers || {}).map(([key, value]) =>
    value === null
      ? `pm.request.headers.remove(${q(key)});`
      : `pm.request.headers.upsert({ key: ${q(key)}, value: pm.variables.replaceIn(${q(value)}) });`
  );
  return {
    name: `${s.id} - ${controle.naam}`,
    event: [
      ...(headerScript.length ? [script('prerequest', headerScript.join('\n'))] : []),
      test(`${RETRY}
${HELPERS}
const vereniging = pm.response.code === 200 ? pm.response.json() : {}; // the JSON root, checked like a vereniging
const klaar = pm.response.code === 200 && voldoet(vereniging, resolve(${q(controle.spec)}));
if (!retryIf(!klaar)) {
    pm.test('${title(`${s.id} ${label}: 200`)}', () => pm.response.to.have.status(200));
${indent(specChecks(s.id, label, controle.spec), 4)}
}`),
    ],
    request: {
      method: controle.body === undefined ? 'GET' : 'POST',
      header: [...(api === 'publiek' ? PUBLIEK_HEADERS : []), ...(controle.body === undefined ? [] : JSON_HEADERS)],
      url: `${CONTROLE_BASIS[api]}/${controle.pad}`,
      ...(controle.body === undefined ? {} : { body: jsonBody(controle.body) }),
    },
  };
}

// vereist = { <variabele>: waarde }: every request of the scenario is skipped when the environment differs
// (e.g. { kboModus: 'bestaand' } for checks that need the real KBO).
// The skip is visible: the first request of the scenario records one passing test '<id> overgeslagen: <reden>' (a
// skipped request otherwise only shows as "Request could not be sent" in the Postman app), and the folder and
// collection descriptions say on which environment the scenario runs.
const VEREIST_UITLEG = {
  kboModus: { random: 'enkel op Local', bestaand: 'enkel op Test' },
  superAdmin: { true: 'enkel met een superadmin-token' },
};
const omschrijfVereist = (vereist) =>
  Object.entries(vereist)
    .map(([k, v]) => {
      const uitleg = (VEREIST_UITLEG[k] || {})[String(v)];
      return uitleg ? `${uitleg} (${k} = ${v})` : `enkel als ${k} = ${v}`;
    })
    .join(' en ');
const vereistScript = (vereist, id, eersteRequest) => `const vereist = ${q(vereist)};
const anders = Object.keys(vereist).filter(k => String(pm.variables.get(k)) !== String(vereist[k]));
if (anders.length) {
    const nu = anders.map(k => k + ' = ' + (pm.variables.get(k) === undefined ? '(niet ingevuld)' : pm.variables.get(k))).join(', ');
    if (pm.info.requestName === ${q(eersteRequest)})
        pm.test(${q(`${id} overgeslagen: draait ${omschrijfVereist(vereist)}`)} + ' (deze environment: ' + nu + ')', () => {});
    console.log('⏭️ ' + pm.info.requestName + ' overgeslagen, vereist ' + anders.map(k => k + '=' + vereist[k]).join(', '));
    pm.execution.skipRequest();
}`;

// Folder script of a scenario with a registration: the registration clears the stop flag (a new attempt), every other
// request of the scenario is skipped once the registration or a lees step has set it.
// The first skipped request records one passing test '<id> overgeslagen: <reden>', so the report says why the rest of
// the scenario has no results (the cause itself fails at its own request).
const stopScript = (id, registratieNaam) => `if (pm.info.requestName === ${q(registratieNaam)}) {
    pm.collectionVariables.unset('${stopVariabele(id)}');
    pm.collectionVariables.unset('${stopVariabele(id)}_gemeld');
    pm.collectionVariables.unset('${kboGebruiktVariabele(id)}');
} else if (pm.collectionVariables.get('${stopVariabele(id)}')) {
    if (!pm.collectionVariables.get('${stopVariabele(id)}_gemeld')) {
        pm.collectionVariables.set('${stopVariabele(id)}_gemeld', 'ja');
        pm.test(${q(`${id} overgeslagen: `)} + pm.collectionVariables.get('${stopVariabele(id)}'), () => {});
    }
    console.log('⏭️ ' + pm.info.requestName + ' overgeslagen: ' + pm.collectionVariables.get('${stopVariabele(id)}'));
    pm.execution.skipRequest();
}`;

// ---------------------------------------------------------------- folders and collection

// registratieUrl: path of the registration of A after /v1/verenigingen/ (default {{registreerVereniging}} = 'vzer'),
// e.g. 'feitelijkeverenigingen' for the old endpoint.
// scenario = { id, name, kbo?, geenRegistratie?, vereist?, setup?, a?, registratie? (verwacht), registratieUrl?, verenigingen?, stappen?,
//              historiek?, final?: { <endpoint>: spec }, zoeken?: [zoek], controles?: [controle] }
// kbo: true (funcs.kboNummer) or the name of an environment variable with KBO numbers to pick from (see registreerKbo).
// geenRegistratie: no vereniging A (scenarios that only send requests that must be refused).
// verenigingen: { <rol>: registration body | { kbo: true | '<lijstVariabele>' } }: extra verenigingen (D, X, ...),
// registered after A in this order, before the stappen; vCode in vCode<rol>, naam in naam<rol>.
function scenarioFolder(feature, s) {
  const setup = s.setup !== undefined ? s.setup : feature.setup || '';
  const items = [];
  if (!s.geenRegistratie)
    items.push(
      s.kbo
        ? registreerKbo(s.id, 'A', setup, typeof s.kbo === 'string' ? s.kbo : null)
        : registreer(s.id, 'A', s.a, { setup, verwacht: s.registratie, url: s.registratieUrl })
    );
  Object.entries(s.verenigingen || {}).forEach(([rol, gegevens]) =>
    items.push(
      gegevens && gegevens.kbo
        ? registreerKbo(s.id, rol, '', typeof gegevens.kbo === 'string' ? gegevens.kbo : null)
        : registreer(s.id, rol, gegevens)
    )
  );
  const registraties = items.length;
  (s.stappen || []).forEach((stap) => items.push(stapRequest(s.id, stap)));
  const final = s.final || {};
  if (final.beheerDetail) items.push(beheerDetail(s));
  if (s.historiek) items.push(historiek(s));
  ['beheerZoeken', 'publiekDetail', 'publiekZoeken'].forEach((e) => final[e] && items.push(ENDPOINTS[e](s)));
  (s.zoeken || []).forEach((zoek) => items.push(zoekRequest(s, zoek)));
  (s.controles || []).forEach((controle) => items.push(controleRequest(s, controle)));
  const folder = { name: `${s.id} - ${s.name}`, item: items };
  if (s.vereist) folder.description = `Draait ${omschrijfVereist(s.vereist)}; op andere environments wordt dit scenario overgeslagen.`;
  const folderScript = [
    s.vereist && items.length && vereistScript(s.vereist, s.id, items[0].name),
    registraties > 0 && stopScript(s.id, items[0].name),
  ].filter(Boolean);
  if (folderScript.length) folder.event = [script('prerequest', folderScript.join('\n'))];
  return folder;
}

function controleerIds(feature) {
  const ids = feature.groepen.flatMap((g) => g.scenarios.map((s) => s.id));
  const dubbel = ids.filter((id, i) => ids.indexOf(id) !== i);
  if (dubbel.length) throw new Error(`Dubbele scenario-ids in ${feature.name}: ${dubbel.join(', ')}`);
}

// Overview of the scenarios that only run on some environments, per condition (for the collection description and the
// output of generate.js).
function omgevingsOverzicht(feature) {
  const perVoorwaarde = {};
  feature.groepen.forEach((g) =>
    g.scenarios
      .filter((s) => s.vereist)
      .forEach((s) => (perVoorwaarde[omschrijfVereist(s.vereist)] = (perVoorwaarde[omschrijfVereist(s.vereist)] || []).concat(s.id)))
  );
  return Object.entries(perVoorwaarde).map(([voorwaarde, ids]) => `- ${voorwaarde[0].toUpperCase()}${voorwaarde.slice(1)}: ${ids.join(', ')}`);
}

function maakCollectie(feature) {
  controleerIds(feature);
  const rootScript = fs.readFileSync(path.join(__dirname, 'root-prerequest.js'), 'utf8').replace(/\r\n/g, '\n');
  const overzicht = omgevingsOverzicht(feature);
  return {
    info: {
      name: feature.name,
      description:
        `${feature.description || ''}\n\nEerste versie gegenereerd met postman/generate.js uit postman/features; wordt verder onderhouden in de Postman workspace. ` +
        'Configuratie via de environment (Local / Test in de Postman workspace), runtime-waarden in collectievariabelen. ' +
        'Elke folder is een zelfstandig scenario.' +
        (overzicht.length
          ? `\n\nScenario's die niet op elke environment draaien (elders overgeslagen, met een test '<id> overgeslagen: ...'):\n${overzicht.join('\n')}`
          : '\n\nAlle scenario\'s draaien op elke environment.'),
      schema: 'https://schema.getpostman.com/json/collection/v2.1.0/collection.json',
    },
    auth: { type: 'bearer', bearer: [{ key: 'token', value: '{{token}}', type: 'string' }] },
    event: [script('prerequest', rootScript)],
    item: feature.groepen.map((g) => ({ name: g.name, item: g.scenarios.map((s) => scenarioFolder(feature, s)) })),
  };
}

module.exports = { maakCollectie, omgevingsOverzicht, lijstOp, bevatOp, zonderOp, zonderVelden };
