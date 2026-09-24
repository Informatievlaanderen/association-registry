# Association Registry (Verenigingsregister)

# Part 1 — Domain

## What This Is

The Association Registry (Verenigingsregister) for Digitaal Vlaanderen (Flemish government).
It is the authoritative source (basisregister) for associations (verenigingen) in Flanders,
originally 'feitelijke verenigingen'. Associations with legal personality are linked to
KBO and kept in sync.

## Domain Language

This project uses Dutch domain terms. Do not translate them to English.

- **Vereniging**: Association — the central aggregate
- **VCode**: Unique identifier of a vereniging <!-- CHECK: document the exact format -->
- **Feitelijke vereniging / Vereniging zonder eigen rechtspersoonlijkheid (VZER)**: Association without legal personality; feitelijke verenigingen are migrated to VZER
- **Vereniging met rechtspersoonlijkheid**: Association with legal personality, sourced from KBO
- **KBO-nummer**: Enterprise number in the Kruispuntbank van Ondernemingen
- **Vertegenwoordiger**: Representative of a vereniging (personal data, identified by INSZ)
- **Contactgegeven, Locatie, Bankrekeningnummer**: Contact details, locations, bank accounts
- **Hoofdactiviteit, Werkingsgebied, Doelgroep**: Main activity, area of operation, target group
- **Lidmaatschap**: Membership of a vereniging in another vereniging
- **Erkenning**: Official recognition of a vereniging (registered, suspended, expired, ...)
- **Stopzetting**: Discontinuation of a vereniging
- **Bewaartermijn**: Retention period for personal data
- **DubbelDetectie**: Duplicate detection on registration
- **Decentraal beheer**: Management of verenigingen by local governments (beheer)
- **Beheer / Publiek**: Admin side vs. public side (APIs, projections)

## Domain Model

- Aggregate `Vereniging` in `src/AssociationRegistry/DecentraalBeheer/Vereniging/`
- Sub-concepts, each in its own folder there: Bankrekeningen, Bewaartermijnen, ContactGegevens,
  Datums, Doelgroepen, DubbelDetectie, Erkenningen, Hoofdactiviteiten, INSZ, InStopzetting,
  KboNummers, Lidmaatschappen, Locaties, VCodes, VerenigingsNamen, Verenigingstypes,
  Verenigingssubtypes, Vertegenwoordigers, Werkingsgebieden
- Value objects are used within the domain boundary (between request and event)

## Business Rules

- Events are the source of truth and are append-only
- Data taken over from KBO is managed by KBO (see `*InBeheerGenomenDoorKbo`, `*UitKBO` events)
- Addresses are matched against the Adressenregister (GRAR) and kept in sync
- Personal data of vertegenwoordigers is subject to a bewaartermijn
- TODO: registration invariants, rules per verenigingstype, erkenning lifecycle rules

## External Sources & Integrations

- **KBO** (via MAGDA): Sync of verenigingen met rechtspersoonlijkheid (`KboMutations`, `SyncLambda`, `MagdaSync/SyncKbo`)
- **KSZ** (via MAGDA): Verification of vertegenwoordigers (`MagdaSync/SyncKsz`)
- **GRAR / Adressenregister**: Address matching and updates (Kafka consumer + nightly sync)
- **Wegwijs**: Directory services
- **IPDC**: Product/service catalogue integration <!-- CHECK: purpose -->
- **Slack**: Operational notifications
- **ACM/IDM**: Authentication

# Part 2 — Technical

## Architecture

### Event Sourcing (this is the core pattern)

All state changes are stored as immutable events in PostgreSQL via Marten.
Events are NEVER modified or deleted. They are the source of truth.

- Events are Dutch, past tense with `Werd`/`Werden`: `VerenigingWerdErkend`, `LidmaatschapWerdToegevoegd`
- Events are records implementing `IEvent`, in `src/AssociationRegistry/Events/`
- Aggregates are reconstituted from their event stream

### CQRS

- **Command side**: HTTP request → Admin API endpoint → Wolverine → command handler → `Vereniging` → events in Marten
- **Query side**: events → projection hosts → read models (Marten documents + Elasticsearch)

### Hosts

- **Admin.Api**: Beheer API (commands and admin queries)
- **Public.Api**: Public read API
- **Acm.Api**: API for ACM
- **Admin.ProjectionHost / Public.ProjectionHost**: Build admin and public read models
- **Scheduled.Host**: Scheduled jobs (Quartz)
- **KboMutations.SyncLambda**: AWS Lambda processing KBO mutations from SQS
- **Admin.AddressSync**: Address synchronisation with GRAR

## Tech Stack

- .NET 9 (see `global.json`), C#
- PostgreSQL 15 (event store + documents) via Marten / JasperFx.Events
- WolverineFx (messaging, command handling, Amazon SQS)
- Elasticsearch 8 (search projections)
- AWS: SQS, S3, DynamoDB, SSM, Lambda (LocalStack/MiniStack locally)
- Kafka (GRAR consumer), Quartz (scheduling)
- Be.Vlaanderen.Basisregisters.* libraries
- Serilog + OpenTelemetry (Grafana, Loki, Tempo, Prometheus locally)
- Central package management via `Directory.Packages.props` <!-- CHECK: build.sh still runs paket restore -->
- xUnit v3, AutoFixture, FluentAssertions, Moq, Alba

## Project Structure

```
src/
  AssociationRegistry/                       # Core domain (Vereniging aggregate, events, value objects)
  AssociationRegistry.CommandHandling/       # Command handlers per context (DecentraalBeheer, Grar, MagdaSync, ...)
  AssociationRegistry.Admin.Api/             # Beheer API
  AssociationRegistry.Public.Api/            # Public API
  AssociationRegistry.Acm.Api/               # ACM API
  AssociationRegistry.*.ProjectionHost(.Projections)/  # Admin and public projections
  AssociationRegistry.*.Schema/              # Read model schemas / documents
  AssociationRegistry.MartenDb/              # Marten setup
  AssociationRegistry.KboMutations(.SyncLambda)/  # KBO synchronisation
  AssociationRegistry.Scheduled.Host/        # Scheduled jobs
  AssociationRegistry.Admin.AddressSync/     # GRAR address sync
  integrations/                              # Grar, Ipdc, Magda, Slack, Wegwijs clients
test/
  AssociationRegistry.Test/                  # Domain unit tests
  AssociationRegistry.Test.Admin.Api/        # Admin API tests
  AssociationRegistry.Test.Public.Api/       # Public API tests
  AssociationRegistry.Test.Acm.Api/          # ACM API tests
  AssociationRegistry.Test.Projections/      # Projection tests
  AssociationRegistry.Test.E2E/              # End-to-end tests
  AssociationRegistry.Test.Common/           # Shared test utilities
```

## Build & Test

```bash
# Start dependencies (Postgres, Elasticsearch, ACM, LocalStack, ...)
docker compose up -d

# Stop and remove volumes
docker compose down -v

# Full build + tests (FAKE pipeline)
./build.sh

# Build / test with dotnet
dotnet build AssociationRegistry.sln
dotnet test

# Run specific test class
dotnet test test/AssociationRegistry.Test/AssociationRegistry.Test.csproj \
  --filter "FullyQualifiedName~Namespace.ClassName"

# Run everything locally including services
./run_local.sh

# Commit with commitizen
npm run commit
```

## Patterns

### Commands

- Handled via Wolverine in `src/AssociationRegistry.CommandHandling/<Context>/Acties/`
- Endpoints live in `src/AssociationRegistry.Admin.Api/WebApi/<Context>/`
- Requests are mapped to value objects before reaching the domain

### Events

- Dutch, past tense with `Werd`/`Werden`: `ErkenningWerdGeregistreerd`
- Records implementing `IEvent`, in `src/AssociationRegistry/Events/`
- Versioned by suffix when the schema changes: `BewaartermijnWerdGestartV2`

## Persistence Rules

### Database record classes (`*Document`)

- **No nullable primitives** on Marten `*Document` classes.
  This applies to: `bool?`, `int?`, `DateOnly?`, `DateTime?`, `decimal?`, and any other value-type nullable property.
- Instead of a nullable primitive, use a default value or model absence as a separate type.
- **Only exception:** `DateTimeOffset? DeletedAt` for soft-delete (Marten `ISoftDeleted`).

### Migrations

- Schema migrations live in `migrations/local` and `migrations/production`
- Golden-master migration checks run via `docker-compose.migrations.yml` <!-- CHECK: confirm workflow -->

## Code Conventions

- Value objects within the domain boundary (between request and event)
- JSON responses: strings are empty or have a value (never `null`), collections are empty or have values (never `null`), objects may be `null`
- `dotnet format` runs on staged `.cs` files (husky pre-commit)
- Git commits follow conventional commits, enforced by husky: `fix: or-1234 allow X in Y`
- AI assistants must NOT add attribution or Co-Authored-By tags to commit messages

## Testing

- xUnit v3 for all tests
- AutoFixture to generate random valid values for value objects
- FluentAssertions for assertions, Moq for mocks, Alba for HTTP-level API tests
- Shared helpers live in `AssociationRegistry.Test.Common`
- TODO: conventions for event-based domain tests and projection tests

## Security & Authorization

- Authentication via ACM/IDM (OAuth2 introspection / JWT bearer)
- Local fake identity server config: `identityserver/acm.json`, `identityserver/vr.json`
- TODO: authorization attributes and policies

# Part 3 — Working Agreements

## Workflow

Make frequent and small commits following conventional commits format.

When adding new domain functionality: <!-- CHECK: inferred from repo structure -->
1. Define the event in `src/AssociationRegistry/Events/`
2. Add the behaviour to the `Vereniging` aggregate
3. Add the command and handler in `AssociationRegistry.CommandHandling`
4. Add the endpoint in `AssociationRegistry.Admin.Api/WebApi/`
5. Update admin and public projections
6. Add tests

## Guardrails

Shared baseline (identical in OR and VR):
- NEVER modify or delete existing events — event sourcing means append-only
- Do not introduce new packages without discussion
- Commits follow conventional commits; make small, frequent commits
- AI assistants must NOT add attribution or Co-Authored-By tags to commit messages

Repository-specific:
- NEVER add nullable primitive properties to Marten `*Document` classes (only exception: `DateTimeOffset? DeletedAt`)
- NEVER return `null` for strings or collections in API responses
