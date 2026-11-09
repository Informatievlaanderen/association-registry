// Generates one Postman collection per feature from postman/features/<feature>/scenarios.js.
// Usage: node postman/generate.js            (all features)
//        node postman/generate.js <feature>  (one or more features)
// Output: postman/collections/<feature>.postman_collection.json

const fs = require('fs');
const path = require('path');
const { maakCollectie } = require('./shared/builders');

const featuresDir = path.join(__dirname, 'features');
const outDir = path.join(__dirname, 'collections');
const gevraagd = process.argv.slice(2);
const features = gevraagd.length
  ? gevraagd
  : fs.readdirSync(featuresDir).filter((f) => fs.existsSync(path.join(featuresDir, f, 'scenarios.js')));

fs.mkdirSync(outDir, { recursive: true });
for (const feature of features) {
  const bestand = path.join(featuresDir, feature, 'scenarios.js');
  if (!fs.existsSync(bestand)) throw new Error(`Onbekende feature: ${feature} (${bestand} bestaat niet)`);
  const collectie = maakCollectie(require(bestand));
  const out = path.join(outDir, `${feature}.postman_collection.json`);
  fs.writeFileSync(out, JSON.stringify(collectie, null, 2) + '\n');
  const aantal = collectie.item.reduce((n, groep) => n + groep.item.length, 0);
  console.log(`${feature}: ${aantal} scenario's -> ${path.relative(process.cwd(), out)}`);
}
