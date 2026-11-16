// Code snippets that are inlined into the generated test scripts.

// Retry the current request (412, not yet projected, ...). Limit: environment maxRetries, delay: retryDelayMs (root script).
const RETRY = `if (typeof loopCounter === 'undefined') { loopCounter = 0; }
if (typeof maxLoopCounter === 'undefined') { maxLoopCounter = 10; }
function retryIf(condition) {
    if (condition && loopCounter < maxLoopCounter) {
        loopCounter++;
        console.log('🔁 ' + pm.info.requestName + ' opnieuw (' + loopCounter + '/' + maxLoopCounter + ')');
        pm.execution.setNextRequest(pm.info.requestName);
        return true;
    }
    loopCounter = 0;
    return false;
}`;

const HELPERS = `const isEmpty = (v) => v === null || v === undefined || v === '';
// case-insensitive property access (iban vs IBAN); '' is the value itself (a response that is a list)
const getCI = (o, k) => {
    if (!o || typeof o !== 'object') return undefined;
    if (k === '') return o;
    if (k in o) return o[k];
    const f = Object.keys(o).find(x => x.toLowerCase() === k.toLowerCase());
    return f === undefined ? undefined : o[f];
};
// iban / insz are compared without spaces, dots and dashes
const norm = (k, v) => typeof v === 'string' && /^(iban|insz)$/i.test(k) ? v.replace(/[^0-9A-Za-z]/g, '').toUpperCase() : v;
// subset match: every expected property matches (null = empty; array of values: exact; array of objects: same
// length and every expected object matches one of the actual ones)
const matches = (actual, subset) => Object.keys(subset).every(k => {
    const e = subset[k];
    const a = getCI(actual, k);
    if (Array.isArray(e)) {
        if (!e.some(x => x !== null && typeof x === 'object')) return JSON.stringify(a) === JSON.stringify(e);
        return Array.isArray(a) && a.length === e.length && e.every(x => a.some(y => matches(y, x)));
    }
    if (e !== null && typeof e === 'object') return matches(a, e);
    if (e === null) return isEmpty(a);
    // a {{variable}} always resolves to a string: '7' matches the number 7
    if (typeof a === 'number' && typeof e === 'string') return String(a) === e;
    return norm(k, a) === norm(k, e);
});
// fills in {{variables}} set earlier in the scenario
const resolve = (o) => JSON.parse(pm.variables.replaceIn(JSON.stringify(o)));
// the vereniging satisfies the spec (used to wait for eventually consistent endpoints); lijst / bevat can be one
// { veld, items } or an array of them
const voldoet = (v, spec) => {
    if (spec.props && !matches(v, spec.props)) return false;
    const lijsten = [].concat(spec.lijst || []);
    if (!lijsten.every(s => { const l = getCI(v, s.veld) || []; return l.length === s.items.length && s.items.every(i => l.some(x => matches(x, i))); })) return false;
    const bevat = [].concat(spec.bevat || []);
    if (!bevat.every(s => { const l = getCI(v, s.veld) || []; return s.items.every(i => l.some(x => matches(x, i))); })) return false;
    // zonder: also waited for (a removed item disappears from an eventually consistent endpoint)
    const zonder = [].concat(spec.zonder || []);
    if (!zonder.every(s => { const l = getCI(v, s.veld) || []; return !s.items.some(i => l.some(x => matches(x, i))); })) return false;
    // minstens: every value of veld (an object or a list) is a number >= waarde (e.g. the progress of every projection)
    const minstens = [].concat(spec.minstens || []);
    if (!minstens.every(s => { const w = getCI(v, s.veld); const waarden = Array.isArray(w) ? w : Object.values(w || {}); return waarden.length > 0 && waarden.every(x => Number(x) >= Number(s.waarde)); })) return false;
    return true;
};
// the forms in which a value may appear in a response (an IBAN with and without spaces or dots)
const vormen = (v) => {
    const s = String(v).replace(/[\\s.]/g, '');
    return /^BE\\d{14}$/.test(s) ? [s, s.replace(/(.{4})(?=.)/g, '$1 '), s.replace(/(.{4})(?=.)/g, '$1.')] : [String(v)];
};`;

module.exports = { RETRY, HELPERS };
