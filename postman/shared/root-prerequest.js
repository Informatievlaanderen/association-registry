// Root pre-request script of every regression collection (copied into each collection by postman/generate.js).
// Configuration comes from the environment (Local / Test in the Postman workspace): this script never looks at the environment name.
// Runtime state lives in collection variables (token, vCode*, sequence, ...), never in the environment.
const uuid = require('uuid');

pm.request.headers.upsert({ key: 'VR-Initiator', value: pm.variables.get('initiator') || 'Postman' });
pm.request.headers.upsert({ key: 'VR-Api-Version', value: 'v2' });
pm.request.headers.upsert({ key: 'X-Correlation-Id', value: uuid.v4() });
pm.variables.set('registreerVereniging', 'feitelijkeverenigingen');

// One-time initialisation per run
if (typeof regressieInit === 'undefined') {
    regressieInit = true;
    console.log('🚀 Regressie: ' + pm.info.requestName);

    // retry state, used by retryIf() in the test scripts
    loopCounter = 0;
    maxLoopCounter = Number(pm.variables.get('maxRetries') || 10);

    funcs = {
        pickOne: (arr) => arr[Math.floor(Math.random() * arr.length)],
        // valid (random) KBO number: the Magda mock accepts any valid number
        randomKBO: () => {
            const start = Math.floor(Math.random() * 99999998) + 1;
            return String(start).padStart(8, '0') + String(97 - (start % 97)).padStart(2, '0');
        },
        // kboModus 'random' (local) or 'bestaand' (Test: one of testdata_kboNummers, an existing vereniging is reused)
        kboNummer: () =>
            (pm.variables.get('kboModus') || 'random') === 'random'
                ? funcs.randomKBO()
                : funcs.pickOne(String(pm.variables.get('testdata_kboNummers') || '').split(',').map((x) => x.trim()).filter(Boolean)),
    };

    // Token: the secrets are a plain value (Local, --env-var in GitHub) or a {{vault:...}} reference (Test in the app).
    // Reading the vault from a script requires "vault in scripts" to be enabled in Postman; without it pm.vault.get
    // never answers, so it gets a timeout and a clear message instead of hanging the run.
    const metTimeout = (promise, ms, wat) =>
        Promise.race([promise, new Promise((_, reject) => setTimeout(() => reject(new Error(wat + ': geen antwoord na ' + ms + ' ms')), ms))]);
    // pm.vault.get is awaited at the top level of the script, as in Postman's documentation, one secret at a time.
    const geheim = async (naam) => {
        const waarde = pm.variables.get(naam) || '';
        const vault = /^\{\{vault:(.+)\}\}$/.exec(waarde);
        if (!vault) return waarde;
        const uitVault = await metTimeout(pm.vault.get(vault[1]), 5000, 'vault ' + vault[1]);
        if (!uitVault) throw new Error('vault ' + vault[1] + ' is leeg of bestaat niet');
        return uitVault;
    };

    // stop at once on missing configuration, instead of running every request without a token
    const stop = (bericht) => {
        console.log('❌ ' + bericht + ' De run wordt gestopt.');
        pm.execution.setNextRequest(null);
        pm.execution.skipRequest();
    };
    const ontbrekend = ['beheer_url', 'publiek_url', 'token_url', 'clientId', 'clientSecret'].filter((k) => !pm.variables.get(k));
    if (ontbrekend.length) stop('Ontbrekende variabelen in de environment: ' + ontbrekend.join(', ') + '.');

    let clientId, clientSecret;
    if (!ontbrekend.length) try {
        clientId = await geheim('clientId');
        clientSecret = await geheim('clientSecret');
        // vr-api-key header of the publiek requests (see builders.js): the vault secret vr-api-key,
        // or a variable vr-api-key when there is no vault (--env-var in GitHub)
        const apiKey = pm.variables.get('vr-api-key') || (await metTimeout(pm.vault.get('vr-api-key'), 5000, 'vault vr-api-key'));
        if (!apiKey) throw new Error('vault vr-api-key is leeg of bestaat niet');
        pm.collectionVariables.set('publiekApiKey', apiKey);
    } catch (error) {
        stop('Geheimen niet gelezen: ' + error.message +
            '. Zet in Postman "vault in scripts" aan (Settings > Vault), of vul clientId/clientSecret/vr-api-key als current value in de environment.');
    }

    if (clientId && clientSecret) {
        console.log('🔐 Geheimen gelezen, token aanvragen bij ' + pm.variables.get('token_url'));
        try {
            const response = await new Promise((resolve, reject) =>
                pm.sendRequest(
                    {
                        url: pm.variables.get('token_url'),
                        method: 'POST',
                        header: { Authorization: 'Basic ' + Buffer.from(clientId + ':' + clientSecret).toString('base64') },
                        body: {
                            mode: 'urlencoded',
                            urlencoded: [
                                { key: 'grant_type', value: 'client_credentials' },
                                { key: 'scope', value: 'dv_verenigingsregister_beheer dv_verenigingsregister_hoofdvertegenwoordigers' },
                            ],
                        },
                    },
                    (error, response) => (error ? reject(error) : resolve(response))
                )
            );
            if (response.code !== 200) throw new Error(response.code + ' ' + response.text());
            pm.collectionVariables.set('token', response.json().access_token);
            console.log('🔑 Token ontvangen');
        } catch (error) {
            stop('Token aanvragen mislukt: ' + error.message + '.');
        }
    }
}

// a retried request (retryIf) waits first
if (loopCounter > 0) {
    setTimeout(() => {}, Number(pm.variables.get('retryDelayMs') || 800));
}
