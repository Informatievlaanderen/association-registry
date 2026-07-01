# Association Registry [![Main](https://github.com/Informatievlaanderen/association-registry/actions/workflows/main.yml/badge.svg)](https://github.com/Informatievlaanderen/association-registry/actions/workflows/main.yml)

## Goal

> Authentic association registry containing 'feitelijke verenigingen' in Flanders.

Please see our [contributing guidelines](CONTRIBUTING.md) before contributing.

## Required tools

- dotnet sdk (see `global.json` for exact version)
- docker compose

## Setup

Working with docker containers :

### Start docker containers

```~~~~
docker compose up -d
```

### Stop docker containers

```~~~~
docker compose down -v
```

## Conventions

### Json --- responses

- strings are empty or have a value (never NULL)
- collections are empty or have values (never NULL)
- objects can be NULL

## WIP — OR-3248 (temporary)

### Current state

- **Beheer zoeken** is fixed: `IsErkend` is now set from `VerenigingWerdErkend` / `VerenigingWerdNietLangerErkend`.
- Admin zoeken projection tests (`Beheer.Zoeken.Erkenningen`): **13/13 passing**.

### TODO

1. Fix domain event emission for the verlopen transition so `VerenigingWerdNietLangerErkend` is emitted when no active erkenning remains.
2. Stabilize command handler status-transition tests (`When_Erkenning_Werd_Verlopen`, `When_Hef_Schorsing_Erkenning_Op`).
3. Fix Admin API search integration tests (`When_Searching_An_Erkende_Vereniging`) — indexed documents currently return `IsErkend = false`.
4. Add no-op projection handlers for `VerenigingWerdErkend` / `VerenigingWerdNietLangerErkend` in Detail, Historiek, PowerBi and sequence projections (sanity checks currently fail).
5. Decide whether to apply the same `IsErkend` logic to **Publiek zoeken**.

