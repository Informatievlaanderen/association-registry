# OR-3070 .NET 10 Migratie Update

Deze nota legt uit waarom de .NET 10 migratiecommit meer bestanden raakt dan
alleen een eenvoudige target-framework bump.

## Scope

Het ticket migreert de solution van .NET 9 naar .NET 10 LTS en behoudt centrale
package-versionering via `Directory.Packages.props`.

De migratiecommit is:

```text
be44d15ed chore: or-3070 upgrade dotnet 10 with central package versioning
```

## Waarom de diff breed is

Deze codebase hangt sterk af van Marten, Wolverine, JasperFx, ASP.NET Core, AWS
SDK packages, test hosts, gegenereerde service metadata en Docker build images.
Om naar .NET 10 te kunnen gaan, moesten die package families ook naar versies die
het nieuwe target framework ondersteunen. Enkele van die upgrades hebben
breaking API changes, waardoor er compatibiliteitsaanpassingen nodig waren in
startupcode, projecties, testfixtures en E2E setup.

## Noodzakelijke Wijzigingsgroepen

### Target framework en runtime referenties

Alle projecten zijn naar `net10.0` gezet, `global.json` is bijgewerkt naar de
.NET 10 SDK, en Docker images zijn aangepast naar .NET 10 SDK/runtime images.
Zonder die wijzigingen zouden lokale builds, container builds en publish scripts
nog tegen de oude runtime compileren of draaien.

### Centrale package-versionering

Packageversies blijven centraal beheerd in `Directory.Packages.props`, volgens
dezelfde package-versioning architectuur als de gerefereerde
organisation-registry PR. Inline `PackageReference Version=...` entries zijn
vermeden.

Enkele oude Paket-referenties in Docker/lambda buildpaden zijn verwijderd omdat
er geen Paket dependency files meer zijn voor project package restore.
`COPY paket.*` zou Docker builds breken, en `dotnet paket restore` was niet meer
het juiste restorepad voor de lambda package build.

### Marten 9 en JasperFx wijzigingen

De Marten upgrade vroeg meer dan packageversies aanpassen:

- Marten 9 gebruikt nieuwere JasperFx APIs, waardoor `OaktonEnvironment` is
  vervangen door `JasperFxEnvironment`.
- Sommige APIs ondersteunen alleen nog async access, dus synchrone
  Marten-query/event toegang is waar nodig omgezet naar async access.
- Event- en projection-APIs zijn gewijzigd, waardoor projection classes en test
  event doubles moesten aansluiten op de nieuwe interfaces.
- Schema setup moest expliciet gemaakt worden in tests en E2E fixtures, omdat
  runtime hosts `AutoCreate.None` moeten behouden terwijl geisoleerde
  testdatabases nog altijd schema-aanmaak nodig hebben.
- Event stream creation moest de actuele Marten append/start-stream APIs
  gebruiken.

Deze wijzigingen verklaren veel edits in projection classes, Marten setup
files, testfixtures en `TestEvent` helpers.

### Wolverine 6 compatibiliteit

Wolverine is samen met Marten/JasperFx geupgraded. Dat vroeg expliciete runtime
compilation references voor hosts die handlers genereren, aangepaste startup
integratie en een service-location policy aanpassing voor bestaande handler
patronen. Zonder die wijzigingen faalden hosts tijdens startup/codegen.

### ASP.NET en API versioning compatibiliteit

ASP.NET Core package references zijn bijgewerkt naar .NET 10-compatibele
versies. De API versioning packages zijn ook gewijzigd, waardoor sommige API
projecten nu rechtstreeks `Asp.Versioning.Mvc.ApiExplorer` refereren om runtime
method/attribute loading failures te vermijden.

### Projectie- en zoekgedrag

De Elastic client/package upgrades maakten enkele search-indexing aannames
zichtbaar:

- Search consumers indexeren documenten nu met deterministische IDs op basis van
  `VCode`, in plaats van te vertrouwen op gegenereerde IDs.
- Search projection handlers ondersteunen registratie-event varianten zonder
  persoonsgegevens, omdat die varianten in bestaande scenarios voorkomen en nog
  altijd zoekdocumenten moeten opleveren.

Deze wijzigingen waren nodig om de E2E search scenarios groen te krijgen na de
package upgrade.

### Testfixture en E2E infrastructuur

De E2E- en integratiefixtures hadden expliciete setup nodig voor:

- gekloonde PostgreSQL testdatabases;
- Marten schema application;
- geisoleerde public/admin projection schemas;
- Elasticsearch index cleanup;
- LocalStack S3 access in lokale tests.

Deze updates horen bij de migratie omdat nieuwere Marten/Wolverine versies
strikter zijn rond schema validation en startup behavior.

### AWS SDK en LocalStack gedrag

AWS SDK package updates wijzigden lokaal signing/presigned URL gedrag genoeg dat
het LocalStack S3 pad in de public detail-all client een directe lokale URL nodig
had. Het productiepad voor S3 blijft ongewijzigd.

### Magda persoon lookup sequencing

Een deel van de parallelle Magda lookup code gebruikte een gedeelde scoped
Marten session. Na de Marten async-only wijzigingen werd dat onveilig omdat
dezelfde session concurrent gebruikt kon worden. De lookup is sequentieel
gemaakt om het bestaande scoped-session model stabiel te houden.

## Validatie Samenvatting

Voor de migratie was de volledige test suite groen:

```text
4181 passed, 10 skipped, 0 failed
```

Na de migratie:

- de solution buildde succesvol voor de latere formatting cleanup;
- geisoleerde E2E tests waren groen met `526 passed, 4 skipped, 0 failed`;
- de hoofd-APIs, projection hosts en scheduled host startten succesvol onder
  .NET 10 na het toepassen van lokale Marten schema changes;
- `Admin.AddressSync` faalt nog op dezelfde scoped-service lifetime validation
  issue die al bestond voor de migratie.

De finale formatting-cleanup amend verwijderde onnodige formatting noise uit de
commit. Een volledige rebuild na die cleanup is twee keer geprobeerd, maar
MSBuild bleef hangen en moest na ongeveer 10 minuten geannuleerd worden. De
geannuleerde runs rapporteerden `0 Error(s)`.

## Niet Opgenomen

`.secrets.baseline` is nog altijd een unstaged lokale wijziging en is bewust
geen onderdeel van de .NET 10 migratiecommit.
