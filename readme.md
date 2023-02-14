# Association Registry

## Goal

> Authentic association registry containing 'feitelijke verenigingen' in Flanders.

Please see our [contributing guidelines](CONTRIBUTING.md) before contributing.

## Required tools
- dotnet sdk (see `global.json` for exact version)
- docker compose

## Setup
### Start docker containers
```~~~~
docker compose up
```

## Conventions
### Json --- responses
- strings are empty or have a value (never NULL)
- collections are empty or have values (never NULL)
- objects can be NULL
